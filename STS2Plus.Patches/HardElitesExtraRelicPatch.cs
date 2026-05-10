using System;
using System.Collections;
using System.Reflection;
using HarmonyLib;
using STS2Plus.Features;
using STS2Plus.Reflection;

namespace STS2Plus.Patches;

[HarmonyPatchCategory("MoreRules")]
[HarmonyPatch]
internal static class HardElitesExtraRelicPatch
{
	private static MethodBase? TargetMethod()
	{
		Type type = RuntimeTypeResolver.FindType("MegaCrit.Sts2.Core.Rewards.RewardsSet");
		return (type == null) ? null : AccessTools.Method(type, "GenerateRewardsFor", (Type[])null, (Type[])null);
	}

	private static void Postfix(object? player, object? room, ref object? __result)
	{
		if (!MultiplayerSafety.ShouldApplyAuthoritativeGameplayPatches() || !PlusState.IsHardElitesActive() || player == null || room == null || !(__result is IList list) || !IsEliteRoom(room) || !AppliedTracker.MarkHardEliteRelicReward(room, player))
		{
			return;
		}
		Type type = RuntimeTypeResolver.FindType("MegaCrit.Sts2.Core.Rewards.RelicReward");
		ConstructorInfo constructorInfo = ((type == null) ? null : AccessTools.Constructor(type, new Type[1] { player.GetType() }, false));
		if (constructorInfo == null)
		{
			return;
		}
		int num = 0;
		foreach (object item in list)
		{
			if (item != null && IsRelicReward(item))
			{
				num++;
			}
		}
		if (num > 0)
		{
			ModEntry.Verbose($"HardElitesExtraRelic: adding extra relic drop existingRelics={num}");
			list.Add(constructorInfo.Invoke(new object[1] { player }));
		}
	}

	private static bool IsRelicReward(object reward)
	{
		string text = reward.GetType().FullName ?? reward.GetType().Name;
		return text.Contains("RelicReward", StringComparison.Ordinal);
	}

	private static bool IsEliteRoom(object room)
	{
		string a = AccessTools.Property(room.GetType(), "RoomType")?.GetValue(room)?.ToString();
		return string.Equals(a, "Elite", StringComparison.OrdinalIgnoreCase);
	}
}
