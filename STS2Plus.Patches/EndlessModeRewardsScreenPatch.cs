using System;
using System.Reflection;
using System.Threading.Tasks;
using HarmonyLib;
using STS2Plus.Reflection;
using STS2Plus.Ui;

namespace STS2Plus.Patches;

[HarmonyPatchCategory("MoreRules")]
[HarmonyPatch]
internal static class EndlessModeRewardsScreenPatch
{
    [HarmonyTargetMethod]
    private static MethodBase? TargetMethod()
    {
        Type? type = RuntimeTypeResolver.FindType("MegaCrit.Sts2.Core.Runs.RunManager") ?? RuntimeTypeResolver.FindTypeByName("RunManager");
        return type != null ? AccessTools.Method(type, "ProceedFromTerminalRewardsScreen", (Type[])null, (Type[])null) : null;
    }

    private static bool Prefix(object __instance, ref Task? __result)
    {
        return true;
    }
}
