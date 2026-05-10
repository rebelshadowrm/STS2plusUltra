using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Relics;
using STS2Plus.Ui;

namespace STS2Plus.Patches;

[HarmonyPatchCategory("Core")]
[HarmonyPatch(typeof(NRelicInventory), "ShowImmediately")]
internal static class CompactRelicDrawerShowImmediatelyPatch
{
	private static void Postfix(NRelicInventory __instance)
	{
		CompactRelicDrawer.SyncToLegacy(__instance);
	}
}
