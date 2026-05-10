using System;
using System.Reflection;
using HarmonyLib;
using STS2Plus.Reflection;

namespace STS2Plus.Patches;

[HarmonyPatchCategory("MoreRules")]
[HarmonyPatch]
internal static class GlassCannonEventInitialStatePatch
{
	private static MethodBase? TargetMethod()
	{
		Type type = RuntimeTypeResolver.FindType("MegaCrit.Sts2.Core.Models.EventModel") ?? RuntimeTypeResolver.FindTypeByName("EventModel");
		return (type == null) ? null : AccessTools.Method(type, "SetInitialEventState", (Type[])null, (Type[])null);
	}

	private static void Postfix(object __instance)
	{
		GameReflection.ClearTrackedGlassCannonEvent(__instance);
	}
}
