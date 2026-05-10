using System.Collections.Generic;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Screens.MainMenu;
using STS2Plus.Config;
using STS2Plus.Modifiers;

namespace STS2Plus.Patches;

[HarmonyPatchCategory("MoreRules")]
[HarmonyPatch(typeof(NCustomRunModifiersList), "GetModifiersTickedOn")]
internal static class CustomModifierListPatch
{
	private static void Postfix(NCustomRunModifiersList __instance, ref List<ModifierModel> __result)
	{
		if (!ConfigManager.Current.MoreRulesEnabled)
		{
			__result.RemoveAll((ModifierModel modifier) => CustomModifierCatalog.IsKnownModifier(modifier));
		}
		else if (MultiplayerSafety.ShouldInjectGameplayRules((Node?)(object)__instance))
		{
			__result.RemoveAll((ModifierModel modifier) => CustomModifierCatalog.IsKnownModifier(modifier));
			AppendIfSelected(__result, PlusState.AttackDefenseRuleSelected, "ATTACK_DEFENSE");
			AppendIfSelected(__result, PlusState.AttackDefensePlusRuleSelected, "ATTACK_DEFENSE_PLUS");
			AppendIfSelected(__result, PlusState.IronSkinRuleSelected, "IRON_SKIN");
			AppendIfSelected(__result, PlusState.GiantCreaturesRuleSelected, "GIANT_CREATURES");
			AppendIfSelected(__result, PlusState.HardElitesRuleSelected, "HARD_ELITES");
			AppendIfSelected(__result, PlusState.EndlessModeSelected, "ENDLESS_MODE");
			AppendIfSelected(__result, PlusState.GlassCannonRuleSelected, "GLASS_CANNON");
			AppendIfSelected(__result, PlusState.UnlimitedGrowthRuleSelected, "UNLIMITED_GROWTH");
			AppendIfSelected(__result, PlusState.SandboxRuleSelected, "SANDBOX");
			AppendIfSelected(__result, PlusState.BuildCreatorRuleSelected, "BUILD_CREATOR");
		}
	}

	private static void AppendIfSelected(List<ModifierModel> modifiers, bool enabled, string entry)
	{
		if (enabled && !CustomModifierCatalog.ContainsEntry(modifiers, entry))
		{
			modifiers.Add(CustomModifierCatalog.Create(entry));
		}
	}
}
