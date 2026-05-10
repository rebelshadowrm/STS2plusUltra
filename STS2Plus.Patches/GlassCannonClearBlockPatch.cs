using System;
using System.Reflection;
using System.Threading.Tasks;
using HarmonyLib;
using STS2Plus.Reflection;

namespace STS2Plus.Patches;

[HarmonyPatchCategory("MoreRules")]
[HarmonyPatch]
internal static class GlassCannonClearBlockPatch
{
	private static MethodBase? TargetMethod()
	{
		return (GameReflection.CreatureRuntimeType == null) ? null : AccessTools.Method(GameReflection.CreatureRuntimeType, "ClearBlock", (Type[])null, (Type[])null);
	}

	private static bool Prefix(object __instance, ref Task? __result)
	{
		if (!PlusState.IsGlassCannonActive() || !GameReflection.IsAnyPlayerCreature(__instance))
		{
			return true;
		}
		if (GameReflection.ShouldGamePreventBlockClear(__instance))
		{
			return true;
		}
		int value = Math.Min(GameReflection.GetCurrentBlock(__instance), 15);
		GameReflection.SetCurrentBlock(__instance, value);
		__result = Task.CompletedTask;
		return false;
	}
}
