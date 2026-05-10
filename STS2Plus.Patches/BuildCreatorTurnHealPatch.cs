using System;
using System.Reflection;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using STS2Plus.Features;

namespace STS2Plus.Patches;

[HarmonyPatchCategory("MoreRules")]
[HarmonyPatch]
internal static class BuildCreatorTurnHealPatch
{
	private static MethodBase? TargetMethod()
	{
		return AccessTools.Method(typeof(CombatManager), "StartTurn", new Type[1] { typeof(Func<Task>) }, (Type[])null);
	}

	private static void Prefix(CombatManager __instance)
	{
		ModEntry.Verbose("BuildCreatorTurnHeal: restoring enemy HP for turn");
		BuildCreatorRuntime.RestoreEnemyHpForTurn(__instance.DebugOnlyGetState());
	}
}
