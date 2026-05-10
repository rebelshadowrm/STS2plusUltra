using System;
using System.Reflection;
using HarmonyLib;
using STS2Plus.Reflection;

namespace STS2Plus.Patches;

[HarmonyPatchCategory("MoreRules")]
[HarmonyPatch]
internal static class GlassCannonPlayerDeserializePatch
{
	private static MethodBase? TargetMethod()
	{
		Type type = RuntimeTypeResolver.FindType("MegaCrit.Sts2.Core.Entities.Players.Player");
		return (type == null) ? null : AccessTools.Method(type, "FromSerializable", (Type[])null, (Type[])null);
	}

	private static void Postfix(object? __result)
	{
		if (PlusState.ShouldForceGlassCannonRepair() && __result != null && GameReflection.ApplyGlassCannon(__result))
		{
			ModEntry.Logger.Info("STS2Plus applied Glass Cannon after player deserialize: " + GameReflection.DescribeGlassCannonState(__result), 1);
		}
	}
}
