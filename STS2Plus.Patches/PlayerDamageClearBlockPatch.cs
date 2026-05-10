using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Creatures;
using STS2Plus.Features;

namespace STS2Plus.Patches;

[HarmonyPatchCategory("Core")]
[HarmonyPatch(typeof(Creature), "ClearBlock")]
internal static class PlayerDamageClearBlockPatch
{
	private static void Postfix()
	{
		PlayerDamageTracker.Recalculate();
	}
}
