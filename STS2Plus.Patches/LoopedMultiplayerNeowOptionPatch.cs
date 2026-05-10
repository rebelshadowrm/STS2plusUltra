using System;
using System.Collections;
using System.Reflection;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Helpers;
using STS2Plus.Reflection;

namespace STS2Plus.Patches;

[HarmonyPatchCategory("MoreRules")]
[HarmonyPatch]
internal static class LoopedMultiplayerNeowOptionPatch
{
	private static readonly Type? EventSynchronizerType = RuntimeTypeResolver.FindType("MegaCrit.Sts2.Core.Multiplayer.Game.EventSynchronizer") ?? RuntimeTypeResolver.FindTypeByName("EventSynchronizer");

	private static readonly Type? EventRoomNodeType = RuntimeTypeResolver.FindType("MegaCrit.Sts2.Core.Nodes.Rooms.NEventRoom") ?? RuntimeTypeResolver.FindTypeByName("NEventRoom");

	private static readonly MethodInfo? GetEventForPlayerMethod = ((EventSynchronizerType == null) ? null : AccessTools.Method(EventSynchronizerType, "GetEventForPlayer", (Type[])null, (Type[])null));

	private static readonly MethodInfo? SaveHistoryMethod = ((EventSynchronizerType == null) ? null : AccessTools.Method(EventSynchronizerType, "SaveEventOptionToHistory", (Type[])null, (Type[])null));

	private static readonly PropertyInfo? EventModelCurrentOptionsProperty = AccessTools.Property(RuntimeTypeResolver.FindType("MegaCrit.Sts2.Core.Models.EventModel") ?? RuntimeTypeResolver.FindTypeByName("EventModel"), "CurrentOptions");

	private static readonly PropertyInfo? EventModelIsFinishedProperty = AccessTools.Property(RuntimeTypeResolver.FindType("MegaCrit.Sts2.Core.Models.EventModel") ?? RuntimeTypeResolver.FindTypeByName("EventModel"), "IsFinished");

	private static readonly PropertyInfo? EventModelIdProperty = AccessTools.Property(RuntimeTypeResolver.FindType("MegaCrit.Sts2.Core.Models.EventModel") ?? RuntimeTypeResolver.FindTypeByName("EventModel"), "Id");

	[HarmonyPatch]
	private static class ChooseOptionForEventPatch
	{
		[HarmonyTargetMethod]
		private static MethodBase? TargetMethod()
		{
			return (EventSynchronizerType == null) ? null : AccessTools.Method(EventSynchronizerType, "ChooseOptionForEvent", (Type[])null, (Type[])null);
		}

		private static void Prefix(object __instance, object player, int optionIndex)
		{
			if (!TryGetLoopedMultiplayerNeowContext(__instance, player, out object? eventForPlayer, out IList? currentOptions))
			{
				return;
			}
			string playerId = DescribePlayerId(player);
			string eventId = DescribeEventId(eventForPlayer);
			if ((EventModelIsFinishedProperty?.GetValue(eventForPlayer) as bool?).GetValueOrDefault())
			{
				ModEntry.Logger.Warn($"STS2Plus looped multiplayer Neow option rejected: player={playerId} event={eventId} loopIndex={GameReflection.GetLoopCount()} reason=event already finished.", 1);
				return;
			}
			if (currentOptions == null || optionIndex < 0 || optionIndex >= currentOptions.Count)
			{
				ModEntry.Logger.Warn($"STS2Plus looped multiplayer Neow option invalid: player={playerId} event={eventId} loopIndex={GameReflection.GetLoopCount()} optionIndex={optionIndex} optionCount={currentOptions?.Count ?? -1}.", 1);
				return;
			}
			EventOption option = (EventOption)currentOptions[optionIndex];
			LogOptionExecution("starting", player, optionIndex, option, eventForPlayer, null);
		}

		private static Exception? Finalizer(object __instance, object player, int optionIndex, Exception? __exception)
		{
			if (__exception == null)
			{
				return null;
			}
			if (TryGetLoopedMultiplayerNeowContext(__instance, player, out object? eventForPlayer, out IList? currentOptions) && currentOptions != null && optionIndex >= 0 && optionIndex < currentOptions.Count)
			{
				EventOption option = (EventOption)currentOptions[optionIndex];
				LogOptionExecution("threw", player, optionIndex, option, eventForPlayer, __exception);
			}
			else
			{
				ModEntry.Logger.Warn($"STS2Plus looped multiplayer Neow option exception before metadata capture player={DescribePlayerId(player)} optionIndex={optionIndex} loopIndex={GameReflection.GetLoopCount()}: {__exception}", 1);
			}
			return __exception;
		}
	}

	[HarmonyPatch]
	private static class ChooseLocalOptionPatch
	{
		[HarmonyTargetMethod]
		private static MethodBase? TargetMethod()
		{
			return (EventSynchronizerType == null) ? null : AccessTools.Method(EventSynchronizerType, "ChooseLocalOption", (Type[])null, (Type[])null);
		}

		private static void Prefix(object __instance, int index)
		{
			if (!GameReflection.IsMultiplayerRun() || GameReflection.GetLoopCount() <= 0)
			{
				return;
			}
			ModEntry.Logger.Info($"STS2Plus looped multiplayer Neow option local choose requested optionIndex={index} loopIndex={GameReflection.GetLoopCount()}.", 1);
		}
	}

	[HarmonyPatch]
	private static class OptionButtonClickedPatch
	{
		[HarmonyTargetMethod]
		private static MethodBase? TargetMethod()
		{
			return (EventRoomNodeType == null) ? null : AccessTools.Method(EventRoomNodeType, "OptionButtonClicked", (Type[])null, (Type[])null);
		}

		private static void Prefix(EventOption option, int index)
		{
			if (!GameReflection.IsMultiplayerRun() || GameReflection.GetLoopCount() <= 0)
			{
				return;
			}
			string title = DescribeOptionTitle(option);
			string textKey = DescribeTextKey(option);
			bool isReplacement = EndlessModeNeowOptionsPatch.TryGetReplacementOptionMetadata(option, out int replacementLoop, out string replacementSource);
			ModEntry.Logger.Info($"STS2Plus looped multiplayer Neow option button clicked optionIndex={index} title={title} textKey={textKey} loopIndex={GameReflection.GetLoopCount()} replacement={isReplacement} replacementLoop={replacementLoop} replacementSource={replacementSource}.", 1);
		}
	}

	private static bool TryGetLoopedMultiplayerNeowContext(object synchronizer, object player, out object? eventForPlayer, out IList? currentOptions)
	{
		eventForPlayer = null;
		currentOptions = null;
		if (!GameReflection.IsMultiplayerRun() || GameReflection.GetLoopCount() <= 0 || GetEventForPlayerMethod == null)
		{
			return false;
		}
		try
		{
			eventForPlayer = GetEventForPlayerMethod.Invoke(synchronizer, new object[1] { player });
			if (eventForPlayer == null || !IsNeowEvent(eventForPlayer))
			{
				return false;
			}
			currentOptions = EventModelCurrentOptionsProperty?.GetValue(eventForPlayer) as IList;
			return true;
		}
		catch (Exception ex)
		{
			ModEntry.Logger.Warn("STS2Plus looped multiplayer Neow option context lookup failed: " + ex, 1);
			return false;
		}
	}

	private static bool IsNeowEvent(object eventForPlayer)
	{
		if (string.Equals(eventForPlayer.GetType().Name, "Neow", StringComparison.Ordinal))
		{
			return true;
		}
		object? id = EventModelIdProperty?.GetValue(eventForPlayer);
		string? entry = AccessTools.Property(id?.GetType(), "Entry")?.GetValue(id)?.ToString();
		return string.Equals(entry, "NEOW", StringComparison.OrdinalIgnoreCase);
	}

	private static void LogOptionExecution(string phase, object player, int optionIndex, EventOption option, object? eventForPlayer, Exception? exception)
	{
		string playerId = DescribePlayerId(player);
		string title = DescribeOptionTitle(option);
		string textKey = DescribeTextKey(option);
		string eventId = DescribeEventId(eventForPlayer);
		bool isReplacement = EndlessModeNeowOptionsPatch.TryGetReplacementOptionMetadata(option, out int replacementLoop, out string replacementSource);
		bool callbackNull = false;
		try
		{
			callbackNull = AccessTools.Field(option.GetType(), "<Chosen>k__BackingField")?.GetValue(option) == null
				&& AccessTools.Property(option.GetType(), "Chosen") == null;
		}
		catch
		{
			callbackNull = false;
		}
		string exceptionText = (exception == null) ? "<none>" : $"{exception.GetType().Name}: {exception.Message}{Environment.NewLine}{exception}";
		ModEntry.Logger.Info($"STS2Plus looped multiplayer Neow option {phase} player={playerId} optionIndex={optionIndex} title={title} textKey={textKey} event={eventId} loopIndex={GameReflection.GetLoopCount()} callbackNull={callbackNull} replacement={isReplacement} replacementLoop={replacementLoop} replacementSource={replacementSource} exception={exceptionText}.", 1);
	}

	private static string DescribePlayerId(object? player)
	{
		return AccessTools.Property(player?.GetType(), "NetId")?.GetValue(player)?.ToString() ?? "<null>";
	}

	private static string DescribeEventId(object? eventForPlayer)
	{
		if (eventForPlayer == null)
		{
			return "<null>";
		}
		object? id = EventModelIdProperty?.GetValue(eventForPlayer);
		return AccessTools.Property(id?.GetType(), "Entry")?.GetValue(id)?.ToString() ?? id?.ToString() ?? eventForPlayer.GetType().Name;
	}

	private static string DescribeOptionTitle(EventOption option)
	{
		return AccessTools.Property(option.GetType(), "Title")?.GetValue(option)?.ToString() ?? AccessTools.Property(option.GetType(), "HistoryName")?.GetValue(option)?.ToString() ?? option.ToString();
	}

	private static string DescribeTextKey(EventOption option)
	{
		return AccessTools.Property(option.GetType(), "TextKey")?.GetValue(option)?.ToString() ?? "<null>";
	}
}
