using System;
using System.Reflection;
using HarmonyLib;
using STS2Plus.Reflection;

namespace STS2Plus.Patches;

[HarmonyPatchCategory("MoreRules")]
[HarmonyPatch]
internal static class GlassCannonEventBeginPatch
{
	private static MethodBase? TargetMethod()
	{
		Type type = RuntimeTypeResolver.FindType("MegaCrit.Sts2.Core.Models.EventModel") ?? RuntimeTypeResolver.FindTypeByName("EventModel");
		return (type == null) ? null : AccessTools.Method(type, "BeginEvent", (Type[])null, (Type[])null);
	}

	private static void Prefix(object __instance, object? player, bool isPreFinished)
	{
		if (PlusState.IsGlassCannonActive() && player != null)
		{
			GameReflection.TrackGlassCannonEventStart(__instance, player, isPreFinished);
			ModEntry.Logger.Info("STS2Plus Glass Cannon BeginEvent pre: " + GameReflection.DescribeGlassCannonState(player), 1);
			if (GameReflection.ApplyGlassCannon(player) | GameReflection.RepairGlassCannonPlayerCreature(player) | GameReflection.RepairGlassCannonState(player))
			{
				ModEntry.Logger.Info("STS2Plus repaired Glass Cannon player state before event begin. " + GameReflection.DescribeGlassCannonState(player), 1);
			}
		}
	}
}
