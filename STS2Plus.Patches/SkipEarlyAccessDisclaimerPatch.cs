using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Screens.MainMenu;
using MegaCrit.Sts2.Core.Saves;
using STS2Plus.Config;

namespace STS2Plus.Patches;

[HarmonyPatchCategory("Core")]
[HarmonyPatch(typeof(NMainMenu), "_Ready")]
internal static class SkipEarlyAccessDisclaimerPatch
{
	private static void Prefix()
	{
		if (ConfigManager.Current.SkipIntroEnabled)
		{
			ModEntry.Verbose("SkipIntro: EA disclaimer skipped");
			SettingsSave settingsSave = SaveManager.Instance.SettingsSave;
			settingsSave.SkipIntroLogo = true;
			settingsSave.SeenEaDisclaimer = true;
		}
	}
}
