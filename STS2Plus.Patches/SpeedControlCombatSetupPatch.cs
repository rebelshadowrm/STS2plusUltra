using System;
using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using STS2Plus.Ui;

namespace STS2Plus.Patches;

[HarmonyPatchCategory("Core")]
[HarmonyPatch]
internal static class SpeedControlCombatSetupPatch
{
	private static MethodBase? TargetMethod()
	{
		return AccessTools.Method(typeof(NCombatRoom), "OnCombatSetUp", (Type[])null, (Type[])null);
	}

	private static void Postfix()
	{
		ModEntry.Verbose("SpeedControl: combat speed setup");
		SpeedControlOverlay.SetMainMenuVisible(visible: false);
		SpeedControlOverlay.Show();
	}
}
