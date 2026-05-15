using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.Runs;
using STS2Plus.Reflection;

namespace STS2Plus.Patches;

[HarmonyPatchCategory("MoreRules")]
internal static class RandomEndlessAncientPatch
{
	[HarmonyPatch(typeof(RunManager), "GenerateRooms")]
	private static class RunManagerGenerateRoomsPatch
	{
		private static void Postfix(RunManager __instance)
		{
			try
			{
				ApplyLoopStartAncientIfNeeded(ReadRunState(__instance), "RunManager.GenerateRooms");
			}
			catch (Exception ex)
			{
				ModEntry.Logger.Warn("RandomEndlessAncient fallbackToNeow reason=GenerateRooms patch exception " + ex, 1);
			}
		}
	}

	[HarmonyPatch(typeof(ActModel), "PullAncient")]
	private static class ActModelPullAncientPatch
	{
		private static void Postfix(ActModel __instance, ref EventModel __result)
		{
			try
			{
				RunState? state = ReadRunState(RunManager.Instance);
				if (!ShouldUseRandomLoopedAncient(state, __instance))
				{
					return;
				}
				if (TrySelectLoopStartAncient(state!, out AncientEventModel? selectedAncient, out string poolLog, out uint resolvedSeed, out string reason) && selectedAncient != null)
				{
					bool applied = TrySetActAncient(__instance, selectedAncient);
					__result = selectedAncient;
					ModEntry.Logger.Info($"RandomEndlessAncient pool=[{poolLog}]", 1);
					ModEntry.Logger.Info($"RandomEndlessAncient selected={selectedAncient.Id.Entry} loopIndex={GameReflection.GetLoopCount()} resolvedSeed={resolvedSeed}", 1);
					ModEntry.Logger.Info($"RandomEndlessAncient appliedToActRooms={applied}", 1);
					return;
				}
				ModEntry.Logger.Warn("RandomEndlessAncient fallbackToNeow reason=" + reason, 1);
			}
			catch (Exception ex)
			{
				ModEntry.Logger.Warn("RandomEndlessAncient fallbackToNeow reason=PullAncient patch exception " + ex, 1);
			}
		}
	}

	private static RunState? ReadRunState(RunManager? runManager)
	{
		if (runManager == null)
		{
			return null;
		}
		return AccessTools.Property(runManager.GetType(), "State")?.GetValue(runManager) as RunState;
	}

	private static void ApplyLoopStartAncientIfNeeded(RunState? state, string source)
	{
		if (!ShouldUseRandomLoopedAncient(state, state?.Acts?.FirstOrDefault()))
		{
			return;
		}
		if (!TrySelectLoopStartAncient(state!, out AncientEventModel? selectedAncient, out string poolLog, out uint resolvedSeed, out string reason) || selectedAncient == null)
		{
			ModEntry.Logger.Warn("RandomEndlessAncient fallbackToNeow reason=" + reason, 1);
			return;
		}
		ActModel? actModel = state.Acts[0];
		bool applied = TrySetActAncient(actModel, selectedAncient);
		ModEntry.Logger.Info($"RandomEndlessAncient pool=[{poolLog}]", 1);
		ModEntry.Logger.Info($"RandomEndlessAncient selected={selectedAncient.Id.Entry} loopIndex={GameReflection.GetLoopCount()} resolvedSeed={resolvedSeed}", 1);
		ModEntry.Logger.Info($"RandomEndlessAncient appliedToActRooms={applied} source={source}", 1);
	}

	private static bool ShouldUseRandomLoopedAncient(RunState? state, ActModel? act)
	{
		if (state == null || act == null || !PlusState.IsEndlessModeActive())
		{
			return false;
		}
		if (GameReflection.GetLoopCount() <= 0)
		{
			return false;
		}
		if (state.CurrentActIndex != 0)
		{
			return false;
		}
		if (state.Acts == null || state.Acts.Count <= 0)
		{
			return false;
		}
		return ReferenceEquals(state.Acts[0], act) || state.Acts[0].Id == act.Id;
	}

	private static bool TrySelectLoopStartAncient(RunState state, out AncientEventModel? selectedAncient, out string poolLog, out uint resolvedSeed, out string reason)
	{
		selectedAncient = null;
		poolLog = string.Empty;
		resolvedSeed = 0u;
		reason = "unknown";
		List<AncientEventModel> pool = BuildAncientPool(state);
		poolLog = string.Join(", ", pool.Select(static a => a.Id.Entry));
		if (pool.Count == 0)
		{
			reason = "empty pool";
			return false;
		}
		string normalizedBaseSeed = NormalizeBaseSeed(state.Rng.StringSeed);
		int loopIndex = GameReflection.GetLoopCount();
		resolvedSeed = StableHashToUInt32(normalizedBaseSeed + "|loop=" + loopIndex + "|slot=act0-start|salt=STS2Plus.RandomEndlessAncient");
		Rng rng = new Rng(resolvedSeed);
		selectedAncient = rng.NextItem(pool);
		if (selectedAncient == null)
		{
			reason = "rng returned null";
			return false;
		}
		if (string.Equals(selectedAncient.GetType().Name, "DeprecatedAncientEvent", StringComparison.Ordinal))
		{
			reason = "selected ancient was deprecated";
			selectedAncient = null;
			return false;
		}
		reason = string.Empty;
		return true;
	}

	private static List<AncientEventModel> BuildAncientPool(RunState state)
	{
		List<AncientEventModel> list = new List<AncientEventModel>();
		for (int i = 1; i < state.Acts.Count; i++)
		{
			foreach (AncientEventModel unlockedAncient in state.Acts[i].GetUnlockedAncients(state.UnlockState))
			{
				AddAncientIfAllowed(list, unlockedAncient);
			}
		}
		if (AccessTools.Property(state.UnlockState.GetType(), "SharedAncients")?.GetValue(state.UnlockState) is IEnumerable enumerable)
		{
			foreach (object item in enumerable)
			{
				if (item is AncientEventModel ancient)
				{
					AddAncientIfAllowed(list, ancient);
				}
			}
		}
		return list.OrderBy(static a => a.Id.Entry, StringComparer.Ordinal).ToList();
	}

	private static void AddAncientIfAllowed(ICollection<AncientEventModel> pool, AncientEventModel? ancient)
	{
		if (ancient == null)
		{
			return;
		}
		if (string.Equals(ancient.GetType().Name, "Neow", StringComparison.Ordinal))
		{
			return;
		}
		if (string.Equals(ancient.GetType().Name, "DeprecatedAncientEvent", StringComparison.Ordinal))
		{
			return;
		}
		if (pool.Any((AncientEventModel existing) => existing.Id == ancient.Id))
		{
			return;
		}
		pool.Add(ancient);
	}

	private static bool TrySetActAncient(ActModel act, AncientEventModel selectedAncient)
	{
		try
		{
			object? rooms = AccessTools.Field(act.GetType(), "_rooms")?.GetValue(act);
			if (rooms == null)
			{
				return false;
			}
			PropertyInfo? propertyInfo = AccessTools.Property(rooms.GetType(), "Ancient");
			if (propertyInfo != null && propertyInfo.CanWrite)
			{
				propertyInfo.SetValue(rooms, selectedAncient);
				return true;
			}
			FieldInfo? fieldInfo = AccessTools.Field(rooms.GetType(), "<Ancient>k__BackingField") ?? AccessTools.Field(rooms.GetType(), "_ancient") ?? AccessTools.Field(rooms.GetType(), "Ancient");
			if (fieldInfo != null)
			{
				fieldInfo.SetValue(rooms, selectedAncient);
				return true;
			}
		}
		catch (Exception ex)
		{
			ModEntry.Logger.Warn("RandomEndlessAncient fallbackToNeow reason=failed to set act ancient " + ex, 1);
		}
		return false;
	}

	private static string NormalizeBaseSeed(string? seed)
	{
		string text = SeedHelper.CanonicalizeSeed(seed ?? "STS2PLUS");
		int num = text.LastIndexOf("_L", StringComparison.Ordinal);
		if (num > 0 && num + 2 < text.Length)
		{
			bool flag = true;
			for (int i = num + 2; i < text.Length; i++)
			{
				if (!char.IsDigit(text[i]))
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				return text.Substring(0, num);
			}
		}
		return text;
	}

	private static uint StableHashToUInt32(string value)
	{
		uint num = 2166136261u;
		for (int i = 0; i < value.Length; i++)
		{
			num ^= value[i];
			num *= 16777619;
		}
		if (num == 0)
		{
			return 2166136261u;
		}
		return num;
	}
}
