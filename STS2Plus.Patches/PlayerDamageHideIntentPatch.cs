using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Combat;
using STS2Plus.Features;

namespace STS2Plus.Patches;

[HarmonyPatchCategory("Core")]
[HarmonyPatch(typeof(NCreature), "AnimHideIntent")]
internal static class PlayerDamageHideIntentPatch
{
	private static void Postfix()
	{
		PlayerDamageTracker.Recalculate();
	}
}
