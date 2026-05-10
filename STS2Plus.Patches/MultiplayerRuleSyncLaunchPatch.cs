using HarmonyLib;
using MegaCrit.Sts2.Core.Runs;
using STS2Plus.Config;

namespace STS2Plus.Patches;

[HarmonyPatchCategory("MoreRules")]
[HarmonyPatch(typeof(RunManager), "Launch")]
internal static class MultiplayerRuleSyncLaunchPatch
{
	private static void Postfix()
	{
		ModEntry.Verbose("RuleSync: launch sync triggered");
		if (!ConfigManager.Current.MoreRulesEnabled)
		{
			MultiplayerRuleSyncCoordinator.Detach();
		}
		else
		{
			MultiplayerRuleSyncCoordinator.AttachCurrentRun();
		}
	}
}
