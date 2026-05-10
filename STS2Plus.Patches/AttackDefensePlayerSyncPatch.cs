using System;
using System.Reflection;
using HarmonyLib;
using STS2Plus.Reflection;

namespace STS2Plus.Patches;

[HarmonyPatchCategory("MoreRules")]
[HarmonyPatch]
internal static class AttackDefensePlayerSyncPatch
{
	private static MethodBase? TargetMethod()
	{
		Type type = RuntimeTypeResolver.FindType("MegaCrit.Sts2.Core.Entities.Players.Player");
		return (type == null) ? null : AccessTools.Method(type, "SyncWithSerializedPlayer", (Type[])null, (Type[])null);
	}

	private static void Postfix(object __instance)
	{
		PlusState.SyncRuleSelectionsFromRunState();
		if (GameReflection.IsMultiplayerRun())
		{
			ModEntry.Logger.Info($"STS2Plus.Net sync source=player authoritative={MultiplayerSafety.ShouldApplyAuthoritativeGameplayPatches()} local_player={GameReflection.IsLocalPlayerObject(__instance)} service={MultiplayerReflection.DescribeCurrentService()}", 1);
		}
		CardRuleHelpers.ReapplyBonusesToPlayerDeck(__instance);
	}
}
