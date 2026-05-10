using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Screens.MainMenu;
using STS2Plus.Config;
using STS2Plus.Localization;
using STS2Plus.Reflection;

namespace STS2Plus.Patches;

[HarmonyPatchCategory("Core")]
[HarmonyPatch(typeof(NMainMenu), "_Ready")]
internal static class NMainMenuPatch
{
	private static void Postfix()
	{
		PlusLoc.MergeIntoModifiersTable();
		MultiplayerReflection.ClearRole();
		PlusState.ResetRunRules();
		ModConfigBridge.TryRegister();
		LiveSettings.ApplyAll();
	}
}
