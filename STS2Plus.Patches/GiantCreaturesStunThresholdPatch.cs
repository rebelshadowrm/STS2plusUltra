using System;
using System.Reflection;
using HarmonyLib;
using STS2Plus.Features;
using STS2Plus.Reflection;

namespace STS2Plus.Patches;

// When Giant Creatures doubles enemy HP, stun/phase-transition HP thresholds inside
// ConditionalBranchState predicates would fire at half the intended HP ratio because
// the predicates compare against hardcoded absolute HP values. Temporarily halving
// both _currentHp and _maxHp during predicate evaluation makes all threshold checks
// (absolute or percentage) see vanilla-equivalent values.
[HarmonyPatchCategory("MoreRules")]
[HarmonyPatch]
internal static class GiantCreaturesStunThresholdPatch
{
	private static readonly Type? ConditionalBranchStateType =
		AccessTools.TypeByName("MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine.ConditionalBranchState");

	private static readonly Type? CreatureType =
		AccessTools.TypeByName("MegaCrit.Sts2.Core.Entities.Creatures.Creature");

	private static readonly FieldInfo? CurrentHpField =
		CreatureType != null ? AccessTools.Field(CreatureType, "_currentHp") : null;

	private static readonly FieldInfo? MaxHpField =
		CreatureType != null ? AccessTools.Field(CreatureType, "_maxHp") : null;

	private static MethodBase? TargetMethod()
	{
		return ConditionalBranchStateType != null
			? AccessTools.Method(ConditionalBranchStateType, "GetNextState")
			: null;
	}

	private static void Prefix(object? __0, out (int hp, int maxHp)? __state)
	{
		__state = null;
		if (__0 == null || CurrentHpField == null || MaxHpField == null)
			return;
		if (!GameReflection.IsEnemyCreature(__0))
			return;

		// Build the total HP multiplier actually applied to this creature.
		decimal totalMultiplier = 1.0m;
		if (PlusState.IsGiantCreaturesActive() && AppliedTracker.IsGiantCreature(__0))
			totalMultiplier *= 2.0m;
		if (PlusState.IsHardElitesActive() && AppliedTracker.IsHardElite(__0))
			totalMultiplier *= 1.5m;
		if (PlusState.IsEndlessModeActive() && AppliedTracker.IsEndlessScaled(__0))
			totalMultiplier *= GameReflection.GetEndlessHpMultiplier();

		if (totalMultiplier <= 1.0m)
			return;

		int hp = (int)CurrentHpField.GetValue(__0)!;
		int maxHp = (int)MaxHpField.GetValue(__0)!;
		if (hp <= 0 || maxHp <= 1)
			return;

		__state = (hp, maxHp);
		CurrentHpField.SetValue(__0, Math.Max(1, (int)Math.Round(hp / totalMultiplier, MidpointRounding.AwayFromZero)));
		MaxHpField.SetValue(__0, Math.Max(1, (int)Math.Round(maxHp / totalMultiplier, MidpointRounding.AwayFromZero)));
	}

	private static void Postfix(object? __0, (int hp, int maxHp)? __state)
	{
		if (__state.HasValue && __0 != null && CurrentHpField != null && MaxHpField != null)
		{
			CurrentHpField.SetValue(__0, __state.Value.hp);
			MaxHpField.SetValue(__0, __state.Value.maxHp);
		}
	}
}
