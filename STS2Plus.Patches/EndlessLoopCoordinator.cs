using System;
using System.Threading.Tasks;
using Godot;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Multiplayer.Game;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Screens.GameOverScreen;
using MegaCrit.Sts2.Core.Runs;
using STS2Plus.Multiplayer;
using STS2Plus.Reflection;
using STS2Plus.Ui;

namespace STS2Plus.Patches;

internal static class EndlessLoopCoordinator
{
	private static readonly object SyncRoot = new object();

	private static INetGameService? attachedService;

	private static WeakReference<NGameOverScreen>? gameOverScreenRef;

	private static Task activeLaunchTask = Task.CompletedTask;

	private static long activeLaunchNonce;

	private static long lastHandledLaunchNonce;

	private static int lastHandledLoopIndex;

	private static bool isLaunching;

	private static bool postEndlessLoadCleanupActive;

	private static bool awaitingFirstCombatReset;

	public static bool IsLaunching => isLaunching;

	public static bool IsPostEndlessLoadCleanupActive => postEndlessLoadCleanupActive;

	public static void AttachCurrentRun()
	{
		INetGameService netService = RunManager.Instance.NetService;
		if ((int)netService.Type != 2 && (int)netService.Type != 3)
		{
			Detach();
			return;
		}
		if (attachedService == netService)
		{
			return;
		}
		Detach();
		attachedService = netService;
		netService.RegisterMessageHandler<EndlessLoopBeginMessage>((MessageHandlerDelegate<EndlessLoopBeginMessage>)OnEndlessLoopBegin);
	}

	public static void Detach()
	{
		if (attachedService != null)
		{
			attachedService.UnregisterMessageHandler<EndlessLoopBeginMessage>((MessageHandlerDelegate<EndlessLoopBeginMessage>)OnEndlessLoopBegin);
		}
		attachedService = null;
		gameOverScreenRef = null;
		activeLaunchTask = Task.CompletedTask;
		activeLaunchNonce = 0L;
		lastHandledLaunchNonce = 0L;
		lastHandledLoopIndex = 0;
		isLaunching = false;
		postEndlessLoadCleanupActive = false;
		awaitingFirstCombatReset = false;
		EndlessModeWinRunPatch.InWinTransition = false;
	}

	public static void RegisterGameOverScreen(NGameOverScreen screen)
	{
		gameOverScreenRef = new WeakReference<NGameOverScreen>(screen);
	}

	public static void ClearGameOverScreen(NGameOverScreen screen)
	{
		if (gameOverScreenRef != null && gameOverScreenRef.TryGetTarget(out var target) && target == screen)
		{
			gameOverScreenRef = null;
		}
	}

	public static Task StartFromWinRunAsync(RunManager runManager)
	{
		if (!PlusState.IsEndlessModeActive())
		{
			return Task.CompletedTask;
		}
		InferCurrentGameOverScreen();
		int nextLoopIndex = GameReflection.GetLoopCount() + 1;
		string nextSeed = BuildNextLoopSeed(runManager.DebugOnlyGetState(), nextLoopIndex);
		INetGameService? netService = attachedService;
		if (netService != null && (int)netService.Type == 3)
		{
			ModEntry.Logger.Info("STS2Plus LOOP PATH: MULTIPLAYER_CLIENT", 1);
			return WaitForHostLaunchOrFallbackAsync(nextLoopIndex, nextSeed);
		}
		ModEntry.Logger.Info("STS2Plus LOOP PATH: MULTIPLAYER_HOST", 1);
		return BeginLaunchAsync(nextLoopIndex, nextSeed, shouldBroadcast: netService != null && netService.IsConnected && (int)netService.Type == 2, launchNonce: CreateLaunchNonce());
	}

	public static Task BeginFromGameOverScreenAsync(NGameOverScreen screen)
	{
		RegisterGameOverScreen(screen);
		RunManager runManager = RunManager.Instance;
		int nextLoopIndex = GameReflection.GetLoopCount() + 1;
		string nextSeed = BuildNextLoopSeed(runManager.DebugOnlyGetState(), nextLoopIndex);
		INetGameService? netService = attachedService;
		return BeginLaunchAsync(nextLoopIndex, nextSeed, shouldBroadcast: netService != null && netService.IsConnected && (int)netService.Type == 2, launchNonce: CreateLaunchNonce());
	}

	public static Task StartDebugLoopAsync()
	{
		RunManager? runManager = RunManager.Instance;
		if (runManager == null)
		{
			ModEntry.Logger.Warn("STS2Plus endless debug: cannot trigger loop because RunManager.Instance is null.", 1);
			return Task.CompletedTask;
		}
		return StartFromWinRunAsync(runManager);
	}

	public static void PrepareDebugTransitionContext()
	{
		InferCurrentGameOverScreen();
		NGameOverScreen? screen = GetKnownGameOverScreen();
		EndlessModeWinRunPatch.InWinTransition = screen != null;
		ModEntry.Logger.Info("STS2Plus endless debug: prepared endless transition context screenPresent=" + (screen != null) + ".", 1);
	}

	private static async Task WaitForHostLaunchOrFallbackAsync(int nextLoopIndex, string nextSeed)
	{
		for (int i = 0; i < 12; i++)
		{
			Task? pendingTask = TryGetPendingLaunchForLoop(nextLoopIndex);
			if (pendingTask != null)
			{
				await pendingTask;
				return;
			}
			await Task.Delay(50);
		}
		ModEntry.Logger.Warn($"STS2Plus endless loop: client did not receive host launch for loopIndex={nextLoopIndex}, starting deterministic fallback.", 1);
		await BeginLaunchAsync(nextLoopIndex, nextSeed, shouldBroadcast: false, launchNonce: CreateLaunchNonce());
	}

	private static Task? TryGetPendingLaunchForLoop(int nextLoopIndex)
	{
		lock (SyncRoot)
		{
			if (isLaunching || lastHandledLoopIndex >= nextLoopIndex)
			{
				return activeLaunchTask;
			}
		}
		return null;
	}

	private static void OnEndlessLoopBegin(EndlessLoopBeginMessage message, ulong _)
	{
		ModEntry.Logger.Info("STS2Plus LOOP PATH: MULTIPLAYER_CLIENT", 1);
		ModEntry.Logger.Info($"STS2Plus endless loop: peer received begin nonce={message.LaunchToken} loopIndex={message.LoopIndex}.", 1);
		RunManager? runManager = RunManager.Instance;
		RunState? runState = runManager?.DebugOnlyGetState();
		if (runState == null)
		{
			ModEntry.Logger.Warn("STS2Plus endless loop: ignoring begin message because run state is unavailable.", 1);
			return;
		}
		string nextSeed = BuildNextLoopSeed(runState, message.LoopIndex);
		TaskHelper.RunSafely(BeginLaunchAsync(message.LoopIndex, nextSeed, shouldBroadcast: false, launchNonce: message.LaunchToken));
	}

	private static Task BeginLaunchAsync(int nextLoopIndex, string nextSeed, bool shouldBroadcast, long launchNonce)
	{
		lock (SyncRoot)
		{
			if (isLaunching)
			{
				if (activeLaunchNonce == launchNonce || lastHandledLoopIndex >= nextLoopIndex)
				{
					ModEntry.Logger.Info($"STS2Plus endless loop: duplicate launch ignored nonce={launchNonce} loopIndex={nextLoopIndex}.", 1);
					return activeLaunchTask;
				}
			}
			if (!shouldBroadcast && lastHandledLaunchNonce == launchNonce && launchNonce != 0L)
			{
				ModEntry.Logger.Info($"STS2Plus endless loop: begin message already handled nonce={launchNonce}.", 1);
				return activeLaunchTask;
			}
			if (lastHandledLoopIndex >= nextLoopIndex)
			{
				ModEntry.Logger.Info($"STS2Plus endless loop: loopIndex={nextLoopIndex} already handled locally.", 1);
				return activeLaunchTask;
			}
			isLaunching = true;
			activeLaunchNonce = launchNonce;
			activeLaunchTask = StartLocalLaunchCoreAsync(nextLoopIndex, nextSeed, shouldBroadcast, launchNonce);
			return activeLaunchTask;
		}
	}

	private static async Task StartLocalLaunchCoreAsync(int nextLoopIndex, string nextSeed, bool shouldBroadcast, long launchNonce)
	{
		try
		{
			postEndlessLoadCleanupActive = true;
			awaitingFirstCombatReset = true;
			if (shouldBroadcast)
			{
				INetGameService? netService = attachedService;
				if (netService != null && netService.IsConnected)
				{
					netService.SendMessage(new EndlessLoopBeginMessage
					{
						LaunchToken = launchNonce,
						LoopIndex = nextLoopIndex
					});
					ModEntry.Logger.Info($"STS2Plus endless loop: host broadcast begin nonce={launchNonce} loopIndex={nextLoopIndex}.", 1);
				}
			}
			await (GameReflection.TriggerEndlessLoop(GetKnownGameOverScreen(), nextSeed, nextLoopIndex) ?? Task.CompletedTask);
			ModEntry.Logger.Info($"STS2Plus endless loop: local transition completed loopIndex={nextLoopIndex}.", 1);
		}
		catch (Exception ex)
		{
			ModEntry.Logger.Error("STS2Plus endless loop transition failed: " + ex, 1);
			throw;
		}
		finally
		{
			lock (SyncRoot)
			{
				lastHandledLaunchNonce = launchNonce;
				lastHandledLoopIndex = Math.Max(lastHandledLoopIndex, nextLoopIndex);
				isLaunching = false;
			}
			EndlessModeWinRunPatch.InWinTransition = false;
			EndlessModeOverlay.Refresh();
			ModEntry.Logger.Info("STS2Plus endless loop: overlay refreshed.", 1);
		}
	}

	private static NGameOverScreen? GetKnownGameOverScreen()
	{
		if (gameOverScreenRef != null && gameOverScreenRef.TryGetTarget(out var screen) && GodotObject.IsInstanceValid(screen))
		{
			return screen;
		}
		return null;
	}

	private static void InferCurrentGameOverScreen()
	{
		NGameOverScreen? known = GetKnownGameOverScreen();
		if (known != null)
		{
			return;
		}
		Node? currentRunNode = NGame.Instance?.CurrentRunNode;
		if (currentRunNode == null)
		{
			return;
		}
		NGameOverScreen? found = currentRunNode.FindChild("NGameOverScreen", true, false) as NGameOverScreen;
		if (found != null)
		{
			RegisterGameOverScreen(found);
		}
	}

	private static string BuildNextLoopSeed(RunState runState, int nextLoopIndex)
	{
		string stringSeed = runState.Rng.StringSeed ?? "STS2PLUS";
		int markerIndex = stringSeed.LastIndexOf("_L", StringComparison.Ordinal);
		string baseSeed = (markerIndex < 0) ? stringSeed : stringSeed.Substring(0, markerIndex);
		return baseSeed + "_L" + nextLoopIndex;
	}

	private static long CreateLaunchNonce()
	{
		return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
	}

	internal static bool TryConsumeFirstCombatResetRequest()
	{
		if (!awaitingFirstCombatReset)
		{
			return false;
		}
		awaitingFirstCombatReset = false;
		return true;
	}

	internal static void CompletePostEndlessLoadCleanup(string source)
	{
		if (!postEndlessLoadCleanupActive)
		{
			return;
		}
		postEndlessLoadCleanupActive = false;
		ModEntry.Logger.Info("STS2Plus endless loop: completed post-load cleanup at " + source + ".", 1);
	}
}
