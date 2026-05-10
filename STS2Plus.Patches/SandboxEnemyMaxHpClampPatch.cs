using System;
using System.Reflection;
using HarmonyLib;
using STS2Plus.Reflection;

namespace STS2Plus.Patches;

[HarmonyPatchCategory("MoreRules")]
[HarmonyPatch]
internal static class SandboxEnemyMaxHpClampPatch
{
	private static MethodBase? TargetMethod()
	{
		Type creatureRuntimeType = GameReflection.CreatureRuntimeType;
		return (creatureRuntimeType == null) ? null : AccessTools.Method(creatureRuntimeType, "SetMaxHpInternal", (Type[])null, (Type[])null);
	}

	private static void Prefix(object __instance, ref decimal amount)
	{
		if (PlusState.IsSandboxActive() && amount > 1m && GameReflection.IsEnemyCreature(__instance))
		{
			amount = 1m;
		}
	}
}
