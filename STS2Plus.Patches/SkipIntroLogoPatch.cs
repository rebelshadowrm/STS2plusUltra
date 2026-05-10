using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes;
using STS2Plus.Config;

namespace STS2Plus.Patches;

[HarmonyPatchCategory("Core")]
[HarmonyPatch(typeof(NGame), "LaunchMainMenu")]
internal static class SkipIntroLogoPatch
{
	private static void Prefix(ref bool skipLogo)
	{
		if (ConfigManager.Current.SkipIntroEnabled)
		{
			ModEntry.Verbose("SkipIntro: intro logo skipped");
			skipLogo = true;
		}
	}
}
