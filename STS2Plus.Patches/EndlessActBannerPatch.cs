using System;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes;
using STS2Plus.Localization;
using STS2Plus.Reflection;

namespace STS2Plus.Patches;

[HarmonyPatchCategory("MoreRules")]
[HarmonyPatch(typeof(NActBanner), "_Ready")]
internal static class EndlessActBannerPatch
{
	private static void Postfix(NActBanner __instance)
	{
		if (PlusState.IsEndlessModeActive())
		{
			Node nodeOrNull = ((Node)__instance).GetNodeOrNull((NodePath)"ActNumber");
			if (nodeOrNull != null)
			{
				AccessTools.Method(((object)nodeOrNull).GetType(), "SetTextAutoSize", new Type[1] { typeof(string) }, (Type[])null)?.Invoke(nodeOrNull, new object[1] { PlusLoc.ActNumber(GameReflection.GetTotalActNumber()) });
			}
		}
	}
}
