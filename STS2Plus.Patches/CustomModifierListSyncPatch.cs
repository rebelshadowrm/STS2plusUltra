using System.Collections.Generic;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Screens.MainMenu;
using STS2Plus.Config;
using STS2Plus.Ui;

namespace STS2Plus.Patches;

[HarmonyPatchCategory("MoreRules")]
[HarmonyPatch(typeof(NCustomRunModifiersList), "SyncModifierList")]
internal static class CustomModifierListSyncPatch
{
	private static void Postfix(NCustomRunModifiersList __instance, IReadOnlyList<ModifierModel> modifiers)
	{
		if (!ConfigManager.Current.MoreRulesEnabled)
		{
			PlusState.ClearGameplayRuleSelections();
			MoreRulesUi.Refresh((Control)(object)__instance);
		}
		else
		{
			PlusState.SyncRuleSelections(modifiers);
			MoreRulesUi.Refresh((Control)(object)__instance);
		}
	}
}
