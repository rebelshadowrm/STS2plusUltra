using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using STS2Plus.Modifiers;
using STS2Plus.Reflection;

namespace STS2Plus.Patches;

[HarmonyPatchCategory("MoreRules")]
[HarmonyPatch]
internal static class EndlessModeRewardsScreenPatch
{
	[HarmonyTargetMethod]
	private static MethodBase? TargetMethod()
	{
		try
		{
			Type? rewardsSetType = RuntimeTypeResolver.FindType("MegaCrit.Sts2.Core.Rewards.RewardsSet");
			MethodBase? method = (rewardsSetType == null) ? null : AccessTools.Method(rewardsSetType, "WithRewardsFromRoom", new[] { RuntimeTypeResolver.FindType("MegaCrit.Sts2.Core.Rooms.AbstractRoom") });
			ModEntry.Logger.Info("EndlessModeRewardsScreenPatch: TargetMethod resolved RewardsSet.WithRewardsFromRoom = " + (method != null), 1);
			if (method != null)
			{
				ModEntry.Logger.Info("EndlessModeRewardsScreenPatch: patch active", 1);
			}
			return method;
		}
		catch (Exception ex)
		{
			ModEntry.Logger.Warn("EndlessModeRewardsScreenPatch: TargetMethod failed - " + ex.GetType().Name + ": " + ex.Message, 1);
			return null;
		}
	}

	private static void Postfix(object __instance, object? room)
	{
		try
		{
			if (__instance is not RewardsSet rewardsSet)
			{
				return;
			}
			Player? player = rewardsSet.Player;
			if (player == null)
			{
				return;
			}
			object? runState = player.RunState;
			bool endlessActive = HasEndlessModeModifier(runState);
			ModEntry.Logger.Info($"EndlessGate: ENDLESS_MODE active={endlessActive} source=RewardsSet.WithRewardsFromRoom", 1);
			if (!TryDescribeFinalBossContext(runState, room, out int currentActIndex, out int actsCount, out string roomType))
			{
				ModEntry.Logger.Info($"EndlessFinalBossRewards: skipped reason=not-final-boss room={roomType} act={currentActIndex} actsCount={actsCount}", 1);
				return;
			}
			ModEntry.Logger.Info($"EndlessFinalBossRewards: WithRewardsFromRoom finalBoss=true endlessActive={endlessActive} room={roomType} act={currentActIndex} actsCount={actsCount}", 1);
			if (!endlessActive)
			{
				ModEntry.Logger.Info("EndlessFinalBossRewards: skipped reason=modifier-not-active", 1);
				return;
			}
			if (rewardsSet.Rewards is not IList rewards)
			{
				ModEntry.Logger.Warn("EndlessFinalBossRewards: skipped because rewards list could not be resolved.", 1);
				return;
			}
			if (rewards.Count > 0)
			{
				ModEntry.Logger.Info($"EndlessFinalBossRewards: skipped reason=rewards-already-present count={rewards.Count}", 1);
				return;
			}
			MethodInfo? generateRewardsFor = AccessTools.Method(typeof(RewardsSet), "GenerateRewardsFor", new[] { typeof(Player), typeof(AbstractRoom) });
			ModEntry.Logger.Info("EndlessFinalBossRewards: GenerateRewardsFor resolved=" + (generateRewardsFor != null), 1);
			if (generateRewardsFor == null)
			{
				ModEntry.Logger.Warn("EndlessFinalBossRewards: failed because GenerateRewardsFor could not be resolved.", 1);
				return;
			}
			List<Reward>? generated = generateRewardsFor.Invoke(rewardsSet, new object[] { player, room! }) as List<Reward>;
			if (generated == null)
			{
				ModEntry.Logger.Warn("EndlessFinalBossRewards: GenerateRewardsFor returned null.", 1);
				return;
			}
			ModEntry.Logger.Info($"EndlessFinalBossRewards: generated count={generated.Count} types=[{string.Join(", ", generated.Select(DescribeRewardType))}]", 1);
			rewardsSet.Rewards.AddRange(generated);
			int extraCount = 0;
			if (room is CombatRoom combatRoom && combatRoom.ExtraRewards.TryGetValue(player, out List<Reward>? extraRewards))
			{
				extraCount = extraRewards?.Count ?? 0;
				if (extraRewards != null)
				{
					rewardsSet.Rewards.AddRange(extraRewards);
				}
			}
			ModEntry.Logger.Info($"EndlessFinalBossRewards: extraRewards count={extraCount}", 1);
			ModEntry.Logger.Info($"EndlessFinalBossRewards: final rewards count={rewardsSet.Rewards.Count} types=[{string.Join(", ", rewardsSet.Rewards.Select(DescribeRewardType))}]", 1);
		}
		catch (Exception ex)
		{
			ModEntry.Logger.Warn("EndlessFinalBossRewards: failed - " + ex, 1);
		}
	}

	private static bool TryDescribeFinalBossContext(object? runState, object? room, out int currentActIndex, out int actsCount, out string roomType)
	{
		currentActIndex = (AccessTools.Property(runState?.GetType(), "CurrentActIndex")?.GetValue(runState) as int?).GetValueOrDefault(-1);
		actsCount = CountCollection(AccessTools.Property(runState?.GetType(), "Acts")?.GetValue(runState));
		roomType = AccessTools.Property(room?.GetType(), "RoomType")?.GetValue(room)?.ToString() ?? room?.GetType().Name ?? "<null>";
		if (runState == null || room == null)
		{
			return false;
		}
		if (!string.Equals(roomType, "Boss", StringComparison.OrdinalIgnoreCase))
		{
			return false;
		}
		return currentActIndex >= actsCount - 1;
	}

	private static bool HasEndlessModeModifier(object? runState)
	{
		if (runState == null)
		{
			return false;
		}
		if (AccessTools.Property(runState.GetType(), "Modifiers")?.GetValue(runState) is not IEnumerable<ModifierModel> modifiers)
		{
			return false;
		}
		return CustomModifierCatalog.ContainsEntry(modifiers, CustomModifierCatalog.EndlessModeEntry);
	}

	private static int CountCollection(object? value)
	{
		if (value is ICollection collection)
		{
			return collection.Count;
		}
		if (value is IEnumerable enumerable)
		{
			int count = 0;
			foreach (object _ in enumerable)
			{
				count++;
			}
			return count;
		}
		return -1;
	}

	private static string DescribeRewardType(object reward)
	{
		string name = reward.GetType().Name;
		return name.EndsWith("Reward", StringComparison.Ordinal) ? name : (reward.GetType().FullName ?? name);
	}
}
