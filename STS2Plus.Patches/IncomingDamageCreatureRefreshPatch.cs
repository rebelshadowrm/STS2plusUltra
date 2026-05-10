using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Nodes.Combat;
using STS2Plus.Reflection;
using STS2Plus.Ui;

namespace STS2Plus.Patches;

[HarmonyPatchCategory("Core")]
[HarmonyPatch(typeof(NCreature), "RefreshIntents")]
internal static class IncomingDamageCreatureRefreshPatch
{
	private static void Postfix(NCreature __instance)
	{
		Creature entity = __instance.Entity;
		if (entity != null && GameReflection.IsLocalPlayerObject(entity))
		{
			IncomingDamageOverlay.Attach((Node)(object)__instance, entity);
			IncomingDamageOverlay.RequestRefresh();
		}
	}
}
