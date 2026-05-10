using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Multiplayer.Game;
using MegaCrit.Sts2.Core.Runs;

namespace STS2Plus.Patches;

internal static class EndlessMapSelectionParity
{
	private static readonly FieldInfo? RunStateField = AccessTools.Field(typeof(MapSelectionSynchronizer), "_runState");

	private static readonly FieldInfo? VotesField = AccessTools.Field(typeof(MapSelectionSynchronizer), "_votes");

	private static readonly MethodInfo? ClearPlayerVotesMethod = AccessTools.Method(typeof(MapSelectionSynchronizer), "ClearPlayerVotes", Type.EmptyTypes, null);

	private static readonly string[] PlayerIdentifierMembers = new string[4] { "OwnerId", "PlayerId", "NetId", "Id" };

	internal static void PrepareIncomingPlayer(MapSelectionSynchronizer synchronizer, ref Player player, string methodName)
	{
		RunState? runState = GetRunState(synchronizer);
		if (runState == null)
		{
			return;
		}
		TryRemapPlayerToCurrentRun(runState, ref player, methodName);
		if (EnsureVotesMatchPlayers(synchronizer, runState))
		{
			ModEntry.Logger.Info($"STS2Plus endless loop: repaired map-selection votes before {methodName} (players={runState.Players.Count}).", 1);
		}
	}

	internal static void PrepareForFreshMap(RunManager runManager, RunState runState, string phase)
	{
		MapSelectionSynchronizer? synchronizer = runManager.MapSelectionSynchronizer;
		if (synchronizer == null)
		{
			ModEntry.Logger.Warn($"STS2Plus endless loop: map selection synchronizer missing during {phase}.", 1);
			return;
		}
		RunStateField?.SetValue(synchronizer, runState);
		try
		{
			ClearPlayerVotesMethod?.Invoke(synchronizer, Array.Empty<object>());
		}
		catch (Exception ex)
		{
			ModEntry.Logger.Warn("STS2Plus endless loop: ClearPlayerVotes failed during " + phase + " - " + ex.Message, 1);
		}
		synchronizer.BeforeMapGenerated();
		if (EnsureVotesMatchPlayers(synchronizer, runState))
		{
			ModEntry.Logger.Info($"STS2Plus endless loop: repaired map-selection vote list during {phase} (players={runState.Players.Count}).", 1);
		}
	}

	internal static RunState? GetRunState(MapSelectionSynchronizer synchronizer)
	{
		return RunStateField?.GetValue(synchronizer) as RunState ?? RunManager.Instance?.DebugOnlyGetState();
	}

	internal static bool EnsureVotesMatchPlayers(MapSelectionSynchronizer synchronizer, RunState? runState = null)
	{
		runState ??= GetRunState(synchronizer);
		if (runState == null)
		{
			return false;
		}
		bool changed = false;
		int playerCount = runState.Players.Count;
		changed |= EnsureCollectionMemberSize(synchronizer, VotesField, playerCount);
		changed |= TryEnsureCollectionMemberSize(synchronizer, "_playerVotes", playerCount);
		changed |= TryEnsureCollectionMemberSize(synchronizer, "playerVotes", playerCount);
		changed |= TryEnsureCollectionMemberSize(synchronizer, "PlayerVotes", playerCount);
		return changed;
	}

	private static bool TryEnsureCollectionMemberSize(object target, string memberName, int expectedCount)
	{
		FieldInfo? field = AccessTools.Field(target.GetType(), memberName);
		if (field != null)
		{
			return EnsureCollectionMemberSize(target, field, expectedCount);
		}
		PropertyInfo? property = AccessTools.Property(target.GetType(), memberName);
		if (property == null || !property.CanRead)
		{
			return false;
		}
		object? value = property.GetValue(target);
		if (!TryNormalizeCollection(value, property.PropertyType, expectedCount, out object? replacement))
		{
			return false;
		}
		if (property.CanWrite)
		{
			property.SetValue(target, replacement);
		}
		return !ReferenceEquals(value, replacement);
	}

	private static bool EnsureCollectionMemberSize(object target, FieldInfo? field, int expectedCount)
	{
		if (field == null)
		{
			return false;
		}
		object? current = field.GetValue(target);
		if (!TryNormalizeCollection(current, field.FieldType, expectedCount, out object? replacement))
		{
			return false;
		}
		if (!ReferenceEquals(current, replacement))
		{
			field.SetValue(target, replacement);
			return true;
		}
		return false;
	}

	private static bool TryNormalizeCollection(object? current, Type collectionType, int expectedCount, out object? replacement)
	{
		replacement = current;
		if (current is IList list)
		{
			bool changed = false;
			while (list.Count > expectedCount)
			{
				list.RemoveAt(list.Count - 1);
				changed = true;
			}
			while (list.Count < expectedCount)
			{
				list.Add(null);
				changed = true;
			}
			return changed;
		}
		if (current == null)
		{
			replacement = CreateVoteList(expectedCount);
			return true;
		}
		return false;
	}

	private static List<MapVote?> CreateVoteList(int expectedCount)
	{
		List<MapVote?> votes = new List<MapVote?>(expectedCount);
		for (int i = 0; i < expectedCount; i++)
		{
			votes.Add(default(MapVote?));
		}
		return votes;
	}

	private static void TryRemapPlayerToCurrentRun(RunState runState, ref Player player, string methodName)
	{
		Player? remapped = FindCurrentRunPlayer(runState, player);
		if (remapped == null || ReferenceEquals(remapped, player))
		{
			return;
		}
		player = remapped;
		int slot = runState.GetPlayerSlotIndex(remapped);
		ModEntry.Logger.Info($"STS2Plus endless loop: remapped stale multiplayer player reference before {methodName} to slot {slot}.", 1);
	}

	private static Player? FindCurrentRunPlayer(RunState runState, Player player)
	{
		foreach (Player current in runState.Players)
		{
			if (ReferenceEquals(current, player))
			{
				return current;
			}
		}
		string? playerIdentifier = TryGetIdentifier(player);
		if (playerIdentifier == null)
		{
			return null;
		}
		foreach (Player current2 in runState.Players)
		{
			if (string.Equals(TryGetIdentifier(current2), playerIdentifier, StringComparison.OrdinalIgnoreCase))
			{
				return current2;
			}
		}
		return null;
	}

	private static string? TryGetIdentifier(object target)
	{
		foreach (string memberName in PlayerIdentifierMembers)
		{
			if (TryGetMemberString(target, memberName, out string? value))
			{
				return value;
			}
		}
		return null;
	}

	private static bool TryGetMemberString(object target, string memberName, out string? value)
	{
		value = null;
		try
		{
			PropertyInfo? property = AccessTools.Property(target.GetType(), memberName);
			if (property != null)
			{
				object? propertyValue = property.GetValue(target);
				if (propertyValue != null)
				{
					value = propertyValue.ToString();
					return !string.IsNullOrWhiteSpace(value);
				}
			}
			FieldInfo? field = AccessTools.Field(target.GetType(), memberName);
			if (field != null)
			{
				object? fieldValue = field.GetValue(target);
				if (fieldValue != null)
				{
					value = fieldValue.ToString();
					return !string.IsNullOrWhiteSpace(value);
				}
			}
		}
		catch
		{
		}
		return false;
	}
}
