using System;
using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Players;
using STS2Plus.Reflection;

namespace STS2Plus.Patches;

[HarmonyPatchCategory("MoreRules")]
[HarmonyPatch]
internal static class GiantCreaturesGoldRewardRangePatch
{
	private static MethodBase? TargetMethod()
	{
		Type type = RuntimeTypeResolver.FindType("MegaCrit.Sts2.Core.Rewards.GoldReward");
		return (type == null) ? null : AccessTools.Constructor(type, new Type[4]
		{
			typeof(int),
			typeof(int),
			typeof(Player),
			typeof(bool)
		}, false);
	}

	private static void Prefix(ref int min, ref int max, object? player)
	{
		if (MultiplayerSafety.ShouldApplyAuthoritativeGameplayPatches() && ShouldBoostGold(player))
		{
			ModEntry.Verbose($"GiantCreaturesGoldRange: boosting gold min={min} max={max} multiplier=2.0");
			min = GameReflection.ApplyGoldBonus(min, 2.0m);
			max = GameReflection.ApplyGoldBonus(max, 2.0m);
		}
	}

	private static bool ShouldBoostGold(object? player)
	{
		return PlusState.IsGiantCreaturesActive() && player != null && GameReflection.IsCurrentCombatRewardRoom();
	}
}
