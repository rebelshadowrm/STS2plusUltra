using System;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace STS2Plus.Patches;

[HarmonyPatchCategory("MoreRules")]
[HarmonyPatch(typeof(CardModel), "FromSerializable")]
internal static class UnlimitedGrowthDeserializePatch
{
	private static void Prefix(SerializableCard save)
	{
		int upgradeLevel = UnlimitedGrowthSafety.PrepareSerializableUpgradeLevel(save);
		UnlimitedGrowthSerializationContext.Push(upgradeLevel);
	}

	private static Exception? Finalizer(Exception? __exception)
	{
		UnlimitedGrowthSerializationContext.Pop();
		return __exception;
	}
}
