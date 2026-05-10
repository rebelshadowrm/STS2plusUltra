using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Audio;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves;
using STS2Plus.Config;
using STS2Plus.Reflection;
using STS2Plus.Ui;

namespace STS2Plus.Patches;

[HarmonyPatchCategory("Core")]
[HarmonyPatch(typeof(NGame), "_Input")]
internal static class QuickRestartPatch
{
	private static bool isRestarting;

	private static void Prefix(InputEvent inputEvent)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Invalid comparison between Unknown and I8
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Invalid comparison between Unknown and I8
		InputEventKey val = (InputEventKey)(object)((inputEvent is InputEventKey) ? inputEvent : null);
		if (val != null && val.Pressed && !val.Echo && ((long)val.Keycode == 4194336 || (long)val.PhysicalKeycode == 4194336) && !isRestarting && ConfigManager.Current.QuickRestartEnabled && GameReflection.IsRunActive())
		{
			ModEntry.Verbose("QuickRestart: F5 triggered");
			if (GameReflection.IsMultiplayerRun())
			{
				MultiplayerQuickRestartCoordinator.RequestRestart();
			}
			else
			{
				DoQuickRestart();
			}
		}
	}

	private static async Task DoQuickRestart()
	{
		isRestarting = true;
		SpeedControlOverlay.SuspendGameplaySpeed(suspended: true);
		bool toreDownCurrentRun = false;
		NGame game = null;
		try
		{
			game = NGame.Instance;
			RunManager runManager = RunManager.Instance;
			SaveManager saveManager = SaveManager.Instance;
			if (game == null || runManager == null || saveManager == null)
			{
				return;
			}
			ReadSaveResult<SerializableRun> loadResult = saveManager.LoadRunSave();
			if (!(AccessTools.Property(((object)loadResult).GetType(), "Success")?.GetValue(loadResult) as bool?).GetValueOrDefault())
			{
				ModEntry.Logger.Warn("STS2Plus quick restart failed to load run save.", 1);
				return;
			}
			object? obj = AccessTools.Property(((object)loadResult).GetType(), "SaveData")?.GetValue(loadResult);
			SerializableRun save = (SerializableRun)((obj is SerializableRun) ? obj : null);
			if (save == null)
			{
				ModEntry.Logger.Warn("STS2Plus quick restart could not resolve saved run data.", 1);
				return;
			}
			if (!QuickRestartSetupBuilder.TryGetSingleplayerCharacter(save, out CharacterModel character) || !QuickRestartSetupBuilder.TryGetRestartActs(save, out List<ActModel> acts))
			{
				return;
			}
			List<ModifierModel> modifiers = QuickRestartSetupBuilder.CreateRestartModifiers(save);
			string seed = QuickRestartSetupBuilder.ResolveRestartSeed(save);
			NAudioManager instance = NAudioManager.Instance;
			if (instance != null)
			{
				instance.StopMusic();
			}
			SfxCmd.Play(character.CharacterTransitionSfx, 1f);
			await game.Transition.FadeOut(0.8f, character.CharacterSelectTransitionPath, (CancellationToken?)null);
			runManager.CleanUp(true);
			toreDownCurrentRun = true;
			game.DebugSeedOverride = null;
			await game.StartNewSingleplayerRun(character, true, (IReadOnlyList<ActModel>)acts, (IReadOnlyList<ModifierModel>)modifiers, seed, save.GameMode, save.Ascension, save.DailyTime);
			game.ReactionContainer.InitializeNetworking(runManager.NetService);
			ModEntry.Logger.Info("STS2Plus quick restart started a fresh run. Seed: " + seed, 1);
		}
		catch (Exception exception)
		{
			ModEntry.Logger.Error($"STS2Plus quick restart failed: {exception}", 1);
			if (toreDownCurrentRun && game != null)
			{
				try
				{
					await game.ReturnToMainMenu();
				}
				catch (Exception ex)
				{
					Exception returnException = ex;
					ModEntry.Logger.Warn("STS2Plus quick restart failed to return to main menu: " + returnException.Message, 1);
				}
			}
		}
		finally
		{
			SpeedControlOverlay.SuspendGameplaySpeed(suspended: false);
			isRestarting = false;
		}
	}
}
