using System;
using System.Reflection;
using Godot;
using HarmonyLib;

namespace STS2Plus.Ui;

internal static class MoreRulesUi
{
	public const string AttackDefenseRowName = "STS2PlusAttackDefenseRow";

	public const string AttackDefensePlusRowName = "STS2PlusAttackDefensePlusRow";

	public const string IronSkinRowName = "STS2PlusIronSkinRow";

	public const string GiantCreaturesRowName = "STS2PlusGiantCreaturesRow";

	public const string HardElitesRowName = "STS2PlusHardElitesRow";

	public const string EndlessModeRowName = "STS2PlusEndlessModeRow";

	public const string GlassCannonRowName = "STS2PlusGlassCannonRow";

	public const string UnlimitedGrowthRowName = "STS2PlusUnlimitedGrowthRow";

	public const string SandboxRowName = "STS2PlusSandboxRow";

	public const string BuildCreatorRowName = "STS2PlusBuildCreatorRow";

	public static VBoxContainer? FindContentContainer(Control modifiersList)
	{
		Node? obj = FindNodeByName((Node)(object)modifiersList, "Content");
		return (VBoxContainer?)(object)((obj is VBoxContainer) ? obj : null);
	}

	public static Control? FindRuleRow(Control root, string rowName)
	{
		Node? obj = FindNodeByName((Node)(object)root, rowName);
		return (Control?)(object)((obj is Control) ? obj : null);
	}

	public static Control? CreateRuleRow(VBoxContainer contentContainer, string rowName, string title, string description)
	{
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Expected O, but got Unknown
		Control val = FindTemplateRow((Control)(object)contentContainer);
		if (val == null)
		{
			return null;
		}
		Control row = default(Control);
		ref Control reference = ref row;
		Node obj = ((Node)val).Duplicate(15);
		reference = (Control)(object)((obj is Control) ? obj : null);
		if (row == null)
		{
			return null;
		}
		((Node)row).Name = (StringName)(rowName);
		row.FocusMode = (Control.FocusModeEnum)0;
		row.GuiInput += (Control.GuiInputEventHandler)delegate(InputEvent @event)
		{
			OnRowGuiInput(row, @event);
		};
		ConfigurePassiveChildren(row);
		Control val2 = FindLabelNode(row);
		if (val2 != null)
		{
			SetText(val2, "[color=#4da3ff]" + title + "[/color] " + description);
		}
		return row;
	}

	public static void Refresh(Control root)
	{
		bool interactionLocked = MultiplayerSafety.IsGameplayRuleSelectionLocked((Node?)(object)root);
		RefreshRow(root, "STS2PlusAttackDefenseRow", PlusState.AttackDefenseRuleSelected, interactionLocked);
		RefreshRow(root, "STS2PlusAttackDefensePlusRow", PlusState.AttackDefensePlusRuleSelected, interactionLocked);
		RefreshRow(root, "STS2PlusIronSkinRow", PlusState.IronSkinRuleSelected, interactionLocked);
		RefreshRow(root, "STS2PlusGiantCreaturesRow", PlusState.GiantCreaturesRuleSelected, interactionLocked);
		RefreshRow(root, "STS2PlusHardElitesRow", PlusState.HardElitesRuleSelected, interactionLocked);
		RefreshRow(root, "STS2PlusEndlessModeRow", PlusState.EndlessModeSelected, interactionLocked);
		RefreshRow(root, "STS2PlusGlassCannonRow", PlusState.GlassCannonRuleSelected, interactionLocked);
		RefreshRow(root, "STS2PlusUnlimitedGrowthRow", PlusState.UnlimitedGrowthRuleSelected, interactionLocked);
		RefreshRow(root, "STS2PlusSandboxRow", PlusState.SandboxRuleSelected, interactionLocked);
		RefreshRow(root, "STS2PlusBuildCreatorRow", PlusState.BuildCreatorRuleSelected, interactionLocked);
	}

	private static void RefreshRow(Control root, string rowName, bool enabled, bool interactionLocked)
	{
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		Control val = (Control)((((Node)root).Name == (StringName)(rowName)) ? ((object)root) : ((object)FindRuleRow(root, rowName)));
		if (val != null)
		{
			Node val2 = FindTickboxNode(val);
			if (val2 != null)
			{
				SetBoolProperty(val2, "IsTicked", enabled);
				SetMouseFilter(val2, (Control.MouseFilterEnum)2);
			}
			ApplyInteractionState(val, !interactionLocked);
			((CanvasItem)val).Modulate = (Color)(interactionLocked ? new Color(1f, 1f, 1f, 0.65f) : Colors.White);
		}
	}

	private static void ApplyVisualState(Control row)
	{
		bool enabled = false;
		if (((Node)row).Name == (StringName)"STS2PlusAttackDefenseRow")
		{
			enabled = PlusState.AttackDefenseRuleSelected;
		}
		else if (((Node)row).Name == (StringName)"STS2PlusAttackDefensePlusRow")
		{
			enabled = PlusState.AttackDefensePlusRuleSelected;
		}
		else if (((Node)row).Name == (StringName)"STS2PlusIronSkinRow")
		{
			enabled = PlusState.IronSkinRuleSelected;
		}
		else if (((Node)row).Name == (StringName)"STS2PlusGiantCreaturesRow")
		{
			enabled = PlusState.GiantCreaturesRuleSelected;
		}
		else if (((Node)row).Name == (StringName)"STS2PlusHardElitesRow")
		{
			enabled = PlusState.HardElitesRuleSelected;
		}
		else if (((Node)row).Name == (StringName)"STS2PlusEndlessModeRow")
		{
			enabled = PlusState.EndlessModeSelected;
		}
		else if (((Node)row).Name == (StringName)"STS2PlusGlassCannonRow")
		{
			enabled = PlusState.GlassCannonRuleSelected;
		}
		else if (((Node)row).Name == (StringName)"STS2PlusUnlimitedGrowthRow")
		{
			enabled = PlusState.UnlimitedGrowthRuleSelected;
		}
		else if (((Node)row).Name == (StringName)"STS2PlusSandboxRow")
		{
			enabled = PlusState.SandboxRuleSelected;
		}
		else if (((Node)row).Name == (StringName)"STS2PlusBuildCreatorRow")
		{
			enabled = PlusState.BuildCreatorRuleSelected;
		}
		RefreshRow(row, (StringName)(((Node)row).Name), enabled, MultiplayerSafety.IsGameplayRuleSelectionLocked((Node?)(object)row));
	}

	private static void OnRowGuiInput(Control row, InputEvent @event)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Invalid comparison between Unknown and I8
		InputEventMouseButton val = (InputEventMouseButton)(object)((@event is InputEventMouseButton) ? @event : null);
		if (val == null || (long)val.ButtonIndex != 1 || !val.Pressed)
		{
			return;
		}
		if (MultiplayerSafety.IsGameplayRuleSelectionLocked((Node?)(object)row))
		{
			row.AcceptEvent();
			return;
		}
		if (((Node)row).Name == (StringName)"STS2PlusAttackDefenseRow")
		{
			PlusState.SetAttackDefenseRule(!PlusState.AttackDefenseRuleSelected);
		}
		else if (((Node)row).Name == (StringName)"STS2PlusAttackDefensePlusRow")
		{
			PlusState.SetAttackDefensePlusRule(!PlusState.AttackDefensePlusRuleSelected);
		}
		else if (((Node)row).Name == (StringName)"STS2PlusIronSkinRow")
		{
			PlusState.SetIronSkinRule(!PlusState.IronSkinRuleSelected);
		}
		else if (((Node)row).Name == (StringName)"STS2PlusGiantCreaturesRow")
		{
			PlusState.SetGiantCreaturesRule(!PlusState.GiantCreaturesRuleSelected);
		}
		else if (((Node)row).Name == (StringName)"STS2PlusHardElitesRow")
		{
			PlusState.SetHardElitesRule(!PlusState.HardElitesRuleSelected);
		}
		else if (((Node)row).Name == (StringName)"STS2PlusEndlessModeRow")
		{
			PlusState.SetEndlessModeRule(!PlusState.EndlessModeSelected);
		}
		else if (((Node)row).Name == (StringName)"STS2PlusGlassCannonRow")
		{
			PlusState.SetGlassCannonRule(!PlusState.GlassCannonRuleSelected);
		}
		else if (((Node)row).Name == (StringName)"STS2PlusUnlimitedGrowthRow")
		{
			PlusState.SetUnlimitedGrowthRule(!PlusState.UnlimitedGrowthRuleSelected);
		}
		else if (((Node)row).Name == (StringName)"STS2PlusSandboxRow")
		{
			PlusState.SetSandboxRule(!PlusState.SandboxRuleSelected);
		}
		else if (((Node)row).Name == (StringName)"STS2PlusBuildCreatorRow")
		{
			PlusState.SetBuildCreatorRule(!PlusState.BuildCreatorRuleSelected);
		}
		NotifyModifiersChanged((Node)(object)row);
		ApplyVisualState(row);
		row.AcceptEvent();
	}

	private static void NotifyModifiersChanged(Node row)
	{
		Node parent = row.GetParent();
		while (parent != null && !((object)parent).GetType().Name.Contains("NCustomRunModifiersList"))
		{
			parent = parent.GetParent();
		}
		if (parent != null)
		{
			AccessTools.Method(((object)parent).GetType(), "EmitSignalModifiersChanged", (Type[])null, (Type[])null)?.Invoke(parent, Array.Empty<object>());
		}
	}

	private static void ApplyInteractionState(Control root, bool enabled)
	{
		root.MouseFilter = (Control.MouseFilterEnum)(enabled ? 0 : 2);
		foreach (Node child in ((Node)root).GetChildren(false))
		{
			if (child == null)
			{
				continue;
			}
			Node val = child;
			if (true)
			{
				Control val2 = (Control)(object)((val is Control) ? val : null);
				if (val2 != null)
				{
					val2.MouseFilter = (Control.MouseFilterEnum)2;
					val2.FocusMode = (Control.FocusModeEnum)0;
				}
				ApplyInteractionStateRecursive(val, enabled);
			}
		}
	}

	private static void ApplyInteractionStateRecursive(Node root, bool enabled)
	{
		foreach (Node child in root.GetChildren(false))
		{
			if (child == null)
			{
				continue;
			}
			Node val = child;
			if (true)
			{
				Control val2 = (Control)(object)((val is Control) ? val : null);
				if (val2 != null)
				{
					val2.MouseFilter = (Control.MouseFilterEnum)2;
					val2.FocusMode = (Control.FocusModeEnum)0;
				}
				ApplyInteractionStateRecursive(val, enabled);
			}
		}
	}

	private static void ConfigurePassiveChildren(Control root)
	{
		foreach (Node child in ((Node)root).GetChildren(false))
		{
			if (child == null)
			{
				continue;
			}
			Node val = child;
			if (true)
			{
				Control val2 = (Control)(object)((val is Control) ? val : null);
				if (val2 != null)
				{
					val2.MouseFilter = (Control.MouseFilterEnum)2;
					val2.FocusMode = (Control.FocusModeEnum)0;
				}
				ConfigurePassiveChildrenRecursive(val);
			}
		}
	}

	private static void ConfigurePassiveChildrenRecursive(Node root)
	{
		foreach (Node child in root.GetChildren(false))
		{
			if (child == null)
			{
				continue;
			}
			Node val = child;
			if (true)
			{
				Control val2 = (Control)(object)((val is Control) ? val : null);
				if (val2 != null)
				{
					val2.MouseFilter = (Control.MouseFilterEnum)2;
					val2.FocusMode = (Control.FocusModeEnum)0;
				}
				ConfigurePassiveChildrenRecursive(val);
			}
		}
	}

	private static Control? FindTemplateRow(Control parent)
	{
		foreach (Node child in ((Node)parent).GetChildren(false))
		{
			Control val = (Control)(object)((child is Control) ? child : null);
			if (val != null && ((object)val).GetType().Name.Contains("NRunModifierTickbox"))
			{
				return val;
			}
		}
		return null;
	}

	private static Node? FindTickboxNode(Control row)
	{
		string name = ((object)row).GetType().Name;
		if (name.Contains("NRunModifierTickbox", StringComparison.Ordinal) || name.Contains("NTickbox", StringComparison.Ordinal))
		{
			return (Node?)(object)row;
		}
		return FindNodeByTypeName((Node)(object)row, "NTickbox");
	}

	private static Node? FindNodeByName(Node root, string name)
	{
		if (root.Name == (StringName)(name))
		{
			return root;
		}
		foreach (Node child in root.GetChildren(false))
		{
			if (child == null)
			{
				continue;
			}
			Node root2 = child;
			if (true)
			{
				Node val = FindNodeByName(root2, name);
				if (val != null)
				{
					return val;
				}
			}
		}
		return null;
	}

	private static Node? FindNodeByTypeName(Node root, string typeName)
	{
		if (((object)root).GetType().Name.Contains(typeName))
		{
			return root;
		}
		foreach (Node child in root.GetChildren(false))
		{
			if (child == null)
			{
				continue;
			}
			Node root2 = child;
			if (true)
			{
				Node val = FindNodeByTypeName(root2, typeName);
				if (val != null)
				{
					return val;
				}
			}
		}
		return null;
	}

	private static Control? FindLabelNode(Control root)
	{
		foreach (Node child in ((Node)root).GetChildren(false))
		{
			Control val = (Control)(object)((child is Control) ? child : null);
			if (val != null)
			{
				if (((object)val).GetType().Name.Contains("Label"))
				{
					return val;
				}
				Control val2 = FindLabelNode(val);
				if (val2 != null)
				{
					return val2;
				}
			}
		}
		return null;
	}

	private static void SetBoolProperty(object target, string propertyName, bool value)
	{
		try
		{
			AccessTools.Property(target.GetType(), propertyName)?.SetValue(target, value);
		}
		catch
		{
		}
	}

	private static void SetMouseFilter(object target, Control.MouseFilterEnum value)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		Control val = (Control)((target is Control) ? target : null);
		if (val != null)
		{
			val.MouseFilter = value;
		}
	}

	private static void SetText(object target, string text)
	{
		MethodInfo methodInfo = AccessTools.Method(target.GetType(), "SetTextAutoSize", new Type[1] { typeof(string) }, (Type[])null);
		if (methodInfo != null)
		{
			methodInfo.Invoke(target, new object[1] { text });
		}
		else
		{
			AccessTools.Property(target.GetType(), "BbcodeEnabled")?.SetValue(target, true);
			AccessTools.Property(target.GetType(), "Text")?.SetValue(target, text);
		}
	}
}
