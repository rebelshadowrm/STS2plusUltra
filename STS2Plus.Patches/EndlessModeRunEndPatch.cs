using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using HarmonyLib;
using STS2Plus.Reflection;
using STS2Plus.Ui;

namespace STS2Plus.Patches;

[HarmonyPatchCategory("MoreRules")]
[HarmonyPatch]
internal static class EndlessModeRunEndPatch
{
    [HarmonyTargetMethods]
    private static IEnumerable<MethodBase> TargetMethods()
    {
        Type? ngame = RuntimeTypeResolver.FindType("MegaCrit.Sts2.Core.Nodes.NGame") ?? RuntimeTypeResolver.FindTypeByName("NGame");
        if (ngame == null) yield break;

        var goToTimeline = AccessTools.Method(ngame, "GoToTimelineAfterRun", (Type[])null, (Type[])null);
        if (goToTimeline != null) yield return goToTimeline;

        var returnToMenu = AccessTools.Method(ngame, "ReturnToMainMenuAfterRun", (Type[])null, (Type[])null);
        if (returnToMenu != null) yield return returnToMenu;
    }

    private static bool Prefix(object __instance, ref Task? __result)
    {
        return true;
    }
}
