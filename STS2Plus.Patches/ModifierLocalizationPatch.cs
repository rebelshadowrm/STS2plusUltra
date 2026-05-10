using HarmonyLib;
using MegaCrit.Sts2.Core.Localization;
using STS2Plus.Localization;

namespace STS2Plus.Patches;

[HarmonyPatchCategory("Core")]
[HarmonyPatch(typeof(LocManager), "SetLanguage")]
internal static class ModifierLocalizationPatch
{
	private static void Postfix()
	{
		PlusLoc.MergeIntoModifiersTable();
	}
}
