using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using STS2Plus.Config;
using STS2Plus.Reflection;

namespace STS2Plus.Patches;

[HarmonyPatchCategory("Core")]
[HarmonyPatch]
internal static class CardTotalDamageHoverPatch
{
	private const string TotalDamageLabelName = "STS2PlusCardTotalDamageLabel";

	private static readonly Type? CardModelType = AccessTools.TypeByName("MegaCrit.Sts2.Core.Models.CardModel");

	private static IEnumerable<MethodBase> TargetMethods()
	{
		return from method in typeof(NHoverTipSet).GetMethods(BindingFlags.Static | BindingFlags.Public)
			where method.Name == "CreateAndShow" && method.ReturnType == typeof(NHoverTipSet) && method.GetParameters().Length == 3 && typeof(IEnumerable).IsAssignableFrom(method.GetParameters()[1].ParameterType)
			select method;
	}

	private static void Postfix(NHoverTipSet __result, Control owner, IEnumerable hoverTips)
	{
		if (!ConfigManager.Current.CardTotalDamagePreviewEnabled)
		{
			return;
		}
		object obj = ResolveHoverTipModel(hoverTips) ?? ResolveOwnerModel(owner);
		if (obj == null)
		{
			return;
		}
		Type? cardModelType = CardModelType;
		if ((object)cardModelType == null || !cardModelType.IsInstanceOfType(obj) || !GameReflection.IsAttackCard(obj))
		{
			return;
		}
		int cardDisplayedDamage = GameReflection.GetCardDisplayedDamage(obj);
		int cardMultiPlayCount = GameReflection.GetCardMultiPlayCount(obj);
		if (cardDisplayedDamage > 0 && cardMultiPlayCount > 1)
		{
			Node textHoverTipContainer = GetTextHoverTipContainer(__result);
			Control val = (Control)(object)((textHoverTipContainer is Control) ? textHoverTipContainer : null);
			if (val != null)
			{
				ApplyTotalDamageLabel(val, cardDisplayedDamage * cardMultiPlayCount, cardMultiPlayCount);
			}
		}
	}

	private static object? ResolveHoverTipModel(IEnumerable hoverTips)
	{
		foreach (object hoverTip in hoverTips)
		{
			if (hoverTip != null)
			{
				object obj = AccessTools.Property(hoverTip.GetType(), "CanonicalModel")?.GetValue(hoverTip);
				if (obj != null)
				{
					return obj;
				}
				object obj2 = GameReflection.ResolveCanonicalModel(hoverTip);
				if (obj2 != null)
				{
					return obj2;
				}
			}
		}
		return null;
	}

	private static object? ResolveOwnerModel(Control owner)
	{
		return AccessTools.Property(((object)owner).GetType(), "CardModel")?.GetValue(owner) ?? GameReflection.ResolveCanonicalModel(owner);
	}

	private static Node? GetTextHoverTipContainer(NHoverTipSet hoverTipSet)
	{
		object? obj = AccessTools.Field(((object)hoverTipSet).GetType(), "_textHoverTipContainer")?.GetValue(hoverTipSet);
		return (Node?)(((obj is Node) ? obj : null) ?? FindNodeByName((Node)(object)hoverTipSet, "textHoverTipContainer") ?? FindNodeByName((Node)(object)hoverTipSet, "_textHoverTipContainer"));
	}

	private static void ApplyTotalDamageLabel(Control container, int totalDamage, int playCount)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Expected O, but got Unknown
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		Label val = ((Node)container).GetNodeOrNull<Label>((NodePath)"STS2PlusCardTotalDamageLabel");
		if (val == null)
		{
			val = new Label
			{
				Name = (StringName)"STS2PlusCardTotalDamageLabel",
				MouseFilter = (Control.MouseFilterEnum)2,
				FocusMode = (Control.FocusModeEnum)0,
				HorizontalAlignment = (HorizontalAlignment)0,
				VerticalAlignment = (VerticalAlignment)1,
				AutoTranslateMode = (Node.AutoTranslateModeEnum)2
			};
			((Control)val).AddThemeFontSizeOverride((StringName)"font_size", 16);
			((Control)val).AddThemeColorOverride((StringName)"font_color", new Color(1f, 0.82f, 0.45f, 1f));
			((Control)val).AddThemeColorOverride((StringName)"font_outline_color", new Color(0.08f, 0.04f, 0.02f, 0.9f));
			((Control)val).AddThemeConstantOverride((StringName)"outline_size", 4);
			((Node)container).AddChild((Node)(object)val, false, (Node.InternalMode)0);
		}
		val.Text = $"Total Damage = {totalDamage}";
		Control val2 = FindDescriptionAnchor(container) ?? FindBottomMostControl(container);
		float num = ((val2 != null) ? val2.Position.X : 0f);
		float num2 = ((val2 == null) ? 0f : (val2.Position.Y + val2.Size.Y + 6f));
		((Control)val).Position = new Vector2(num, num2);
		((Control)val).Size = new Vector2(Math.Max(220f, container.Size.X - num), 22f);
		((Control)val).TooltipText = $"Calculated from {playCount} hits.";
		((CanvasItem)val).Visible = true;
	}

	private static Control? FindDescriptionAnchor(Control container)
	{
		Node? obj = FindNodeByName((Node)(object)container, "Description");
		return (obj is Control ctl) ? ctl : null;
	}

	private static Control? FindBottomMostControl(Control root)
	{
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		Control val = null;
		foreach (Node child in ((Node)root).GetChildren(false))
		{
			Control val2 = (Control)(object)((child is Control) ? child : null);
			if (val2 != null && !(((Node)val2).Name == (StringName)"STS2PlusCardTotalDamageLabel"))
			{
				Control val3 = FindBottomMostControl(val2) ?? val2;
				if (val == null || val3.Position.Y + val3.Size.Y > val.Position.Y + val.Size.Y)
				{
					val = val3;
				}
			}
		}
		return val;
	}

	private static Node? FindNodeByName(Node root, string name)
	{
		if (root.Name == (StringName)(name))
		{
			return root;
		}
		foreach (Node child in root.GetChildren(false))
		{
			if (child == null)
			{
				continue;
			}
			Node val = FindNodeByName(child, name);
			if (val != null)
			{
				return val;
			}
		}
		return null;
	}
}
