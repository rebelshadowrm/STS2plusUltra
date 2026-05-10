using System;
using System.Collections.Generic;
using Godot;
using STS2Plus.Config;
using STS2Plus.Features;
using STS2Plus.Reflection;

namespace STS2Plus.Ui;

[ScriptPath("res://src/Ui/IncomingDamageOverlay.cs")]
internal sealed class IncomingDamageOverlay : PanelContainer
{
	private const string OverlayNodeName = "STS2PlusIncomingDamageOverlay";

	private static readonly Dictionary<Node, IncomingDamageOverlay> Instances = new Dictionary<Node, IncomingDamageOverlay>();

	private StyleBoxFlat? panelStyle;

	private Label? valueLabel;

	private Label? detailLabel;

	private Node? attachedCreatureNode;

	private object? attachedCreatureEntity;

	private bool needsRefresh = true;

	public static void Attach(Node creatureNode, object creatureEntity)
	{
		if (!ConfigManager.Current.PlayerCombatShieldEnabled)
		{
			return;
		}
		if (!GodotObject.IsInstanceValid((GodotObject)(object)creatureNode))
		{
			return;
		}
		if (Instances.TryGetValue(creatureNode, out IncomingDamageOverlay value) && GodotObject.IsInstanceValid((GodotObject)(object)value))
		{
			value.Bind(creatureNode, creatureEntity);
			return;
		}
		PruneInvalidInstances();
		IncomingDamageOverlay incomingDamageOverlay = new IncomingDamageOverlay();
		((Node)incomingDamageOverlay).Name = (StringName)OverlayNodeName;
		value = incomingDamageOverlay;
		Instances[creatureNode] = value;
		creatureNode.AddChild((Node)(object)value, false, (Node.InternalMode)0);
		value.Bind(creatureNode, creatureEntity);
	}

	public static void Detach()
	{
		foreach (IncomingDamageOverlay item in SnapshotInstances())
		{
			item.ClearBinding();
		}
		Instances.Clear();
	}

	public static void DetachIfBound(Node creatureNode)
	{
		if (Instances.Remove(creatureNode, out IncomingDamageOverlay value) && GodotObject.IsInstanceValid((GodotObject)(object)value))
		{
			value.ClearBinding();
		}
	}

	public static void RequestRefresh()
	{
		PruneInvalidInstances();
		foreach (IncomingDamageOverlay overlay in Instances.Values)
		{
			overlay.needsRefresh = true;
		}
	}

	public override void _Ready()
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Expected O, but got Unknown
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Expected O, but got Unknown
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Expected O, but got Unknown
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Expected O, but got Unknown
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Expected O, but got Unknown
		//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0324: Unknown result type (might be due to invalid IL or missing references)
		((CanvasItem)this).TopLevel = false;
		((Control)this).MouseFilter = (Control.MouseFilterEnum)2;
		((CanvasItem)this).Visible = false;
		((CanvasItem)this).ZIndex = 250;
		((Control)this).Size = new Vector2(84f, 34f);
		((Control)this).CustomMinimumSize = new Vector2(72f, 28f);
		panelStyle = new StyleBoxFlat();
		panelStyle.BgColor = new Color(0f, 0f, 0f, 0f);
		panelStyle.BorderColor = new Color(0f, 0f, 0f, 0f);
		panelStyle.SetBorderWidthAll(0);
		panelStyle.SetCornerRadiusAll(0);
		panelStyle.ShadowColor = new Color(0f, 0f, 0f, 0f);
		panelStyle.ShadowSize = 0;
		((Control)this).AddThemeStyleboxOverride((StringName)"panel", (StyleBox)(object)panelStyle);
		MarginContainer val = new MarginContainer();
		((Control)val).AddThemeConstantOverride((StringName)"margin_left", 0);
		((Control)val).AddThemeConstantOverride((StringName)"margin_top", 0);
		((Control)val).AddThemeConstantOverride((StringName)"margin_right", 0);
		((Control)val).AddThemeConstantOverride((StringName)"margin_bottom", 0);
		((Node)this).AddChild((Node)(object)val, false, (Node.InternalMode)0);
		VBoxContainer val2 = new VBoxContainer();
		((BoxContainer)val2).Alignment = (BoxContainer.AlignmentMode)1;
		((Node)val).AddChild((Node)(object)val2, false, (Node.InternalMode)0);
		valueLabel = new Label
		{
			HorizontalAlignment = (HorizontalAlignment)1,
			VerticalAlignment = (VerticalAlignment)1,
			Text = string.Empty
		};
		((Control)valueLabel).AddThemeFontSizeOverride((StringName)"font_size", 18);
		((Control)valueLabel).AddThemeColorOverride((StringName)"font_color", new Color(1f, 0.83f, 0.72f, 1f));
		((Control)valueLabel).AddThemeColorOverride((StringName)"font_outline_color", new Color(0.1f, 0.02f, 0.02f, 0.95f));
		((Control)valueLabel).AddThemeConstantOverride((StringName)"outline_size", 5);
		((Control)valueLabel).CustomMinimumSize = new Vector2(72f, 18f);
		((Node)val2).AddChild((Node)(object)valueLabel, false, (Node.InternalMode)0);
		detailLabel = new Label
		{
			HorizontalAlignment = (HorizontalAlignment)1,
			VerticalAlignment = (VerticalAlignment)1,
			Text = string.Empty
		};
		((Control)detailLabel).AddThemeFontSizeOverride((StringName)"font_size", 9);
		((Control)detailLabel).AddThemeColorOverride((StringName)"font_color", new Color(0.92f, 0.84f, 0.72f, 0.92f));
		((Control)detailLabel).AddThemeColorOverride((StringName)"font_outline_color", new Color(0.08f, 0.03f, 0.03f, 0.9f));
		((Control)detailLabel).AddThemeConstantOverride((StringName)"outline_size", 3);
		((Control)detailLabel).CustomMinimumSize = new Vector2(72f, 10f);
		((Node)val2).AddChild((Node)(object)detailLabel, false, (Node.InternalMode)0);
	}

	public override void _ExitTree()
	{
		if (attachedCreatureNode != null)
		{
			Instances.Remove(attachedCreatureNode);
		}
	}

	public override void _Process(double delta)
	{
		if (!IsAttached())
		{
			((CanvasItem)this).Visible = false;
			return;
		}
		UpdateText();
		UpdatePosition();
	}

	private void Bind(Node creatureNode, object creatureEntity)
	{
		attachedCreatureNode = creatureNode;
		attachedCreatureEntity = creatureEntity;
		needsRefresh = true;
		((CanvasItem)this).Visible = true;
	}

	private void ClearBinding()
	{
		attachedCreatureNode = null;
		attachedCreatureEntity = null;
		needsRefresh = true;
		((CanvasItem)this).Visible = false;
	}

	private bool IsAttached()
	{
		return attachedCreatureNode != null && GodotObject.IsInstanceValid((GodotObject)(object)attachedCreatureNode) && attachedCreatureEntity != null && GameReflection.IsLocalPlayerObject(attachedCreatureEntity);
	}

	private void UpdateText()
	{
		if (!needsRefresh && ((CanvasItem)this).Visible && GameReflection.GetCreatureIntentAnchor(attachedCreatureNode).HasValue)
		{
			return;
		}
		needsRefresh = false;
		int currentHp = GameReflection.GetCurrentHp(attachedCreatureEntity);
		int currentBlock = GameReflection.GetCurrentBlock(attachedCreatureEntity);
		int incomingDamageFor = IncomingDamageTracker.GetIncomingDamageFor(attachedCreatureEntity);
		if (incomingDamageFor <= 0 || currentHp <= 0)
		{
			((CanvasItem)this).Visible = false;
			SetDisplay(string.Empty, string.Empty, lethal: false, visible: false);
			return;
		}
		int num = Math.Min(currentBlock, incomingDamageFor);
		int num2 = Math.Max(0, incomingDamageFor - num);
		if (num2 <= 0)
		{
			((CanvasItem)this).Visible = false;
			SetDisplay(string.Empty, string.Empty, lethal: false, visible: false);
			return;
		}
		bool flag = num2 >= currentHp;
		string detail = (flag ? "LETHAL" : string.Empty);
		SetDisplay($"-{num2}", detail, flag, visible: true);
		((CanvasItem)this).Visible = true;
	}

	private void UpdatePosition()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		((Control)this).Position = new Vector2(-36f, -168f);
	}

	private void SetDisplay(string value, string detail, bool lethal, bool visible)
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		if (valueLabel != null && detailLabel != null)
		{
			valueLabel.Text = value;
			detailLabel.Text = detail;
			int result;
			int effectiveDamage = (int.TryParse(value.TrimStart('-'), out result) ? result : 0);
			Color val = ResolveAccentColor(effectiveDamage, lethal);
			((CanvasItem)valueLabel).Modulate = val.Lightened(0.18f);
			((CanvasItem)detailLabel).Modulate = val.Lightened(0.08f);
			((Control)valueLabel).AddThemeFontSizeOverride((StringName)"font_size", lethal ? 22 : 18);
			detailLabel.Text = detail;
			((CanvasItem)detailLabel).Visible = lethal;
			((Control)this).Scale = (Vector2)(lethal ? new Vector2(1.06f, 1.06f) : Vector2.One);
			((CanvasItem)this).Visible = visible;
		}
	}

	private static IReadOnlyList<IncomingDamageOverlay> SnapshotInstances()
	{
		PruneInvalidInstances();
		return new List<IncomingDamageOverlay>(Instances.Values);
	}

	private static void PruneInvalidInstances()
	{
		List<Node> list = new List<Node>();
		foreach (KeyValuePair<Node, IncomingDamageOverlay> instance in Instances)
		{
			if (!GodotObject.IsInstanceValid((GodotObject)(object)instance.Key) || !GodotObject.IsInstanceValid((GodotObject)(object)instance.Value))
			{
				list.Add(instance.Key);
			}
		}
		foreach (Node item in list)
		{
			Instances.Remove(item);
		}
	}

	private static Color ResolveAccentColor(int effectiveDamage, bool lethal)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		if (lethal || effectiveDamage > 20)
		{
			return new Color(0.92f, 0.18f, 0.18f, 0.98f);
		}
		if (effectiveDamage > 10)
		{
			return new Color(0.72f, 0.4f, 0.94f, 0.98f);
		}
		return new Color(0.98f, 0.8f, 0.24f, 0.98f);
	}

}
