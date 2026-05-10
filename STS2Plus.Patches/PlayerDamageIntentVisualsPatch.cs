using HarmonyLib;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.Nodes.Combat;
using STS2Plus.Features;

namespace STS2Plus.Patches;

[HarmonyPatchCategory("Core")]
[HarmonyPatch(typeof(NIntent), "UpdateVisuals")]
internal static class PlayerDamageIntentVisualsPatch
{
	private static void Postfix(AbstractIntent ____intent)
	{
		if (____intent is AttackIntent)
		{
			PlayerDamageTracker.Recalculate();
		}
	}
}
