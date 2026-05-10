using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Saves.Runs;
using STS2Plus.Config;
using STS2Plus.Modifiers;

namespace STS2Plus.Patches;

[HarmonyPatchCategory("MoreRules")]
[HarmonyPatch(typeof(ModifierModel), "FromSerializable")]
internal static class CustomModifierSerializationPatch
{
	private static bool Prefix(SerializableModifier serializable, ref ModifierModel __result)
	{
		if (!ConfigManager.Current.MoreRulesEnabled)
		{
			return true;
		}
		ModifierModel val = CustomModifierCatalog.TryCreate(serializable);
		if (val == null)
		{
			return true;
		}
		__result = val;
		SavedProperties props = serializable.Props;
		if (props != null)
		{
			props.Fill((AbstractModel)(object)__result);
		}
		return false;
	}
}
