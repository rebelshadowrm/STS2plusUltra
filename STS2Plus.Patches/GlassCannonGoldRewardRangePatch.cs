using System;
using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Players;
using STS2Plus.Reflection;

namespace STS2Plus.Patches;

[HarmonyPatchCategory("MoreRules")]
[HarmonyPatch]
internal static class GlassCannonGoldRewardRangePatch
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
		if (MultiplayerSafety.ShouldApplyAuthoritativeGameplayPatches() && PlusState.IsGlassCannonActive() && player != null)
		{
			ModEntry.Verbose($"GlassCannonGoldRange: doubling gold min={min} max={max}");
			min = GameReflection.ApplyGoldBonus(min, 2.0m);
			max = GameReflection.ApplyGoldBonus(max, 2.0m);
		}
	}
}
