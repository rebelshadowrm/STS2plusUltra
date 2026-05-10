using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using STS2Plus.Reflection;
using STS2Plus.Ui;

namespace STS2Plus.Patches;

[HarmonyPatchCategory("Core")]
[HarmonyPatch(typeof(NCombatRoom), "OnCombatSetUp")]
internal static class IncomingDamageCombatSetupPatch
{
	private static void Postfix(NCombatRoom __instance)
	{
		ModEntry.Verbose("IncomingDamage: combat setup, attaching overlays");
		AttachPlayerSideOverlays((Node)(object)__instance);
		IncomingDamageOverlay.RequestRefresh();
	}

	private static int AttachPlayerSideOverlays(Node node)
	{
		int num = 0;
		NCreature val = (NCreature)(object)((node is NCreature) ? node : null);
		if (val != null)
		{
			Creature entity = val.Entity;
			if (entity != null && GameReflection.IsLocalPlayerObject(entity))
			{
				IncomingDamageOverlay.Attach((Node)(object)val, entity);
				num++;
			}
		}
		foreach (Node child in node.GetChildren(false))
		{
			num += AttachPlayerSideOverlays(child);
		}
		return num;
	}
}
