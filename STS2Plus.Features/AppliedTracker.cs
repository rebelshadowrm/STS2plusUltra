using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace STS2Plus.Features;

internal static class AppliedTracker
{
	private static readonly HashSet<int> GiantCreatureSet = new HashSet<int>();

	private static readonly HashSet<int> HardEliteSet = new HashSet<int>();

	private static readonly HashSet<int> EndlessScaledSet = new HashSet<int>();

	private static readonly HashSet<string> HardEliteRelicRewardSet = new HashSet<string>();

	private static readonly HashSet<string> GlassCannonRewardSet = new HashSet<string>();

	private static readonly HashSet<int> AttackDefenseSet = new HashSet<int>();

	private static readonly HashSet<int> GlassCannonSet = new HashSet<int>();

	public static bool IsGiantCreature(object instance)
	{
		return GiantCreatureSet.Contains(RuntimeHelpers.GetHashCode(instance));
	}

	public static bool MarkGiantCreature(object instance)
	{
		return GiantCreatureSet.Add(RuntimeHelpers.GetHashCode(instance));
	}

	public static bool MarkHardElite(object instance)
	{
		return HardEliteSet.Add(RuntimeHelpers.GetHashCode(instance));
	}

	public static bool MarkEndlessScaled(object instance)
	{
		return EndlessScaledSet.Add(RuntimeHelpers.GetHashCode(instance));
	}

	public static bool MarkHardEliteRelicReward(object room, object player)
	{
		return HardEliteRelicRewardSet.Add($"{RuntimeHelpers.GetHashCode(room)}:{RuntimeHelpers.GetHashCode(player)}");
	}

	public static bool MarkGlassCannonReward(object room, object player)
	{
		return GlassCannonRewardSet.Add($"{RuntimeHelpers.GetHashCode(room)}:{RuntimeHelpers.GetHashCode(player)}");
	}

	public static bool MarkAttackDefenseCard(object instance)
	{
		return AttackDefenseSet.Add(RuntimeHelpers.GetHashCode(instance));
	}

	public static bool MarkGlassCannonPlayer(object instance)
	{
		return GlassCannonSet.Add(RuntimeHelpers.GetHashCode(instance));
	}

	public static void Reset()
	{
		GiantCreatureSet.Clear();
		HardEliteSet.Clear();
		EndlessScaledSet.Clear();
		HardEliteRelicRewardSet.Clear();
		GlassCannonRewardSet.Clear();
		AttackDefenseSet.Clear();
		GlassCannonSet.Clear();
	}
}
