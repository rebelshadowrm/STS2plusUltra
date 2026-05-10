using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes;
using STS2Plus.Ui;

namespace STS2Plus.Patches;

[HarmonyPatchCategory("Core")]
[HarmonyPatch(typeof(NGame), "_Ready")]
internal static class SpeedControlBootstrapPatch
{
	private static void Postfix()
	{
		ModEntry.Verbose("SpeedControl: attached to game bootstrap");
		SpeedControlOverlay.SetMainMenuVisible(visible: false);
		SpeedControlOverlay.Show();
	}
}
