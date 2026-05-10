using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using STS2Plus.Reflection;
using STS2Plus.Ui;

namespace STS2Plus.Features;

internal static class IncomingDamageTracker
{
	private sealed class ReferenceEqualityComparer : IEqualityComparer<object>
	{
		public static readonly ReferenceEqualityComparer Instance = new ReferenceEqualityComparer();

		public new bool Equals(object? x, object? y)
		{
			return x == y;
		}

		public int GetHashCode(object obj)
		{
			return RuntimeHelpers.GetHashCode(obj);
		}
	}

	private sealed class TargetKeyComparer : IEqualityComparer<object>
	{
		public static readonly TargetKeyComparer Instance = new TargetKeyComparer();

		public new bool Equals(object? x, object? y)
		{
			if (x is string a && y is string b)
			{
				return string.Equals(a, b, StringComparison.Ordinal);
			}
			return x == y;
		}

		public int GetHashCode(object obj)
		{
			return (obj is string obj2) ? StringComparer.Ordinal.GetHashCode(obj2) : RuntimeHelpers.GetHashCode(obj);
		}
	}

	private static readonly object Sync = new object();

	private static readonly Dictionary<object, IReadOnlyDictionary<object, int>> Snapshots = new Dictionary<object, IReadOnlyDictionary<object, int>>(ReferenceEqualityComparer.Instance);

	public static void UpdateIntent(object? intent, IEnumerable? targets, object? owner)
	{
		object intent2 = intent;
		object owner2 = owner;
		if (owner2 == null)
		{
			return;
		}
		(object, object)[] source = targets != null
			? (from object target in targets
				select (Original: target, Key: NormalizeTarget(target)) into pair
				where pair.Key != null
				select pair).ToArray()
			: Array.Empty<(object, object)>();
		Dictionary<object, int> dictionary = source.GroupBy(((object Original, object Key) pair) => pair.Key, TargetKeyComparer.Instance).ToDictionary((IGrouping<object, (object Original, object Key)> group) => group.Key, (IGrouping<object, (object Original, object Key)> group) => GameReflection.GetIntentTotalDamage(intent2, new object[1] { group.First().Original }, owner2), TargetKeyComparer.Instance);
		lock (Sync)
		{
			if (dictionary.Count == 0 || dictionary.Values.All((int damage) => damage <= 0))
			{
				Snapshots.Remove(owner2);
			}
			else
			{
				Snapshots[owner2] = dictionary;
			}
		}
		if (dictionary.Count > 0 && dictionary.Values.Any((int damage) => damage > 0))
		{
			ModEntry.Logger.Info("STS2Plus incoming-damage snapshot owner=" + DescribeKey(owner2) + " targets=" + string.Join(", ", dictionary.Select((KeyValuePair<object, int> pair) => $"{pair.Key}:{pair.Value}")), 1);
		}
		IncomingDamageOverlay.RequestRefresh();
	}

	public static int GetIncomingDamageFor(object? target)
	{
		if (target == null)
		{
			return 0;
		}
		object obj = NormalizeTarget(target);
		if (obj == null)
		{
			return 0;
		}
		lock (Sync)
		{
			int num = 0;
			foreach (IReadOnlyDictionary<object, int> value2 in Snapshots.Values)
			{
				if (value2.TryGetValue(obj, out var value) && value > 0)
				{
					num += value;
				}
			}
			return num;
		}
	}

	public static void ClearOwner(object? owner)
	{
		if (owner != null)
		{
			lock (Sync)
			{
				Snapshots.Remove(owner);
			}
			IncomingDamageOverlay.RequestRefresh();
		}
	}

	public static void Reset()
	{
		lock (Sync)
		{
			Snapshots.Clear();
		}
		IncomingDamageOverlay.RequestRefresh();
	}

	private static object? NormalizeTarget(object? target)
	{
		if (target == null)
		{
			return null;
		}
		object obj = GameReflection.GetCreatureEntity(target) ?? target;
		if (GameReflection.IsAnyPlayerCreature(obj))
		{
			string playerStateKey = GameReflection.GetPlayerStateKey(obj);
			if (!string.IsNullOrWhiteSpace(playerStateKey))
			{
				return "player:" + playerStateKey;
			}
		}
		return obj;
	}

	private static string DescribeKey(object? value)
	{
		if (value == null)
		{
			return "<null>";
		}
		if (value is string result)
		{
			return result;
		}
		string playerStateKey = GameReflection.GetPlayerStateKey(value);
		if (!string.IsNullOrWhiteSpace(playerStateKey))
		{
			return playerStateKey;
		}
		return value.GetType().Name;
	}
}
