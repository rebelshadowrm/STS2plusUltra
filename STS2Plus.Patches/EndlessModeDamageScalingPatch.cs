using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Combat;
using STS2Plus.Reflection;

namespace STS2Plus.Patches;

// Scales enemy damage output in endless mode by 1 + (actNumber - 3) * 0.2.
// Acts 1-3 are unscaled (multiplier = 1.0). Act 4 = x1.2, act 8 = x2.0, etc.
// Patches NCreature.PerformIntent: scales intent damage fields before execution,
// restores them after so the base intent values remain unchanged for future use.
[HarmonyPatchCategory("MoreRules")]
[HarmonyPatch(typeof(NCreature), "PerformIntent")]
internal static class EndlessModeDamageScalingPatch
{
    private static readonly string[] IntentPropertyNames =
        { "CurrentIntent", "ActiveIntent", "Intent", "CurrentMove" };

    private static readonly string[] DamageFieldNames =
        { "_damage", "_baseDamage", "BaseDamage", "Damage", "_hitDamage", "HitDamage" };

    private static void Prefix(NCreature __instance, out (object intent, List<(FieldInfo field, int original)> scaled)? __state)
    {
        __state = null;
        if (!PlusState.IsEndlessModeActive())
            return;

        object? entity = __instance.Entity;
        if (entity == null || !GameReflection.IsEnemyCreature(entity))
            return;

        decimal multiplier = GameReflection.GetEndlessDamageMultiplier();
        if (multiplier <= 1.0m)
            return;

        object? intent = FindCurrentIntent(entity);
        if (intent == null)
        {
            ModEntry.Verbose("EndlessModeDamageScaling: could not find current intent on entity");
            return;
        }

        var scaled = new List<(FieldInfo, int)>();
        foreach (string name in DamageFieldNames)
        {
            FieldInfo? field = AccessTools.Field(intent.GetType(), name);
            if (field?.FieldType != typeof(int))
                continue;
            int original = (int)field.GetValue(intent)!;
            if (original <= 0)
                continue;
            int scaledVal = (int)Math.Round((decimal)original * multiplier, MidpointRounding.AwayFromZero);
            field.SetValue(intent, scaledVal);
            scaled.Add((field, original));
        }

        if (scaled.Count > 0)
        {
            ModEntry.Verbose($"EndlessModeDamageScaling: scaled {scaled.Count} field(s) on {intent.GetType().Name} by x{multiplier:F2}");
            __state = (intent, scaled);
        }
        else
        {
            ModEntry.Verbose($"EndlessModeDamageScaling: no damage fields found on {intent.GetType().Name}");
        }
    }

    private static void Postfix((object intent, List<(FieldInfo field, int original)> scaled)? __state)
    {
        if (__state == null)
            return;
        foreach (var (field, original) in __state.Value.scaled)
            field.SetValue(__state.Value.intent, original);
    }

    private static object? FindCurrentIntent(object entity)
    {
        foreach (string name in IntentPropertyNames)
        {
            object? val = AccessTools.Property(entity.GetType(), name)?.GetValue(entity)
                ?? AccessTools.Field(entity.GetType(), "_" + char.ToLower(name[0]) + name.Substring(1))?.GetValue(entity);
            if (val != null)
                return val;
        }
        return null;
    }
}
