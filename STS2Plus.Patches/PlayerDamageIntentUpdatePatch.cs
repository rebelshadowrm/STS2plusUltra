using HarmonyLib;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.Nodes.Combat;
using STS2Plus.Features;

namespace STS2Plus.Patches;

[HarmonyPatchCategory("Core")]
[HarmonyPatch(typeof(NIntent), "UpdateIntent")]
internal static class PlayerDamageIntentUpdatePatch
{
	private static void Postfix(AbstractIntent intent)
	{
		if (intent is AttackIntent)
		{
			PlayerDamageTracker.Recalculate();
		}
	}
}
