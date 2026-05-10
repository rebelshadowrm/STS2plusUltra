using System;
using System.Reflection;
using System.Threading.Tasks;
using HarmonyLib;
using STS2Plus.Reflection;

namespace STS2Plus.Patches;

[HarmonyPatchCategory("MoreRules")]
[HarmonyPatch]
internal static class EndlessModeKillPlayersPatch
{
	[HarmonyTargetMethod]
	private static MethodBase? TargetMethod()
	{
		Type type = RuntimeTypeResolver.FindType("MegaCrit.Sts2.Core.Runs.RunManager") ?? RuntimeTypeResolver.FindTypeByName("RunManager");
		return (type == null) ? null : AccessTools.Method(type, "GuaranteeKillAllPlayers", (Type[])null, (Type[])null);
	}

	private static bool Prefix(object __instance, ref Task? __result)
	{
		return true;
	}
}
