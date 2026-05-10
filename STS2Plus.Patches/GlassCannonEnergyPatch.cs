using System;
using System.Reflection;
using HarmonyLib;
using STS2Plus.Reflection;

namespace STS2Plus.Patches;

[HarmonyPatchCategory("MoreRules")]
[HarmonyPatch]
internal static class GlassCannonEnergyPatch
{
	private static MethodBase? TargetMethod()
	{
		Type type = RuntimeTypeResolver.FindType("MegaCrit.Sts2.Core.Entities.Players.PlayerCombatState");
		return (type == null) ? null : AccessTools.Method(type, "ResetEnergy", (Type[])null, (Type[])null);
	}

	private static void Postfix(object __instance)
	{
		if (PlusState.IsGlassCannonActive())
		{
			ModEntry.Verbose("GlassCannonEnergy: overriding energy to 4");
			object playerFromCombatState = GameReflection.GetPlayerFromCombatState(__instance);
			if (playerFromCombatState != null)
			{
				GameReflection.EnsurePlayerBaseEnergy(playerFromCombatState, 4, fillCurrent: true);
			}
			else
			{
				GameReflection.SetCombatStateEnergy(__instance, 4, fillCurrent: true);
			}
		}
	}
}
