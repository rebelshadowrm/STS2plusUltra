using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Screens.MainMenu;

namespace STS2Plus.Patches;

[HarmonyPatchCategory("MoreRules")]
[HarmonyPatch(typeof(NMainMenu), "_Ready")]
internal static class MultiplayerRuleSyncMainMenuPatch
{
	private static void Prefix()
	{
		MultiplayerRuleSyncCoordinator.Detach();
	}
}
