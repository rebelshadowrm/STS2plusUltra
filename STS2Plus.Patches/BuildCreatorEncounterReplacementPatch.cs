using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using STS2Plus.Features;

namespace STS2Plus.Patches;

[HarmonyPatchCategory("MoreRules")]
[HarmonyPatch(typeof(CombatRoom), MethodType.Constructor, typeof(EncounterModel), typeof(IRunState))]
internal static class BuildCreatorEncounterReplacementPatch
{
	private static void Prefix(ref EncounterModel encounter, IRunState? runState)
	{
		ModEntry.Verbose($"BuildCreatorEncounter: checking encounter replacement type={encounter?.GetType().Name}");
		encounter = BuildCreatorRuntime.ReplaceEncounterIfNeeded(encounter, runState);
	}
}
