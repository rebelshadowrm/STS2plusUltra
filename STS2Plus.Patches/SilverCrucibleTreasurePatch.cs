using System;
using System.Collections;
using System.Reflection;
using HarmonyLib;
using STS2Plus.Reflection;

namespace STS2Plus.Patches;

[HarmonyPatchCategory("Core")]
[HarmonyPatch]
internal static class SilverCrucibleTreasurePatch
{
	private static MethodBase? TargetMethod()
	{
		Type type = RuntimeTypeResolver.FindType("MegaCrit.Sts2.Core.Models.Relics.SilverCrucible") ?? RuntimeTypeResolver.FindTypeByName("SilverCrucible");
		return (type == null) ? null : AccessTools.Method(type, "ShouldGenerateTreasure", (Type[])null, (Type[])null);
	}

	private static void Postfix(object? player, ref bool __result)
	{
		if (!__result && player != null)
		{
			object obj = AccessTools.Property(player.GetType(), "RunState")?.GetValue(player);
			ICollection collection = ((obj != null) ? (AccessTools.Property(obj.GetType(), "Players")?.GetValue(obj) as ICollection) : null);
			if (collection != null && collection.Count > 1)
			{
				__result = true;
			}
		}
	}
}
