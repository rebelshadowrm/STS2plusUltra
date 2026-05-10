using System;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models.Modifiers;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Runs;
using STS2Plus.Reflection;

namespace STS2Plus.Patches;

internal static class EndlessPostLoadCleanup
{
	internal static bool ShouldSuppressStartRunUi()
	{
		RunState? runState = RunManager.Instance?.DebugOnlyGetState();
		return PlusState.IsEndlessModeActive() && ((GameReflection.GetLoopCount() > 0) || EndlessLoopCoordinator.IsPostEndlessLoadCleanupActive) && runState != null;
	}

	internal static void LogSuppression(string screenOrFlow)
	{
		RunState? runState = RunManager.Instance?.DebugOnlyGetState();
		ModEntry.Logger.Info($"STS2Plus endless loop: suppressing start-run UI flow={screenOrFlow} loopCount={GameReflection.GetLoopCount()} cleanupActive={EndlessLoopCoordinator.IsPostEndlessLoadCleanupActive} act={runState?.CurrentActIndex ?? -1} floor={runState?.ActFloor ?? -1} trueNewRun={GameReflection.GetLoopCount() <= 0}.", 1);
	}
}

[HarmonyPatchCategory("MoreRules")]
[HarmonyPatch(typeof(SealedDeck), "ChooseCards")]
internal static class EndlessSealedDeckSuppressionPatch
{
	private static bool Prefix(ref Task __result)
	{
		if (!EndlessPostLoadCleanup.ShouldSuppressStartRunUi())
		{
			return true;
		}
		EndlessPostLoadCleanup.LogSuppression("SealedDeck.ChooseCards");
		__result = Task.CompletedTask;
		return false;
	}
}

[HarmonyPatchCategory("MoreRules")]
[HarmonyPatch(typeof(Draft), "OfferRewards")]
internal static class EndlessDraftSuppressionPatch
{
	private static bool Prefix(Player player, ref Task __result)
	{
		if (!EndlessPostLoadCleanup.ShouldSuppressStartRunUi())
		{
			return true;
		}
		EndlessPostLoadCleanup.LogSuppression("Draft.OfferRewards");
		__result = Task.CompletedTask;
		return false;
	}
}

[HarmonyPatchCategory("MoreRules")]
[HarmonyPatch(typeof(NCombatRoom), "OnCombatSetUp")]
internal static class EndlessFirstCombatResetPatch
{
	private static void Prefix(CombatState state)
	{
		if (!EndlessLoopCoordinator.TryConsumeFirstCombatResetRequest())
		{
			return;
		}
		string roomType = state?.GetType().Name ?? "<unknown-room>";
		ModEntry.Logger.Info($"STS2Plus endless loop: first combat setup after loop roomType={roomType}.", 1);
		EndlessLoopTransition.ApplyFinalFirstCombatReset("before first combat");
	}

	private static void Postfix(CombatState state)
	{
		if (!EndlessLoopCoordinator.IsPostEndlessLoadCleanupActive)
		{
			return;
		}
		EndlessLoopCoordinator.CompletePostEndlessLoadCleanup("NCombatRoom.OnCombatSetUp");
	}
}
