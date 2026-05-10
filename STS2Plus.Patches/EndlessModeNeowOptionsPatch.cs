using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Random;
using STS2Plus.Reflection;

namespace STS2Plus.Patches;

[HarmonyPatchCategory("MoreRules")]
[HarmonyPatch]
internal static class EndlessModeNeowOptionsPatch
{
	private sealed class SuppressedModifierState
	{
		public required IList ModifiersList { get; init; }

		public required List<object?> Snapshot { get; init; }

		public int LoopCount { get; init; }
	}

	private sealed class LoopedNeowRngState
	{
		public required object OriginalRng { get; init; }

		public required string SourceBaseSeed { get; init; }

		public required string PlayerKey { get; init; }

		public required uint ResolvedSeed { get; init; }

		public int LoopCount { get; init; }
	}

	private sealed class ReplacementOptionState
	{
		public int LoopCount { get; init; }

		public string Source { get; init; } = string.Empty;
	}

	private static readonly ConditionalWeakTable<object, SuppressedModifierState> SuppressedModifierStates = new ConditionalWeakTable<object, SuppressedModifierState>();

	private static readonly ConditionalWeakTable<object, LoopedNeowRngState> LoopedNeowRngStates = new ConditionalWeakTable<object, LoopedNeowRngState>();

	private static readonly ConditionalWeakTable<EventOption, ReplacementOptionState> ReplacementOptions = new ConditionalWeakTable<EventOption, ReplacementOptionState>();

	[HarmonyTargetMethod]
	private static MethodBase? TargetMethod()
	{
		Type type = RuntimeTypeResolver.FindType("MegaCrit.Sts2.Core.Models.Events.Neow") ?? RuntimeTypeResolver.FindTypeByName("Neow");
		return (type == null) ? null : AccessTools.Method(type, "GenerateInitialOptions", (Type[])null, (Type[])null);
	}

	private static void Prefix(object __instance)
	{
		TryApplyLoopedDeterministicNeowRng(__instance);
		if (ShouldTemporarilySuppressModifierOptions(__instance, out int loopCount, out IList? modifiersList))
		{
			List<object?> snapshot = new List<object?>();
			foreach (object? item in modifiersList)
			{
				snapshot.Add(item);
			}
			modifiersList.Clear();
			SuppressedModifierStates.Remove(__instance);
			SuppressedModifierStates.Add(__instance, new SuppressedModifierState
			{
				ModifiersList = modifiersList,
				Snapshot = snapshot,
				LoopCount = loopCount
			});
			ModEntry.Logger.Info($"EndlessModeNeowOptions: temporarily suppressed modifier-generated Neow options for multiplayer loopIndex={loopCount} modifierCount={snapshot.Count}.", 1);
		}
	}

	private static void Postfix(object __instance, ref IReadOnlyList<EventOption> __result)
	{
		try
		{
			if (TryRestoreSuppressedModifiers(__instance, out SuppressedModifierState? suppressedState))
			{
				ModEntry.Logger.Info($"EndlessModeNeowOptions: restored modifier list after multiplayer loopIndex={suppressedState.LoopCount} generatedCount={__result?.Count ?? 0}.", 1);
				LogGeneratedOptionKeys(__instance, __result, "live-or-reload");
				if ((__result?.Count ?? 0) > 0)
				{
					return;
				}
				if (TryRestoreLoopContinuationNeowOptions(__instance, out IReadOnlyList<EventOption> fallbackAfterSuppression, "multiplayer-fallback"))
				{
					__result = fallbackAfterSuppression;
					ModEntry.Logger.Warn($"EndlessModeNeowOptions: multiplayer normal generation returned no options, using fallback loopIndex={suppressedState.LoopCount} replacementCount={fallbackAfterSuppression.Count}.", 1);
				}
				return;
			}
			if (PlusState.IsEndlessModeActive())
			{
				if (ShouldSkipLoopedNeowHandling(out int loopCount, out bool trueNewRun))
				{
					if (TryRestoreLoopContinuationNeowOptions(__instance, out IReadOnlyList<EventOption> replacement, "singleplayer-fallback"))
					{
						__result = replacement;
						ModEntry.Logger.Info($"EndlessModeNeowOptions skipped modifier options because loopIndex > 0 loopIndex={loopCount} trueNewRun={trueNewRun} replacementCount={replacement.Count}.", 1);
						LogGeneratedOptionKeys(__instance, __result, "singleplayer-fallback");
					}
					else
					{
						ModEntry.Logger.Warn($"EndlessModeNeowOptions could not replace modifier options on loop continuation loopIndex={loopCount} trueNewRun={trueNewRun}.", 1);
					}
					return;
				}
				ModEntry.Verbose($"EndlessModeNeowOptions: replacing Neow options count={__result?.Count ?? 0}");
				EnsureStartedWithNeowFlag();
				if ((__result?.Count ?? 0) <= 0 && TryRestoreNeowOptions(__instance, out IReadOnlyList<EventOption> fallback))
				{
					__result = fallback;
				}
				LogGeneratedOptionKeys(__instance, __result, "default");
			}
		}
		finally
		{
			RestoreLoopedDeterministicNeowRng(__instance);
		}
	}

	internal static bool RepairNeowRoomIfNeeded(object? source)
	{
		if (!PlusState.IsEndlessModeActive())
		{
			return false;
		}
		if (ShouldSkipLoopedNeowHandling(out int loopCount, out bool trueNewRun))
		{
			object? obj2 = ResolveNeowModel(source);
			if (obj2 == null)
			{
				return false;
			}
			if (!TryRestoreLoopContinuationNeowOptions(obj2, out IReadOnlyList<EventOption> loopFallback, "room-repair"))
			{
				ModEntry.Logger.Warn($"EndlessModeNeowOptions failed to repair loop continuation Neow room loopIndex={loopCount} trueNewRun={trueNewRun}.", 1);
				return false;
			}
			bool wroteLoopFallback = TryWriteOptions(obj2, loopFallback);
			if (source != obj2)
			{
				wroteLoopFallback |= TryWriteOptions(source, loopFallback);
			}
			ModEntry.Logger.Info($"EndlessModeNeowOptions repaired loop continuation room loopIndex={loopCount} trueNewRun={trueNewRun} replacementCount={loopFallback.Count} wrote={wroteLoopFallback}.", 1);
			return wroteLoopFallback;
		}
		object obj = ResolveNeowModel(source);
		if (obj == null)
		{
			return false;
		}
		EnsureStartedWithNeowFlag();
		if (CountOptions(ReadOptions(obj)) > 0 || CountOptions(ReadOptions(source)) > 0)
		{
			return false;
		}
		if (!TryRestoreNeowOptions(obj, out IReadOnlyList<EventOption> fallback))
		{
			return false;
		}
		bool flag = TryWriteOptions(obj, fallback);
		if (source != obj)
		{
			flag |= TryWriteOptions(source, fallback);
		}
		if (flag)
		{
			ModEntry.Logger.Warn($"STS2Plus repaired host-side Neow room with {fallback.Count} option(s).", 1);
		}
		return flag;
	}

	private static bool TryRestoreNeowOptions(object neow, out IReadOnlyList<EventOption> fallback)
	{
		fallback = BuildFallbackOptions(neow, null);
		if (fallback.Count == 0)
		{
			ModEntry.Logger.Warn("STS2Plus endless Neow fallback could not find any replacement options.", 1);
			return false;
		}
		ModEntry.Logger.Warn($"STS2Plus restored {fallback.Count} Neow option(s) for endless mode.", 1);
		return true;
	}

	private static IReadOnlyList<EventOption> BuildFallbackOptions(object neow, string? replacementSource)
	{
		object rng = AccessTools.Property(neow.GetType(), "Rng")?.GetValue(neow);
		object owner = AccessTools.Property(neow.GetType(), "Owner")?.GetValue(neow);
		int loopCount = GameReflection.GetLoopCount();
		int playerCount = GetPlayerCount(owner);
		List<EventOption> list = ReadEventOptions(AccessTools.Property(neow.GetType(), "CurseOptions")?.GetValue(neow));
		if (ShouldAllowBundleOption(owner))
		{
			AddOption(list, AccessTools.Property(neow.GetType(), "BundleOption")?.GetValue(neow));
		}
		EventOption val = PickOption(list, rng);
		if (val == null)
		{
			return BuildShuffledFallbackOptions(neow, owner, rng, playerCount);
		}
		List<EventOption> list2 = ReadEventOptions(AccessTools.Property(neow.GetType(), "PositiveOptions")?.GetValue(neow));
		RemoveOptionByRelic(list2, val, "CursedPearl", "GoldenPearl");
		RemoveOptionByRelic(list2, val, "PrecariousShears", "PreciseScissors");
		RemoveOptionByRelic(list2, val, "LeafyPoultice", "NewLeaf");
		if (playerCount > 1)
		{
			AddOption(list2, AccessTools.Property(neow.GetType(), "ClericOption")?.GetValue(neow));
		}
		AddOption(list2, AccessTools.Property(neow.GetType(), NextBool(rng) ? "ToughnessOption" : "SafetyOption")?.GetValue(neow));
		if (!HasRelic(val, "LargeCapsule"))
		{
			AddOption(list2, AccessTools.Property(neow.GetType(), NextBool(rng) ? "PatienceOption" : "ScavengerOption")?.GetValue(neow));
		}
		Shuffle(list2, rng);
		List<EventOption> list3 = list2.Take(2).Append(val).Distinct()
			.ToList();
		IReadOnlyList<EventOption> result;
		if (list3.Count <= 0)
		{
			result = BuildShuffledFallbackOptions(neow, owner, rng, playerCount);
		}
		else
		{
			IReadOnlyList<EventOption> readOnlyList = list3;
			result = readOnlyList;
		}
		TrackReplacementOptions(result, loopCount, replacementSource);
		return result;
	}

	private static void AddOptions(List<EventOption> destination, object? source)
	{
		if (!(source is IEnumerable enumerable))
		{
			return;
		}
		foreach (object item in enumerable)
		{
			EventOption val = (EventOption)((item is EventOption) ? item : null);
			if (val != null)
			{
				destination.Add(val);
			}
		}
	}

	private static void AddOption(List<EventOption> destination, object? source)
	{
		EventOption val = (EventOption)((source is EventOption) ? source : null);
		if (val != null && !IsSilverCrucibleOption(val) && !destination.Contains(val))
		{
			destination.Add(val);
		}
	}

	private static List<EventOption> ReadEventOptions(object? source)
	{
		List<EventOption> list = new List<EventOption>();
		AddOptions(list, source);
		return list;
	}

	private static IReadOnlyList<EventOption> BuildShuffledFallbackOptions(object neow, object? owner, object? rng, int playerCount)
	{
		List<EventOption> list = new List<EventOption>();
		AddOptions(list, AccessTools.Property(neow.GetType(), "PositiveOptions")?.GetValue(neow));
		AddOptions(list, AccessTools.Property(neow.GetType(), "CurseOptions")?.GetValue(neow));
		if (ShouldAllowBundleOption(owner))
		{
			AddOption(list, AccessTools.Property(neow.GetType(), "BundleOption")?.GetValue(neow));
		}
		if (playerCount > 1)
		{
			AddOption(list, AccessTools.Property(neow.GetType(), "ClericOption")?.GetValue(neow));
		}
		AddOption(list, AccessTools.Property(neow.GetType(), "PatienceOption")?.GetValue(neow));
		AddOption(list, AccessTools.Property(neow.GetType(), "SafetyOption")?.GetValue(neow));
		AddOption(list, AccessTools.Property(neow.GetType(), "ScavengerOption")?.GetValue(neow));
		AddOption(list, AccessTools.Property(neow.GetType(), "ToughnessOption")?.GetValue(neow));
		if (list.Count == 0)
		{
			AddOptions(list, AccessTools.Property(neow.GetType(), "AllPossibleOptions")?.GetValue(neow));
		}
		Shuffle(list, rng);
		IReadOnlyList<EventOption> readOnlyList = list.Take(3).ToList();
		TrackReplacementOptions(readOnlyList, GameReflection.GetLoopCount(), "shuffled-fallback");
		return readOnlyList;
	}

	private static bool ShouldAllowBundleOption(object? owner)
	{
		if (owner == null)
		{
			return false;
		}
		Type type = RuntimeTypeResolver.FindType("MegaCrit.Sts2.Core.Models.Relics.ScrollBoxes") ?? RuntimeTypeResolver.FindTypeByName("ScrollBoxes");
		MethodInfo methodInfo = ((type == null) ? null : AccessTools.Method(type, "CanGenerateBundles", (Type[])null, (Type[])null));
		if (methodInfo == null)
		{
			return false;
		}
		try
		{
			return (methodInfo.Invoke(null, new object[1] { owner }) as bool?).GetValueOrDefault();
		}
		catch
		{
			return false;
		}
	}

	private static int GetPlayerCount(object? owner)
	{
		object obj2;
		if (owner != null)
		{
			object obj = AccessTools.Property(owner.GetType(), "RunState")?.GetValue(owner);
			obj2 = ((obj != null) ? (AccessTools.Property(obj.GetType(), "Players")?.GetValue(obj) as ICollection) : null);
		}
		else
		{
			obj2 = null;
		}
		return ((ICollection)obj2)?.Count ?? 0;
	}

	private static EventOption? PickOption(List<EventOption> options, object? rng)
	{
		if (options.Count == 0)
		{
			return null;
		}
		int num = NextInt(rng, options.Count);
		if (num < 0 || num >= options.Count)
		{
			num = 0;
		}
		return options[num];
	}

	private static void Shuffle(List<EventOption> options, object? rng)
	{
		if (options.Count <= 1)
		{
			return;
		}
		for (int num = options.Count - 1; num > 0; num--)
		{
			int num2 = NextInt(rng, num + 1);
			if (num2 < 0 || num2 > num)
			{
				num2 = 0;
			}
			int index = num;
			int index2 = num2;
			EventOption value = options[num2];
			EventOption value2 = options[num];
			options[index] = value;
			options[index2] = value2;
		}
	}

	private static int NextInt(object? rng, int maxExclusive)
	{
		if (rng == null || maxExclusive <= 1)
		{
			return 0;
		}
		MethodInfo methodInfo = AccessTools.Method(rng.GetType(), "NextInt", new Type[1] { typeof(int) }, (Type[])null);
		try
		{
			return (methodInfo?.Invoke(rng, new object[1] { maxExclusive }) as int?).GetValueOrDefault();
		}
		catch
		{
			return 0;
		}
	}

	private static bool NextBool(object? rng)
	{
		if (rng == null)
		{
			return false;
		}
		MethodInfo methodInfo = AccessTools.Method(rng.GetType(), "NextBool", Type.EmptyTypes, (Type[])null);
		try
		{
			return (methodInfo?.Invoke(rng, null) as bool?).GetValueOrDefault();
		}
		catch
		{
			return false;
		}
	}

	private static void RemoveOptionByRelic(List<EventOption> options, EventOption selected, string sourceRelicName, string blockedRelicName)
	{
		string blockedRelicName2 = blockedRelicName;
		if (HasRelic(selected, sourceRelicName))
		{
			options.RemoveAll((EventOption option) => HasRelic(option, blockedRelicName2));
		}
	}

	private static bool IsSilverCrucibleOption(EventOption option)
	{
		return HasRelic(option, "SilverCrucible");
	}

	private static bool HasRelic(EventOption option, string relicTypeName)
	{
		return string.Equals(((object)option.Relic)?.GetType().Name, relicTypeName, StringComparison.Ordinal);
	}

	private static object? ResolveNeowModel(object? source)
	{
		if (source == null)
		{
			return null;
		}
		if (string.Equals(source.GetType().Name, "Neow", StringComparison.Ordinal))
		{
			return source;
		}
		string[] array = new string[3] { "CanonicalEvent", "Event", "Model" };
		foreach (string text in array)
		{
			object obj = AccessTools.Property(source.GetType(), text)?.GetValue(source) ?? AccessTools.Field(source.GetType(), text)?.GetValue(source);
			if (obj != null && string.Equals(obj.GetType().Name, "Neow", StringComparison.Ordinal))
			{
				return obj;
			}
		}
		return null;
	}

	private static object? ReadOptions(object? source)
	{
		if (source == null)
		{
			return null;
		}
		return AccessTools.Property(source.GetType(), "EventOptions")?.GetValue(source) ?? AccessTools.Property(source.GetType(), "CurrentOptions")?.GetValue(source) ?? AccessTools.Property(source.GetType(), "Options")?.GetValue(source) ?? AccessTools.Field(source.GetType(), "_currentOptions")?.GetValue(source) ?? AccessTools.Field(source.GetType(), "_eventOptions")?.GetValue(source) ?? AccessTools.Field(source.GetType(), "_options")?.GetValue(source);
	}

	private static int CountOptions(object? source)
	{
		if (source is IEnumerable enumerable)
		{
			int num = 0;
			foreach (object item in enumerable)
			{
				if (item is EventOption)
				{
					num++;
				}
			}
			return num;
		}
		return 0;
	}

	private static bool TryWriteOptions(object? target, IReadOnlyList<EventOption> options)
	{
		if (target == null)
		{
			return false;
		}
		string[] array = new string[3] { "EventOptions", "CurrentOptions", "Options" };
		foreach (string text in array)
		{
			PropertyInfo propertyInfo = AccessTools.Property(target.GetType(), text);
			if (!(propertyInfo == null) && propertyInfo.CanWrite)
			{
				try
				{
					propertyInfo.SetValue(target, options);
					return true;
				}
				catch
				{
				}
			}
		}
		string[] array2 = new string[3] { "_currentOptions", "_eventOptions", "_options" };
		foreach (string text2 in array2)
		{
			FieldInfo fieldInfo = AccessTools.Field(target.GetType(), text2);
			if (!(fieldInfo == null))
			{
				try
				{
					fieldInfo.SetValue(target, options);
					return true;
				}
				catch
				{
				}
			}
		}
		return false;
	}

	private static bool TryRestoreLoopContinuationNeowOptions(object neow, out IReadOnlyList<EventOption> replacement, string replacementSource)
	{
		replacement = BuildFallbackOptions(neow, replacementSource);
		return replacement.Count > 0;
	}

	internal static bool TryGetReplacementOptionMetadata(EventOption option, out int loopCount, out string source)
	{
		if (ReplacementOptions.TryGetValue(option, out ReplacementOptionState? value))
		{
			loopCount = value.LoopCount;
			source = value.Source;
			return true;
		}
		loopCount = -1;
		source = string.Empty;
		return false;
	}

	private static bool ShouldTemporarilySuppressModifierOptions(object neow, out int loopCount, out IList? modifiersList)
	{
		loopCount = GameReflection.GetLoopCount();
		modifiersList = null;
		if (!PlusState.IsEndlessModeActive() || loopCount <= 0 || !GameReflection.IsMultiplayerRun())
		{
			return false;
		}
		object? owner = AccessTools.Property(neow.GetType(), "Owner")?.GetValue(neow);
		object? runState = (owner == null) ? null : AccessTools.Property(owner.GetType(), "RunState")?.GetValue(owner);
		object? modifiers = (runState == null) ? null : AccessTools.Property(runState.GetType(), "Modifiers")?.GetValue(runState);
		modifiersList = modifiers as IList;
		return modifiersList != null && modifiersList.Count > 0;
	}

	private static bool TryRestoreSuppressedModifiers(object neow, out SuppressedModifierState? state)
	{
		if (!SuppressedModifierStates.TryGetValue(neow, out state) || state == null)
		{
			return false;
		}
		SuppressedModifierStates.Remove(neow);
		state.ModifiersList.Clear();
		foreach (object? item in state.Snapshot)
		{
			state.ModifiersList.Add(item);
		}
		return true;
	}

	private static void TrackReplacementOptions(IEnumerable<EventOption> options, int loopCount, string? replacementSource)
	{
		if (string.IsNullOrWhiteSpace(replacementSource))
		{
			return;
		}
		foreach (EventOption option in options)
		{
			ReplacementOptions.Remove(option);
			ReplacementOptions.Add(option, new ReplacementOptionState
			{
				LoopCount = loopCount,
				Source = replacementSource
			});
		}
	}

	private static void EnsureStartedWithNeowFlag()
	{
		Type type = RuntimeTypeResolver.FindType("MegaCrit.Sts2.Core.Runs.RunManager") ?? RuntimeTypeResolver.FindTypeByName("RunManager");
		if (type == null)
		{
			return;
		}
		object obj = AccessTools.Property(type, "Instance")?.GetValue(null);
		if (obj == null)
		{
			return;
		}
		bool flag = false;
		MethodInfo methodInfo = AccessTools.Method(type, "SetStartedWithNeowFlag", (Type[])null, (Type[])null);
		if (methodInfo != null)
		{
			try
			{
				ParameterInfo[] parameters = methodInfo.GetParameters();
				if (parameters.Length == 0)
				{
					methodInfo.Invoke(obj, Array.Empty<object>());
					flag = true;
				}
				else if (parameters.Length == 1 && parameters[0].ParameterType == typeof(bool))
				{
					methodInfo.Invoke(obj, new object[1] { true });
					flag = true;
				}
			}
			catch
			{
			}
		}
		flag |= TrySetBool(obj, "StartedWithNeow", value: true);
		flag |= TrySetBool(obj, "_startedWithNeow", value: true);
		object obj3 = AccessTools.Property(type, "State")?.GetValue(obj) ?? AccessTools.Field(type, "_state")?.GetValue(obj) ?? AccessTools.Field(type, "_runState")?.GetValue(obj);
		if (obj3 != null)
		{
			flag |= TrySetBool(obj3, "StartedWithNeow", value: true);
			flag |= TrySetBool(obj3, "_startedWithNeow", value: true);
		}
		if (flag)
		{
			ModEntry.Logger.Info("STS2Plus forced StartedWithNeow on current run.", 1);
		}
	}

	private static bool TrySetBool(object target, string memberName, bool value)
	{
		PropertyInfo propertyInfo = AccessTools.Property(target.GetType(), memberName);
		if ((object)propertyInfo != null && propertyInfo.CanWrite)
		{
			try
			{
				propertyInfo.SetValue(target, value);
				return true;
			}
			catch
			{
			}
		}
		FieldInfo fieldInfo = AccessTools.Field(target.GetType(), memberName);
		if (fieldInfo?.FieldType == typeof(bool))
		{
			try
			{
				fieldInfo.SetValue(target, value);
				return true;
			}
			catch
			{
			}
		}
		return false;
	}

	internal static bool ShouldSkipLoopedNeowHandling(out int loopCount, out bool trueNewRun)
	{
		loopCount = GameReflection.GetLoopCount();
		trueNewRun = loopCount <= 0;
		return loopCount > 0;
	}

	private static void LogGeneratedOptionKeys(object neow, IReadOnlyList<EventOption>? options, string generationSource)
	{
		try
		{
			object? owner = AccessTools.Property(neow.GetType(), "Owner")?.GetValue(neow);
			object? player = owner == null ? null : AccessTools.Property(owner.GetType(), "NetId")?.GetValue(owner);
			object? runState = owner == null ? null : AccessTools.Property(owner.GetType(), "RunState")?.GetValue(owner);
			string seed = (runState == null) ? "<null>" : (AccessTools.Property(runState.GetType(), "Rng")?.GetValue(runState)?.GetType().GetProperty("StringSeed")?.GetValue(AccessTools.Property(runState.GetType(), "Rng")?.GetValue(runState))?.ToString() ?? "<null>");
			List<string> textKeys = new List<string>();
			if (options != null)
			{
				foreach (EventOption option in options)
				{
					textKeys.Add(AccessTools.Property(option.GetType(), "TextKey")?.GetValue(option)?.ToString() ?? "<null>");
				}
			}
			if (LoopedNeowRngStates.TryGetValue(neow, out LoopedNeowRngState? value) && value != null)
			{
				ModEntry.Logger.Info($"LoopedNeowRng sourceBaseSeed={value.SourceBaseSeed} loopIndex={value.LoopCount} player={value.PlayerKey} resolvedSeed={value.ResolvedSeed} optionTextKeys=[{string.Join(", ", textKeys)}].", 1);
			}
			ModEntry.Logger.Info($"EndlessModeNeowOptions: generated options source={generationSource} player={player ?? "<null>"} loopIndex={GameReflection.GetLoopCount()} mapSeed={seed} event=NEOW optionTextKeys=[{string.Join(", ", textKeys)}].", 1);
		}
		catch (Exception ex)
		{
			ModEntry.Logger.Warn("EndlessModeNeowOptions: failed to log option keys - " + ex.Message, 1);
		}
	}

	private static void TryApplyLoopedDeterministicNeowRng(object neow)
	{
		if (!PlusState.IsEndlessModeActive())
		{
			return;
		}
		int loopCount = GameReflection.GetLoopCount();
		if (loopCount <= 0 || !GameReflection.IsMultiplayerRun())
		{
			return;
		}
		if (LoopedNeowRngStates.TryGetValue(neow, out _))
		{
			return;
		}
		object? owner = AccessTools.Property(neow.GetType(), "Owner")?.GetValue(neow);
		object? runState = owner == null ? null : AccessTools.Property(owner.GetType(), "RunState")?.GetValue(owner);
		if (owner == null || runState == null)
		{
			return;
		}
		string sourceBaseSeed = GetBaseSeedString(runState);
		string playerKey = GetLoopedNeowPlayerKey(owner, runState);
		uint resolvedSeed = StableHashToUInt32(sourceBaseSeed + "|loop=" + loopCount + "|player=" + playerKey + "|event=NEOW|salt=STS2Plus.LoopedNeowOptions");
		object? rng = ReadRng(neow);
		if (rng == null)
		{
			ModEntry.Logger.Warn($"LoopedNeowRng could not read live Neow RNG loopIndex={loopCount} player={playerKey}.", 1);
			return;
		}
		object? replacementRng = CreateRng(resolvedSeed, loopCount, playerKey, sourceBaseSeed);
		if (replacementRng == null)
		{
			ModEntry.Logger.Warn($"LoopedNeowRng could not create deterministic RNG loopIndex={loopCount} player={playerKey} resolvedSeed={resolvedSeed}.", 1);
			return;
		}
		if (!TryWriteRng(neow, replacementRng, out string targetMember, out string writeError))
		{
			ModEntry.Logger.Warn($"LoopedNeowRng could not assign deterministic RNG loopIndex={loopCount} player={playerKey} resolvedSeed={resolvedSeed} target={targetMember} error={writeError}.", 1);
			return;
		}
		LoopedNeowRngStates.Remove(neow);
		LoopedNeowRngStates.Add(neow, new LoopedNeowRngState
		{
			OriginalRng = rng,
			SourceBaseSeed = sourceBaseSeed,
			PlayerKey = playerKey,
			ResolvedSeed = resolvedSeed,
			LoopCount = loopCount
		});
		ModEntry.Logger.Info($"LoopedNeowRng prepared sourceBaseSeed={sourceBaseSeed} loopIndex={loopCount} player={playerKey} resolvedSeed={resolvedSeed} appliedType={replacementRng.GetType().FullName} target={targetMember}.", 1);
	}

	private static void RestoreLoopedDeterministicNeowRng(object neow)
	{
		if (!LoopedNeowRngStates.TryGetValue(neow, out LoopedNeowRngState? value) || value == null)
		{
			return;
		}
		LoopedNeowRngStates.Remove(neow);
		if (!TryWriteRng(neow, value.OriginalRng, out string targetMember, out string writeError))
		{
			ModEntry.Logger.Warn($"LoopedNeowRng failed to restore original RNG loopIndex={value.LoopCount} player={value.PlayerKey} resolvedSeed={value.ResolvedSeed} target={targetMember} error={writeError}.", 1);
		}
	}

	private static object? ReadRng(object target)
	{
		for (Type? type = target.GetType(); type != null; type = type.BaseType)
		{
			PropertyInfo? propertyInfo = type.GetProperty("Rng", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
			if (propertyInfo != null)
			{
				try
				{
					return propertyInfo.GetValue(target);
				}
				catch
				{
				}
			}
			FieldInfo? fieldInfo = type.GetField("<Rng>k__BackingField", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly) ?? type.GetField("_rng", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly) ?? type.GetField("Rng", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
			if (fieldInfo != null)
			{
				try
				{
					return fieldInfo.GetValue(target);
				}
				catch
				{
				}
			}
		}
		return null;
	}

	private static bool TryWriteRng(object target, object rng, out string targetMember, out string writeError)
	{
		targetMember = "<none>";
		writeError = string.Empty;
		for (Type? type = target.GetType(); type != null; type = type.BaseType)
		{
			PropertyInfo? propertyInfo = type.GetProperty("Rng", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
			if (propertyInfo != null && propertyInfo.CanWrite)
			{
				try
				{
					propertyInfo.SetValue(target, rng);
					targetMember = type.FullName + ".Rng";
					return true;
				}
				catch (Exception ex)
				{
					targetMember = type.FullName + ".Rng";
					writeError = ex.GetType().Name + ": " + ex.Message;
				}
			}
			FieldInfo? fieldInfo = type.GetField("<Rng>k__BackingField", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly) ?? type.GetField("_rng", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly) ?? type.GetField("Rng", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
			if (fieldInfo != null)
			{
				try
				{
					fieldInfo.SetValue(target, rng);
					targetMember = type.FullName + "." + fieldInfo.Name;
					return true;
				}
				catch (Exception ex2)
				{
					targetMember = type.FullName + "." + fieldInfo.Name;
					writeError = ex2.GetType().Name + ": " + ex2.Message;
				}
			}
		}
		if (string.IsNullOrEmpty(writeError))
		{
			writeError = "no writable Rng property or field found";
		}
		return false;
	}

	private static object? CreateRng(uint resolvedSeed, int loopCount, string playerKey, string sourceBaseSeed)
	{
		try
		{
			Rng rng = new Rng(resolvedSeed);
			return rng;
		}
		catch (Exception ex)
		{
			ModEntry.Logger.Warn($"LoopedNeowRng create failed sourceBaseSeed={sourceBaseSeed} loopIndex={loopCount} player={playerKey} resolvedSeed={resolvedSeed}: {ex}", 1);
			return null;
		}
	}

	private static string GetBaseSeedString(object runState)
	{
		object? value = AccessTools.Property(runState.GetType(), "Rng")?.GetValue(runState);
		string text = value?.GetType().GetProperty("StringSeed")?.GetValue(value)?.ToString() ?? "STS2PLUS";
		text = SeedHelper.CanonicalizeSeed(text);
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

	private static string GetLoopedNeowPlayerKey(object owner, object runState)
	{
		object? value = AccessTools.Property(owner.GetType(), "NetId")?.GetValue(owner);
		string text = value?.ToString() ?? "<null>";
		int num = GetPlayerSlotIndex(owner, runState);
		return $"slot={num};netId={text}";
	}

	private static int GetPlayerSlotIndex(object owner, object runState)
	{
		if (!(AccessTools.Property(runState.GetType(), "Players")?.GetValue(runState) is IEnumerable enumerable))
		{
			return -1;
		}
		int num = 0;
		object? value = AccessTools.Property(owner.GetType(), "NetId")?.GetValue(owner);
		foreach (object item in enumerable)
		{
			if (ReferenceEquals(item, owner))
			{
				return num;
			}
			object? value2 = AccessTools.Property(item.GetType(), "NetId")?.GetValue(item);
			if (value != null && value.Equals(value2))
			{
				return num;
			}
			num++;
		}
		return -1;
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
