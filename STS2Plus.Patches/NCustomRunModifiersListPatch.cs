using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Screens.MainMenu;
using STS2Plus.Config;
using STS2Plus.Localization;
using STS2Plus.Ui;

namespace STS2Plus.Patches;

[HarmonyPatchCategory("MoreRules")]
[HarmonyPatch(typeof(NCustomRunModifiersList), "_Ready")]
internal static class NCustomRunModifiersListPatch
{
	private static void Postfix(NCustomRunModifiersList __instance)
	{
		if (!ConfigManager.Current.MoreRulesEnabled)
		{
			return;
		}
		VBoxContainer val = MoreRulesUi.FindContentContainer((Control)(object)__instance);
		if (val == null)
		{
			return;
		}
		if (MoreRulesUi.FindRuleRow((Control)(object)__instance, "STS2PlusAttackDefenseRow") == null)
		{
			Control val2 = MoreRulesUi.CreateRuleRow(val, "STS2PlusAttackDefenseRow", PlusLoc.Text("RULE_ATTACK_DEFENSE_TITLE"), PlusLoc.Text("RULE_ATTACK_DEFENSE_DESC"));
			if (val2 != null)
			{
				((Node)val).AddChild((Node)(object)val2, false, (Node.InternalMode)0);
				((Node)val).MoveChild((Node)(object)val2, -1);
			}
		}
		if (MoreRulesUi.FindRuleRow((Control)(object)__instance, "STS2PlusAttackDefensePlusRow") == null)
		{
			Control val3 = MoreRulesUi.CreateRuleRow(val, "STS2PlusAttackDefensePlusRow", PlusLoc.Text("RULE_ATTACK_DEFENSE_PLUS_TITLE"), PlusLoc.Text("RULE_ATTACK_DEFENSE_PLUS_DESC"));
			if (val3 != null)
			{
				((Node)val).AddChild((Node)(object)val3, false, (Node.InternalMode)0);
				((Node)val).MoveChild((Node)(object)val3, -1);
			}
		}
		if (MoreRulesUi.FindRuleRow((Control)(object)__instance, "STS2PlusIronSkinRow") == null)
		{
			Control val4 = MoreRulesUi.CreateRuleRow(val, "STS2PlusIronSkinRow", PlusLoc.Text("RULE_IRON_SKIN_TITLE"), PlusLoc.Text("RULE_IRON_SKIN_DESC"));
			if (val4 != null)
			{
				((Node)val).AddChild((Node)(object)val4, false, (Node.InternalMode)0);
				((Node)val).MoveChild((Node)(object)val4, -1);
			}
		}
		if (MoreRulesUi.FindRuleRow((Control)(object)__instance, "STS2PlusGiantCreaturesRow") == null)
		{
			Control val5 = MoreRulesUi.CreateRuleRow(val, "STS2PlusGiantCreaturesRow", PlusLoc.Text("RULE_GIANT_CREATURES_TITLE"), PlusLoc.Text("RULE_GIANT_CREATURES_DESC"));
			if (val5 != null)
			{
				((Node)val).AddChild((Node)(object)val5, false, (Node.InternalMode)0);
				((Node)val).MoveChild((Node)(object)val5, -1);
			}
		}
		if (MoreRulesUi.FindRuleRow((Control)(object)__instance, "STS2PlusHardElitesRow") == null)
		{
			Control val6 = MoreRulesUi.CreateRuleRow(val, "STS2PlusHardElitesRow", PlusLoc.Text("RULE_HARD_ELITES_TITLE"), PlusLoc.Text("RULE_HARD_ELITES_DESC"));
			if (val6 != null)
			{
				((Node)val).AddChild((Node)(object)val6, false, (Node.InternalMode)0);
				((Node)val).MoveChild((Node)(object)val6, -1);
			}
		}
		if (MoreRulesUi.FindRuleRow((Control)(object)__instance, "STS2PlusEndlessModeRow") == null)
		{
			Control val7 = MoreRulesUi.CreateRuleRow(val, "STS2PlusEndlessModeRow", PlusLoc.Text("RULE_ENDLESS_TITLE"), PlusLoc.Text("RULE_ENDLESS_DESC"));
			if (val7 != null)
			{
				((Node)val).AddChild((Node)(object)val7, false, (Node.InternalMode)0);
				((Node)val).MoveChild((Node)(object)val7, -1);
			}
		}
		if (MoreRulesUi.FindRuleRow((Control)(object)__instance, "STS2PlusGlassCannonRow") == null)
		{
			Control val8 = MoreRulesUi.CreateRuleRow(val, "STS2PlusGlassCannonRow", PlusLoc.Text("RULE_GLASS_CANNON_TITLE"), PlusLoc.Text("RULE_GLASS_CANNON_DESC"));
			if (val8 != null)
			{
				((Node)val).AddChild((Node)(object)val8, false, (Node.InternalMode)0);
				((Node)val).MoveChild((Node)(object)val8, -1);
			}
		}
		if (MoreRulesUi.FindRuleRow((Control)(object)__instance, "STS2PlusUnlimitedGrowthRow") == null)
		{
			Control val9 = MoreRulesUi.CreateRuleRow(val, "STS2PlusUnlimitedGrowthRow", PlusLoc.Text("RULE_UNLIMITED_GROWTH_TITLE"), PlusLoc.Text("RULE_UNLIMITED_GROWTH_DESC"));
			if (val9 != null)
			{
				((Node)val).AddChild((Node)(object)val9, false, (Node.InternalMode)0);
				((Node)val).MoveChild((Node)(object)val9, -1);
			}
		}
		if (MoreRulesUi.FindRuleRow((Control)(object)__instance, "STS2PlusSandboxRow") == null)
		{
			Control val10 = MoreRulesUi.CreateRuleRow(val, "STS2PlusSandboxRow", PlusLoc.Text("RULE_SANDBOX_TITLE"), PlusLoc.Text("RULE_SANDBOX_DESC"));
			if (val10 != null)
			{
				((Node)val).AddChild((Node)(object)val10, false, (Node.InternalMode)0);
				((Node)val).MoveChild((Node)(object)val10, -1);
			}
		}
		if (MoreRulesUi.FindRuleRow((Control)(object)__instance, "STS2PlusBuildCreatorRow") == null)
		{
			Control val11 = MoreRulesUi.CreateRuleRow(val, "STS2PlusBuildCreatorRow", PlusLoc.Text("RULE_BUILD_CREATOR_TITLE"), PlusLoc.Text("RULE_BUILD_CREATOR_DESC"));
			if (val11 != null)
			{
				((Node)val).AddChild((Node)(object)val11, false, (Node.InternalMode)0);
				((Node)val).MoveChild((Node)(object)val11, -1);
			}
		}
		MoreRulesUi.Refresh((Control)(object)__instance);
	}
}
