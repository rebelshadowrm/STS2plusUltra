using System;
using System.Reflection;
using System.Threading.Tasks;
using HarmonyLib;
using STS2Plus.Reflection;
using STS2Plus.Ui;

namespace STS2Plus.Patches;

[HarmonyPatchCategory("MoreRules")]
[HarmonyPatch]
internal static class EndlessModeArchitectWinRunPatch
{
	[HarmonyTargetMethod]
	private static MethodBase? TargetMethod()
	{
		Type type = RuntimeTypeResolver.FindType("MegaCrit.Sts2.Core.Models.Events.TheArchitect") ?? RuntimeTypeResolver.FindTypeByName("TheArchitect");
		return (type == null) ? null : AccessTools.Method(type, "WinRun", (Type[])null, (Type[])null);
	}

	private static bool Prefix(ref Task? __result)
	{
		return true;
	}
}
