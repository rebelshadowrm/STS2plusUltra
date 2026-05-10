using HarmonyLib;
using MegaCrit.Sts2.Core.Runs;

namespace STS2Plus.Patches;

[HarmonyPatchCategory("MoreRules")]
[HarmonyPatch(typeof(RunManager), "CleanUp")]
internal static class MultiplayerRuleSyncCleanupPatch
{
	private static void Prefix()
	{
		MultiplayerRuleSyncCoordinator.Detach();
	}
}
