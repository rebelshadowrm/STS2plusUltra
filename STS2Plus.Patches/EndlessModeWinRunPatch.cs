using System;
using System.Reflection;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Runs;
using STS2Plus.Reflection;
using STS2Plus.Ui;

namespace STS2Plus.Patches;

[HarmonyPatchCategory("MoreRules")]
[HarmonyPatch]
internal static class EndlessModeWinRunPatch
{
	[HarmonyTargetMethod]
	private static MethodBase? TargetMethod()
	{
		Type type = RuntimeTypeResolver.FindType("MegaCrit.Sts2.Core.Runs.RunManager") ?? RuntimeTypeResolver.FindTypeByName("RunManager");
		return (type == null) ? null : AccessTools.Method(type, "WinRun", (Type[])null, (Type[])null);
	}

	internal static bool InWinTransition { get; set; }

	private static bool Prefix(RunManager __instance, ref Task? __result)
	{
		if (!PlusState.IsEndlessModeActive())
		{
			return true;
		}
		if (!GameReflection.IsMultiplayerRun())
		{
			ModEntry.Logger.Info("STS2Plus LOOP PATH: NATURAL_SINGLEPLAYER", 1);
			ModEntry.Logger.Info("STS2Plus endless loop: intercepted RunManager.WinRun and starting legacy singleplayer endless transition.", 1);
			InWinTransition = false;
			__result = GameReflection.TriggerLegacySingleplayerEndlessLoop(__instance, "NATURAL_SINGLEPLAYER") ?? Task.CompletedTask;
			return false;
		}
		ModEntry.Logger.Info("STS2Plus LOOP PATH: MULTIPLAYER_HOST", 1);
		ModEntry.Logger.Info("STS2Plus endless loop: intercepted RunManager.WinRun and starting local coordinator transition.", 1);
		InWinTransition = true;
		__result = EndlessLoopCoordinator.StartFromWinRunAsync(__instance);
		return false;
	}
}
