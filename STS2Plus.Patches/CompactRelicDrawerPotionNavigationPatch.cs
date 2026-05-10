using System.Collections;
using System.Reflection;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Potions;
using STS2Plus.Ui;

namespace STS2Plus.Patches;

[HarmonyPatchCategory("Core")]
[HarmonyPatch(typeof(NPotionContainer), "UpdateNavigation")]
internal static class CompactRelicDrawerPotionNavigationPatch
{
	private static readonly FieldInfo? HoldersField = AccessTools.Field(typeof(NPotionContainer), "_holders");

	private static void Postfix(NPotionContainer __instance)
	{
		NRun instance = NRun.Instance;
		Control primaryControl = CompactRelicDrawer.GetPrimaryControl((instance != null) ? instance.GlobalUi : null);
		if (primaryControl == null || !GodotObject.IsInstanceValid((GodotObject)(object)primaryControl) || !(HoldersField?.GetValue(__instance) is IList list))
		{
			return;
		}
		foreach (object item in list)
		{
			Control val = (Control)((item is Control) ? item : null);
			if (val != null && GodotObject.IsInstanceValid((GodotObject)(object)val))
			{
				val.FocusNeighborBottom = ((Node)primaryControl).GetPath();
			}
		}
	}
}
