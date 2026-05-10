using System;
using System.Reflection;
using HarmonyLib;
using STS2Plus.Reflection;

namespace STS2Plus.Patches;

[HarmonyPatchCategory("MoreRules")]
[HarmonyPatch]
internal static class GlassCannonMaxHpSyncPatch
{
	private static MethodBase? TargetMethod()
	{
		Type type = RuntimeTypeResolver.FindType("MegaCrit.Sts2.Core.Commands.CreatureCmd");
		return (type == null || GameReflection.CreatureRuntimeType == null) ? null : AccessTools.Method(type, "SetMaxHp", new Type[2]
		{
			GameReflection.CreatureRuntimeType,
			typeof(decimal)
		}, (Type[])null);
	}

	private static void Postfix(object creature)
	{
		if (PlusState.IsGlassCannonActive() && GameReflection.IsPlayerCreature(creature) && (PlusState.TryGetGlassCannonExpectedMaxHp(creature, out var _) || !GameReflection.ShouldApplyGlassCannon()))
		{
			int maxHp = GameReflection.GetMaxHp(creature);
			if (maxHp > 0)
			{
				PlusState.RememberGlassCannonExpectedMaxHp(creature, maxHp);
			}
		}
	}
}
