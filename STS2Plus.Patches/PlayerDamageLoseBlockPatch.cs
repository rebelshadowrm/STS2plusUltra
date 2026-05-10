using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Creatures;
using STS2Plus.Features;

namespace STS2Plus.Patches;

[HarmonyPatchCategory("Core")]
[HarmonyPatch(typeof(Creature), "LoseBlockInternal")]
internal static class PlayerDamageLoseBlockPatch
{
	private static void Postfix()
	{
		PlayerDamageTracker.Recalculate();
	}
}
