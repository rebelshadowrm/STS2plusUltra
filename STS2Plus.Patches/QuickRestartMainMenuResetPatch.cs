using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Screens.MainMenu;

namespace STS2Plus.Patches;

[HarmonyPatchCategory("Core")]
[HarmonyPatch(typeof(NMainMenu), "_Ready")]
internal static class QuickRestartMainMenuResetPatch
{
	private static void Prefix()
	{
		MultiplayerQuickRestartCoordinator.Detach();
		EndlessLoopCoordinator.Detach();
	}
}
