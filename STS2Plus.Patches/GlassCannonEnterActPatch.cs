using System;
using System.Reflection;
using HarmonyLib;
using STS2Plus.Features;
using STS2Plus.Reflection;

namespace STS2Plus.Patches;

[HarmonyPatchCategory("MoreRules")]
internal static class GlassCannonEnterActPatch
{
	private static MethodBase? TargetMethod()
	{
		Type type = RuntimeTypeResolver.FindType("MegaCrit.Sts2.Core.Runs.RunManager");
		return (type == null) ? null : AccessTools.Method(type, "EnterAct", (Type[])null, (Type[])null);
	}

	private static void Postfix()
	{
		if (!PlusState.IsGlassCannonActive())
		{
			return;
		}
		foreach (object player in GameReflection.GetPlayers())
		{
			if (AppliedTracker.MarkGlassCannonPlayer(player) && GameReflection.ApplyGlassCannon(player))
			{
				ModEntry.Logger.Info("STS2Plus applied Glass Cannon to " + GameReflection.DescribeCreature(player) + ".", 1);
			}
		}
	}
}
