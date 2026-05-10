using System;
using Godot;
using HarmonyLib;
using STS2Plus.Features;
using STS2Plus.Localization;
using STS2Plus.Patches;
using STS2Plus.Reflection;
using STS2Plus.Ui;

namespace STS2Plus.Config;

internal static class LiveSettings
{
	public static void ApplyAll()
	{
		ApplyMoreRules();
		ApplyRouteAdvisor();
		ApplyCompactRelicDrawer();
		ApplySpeedControl();
		ApplyPlayerCombatShield();
		ApplyEndlessDebugTools();
	}

	public static void ApplyMoreRules()
	{
		if (ConfigManager.Current.MoreRulesEnabled)
		{
			PlusLoc.MergeIntoModifiersTable();
			PlusState.SyncRuleSelectionsFromRunState();
		}
		else
		{
			PlusState.ClearGameplayRuleSelections();
		}
		EndlessModeOverlay.Refresh();
	}

	public static void ApplyRouteAdvisor()
	{
		Type type = RuntimeTypeResolver.FindType("MegaCrit.Sts2.Core.Nodes.Screens.Map.NMapScreen") ?? RuntimeTypeResolver.FindTypeByName("NMapScreen");
		if (type == null)
		{
			return;
		}
		Node val;
		try
		{
			object? obj = AccessTools.Property(type, "Instance")?.GetValue(null);
			val = (Node)((obj is Node) ? obj : null);
		}
		catch
		{
			return;
		}
		if (val != null && GodotObject.IsInstanceValid((GodotObject)(object)val))
		{
			if (ConfigManager.Current.RouteAdvisorEnabled)
			{
				RouteAdvisorHighlighter.Refresh(val);
			}
			else
			{
				RouteAdvisorHighlighter.Reset(val);
			}
		}
	}

	public static void ApplyCompactRelicDrawer()
	{
		CompactRelicDrawer.ApplyConfiguration();
	}

	public static void ApplySpeedControl()
	{
		if (ConfigManager.Current.SpeedControlEnabled)
		{
			SpeedControlOverlay.Show();
		}
		else
		{
			SpeedControlOverlay.Hide(resetSpeed: true);
		}
	}

	public static void ApplyPlayerCombatShield()
	{
		if (ConfigManager.Current.PlayerCombatShieldEnabled)
		{
			PlayerDamageTracker.Recalculate();
		}
		else
		{
			PlayerDamageTracker.Hide();
		}
	}

	public static void ApplyEndlessDebugTools()
	{
		if (!ConfigManager.Current.EnableEndlessDebugTools)
		{
			try
			{
				EndlessDebugOverlay.Hide();
			}
			catch (Exception ex)
			{
				ModEntry.Logger.Warn("STS2Plus debug HUD hide failed: " + ex.Message, 1);
			}
			ModEntry.Verbose("STS2Plus endless debug tools disabled.");
			return;
		}
		try
		{
			if (DebugToolsRuntime.IsSessionDisabled)
			{
				ModEntry.Logger.Warn("STS2Plus debug tools remain disabled for this session after a prior registration failure.", 1);
				return;
			}
			EndlessDebugOverlay.Show();
		}
		catch (Exception ex)
		{
			DebugToolsRuntime.DisableForSession("HUD setup failed", ex);
		}
	}
}
