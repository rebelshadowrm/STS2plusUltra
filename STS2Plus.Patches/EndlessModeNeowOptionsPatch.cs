using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Events;
using STS2Plus.Reflection;

namespace STS2Plus.Patches;

[HarmonyPatchCategory("MoreRules")]
[HarmonyPatch]
internal static class EndlessModeNeowOptionsPatch
{
	[HarmonyTargetMethod]
	private static MethodBase? TargetMethod()
	{
		Type type = RuntimeTypeResolver.FindType("MegaCrit.Sts2.Core.Models.Events.Neow") ?? RuntimeTypeResolver.FindTypeByName("Neow");
		return (type == null) ? null : AccessTools.Method(type, "GenerateInitialOptions", (Type[])null, (Type[])null);
	}

	private static void Postfix(object __instance, ref IReadOnlyList<EventOption> __result)
	{
		if (PlusState.IsEndlessModeActive())
		{
			if (ShouldSkipLoopedNeowHandling(out int loopCount, out bool trueNewRun))
			{
				if (TryRestoreLoopContinuationNeowOptions(__instance, out IReadOnlyList<EventOption> replacement))
				{
					__result = replacement;
					ModEntry.Logger.Info($"EndlessModeNeowOptions skipped modifier options because loopIndex > 0 loopIndex={loopCount} trueNewRun={trueNewRun} replacementCount={replacement.Count}.", 1);
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
			if (!TryRestoreLoopContinuationNeowOptions(obj2, out IReadOnlyList<EventOption> loopFallback))
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
		fallback = BuildFallbackOptions(neow);
		if (fallback.Count == 0)
		{
			ModEntry.Logger.Warn("STS2Plus endless Neow fallback could not find any replacement options.", 1);
			return false;
		}
		ModEntry.Logger.Warn($"STS2Plus restored {fallback.Count} Neow option(s) for endless mode.", 1);
		return true;
	}

	private static IReadOnlyList<EventOption> BuildFallbackOptions(object neow)
	{
		object rng = AccessTools.Property(neow.GetType(), "Rng")?.GetValue(neow);
		object owner = AccessTools.Property(neow.GetType(), "Owner")?.GetValue(neow);
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
		return list.Take(3).ToList();
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

	private static bool TryRestoreLoopContinuationNeowOptions(object neow, out IReadOnlyList<EventOption> replacement)
	{
		replacement = BuildFallbackOptions(neow);
		return replacement.Count > 0;
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
}
