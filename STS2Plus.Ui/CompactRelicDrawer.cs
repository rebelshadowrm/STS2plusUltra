using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Godot;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.ControllerInput;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Multiplayer;
using MegaCrit.Sts2.Core.Nodes.Relics;
using MegaCrit.Sts2.Core.Nodes.Screens.InspectScreens;
using MegaCrit.Sts2.Core.Runs;
using STS2Plus.Config;
using STS2Plus.Localization;

namespace STS2Plus.Ui;

[ScriptPath("res://src/Ui/CompactRelicDrawer.cs")]
internal sealed class CompactRelicDrawer : Control
{
	private const string DrawerNodeName = "STS2PlusCompactRelicDrawer";

	private const float ButtonWidth = 116f;

	private const float ButtonHeight = 64f;

	private const float PopupMargin = 16f;

	private const float PopupTopGap = 10f;

	private const float MultiplayerGap = 10f;

	private const float GridItemSize = 76f;

	private const float GridSpacing = 10f;

	private static readonly Dictionary<NGlobalUi, CompactRelicDrawer> Instances = new Dictionary<NGlobalUi, CompactRelicDrawer>();

	private static readonly Color DrawerBackground = new Color(0.11f, 0.08f, 0.08f, 0.96f);

	private static readonly Color DrawerBorder = new Color(0.79f, 0.58f, 0.28f, 0.96f);

	private static readonly Color DrawerHover = new Color(0.16f, 0.12f, 0.12f, 0.98f);

	private static readonly Color DrawerPressed = new Color(0.08f, 0.06f, 0.06f, 0.98f);

	private static readonly Color DrawerPanel = new Color(0.08f, 0.06f, 0.06f, 0.97f);

	private static readonly Color DrawerPanelBorder = new Color(0.82f, 0.57f, 0.25f, 1f);

	private static readonly Color DrawerBackdrop = new Color(0f, 0f, 0f, 0.16f);

	private static readonly Color LabelAccent = new Color(0.96f, 0.84f, 0.48f, 1f);

	private static readonly Color CountAccent = new Color(1f, 0.95f, 0.84f, 1f);

	private NGlobalUi? globalUi;

	private NRelicInventory? legacyInventory;

	private Player? player;

	private Button? summaryButton;

	private Label? titleLabel;

	private Label? countLabel;

	private TextureRect? previewIcon;

	private ColorRect? backstop;

	private PanelContainer? popupPanel;

	private Label? popupTitleLabel;

	private Button? closeButton;

	private ScrollContainer? scrollContainer;

	private GridContainer? gridContainer;

	private Label? emptyLabel;

	private Tween? buttonTween;

	private Vector2 lastKnownLegacyPosition;

	private int lastColumnCount;

	private bool popupOpen;

	private bool _readyRan;

	public static void Attach(NGlobalUi globalUi, RunState runState)
	{
		if (!GodotObject.IsInstanceValid((GodotObject)(object)globalUi))
		{
			return;
		}
		if (!ConfigManager.Current.CompactRelicDrawerEnabled)
		{
			RestoreLegacyFor(globalUi);
			return;
		}
		PruneInvalidInstances();
		if (!Instances.TryGetValue(globalUi, out CompactRelicDrawer value) || !GodotObject.IsInstanceValid((GodotObject)(object)value))
		{
			CompactRelicDrawer compactRelicDrawer = new CompactRelicDrawer();
			((Node)compactRelicDrawer).Name = (StringName)"STS2PlusCompactRelicDrawer";
			value = compactRelicDrawer;
			Instances[globalUi] = value;
			((Node)globalUi).AddChild((Node)(object)value, false, (Node.InternalMode)0);
			((Node)globalUi).MoveChild((Node)(object)value, -1);
		}
		value.Bind(globalUi, runState);
	}

	public static Control? GetPrimaryControl(NGlobalUi? globalUi)
	{
		if (!ConfigManager.Current.CompactRelicDrawerEnabled)
		{
			return null;
		}
		CompactRelicDrawer drawer;
		return (Control?)(object)(TryGetInstance(globalUi, out drawer) ? drawer.summaryButton : null);
	}

	public static bool TryApplyMultiplayerLayout(NMultiplayerPlayerStateContainer container)
	{
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		if (!ConfigManager.Current.CompactRelicDrawerEnabled)
		{
			return false;
		}
		NRun instance = NRun.Instance;
		if (!TryGetInstance((instance != null) ? instance.GlobalUi : null, out CompactRelicDrawer drawer) || drawer.legacyInventory == null || drawer.summaryButton == null)
		{
			return false;
		}
		Vector2 position = ((Control)drawer.legacyInventory).Position;
		position.Y += drawer.GetReservedHeight();
		((Control)container).Position = position;
		return true;
	}

	public static void SyncToLegacy(NRelicInventory legacyInventory)
	{
		if (ConfigManager.Current.CompactRelicDrawerEnabled)
		{
			NRun instance = NRun.Instance;
			if (TryGetInstance((instance != null) ? instance.GlobalUi : null, out CompactRelicDrawer drawer))
			{
				drawer.SyncToLegacyPosition(legacyInventory);
			}
		}
	}

	public static void ApplyConfiguration()
	{
		if (ConfigManager.Current.CompactRelicDrawerEnabled)
		{
			NRun instance = NRun.Instance;
			NGlobalUi val = ((instance != null) ? instance.GlobalUi : null);
			RunManager instance2 = RunManager.Instance;
			RunState val2 = ((instance2 != null) ? instance2.DebugOnlyGetState() : null);
			if (val != null && val2 != null)
			{
				Attach(val, val2);
			}
		}
		else
		{
			NGlobalUi[] array = Instances.Keys.ToArray();
			foreach (NGlobalUi val3 in array)
			{
				RestoreLegacyFor(val3);
			}
		}
	}

	public override void _Ready()
	{
		EnsureReady();
	}

	private void EnsureReady()
	{
		if (_readyRan)
		{
			return;
		}
		_readyRan = true;
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Expected O, but got Unknown
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Expected O, but got Unknown
		((Control)this).MouseFilter = (Control.MouseFilterEnum)2;
		((Control)this).FocusMode = (Control.FocusModeEnum)0;
		((Control)this).SizeFlagsHorizontal = (Control.SizeFlags)3;
		((Control)this).SizeFlagsVertical = (Control.SizeFlags)3;
		((Control)this).SetAnchorsAndOffsetsPreset((LayoutPreset)15, (LayoutPresetMode)0, 0);
		((CanvasItem)this).ZIndex = 180;
		summaryButton = CreateSummaryButton();
		((Node)this).AddChild((Node)(object)summaryButton, false, (Node.InternalMode)0);
		backstop = new ColorRect
		{
			Name = (StringName)"Backstop",
			Visible = false,
			MouseFilter = (Control.MouseFilterEnum)2,
			Color = DrawerBackdrop,
			FocusMode = (Control.FocusModeEnum)0
		};
		((Control)backstop).SetAnchorsAndOffsetsPreset((LayoutPreset)15, (LayoutPresetMode)0, 0);
		((Control)backstop).GuiInput += new Control.GuiInputEventHandler(OnBackstopGuiInput);
		((Node)this).AddChild((Node)(object)backstop, false, (Node.InternalMode)0);
		popupPanel = CreatePopupPanel();
		((CanvasItem)popupPanel).Visible = false;
		((Node)this).AddChild((Node)(object)popupPanel, false, (Node.InternalMode)0);
	}

	public override void _ExitTree()
	{
		if (legacyInventory != null && GodotObject.IsInstanceValid((GodotObject)(object)legacyInventory))
		{
			((CanvasItem)legacyInventory).Visible = true;
			((Control)legacyInventory).MouseFilter = (Control.MouseFilterEnum)1;
			((Control)legacyInventory).FocusMode = (Control.FocusModeEnum)2;
			((Control)legacyInventory).FocusBehaviorRecursive = (FocusBehaviorRecursiveEnum)0;
		}
		if (player != null)
		{
			player.RelicObtained -= OnRelicObtained;
			player.RelicRemoved -= OnRelicRemoved;
		}
		if (globalUi != null)
		{
			Instances.Remove(globalUi);
		}
		Tween? obj = buttonTween;
		if (obj != null)
		{
			obj.Kill();
		}
		player = null;
		legacyInventory = null;
		globalUi = null;
	}

	public override void _Input(InputEvent @event)
	{
		if (popupOpen && (@event.IsActionPressed(MegaInput.cancel, false, false) || @event.IsActionPressed(MegaInput.pauseAndBack, false, false)))
		{
			ClosePopup();
			((Node)this).GetViewport().SetInputAsHandled();
		}
	}

	public override void _Process(double delta)
	{
		if (popupOpen)
		{
			UpdatePopupLayout();
		}
	}

	private void Bind(NGlobalUi globalUi, RunState runState)
	{
		EnsureReady();
		if (player != null)
		{
			player.RelicObtained -= OnRelicObtained;
			player.RelicRemoved -= OnRelicRemoved;
		}
		this.globalUi = globalUi;
		legacyInventory = globalUi.RelicInventory;
		player = LocalContext.GetMe((IPlayerCollection)(object)runState);
		if (player != null)
		{
			player.RelicObtained += OnRelicObtained;
			player.RelicRemoved += OnRelicRemoved;
			HideLegacyInventory();
			SyncToLegacyPosition(legacyInventory);
			RefreshDisplay(rebuildGrid: true);
			((CanvasItem)this).Visible = true;
			((Node)this).SetProcess(true);
			((Node)this).SetProcessInput(true);
			if (summaryButton != null)
				((Control)summaryButton).MouseFilter = (Control.MouseFilterEnum)0;
		}
	}

	private void HideLegacyInventory()
	{
		if (legacyInventory != null && GodotObject.IsInstanceValid((GodotObject)(object)legacyInventory))
		{
			((CanvasItem)legacyInventory).Visible = false;
			((Control)legacyInventory).MouseFilter = (Control.MouseFilterEnum)2;
			((Control)legacyInventory).FocusMode = (Control.FocusModeEnum)0;
			((Control)legacyInventory).FocusBehaviorRecursive = (FocusBehaviorRecursiveEnum)1;
		}
	}

	private void RestoreLegacyInventory()
	{
		if (legacyInventory != null && GodotObject.IsInstanceValid((GodotObject)(object)legacyInventory))
		{
			((CanvasItem)legacyInventory).Visible = true;
			((Control)legacyInventory).MouseFilter = (Control.MouseFilterEnum)1;
			((Control)legacyInventory).FocusMode = (Control.FocusModeEnum)2;
			((Control)legacyInventory).FocusBehaviorRecursive = (FocusBehaviorRecursiveEnum)0;
		}
		ClosePopup();
		((CanvasItem)this).Visible = false;
		((Node)this).SetProcess(false);
		((Node)this).SetProcessInput(false);
		if (summaryButton != null)
			((Control)summaryButton).MouseFilter = (Control.MouseFilterEnum)2;
		if (backstop != null)
			((Control)backstop).MouseFilter = (Control.MouseFilterEnum)2;
		if (popupPanel != null)
			((Control)popupPanel).MouseFilter = (Control.MouseFilterEnum)2;
		ModEntry.Verbose("CompactRelicDrawer: disabled, input processing stopped");
	}

	private void SyncToLegacyPosition(NRelicInventory inventory)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		legacyInventory = inventory;
		HideLegacyInventory();
		if (summaryButton == null)
		{
			return;
		}
		lastKnownLegacyPosition = ((Control)inventory).Position;
		((Control)summaryButton).Position = lastKnownLegacyPosition;
		if (popupOpen)
		{
			if (((Control)inventory).Position.Y < -4f)
			{
				ClosePopup();
			}
			else
			{
				UpdatePopupLayout();
			}
		}
	}

	private float GetReservedHeight()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		if (summaryButton != null && GodotObject.IsInstanceValid((GodotObject)(object)summaryButton))
		{
			return Math.Max(((Control)summaryButton).Size.Y, 64f) + 10f;
		}
		return 74f;
	}

	private Button CreateSummaryButton()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Expected O, but got Unknown
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Expected O, but got Unknown
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Expected O, but got Unknown
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Expected O, but got Unknown
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Expected O, but got Unknown
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0305: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Expected O, but got Unknown
		//IL_033b: Unknown result type (might be due to invalid IL or missing references)
		//IL_036a: Unknown result type (might be due to invalid IL or missing references)
		//IL_039d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03db: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_040b: Unknown result type (might be due to invalid IL or missing references)
		//IL_041b: Expected O, but got Unknown
		Button val = new Button
		{
			Name = (StringName)"DrawerButton",
			CustomMinimumSize = new Vector2(116f, 64f),
			Size = new Vector2(116f, 64f),
			FocusMode = (Control.FocusModeEnum)2,
			MouseDefaultCursorShape = (Control.CursorShape)2,
			Text = string.Empty
		};
		((Control)val).AddThemeStyleboxOverride((StringName)"normal", (StyleBox)(object)CreateStyle(DrawerBackground, DrawerBorder, 2));
		((Control)val).AddThemeStyleboxOverride((StringName)"hover", (StyleBox)(object)CreateStyle(DrawerHover, DrawerBorder.Lightened(0.08f), 2));
		((Control)val).AddThemeStyleboxOverride((StringName)"pressed", (StyleBox)(object)CreateStyle(DrawerPressed, DrawerBorder.Lightened(0.12f), 2));
		((Control)val).AddThemeStyleboxOverride((StringName)"focus", (StyleBox)(object)CreateFocusStyle());
		((Control)val).AddThemeStyleboxOverride((StringName)"disabled", (StyleBox)(object)CreateStyle(DrawerBackground.Darkened(0.12f), DrawerBorder, 2));
		((BaseButton)val).Pressed += TogglePopup;
		MarginContainer val2 = new MarginContainer();
		((Control)val2).SetAnchorsAndOffsetsPreset((LayoutPreset)15, (LayoutPresetMode)0, 0);
		((Control)val2).AddThemeConstantOverride((StringName)"margin_left", 10);
		((Control)val2).AddThemeConstantOverride((StringName)"margin_top", 8);
		((Control)val2).AddThemeConstantOverride((StringName)"margin_right", 10);
		((Control)val2).AddThemeConstantOverride((StringName)"margin_bottom", 8);
		((Control)val2).MouseFilter = (Control.MouseFilterEnum)2;
		((Node)val).AddChild((Node)(object)val2, false, (Node.InternalMode)0);
		HBoxContainer val3 = new HBoxContainer
		{
			MouseFilter = (Control.MouseFilterEnum)2
		};
		((Control)val3).SizeFlagsHorizontal = (Control.SizeFlags)3;
		((Control)val3).SizeFlagsVertical = (Control.SizeFlags)3;
		((Node)val2).AddChild((Node)(object)val3, false, (Node.InternalMode)0);
		VBoxContainer val4 = new VBoxContainer
		{
			MouseFilter = (Control.MouseFilterEnum)2
		};
		((Control)val4).SizeFlagsHorizontal = (Control.SizeFlags)3;
		((Node)val3).AddChild((Node)(object)val4, false, (Node.InternalMode)0);
		titleLabel = new Label
		{
			Text = PlusLoc.Text("RELIC_DRAWER_LABEL"),
			HorizontalAlignment = (HorizontalAlignment)0,
			VerticalAlignment = (VerticalAlignment)1,
			MouseFilter = (Control.MouseFilterEnum)2
		};
		((Control)titleLabel).AddThemeFontSizeOverride((StringName)"font_size", 13);
		((Control)titleLabel).AddThemeColorOverride((StringName)"font_color", LabelAccent);
		((Control)titleLabel).AddThemeColorOverride((StringName)"font_outline_color", new Color(0.14f, 0.08f, 0.04f, 0.9f));
		((Control)titleLabel).AddThemeConstantOverride((StringName)"outline_size", 2);
		((Node)val4).AddChild((Node)(object)titleLabel, false, (Node.InternalMode)0);
		countLabel = new Label
		{
			Text = "0",
			HorizontalAlignment = (HorizontalAlignment)0,
			VerticalAlignment = (VerticalAlignment)1,
			MouseFilter = (Control.MouseFilterEnum)2
		};
		((Control)countLabel).AddThemeFontSizeOverride((StringName)"font_size", 28);
		((Control)countLabel).AddThemeColorOverride((StringName)"font_color", CountAccent);
		((Control)countLabel).AddThemeColorOverride((StringName)"font_outline_color", new Color(0.16f, 0.08f, 0.04f, 0.96f));
		((Control)countLabel).AddThemeConstantOverride((StringName)"outline_size", 3);
		((Node)val4).AddChild((Node)(object)countLabel, false, (Node.InternalMode)0);
		previewIcon = new TextureRect
		{
			Name = (StringName)"PreviewIcon",
			StretchMode = (TextureRect.StretchModeEnum)5,
			ExpandMode = (TextureRect.ExpandModeEnum)1,
			CustomMinimumSize = new Vector2(34f, 34f),
			SizeFlagsHorizontal = (Control.SizeFlags)8,
			SizeFlagsVertical = (Control.SizeFlags)4,
			MouseFilter = (Control.MouseFilterEnum)2,
			Modulate = new Color(1f, 1f, 1f, 0.92f)
		};
		((Node)val3).AddChild((Node)(object)previewIcon, false, (Node.InternalMode)0);
		return val;
	}

	private PanelContainer CreatePopupPanel()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Expected O, but got Unknown
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Expected O, but got Unknown
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Expected O, but got Unknown
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Expected O, but got Unknown
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Expected O, but got Unknown
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0338: Unknown result type (might be due to invalid IL or missing references)
		//IL_0367: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ec: Expected O, but got Unknown
		//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_040e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0417: Unknown result type (might be due to invalid IL or missing references)
		//IL_0420: Unknown result type (might be due to invalid IL or missing references)
		//IL_0429: Unknown result type (might be due to invalid IL or missing references)
		//IL_0432: Unknown result type (might be due to invalid IL or missing references)
		//IL_043b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0448: Expected O, but got Unknown
		//IL_048d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0493: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b9: Expected O, but got Unknown
		//IL_051b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0520: Unknown result type (might be due to invalid IL or missing references)
		//IL_0531: Unknown result type (might be due to invalid IL or missing references)
		//IL_0539: Unknown result type (might be due to invalid IL or missing references)
		//IL_0542: Unknown result type (might be due to invalid IL or missing references)
		//IL_0550: Expected O, but got Unknown
		//IL_0592: Unknown result type (might be due to invalid IL or missing references)
		//IL_0597: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d0: Expected O, but got Unknown
		//IL_060c: Unknown result type (might be due to invalid IL or missing references)
		//IL_063b: Unknown result type (might be due to invalid IL or missing references)
		PanelContainer val = new PanelContainer
		{
			Name = (StringName)"PopupPanel",
			MouseFilter = (Control.MouseFilterEnum)0,
			FocusMode = (Control.FocusModeEnum)0,
			CustomMinimumSize = new Vector2(760f, 360f),
			Size = new Vector2(760f, 360f),
			ZIndex = 181
		};
		((Control)val).AddThemeStyleboxOverride((StringName)"panel", (StyleBox)(object)CreateStyle(DrawerPanel, DrawerPanelBorder, 2, 12, (Color?)new Color(0f, 0f, 0f, 0.24f), 8));
		VBoxContainer val2 = new VBoxContainer
		{
			MouseFilter = (Control.MouseFilterEnum)2
		};
		((Control)val2).SizeFlagsHorizontal = (Control.SizeFlags)3;
		((Control)val2).SizeFlagsVertical = (Control.SizeFlags)3;
		((Node)val).AddChild((Node)(object)val2, false, (Node.InternalMode)0);
		HBoxContainer val3 = new HBoxContainer
		{
			CustomMinimumSize = new Vector2(0f, 46f),
			MouseFilter = (Control.MouseFilterEnum)2
		};
		((Control)val3).AddThemeConstantOverride((StringName)"separation", 12);
		((Node)val2).AddChild((Node)(object)val3, false, (Node.InternalMode)0);
		popupTitleLabel = new Label
		{
			Text = PlusLoc.Text("RELIC_DRAWER_ALL"),
			HorizontalAlignment = (HorizontalAlignment)0,
			VerticalAlignment = (VerticalAlignment)1,
			SizeFlagsHorizontal = (Control.SizeFlags)3,
			MouseFilter = (Control.MouseFilterEnum)2
		};
		((Control)popupTitleLabel).AddThemeFontSizeOverride((StringName)"font_size", 20);
		((Control)popupTitleLabel).AddThemeColorOverride((StringName)"font_color", CountAccent);
		((Control)popupTitleLabel).AddThemeColorOverride((StringName)"font_outline_color", new Color(0.12f, 0.06f, 0.03f, 0.9f));
		((Control)popupTitleLabel).AddThemeConstantOverride((StringName)"outline_size", 2);
		((Node)val3).AddChild((Node)(object)popupTitleLabel, false, (Node.InternalMode)0);
		closeButton = new Button
		{
			Text = "x",
			CustomMinimumSize = new Vector2(34f, 34f),
			Size = new Vector2(34f, 34f),
			FocusMode = (Control.FocusModeEnum)2,
			MouseDefaultCursorShape = (Control.CursorShape)2
		};
		((Control)closeButton).AddThemeStyleboxOverride((StringName)"normal", (StyleBox)(object)CreateStyle(new Color(0.23f, 0.1f, 0.08f, 0.95f), DrawerPanelBorder, 1, 8));
		((Control)closeButton).AddThemeStyleboxOverride((StringName)"hover", (StyleBox)(object)CreateStyle(new Color(0.32f, 0.12f, 0.1f, 0.98f), DrawerPanelBorder.Lightened(0.15f), 1, 8));
		((Control)closeButton).AddThemeStyleboxOverride((StringName)"pressed", (StyleBox)(object)CreateStyle(new Color(0.18f, 0.08f, 0.07f, 0.98f), DrawerPanelBorder, 1, 8));
		((Control)closeButton).AddThemeStyleboxOverride((StringName)"focus", (StyleBox)(object)CreateFocusStyle());
		((Control)closeButton).AddThemeColorOverride((StringName)"font_color", CountAccent);
		((Control)closeButton).AddThemeColorOverride((StringName)"font_outline_color", new Color(0.12f, 0.05f, 0.03f, 0.9f));
		((Control)closeButton).AddThemeConstantOverride((StringName)"outline_size", 2);
		((BaseButton)closeButton).Pressed += ClosePopup;
		((Node)val3).AddChild((Node)(object)closeButton, false, (Node.InternalMode)0);
		ColorRect val4 = new ColorRect
		{
			Color = new Color(DrawerPanelBorder, 0.35f),
			CustomMinimumSize = new Vector2(0f, 1f),
			MouseFilter = (Control.MouseFilterEnum)2
		};
		((Node)val2).AddChild((Node)(object)val4, false, (Node.InternalMode)0);
		scrollContainer = new ScrollContainer
		{
			Name = (StringName)"ScrollContainer",
			SizeFlagsHorizontal = (Control.SizeFlags)3,
			SizeFlagsVertical = (Control.SizeFlags)3,
			MouseFilter = (Control.MouseFilterEnum)0,
			HorizontalScrollMode = (ScrollContainer.ScrollMode)0,
			VerticalScrollMode = (ScrollContainer.ScrollMode)4,
			FollowFocus = true
		};
		((Control)scrollContainer).AddThemeConstantOverride((StringName)"h_separation", 0);
		((Control)scrollContainer).AddThemeConstantOverride((StringName)"v_separation", 0);
		((GodotObject)scrollContainer).Connect(SignalName.Resized, Callable.From((Action)UpdatePopupLayout), 0u);
		((Node)val2).AddChild((Node)(object)scrollContainer, false, (Node.InternalMode)0);
		MarginContainer val5 = new MarginContainer
		{
			MouseFilter = (Control.MouseFilterEnum)2
		};
		((Control)val5).AddThemeConstantOverride((StringName)"margin_left", 12);
		((Control)val5).AddThemeConstantOverride((StringName)"margin_top", 12);
		((Control)val5).AddThemeConstantOverride((StringName)"margin_right", 12);
		((Control)val5).AddThemeConstantOverride((StringName)"margin_bottom", 12);
		((Node)scrollContainer).AddChild((Node)(object)val5, false, (Node.InternalMode)0);
		gridContainer = new GridContainer
		{
			Name = (StringName)"RelicGrid",
			Columns = 8,
			MouseFilter = (Control.MouseFilterEnum)2,
			SizeFlagsHorizontal = (Control.SizeFlags)3
		};
		((Control)gridContainer).AddThemeConstantOverride((StringName)"h_separation", 10);
		((Control)gridContainer).AddThemeConstantOverride((StringName)"v_separation", 10);
		((Node)val5).AddChild((Node)(object)gridContainer, false, (Node.InternalMode)0);
		emptyLabel = new Label
		{
			Text = PlusLoc.Text("RELIC_DRAWER_EMPTY"),
			HorizontalAlignment = (HorizontalAlignment)1,
			VerticalAlignment = (VerticalAlignment)1,
			Visible = false,
			MouseFilter = (Control.MouseFilterEnum)2
		};
		((Control)emptyLabel).AddThemeFontSizeOverride((StringName)"font_size", 18);
		((Control)emptyLabel).AddThemeColorOverride((StringName)"font_color", new Color(0.88f, 0.78f, 0.64f, 0.86f));
		((Control)emptyLabel).AddThemeColorOverride((StringName)"font_outline_color", new Color(0.12f, 0.06f, 0.03f, 0.85f));
		((Control)emptyLabel).AddThemeConstantOverride((StringName)"outline_size", 2);
		((Node)val5).AddChild((Node)(object)emptyLabel, false, (Node.InternalMode)0);
		return val;
	}

	private void TogglePopup()
	{
		if (popupOpen)
		{
			ClosePopup();
		}
		else
		{
			OpenPopup();
		}
	}

	private void OpenPopup()
	{
		if (popupPanel != null && backstop != null)
		{
			RefreshDisplay(rebuildGrid: true);
			popupOpen = true;
			((CanvasItem)backstop).Visible = true;
			((Control)backstop).MouseFilter = (Control.MouseFilterEnum)1;
			((CanvasItem)popupPanel).Visible = true;
			((Control)popupPanel).MouseFilter = (Control.MouseFilterEnum)0;
			((Control)popupPanel).FocusMode = (Control.FocusModeEnum)2;
			UpdatePopupLayout();
			if (closeButton != null)
			{
				((Control)closeButton).GrabFocus();
			}
		}
	}

	private void ClosePopup()
	{
		popupOpen = false;
		if (backstop != null)
		{
			((CanvasItem)backstop).Visible = false;
			((Control)backstop).MouseFilter = (Control.MouseFilterEnum)2;
		}
		if (popupPanel != null)
		{
			((CanvasItem)popupPanel).Visible = false;
			((Control)popupPanel).MouseFilter = (Control.MouseFilterEnum)2;
			((Control)popupPanel).FocusMode = (Control.FocusModeEnum)0;
		}
		if (summaryButton != null && GodotObject.IsInstanceValid((GodotObject)(object)summaryButton))
		{
			((Control)summaryButton).GrabFocus();
		}
	}

	private void OnBackstopGuiInput(InputEvent @event)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Invalid comparison between Unknown and I8
		if (popupOpen)
		{
			InputEventMouseButton val = (InputEventMouseButton)(object)((@event is InputEventMouseButton) ? @event : null);
			if (val != null && val.Pressed && (long)val.ButtonIndex == 1)
			{
				ClosePopup();
			}
		}
	}

	private void OnRelicObtained(RelicModel relic)
	{
		RefreshDisplay(rebuildGrid: true);
		PulseButton();
	}

	private void OnRelicRemoved(RelicModel relic)
	{
		RefreshDisplay(rebuildGrid: true);
	}

	private void PulseButton()
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		if (summaryButton != null && GodotObject.IsInstanceValid((GodotObject)(object)summaryButton))
		{
			Tween? obj = buttonTween;
			if (obj != null)
			{
				obj.Kill();
			}
			((Control)summaryButton).Scale = Vector2.One;
			((CanvasItem)summaryButton).Modulate = Colors.White;
			buttonTween = ((Node)this).CreateTween().SetParallel(true);
			buttonTween.TweenProperty((GodotObject)(object)summaryButton, (NodePath)"scale", (Variant)(new Vector2(1.06f, 1.06f)), 0.12).SetEase((Tween.EaseType)1).SetTrans((Tween.TransitionType)10);
			buttonTween.TweenProperty((GodotObject)(object)summaryButton, (NodePath)"modulate", (Variant)(new Color(1f, 0.97f, 0.89f, 1f)), 0.12);
			buttonTween.TweenProperty((GodotObject)(object)summaryButton, (NodePath)"scale", (Variant)(Vector2.One), 0.18).SetDelay(0.12).SetEase((Tween.EaseType)1)
				.SetTrans((Tween.TransitionType)7);
			buttonTween.TweenProperty((GodotObject)(object)summaryButton, (NodePath)"modulate", (Variant)(Colors.White), 0.18).SetDelay(0.12);
		}
	}

	private void RefreshDisplay(bool rebuildGrid)
	{
		Player? obj = player;
		IReadOnlyList<RelicModel> readOnlyList = ((obj != null) ? obj.Relics : null) ?? Array.Empty<RelicModel>();
		if (titleLabel != null)
		{
			titleLabel.Text = PlusLoc.Text("RELIC_DRAWER_LABEL");
		}
		if (countLabel != null)
		{
			countLabel.Text = readOnlyList.Count.ToString();
		}
		if (previewIcon != null)
		{
			TextureRect? obj2 = previewIcon;
			RelicModel? obj3 = readOnlyList.FirstOrDefault();
			obj2.Texture = ((obj3 != null) ? obj3.Icon : null);
			((CanvasItem)previewIcon).Visible = previewIcon.Texture != null;
		}
		if (popupTitleLabel != null)
		{
			popupTitleLabel.Text = $"{PlusLoc.Text("RELIC_DRAWER_ALL")} ({readOnlyList.Count})";
		}
		if (emptyLabel != null)
		{
			((CanvasItem)emptyLabel).Visible = readOnlyList.Count == 0;
		}
		if (rebuildGrid)
		{
			RebuildGrid(readOnlyList);
		}
	}

	private void RebuildGrid(IReadOnlyList<RelicModel> relics)
	{
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		if (gridContainer == null || legacyInventory == null)
		{
			return;
		}
		Node[] array = ((IEnumerable)((Node)gridContainer).GetChildren(false)).OfType<Node>().ToArray();
		foreach (Node val in array)
		{
			((Node)gridContainer).RemoveChild(val);
			val.QueueFree();
		}
		if (emptyLabel != null)
		{
			((CanvasItem)emptyLabel).Visible = relics.Count == 0;
		}
		foreach (RelicModel relic in relics)
		{
			NRelicInventoryHolder val2 = NRelicInventoryHolder.Create(relic);
			if (val2 != null)
			{
				val2.Inventory = legacyInventory;
				((Control)val2).CustomMinimumSize = new Vector2(76f, 76f);
				((Control)val2).Size = new Vector2(76f, 76f);
				((GodotObject)val2).Connect((StringName)"Released", Callable.From<NClickableControl>((Action<NClickableControl>)delegate
				{
					OnRelicPressed(relic);
				}), 0u);
				((Node)gridContainer).AddChild((Node)(object)val2, false, (Node.InternalMode)0);
			}
		}
		UpdatePopupLayout();
		UpdateGridNavigation();
	}

	private void OnRelicPressed(RelicModel relic)
	{
		if (player != null)
		{
			ClosePopup();
			NGame instance = NGame.Instance;
			NInspectRelicScreen val = ((instance != null) ? instance.GetInspectRelicScreen() : null);
			if (val != null)
			{
				val.Open((IReadOnlyList<RelicModel>)player.Relics.ToList(), relic);
			}
		}
	}

	private void UpdatePopupLayout()
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		if (popupOpen && popupPanel != null && summaryButton != null && scrollContainer != null && gridContainer != null)
		{
			Rect2 viewportRect = ((CanvasItem)this).GetViewportRect();
			Vector2 size = viewportRect.Size;
			float num = Mathf.Clamp(size.X * 0.56f, 540f, 920f);
			float num2 = Mathf.Clamp(size.Y * 0.42f, 300f, 460f);
			((Control)popupPanel).CustomMinimumSize = new Vector2(num, num2);
			((Control)popupPanel).Size = new Vector2(num, num2);
			float x = ((Control)summaryButton).Position.X;
			float num3 = ((Control)summaryButton).Position.Y + ((Control)summaryButton).Size.Y + 10f;
			((Control)popupPanel).Position = new Vector2(Mathf.Clamp(x, 16f, size.X - num - 16f), Mathf.Clamp(num3, 16f, size.Y - num2 - 16f));
			float num4 = Math.Max(180f, ((Control)scrollContainer).Size.X - 24f);
			int num5 = Math.Max(1, (int)Math.Floor((num4 + 10f) / 86f));
			scrollContainer.ScrollHorizontal = 0;
			if (gridContainer.Columns != num5)
			{
				gridContainer.Columns = num5;
				lastColumnCount = num5;
				UpdateGridNavigation();
			}
		}
	}

	private void UpdateGridNavigation()
	{
		if (gridContainer == null)
		{
			return;
		}
		List<Control> list = ((IEnumerable)((Node)gridContainer).GetChildren(false)).OfType<Control>().ToList();
		if (list.Count == 0)
		{
			return;
		}
		int num = Math.Max(1, gridContainer.Columns);
		for (int i = 0; i < list.Count; i++)
		{
			Control val = list[i];
			int num2 = i / num;
			int num3 = i % num;
			object obj2;
			if (num2 <= 0 || i - num < 0)
			{
				Button? obj = closeButton;
				obj2 = ((obj != null) ? ((Node)obj).GetPath() : null) ?? ((Node)val).GetPath();
			}
			else
			{
				obj2 = ((Node)list[i - num]).GetPath();
			}
			NodePath focusNeighborTop = (NodePath)obj2;
			NodePath focusNeighborBottom = ((i + num < list.Count) ? ((Node)list[i + num]).GetPath() : ((Node)val).GetPath());
			val.FocusNeighborLeft = ((Node)((num3 > 0) ? list[i - 1] : list[i])).GetPath();
			val.FocusNeighborRight = ((Node)((num3 < num - 1 && i + 1 < list.Count) ? list[i + 1] : list[i])).GetPath();
			val.FocusNeighborTop = focusNeighborTop;
			val.FocusNeighborBottom = focusNeighborBottom;
		}
		if (closeButton != null)
		{
			((Control)closeButton).FocusNeighborBottom = ((Node)list[0]).GetPath();
			((Control)closeButton).FocusNeighborLeft = ((Node)closeButton).GetPath();
			((Control)closeButton).FocusNeighborRight = ((Node)closeButton).GetPath();
		}
	}

	private static bool TryGetInstance(NGlobalUi? globalUi, out CompactRelicDrawer drawer)
	{
		PruneInvalidInstances();
		if (globalUi != null && Instances.TryGetValue(globalUi, out CompactRelicDrawer value) && GodotObject.IsInstanceValid((GodotObject)(object)value))
		{
			drawer = value;
			return true;
		}
		drawer = null;
		return false;
	}

	private static void RestoreLegacyFor(NGlobalUi? globalUi)
	{
		if (globalUi == null || !GodotObject.IsInstanceValid((GodotObject)(object)globalUi))
		{
			return;
		}
		if (TryGetInstance(globalUi, out CompactRelicDrawer drawer))
		{
			drawer.RestoreLegacyInventory();
		}
		// If no drawer was ever created, don't touch the legacy inventory —
		// we never modified it, so there's nothing to restore.
	}

	private static void PruneInvalidInstances()
	{
		List<NGlobalUi> list = new List<NGlobalUi>();
		foreach (KeyValuePair<NGlobalUi, CompactRelicDrawer> instance in Instances)
		{
			if (!GodotObject.IsInstanceValid((GodotObject)(object)instance.Key) || !GodotObject.IsInstanceValid((GodotObject)(object)instance.Value))
			{
				list.Add(instance.Key);
			}
		}
		foreach (NGlobalUi item in list)
		{
			Instances.Remove(item);
		}
	}

	private static StyleBoxFlat CreateStyle(Color background, Color border, int borderWidth, int cornerRadius = 10, Color? shadow = null, int shadowSize = 0)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Expected O, but got Unknown
		StyleBoxFlat val = new StyleBoxFlat
		{
			BgColor = background,
			BorderColor = border,
			ShadowColor = (Color)(shadow ?? Colors.Transparent),
			ShadowSize = shadowSize
		};
		val.SetBorderWidthAll(borderWidth);
		val.SetCornerRadiusAll(cornerRadius);
		return val;
	}

	private static StyleBoxFlat CreateFocusStyle()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		StyleBoxFlat val = CreateStyle(new Color(1f, 1f, 1f, 0.02f), new Color(1f, 0.88f, 0.62f, 0.95f), 2);
		val.ExpandMarginLeft = 1f;
		val.ExpandMarginTop = 1f;
		val.ExpandMarginRight = 1f;
		val.ExpandMarginBottom = 1f;
		return val;
	}

}
