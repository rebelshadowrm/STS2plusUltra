using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using STS2Plus.Config;
using STS2Plus.Reflection;

namespace STS2Plus.Patches;

[HarmonyPatchCategory("Core")]
[HarmonyPatch]
internal static class HoverTipRarityPatch
{
	private readonly record struct RarityTag(string Text, Color Color)
	{
		[CompilerGenerated]
		private bool PrintMembers(StringBuilder builder)
		{
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			builder.Append("Text = ");
			builder.Append((object?)Text);
			builder.Append(", Color = ");
			Color color = Color;
			builder.Append(color.ToString());
			return true;
		}
	}

	private const string RarityTagName = "STS2PlusRarityTag";

	private static readonly Type? CardModelType = AccessTools.TypeByName("MegaCrit.Sts2.Core.Models.CardModel");

	private static readonly Type? PotionModelType = AccessTools.TypeByName("MegaCrit.Sts2.Core.Models.PotionModel");

	private static readonly Type? RelicModelType = AccessTools.TypeByName("MegaCrit.Sts2.Core.Models.RelicModel");

	private static readonly Color CommonColor = new Color(0.62f, 0.62f, 0.62f, 1f);

	private static readonly Color UncommonColor = new Color(0.34f, 0.82f, 0.38f, 1f);

	private static readonly Color RareColor = new Color(0.34f, 0.63f, 1f, 1f);

	private static readonly Color LegendaryColor = new Color(1f, 0.56f, 0.16f, 1f);

	private static readonly Color ShopColor = new Color(0.96f, 0.82f, 0.24f, 1f);

	private static IEnumerable<MethodBase> TargetMethods()
	{
		return from method in typeof(NHoverTipSet).GetMethods(BindingFlags.Static | BindingFlags.Public)
			where method.Name == "CreateAndShow" && method.ReturnType == typeof(NHoverTipSet) && method.GetParameters().Length == 3 && typeof(IEnumerable).IsAssignableFrom(method.GetParameters()[1].ParameterType)
			select method;
	}

	private static void Postfix(NHoverTipSet __result, Control owner, IEnumerable hoverTips)
	{
		if (!ConfigManager.Current.RarityTagsEnabled)
		{
			return;
		}
		object model = ResolveHoverTipModel(hoverTips) ?? ResolveOwnerModel(owner);
		RarityTag? rarityTag = GetRarityTag(model);
		if (!rarityTag.HasValue)
		{
			return;
		}
		Node textHoverTipContainer = GetTextHoverTipContainer(__result);
		if (textHoverTipContainer != null)
		{
			Node val = FindNodeByName(textHoverTipContainer, "Title") ?? FindNodeByName(textHoverTipContainer, "%Title");
			Control val2 = (Control)(object)((val is Control) ? val : null);
			if (val2 != null)
			{
				ApplyRarityTag(val2, rarityTag.Value);
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

	private static RarityTag? GetRarityTag(object? model)
	{
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
		if (model == null)
		{
			return null;
		}
		Type type = model.GetType();
		string text = AccessTools.Property(type, "Rarity")?.GetValue(model)?.ToString();
		if (string.IsNullOrWhiteSpace(text))
		{
			return null;
		}
		Type? cardModelType = CardModelType;
		RarityTag? result;
		if ((object)cardModelType != null && cardModelType.IsAssignableFrom(type))
		{
			result = text switch
			{
				"Basic" => new RarityTag("Common", CommonColor),
				"Common" => new RarityTag("Common", CommonColor),
				"Uncommon" => new RarityTag("Uncommon", UncommonColor),
				"Rare" => new RarityTag("Rare", RareColor),
				"Ancient" => new RarityTag("Legendary", LegendaryColor),
				_ => null,
			};
			return result;
		}
		Type? potionModelType = PotionModelType;
		if ((object)potionModelType != null && potionModelType.IsAssignableFrom(type))
		{
			result = text switch
			{
				"Common" => new RarityTag("Common", CommonColor),
				"Uncommon" => new RarityTag("Uncommon", UncommonColor),
				"Rare" => new RarityTag("Rare", RareColor),
				_ => null,
			};
			return result;
		}
		Type? relicModelType = RelicModelType;
		if ((object)relicModelType == null || !relicModelType.IsAssignableFrom(type))
		{
			return null;
		}
		result = text switch
		{
			"Starter" => new RarityTag("Common", CommonColor),
			"Common" => new RarityTag("Common", CommonColor),
			"Uncommon" => new RarityTag("Uncommon", UncommonColor),
			"Rare" => new RarityTag("Rare", RareColor),
			"Shop" => new RarityTag("Shop", ShopColor),
			"Ancient" => new RarityTag("Legendary", LegendaryColor),
			_ => null,
		};
		return result;
	}

	private static Node? GetTextHoverTipContainer(NHoverTipSet hoverTipSet)
	{
		object? obj = AccessTools.Field(((object)hoverTipSet).GetType(), "_textHoverTipContainer")?.GetValue(hoverTipSet);
		return (Node?)(((obj is Node) ? obj : null) ?? FindNodeByName((Node)(object)hoverTipSet, "textHoverTipContainer") ?? FindNodeByName((Node)(object)hoverTipSet, "_textHoverTipContainer"));
	}

	private static void ApplyRarityTag(Control titleControl, RarityTag rarityTag)
	{
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Expected O, but got Unknown
		Node parent = ((Node)titleControl).GetParent();
		Control val = (Control)(object)((parent is Control) ? parent : null);
		if (val != null)
		{
			Label val2 = ((Node)val).GetNodeOrNull<Label>((NodePath)"STS2PlusRarityTag");
			if (val2 == null)
			{
				val2 = new Label
				{
					Name = (StringName)"STS2PlusRarityTag",
					MouseFilter = (Control.MouseFilterEnum)2,
					FocusMode = (Control.FocusModeEnum)0,
					HorizontalAlignment = (HorizontalAlignment)0,
					VerticalAlignment = (VerticalAlignment)1
				};
				((Node)val).AddChild((Node)(object)val2, false, (Node.InternalMode)0);
			}
			val2.Text = "[" + rarityTag.Text + "]";
			((CanvasItem)val2).Modulate = rarityTag.Color;
			((Control)val2).Position = new Vector2(titleControl.Position.X + titleControl.Size.X + 8f, titleControl.Position.Y - 1f);
			((Control)val2).Size = new Vector2(160f, Mathf.Max(22f, titleControl.Size.Y + 4f));
			((CanvasItem)val2).Visible = true;
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
			Node val = FindNodeByName(child, name);
			if (val != null)
			{
				return val;
			}
		}
		return null;
	}
}
