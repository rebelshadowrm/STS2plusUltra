using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Screens.MainMenu;
using STS2Plus.Features;
using STS2Plus.Localization;
using STS2Plus.Ui;

namespace STS2Plus.Patches;

[HarmonyPatchCategory("Core")]
[HarmonyPatch(typeof(NMainMenu), "_Ready")]
internal static class PlusLifecyclePatch
{
	private static void Prefix()
	{
		ModEntry.Verbose("Lifecycle: main menu reset (run end cleanup)");
		AppliedTracker.Reset();
		IncomingDamageTracker.Reset();
		PlusState.ResetCombatDamage();
		PlayerDamageTracker.Hide();
		IncomingDamageOverlay.Detach();
		EndlessModeOverlay.Refresh();
		PlusLoc.MergeIntoModifiersTable();
	}
}
