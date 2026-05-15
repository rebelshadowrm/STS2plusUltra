using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Screens.MainMenu;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using STS2Plus.Config;
using STS2Plus.Reflection;
using STS2Plus.Ui;

namespace STS2Plus.Patches;

internal static class EndlessDebugTools
{
	private static readonly PropertyInfo? RunManagerStateProperty = AccessTools.Property(typeof(RunManager), "State");

	private static readonly PropertyInfo? NGameCurrentScreenProperty = AccessTools.Property(typeof(NGame), "CurrentScreen");

	private static readonly PropertyInfo? EventModelIsFinishedProperty = AccessTools.Property(RuntimeTypeResolver.FindType("MegaCrit.Sts2.Core.Models.EventModel") ?? RuntimeTypeResolver.FindTypeByName("EventModel"), "IsFinished");

	private static readonly MethodInfo? KillWithoutCheckingWinConditionMethod = AccessTools.Method(typeof(CreatureCmd), "KillWithoutCheckingWinCondition", new Type[3]
	{
		typeof(Creature),
		typeof(bool),
		typeof(int)
	}, null);

	internal static void LogEndlessAndMultiplayerState()
	{
		RunManager? runManager = RunManager.Instance;
		RunState? runState = runManager?.DebugOnlyGetState() ?? (RunManagerStateProperty?.GetValue(runManager) as RunState);
		string seed = runState?.Rng.StringSeed ?? "<null>";
		string room = GameReflection.GetCurrentRoom()?.GetType().FullName ?? "<null>";
		string mapPoint = GameReflection.DescribeMapPoint(GameReflection.GetCurrentMapPoint());
		ModEntry.Logger.Info($"STS2Plus endless debug: state multiplayer={GameReflection.IsMultiplayerRun()} runActive={GameReflection.IsRunActive()} endlessActive={PlusState.IsEndlessModeActive()} loopIndex={GameReflection.GetLoopCount()} act={runState?.CurrentActIndex ?? -1} floor={runState?.ActFloor ?? -1} seed={seed} launching={EndlessLoopCoordinator.IsLaunching} postCleanup={EndlessLoopCoordinator.IsPostEndlessLoadCleanupActive} room={room} mapPoint={mapPoint}.", 1);
		EndlessLoopTransition.LogMapSelectionState(runManager, runState, GameReflection.GetLoopCount(), seed, "debug-hotkey F6");
	}

	internal static void LogMapSelectionState()
	{
		RunManager? runManager = RunManager.Instance;
		RunState? runState = runManager?.DebugOnlyGetState() ?? (RunManagerStateProperty?.GetValue(runManager) as RunState);
		string seed = runState?.Rng.StringSeed ?? "<null>";
		ModEntry.Logger.Info("STS2Plus endless debug: logging map-selection state from F10.", 1);
		EndlessLoopTransition.LogMapSelectionState(runManager, runState, GameReflection.GetLoopCount(), seed, "debug-hotkey F10");
	}

	internal static string PrepareEndlessTransitionContext()
	{
		ModEntry.Logger.Info("STS2Plus endless debug: F8 not implemented yet.", 1);
		return "F8 not implemented yet";
	}

	internal static async Task<string> TriggerRealEndlessLoopAsync()
	{
		RunManager? runManager = RunManager.Instance;
		RunState? runState = runManager?.DebugOnlyGetState() ?? (RunManagerStateProperty?.GetValue(runManager) as RunState);
		bool multiplayer = GameReflection.IsMultiplayerRun();
		bool authoritative = !multiplayer || MultiplayerSafety.ShouldApplyAuthoritativeGameplayPatches();
		string localRole = !multiplayer ? "singleplayer" : (authoritative ? "host/authoritative" : "client");
		string currentRoom = GameReflection.GetCurrentRoom()?.GetType().FullName ?? "<null>";
		string currentScreen = NGameCurrentScreenProperty?.GetValue(NGame.Instance)?.GetType().FullName ?? "<unavailable>";
		int act = runState?.CurrentActIndex ?? -1;
		int floor = runState?.ActFloor ?? -1;
		int loopIndex = GameReflection.GetLoopCount();
		string reason = ValidateEndlessTransitionRequest(runManager, runState, multiplayer, authoritative);
		ModEntry.Logger.Info($"STS2Plus endless debug: F9 precheck multiplayer={multiplayer} role={localRole} act={act} floor={floor} loopIndex={loopIndex} room={currentRoom} screen={currentScreen} allowed={string.IsNullOrEmpty(reason)}.", 1);
		if (!string.IsNullOrEmpty(reason))
		{
			ModEntry.Logger.Warn("STS2Plus endless debug: F9 ignored - " + reason, 1);
			return "F9 ignored: " + reason;
		}
		ModEntry.Logger.Info("STS2Plus LOOP PATH: DEBUG_F9", 1);
		ModEntry.Logger.Info("STS2Plus endless debug: triggering real endless loop transition from F9.", 1);
		EndlessDebugOverlay.SetLastAction("F9: endless transition started");
		if (!multiplayer)
		{
			await (GameReflection.TriggerLegacySingleplayerEndlessLoop(runManager, "DEBUG_F9") ?? Task.CompletedTask);
		}
		else
		{
			await EndlessLoopCoordinator.StartDebugLoopAsync();
		}
		return "F9: endless transition complete";
	}

	private static string ValidateEndlessTransitionRequest(RunManager? runManager, RunState? runState, bool multiplayer, bool authoritative)
	{
		if (runManager == null || runState == null)
		{
			return "run state unavailable";
		}
		if (!GameReflection.IsRunActive())
		{
			return "no active run";
		}
		if (!PlusState.IsEndlessModeActive())
		{
			return "endless mode is not active";
		}
		if (EndlessLoopCoordinator.IsLaunching)
		{
			return "endless transition already in progress";
		}
		if (multiplayer && !authoritative)
		{
			return "multiplayer client cannot start host-only transition";
		}
		if (CombatManager.Instance?.IsInProgress == true)
		{
			return "combat is still in progress";
		}
		if (TryDescribeUnfinishedCurrentEvent(runState, out string unfinishedEvent))
		{
			return "unfinished event still active: " + unfinishedEvent;
		}
		return string.Empty;
	}

	private static bool TryDescribeUnfinishedCurrentEvent(RunState runState, out string description)
	{
		description = string.Empty;
		try
		{
			object? currentRoom = GameReflection.GetCurrentRoom();
			if (currentRoom == null)
			{
				return false;
			}
			object? eventModel = AccessTools.Property(currentRoom.GetType(), "CanonicalEvent")?.GetValue(currentRoom) ?? AccessTools.Property(currentRoom.GetType(), "Event")?.GetValue(currentRoom) ?? AccessTools.Property(currentRoom.GetType(), "Model")?.GetValue(currentRoom) ?? AccessTools.Field(currentRoom.GetType(), "CanonicalEvent")?.GetValue(currentRoom) ?? AccessTools.Field(currentRoom.GetType(), "Event")?.GetValue(currentRoom) ?? AccessTools.Field(currentRoom.GetType(), "Model")?.GetValue(currentRoom);
			if (eventModel == null)
			{
				return false;
			}
			bool isFinished = (EventModelIsFinishedProperty?.GetValue(eventModel) as bool?).GetValueOrDefault();
			if (isFinished)
			{
				return false;
			}
			string eventId = AccessTools.Property(eventModel.GetType(), "Id")?.GetValue(eventModel)?.ToString() ?? AccessTools.Property(eventModel.GetType(), "EventId")?.GetValue(eventModel)?.ToString() ?? eventModel.GetType().Name;
			description = eventId;
			return true;
		}
		catch (Exception ex)
		{
			ModEntry.Logger.Warn("STS2Plus endless debug: failed to inspect current event before F9 - " + ex.GetType().Name + ": " + ex.Message, 1);
			return false;
		}
	}

	internal static async Task<string> ForceCurrentRoomOrCombatWinAsync()
	{
		if (GameReflection.IsMultiplayerRun())
		{
			ModEntry.Logger.Warn("STS2Plus endless debug: F7 ignored because multiplayer force-win is unsafe and would desync.", 1);
			return "F7 ignored: multiplayer force-win is unsafe / would desync";
		}
		object? currentRoom = GameReflection.GetCurrentRoom();
		string roomType = currentRoom?.GetType().FullName ?? "<null>";
		CombatRoom? combatRoom = currentRoom as CombatRoom;
		CombatManager? combatManager = CombatManager.Instance;
		CombatState? combatState = combatRoom?.CombatState;
		if (combatRoom == null || combatManager == null || combatState == null || !combatState.IsLiveCombat())
		{
			ModEntry.Logger.Info($"STS2Plus endless debug: F7 ignored roomType={roomType} combatStateFound={combatState != null}.", 1);
			return "F7 ignored: not in combat";
		}
		List<Creature> liveEnemies = new List<Creature>();
		foreach (Creature enemy in combatState.Enemies)
		{
			if (enemy != null && enemy.IsAlive)
			{
				liveEnemies.Add(enemy);
			}
		}
		ModEntry.Logger.Info($"STS2Plus endless debug: F7 roomType={roomType} combatStateFound=True enemyCount={liveEnemies.Count} chosenMethod=KillWithoutCheckingWinCondition+CheckWinCondition.", 1);
		foreach (Creature liveEnemy in liveEnemies)
		{
			await InvokeKillWithoutCheckingWinConditionAsync(liveEnemy);
		}
		bool resolved = await combatManager.CheckWinCondition();
		ModEntry.Logger.Info("STS2Plus endless debug: F7 win resolution result=" + resolved + ".", 1);
		if (!resolved)
		{
			return (liveEnemies.Count == 0) ? "F7 failed: no live enemies and win did not resolve" : "F7 failed: win condition did not resolve";
		}
		return "F7: combat won";
	}

	private static async Task InvokeKillWithoutCheckingWinConditionAsync(Creature enemy)
	{
		if (KillWithoutCheckingWinConditionMethod == null)
		{
			throw new MissingMethodException("CreatureCmd.KillWithoutCheckingWinCondition");
		}
		object? result = KillWithoutCheckingWinConditionMethod.Invoke(null, new object[3] { enemy, true, 0 });
		if (result is Task task)
		{
			await task;
		}
	}

	internal static string DescribeSessionState()
	{
		if (!ConfigManager.Current.EnableEndlessDebugTools)
		{
			return "tools disabled";
		}
		return DebugToolsRuntime.IsSessionDisabled ? "session disabled" : "enabled";
	}

	internal static void UpdateHudStatus(string message)
	{
		EndlessDebugOverlay.SetLastAction(message);
	}

	internal static string GetHotkeyDisabledReason()
	{
		if (!ConfigManager.Current.EnableEndlessDebugTools)
		{
			return "tools disabled";
		}
		return DebugToolsRuntime.IsSessionDisabled ? "session disabled" : "tools disabled";
	}

	internal static async Task ExecuteHotkeyAsync(string hotkeyName)
	{
		try
		{
			ModEntry.Logger.Info("Debug hotkey received: " + hotkeyName, 1);
			if (!DebugToolsRuntime.IsEnabledForSession)
			{
				string ignoredReason = GetHotkeyDisabledReason();
				ModEntry.Logger.Info("Debug hotkey ignored: " + ignoredReason, 1);
				UpdateHudStatus(hotkeyName + " ignored: " + ignoredReason);
				return;
			}
			ModEntry.Logger.Info("Debug hotkey executing: " + hotkeyName, 1);
			string result = hotkeyName switch
			{
				"F6" => ExecuteDumpState(),
				"F7" => await ForceCurrentRoomOrCombatWinAsync(),
				"F8" => PrepareEndlessTransitionContext(),
				"F9" => await TriggerRealEndlessLoopAsync(),
				"F10" => ExecuteDumpMapState(),
				"Shift+F10" => ExecuteToggleHud(),
				_ => hotkeyName + " ignored: unknown hotkey"
			};
			UpdateHudStatus(result);
		}
		catch (Exception ex)
		{
			string shortReason = ex.GetType().Name + ": " + ex.Message;
			ModEntry.Logger.Error("STS2Plus endless debug: hotkey failed " + hotkeyName + " -> " + ex, 1);
			UpdateHudStatus(hotkeyName + " failed: " + shortReason);
		}
	}

	private static string ExecuteDumpState()
	{
		LogEndlessAndMultiplayerState();
		return "F6: state dumped to log";
	}

	private static string ExecuteDumpMapState()
	{
		LogMapSelectionState();
		return "F10: map state dumped to log";
	}

	private static string ExecuteToggleHud()
	{
		EndlessDebugOverlay.ToggleCollapsed();
		return "Shift+F10: HUD toggled";
	}
}

[HarmonyPatchCategory("DebugTools")]
[HarmonyPatch(typeof(NGame), "_Input")]
internal static class EndlessDebugInputPatch
{
	private static void Prefix(InputEvent inputEvent)
	{
		try
		{
			InputEventKey keyEvent = (InputEventKey)(object)((inputEvent is InputEventKey) ? inputEvent : null);
			if (keyEvent == null || !keyEvent.Pressed || keyEvent.Echo)
			{
				return;
			}
			Key key = (keyEvent.PhysicalKeycode != Key.None) ? keyEvent.PhysicalKeycode : keyEvent.Keycode;
			string? hotkeyName = GetDebugHotkeyName(key, keyEvent.ShiftPressed);
			if (hotkeyName == null)
			{
				return;
			}
			TaskHelper.RunSafely(EndlessDebugTools.ExecuteHotkeyAsync(hotkeyName));
		}
		catch (Exception ex)
		{
			DebugToolsRuntime.DisableForSession("debug hotkey processing failed", ex);
		}
	}

	private static string? GetDebugHotkeyName(Key key, bool shiftPressed)
	{
		return key switch
		{
			Key.F6 => "F6",
			Key.F7 => "F7",
			Key.F8 => "F8",
			Key.F9 => "F9",
			Key.F10 => (shiftPressed ? "Shift+F10" : "F10"),
			_ => null
		};
	}
}

[HarmonyPatchCategory("DebugTools")]
[HarmonyPatch(typeof(NGame), "_Ready")]
internal static class EndlessDebugBootstrapPatch
{
	private static void Postfix()
	{
		if (!DebugToolsRuntime.IsEnabledForSession)
		{
			return;
		}
		try
		{
			EndlessDebugOverlay.Show();
		}
		catch (Exception ex)
		{
			DebugToolsRuntime.DisableForSession("debug HUD bootstrap failed", ex);
		}
	}
}

[HarmonyPatchCategory("DebugTools")]
[HarmonyPatch(typeof(NMainMenu), "_Ready")]
internal static class EndlessDebugMainMenuPatch
{
	private static void Postfix()
	{
		try
		{
			EndlessDebugOverlay.Refresh("main-menu-ready");
		}
		catch (Exception ex)
		{
			DebugToolsRuntime.DisableForSession("debug HUD main-menu refresh failed", ex);
		}
	}
}

[HarmonyPatchCategory("DebugTools")]
[HarmonyPatch(typeof(NCombatRoom), "OnCombatSetUp")]
internal static class EndlessDebugCombatSetupPatch
{
	private static void Postfix()
	{
		try
		{
			EndlessDebugOverlay.Refresh("combat-setup");
		}
		catch (Exception ex)
		{
			DebugToolsRuntime.DisableForSession("debug HUD combat refresh failed", ex);
		}
	}
}

[HarmonyPatchCategory("DebugTools")]
[HarmonyPatch]
internal static class EndlessDebugRunScreenRefreshPatch
{
	[CompilerGenerated]
	private sealed class _003CTargetMethods_003Ed__0 : IEnumerable<MethodBase>, IEnumerable, IEnumerator<MethodBase>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private MethodBase _003C_003E2__current;

		private int _003C_003El__initialThreadId;

		private Type _003CmapScreenType_003E5__1;

		private Type _003CrunType_003E5__2;

		private MethodInfo _003Cready_003E5__3;

		private MethodInfo _003Cready_003E5__4;

		MethodBase IEnumerator<MethodBase>.Current
		{
			[DebuggerHidden]
			get
			{
				return _003C_003E2__current;
			}
		}

		object IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return _003C_003E2__current;
			}
		}

		[DebuggerHidden]
		public _003CTargetMethods_003Ed__0(int _003C_003E1__state)
		{
			this._003C_003E1__state = _003C_003E1__state;
			_003C_003El__initialThreadId = System.Environment.CurrentManagedThreadId;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
			_003CmapScreenType_003E5__1 = null;
			_003CrunType_003E5__2 = null;
			_003Cready_003E5__3 = null;
			_003Cready_003E5__4 = null;
			_003C_003E1__state = -2;
		}

		private bool MoveNext()
		{
			switch (_003C_003E1__state)
			{
			default:
				return false;
			case 0:
				_003C_003E1__state = -1;
				_003CmapScreenType_003E5__1 = RuntimeTypeResolver.FindType("MegaCrit.Sts2.Core.Nodes.Screens.Map.NMapScreen") ?? RuntimeTypeResolver.FindTypeByName("NMapScreen");
				if (_003CmapScreenType_003E5__1 != null)
				{
					_003Cready_003E5__3 = AccessTools.Method(_003CmapScreenType_003E5__1, "_Ready", (Type[])null, (Type[])null);
					if (_003Cready_003E5__3 != null)
					{
						_003C_003E2__current = _003Cready_003E5__3;
						_003C_003E1__state = 1;
						return true;
					}
					goto IL_00a3;
				}
				goto IL_00ab;
			case 1:
				_003C_003E1__state = -1;
				goto IL_00a3;
			case 2:
				_003C_003E1__state = -1;
				goto IL_0122;
			}
			goto IL_00ab;
			IL_00a3:
			_003Cready_003E5__3 = null;
			IL_00ab:
			_003CrunType_003E5__2 = RuntimeTypeResolver.FindType("MegaCrit.Sts2.Core.Nodes.NRun") ?? RuntimeTypeResolver.FindTypeByName("NRun");
			if (!(_003CrunType_003E5__2 != null))
			{
				return false;
			}
			_003Cready_003E5__4 = AccessTools.Method(_003CrunType_003E5__2, "_Ready", (Type[])null, (Type[])null);
			if (_003Cready_003E5__4 != null)
			{
				_003C_003E2__current = _003Cready_003E5__4;
				_003C_003E1__state = 2;
				return true;
			}
			IL_0122:
			_003Cready_003E5__4 = null;
			return false;
		}

		bool IEnumerator.MoveNext()
		{
			return MoveNext();
		}

		[DebuggerHidden]
		void IEnumerator.Reset()
		{
			throw new NotSupportedException();
		}

		[DebuggerHidden]
		IEnumerator<MethodBase> IEnumerable<MethodBase>.GetEnumerator()
		{
			if (_003C_003E1__state == -2 && _003C_003El__initialThreadId == System.Environment.CurrentManagedThreadId)
			{
				_003C_003E1__state = 0;
				return this;
			}
			return new _003CTargetMethods_003Ed__0(0);
		}

		[DebuggerHidden]
		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<MethodBase>)this).GetEnumerator();
		}
	}

	[IteratorStateMachine(typeof(_003CTargetMethods_003Ed__0))]
	[HarmonyTargetMethods]
	private static IEnumerable<MethodBase> TargetMethods()
	{
		return new _003CTargetMethods_003Ed__0(-2);
	}

	private static void Postfix(MethodBase __originalMethod)
	{
		try
		{
			EndlessDebugOverlay.Refresh(__originalMethod.Name + "-ready");
		}
		catch (Exception ex)
		{
			DebugToolsRuntime.DisableForSession("debug HUD run-screen refresh failed", ex);
		}
	}
}
