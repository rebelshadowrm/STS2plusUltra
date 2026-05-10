using System;
using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Players;
using STS2Plus.Reflection;

namespace STS2Plus.Patches;

[HarmonyPatchCategory("MoreRules")]
[HarmonyPatch]
internal static class GiantCreaturesGoldRewardFixedPatch
{
	private static MethodBase? TargetMethod()
	{
		Type type = RuntimeTypeResolver.FindType("MegaCrit.Sts2.Core.Rewards.GoldReward");
		return (type == null) ? null : AccessTools.Constructor(type, new Type[3]
		{
			typeof(int),
			typeof(Player),
			typeof(bool)
		}, false);
	}

	private static void Prefix(ref int amount, object? player)
	{
		if (MultiplayerSafety.ShouldApplyAuthoritativeGameplayPatches() && ShouldBoostGold(player))
		{
			ModEntry.Verbose($"GiantCreaturesGoldFixed: boosting gold amount={amount} multiplier=2.0");
			amount = GameReflection.ApplyGoldBonus(amount, 2.0m);
		}
	}

	private static bool ShouldBoostGold(object? player)
	{
		return PlusState.IsGiantCreaturesActive() && player != null && GameReflection.IsCurrentCombatRewardRoom();
	}
}
