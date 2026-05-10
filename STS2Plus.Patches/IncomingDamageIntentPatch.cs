using System.Collections.Generic;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.Nodes.Combat;
using STS2Plus.Features;

namespace STS2Plus.Patches;

[HarmonyPatchCategory("Core")]
[HarmonyPatch(typeof(NIntent), "UpdateIntent")]
internal static class IncomingDamageIntentPatch
{
	private static void Postfix(NIntent __instance, AbstractIntent intent, IEnumerable<Creature> targets, Creature owner)
	{
		IncomingDamageTracker.UpdateIntent(intent, targets, owner);
	}
}
