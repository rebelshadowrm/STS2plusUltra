using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using STS2Plus.Reflection;

namespace STS2Plus.Patches;

[HarmonyPatchCategory("MoreRules")]
[HarmonyPatch(typeof(NCombatRoom), "OnCombatSetUp")]
internal static class GlassCannonCombatRoomSetupPatch
{
	private static void Postfix()
	{
		if (!PlusState.IsGlassCannonActive())
		{
			return;
		}
		int num = 0;
		foreach (object player in GameReflection.GetPlayers())
		{
			if (GameReflection.ApplyGlassCannon(player) | GameReflection.RepairGlassCannonPlayerCreature(player) | GameReflection.RepairGlassCannonState(player))
			{
				num++;
			}
		}
		if (num > 0)
		{
			ModEntry.Logger.Warn($"STS2Plus repaired Glass Cannon during combat room setup for {num} player(s).", 1);
		}
	}
}
