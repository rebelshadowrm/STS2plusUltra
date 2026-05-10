using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Combat;
using STS2Plus.Features;
using STS2Plus.Ui;

namespace STS2Plus.Patches;

[HarmonyPatchCategory("Core")]
[HarmonyPatch(typeof(NCreature), "_ExitTree")]
internal static class IncomingDamageCreatureExitPatch
{
	private static void Prefix(NCreature __instance)
	{
		IncomingDamageTracker.ClearOwner(__instance.Entity);
		IncomingDamageOverlay.DetachIfBound((Node)(object)__instance);
	}
}
