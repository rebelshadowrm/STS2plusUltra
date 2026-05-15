using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Runs;
using STS2Plus.Reflection;

namespace STS2Plus.Patches;

[HarmonyPatchCategory("MoreRules")]
[HarmonyPatch]
internal static class EndlessModeRewardsScreenPatch
{
	private static readonly HashSet<nint> _loggedRewardsScreens = new HashSet<nint>();

	[HarmonyTargetMethod]
	private static MethodBase? TargetMethod()
	{
		Type? type = RuntimeTypeResolver.FindType("MegaCrit.Sts2.Core.Runs.RunManager") ?? RuntimeTypeResolver.FindTypeByName("RunManager");
		return type != null ? AccessTools.Method(type, "ProceedFromTerminalRewardsScreen", (Type[])null, (Type[])null) : null;
	}

	private static bool Prefix(object __instance, ref Task? __result)
	{
		if (__instance is not RunManager runManager)
		{
			return true;
		}
		if (!TryDescribeFinalBossRewardsTransition(runManager, out var currentRoomType, out var isVictoryRoom, out var currentActIndex, out var actsCount, out var currentEncounterId))
		{
			return true;
		}
		ModEntry.Logger.Info($"EndlessFinalBossProceedSuppress: finished-combat boss room detected encounter={currentEncounterId} currentActIndex={currentActIndex} actsCount={actsCount} currentRoom={currentRoomType} isVictoryRoom={isVictoryRoom}.", 1);
		if (EndlessLoopCoordinator.IsLaunching)
		{
			ModEntry.Logger.Info("EndlessFinalBossProceedSuppress: ProceedFromTerminalRewardsScreen ignored because endless transition is already launching.", 1);
			__result = Task.CompletedTask;
			return false;
		}
		ModEntry.Logger.Info("EndlessFinalBossProceedSuppress: auto-starting endless transition.", 1);
		__result = EndlessModePreArchitectPatch.StartBypassTransition(runManager, isArchitectEvent: false, fromUi: true) ?? Task.CompletedTask;
		return false;
	}

	internal static bool TryDescribeFinalBossRewardsTransition(RunManager runManager, out string currentRoomType, out bool isVictoryRoom, out int currentActIndex, out int actsCount, out string currentEncounterId)
	{
		currentRoomType = "<null>";
		isVictoryRoom = false;
		currentActIndex = -1;
		actsCount = -1;
		currentEncounterId = "<none>";
		if (!EndlessModePreArchitectPatch.TryDescribeCurrentTransitionContext(runManager, out var state, out var currentRoom, out _, out _, out currentRoomType, out isVictoryRoom, out currentActIndex, out actsCount, out currentEncounterId))
		{
			return false;
		}
		if (state == null || currentRoom == null)
		{
			return false;
		}
		if (!string.Equals(currentRoomType, "CombatRoom", StringComparison.Ordinal) && !currentRoomType.EndsWith("CombatRoom", StringComparison.Ordinal))
		{
			return false;
		}
		if (!currentEncounterId.EndsWith("_BOSS", StringComparison.OrdinalIgnoreCase))
		{
			return false;
		}
		return true;
	}

	internal static (bool Found, bool Visible, bool Enabled, string Label) DescribeProceedButton(object rewardsScreen)
	{
		try
		{
			GodotObject? proceedButton = FindProceedButton(rewardsScreen);
			if (proceedButton == null)
			{
				return (false, false, false, "<none>");
			}
			return (true, ReadVisible(proceedButton), ReadEnabled(proceedButton), ReadLabel(proceedButton));
		}
		catch (Exception ex)
		{
			ModEntry.Logger.Warn("EndlessFinalBossProceedSuppress: failed to describe rewards proceed button - " + ex.GetType().Name + ": " + ex.Message, 1);
			return (false, false, false, "<error>");
		}
	}

	internal static void TryHideProceedButton(object rewardsScreen)
	{
		try
		{
			GodotObject? proceedButton = FindProceedButton(rewardsScreen);
			if (proceedButton == null)
			{
				return;
			}
			if (proceedButton is CanvasItem canvasItem)
			{
				canvasItem.Visible = false;
			}
			AccessTools.Method(proceedButton.GetType(), "Disable")?.Invoke(proceedButton, null);
		}
		catch (Exception ex)
		{
			ModEntry.Logger.Warn("EndlessFinalBossProceedSuppress: failed to hide rewards proceed button - " + ex.GetType().Name + ": " + ex.Message, 1);
		}
	}

	internal static void TryRelabelProceedButton(object rewardsScreen, string label)
	{
		try
		{
			GodotObject? proceedButton = FindProceedButton(rewardsScreen);
			if (proceedButton == null)
			{
				return;
			}
			PropertyInfo? textProperty = AccessTools.Property(proceedButton.GetType(), "Text");
			if (textProperty != null && textProperty.CanWrite)
			{
				textProperty.SetValue(proceedButton, label);
				return;
			}
			MethodInfo? setTextAutoSize = AccessTools.Method(proceedButton.GetType(), "SetTextAutoSize", new[] { typeof(string) });
			if (setTextAutoSize != null)
			{
				setTextAutoSize.Invoke(proceedButton, new object[] { label });
				return;
			}
		}
		catch (Exception ex)
		{
			ModEntry.Logger.Warn("EndlessFinalBossProceedSuppress: failed to relabel rewards proceed button - " + ex.GetType().Name + ": " + ex.Message, 1);
		}
	}

	internal static GodotObject? FindProceedButton(object rewardsScreen)
	{
		if (rewardsScreen is not Node root)
		{
			return null;
		}
		return FindProceedButtonRecursive(root);
	}

	private static GodotObject? FindProceedButtonRecursive(Node node)
	{
		string nodeName = node.Name.ToString();
		string typeName = node.GetType().Name;
		if (nodeName.Contains("ProceedButton", StringComparison.OrdinalIgnoreCase) || typeName.Contains("ProceedButton", StringComparison.OrdinalIgnoreCase))
		{
			return node;
		}
		foreach (Node child in node.GetChildren())
		{
			GodotObject? found = FindProceedButtonRecursive(child);
			if (found != null)
			{
				return found;
			}
		}
		return null;
	}

	private static bool ReadVisible(GodotObject proceedButton)
	{
		if (proceedButton is CanvasItem canvasItem)
		{
			return canvasItem.Visible;
		}
		return (AccessTools.Property(proceedButton.GetType(), "Visible")?.GetValue(proceedButton) as bool?).GetValueOrDefault();
	}

	private static bool ReadEnabled(GodotObject proceedButton)
	{
		object? disabled = AccessTools.Property(proceedButton.GetType(), "Disabled")?.GetValue(proceedButton) ?? AccessTools.Field(proceedButton.GetType(), "Disabled")?.GetValue(proceedButton);
		if (disabled is bool disabledBool)
		{
			return !disabledBool;
		}
		MethodInfo? isDisabled = AccessTools.Method(proceedButton.GetType(), "IsDisabled");
		if (isDisabled != null && isDisabled.Invoke(proceedButton, null) is bool isDisabledBool)
		{
			return !isDisabledBool;
		}
		return true;
	}

	private static string ReadLabel(GodotObject proceedButton)
	{
		return AccessTools.Property(proceedButton.GetType(), "Text")?.GetValue(proceedButton)?.ToString()
			?? AccessTools.Field(proceedButton.GetType(), "Text")?.GetValue(proceedButton)?.ToString()
			?? proceedButton.GetType().Name;
	}

	private static bool ShouldLogForScreen(object rewardsScreen)
	{
		if (rewardsScreen is not GodotObject godotObject)
		{
			return false;
		}
		nint id = (nint)(long)godotObject.GetInstanceId();
		return _loggedRewardsScreens.Add(id);
	}

	[HarmonyPatch]
	internal static class EndlessRewardsScreenUiPatch
	{
		[HarmonyTargetMethod]
		private static MethodBase? TargetMethod()
		{
			Type? type = RuntimeTypeResolver.FindTypeByName("NRewardsScreen");
			return type != null ? AccessTools.Method(type, "_Process", (Type[])null, (Type[])null) : null;
		}

		private static void Postfix(object __instance)
		{
			try
			{
				RunManager? runManager = RunManager.Instance;
				if (runManager == null)
				{
					return;
				}
				if (!TryDescribeFinalBossRewardsTransition(runManager, out var currentRoomType, out var isVictoryRoom, out var currentActIndex, out var actsCount, out var currentEncounterId))
				{
					return;
				}
				(bool proceedFound, bool visible, bool enabled, string buttonLabel) = DescribeProceedButton(__instance);
				if (ShouldLogForScreen(__instance))
				{
					ModEntry.Logger.Info($"EndlessFinalBossProceedSuppress: rewardState=terminal proceedButtonFound={proceedFound} visible={visible} enabled={enabled} label={buttonLabel} encounter={currentEncounterId} currentActIndex={currentActIndex} actsCount={actsCount} currentRoom={currentRoomType} isVictoryRoom={isVictoryRoom}.", 1);
				}
				if (!proceedFound)
				{
					return;
				}
				TryRelabelProceedButton(__instance, GameReflection.IsMultiplayerRun() ? "Continue" : "Next Act");
				if (GameReflection.IsMultiplayerRun())
				{
					return;
				}
				if (!visible || !enabled || EndlessLoopCoordinator.IsLaunching)
				{
					return;
				}
				TryHideProceedButton(__instance);
				ModEntry.Logger.Info("EndlessFinalBossProceedSuppress: hiding/suppressing Architect proceed button.", 1);
				ModEntry.Logger.Info("EndlessFinalBossProceedSuppress: auto-starting endless transition.", 1);
				ModEntry.Logger.Info("STS2Plus LOOP PATH: NATURAL_SINGLEPLAYER_PRE_ARCHITECT_UI", 1);
				TaskHelper.RunSafely(EndlessModePreArchitectPatch.StartBypassTransition(runManager, isArchitectEvent: false, fromUi: true) ?? Task.CompletedTask);
			}
			catch (Exception ex)
			{
				ModEntry.Logger.Warn("EndlessFinalBossProceedSuppress: failed during NRewardsScreen._Process - " + ex.GetType().Name + ": " + ex.Message, 1);
			}
		}
	}
}
