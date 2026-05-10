using System;
using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using STS2Plus.Features;

namespace STS2Plus.Patches;

[HarmonyPatchCategory("Core")]
[HarmonyPatch]
internal static class PlayerDamageCombatSetupPatch
{
	private static MethodBase? TargetMethod()
	{
		return AccessTools.Method(typeof(NCombatRoom), "OnCombatSetUp", (Type[])null, (Type[])null);
	}

	private static void Postfix()
	{
		ModEntry.Verbose("PlayerDamage: combat setup, recalculating");
		PlayerDamageTracker.Recalculate();
	}
}
