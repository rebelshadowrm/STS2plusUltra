using System;
using System.Reflection;
using System.Threading.Tasks;
using HarmonyLib;
using STS2Plus.Reflection;

namespace STS2Plus.Patches;

[HarmonyPatchCategory("MoreRules")]
[HarmonyPatch]
internal static class GlassCannonAncientEventSetupPatch
{
	private static MethodBase? TargetMethod()
	{
		Type type = RuntimeTypeResolver.FindType("MegaCrit.Sts2.Core.Models.AncientEventModel") ?? RuntimeTypeResolver.FindTypeByName("AncientEventModel");
		return (type == null) ? null : AccessTools.Method(type, "BeforeEventStarted", (Type[])null, (Type[])null);
	}

	private static void Postfix(object __instance, ref Task __result)
	{
		if (PlusState.IsGlassCannonActive())
		{
			__result = Wrap(__instance, __result);
		}
	}

	private static async Task Wrap(object eventModel, Task originalTask)
	{
		await originalTask;
		if (PlusState.IsGlassCannonActive() && string.Equals(eventModel.GetType().Name, "Neow", StringComparison.Ordinal))
		{
			object owner = AccessTools.Property(eventModel.GetType(), "Owner")?.GetValue(eventModel);
			if (owner != null && (GameReflection.ApplyGlassCannon(owner) | GameReflection.RepairGlassCannonPlayerCreature(owner) | GameReflection.RepairGlassCannonState(owner)))
			{
				ModEntry.Logger.Warn("STS2Plus repaired Glass Cannon after AncientEvent setup. " + GameReflection.DescribeGlassCannonState(owner), 1);
			}
		}
	}
}
