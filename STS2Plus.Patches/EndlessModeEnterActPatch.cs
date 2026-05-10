using System;
using System.Reflection;
using HarmonyLib;
using STS2Plus.Reflection;
using STS2Plus.Ui;

namespace STS2Plus.Patches;

[HarmonyPatchCategory("MoreRules")]
[HarmonyPatch]
internal static class EndlessModeEnterActPatch
{
	[HarmonyTargetMethod]
	private static MethodBase? TargetMethod()
	{
		Type type = RuntimeTypeResolver.FindType("MegaCrit.Sts2.Core.Runs.RunManager") ?? RuntimeTypeResolver.FindTypeByName("RunManager");
		return (type == null) ? null : AccessTools.Method(type, "EnterAct", (Type[])null, (Type[])null);
	}

	private static void Postfix()
	{
		ModEntry.Verbose("EndlessModeEnterAct: act entry refreshing overlay");
		EndlessModeOverlay.Refresh();
	}
}
