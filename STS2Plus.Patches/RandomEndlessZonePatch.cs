using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Map;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;
using STS2Plus.Reflection;

namespace STS2Plus.Patches;

[HarmonyPatchCategory("MoreRules")]
[HarmonyPatch(typeof(RunManager), "GenerateRooms")]
internal static class RandomEndlessZonePatch
{
	private sealed class ZoneSelectionState
	{
		public required string BaseSeed { get; init; }

		public required int DisplayActNumber { get; init; }

		public required string SelectedActId { get; init; }

		public uint ResolvedSeed { get; init; }

		public bool Applied { get; init; }

		public string? FallbackReason { get; init; }
	}

	private static readonly string[] ZonePool = new string[4] { "OVERGROWTH", "UNDERDOCKS", "HIVE", "GLORY" };

	private static readonly ConditionalWeakTable<RunManager, ZoneSelectionState> SelectionStates = new ConditionalWeakTable<RunManager, ZoneSelectionState>();

	private static void Prefix(RunManager __instance)
	{
		SelectionStates.Remove(__instance);
		try
		{
			RunState? state = ReadRunState(__instance);
			if (!ShouldApplyRandomEndlessZone(state))
			{
				return;
			}
			string baseSeed = NormalizeBaseSeed(state!.Rng.StringSeed);
			int loopIndex = GameReflection.GetLoopCount();
			uint resolvedSeed = StableHashToUInt32(baseSeed + "|loop=" + loopIndex + "|salt=STS2Plus.RandomEndlessZone");
			string selectedActId = ChooseZone(resolvedSeed);
			int displayActNumber = GameReflection.GetTotalActNumber();
			ModEntry.Logger.Info($"EndlessActContext baseSeed={baseSeed} loopIndex={loopIndex} displayAct={displayActNumber} selectedAct={selectedActId} resolvedSeed={resolvedSeed}", 1);
			ModEntry.Logger.Info($"RandomEndlessZone pool=[{string.Join(", ", ZonePool)}]", 1);
			bool applied = TryReplaceCurrentAct(state, selectedActId, out string fallbackReason);
			SelectionStates.Add(__instance, new ZoneSelectionState
			{
				BaseSeed = baseSeed,
				DisplayActNumber = displayActNumber,
				SelectedActId = selectedActId,
				ResolvedSeed = resolvedSeed,
				Applied = applied,
				FallbackReason = fallbackReason
			});
			ModEntry.Logger.Info($"RandomEndlessZone selected={selectedActId} appliedToCurrentAct={applied}", 1);
			if (!applied && !string.IsNullOrWhiteSpace(fallbackReason))
			{
				ModEntry.Logger.Warn("RandomEndlessZone fallback reason=" + fallbackReason, 1);
			}
		}
		catch (Exception ex)
		{
			ModEntry.Logger.Warn("RandomEndlessZone fallback reason=prefix exception " + ex, 1);
		}
	}

	private static void Postfix(RunManager __instance)
	{
		try
		{
			RunState? state = ReadRunState(__instance);
			if (state == null || GameReflection.GetLoopCount() <= 0 || state.CurrentActIndex != 0)
			{
				return;
			}
			string actId = state.Act.Id.Entry;
			string startingAncient = SafeGetAncientId(state.Act);
			string bossEncounter = SafeGetBossEncounterId(state.Act);
			int roomCount = state.Act.GetNumberOfRooms(state.Players.Count > 1);
			int floorCount = state.Act.GetNumberOfFloors(state.Players.Count > 1);
			string startingMapPoint = state.Map == null ? "<pending map generation>" : DescribeStartingMapPoint(state.Map);
			string source = SelectionStates.TryGetValue(__instance, out ZoneSelectionState? selection) && selection != null && selection.Applied ? "act-native" : "fallback";
			ModEntry.Logger.Info($"RandomEndlessAncient act={actId} selected={startingAncient} source={source}", 1);
			ModEntry.Logger.Info($"RandomEndlessZone after generation currentActIndex={state.CurrentActIndex} actId={actId} startingMapPoint={startingMapPoint} startingAncient={startingAncient} bossEncounter={bossEncounter} roomCount={roomCount} floorCount={floorCount}", 1);
		}
		catch (Exception ex)
		{
			ModEntry.Logger.Warn("RandomEndlessZone fallback reason=postfix exception " + ex, 1);
		}
		finally
		{
			SelectionStates.Remove(__instance);
		}
	}

	private static bool ShouldApplyRandomEndlessZone(RunState? state)
	{
		return state != null && PlusState.IsEndlessModeActive() && GameReflection.GetLoopCount() > 0 && state.CurrentActIndex == 0;
	}

	private static string ChooseZone(uint resolvedSeed)
	{
		return ZonePool[new MegaCrit.Sts2.Core.Random.Rng(resolvedSeed).NextInt(ZonePool.Length)];
	}

	private static bool TryReplaceCurrentAct(RunState state, string selectedActId, out string fallbackReason)
	{
		fallbackReason = string.Empty;
		try
		{
			ActModel? actModel = ModelDb.Acts.FirstOrDefault((ActModel c) => c.Id.Entry == selectedActId);
			if (actModel == null)
			{
				fallbackReason = "act not found: " + selectedActId;
				return false;
			}
			ActModel mutableAct = actModel.ToMutable();
			MethodInfo? setActDebugMethod = AccessTools.Method(state.GetType(), "SetActDebug", new Type[1] { typeof(ActModel) }, null);
			if (setActDebugMethod == null)
			{
				fallbackReason = "RunState.SetActDebug not found";
				return false;
			}
			setActDebugMethod.Invoke(state, new object[1] { mutableAct });
			return true;
		}
		catch (Exception ex)
		{
			fallbackReason = ex.GetType().Name + ": " + ex.Message;
			return false;
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
		return num == 0 ? 2166136261u : num;
	}

	private static string SafeGetAncientId(ActModel act)
	{
		try
		{
			return act.Ancient?.Id.Entry ?? "<null>";
		}
		catch
		{
			return "<error>";
		}
	}

	private static string SafeGetBossEncounterId(ActModel act)
	{
		try
		{
			return act.BossEncounter?.Id.Entry ?? "<null>";
		}
		catch
		{
			return "<error>";
		}
	}

	private static string DescribeStartingMapPoint(ActMap map)
	{
		try
		{
			MapPoint point = map.StartingMapPoint;
			return $"{point.PointType}@({point.coord.col}, {point.coord.row})";
		}
		catch
		{
			return "<error>";
		}
	}
}
