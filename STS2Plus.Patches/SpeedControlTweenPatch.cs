using Godot;
using HarmonyLib;
using STS2Plus.Ui;

namespace STS2Plus.Patches;

[HarmonyPatchCategory("Core")]
[HarmonyPatch(typeof(Tween), "SetParallel")]
internal static class SpeedControlTweenPatch
{
	private static void Postfix(Tween __result)
	{
		SpeedControlOverlay.ApplyTweenSpeed(__result);
	}
}
