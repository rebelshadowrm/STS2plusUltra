using System;
using System.Reflection;
using HarmonyLib;
using STS2Plus.Reflection;
using STS2Plus.Ui;

namespace STS2Plus.Patches;

[HarmonyPatchCategory("Core")]
[HarmonyPatch]
internal static class SpeedControlEnterActPatch
{
	[HarmonyTargetMethod]
	private static MethodBase? TargetMethod()
	{
		Type type = RuntimeTypeResolver.FindType("MegaCrit.Sts2.Core.Runs.RunManager") ?? RuntimeTypeResolver.FindTypeByName("RunManager");
		return (type == null) ? null : AccessTools.Method(type, "EnterAct", (Type[])null, (Type[])null);
	}

	private static void Postfix()
	{
		SpeedControlOverlay.SetMainMenuVisible(visible: false);
		SpeedControlOverlay.Show();
	}
}
