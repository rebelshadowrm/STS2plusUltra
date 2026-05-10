using System;
using System.Reflection;
using HarmonyLib;
using STS2Plus.Reflection;

namespace STS2Plus.Patches;

[HarmonyPatchCategory("MoreRules")]
[HarmonyPatch]
internal static class GlassCannonEventDeathPatch
{
	private static MethodBase? TargetMethod()
	{
		Type type = RuntimeTypeResolver.FindType("MegaCrit.Sts2.Core.Models.EventModel") ?? RuntimeTypeResolver.FindTypeByName("EventModel");
		return (type == null) ? null : AccessTools.Method(type, "SetEventFinished", (Type[])null, (Type[])null);
	}

	private static bool Prefix(object __instance, object? description)
	{
		return !GameReflection.TryRecoverGlassCannonEventFromDeath(__instance, description);
	}
}
