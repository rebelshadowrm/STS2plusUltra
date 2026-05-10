using System;
using System.Reflection;
using HarmonyLib;
using STS2Plus.Features;
using STS2Plus.Reflection;

namespace STS2Plus.Patches;

[HarmonyPatchCategory("MoreRules")]
[HarmonyPatch]
internal static class GlassCannonCombatRewardPatch
{
	private static MethodBase? TargetMethod()
	{
		Type type = RuntimeTypeResolver.FindType("MegaCrit.Sts2.Core.Rewards.RewardsSet");
		return (type == null) ? null : AccessTools.Method(type, "GenerateRewardsFor", (Type[])null, (Type[])null);
	}

	private static void Postfix(object? player, object? room)
	{
		if (!PlusState.IsGlassCannonActive() || room == null || !GameReflection.IsCombatRewardRoom(room))
		{
			return;
		}
		int num = 0;
		foreach (object player2 in GameReflection.GetPlayers())
		{
			if (AppliedTracker.MarkGlassCannonReward(room, player2) && GameReflection.IncreaseMaxHp(player2, 1, healToMax: false, healByAmount: true))
			{
				GameReflection.RepairGlassCannonState(player2);
				num++;
			}
		}
		if (num == 0 && player != null && AppliedTracker.MarkGlassCannonReward(room, player) && GameReflection.IncreaseMaxHp(player, 1, healToMax: false, healByAmount: true))
		{
			GameReflection.RepairGlassCannonState(player);
			num = 1;
		}
		if (num > 0)
		{
			ModEntry.Logger.Info($"STS2Plus applied Glass Cannon combat max HP to {num} player(s).", 1);
		}
	}
}
