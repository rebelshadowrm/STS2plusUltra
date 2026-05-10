using System;
using System.Collections.Generic;
using System.Linq;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.Unlocks;

namespace STS2Plus.Patches;

internal static class QuickRestartSetupBuilder
{
	public static bool TryGetSingleplayerCharacter(SerializableRun save, out CharacterModel character)
	{
		character = null;
		if (save.Players.Count == 0)
		{
			ModEntry.Logger.Warn("STS2Plus quick restart could not resolve the current character.", 1);
			return false;
		}
		ModelId characterId = save.Players[0].CharacterId;
		if (characterId == (ModelId)null)
		{
			ModEntry.Logger.Warn("STS2Plus quick restart found a run save without a character id.", 1);
			return false;
		}
		character = ModelDb.GetById<CharacterModel>(characterId);
		return true;
	}

	public static bool TryGetRestartActs(SerializableRun save, out List<ActModel> acts)
	{
		acts = new List<ActModel>();
		if (save.Acts.Count > 0)
		{
			foreach (SerializableActModel act in save.Acts)
			{
				if (act.Id == (ModelId)null)
				{
					ModEntry.Logger.Warn("STS2Plus quick restart found a run save with a missing act id.", 1);
					return false;
				}
				acts.Add(ModelDb.GetById<ActModel>(act.Id));
			}
			return true;
		}
		acts.AddRange(ActModel.GetDefaultList());
		return true;
	}

	public static List<ModifierModel> CreateRestartModifiers(SerializableRun save)
	{
		return ((IEnumerable<SerializableModifier>)save.Modifiers).Select((Func<SerializableModifier, ModifierModel>)ModifierModel.FromSerializable).ToList();
	}

	public static string ResolveRestartSeed(SerializableRun save)
	{
		string result;
		if (save.DailyTime.HasValue)
		{
			SerializableRunRngSet serializableRng = save.SerializableRng;
			if (!string.IsNullOrWhiteSpace((serializableRng != null) ? serializableRng.Seed : null))
			{
				result = save.SerializableRng.Seed;
				goto IL_003e;
			}
		}
		result = SeedHelper.GetRandomSeed(10);
		goto IL_003e;
		IL_003e:
		return result;
	}

	public static bool TryCreateRestartPlayers(SerializableRun save, out List<Player> players)
	{
		players = new List<Player>();
		if (save.Players.Count == 0)
		{
			ModEntry.Logger.Warn("STS2Plus multiplayer quick restart found a save without players.", 1);
			return false;
		}
		foreach (SerializablePlayer player in save.Players)
		{
			if (player.CharacterId == (ModelId)null)
			{
				ModEntry.Logger.Warn($"STS2Plus multiplayer quick restart found a player without a character id. NetId: {player.NetId}", 1);
				return false;
			}
			players.Add(Player.CreateForNewRun(ModelDb.GetById<CharacterModel>(player.CharacterId), UnlockState.FromSerializable(player.UnlockState), player.NetId));
		}
		return true;
	}
}
