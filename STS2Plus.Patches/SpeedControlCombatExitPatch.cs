using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using STS2Plus.Ui;

namespace STS2Plus.Patches;

[HarmonyPatchCategory("Core")]
[HarmonyPatch(typeof(NCombatRoom), "_ExitTree")]
internal static class SpeedControlCombatExitPatch
{
	private static void Prefix()
	{
		ModEntry.Verbose("SpeedControl: combat exit cleanup");
		SpeedControlOverlay.SetMainMenuVisible(visible: false);
		SpeedControlOverlay.Show();
	}
}
