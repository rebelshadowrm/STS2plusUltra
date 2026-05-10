using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using STS2Plus.Reflection;

namespace STS2Plus.Patches;

[HarmonyPatchCategory("Core")]
[HarmonyPatch]
internal static class RelicHoverTipLayerPatch
{
	private const int RelicHoverTipZIndex = 1000;

	private static readonly Type? RelicModelType = AccessTools.TypeByName("MegaCrit.Sts2.Core.Models.RelicModel");

	[HarmonyTargetMethods]
	private static IEnumerable<MethodBase> TargetMethods()
	{
		return from method in typeof(NHoverTipSet).GetMethods(BindingFlags.Static | BindingFlags.Public)
			where method.Name == "CreateAndShow" && method.ReturnType == typeof(NHoverTipSet) && method.GetParameters().Length == 3 && typeof(IEnumerable).IsAssignableFrom(method.GetParameters()[1].ParameterType)
			select method;
	}

	private static void Postfix(NHoverTipSet __result, Control owner, IEnumerable hoverTips)
	{
		if (ShouldElevate(owner, hoverTips))
		{
			((CanvasItem)__result).ZIndex = 1000;
			Node parent = ((Node)__result).GetParent();
			CanvasItem val = (CanvasItem)(object)((parent is CanvasItem) ? parent : null);
			if (val != null)
			{
				val.ZIndex = Math.Max(val.ZIndex, 999);
			}
			Node parent2 = ((Node)__result).GetParent();
			if (parent2 != null)
			{
				parent2.MoveChild((Node)(object)__result, -1);
			}
		}
	}

	private static bool ShouldElevate(Control owner, IEnumerable hoverTips)
	{
		if (IsRelicOwner(owner))
		{
			return true;
		}
		object obj = ResolveHoverTipModel(hoverTips) ?? ResolveOwnerModel(owner);
		return obj != null && (RelicModelType?.IsInstanceOfType(obj) ?? false);
	}

	private static bool IsRelicOwner(Control owner)
	{
		string text = ((object)owner).GetType().FullName ?? ((object)owner).GetType().Name;
		return text.Contains("Relic", StringComparison.Ordinal);
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
		return AccessTools.Property(((object)owner).GetType(), "Relic")?.GetValue(owner) ?? AccessTools.Property(((object)owner).GetType(), "RelicModel")?.GetValue(owner) ?? GameReflection.ResolveCanonicalModel(owner);
	}
}
