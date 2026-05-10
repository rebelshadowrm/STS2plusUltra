using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using STS2Plus.Localization;

namespace STS2Plus.Patches;

[HarmonyPatchCategory("MoreRules")]
[HarmonyPatch]
internal static class DeprecatedModifierHoverTipPatch
{
	private const string GenericEntry = "DEPRECATED_MODIFIER";

	private const string DisplayTitleLabelName = "STS2PlusModifierTitle";

	private const string DisplayDescriptionLabelName = "STS2PlusModifierDescription";

	[HarmonyTargetMethods]
	private static IEnumerable<MethodBase> TargetMethods()
	{
		return from method in typeof(NHoverTipSet).GetMethods(BindingFlags.Static | BindingFlags.Public)
			where method.Name == "CreateAndShow" && method.ReturnType == typeof(NHoverTipSet) && method.GetParameters().Length == 3 && typeof(IEnumerable).IsAssignableFrom(method.GetParameters()[1].ParameterType)
			select method;
	}

	private static void Postfix(NHoverTipSet __result, Control owner)
	{
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		if (!DeprecatedModifierTopBarPatch.TryGetDisplayEntry(owner, out string entry))
		{
			return;
		}
		object? obj = AccessTools.Field(((object)__result).GetType(), "_textHoverTipContainer")?.GetValue(__result);
		Node val = (Node)(((obj is Node) ? obj : null) ?? FindNodeByName((Node)(object)__result, "textHoverTipContainer") ?? FindNodeByName((Node)(object)__result, "_textHoverTipContainer"));
		Control val2 = (Control)(object)((val is Control) ? val : null);
		if (val2 != null)
		{
			string text = PlusLoc.Text(PlusLoc.ModifierTitleKey(entry));
			string text2 = PlusLoc.Text(PlusLoc.ModifierDescriptionKey(entry));
			if (string.Equals(entry, "DEPRECATED_MODIFIER", StringComparison.Ordinal))
			{
				text = PlusLoc.GenericModifierTitle();
				text2 = PlusLoc.GenericModifierDescription();
			}
			string text3 = text;
			Node? obj2 = FindNodeByName((Node)(object)val2, "Title");
			ApplyOrCreateLabel(val2, "STS2PlusModifierTitle", text3, (Control?)(object)((obj2 is Control) ? obj2 : null), 20, Colors.White);
			string text4 = text2;
			Node? obj3 = FindNodeByName((Node)(object)val2, "Description");
			ApplyOrCreateLabel(val2, "STS2PlusModifierDescription", text4, (Control?)(object)((obj3 is Control) ? obj3 : null), 16, new Color(0.88f, 0.88f, 0.88f, 1f));
		}
	}

	private static void ApplyOrCreateLabel(Control container, string nodeName, string text, Control? anchor, int fontSize, Color color)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Expected O, but got Unknown
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		if (anchor != null)
		{
			SetText(anchor, text);
			((CanvasItem)anchor).Visible = true;
			return;
		}
		Label val = ((Node)container).GetNodeOrNull<Label>((NodePath)(nodeName));
		if (val == null)
		{
			val = new Label
			{
				Name = (StringName)(nodeName),
				MouseFilter = (Control.MouseFilterEnum)2,
				FocusMode = (Control.FocusModeEnum)0,
				HorizontalAlignment = (HorizontalAlignment)0,
				VerticalAlignment = (VerticalAlignment)1,
				AutoTranslateMode = (Node.AutoTranslateModeEnum)2,
				AutowrapMode = (TextServer.AutowrapMode)3
			};
			((Control)val).AddThemeFontSizeOverride((StringName)"font_size", fontSize);
			((Control)val).AddThemeColorOverride((StringName)"font_color", color);
			((Node)container).AddChild((Node)(object)val, false, (Node.InternalMode)0);
		}
		val.Text = text;
		((Control)val).Position = (Vector2)((anchor != null) ? anchor.Position : new Vector2(0f, (nodeName == "STS2PlusModifierTitle") ? 0f : 28f));
		((Control)val).Size = new Vector2(Math.Max(260f, container.Size.X), (nodeName == "STS2PlusModifierTitle") ? 26f : 72f);
		((CanvasItem)val).Visible = true;
	}

	private static void SetText(object target, string text)
	{
		MethodInfo methodInfo = AccessTools.Method(target.GetType(), "SetTextAutoSize", new Type[1] { typeof(string) }, (Type[])null);
		if (methodInfo != null)
		{
			methodInfo.Invoke(target, new object[1] { text });
		}
		else
		{
			AccessTools.Property(target.GetType(), "BbcodeEnabled")?.SetValue(target, true);
			AccessTools.Property(target.GetType(), "Text")?.SetValue(target, text);
			AccessTools.Property(target.GetType(), "TooltipText")?.SetValue(target, text);
		}
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
			Node root2 = child;
			if (true)
			{
				Node val = FindNodeByName(root2, name);
				if (val != null)
				{
					return val;
				}
			}
		}
		return null;
	}
}
