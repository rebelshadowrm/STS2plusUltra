using HarmonyLib;
using MegaCrit.Sts2.Core.Runs;

namespace STS2Plus.Patches;

[HarmonyPatchCategory("Core")]
[HarmonyPatch(typeof(RunManager), "Launch")]
internal static class QuickRestartRunLaunchPatch
{
	private static void Postfix()
	{
		MultiplayerQuickRestartCoordinator.AttachCurrentRun();
		EndlessLoopCoordinator.AttachCurrentRun();
	}
}
