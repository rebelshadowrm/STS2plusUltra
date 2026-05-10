using System;
using System.Reflection;
using HarmonyLib;
using STS2Plus.Reflection;

namespace STS2Plus.Patches;

[HarmonyPatchCategory("MoreRules")]
[HarmonyPatch]
internal static class GlassCannonBlockRetentionPatch
{
	private static MethodBase? TargetMethod()
	{
		return (GameReflection.CreatureRuntimeType == null) ? null : AccessTools.Method(GameReflection.CreatureRuntimeType, "PrepareForNextTurn", (Type[])null, (Type[])null);
	}

	private static void Prefix(object __instance, ref int __state)
	{
		__state = 0;
		if (PlusState.IsGlassCannonActive() && GameReflection.IsAnyPlayerCreature(__instance))
		{
			__state = GameReflection.GetCurrentBlock(__instance);
			ModEntry.Verbose($"GlassCannonBlockRetention: saving block before turn block={__state}");
		}
	}

	private static void Postfix(object __instance, int __state)
	{
		if (__state > 0 && PlusState.IsGlassCannonActive() && GameReflection.IsAnyPlayerCreature(__instance) && !GameReflection.ShouldGamePreventBlockClear(__instance))
		{
			ModEntry.Verbose($"GlassCannonBlockRetention: retaining block retained={Math.Min(__state, 15)} cap=15");
			GameReflection.SetCurrentBlock(__instance, Math.Min(__state, 15));
		}
	}
}
