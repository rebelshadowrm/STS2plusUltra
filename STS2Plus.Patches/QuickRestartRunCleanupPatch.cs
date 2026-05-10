using HarmonyLib;
using MegaCrit.Sts2.Core.Runs;

namespace STS2Plus.Patches;

[HarmonyPatchCategory("Core")]
[HarmonyPatch(typeof(RunManager), "CleanUp")]
internal static class QuickRestartRunCleanupPatch
{
	private static void Prefix()
	{
		MultiplayerQuickRestartCoordinator.Detach();
	}
}
