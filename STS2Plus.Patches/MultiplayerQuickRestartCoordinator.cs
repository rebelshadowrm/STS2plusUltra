using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Multiplayer.Game;
using MegaCrit.Sts2.Core.Multiplayer.Game.Lobby;
using MegaCrit.Sts2.Core.Multiplayer.Game.PeerInput;
using MegaCrit.Sts2.Core.Multiplayer.Messages.Lobby;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Audio;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.Screens.Capstones;
using MegaCrit.Sts2.Core.Nodes.Screens.Map;
using MegaCrit.Sts2.Core.Nodes.Screens.Overlays;
using MegaCrit.Sts2.Core.Platform;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves;
using STS2Plus.Multiplayer;
using STS2Plus.Ui;

namespace STS2Plus.Patches;

internal static class MultiplayerQuickRestartCoordinator
{
	private sealed class RestartLoadRunListener : ILoadRunLobbyListener
	{
		private LoadRunLobby? lobby;

		private bool hasStarted;

		public void Bind(LoadRunLobby value)
		{
			lobby = value;
		}

		public void PlayerConnected(ulong playerId)
		{
			ModEntry.Logger.Info($"STS2Plus quick restart player connected: {playerId}", 1);
		}

		public void RemotePlayerDisconnected(ulong playerId)
		{
			ModEntry.Logger.Warn($"STS2Plus quick restart player disconnected: {playerId}", 1);
			if (!hasStarted && lobby != null)
			{
				TaskHelper.RunSafely(AbortToMainMenuAsync(lobby.NetService));
			}
		}

		public Task<bool> ShouldAllowRunToBegin()
		{
			if (lobby == null)
			{
				return Task.FromResult(result: false);
			}
			bool flag = lobby.ConnectedPlayerIds.Count >= lobby.Run.Players.Count;
			if (!flag)
			{
				ModEntry.Logger.Warn("STS2Plus quick restart blocked because not all save players reconnected.", 1);
			}
			return Task.FromResult(flag);
		}

		public void BeginRun()
		{
			if (!hasStarted && lobby != null)
			{
				hasStarted = true;
				TaskHelper.RunSafely(StartFreshMultiplayerRunAsync(lobby));
			}
		}

		public void PlayerReadyChanged(ulong playerId)
		{
			ModEntry.Logger.Info($"STS2Plus quick restart ready changed: {playerId}", 1);
		}

		public void LocalPlayerDisconnected(NetErrorInfo info)
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			ModEntry.Logger.Warn($"STS2Plus quick restart local disconnect: {info.GetReason()}", 1);
			if (lobby == null)
			{
				ResetRestartState();
			}
			else
			{
				TaskHelper.RunSafely(AbortToMainMenuAsync(lobby.NetService));
			}
		}
	}

	private static readonly FieldInfo? RunHistoryUploadedField = AccessTools.Field(typeof(RunManager), "_runHistoryWasUploaded");

	private static readonly PropertyInfo? RunManagerStateProperty = AccessTools.Property(typeof(RunManager), "State");

	private static readonly PropertyInfo? RunManagerRunLobbyProperty = AccessTools.Property(typeof(RunManager), "RunLobby");

	private static readonly PropertyInfo? RunManagerCleaningUpProperty = AccessTools.Property(typeof(RunManager), "IsCleaningUp");

	private static readonly MethodInfo? RunManagerInitializeSharedMethod = AccessTools.Method(typeof(RunManager), "InitializeShared", new Type[8]
	{
		typeof(INetGameService),
		typeof(PeerInputSynchronizer),
		typeof(bool),
		typeof(DateTimeOffset?),
		typeof(long),
		typeof(long),
		typeof(long),
		typeof(int)
	}, (Type[])null);

	private static readonly MethodInfo? RunManagerInitializeRunLobbyMethod = AccessTools.Method(typeof(RunManager), "InitializeRunLobby", new Type[2]
	{
		typeof(INetGameService),
		typeof(RunState)
	}, (Type[])null);

	private static readonly MethodInfo? RunManagerInitializeNewRunMethod = AccessTools.Method(typeof(RunManager), "InitializeNewRun", (Type[])null, (Type[])null);

	private static readonly MethodInfo? NGameStartRunMethod = AccessTools.Method(typeof(NGame), "StartRun", new Type[1] { typeof(RunState) }, (Type[])null);

	private static INetGameService? attachedService;

	private static TaskCompletionSource<ClientLoadJoinResponseMessage>? clientLoadJoinCompletion;

	private static LoadRunLobby? activeLobby;

	private static RunState? stagedRunState;

	private static long activeRestartToken;

	private static bool isRestarting;

	public static void AttachCurrentRun()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Invalid comparison between Unknown and I4
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Invalid comparison between Unknown and I4
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Invalid comparison between Unknown and I4
		INetGameService netService = RunManager.Instance.NetService;
		if ((int)netService.Type != 2 && (int)netService.Type != 3)
		{
			Detach();
		}
		else if (attachedService != netService)
		{
			Detach();
			attachedService = netService;
			ModEntry.Logger.Info($"STS2Plus.Net quick-restart attach type={netService.Type} connected={netService.IsConnected} netId={netService.NetId}", 1);
			netService.RegisterMessageHandler<QuickRestartBeginMessage>((MessageHandlerDelegate<QuickRestartBeginMessage>)OnQuickRestartBegin);
			if ((int)netService.Type == 2)
			{
				netService.RegisterMessageHandler<QuickRestartRequestedMessage>((MessageHandlerDelegate<QuickRestartRequestedMessage>)OnQuickRestartRequested);
			}
		}
	}

	public static void Detach()
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Invalid comparison between Unknown and I4
		if (attachedService != null)
		{
			ModEntry.Logger.Info($"STS2Plus.Net quick-restart detach type={attachedService.Type} connected={attachedService.IsConnected} netId={attachedService.NetId}", 1);
			attachedService.UnregisterMessageHandler<QuickRestartBeginMessage>((MessageHandlerDelegate<QuickRestartBeginMessage>)OnQuickRestartBegin);
			if ((int)attachedService.Type == 2)
			{
				attachedService.UnregisterMessageHandler<QuickRestartRequestedMessage>((MessageHandlerDelegate<QuickRestartRequestedMessage>)OnQuickRestartRequested);
			}
			UnregisterLoadJoinResponseHandler(attachedService);
		}
		attachedService = null;
		clientLoadJoinCompletion = null;
		activeLobby = null;
		stagedRunState = null;
		activeRestartToken = 0L;
		isRestarting = false;
		SpeedControlOverlay.SuspendGameplaySpeed(suspended: false);
	}

	public static void RequestRestart()
	{
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Invalid comparison between Unknown and I4
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Invalid comparison between Unknown and I4
		INetGameService val = attachedService;
		if (val == null || !val.IsConnected || isRestarting)
		{
			ModEntry.Logger.Warn($"STS2Plus.Net quick-restart request ignored attached={val != null} connected={((val != null) ? new bool?(val.IsConnected) : null)} restarting={isRestarting}", 1);
		}
		else
		{
			SpeedControlOverlay.SuspendGameplaySpeed(suspended: true);
			if ((int)val.Type == 2)
			{
				TaskHelper.RunSafely(BeginHostRestartAsync(val));
			}
			else if ((int)val.Type == 3)
			{
				val.SendMessage<QuickRestartRequestedMessage>(default(QuickRestartRequestedMessage));
				ModEntry.Logger.Info("STS2Plus quick restart requested from client.", 1);
			}
		}
	}

	private static void OnQuickRestartRequested(QuickRestartRequestedMessage _, ulong senderId)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Invalid comparison between Unknown and I4
		INetGameService val = attachedService;
		if (val != null && (int)val.Type == 2 && !isRestarting && senderId != val.NetId)
		{
			ModEntry.Logger.Info($"STS2Plus.Net quick-restart host received request sender={senderId}", 1);
			SpeedControlOverlay.SuspendGameplaySpeed(suspended: true);
			TaskHelper.RunSafely(BeginHostRestartAsync(val));
		}
	}

	private static void OnQuickRestartBegin(QuickRestartBeginMessage message, ulong _)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Invalid comparison between Unknown and I4
		INetGameService val = attachedService;
		if (val != null && (int)val.Type == 3 && (!isRestarting || activeRestartToken != message.RestartToken))
		{
			ModEntry.Logger.Info($"STS2Plus.Net quick-restart client begin token={message.RestartToken}", 1);
			SpeedControlOverlay.SuspendGameplaySpeed(suspended: true);
			TaskHelper.RunSafely(BeginClientRestartAsync(val, message.RestartToken));
		}
	}

	private static async Task BeginHostRestartAsync(INetGameService service)
	{
		if (isRestarting)
		{
			return;
		}
		ModEntry.Logger.Info($"STS2Plus.Net quick-restart host begin connected={service.IsConnected} netId={service.NetId}", 1);
		SerializableRun save = LoadMultiplayerRunSave(service);
		if (save == null)
		{
			SpeedControlOverlay.SuspendGameplaySpeed(suspended: false);
			return;
		}
		isRestarting = true;
		activeRestartToken = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
		bool stageSucceeded = false;
		bool beginSent = false;
		LoadRunLobby lobby = null;
		try
		{
			stageSucceeded = StageCurrentRunLobbyOnly();
			RestartLoadRunListener listener = new RestartLoadRunListener();
			lobby = (activeLobby = new LoadRunLobby(service, (ILoadRunLobbyListener)(object)listener, save));
			listener.Bind(lobby);
			lobby.AddLocalHostPlayer();
			InitializeLobbyNetworking(lobby);
			service.SendMessage<QuickRestartBeginMessage>(new QuickRestartBeginMessage
			{
				RestartToken = activeRestartToken
			});
			beginSent = true;
			lobby.SetReady(true);
			ModEntry.Logger.Info("STS2Plus quick restart lobby started as host.", 1);
		}
		catch (Exception exception)
		{
			ModEntry.Logger.Error($"STS2Plus multiplayer quick restart failed before run reload: {exception}", 1);
			if (lobby != null)
			{
				try
				{
					lobby.CleanUp(false);
				}
				catch (Exception ex)
				{
					Exception cleanupException = ex;
					ModEntry.Logger.Warn("STS2Plus failed to clean up quick restart lobby: " + cleanupException.Message, 1);
				}
			}
			activeLobby = null;
			if (!beginSent && stageSucceeded)
			{
				RestoreStagedRunLobby();
				ResetRestartState();
				return;
			}
			await AbortToMainMenuAsync(service);
		}
	}

	private static async Task BeginClientRestartAsync(INetGameService service, long restartToken)
	{
		if (isRestarting)
		{
			return;
		}
		ModEntry.Logger.Info($"STS2Plus.Net quick-restart client begin-load connected={service.IsConnected} netId={service.NetId} token={restartToken}", 1);
		isRestarting = true;
		activeRestartToken = restartToken;
		try
		{
			StageCurrentRunLobbyOnly();
			clientLoadJoinCompletion = new TaskCompletionSource<ClientLoadJoinResponseMessage>(TaskCreationOptions.RunContinuationsAsynchronously);
			service.RegisterMessageHandler<ClientLoadJoinResponseMessage>((MessageHandlerDelegate<ClientLoadJoinResponseMessage>)OnClientLoadJoinResponse);
			service.SendMessage<ClientLoadJoinRequestMessage>(default(ClientLoadJoinRequestMessage));
			ClientLoadJoinResponseMessage response = await clientLoadJoinCompletion.Task.WaitAsync(TimeSpan.FromSeconds(10L));
			RestartLoadRunListener listener = new RestartLoadRunListener();
			LoadRunLobby lobby = (activeLobby = new LoadRunLobby(service, (ILoadRunLobbyListener)(object)listener, response));
			listener.Bind(lobby);
			InitializeLobbyNetworking(lobby);
			lobby.SetReady(true);
			ModEntry.Logger.Info("STS2Plus quick restart lobby started as client.", 1);
		}
		catch (Exception exception)
		{
			ModEntry.Logger.Error($"STS2Plus multiplayer quick restart failed on client: {exception}", 1);
			await AbortToMainMenuAsync(service);
		}
		finally
		{
			UnregisterLoadJoinResponseHandler(service);
		}
	}

	private static void OnClientLoadJoinResponse(ClientLoadJoinResponseMessage message, ulong _)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		clientLoadJoinCompletion?.TrySetResult(message);
	}

	private static void UnregisterLoadJoinResponseHandler(INetGameService service)
	{
		service.UnregisterMessageHandler<ClientLoadJoinResponseMessage>((MessageHandlerDelegate<ClientLoadJoinResponseMessage>)OnClientLoadJoinResponse);
		clientLoadJoinCompletion = null;
	}

	private static SerializableRun? LoadMultiplayerRunSave(INetGameService service)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		SaveManager instance = SaveManager.Instance;
		ulong num = PlatformUtil.GetLocalPlayerId(service.Platform);
		if (num == 0)
		{
			num = service.NetId;
		}
		ReadSaveResult<SerializableRun> val = instance.LoadAndCanonicalizeMultiplayerRunSave(num);
		if (!val.Success || val.SaveData == null)
		{
			ModEntry.Logger.Warn($"STS2Plus quick restart could not load multiplayer save. Status: {val.Status}", 1);
			return null;
		}
		return val.SaveData;
	}

	private static bool StageCurrentRunLobbyOnly()
	{
		if (stagedRunState != null)
		{
			return true;
		}
		RunManager instance = RunManager.Instance;
		RunState runState = GetRunState(instance);
		if (runState == null)
		{
			return false;
		}
		stagedRunState = runState;
		if (instance.RunLobby != null)
		{
			DisposeIfPossible(instance.RunLobby);
			SetRunLobby(instance, null);
		}
		return true;
	}

	private static void RestoreStagedRunLobby()
	{
		INetGameService val = attachedService;
		if (val == null || stagedRunState == null)
		{
			ResetRestartState();
			return;
		}
		try
		{
			RunManagerInitializeRunLobbyMethod?.Invoke(RunManager.Instance, new object[] { val, stagedRunState });
			RestoreCurrentRunNetworking();
		}
		catch (Exception value)
		{
			ModEntry.Logger.Error($"STS2Plus failed to restore the live run lobby after quick restart abort: {value}", 1);
		}
		ResetRestartState();
	}

	private static void RestoreCurrentRunNetworking()
	{
		NGame instance = NGame.Instance;
		RunManager instance2 = RunManager.Instance;
		if (instance != null && attachedService != null)
		{
			if (instance2.InputSynchronizer != null && instance2.RunLobby != null)
			{
				instance.RemoteCursorContainer.Initialize(instance2.InputSynchronizer, (IEnumerable<ulong>)instance2.RunLobby.ConnectedPlayerIds);
			}
			instance.ReactionContainer.InitializeNetworking(attachedService);
		}
	}

	private static void InitializeLobbyNetworking(LoadRunLobby lobby)
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Invalid comparison between Unknown and I4
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Invalid comparison between Unknown and I4
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Invalid comparison between Unknown and I4
		NGame instance = NGame.Instance;
		if (instance != null)
		{
			instance.RemoteCursorContainer.Initialize(lobby.InputSynchronizer, (IEnumerable<ulong>)lobby.ConnectedPlayerIds);
			instance.ReactionContainer.InitializeNetworking(lobby.NetService);
			instance.DebugSeedOverride = null;
			MegaCrit.Sts2.Core.Logging.Logger.logLevelTypeMap[(LogType)1] = (LogLevel)(((int)lobby.NetService.Type == 1) ? 3 : 2);
			MegaCrit.Sts2.Core.Logging.Logger.logLevelTypeMap[(LogType)2] = (LogLevel)(((int)lobby.NetService.Type == 1) ? 3 : 0);
			MegaCrit.Sts2.Core.Logging.Logger.logLevelTypeMap[(LogType)3] = (LogLevel)(((int)lobby.NetService.Type == 1) ? 3 : 0);
		}
	}

	private static async Task StartFreshMultiplayerRunAsync(LoadRunLobby lobby)
	{
		NGame game = NGame.Instance;
		if (game == null)
		{
			ResetRestartState();
			return;
		}
		bool fullyToreDownCurrentRun = false;
		try
		{
			NetLoadingHandle loading = new NetLoadingHandle(lobby.NetService);
			try
			{
				NAudioManager instance = NAudioManager.Instance;
				if (instance != null)
				{
					instance.StopMusic();
				}
				await game.Transition.FadeOut(0.8f, "res://materials/transitions/fade_transition_mat.tres", (CancellationToken?)null);
				PrepareRunManagerForSessionReuse();
				fullyToreDownCurrentRun = true;
				game.RootSceneContainer.SetCurrentScene(new Control
				{
					Name = (StringName)"STS2PlusQuickRestartLoading"
				});
				if (!QuickRestartSetupBuilder.TryCreateRestartPlayers(lobby.Run, out List<Player> players) || !QuickRestartSetupBuilder.TryGetRestartActs(lobby.Run, out List<ActModel> acts))
				{
					throw new InvalidOperationException("Could not build the fresh multiplayer restart setup.");
				}
				List<ModifierModel> modifiers = QuickRestartSetupBuilder.CreateRestartModifiers(lobby.Run);
				string seed = QuickRestartSetupBuilder.ResolveRestartSeed(lobby.Run);
				RunState runState = RunState.CreateForNewRun((IReadOnlyList<Player>)players, (IReadOnlyList<ActModel>)acts.Select((ActModel act) => act.ToMutable()).ToList(), (IReadOnlyList<ModifierModel>)modifiers, lobby.Run.GameMode, lobby.Run.Ascension, seed);
				SetUpFreshMultiplayerRun(RunManager.Instance, runState, lobby);
				stagedRunState = null;
				object obj = NGameStartRunMethod?.Invoke(game, new object[1] { runState });
				if (!(obj is Task startRunTask))
				{
					throw new InvalidOperationException("STS2Plus quick restart could not find NGame.StartRun.");
				}
				await startRunTask;
				lobby.CleanUp(false);
				activeLobby = null;
				await game.Transition.FadeIn(0.8f, "res://materials/transitions/fade_transition_mat.tres", (CancellationToken?)null);
				ResetRestartState();
				ModEntry.Logger.Info("STS2Plus multiplayer quick restart started a fresh run. Seed: " + seed, 1);
			}
			finally
			{
				((IDisposable)loading)?.Dispose();
			}
		}
		catch (Exception exception)
		{
			ModEntry.Logger.Error($"STS2Plus multiplayer quick restart failed while starting a fresh run: {exception}", 1);
			if (!fullyToreDownCurrentRun)
			{
				await AbortToMainMenuAsync(lobby.NetService);
				return;
			}
			try
			{
				lobby.CleanUp(true);
			}
			catch (Exception cleanupException)
			{
				ModEntry.Logger.Warn("STS2Plus failed to clean up restart lobby after load failure: " + cleanupException.Message, 1);
			}
			await ReturnToMainMenuAsync();
		}
	}

	private static void SetUpFreshMultiplayerRun(RunManager runManager, RunState runState, LoadRunLobby lobby)
	{
		if (RunManagerInitializeSharedMethod == null || RunManagerInitializeNewRunMethod == null)
		{
			throw new InvalidOperationException("STS2Plus quick restart could not resolve the multiplayer run setup methods.");
		}
		SetRunState(runManager, runState);
		RunManagerInitializeSharedMethod.Invoke(runManager, new object[8]
		{
			lobby.NetService,
			lobby.InputSynchronizer,
			true,
			lobby.Run.DailyTime,
			DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
			0L,
			0L,
			0
		});
		RunManagerInitializeRunLobbyMethod?.Invoke(runManager, new object[] { lobby.NetService, runState });
		RunManagerInitializeNewRunMethod.Invoke(runManager, Array.Empty<object>());
		runManager.GenerateRooms();
	}

	private static async Task AbortToMainMenuAsync(INetGameService service)
	{
		activeLobby = null;
		ModEntry.Logger.Warn($"STS2Plus.Net quick-restart abort type={service.Type} connected={service.IsConnected} netId={service.NetId}", 1);
		try
		{
			if (service.IsConnected)
			{
				service.Disconnect((NetError)17, false);
			}
		}
		catch (Exception ex)
		{
			Exception exception = ex;
			ModEntry.Logger.Warn("STS2Plus failed to disconnect after quick restart abort: " + exception.Message, 1);
		}
		await ReturnToMainMenuAsync();
	}

	private static async Task ReturnToMainMenuAsync()
	{
		ResetRestartState();
		NGame game = NGame.Instance;
		if (game == null)
		{
			return;
		}
		try
		{
			await game.ReturnToMainMenu();
		}
		catch (Exception exception)
		{
			ModEntry.Logger.Warn("STS2Plus failed to return to the main menu after quick restart abort: " + exception.Message, 1);
		}
	}

	private static void ResetRestartState()
	{
		activeLobby = null;
		stagedRunState = null;
		activeRestartToken = 0L;
		isRestarting = false;
		SpeedControlOverlay.SuspendGameplaySpeed(suspended: false);
	}

	private static RunState? GetRunState(RunManager runManager)
	{
		object? obj = RunManagerStateProperty?.GetValue(runManager);
		RunState val = (RunState)((obj is RunState) ? obj : null);
		if (val != null)
		{
			return val;
		}
		return null;
	}

	private static void SetRunState(RunManager runManager, RunState? state)
	{
		RunManagerStateProperty?.SetValue(runManager, state);
	}

	private static void SetRunLobby(RunManager runManager, object? runLobby)
	{
		RunManagerRunLobbyProperty?.SetValue(runManager, runLobby);
	}

	private static void PrepareRunManagerForSessionReuse()
	{
		RunManager instance = RunManager.Instance;
		if (GetRunState(instance) == null)
		{
			return;
		}
		RunManagerCleaningUpProperty?.SetValue(instance, true);
		try
		{
			RunHistoryUploadedField?.SetValue(instance, false);
			ActionQueueSet actionQueueSet = instance.ActionQueueSet;
			if (actionQueueSet != null)
			{
				actionQueueSet.Reset();
			}
			NAudioManager instance2 = NAudioManager.Instance;
			if (instance2 != null)
			{
				instance2.StopAllLoops();
			}
			NOverlayStack instance3 = NOverlayStack.Instance;
			if (instance3 != null)
			{
				instance3.Clear();
			}
			NCapstoneContainer instance4 = NCapstoneContainer.Instance;
			if (instance4 != null)
			{
				instance4.CleanUp();
			}
			NMapScreen instance5 = NMapScreen.Instance;
			if (instance5 != null)
			{
				instance5.CleanUp();
			}
			NModalContainer instance6 = NModalContainer.Instance;
			if (instance6 != null)
			{
				instance6.Clear();
			}
			CombatManager.Instance.Reset(true);
			DisposeIfPossible(instance.CombatReplayWriter);
			DisposeIfPossible(instance.ActionQueueSynchronizer);
			DisposeIfPossible(instance.PlayerChoiceSynchronizer);
			DisposeIfPossible(instance.RewardSynchronizer);
			DisposeIfPossible(instance.RestSiteSynchronizer);
			DisposeIfPossible(instance.FlavorSynchronizer);
			DisposeIfPossible(instance.ChecksumTracker);
			DisposeIfPossible(instance.EventSynchronizer);
			DisposeIfPossible(instance.OneOffSynchronizer);
			DisposeIfPossible(instance.TreasureRoomRelicSynchronizer);
			DisposeIfPossible(instance.RunLocationTargetedBuffer);
			DisposeIfPossible(instance.HoveredModelTracker);
			DisposeIfPossible(instance.InputSynchronizer);
			DisposeIfPossible(instance.MapSelectionSynchronizer);
			DisposeIfPossible(instance.ActChangeSynchronizer);
			DisposeIfPossible(instance.CombatStateSynchronizer);
			DisposeIfPossible(instance.RunLobby);
			SetRunLobby(instance, null);
		}
		finally
		{
			RunManagerCleaningUpProperty?.SetValue(instance, false);
			LocalContext.NetId = null;
			SetRunState(instance, null);
			stagedRunState = null;
		}
	}

	private static void DisposeIfPossible(object? value)
	{
		if (value == null)
		{
			return;
		}
		try
		{
			if (value is IDisposable disposable)
			{
				disposable.Dispose();
			}
			else
			{
				AccessTools.Method(value.GetType(), "Dispose", (Type[])null, (Type[])null)?.Invoke(value, Array.Empty<object>());
			}
		}
		catch (Exception ex)
		{
			ModEntry.Logger.Warn("STS2Plus failed to dispose " + value.GetType().Name + ": " + ex.Message, 1);
		}
	}
}
