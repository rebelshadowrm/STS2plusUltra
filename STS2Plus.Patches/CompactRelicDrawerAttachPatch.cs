using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Runs;
using STS2Plus.Ui;

namespace STS2Plus.Patches;

[HarmonyPatchCategory("Core")]
[HarmonyPatch(typeof(NGlobalUi), "Initialize")]
internal static class CompactRelicDrawerAttachPatch
{
	private static void Postfix(NGlobalUi __instance, RunState runState)
	{
		ModEntry.Verbose("CompactRelicDrawer: attached to global UI");
		CompactRelicDrawer.Attach(__instance, runState);
	}
}
