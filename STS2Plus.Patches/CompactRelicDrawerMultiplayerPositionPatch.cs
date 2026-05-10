using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Multiplayer;
using STS2Plus.Ui;

namespace STS2Plus.Patches;

[HarmonyPatchCategory("Core")]
[HarmonyPatch(typeof(NMultiplayerPlayerStateContainer), "UpdatePosition")]
internal static class CompactRelicDrawerMultiplayerPositionPatch
{
	private static void Postfix(NMultiplayerPlayerStateContainer __instance)
	{
		CompactRelicDrawer.TryApplyMultiplayerLayout(__instance);
	}
}
