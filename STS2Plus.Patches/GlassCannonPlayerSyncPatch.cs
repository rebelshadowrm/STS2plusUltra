using System;
using System.Reflection;
using HarmonyLib;
using STS2Plus.Reflection;

namespace STS2Plus.Patches;

[HarmonyPatchCategory("MoreRules")]
[HarmonyPatch]
internal static class GlassCannonPlayerSyncPatch
{
	private static MethodBase? TargetMethod()
	{
		Type type = RuntimeTypeResolver.FindType("MegaCrit.Sts2.Core.Entities.Players.Player");
		return (type == null) ? null : AccessTools.Method(type, "SyncWithSerializedPlayer", (Type[])null, (Type[])null);
	}

	private static void Prefix(object __instance, object? player)
	{
		if (PlusState.IsGlassCannonActive() && player != null && GameReflection.NormalizeSerializedGlassCannonPlayer(player, __instance))
		{
			ModEntry.Logger.Info("STS2Plus normalized serialized Glass Cannon hp before player sync.", 1);
		}
	}

	private static void Postfix(object __instance)
	{
		if (PlusState.IsGlassCannonActive() && GameReflection.RepairGlassCannonState(__instance))
		{
			ModEntry.Logger.Info("STS2Plus repaired Glass Cannon health after player sync.", 1);
		}
	}
}
