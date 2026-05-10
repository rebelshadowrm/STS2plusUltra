using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Map;
using MegaCrit.Sts2.Core.Multiplayer;
using MegaCrit.Sts2.Core.Multiplayer.Game;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.Relics;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.Saves.MapDrawing;
using MegaCrit.Sts2.Core.Saves.Runs;
using STS2Plus.Reflection;
using STS2Plus.Ui;

namespace STS2Plus.Patches;

internal static class EndlessLoopTransition
{
	private static readonly FieldInfo? RunStateRngField = AccessTools.Field(typeof(RunState), "<Rng>k__BackingField");

	private static readonly PropertyInfo? RunManagerStateProperty = AccessTools.Property(typeof(RunManager), "State");

	private static readonly PropertyInfo? RunManagerShouldSaveProperty = AccessTools.Property(typeof(RunManager), "ShouldSave");

	private static readonly PropertyInfo? RunManagerIsCleaningUpProperty = AccessTools.Property(typeof(RunManager), "IsCleaningUp");

	private static readonly PropertyInfo? RunManagerIsAbandonedProperty = AccessTools.Property(typeof(RunManager), "IsAbandoned");

	private static readonly MethodInfo? RunManagerInitializeSavedRunMethod = AccessTools.Method(typeof(RunManager), "InitializeSavedRun", new Type[1] { typeof(SerializableRun) }, null);

	private static readonly MethodInfo? RunManagerClearScreensMethod = AccessTools.Method(typeof(RunManager), "ClearScreens", Type.EmptyTypes, null);

	private static readonly PropertyInfo? RunStateNextRoomIdProperty = AccessTools.Property(typeof(RunState), "NextRoomId");

	private static readonly FieldInfo? RunStateSharedRelicGrabBagField = AccessTools.Field(typeof(RunState), "<SharedRelicGrabBag>k__BackingField");

	private static readonly FieldInfo? RunStateVisitedEventIdsField = AccessTools.Field(typeof(RunState), "_visitedEventIds");

	private static readonly PropertyInfo? RunStateExtraFieldsProperty = AccessTools.Property(typeof(RunState), "ExtraFields");

	private static readonly FieldInfo? MapSelectionRunStateField = AccessTools.Field(typeof(MapSelectionSynchronizer), "_runState");

	private static readonly FieldInfo? MapSelectionVotesField = AccessTools.Field(typeof(MapSelectionSynchronizer), "_votes");

	private static readonly FieldInfo? MapSelectionAcceptingVotesField = AccessTools.Field(typeof(MapSelectionSynchronizer), "_acceptingVotesFromSource");

	private static readonly PropertyInfo? MapSelectionGenerationCountProperty = AccessTools.Property(typeof(MapSelectionSynchronizer), "MapGenerationCount");

	private static readonly FieldInfo? ActionQueueRequestedActionsField = AccessTools.Field(typeof(ActionQueueSynchronizer), "_requestedActionsWaitingForPlayerTurn");

	private static readonly FieldInfo? CombatStateSyncRunStateField = AccessTools.Field(typeof(CombatStateSynchronizer), "_runState");

	private static readonly FieldInfo? CombatStateSyncDataField = AccessTools.Field(typeof(CombatStateSynchronizer), "_syncData");

	private static readonly FieldInfo? CombatStateSyncRngField = AccessTools.Field(typeof(CombatStateSynchronizer), "_rngSet");

	private static readonly FieldInfo? CombatStateSyncRelicGrabBagField = AccessTools.Field(typeof(CombatStateSynchronizer), "_sharedRelicGrabBag");

	private static readonly FieldInfo? RunLocationBufferMessagesField = AccessTools.Field(typeof(RunLocationTargetedMessageBuffer), "_messagesWaitingOnLocationChange");

	private static readonly FieldInfo? RunLocationBufferVisitedLocationsField = AccessTools.Field(typeof(RunLocationTargetedMessageBuffer), "_visitedLocations");

	private static readonly PropertyInfo? CombatManagerPlayerActionsDisabledProperty = AccessTools.Property(typeof(CombatManager), "PlayerActionsDisabled");

	private static readonly PropertyInfo? CombatManagerIsInProgressProperty = AccessTools.Property(typeof(CombatManager), "IsInProgress");

	private static readonly PropertyInfo? CombatManagerIsPausedProperty = AccessTools.Property(typeof(CombatManager), "IsPaused");

	private static readonly PropertyInfo? CombatManagerIsEnemyTurnStartedProperty = AccessTools.Property(typeof(CombatManager), "IsEnemyTurnStarted");

	private static readonly PropertyInfo? CombatManagerEndingPhaseOneProperty = AccessTools.Property(typeof(CombatManager), "EndingPlayerTurnPhaseOne");

	private static readonly PropertyInfo? CombatManagerEndingPhaseTwoProperty = AccessTools.Property(typeof(CombatManager), "EndingPlayerTurnPhaseTwo");

	private static readonly FieldInfo? CombatManagerStateField = AccessTools.Field(typeof(CombatManager), "_state");

	private static readonly FieldInfo? CombatManagerPendingLossField = AccessTools.Field(typeof(CombatManager), "_pendingLoss");

	private static readonly FieldInfo? CombatManagerReadyToBeginField = AccessTools.Field(typeof(CombatManager), "_playersReadyToBeginEnemyTurn");

	private static readonly FieldInfo? CombatManagerReadyToEndField = AccessTools.Field(typeof(CombatManager), "_playersReadyToEndTurn");

	private static readonly FieldInfo? CombatManagerPlayersTakingExtraTurnField = AccessTools.Field(typeof(CombatManager), "_playersTakingExtraTurn");

	private static readonly PropertyInfo? SerializableRunExtraFieldsProperty = AccessTools.Property(typeof(SerializableRun), "ExtraFields");

	private static readonly PropertyInfo? ActionExecutorIsPausedProperty = AccessTools.Property(typeof(RunManager).Assembly.GetType("MegaCrit.Sts2.Core.GameActions.ActionExecutor"), "IsPaused");

	private static readonly MethodInfo? ActionExecutorPauseMethod = AccessTools.Method(typeof(RunManager).Assembly.GetType("MegaCrit.Sts2.Core.GameActions.ActionExecutor"), "Pause", Type.EmptyTypes, null);

	private static readonly MethodInfo? ActionExecutorUnpauseMethod = AccessTools.Method(typeof(RunManager).Assembly.GetType("MegaCrit.Sts2.Core.GameActions.ActionExecutor"), "Unpause", Type.EmptyTypes, null);

	private static readonly MethodInfo? ActionQueueSetResetMethod = AccessTools.Method(typeof(RunManager).Assembly.GetType("MegaCrit.Sts2.Core.GameActions.ActionQueueSet"), "Reset", Type.EmptyTypes, null);

	private static readonly PropertyInfo? EventSynchronizerEventsProperty = AccessTools.Property(typeof(EventSynchronizer), "Events");

	private static readonly FieldInfo? EventSynchronizerEventsField = AccessTools.Field(typeof(EventSynchronizer), "_events");

	private static readonly PropertyInfo? EventSynchronizerIsSharedProperty = AccessTools.Property(typeof(EventSynchronizer), "IsShared");

	private static readonly FieldInfo? EventSynchronizerVotesField = AccessTools.Field(typeof(EventSynchronizer), "_playerVotes");

	private static readonly FieldInfo? EventSynchronizerPageIndexField = AccessTools.Field(typeof(EventSynchronizer), "_pageIndex");

	private static readonly MethodInfo? EventSynchronizerGetLocalEventMethod = AccessTools.Method(typeof(EventSynchronizer), "GetLocalEvent", Type.EmptyTypes, null);

	private static readonly FieldInfo? PlayerChoiceSynchronizerChoicesField = AccessTools.Field(typeof(RunManager).Assembly.GetType("MegaCrit.Sts2.Core.Multiplayer.Game.PlayerChoiceSynchronizer"), "_choices");

	private static readonly FieldInfo? PlayerChoiceSynchronizerChoiceIdsField = AccessTools.Field(typeof(RunManager).Assembly.GetType("MegaCrit.Sts2.Core.Multiplayer.Game.PlayerChoiceSynchronizer"), "_choiceIdByPlayer");

	private static readonly FieldInfo? RewardsSetSynchronizerRewardsField = AccessTools.Field(typeof(RunManager).Assembly.GetType("MegaCrit.Sts2.Core.Multiplayer.Game.RewardsSetSynchronizer"), "_rewardsSets");

	private static readonly FieldInfo? RewardsSetSynchronizerIdsField = AccessTools.Field(typeof(RunManager).Assembly.GetType("MegaCrit.Sts2.Core.Multiplayer.Game.RewardsSetSynchronizer"), "_rewardsSetIdByPlayer");

	public static async Task StartAsync(Node? screen, int nextLoopIndex, string nextSeed)
	{
		RunManager runManager = RunManager.Instance ?? throw new InvalidOperationException("STS2Plus endless loop: RunManager.Instance is null.");
		RunState runState = GetRunState(runManager) ?? throw new InvalidOperationException("STS2Plus endless loop: RunManager.State is null.");
		NGame game = NGame.Instance ?? throw new InvalidOperationException("STS2Plus endless loop: NGame.Instance is null.");
		ModEntry.Logger.Info($"STS2Plus endless loop: local transition started loopIndex={nextLoopIndex} seed={nextSeed}.", 1);
		PrepareLoop(runManager, runState, nextSeed);
		SerializableRun save = runManager.ToSave(null);
		ModEntry.Logger.Info("STS2Plus endless loop: save snapshot created.", 1);
		SanitizeSaveSnapshot(save);
		int revivedPlayers = ReviveSerializablePlayers(save);
		if (revivedPlayers > 0)
		{
			ModEntry.Logger.Info($"STS2Plus endless loop: revived {revivedPlayers} serialized player(s).", 1);
		}
		RunState resumedRunState = RunState.FromSerializable(save);
		ModEntry.Logger.Info("STS2Plus endless loop: run state rebuilt from save snapshot.", 1);
		SetRunState(runManager, resumedRunState);
		RunManagerInitializeSavedRunMethod?.Invoke(runManager, new object[1] { save });
		ResetTransientLoadState(runManager);
		ResetMultiplayerState(runManager, resumedRunState, nextLoopIndex, nextSeed, "before LoadRun");
		await PrepareScreensForLoadAsync(runManager, screen);
		SerializableRoom? preFinishedRoom = save.PreFinishedRoom;
		ModEntry.Logger.Info("STS2Plus endless loop: invoking NGame.LoadRun.", 1);
		await game.LoadRun(resumedRunState, preFinishedRoom);
		RunManager refreshedManager = RunManager.Instance ?? runManager;
		RunState refreshedState = GetRunState(refreshedManager) ?? resumedRunState;
		RunManagerClearScreensMethod?.Invoke(refreshedManager, Array.Empty<object>());
		ResetMultiplayerState(refreshedManager, refreshedState, nextLoopIndex, nextSeed, "after LoadRun");
		SafeLogPlayerState(refreshedState, "after LoadRun", nextLoopIndex, nextSeed);
		await AwaitProcessFrameAsync(game);
		refreshedManager = RunManager.Instance ?? refreshedManager;
		refreshedState = GetRunState(refreshedManager) ?? refreshedState;
		RunManagerClearScreensMethod?.Invoke(refreshedManager, Array.Empty<object>());
		ResetMultiplayerState(refreshedManager, refreshedState, nextLoopIndex, nextSeed, "after LoadRun +1 frame");
		SafeLogPlayerState(refreshedState, "after LoadRun +1 frame", nextLoopIndex, nextSeed);
		await EnsureVerifiedPostLoadStateAsync(game, refreshedManager, refreshedState, nextLoopIndex, nextSeed);
		ModEntry.Logger.Info("STS2Plus endless loop: NGame.LoadRun completed.", 1);
		ModEntry.Logger.Info($"STS2Plus endless loop: transition completed successfully loopIndex={nextLoopIndex} seed={nextSeed}.", 1);
	}

	private static void PrepareLoop(RunManager runManager, RunState runState, string nextSeed)
	{
		runState.CurrentActIndex = 0;
		runState.ActFloor = 0;
		RunStateNextRoomIdProperty?.SetValue(runState, 0);
		AssignRunRng(runState, nextSeed);
		RefreshSharedRelicGrabBag(runState);
		runState.Map = runState.Act.CreateMap(runState, false);
		runState.ClearVisitedMapCoordsDebug();
		ClearVisitedEventIds(runState);
		SetStartedWithNeowFlag(runState);
		runManager.WinTime = 0L;
		RunManagerShouldSaveProperty?.SetValue(runManager, true);
		RunManagerIsCleaningUpProperty?.SetValue(runManager, false);
		RunManagerIsAbandonedProperty?.SetValue(runManager, false);
	}

	private static void SanitizeSaveSnapshot(SerializableRun save)
	{
		save.PreFinishedRoom = null;
		save.CurrentActIndex = 0;
		save.WinTime = 0L;
		save.VisitedMapCoords?.Clear();
		save.EventsSeen?.Clear();
		save.MapDrawings = new SerializableMapDrawings();
		SetStartedWithNeowFlag(save);
	}

	private static int ReviveSerializablePlayers(SerializableRun save)
	{
		if (save.Players == null)
		{
			return 0;
		}
		int revived = 0;
		foreach (SerializablePlayer player in save.Players)
		{
			if (player.CurrentHp > 0)
			{
				continue;
			}
			player.CurrentHp = Math.Max(1, player.MaxHp);
			revived++;
		}
		return revived;
	}

	private static async Task PrepareScreensForLoadAsync(RunManager runManager, Node? screen)
	{
		if (screen is CanvasItem canvasItem && GodotObject.IsInstanceValid(canvasItem))
		{
			canvasItem.Visible = false;
		}
		SceneTree? tree = (screen != null && GodotObject.IsInstanceValid(screen)) ? screen.GetTree() : null;
		if (tree != null && screen != null)
		{
			await screen.ToSignal(tree, SceneTree.SignalName.ProcessFrame);
		}
		RunManagerClearScreensMethod?.Invoke(runManager, Array.Empty<object>());
		if (screen != null && GodotObject.IsInstanceValid(screen))
		{
			screen.QueueFree();
		}
	}

	private static void ResetTransientLoadState(RunManager runManager)
	{
		runManager.SavedMapsToLoad = new Dictionary<int, SerializableActMap>();
		runManager.MapDrawingsToLoad = new SerializableMapDrawings();
	}

	private static void AssignRunRng(RunState runState, string nextSeed)
	{
		if (RunStateRngField == null)
		{
			ModEntry.Logger.Warn("STS2Plus endless loop: missing RunState RNG backing field.");
			return;
		}
		RunStateRngField.SetValue(runState, new RunRngSet(nextSeed));
	}

	private static void RefreshSharedRelicGrabBag(RunState runState)
	{
		if (RunStateSharedRelicGrabBagField == null)
		{
			ModEntry.Logger.Warn("STS2Plus endless loop: missing RunState shared relic grab bag backing field.");
			return;
		}
		if (runState.Players.Count == 0)
		{
			return;
		}
		RelicGrabBag grabBag = new RelicGrabBag();
		grabBag.Populate(runState.Players[0], runState.Rng.TreasureRoomRelics);
		RunStateSharedRelicGrabBagField.SetValue(runState, grabBag);
	}

	private static void ClearVisitedEventIds(RunState runState)
	{
		if (RunStateVisitedEventIdsField == null)
		{
			ModEntry.Logger.Warn("STS2Plus endless loop: missing RunState visited event field.");
			return;
		}
		object? value = RunStateVisitedEventIdsField.GetValue(runState);
		if (value is IList list)
		{
			list.Clear();
			return;
		}
		if (value != null)
		{
			AccessTools.Method(value.GetType(), "Clear", Type.EmptyTypes)?.Invoke(value, Array.Empty<object>());
		}
	}

	private static void ResetMultiplayerState(RunManager runManager, RunState resumedRunState, int loopIndex, string mapSeed, string phase)
	{
		ResetMapSelectionState(runManager, resumedRunState, loopIndex, mapSeed, phase);
		ResetCombatManagerState();
		ResetActionQueueSynchronizer(runManager.ActionQueueSynchronizer);
		ResetCombatStateSynchronizer(runManager.CombatStateSynchronizer, resumedRunState);
		RefreshRunLocationBuffer(runManager.RunLocationTargetedBuffer, resumedRunState);
		LogMapSelectionState(runManager, resumedRunState, loopIndex, mapSeed, phase);
	}

	private static void ResetMapSelectionState(RunManager runManager, RunState resumedRunState, int loopIndex, string mapSeed, string phase)
	{
		MapSelectionSynchronizer? synchronizer = runManager.MapSelectionSynchronizer;
		if (synchronizer == null)
		{
			ModEntry.Logger.Warn($"STS2Plus endless loop: map selection synchronizer missing during {phase}.", 1);
			return;
		}
		EndlessMapSelectionParity.PrepareForFreshMap(runManager, resumedRunState, phase);
		ModEntry.Logger.Info($"STS2Plus endless loop: map sync reset {phase} act={resumedRunState.CurrentActIndex} floor={resumedRunState.ActFloor} loopIndex={loopIndex} seed={mapSeed}.", 1);
	}

	private static void ResetActionQueueSynchronizer(ActionQueueSynchronizer? synchronizer)
	{
		if (synchronizer == null)
		{
			return;
		}
		synchronizer.SetCombatState(ActionSynchronizerCombatState.NotInCombat);
		if (ActionQueueRequestedActionsField?.GetValue(synchronizer) is IList list)
		{
			list.Clear();
		}
	}

	private static void ResetCombatStateSynchronizer(CombatStateSynchronizer? synchronizer, RunState resumedRunState)
	{
		if (synchronizer == null)
		{
			return;
		}
		CombatStateSyncRunStateField?.SetValue(synchronizer, resumedRunState);
		ReplaceCollection(CombatStateSyncDataField?.GetValue(synchronizer));
		CombatStateSyncRngField?.SetValue(synchronizer, resumedRunState.Rng.ToSerializable());
		CombatStateSyncRelicGrabBagField?.SetValue(synchronizer, resumedRunState.SharedRelicGrabBag.ToSerializable());
		synchronizer.IsDisabled = false;
	}

	private static void RefreshRunLocationBuffer(RunLocationTargetedMessageBuffer? buffer, RunState resumedRunState)
	{
		if (buffer == null)
		{
			return;
		}
		if (RunLocationBufferMessagesField?.GetValue(buffer) is IList waitingMessages)
		{
			waitingMessages.Clear();
		}
		ReplaceCollection(RunLocationBufferVisitedLocationsField?.GetValue(buffer));
		buffer.OnLocationChanged(resumedRunState.RunLocation);
	}

	private static async Task AwaitProcessFrameAsync(NGame game)
	{
		SceneTree? tree = game.GetTree();
		if (tree != null)
		{
			await game.ToSignal(tree, SceneTree.SignalName.ProcessFrame);
		}
	}

	private static async Task EnsureVerifiedPostLoadStateAsync(NGame game, RunManager runManager, RunState runState, int loopIndex, string mapSeed)
	{
		TryCompletePostCleanupAfterValidAncientEvent(runManager, runState, "before post-load verification");
		string verificationFailure = GetPostLoadVerificationFailure(runManager, runState);
		if (string.IsNullOrEmpty(verificationFailure))
		{
			return;
		}
		ModEntry.Logger.Warn($"STS2Plus endless loop: multiplayer coordinator post-load verification failed before finalization: {verificationFailure}", 1);
		if (GameReflection.IsMultiplayerRun())
		{
			await TryFinalizePostLoadMapStateAsync(game, runManager, runState, loopIndex, mapSeed);
			runManager = RunManager.Instance ?? runManager;
			runState = GetRunState(runManager) ?? runState;
			TryCompletePostCleanupAfterValidAncientEvent(runManager, runState, "after multiplayer act finalization");
			verificationFailure = GetPostLoadVerificationFailure(runManager, runState);
		}
		if (!string.IsNullOrEmpty(verificationFailure))
		{
			ModEntry.Logger.Warn($"STS2Plus endless loop: multiplayer coordinator post-load verification failed: {verificationFailure}", 1);
			throw new InvalidOperationException("multiplayer coordinator post-load verification failed: " + verificationFailure);
		}
	}

	private static async Task TryFinalizePostLoadMapStateAsync(NGame game, RunManager runManager, RunState runState, int loopIndex, string mapSeed)
	{
		ModEntry.Logger.Info("STS2Plus endless loop: attempting multiplayer post-load act-entry finalization.", 1);
		MethodInfo? enterActMethod = AccessTools.Method(runManager.GetType(), "EnterAct", new Type[2]
		{
			typeof(int),
			typeof(bool)
		}, null);
		if (enterActMethod == null)
		{
			return;
		}
		object? result = enterActMethod.Invoke(runManager, new object[2] { 0, false });
		if (result is Task task)
		{
			await task;
		}
		RunManager refreshedManager = RunManager.Instance ?? runManager;
		RunState refreshedState = GetRunState(refreshedManager) ?? runState;
		RunManagerClearScreensMethod?.Invoke(refreshedManager, Array.Empty<object>());
		ResetMultiplayerState(refreshedManager, refreshedState, loopIndex, mapSeed, "after multiplayer act finalization");
		SafeLogPlayerState(refreshedState, "after multiplayer act finalization", loopIndex, mapSeed);
		await AwaitProcessFrameAsync(game);
		refreshedManager = RunManager.Instance ?? refreshedManager;
		refreshedState = GetRunState(refreshedManager) ?? refreshedState;
		RunManagerClearScreensMethod?.Invoke(refreshedManager, Array.Empty<object>());
		ResetMultiplayerState(refreshedManager, refreshedState, loopIndex, mapSeed, "after multiplayer act finalization +1 frame");
		SafeLogPlayerState(refreshedState, "after multiplayer act finalization +1 frame", loopIndex, mapSeed);
		TryCompletePostCleanupAfterValidAncientEvent(refreshedManager, refreshedState, "after multiplayer act finalization +1 frame");
	}

	private static string GetPostLoadVerificationFailure(RunManager runManager, RunState runState)
	{
		List<string> reasons = new List<string>();
		if (runState.MapLocation == null)
		{
			reasons.Add("mapLocation=null");
		}
		if (runState.RunLocation == null)
		{
			reasons.Add("runLocation=null");
		}
		MapPointType? currentMapPointType = GameReflection.GetCurrentMapPointType();
		if (currentMapPointType == null || currentMapPointType == MapPointType.Unknown)
		{
			reasons.Add("mapPoint=Unknown");
		}
		string currentRoomType = GameReflection.GetCurrentRoom()?.GetType().Name ?? "<null>";
		if (string.Equals(currentRoomType, "MapRoom", StringComparison.Ordinal) && currentMapPointType == MapPointType.Unknown)
		{
			reasons.Add("room=MapRoom");
		}
		bool validLoopedAncientEvent = IsValidLoopedAncientEventState(runManager, runState);
		if (EndlessLoopCoordinator.IsPostEndlessLoadCleanupActive)
		{
			if (!validLoopedAncientEvent)
			{
				reasons.Add("postCleanup=true");
			}
		}
		if (reasons.Count == 0)
		{
			return string.Empty;
		}
		LogPostLoadVerificationState(runManager, runState, reasons);
		return string.Join(", ", reasons);
	}

	private static void LogPostLoadVerificationState(RunManager runManager, RunState runState, List<string> reasons)
	{
		string reasonText = string.Join("; ", reasons);
		string votes = DescribeVotes(MapSelectionVotesField?.GetValue(runManager.MapSelectionSynchronizer));
		string bufferCurrent = runManager.RunLocationTargetedBuffer?.CurrentLocation.ToString() ?? "<null>";
		string currentRoom = GameReflection.GetCurrentRoom()?.GetType().Name ?? "<null>";
		string currentMapPoint = GameReflection.DescribeMapPoint(GameReflection.GetCurrentMapPoint());
		object? currentScreen = AccessTools.Property(typeof(NGame), "CurrentScreen")?.GetValue(NGame.Instance);
		bool mapVisible = string.Equals(currentScreen?.GetType().Name, "NMapScreen", StringComparison.Ordinal);
		bool eventVisible = string.Equals(currentScreen?.GetType().Name, "NEventRoom", StringComparison.Ordinal) || string.Equals(currentRoom, "EventRoom", StringComparison.Ordinal);
		ModEntry.Logger.Warn($"STS2Plus endless loop: post-load verification state failed={reasonText} room={currentRoom} mapPoint={currentMapPoint} mapLocation={runState.MapLocation} runLocation={runState.RunLocation} startedWithNeow={ReadStartedWithNeow(runState)} mapGen={runManager.MapSelectionSynchronizer?.MapGenerationCount ?? -1} votes={votes} bufferCurrent={bufferCurrent} postCleanup={EndlessLoopCoordinator.IsPostEndlessLoadCleanupActive} mapVisible={mapVisible}.", 1);
		if (eventVisible)
		{
			LogEventInputState(runManager, runState, "post-load verification event-visible");
		}
	}

	private static string ReadStartedWithNeow(RunState runState)
	{
		object? extraFields = RunStateExtraFieldsProperty?.GetValue(runState);
		object? value = (extraFields == null) ? null : AccessTools.Property(extraFields.GetType(), "StartedWithNeow")?.GetValue(extraFields);
		if (value is bool flag)
		{
			return flag ? "true" : "false";
		}
		return "<null>";
	}

	private static void ResetCombatManagerState()
	{
		CombatManager? manager = CombatManager.Instance;
		if (manager == null)
		{
			return;
		}
		manager.Reset(false);
		TrySetProperty(manager, CombatManagerIsPausedProperty, false);
		TrySetProperty(manager, CombatManagerIsEnemyTurnStartedProperty, false);
		TrySetProperty(manager, CombatManagerEndingPhaseOneProperty, false);
		TrySetProperty(manager, CombatManagerEndingPhaseTwoProperty, false);
		TrySetProperty(manager, CombatManagerIsInProgressProperty, false);
		TrySetProperty(manager, CombatManagerPlayerActionsDisabledProperty, false);
		CombatManagerStateField?.SetValue(manager, null);
		CombatManagerPendingLossField?.SetValue(manager, null);
		ReplaceCollection(CombatManagerReadyToBeginField?.GetValue(manager));
		ReplaceCollection(CombatManagerReadyToEndField?.GetValue(manager));
		ReplaceCollection(CombatManagerPlayersTakingExtraTurnField?.GetValue(manager));
	}

	private static void ReplaceCollection(object? collection)
	{
		if (collection == null)
		{
			return;
		}
		if (collection is IList list)
		{
			list.Clear();
			return;
		}
		AccessTools.Method(collection.GetType(), "Clear", Type.EmptyTypes)?.Invoke(collection, Array.Empty<object>());
	}

	private static void TrySetProperty(object target, PropertyInfo? property, object value)
	{
		try
		{
			property?.SetValue(target, value);
		}
		catch (Exception ex)
		{
			ModEntry.Logger.Warn("STS2Plus endless loop: failed to set " + property?.Name + " - " + ex.Message, 1);
		}
	}

	private static RunState? GetRunState(RunManager runManager)
	{
		return runManager.DebugOnlyGetState() ?? (RunManagerStateProperty?.GetValue(runManager) as RunState);
	}

	private static void SetRunState(RunManager runManager, RunState runState)
	{
		RunManagerStateProperty?.SetValue(runManager, runState);
	}

	internal static void LogMapSelectionState(RunManager? runManager, RunState? runState, int loopIndex, string mapSeed, string phase)
	{
		if (runManager == null || runState == null)
		{
			ModEntry.Logger.Warn($"STS2Plus endless loop: map sync log skipped during {phase} because run state is null.", 1);
			return;
		}
		MapSelectionSynchronizer? synchronizer = runManager.MapSelectionSynchronizer;
		RunLocationTargetedMessageBuffer? buffer = runManager.RunLocationTargetedBuffer;
		object? votes = MapSelectionVotesField?.GetValue(synchronizer);
		object? accepting = MapSelectionAcceptingVotesField?.GetValue(synchronizer);
		int mapGenerationCount = (synchronizer == null) ? -1 : synchronizer.MapGenerationCount;
		int waitingMessages = CountCollection(RunLocationBufferMessagesField?.GetValue(buffer));
		int visitedLocations = CountCollection(RunLocationBufferVisitedLocationsField?.GetValue(buffer));
		string bufferLocation = (buffer == null) ? "<null>" : buffer.CurrentLocation.ToString();
		string voteText = DescribeVotes(votes);
		ModEntry.Logger.Info($"STS2Plus endless loop: map sync state {phase} act={runState.CurrentActIndex} floor={runState.ActFloor} loopIndex={loopIndex} seed={mapSeed} mapLocation={runState.MapLocation} runLocation={runState.RunLocation} accepting={accepting} mapGen={mapGenerationCount} votes={voteText} bufferCurrent={bufferLocation} waiting={waitingMessages} visited={visitedLocations}.", 1);
	}

	internal static string DescribeVotes(object? votesObject)
	{
		if (votesObject is IEnumerable enumerable)
		{
			List<string> parts = new List<string>();
			int index = 0;
			foreach (object? item in enumerable)
			{
				parts.Add(index + ":" + (item?.ToString() ?? "<null>"));
				index++;
			}
			return "[" + string.Join(", ", parts) + "]";
		}
		return "<null>";
	}

	private static int CountCollection(object? value)
	{
		if (value is ICollection collection)
		{
			return collection.Count;
		}
		if (value == null)
		{
			return 0;
		}
		object? count = AccessTools.Property(value.GetType(), "Count")?.GetValue(value);
		return (count as int?) ?? 0;
	}

	internal static void ApplyFinalFirstCombatReset(string source)
	{
		RunManager? runManager = RunManager.Instance;
		RunState? runState = runManager?.DebugOnlyGetState();
		if (runManager == null || runState == null)
		{
			ModEntry.Logger.Warn("STS2Plus endless loop: skipped final first-combat reset because run state is unavailable.", 1);
			return;
		}
		ResetMultiplayerState(runManager, runState, GameReflection.GetLoopCount(), runState.Rng.StringSeed ?? "<null>", source);
		SafeLogPlayerState(runState, source, GameReflection.GetLoopCount(), runState.Rng.StringSeed ?? "<null>");
		ModEntry.Logger.Info("STS2Plus endless loop: applied final multiplayer reset before first endless combat.", 1);
	}

	internal static bool PrepareLoopedMultiplayerEventInputIfNeeded(string source)
	{
		RunManager? runManager = RunManager.Instance;
		RunState? runState = runManager?.DebugOnlyGetState() ?? (runManager == null ? null : GetRunState(runManager));
		if (runManager == null || runState == null || !GameReflection.IsMultiplayerRun() || GameReflection.GetLoopCount() <= 0)
		{
			return true;
		}
		LogEventInputState(runManager, runState, source + " before");
		RefreshLoopedMultiplayerRelicUi(runState, source + " before");
		if (EndlessLoopCoordinator.IsPostEndlessLoadCleanupActive)
		{
			ResetMultiplayerState(runManager, runState, GameReflection.GetLoopCount(), runState.Rng.StringSeed ?? "<null>", source + " cleanup");
			TryResetActionQueueSet(runManager.ActionQueueSet);
			TryPauseAndUnpauseActionExecutor(runManager.ActionExecutor);
			ClearStaleChoiceAndRewardState(runManager);
			RefreshLoopedMultiplayerRelicUi(runState, source + " cleanup");
			TryCompletePostCleanupAfterValidAncientEvent(runManager, runState, source);
		}
		LogEventInputState(runManager, runState, source + " after");
		RefreshLoopedMultiplayerRelicUi(runState, source + " after");
		string notReadyReason = GetLoopedEventInputNotReadyReason(runManager, runState);
		if (!string.IsNullOrEmpty(notReadyReason))
		{
			ModEntry.Logger.Warn("STS2Plus endless loop: looped multiplayer Neow not ready for input: " + notReadyReason, 1);
			return false;
		}
		ModEntry.Logger.Info("STS2Plus endless loop: looped multiplayer Neow ready for input.", 1);
		return true;
	}

	internal static void LogEventInputState(RunManager? runManager, RunState? runState, string phase)
	{
		if (runManager == null || runState == null)
		{
			ModEntry.Logger.Warn("STS2Plus endless loop: event input state skipped because run state is unavailable at " + phase + ".", 1);
			return;
		}
		string roomType = GameReflection.GetCurrentRoom()?.GetType().Name ?? "<null>";
		string mapPoint = GameReflection.DescribeMapPoint(GameReflection.GetCurrentMapPoint());
		string bufferCurrent = (runManager.RunLocationTargetedBuffer == null) ? "<null>" : runManager.RunLocationTargetedBuffer.CurrentLocation.ToString();
		string acceptingVotes = MapSelectionAcceptingVotesField?.GetValue(runManager.MapSelectionSynchronizer)?.ToString() ?? "<null>";
		int mapGenerationCount = runManager.MapSelectionSynchronizer?.MapGenerationCount ?? -1;
		int queuedActions = CountCollection(ActionQueueRequestedActionsField?.GetValue(runManager.ActionQueueSynchronizer));
		bool executorPaused = SafeGet(() => (ActionExecutorIsPausedProperty?.GetValue(runManager.ActionExecutor) as bool?).GetValueOrDefault(), false);
		string eventState = DescribeEventSynchronizerState(runManager.EventSynchronizer);
		int pendingChoices = CountCollection(PlayerChoiceSynchronizerChoicesField?.GetValue(runManager.PlayerChoiceSynchronizer));
		int pendingChoiceIds = CountCollection(PlayerChoiceSynchronizerChoiceIdsField?.GetValue(runManager.PlayerChoiceSynchronizer));
		int pendingRewards = CountCollection(RewardsSetSynchronizerRewardsField?.GetValue(runManager.RewardsSetSynchronizer));
		int pendingRewardIds = CountCollection(RewardsSetSynchronizerIdsField?.GetValue(runManager.RewardsSetSynchronizer));
		string relicUi = DescribeRelicUiReadiness(runState);
		ModEntry.Logger.Info($"STS2Plus endless loop: event input state {phase} postCleanup={EndlessLoopCoordinator.IsPostEndlessLoadCleanupActive} room={roomType} mapPoint={mapPoint} mapLocation={runState.MapLocation} runLocation={runState.RunLocation} bufferCurrent={bufferCurrent} mapGen={mapGenerationCount} acceptingVotesFromSource={acceptingVotes} queuedActions={queuedActions} actionExecutorPaused={executorPaused} pendingChoices={pendingChoices}/{pendingChoiceIds} pendingRewards={pendingRewards}/{pendingRewardIds} eventSync={eventState} relicUi={relicUi}.", 1);
	}

	private static string GetLoopedEventInputNotReadyReason(RunManager runManager, RunState runState)
	{
		List<string> reasons = new List<string>();
		if (EndlessLoopCoordinator.IsPostEndlessLoadCleanupActive)
		{
			reasons.Add("postCleanup=true");
		}
		if (runState.RunLocation == null)
		{
			reasons.Add("runLocation=null");
		}
		if (runManager.RunLocationTargetedBuffer?.CurrentLocation == null)
		{
			reasons.Add("bufferCurrent=null");
		}
		else if (!Equals(runManager.RunLocationTargetedBuffer.CurrentLocation, runState.RunLocation))
		{
			reasons.Add("bufferCurrent!=runLocation");
		}
		if (CountCollection(ActionQueueRequestedActionsField?.GetValue(runManager.ActionQueueSynchronizer)) > 0)
		{
			reasons.Add("queuedActions>0");
		}
		if (CountCollection(PlayerChoiceSynchronizerChoicesField?.GetValue(runManager.PlayerChoiceSynchronizer)) > 0)
		{
			reasons.Add("pendingChoices>0");
		}
		if (CountCollection(RewardsSetSynchronizerRewardsField?.GetValue(runManager.RewardsSetSynchronizer)) > 0)
		{
			reasons.Add("pendingRewards>0");
		}
		if (!HasLiveEventOptionsForAllPlayers(runManager.EventSynchronizer, runState))
		{
			reasons.Add("eventOptionsMissing");
		}
		return string.Join(", ", reasons);
	}

	private static void TryCompletePostCleanupAfterValidAncientEvent(RunManager runManager, RunState runState, string source)
	{
		if (!EndlessLoopCoordinator.IsPostEndlessLoadCleanupActive)
		{
			return;
		}
		if (!IsValidLoopedAncientEventState(runManager, runState))
		{
			return;
		}
		EndlessLoopCoordinator.CompletePostEndlessLoadCleanup(source);
		ModEntry.Logger.Info("STS2Plus endless loop: multiplayer loop postCleanup cleared after valid Ancient event finalization.", 1);
	}

	private static bool IsValidLoopedAncientEventState(RunManager runManager, RunState runState)
	{
		if (!GameReflection.IsMultiplayerRun() || GameReflection.GetLoopCount() <= 0)
		{
			return false;
		}
		string currentRoomType = GameReflection.GetCurrentRoom()?.GetType().Name ?? string.Empty;
		MapPointType? currentMapPointType = GameReflection.GetCurrentMapPointType();
		if (!string.Equals(currentRoomType, "EventRoom", StringComparison.Ordinal))
		{
			return false;
		}
		if (currentMapPointType != MapPointType.Ancient)
		{
			return false;
		}
		if (runState.MapLocation == null || runState.RunLocation == null)
		{
			return false;
		}
		if (CountCollection(ActionQueueRequestedActionsField?.GetValue(runManager.ActionQueueSynchronizer)) > 0)
		{
			return false;
		}
		if (CombatManagerStateField?.GetValue(CombatManager.Instance) != null)
		{
			return false;
		}
		if (!HasLiveEventOptionsForAllPlayers(runManager.EventSynchronizer, runState))
		{
			return false;
		}
		return true;
	}

	internal static bool IsLoopedMultiplayerAncientEventActive()
	{
		if (!GameReflection.IsMultiplayerRun() || GameReflection.GetLoopCount() <= 0)
		{
			return false;
		}
		string currentRoomType = GameReflection.GetCurrentRoom()?.GetType().Name ?? string.Empty;
		return string.Equals(currentRoomType, "EventRoom", StringComparison.Ordinal) && GameReflection.GetCurrentMapPointType() == MapPointType.Ancient;
	}

	internal static void RefreshLoopedMultiplayerRelicUi(RunState? runState, string phase)
	{
		try
		{
			if (runState == null)
			{
				return;
			}
			NGlobalUi? globalUi = NRun.Instance?.GlobalUi;
			if (globalUi != null)
			{
				CompactRelicDrawer.Attach(globalUi, runState);
			}
			ModEntry.Logger.Info("STS2Plus endless loop: looped multiplayer relic UI readiness: " + DescribeRelicUiReadiness(runState) + $" phase={phase}.", 1);
		}
		catch (Exception ex)
		{
			ModEntry.Logger.Warn("STS2Plus endless loop: failed to refresh relic UI - " + ex.Message, 1);
		}
	}

	private static string DescribeRelicUiReadiness(RunState? runState)
	{
		try
		{
			List<string> playerIds = new List<string>();
			foreach (Player player in runState?.Players ?? Array.Empty<Player>())
			{
				playerIds.Add(SafeGet(() => player.NetId.ToString(), "<unknown>"));
			}
			List<string> inventoryTargets = new List<string>();
			int inventoryCount = 0;
			Node? root = NRun.Instance?.GlobalUi;
			if (root != null)
			{
				foreach (Node child in EnumerateDescendants(root))
				{
					if (child is NRelicInventory inventory)
					{
						inventoryCount++;
						object? owner = AccessTools.Property(inventory.GetType(), "Player")?.GetValue(inventory) ?? AccessTools.Field(inventory.GetType(), "_player")?.GetValue(inventory) ?? AccessTools.Property(inventory.GetType(), "Owner")?.GetValue(inventory);
						string target = owner == null ? "<unknown>" : (AccessTools.Property(owner.GetType(), "NetId")?.GetValue(owner)?.ToString() ?? owner.ToString() ?? "<unknown>");
						inventoryTargets.Add(target);
					}
				}
			}
			List<string> missing = new List<string>();
			foreach (string id in playerIds)
			{
				bool found = false;
				foreach (string target in inventoryTargets)
				{
					if (string.Equals(target, id, StringComparison.Ordinal))
					{
						found = true;
						break;
					}
				}
				if (!found)
				{
					missing.Add(id);
				}
			}
			int panelCount = CountNamedNodes(root, "PlayerPanel") + CountNamedNodes(root, "MultiplayerPlayerState");
			return $"players=[{string.Join(", ", playerIds)}] inventories={inventoryCount} inventoryTargets=[{string.Join(", ", inventoryTargets)}] playerPanels={panelCount} missing=[{string.Join(", ", missing)}]";
		}
		catch (Exception ex)
		{
			return "error=" + ex.GetType().Name + ":" + ex.Message;
		}
	}

	private static int CountNamedNodes(Node? root, string nameFragment)
	{
		if (root == null)
		{
			return 0;
		}
		int count = 0;
		foreach (Node child in EnumerateDescendants(root))
		{
			if (child.GetType().Name.Contains(nameFragment, StringComparison.Ordinal))
			{
				count++;
			}
		}
		return count;
	}

	private static IEnumerable<Node> EnumerateDescendants(Node root)
	{
		foreach (Node child in root.GetChildren())
		{
			yield return child;
			foreach (Node descendant in EnumerateDescendants(child))
			{
				yield return descendant;
			}
		}
	}

	private static bool HasLiveEventOptionsForAllPlayers(EventSynchronizer? synchronizer, RunState runState)
	{
		if (synchronizer == null)
		{
			return false;
		}
		object? eventsObject = EventSynchronizerEventsProperty?.GetValue(synchronizer) ?? EventSynchronizerEventsField?.GetValue(synchronizer);
		if (eventsObject is not IEnumerable events)
		{
			return false;
		}
		int count = 0;
		foreach (object? eventModel in events)
		{
			if (eventModel == null)
			{
				return false;
			}
			object? options = AccessTools.Property(eventModel.GetType(), "CurrentOptions")?.GetValue(eventModel);
			if (CountCollection(options) <= 0)
			{
				return false;
			}
			count++;
		}
		return count >= (runState.Players?.Count ?? 0);
	}

	private static void TryResetActionQueueSet(object? actionQueueSet)
	{
		try
		{
			ActionQueueSetResetMethod?.Invoke(actionQueueSet, Array.Empty<object>());
		}
		catch (Exception ex)
		{
			ModEntry.Logger.Warn("STS2Plus endless loop: failed to reset ActionQueueSet - " + ex.Message, 1);
		}
	}

	private static void TryPauseAndUnpauseActionExecutor(object? actionExecutor)
	{
		try
		{
			ActionExecutorPauseMethod?.Invoke(actionExecutor, Array.Empty<object>());
			ActionExecutorUnpauseMethod?.Invoke(actionExecutor, Array.Empty<object>());
		}
		catch (Exception ex)
		{
			ModEntry.Logger.Warn("STS2Plus endless loop: failed to refresh ActionExecutor pause state - " + ex.Message, 1);
		}
	}

	private static void ClearStaleChoiceAndRewardState(RunManager runManager)
	{
		ReplaceCollection(PlayerChoiceSynchronizerChoicesField?.GetValue(runManager.PlayerChoiceSynchronizer));
		ReplaceCollection(PlayerChoiceSynchronizerChoiceIdsField?.GetValue(runManager.PlayerChoiceSynchronizer));
		ReplaceCollection(RewardsSetSynchronizerRewardsField?.GetValue(runManager.RewardsSetSynchronizer));
		ReplaceCollection(RewardsSetSynchronizerIdsField?.GetValue(runManager.RewardsSetSynchronizer));
		ReplaceCollection(EventSynchronizerVotesField?.GetValue(runManager.EventSynchronizer));
		try
		{
			EventSynchronizerPageIndexField?.SetValue(runManager.EventSynchronizer, 0u);
		}
		catch (Exception ex)
		{
			ModEntry.Logger.Warn("STS2Plus endless loop: failed to reset EventSynchronizer page index - " + ex.Message, 1);
		}
	}

	private static string DescribeEventSynchronizerState(EventSynchronizer? synchronizer)
	{
		if (synchronizer == null)
		{
			return "<null>";
		}
		try
		{
			bool isShared = (EventSynchronizerIsSharedProperty?.GetValue(synchronizer) as bool?).GetValueOrDefault();
			int voteCount = CountCollection(EventSynchronizerVotesField?.GetValue(synchronizer));
			uint pageIndex = (EventSynchronizerPageIndexField?.GetValue(synchronizer) as uint?).GetValueOrDefault();
			object? localEvent = EventSynchronizerGetLocalEventMethod?.Invoke(synchronizer, Array.Empty<object>());
			string localEventId = localEvent == null ? "<null>" : (AccessTools.Property(localEvent.GetType(), "Id")?.GetValue(localEvent)?.ToString() ?? localEvent.GetType().Name);
			object? localOptions = localEvent == null ? null : AccessTools.Property(localEvent.GetType(), "CurrentOptions")?.GetValue(localEvent);
			List<string> optionTextKeys = new List<string>();
			if (localOptions is IEnumerable enumerable)
			{
				foreach (object? option in enumerable)
				{
					optionTextKeys.Add(AccessTools.Property(option?.GetType(), "TextKey")?.GetValue(option)?.ToString() ?? "<null>");
				}
			}
			return $"shared={isShared} page={pageIndex} votes={voteCount} localEvent={localEventId} localOptions=[{string.Join(", ", optionTextKeys)}]";
		}
		catch (Exception ex)
		{
			return "error=" + ex.GetType().Name + ":" + ex.Message;
		}
	}

	private static void SafeLogPlayerState(RunState? runState, string phase, int loopIndex, string mapSeed)
	{
		try
		{
			LogPlayerState(runState, phase, loopIndex, mapSeed);
		}
		catch (Exception ex)
		{
			ModEntry.Logger.Warn($"STS2Plus endless loop: ignored LogPlayerState failure phase={phase} loopIndex={loopIndex} seed={mapSeed}: {ex}", 1);
		}
	}

	private static void SetStartedWithNeowFlag(object target)
	{
		try
		{
			object? extraFields = null;
			if (target is RunState)
			{
				extraFields = RunStateExtraFieldsProperty?.GetValue(target);
			}
			else if (target is SerializableRun)
			{
				extraFields = SerializableRunExtraFieldsProperty?.GetValue(target);
			}
			if (extraFields == null)
			{
				return;
			}
			AccessTools.Property(extraFields.GetType(), "StartedWithNeow")?.SetValue(extraFields, true);
		}
		catch (Exception ex)
		{
			ModEntry.Logger.Warn("STS2Plus endless loop: failed to set StartedWithNeow - " + ex.Message, 1);
		}
	}

	private static void LogPlayerState(RunState? runState, string phase, int loopIndex, string mapSeed)
	{
		try
		{
			if (runState == null)
			{
				ModEntry.Logger.Warn($"STS2Plus endless loop: LogPlayerState partial/skipped during {phase}: runState is null.", 1);
				return;
			}
			List<string> players = new List<string>();
			var playerList = runState.Players;
			if (playerList == null)
			{
				ModEntry.Logger.Warn($"STS2Plus endless loop: LogPlayerState partial/skipped during {phase}: player list is null.", 1);
			}
			else
			{
				foreach (Player player in playerList)
				{
					if (player == null)
					{
						players.Add("player=<null>");
						continue;
					}
					players.Add($"player={SafeGet(() => player.NetId.ToString(), "<unknown>")} deck={GetPileCount(player, PileType.Deck)} draw={GetPileCount(player, PileType.Draw)} hand={GetPileCount(player, PileType.Hand)} discard={GetPileCount(player, PileType.Discard)}");
				}
			}
			int queuedActions = SafeGet(() => CountCollection(ActionQueueRequestedActionsField?.GetValue(RunManager.Instance?.ActionQueueSynchronizer)), 0);
			string combatState = SafeGet(() => (CombatManager.Instance == null) ? "<null>" : CombatManagerStateField?.GetValue(CombatManager.Instance)?.ToString() ?? "<null>", "<unavailable>");
			int playerCount = playerList?.Count ?? 0;
			ModEntry.Logger.Info($"STS2Plus endless loop: state {phase} loopIndex={loopIndex} seed={mapSeed} players={playerCount} queuedActions={queuedActions} combatState={combatState} details=[{string.Join("; ", players)}].", 1);
		}
		catch (Exception ex)
		{
			ModEntry.Logger.Warn($"STS2Plus endless loop: LogPlayerState partial/skipped during {phase}: {ex.GetType().Name}: {ex.Message}", 1);
		}
	}

	private static int GetPileCount(Player player, PileType pileType)
	{
		try
		{
			CardPile pile = CardPile.Get(pileType, player);
			if (pile == null)
			{
				return -1;
			}
			return pile.Cards?.Count ?? -1;
		}
		catch (Exception ex)
		{
			ModEntry.Logger.Warn($"STS2Plus endless loop: LogPlayerState partial/skipped for player {SafeGet(() => player.NetId.ToString(), "<unknown>")} pile={pileType}: {ex.GetType().Name}: {ex.Message}", 1);
			return -1;
		}
	}

	private static T SafeGet<T>(Func<T> getter, T fallback)
	{
		try
		{
			return getter();
		}
		catch
		{
			return fallback;
		}
	}
}
