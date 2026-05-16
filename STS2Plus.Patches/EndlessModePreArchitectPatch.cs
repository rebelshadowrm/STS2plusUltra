using System;
using System.Collections;
using System.Reflection;
using System.Threading.Tasks;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Runs;
using STS2Plus.Reflection;

namespace STS2Plus.Patches;

[HarmonyPatchCategory("MoreRules")]
[HarmonyPatch(typeof(RunManager), "EnterNextAct")]
internal static class EndlessModePreArchitectPatch
{
	private static bool Prefix(RunManager __instance, ref Task? __result)
	{
		if (!TryDescribeCurrentTransitionContext(__instance, out var state, out var currentRoom, out var currentEventId, out var isArchitectEvent, out var currentRoomType, out var isVictoryRoom, out var currentActIndex, out var actsCount, out var currentEncounterId))
		{
			return true;
		}
		ModEntry.Logger.Info($"STS2Plus endless loop: EnterNextAct prefix endlessActive=True isFinalAct=True currentActIndex={currentActIndex} actsCount={actsCount} currentRoom={currentRoomType} isVictoryRoom={isVictoryRoom} currentEncounter={currentEncounterId} currentEvent={currentEventId} handling={(isArchitectEvent ? "ARCHITECT_PROCEED" : "PRE_ARCHITECT")}.", 1);
		if (EndlessLoopCoordinator.IsLaunching)
		{
			ModEntry.Logger.Info("STS2Plus endless loop: EnterNextAct ignored because endless transition is already launching.", 1);
			__result = Task.CompletedTask;
			return false;
		}
		__result = StartBypassTransition(__instance, isArchitectEvent, fromUi: false);
		return false;
	}

	internal static bool TryStartRewardsScreenUiBypass(object rewardsScreen)
	{
		return false;
	}

	internal static Task? StartBypassTransition(RunManager runManager, bool isArchitectEvent, bool fromUi)
	{
		if (!GameReflection.IsMultiplayerRun())
		{
			string pathLabel = isArchitectEvent ? "NATURAL_SINGLEPLAYER_ARCHITECT_PROCEED" : (fromUi ? "NATURAL_SINGLEPLAYER_PRE_ARCHITECT_UI" : "NATURAL_SINGLEPLAYER_PRE_ARCHITECT");
			ModEntry.Logger.Info("STS2Plus LOOP PATH: " + pathLabel, 1);
			ModEntry.Logger.Info("STS2Plus endless loop: final-act transition routed into legacy singleplayer endless transition path=" + pathLabel + ".", 1);
			EndlessModeWinRunPatch.InWinTransition = false;
			return GameReflection.TriggerLegacySingleplayerEndlessLoop(runManager, pathLabel) ?? Task.CompletedTask;
		}
		string text;
		if (MultiplayerSafety.ShouldApplyAuthoritativeGameplayPatches())
		{
			text = isArchitectEvent ? "MULTIPLAYER_HOST_ARCHITECT_PROCEED" : (fromUi ? "MULTIPLAYER_HOST_PRE_ARCHITECT_UI" : "MULTIPLAYER_HOST_PRE_ARCHITECT");
		}
		else
		{
			text = isArchitectEvent ? "MULTIPLAYER_CLIENT_ARCHITECT_PROCEED" : (fromUi ? "MULTIPLAYER_CLIENT_PRE_ARCHITECT_UI" : "MULTIPLAYER_CLIENT_PRE_ARCHITECT");
		}
		ModEntry.Logger.Info("STS2Plus LOOP PATH: " + text, 1);
		ModEntry.Logger.Info("STS2Plus endless loop: final-act transition routed into coordinator transition path=" + text + ".", 1);
		EndlessModeWinRunPatch.InWinTransition = true;
		return EndlessLoopCoordinator.StartFromWinRunAsync(runManager);
	}

	internal static bool TryDescribeCurrentTransitionContext(RunManager runManager, out RunState? state, out object? currentRoom, out string currentEventId, out bool isArchitectEvent, out string currentRoomType, out bool isVictoryRoom, out int currentActIndex, out int actsCount, out string currentEncounterId)
	{
		state = runManager?.DebugOnlyGetState();
		currentRoom = null;
		currentEventId = "<none>";
		isArchitectEvent = false;
		currentRoomType = "<null>";
		isVictoryRoom = false;
		currentActIndex = state?.CurrentActIndex ?? -1;
		actsCount = CountActs(state?.Acts);
		currentEncounterId = "<none>";
		if (!PlusState.IsEndlessModeActive() || state == null)
		{
			return false;
		}
		if (currentActIndex < actsCount - 1)
		{
			return false;
		}
		currentRoom = state.CurrentRoom;
		currentRoomType = currentRoom?.GetType().Name ?? "<null>";
		isVictoryRoom = (AccessTools.Property(currentRoom?.GetType(), "IsVictoryRoom")?.GetValue(currentRoom) as bool?).GetValueOrDefault();
		object? currentEvent = ResolveCurrentEvent(currentRoom);
		currentEventId = DescribeEventId(currentEvent);
		currentEncounterId = DescribeEncounterId(currentRoom);
		isArchitectEvent = string.Equals(currentEventId, "THE_ARCHITECT", StringComparison.OrdinalIgnoreCase) || string.Equals(currentEvent?.GetType().Name, "TheArchitect", StringComparison.Ordinal);
		return true;
	}

	private static object? ResolveCurrentEvent(object? currentRoom)
	{
		if (currentRoom == null)
		{
			return null;
		}
		return AccessTools.Property(currentRoom.GetType(), "CanonicalEvent")?.GetValue(currentRoom) ?? AccessTools.Property(currentRoom.GetType(), "Event")?.GetValue(currentRoom) ?? AccessTools.Property(currentRoom.GetType(), "Model")?.GetValue(currentRoom) ?? AccessTools.Field(currentRoom.GetType(), "CanonicalEvent")?.GetValue(currentRoom) ?? AccessTools.Field(currentRoom.GetType(), "Event")?.GetValue(currentRoom) ?? AccessTools.Field(currentRoom.GetType(), "Model")?.GetValue(currentRoom);
	}

	private static string DescribeEventId(object? eventModel)
	{
		if (eventModel == null)
		{
			return "<none>";
		}
		object? id = AccessTools.Property(eventModel.GetType(), "Id")?.GetValue(eventModel) ?? AccessTools.Property(eventModel.GetType(), "ModelId")?.GetValue(eventModel) ?? AccessTools.Field(eventModel.GetType(), "Id")?.GetValue(eventModel) ?? AccessTools.Field(eventModel.GetType(), "ModelId")?.GetValue(eventModel);
		string? entry = ((id == null) ? null : (AccessTools.Property(id.GetType(), "Entry")?.GetValue(id)?.ToString() ?? AccessTools.Field(id.GetType(), "Entry")?.GetValue(id)?.ToString()));
		return entry ?? id?.ToString() ?? eventModel.GetType().Name;
	}

	internal static string DescribeEncounterId(object? room)
	{
		if (room == null)
		{
			return "<none>";
		}
		object? encounter = AccessTools.Property(room.GetType(), "Encounter")?.GetValue(room) ?? AccessTools.Field(room.GetType(), "Encounter")?.GetValue(room);
		if (encounter == null)
		{
			return "<none>";
		}
		object? id = AccessTools.Property(encounter.GetType(), "Id")?.GetValue(encounter) ?? AccessTools.Field(encounter.GetType(), "Id")?.GetValue(encounter);
		string? entry = ((id == null) ? null : (AccessTools.Property(id.GetType(), "Entry")?.GetValue(id)?.ToString() ?? AccessTools.Field(id.GetType(), "Entry")?.GetValue(id)?.ToString()));
		return entry ?? id?.ToString() ?? encounter.GetType().Name;
	}

	private static int CountActs(object? acts)
	{
		if (acts is ICollection collection)
		{
			return collection.Count;
		}
		if (acts is IEnumerable enumerable)
		{
			int num = 0;
			foreach (object _ in enumerable)
			{
				num++;
			}
			return num;
		}
		return -1;
	}
}

[HarmonyPatchCategory("MoreRules")]
[HarmonyPatch(typeof(NCombatRoom), "_Ready")]
internal static class EndlessFinalBossProceedSuppressPatch
{
	private static void Postfix(NCombatRoom __instance)
	{
		return;
	}
}
