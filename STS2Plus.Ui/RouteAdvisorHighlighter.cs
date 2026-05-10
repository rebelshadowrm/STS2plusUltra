using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Godot;
using HarmonyLib;
using STS2Plus.Config;
using STS2Plus.Features;
using STS2Plus.Reflection;

namespace STS2Plus.Ui;

internal static class RouteAdvisorHighlighter
{
	private sealed record PathEntry(object FromCoord, object ToCoord, IReadOnlyList<TextureRect> Segments);

	private sealed record VisualState(Color Modulate, Vector2 Scale)
	{
		[CompilerGenerated]
		private bool PrintMembers(StringBuilder builder)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			RuntimeHelpers.EnsureSufficientExecutionStack();
			builder.Append("Modulate = ");
			Color modulate = Modulate;
			builder.Append(modulate.ToString());
			builder.Append(", Scale = ");
			Vector2 scale = Scale;
			builder.Append(scale.ToString());
			return true;
		}
	}

	private static readonly ConditionalWeakTable<Node, Dictionary<TextureRect, VisualState>> VisualStateCache = new ConditionalWeakTable<Node, Dictionary<TextureRect, VisualState>>();

	private static readonly Color SafeColor = new Color(0.88f, 0.72f, 0.28f, 1f);

	private static readonly Color AggressiveColor = new Color(0.86f, 0.27f, 0.27f, 1f);

	private static readonly Color SharedColor = new Color(1f, 0.56f, 0.18f, 1f);

	public static void Refresh(Node mapScreen)
	{
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		Reset(mapScreen);
		if (!ConfigManager.Current.RouteAdvisorEnabled)
		{
			return;
		}
		RouteAdvice routeAdvice = RouteAdvisor.BuildAdvice();
		if (routeAdvice == null)
		{
			return;
		}
		IReadOnlyList<PathEntry> readOnlyList = ReadPathMap(mapScreen);
		if (readOnlyList.Count == 0)
		{
			return;
		}
		HashSet<TextureRect> hashSet = ((routeAdvice.Safe == null) ? new HashSet<TextureRect>() : CollectSegments(readOnlyList, routeAdvice.Safe));
		HashSet<TextureRect> hashSet2 = ((routeAdvice.Aggressive == null) ? new HashSet<TextureRect>() : CollectSegments(readOnlyList, routeAdvice.Aggressive));
		foreach (TextureRect item in hashSet)
		{
			((CanvasItem)item).Modulate = SafeColor;
		}
		foreach (TextureRect item2 in hashSet2)
		{
			((CanvasItem)item2).Modulate = (hashSet.Contains(item2) ? SharedColor : AggressiveColor);
		}
	}

	public static void Reset(Node? mapScreen = null)
	{
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		if (mapScreen == null || !GodotObject.IsInstanceValid((GodotObject)(object)mapScreen) || !VisualStateCache.TryGetValue(mapScreen, out Dictionary<TextureRect, VisualState> value))
		{
			return;
		}
		foreach (KeyValuePair<TextureRect, VisualState> item in value)
		{
			if (GodotObject.IsInstanceValid((GodotObject)(object)item.Key))
			{
				((CanvasItem)item.Key).Modulate = item.Value.Modulate;
				((Control)item.Key).Scale = item.Value.Scale;
			}
		}
	}

	private static HashSet<TextureRect> CollectSegments(IReadOnlyList<PathEntry> pathEntries, RouteSuggestion suggestion)
	{
		HashSet<TextureRect> hashSet = new HashSet<TextureRect>();
		object point = suggestion.StartPoint;
		foreach (object step in suggestion.Steps)
		{
			object mapPointCoord = GameReflection.GetMapPointCoord(point);
			object mapPointCoord2 = GameReflection.GetMapPointCoord(step);
			if (mapPointCoord == null || mapPointCoord2 == null)
			{
				point = step;
				continue;
			}
			foreach (PathEntry pathEntry in pathEntries)
			{
				if (!object.Equals(pathEntry.FromCoord, mapPointCoord) || !object.Equals(pathEntry.ToCoord, mapPointCoord2))
				{
					continue;
				}
				foreach (TextureRect segment in pathEntry.Segments)
				{
					hashSet.Add(segment);
				}
				break;
			}
			point = step;
		}
		return hashSet;
	}

	private static IReadOnlyList<PathEntry> ReadPathMap(Node mapScreen)
	{
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		if (!(AccessTools.Field(((object)mapScreen).GetType(), "_paths")?.GetValue(mapScreen) is IDictionary dictionary))
		{
			return Array.Empty<PathEntry>();
		}
		Dictionary<TextureRect, VisualState> orCreateValue = VisualStateCache.GetOrCreateValue(mapScreen);
		List<PathEntry> list = new List<PathEntry>();
		foreach (DictionaryEntry item in dictionary)
		{
			object obj = AccessTools.Field(item.Key.GetType(), "Item1")?.GetValue(item.Key) ?? AccessTools.Property(item.Key.GetType(), "Item1")?.GetValue(item.Key);
			object obj2 = AccessTools.Field(item.Key.GetType(), "Item2")?.GetValue(item.Key) ?? AccessTools.Property(item.Key.GetType(), "Item2")?.GetValue(item.Key);
			if (obj == null || obj2 == null || !(item.Value is IEnumerable source))
			{
				continue;
			}
			TextureRect[] array = source.OfType<TextureRect>().Where((Func<TextureRect, bool>)GodotObject.IsInstanceValid).ToArray();
			TextureRect[] array2 = array;
			foreach (TextureRect val in array2)
			{
				if (!orCreateValue.ContainsKey(val))
				{
					orCreateValue[val] = new VisualState(((CanvasItem)val).Modulate, ((Control)val).Scale);
				}
			}
			list.Add(new PathEntry(obj, obj2, array));
		}
		return list;
	}
}
