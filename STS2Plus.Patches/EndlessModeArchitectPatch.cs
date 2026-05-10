using System;
using System.Reflection;
using HarmonyLib;
using STS2Plus.Reflection;

namespace STS2Plus.Patches;

[HarmonyPatchCategory("MoreRules")]
[HarmonyPatch]
internal static class EndlessModeArchitectPatch
{
	[HarmonyTargetMethod]
	private static MethodBase? TargetMethod()
	{
		Type type = RuntimeTypeResolver.FindType("MegaCrit.Sts2.Core.Models.Events.TheArchitect") ?? RuntimeTypeResolver.FindTypeByName("TheArchitect");
		return (type == null) ? null : AccessTools.Method(type, "TriggerVictory", (Type[])null, (Type[])null);
	}

	private static bool Prefix()
	{
		return true;
	}
}
