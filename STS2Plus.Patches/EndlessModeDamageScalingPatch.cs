using System;
using HarmonyLib;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using STS2Plus.Reflection;

namespace STS2Plus.Patches;

// Scales enemy damage output in endless mode by 1 + (actNumber - 3) * 0.2.
// Acts 1-3 are unscaled (multiplier = 1.0). Act 4 = x1.2, act 8 = x2.0, etc.
//
// AttackIntent.GetSingleDamage is the non-virtual base method that returns per-hit
// damage for all intent types:
//   - SingleAttackIntent.GetTotalDamage → GetSingleDamage (1 hit)
//   - MultiAttackIntent: combat calls GetSingleDamage per hit × Repeats
// Patching GetSingleDamage scales both single and multi-hit enemies correctly with
// one patch on the base class (no HarmonyTargetMethods needed).
[HarmonyPatchCategory("MoreRules")]
[HarmonyPatch(typeof(AttackIntent), "GetSingleDamage")]
internal static class EndlessModeDamageScalingPatch
{
    private static void Postfix(ref int __result)
    {
        if (!PlusState.IsEndlessModeActive())
            return;

        decimal multiplier = GameReflection.GetEndlessDamageMultiplier();
        if (multiplier <= 1.0m)
            return;

        int original = __result;
        __result = (int)Math.Round(__result * multiplier, MidpointRounding.AwayFromZero);
        ModEntry.Verbose($"EndlessModeDamageScaling: GetSingleDamage {original} → {__result} (x{multiplier:F2})");
    }
}
