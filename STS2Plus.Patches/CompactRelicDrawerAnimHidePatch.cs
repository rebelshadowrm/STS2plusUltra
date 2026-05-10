using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Relics;
using STS2Plus.Ui;

namespace STS2Plus.Patches;

[HarmonyPatchCategory("Core")]
[HarmonyPatch(typeof(NRelicInventory), "AnimHide")]
internal static class CompactRelicDrawerAnimHidePatch
{
	private static void Postfix(NRelicInventory __instance)
	{
		CompactRelicDrawer.SyncToLegacy(__instance);
	}
}
