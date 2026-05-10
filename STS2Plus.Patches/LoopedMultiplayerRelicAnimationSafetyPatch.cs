using System;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Relics;
using MegaCrit.Sts2.Core.Runs;

namespace STS2Plus.Patches;

[HarmonyPatchCategory("MoreRules")]
[HarmonyPatch(typeof(NRelicInventory), "AnimateRelic")]
internal static class LoopedMultiplayerRelicAnimationSafetyPatch
{
	private static Exception? Finalizer(NRelicInventory __instance, Exception? __exception)
	{
		if (__exception == null)
		{
			return null;
		}
		if (!(__exception is InvalidOperationException) || !__exception.Message.Contains("Sequence contains no matching element", StringComparison.Ordinal))
		{
			return __exception;
		}
		if (!EndlessLoopTransition.IsLoopedMultiplayerAncientEventActive())
		{
			return __exception;
		}
		RunState? runState = RunManager.Instance?.DebugOnlyGetState();
		ModEntry.Logger.Warn("STS2Plus endless loop: missing relic animation target during looped multiplayer Ancient event; skipping animation and continuing obtain. " + __exception.Message, 1);
		EndlessLoopTransition.RefreshLoopedMultiplayerRelicUi(runState, "AnimateRelic finalizer");
		return null;
	}
}
