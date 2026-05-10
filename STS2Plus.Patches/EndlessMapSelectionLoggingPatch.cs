using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Map;
using MegaCrit.Sts2.Core.Multiplayer.Game;
using MegaCrit.Sts2.Core.Runs;
using STS2Plus.Reflection;

namespace STS2Plus.Patches;

[HarmonyPatchCategory("MoreRules")]
[HarmonyPatch(typeof(MapSelectionSynchronizer), "PlayerVotedForMapCoord")]
internal static class EndlessMapSelectionVoteLoggingPatch
{
	private static readonly FieldInfo? AcceptingVotesField = AccessTools.Field(typeof(MapSelectionSynchronizer), "_acceptingVotesFromSource");

	private static readonly FieldInfo? VotesField = AccessTools.Field(typeof(MapSelectionSynchronizer), "_votes");

	private static readonly PropertyInfo? ActMapGridProperty = AccessTools.Property(typeof(ActMap), "Grid");

	private static void Prefix(MapSelectionSynchronizer __instance, ref Player player, MapLocation source, MapVote? destination)
	{
		if (!PlusState.IsEndlessModeActive())
		{
			return;
		}
		try
		{
			EndlessMapSelectionParity.PrepareIncomingPlayer(__instance, ref player, "PlayerVotedForMapCoord");
			LogVoteState("prefix", __instance, player, source, destination);
		}
		catch (Exception ex)
		{
			ModEntry.Logger.Warn("STS2Plus endless loop: map vote prefix logging failed - " + ex.Message, 1);
		}
	}

	private static void Postfix(MapSelectionSynchronizer __instance, Player player, MapLocation source, MapVote? destination)
	{
		if (!PlusState.IsEndlessModeActive())
		{
			return;
		}
		try
		{
			LogVoteState("postfix", __instance, player, source, destination);
		}
		catch (Exception ex)
		{
			ModEntry.Logger.Warn("STS2Plus endless loop: map vote postfix logging failed - " + ex.Message, 1);
		}
	}

	private static void LogVoteState(string phase, MapSelectionSynchronizer synchronizer, Player player, MapLocation source, MapVote? destination)
	{
		RunState? runState = EndlessMapSelectionParity.GetRunState(synchronizer);
		object? acceptingValue = AcceptingVotesField?.GetValue(synchronizer);
		MapLocation accepting = (acceptingValue is MapLocation location) ? location : default(MapLocation);
		string voteText = EndlessLoopTransition.DescribeVotes(VotesField?.GetValue(synchronizer));
		string mapSeed = runState?.Rng.StringSeed ?? "<null>";
		int loopIndex = GameReflection.GetLoopCount();
		int actIndex = runState?.CurrentActIndex ?? -1;
		int actFloor = runState?.ActFloor ?? -1;
		bool sourceMatches = source.Equals(accepting);
		bool generationAccepted = !destination.HasValue || destination.Value.mapGenerationCount >= synchronizer.MapGenerationCount;
		string clickedCoord = destination?.coord.ToString() ?? "<cancel>";
		string clickedPoint = DescribePoint(runState?.Map, destination);
		int slot = (runState == null) ? -1 : runState.GetPlayerSlotIndex(player);
		bool allVotesPresent = AreAllVotesPresent(VotesField?.GetValue(synchronizer));
		ModEntry.Logger.Info($"STS2Plus endless loop map vote {phase}: player={player.NetId} slot={slot} coord={clickedCoord} point={clickedPoint} act={actIndex} floor={actFloor} loopIndex={loopIndex} seed={mapSeed} source={source} accepting={accepting} sourceMatches={sourceMatches} voteMapGen={(destination?.mapGenerationCount.ToString() ?? "<null>")} syncMapGen={synchronizer.MapGenerationCount} generationAccepted={generationAccepted} allVotesPresent={allVotesPresent} votes={voteText}.", 1);
		EndlessLoopTransition.LogMapSelectionState(RunManager.Instance, runState, loopIndex, mapSeed, "vote " + phase);
	}

	private static bool AreAllVotesPresent(object? votesObject)
	{
		if (votesObject is IEnumerable enumerable)
		{
			foreach (object? item in enumerable)
			{
				if (item == null)
				{
					return false;
				}
			}
		}
		return true;
	}

	private static string DescribePoint(ActMap? map, MapVote? vote)
	{
		if (map == null || !vote.HasValue)
		{
			return "<none>";
		}
		MapCoord coord = vote.Value.coord;
		MapPoint[,]? grid = ActMapGridProperty?.GetValue(map) as MapPoint[,];
		if (grid == null)
		{
			return "<grid-unavailable>";
		}
		int width = grid.GetLength(0);
		int height = grid.GetLength(1);
		if (coord.col < 0 || coord.row < 0 || coord.col >= width || coord.row >= height)
		{
			return $"<out-of-grid col={coord.col} row={coord.row} width={width} height={height}>";
		}
		MapPoint point = grid[coord.col, coord.row];
		if (point == null)
		{
			return "<null>";
		}
		string questId = "<none>";
		if (point.Quests.Count > 0)
		{
			questId = AccessTools.Property(point.Quests[0].GetType(), "Id")?.GetValue(point.Quests[0])?.ToString() ?? point.Quests[0].ToString();
		}
		return $"type={point.PointType} quest={questId}";
	}
}

[HarmonyPatchCategory("MoreRules")]
[HarmonyPatch(typeof(MapSelectionSynchronizer), "GetVote")]
internal static class EndlessMapSelectionGetVoteLoggingPatch
{
	private static readonly FieldInfo? VotesField = AccessTools.Field(typeof(MapSelectionSynchronizer), "_votes");

	private static void Prefix(MapSelectionSynchronizer __instance, ref Player player)
	{
		if (!PlusState.IsEndlessModeActive())
		{
			return;
		}
		try
		{
			EndlessMapSelectionParity.PrepareIncomingPlayer(__instance, ref player, "GetVote");
			RunState? runState = EndlessMapSelectionParity.GetRunState(__instance);
			string mapSeed = runState?.Rng.StringSeed ?? "<null>";
			int loopIndex = GameReflection.GetLoopCount();
			ModEntry.Logger.Info($"STS2Plus endless loop map get-vote prefix: player={player.NetId} slot={(runState == null ? -1 : runState.GetPlayerSlotIndex(player))} act={runState?.CurrentActIndex ?? -1} floor={runState?.ActFloor ?? -1} loopIndex={loopIndex} seed={mapSeed} votes={EndlessLoopTransition.DescribeVotes(VotesField?.GetValue(__instance))}.", 1);
		}
		catch (Exception ex)
		{
			ModEntry.Logger.Warn("STS2Plus endless loop: map get-vote prefix logging failed - " + ex.Message, 1);
		}
	}

	private static void Postfix(MapSelectionSynchronizer __instance, Player player, MapVote? __result)
	{
		if (!PlusState.IsEndlessModeActive())
		{
			return;
		}
		try
		{
			RunState? runState = EndlessMapSelectionParity.GetRunState(__instance);
			string mapSeed = runState?.Rng.StringSeed ?? "<null>";
			int loopIndex = GameReflection.GetLoopCount();
			ModEntry.Logger.Info($"STS2Plus endless loop map get-vote postfix: player={player.NetId} slot={(runState == null ? -1 : runState.GetPlayerSlotIndex(player))} result={(__result?.ToString() ?? "<null>")} act={runState?.CurrentActIndex ?? -1} floor={runState?.ActFloor ?? -1} loopIndex={loopIndex} seed={mapSeed}.", 1);
		}
		catch (Exception ex)
		{
			ModEntry.Logger.Warn("STS2Plus endless loop: map get-vote postfix logging failed - " + ex.Message, 1);
		}
	}
}

[HarmonyPatchCategory("MoreRules")]
[HarmonyPatch(typeof(MapSelectionSynchronizer), "MoveToMapCoord")]
internal static class EndlessMapSelectionMoveLoggingPatch
{
	private static readonly FieldInfo? VotesField = AccessTools.Field(typeof(MapSelectionSynchronizer), "_votes");

	private static void Prefix(MapSelectionSynchronizer __instance)
	{
		if (!PlusState.IsEndlessModeActive())
		{
			return;
		}
		try
		{
			RunState? runState = RunManager.Instance?.DebugOnlyGetState();
			string mapSeed = runState?.Rng.StringSeed ?? "<null>";
			int loopIndex = GameReflection.GetLoopCount();
			ModEntry.Logger.Info($"STS2Plus endless loop map move: host resolving votes act={runState?.CurrentActIndex ?? -1} floor={runState?.ActFloor ?? -1} loopIndex={loopIndex} seed={mapSeed} votes={EndlessLoopTransition.DescribeVotes(VotesField?.GetValue(__instance))}.", 1);
			EndlessLoopTransition.LogMapSelectionState(RunManager.Instance, runState, loopIndex, mapSeed, "move-to-mapcoord");
		}
		catch (Exception ex)
		{
			ModEntry.Logger.Warn("STS2Plus endless loop: map move logging failed - " + ex.Message, 1);
		}
	}
}
