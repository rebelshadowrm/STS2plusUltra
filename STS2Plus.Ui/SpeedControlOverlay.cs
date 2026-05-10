using System;
using System.Collections.Generic;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Screens.Overlays;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using STS2Plus.Config;
using STS2Plus.Reflection;

namespace STS2Plus.Ui;

internal static class SpeedControlOverlay
{
	private sealed class SpeedControlManager : Node
	{
		public override void _Process(double delta)
		{
			MainLoop mainLoop = Engine.GetMainLoop();
			MainLoop obj = ((mainLoop is SceneTree) ? mainLoop : null);
			Window val = ((obj != null) ? ((SceneTree)obj).Root : null);
			CanvasLayer val2 = ((val != null) ? ((Node)val).GetNodeOrNull<CanvasLayer>((NodePath)"STS2PlusSpeedLayer") : null);
			if (val2 != null)
			{
				bool flag = ShouldBeVisible();
				if (val2.Visible != flag)
				{
					val2.Visible = flag;
				}
				if (flag)
				{
					SyncLayout(val2);
					RefreshButtons();
				}
			}
			float num = (IsCombatSpeedActive() ? selectedSpeed : 1f);
			if (!Mathf.IsEqualApprox(Engine.TimeScale, (double)num))
			{
				Engine.TimeScale = num;
			}
		}
	}

	private sealed class SpeedGlyph : Control
	{
		private const float TriangleWidth = 18f;

		private const float TriangleHeight = 18f;

		private const float TriangleStep = 5f;

		public int Level { get; set; } = 1;

		public bool Active { get; set; }

		public override void _Draw()
		{
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			Color fill = (Active ? new Color(1f, 0.7f, 0.22f, 1f) : new Color(0.97f, 0.6f, 0.17f, 1f));
			float centerY = Mathf.Floor(((Control)this).Size.Y * 0.5f + 0.5f);
			float num = 18f + (float)(Mathf.Max(Level, 1) - 1) * 5f;
			float num2 = (((Control)this).Size.X - num) * 0.5f;
			for (int i = 0; i < Level; i++)
			{
				DrawTriangle(num2 + (float)i * 5f, centerY, fill);
			}
		}

		private void DrawTriangle(float leftX, float centerY, Color fill)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			Vector2[] array = (Vector2[])(object)new Vector2[3]
			{
				new Vector2(leftX, centerY - 9f),
				new Vector2(leftX + 18f, centerY),
				new Vector2(leftX, centerY + 9f)
			};
			Vector2[] array2 = (Vector2[])(object)new Vector2[3]
			{
				new Vector2(leftX + 3f, centerY - 6f),
				new Vector2(leftX + 12f, centerY),
				new Vector2(leftX + 3f, centerY + 6f)
			};
			((CanvasItem)this).DrawColoredPolygon(array, Colors.Black, (Vector2[])null, (Texture2D)null);
			((CanvasItem)this).DrawColoredPolygon(array2, fill, (Vector2[])null, (Texture2D)null);
		}
	}

	private const string LayerName = "STS2PlusSpeedLayer";

	private const string RootName = "STS2PlusSpeedButtons";

	private const string ManagerName = "STS2PlusSpeedManager";

	private static readonly Vector2 RowSize = new Vector2(228f, 42f);

	private const float TopRightWidth = 228f;

	private const float TopRightRightMargin = 148f;

	private const float TopRightTopMargin = 86f;

	private static float selectedSpeed = 1f;

	private static bool mainMenuVisible = true;

	private static bool gameplaySpeedSuspended;

	public static float SelectedSpeed => selectedSpeed;

	public static void Show()
	{
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		if (!ConfigManager.Current.SpeedControlEnabled)
		{
			Hide(resetSpeed: true);
			return;
		}
		MainLoop mainLoop = Engine.GetMainLoop();
		MainLoop obj = ((mainLoop is SceneTree) ? mainLoop : null);
		Window val = ((obj != null) ? ((SceneTree)obj).Root : null);
		if (val != null)
		{
			CanvasLayer val2 = ((Node)val).GetNodeOrNull<CanvasLayer>((NodePath)"STS2PlusSpeedLayer");
			if (val2 == null || !GodotObject.IsInstanceValid((GodotObject)(object)val2))
			{
				val2 = BuildLayer();
				((GodotObject)val).CallDeferred(Node.MethodName.AddChild, (Variant[])(object)new Variant[1] { (Variant)((GodotObject)(object)val2) });
			}
			EnsureManager((Node)(object)val);
			SyncLayout(val2);
			RefreshButtons();
			mainMenuVisible = false;
			val2.Visible = ShouldBeVisible();
		}
	}

	public static void Hide(bool resetSpeed = false)
	{
		MainLoop mainLoop = Engine.GetMainLoop();
		MainLoop obj = ((mainLoop is SceneTree) ? mainLoop : null);
		Window val = ((obj != null) ? ((SceneTree)obj).Root : null);
		if (val != null)
		{
			CanvasLayer nodeOrNull = ((Node)val).GetNodeOrNull<CanvasLayer>((NodePath)"STS2PlusSpeedLayer");
			if (nodeOrNull != null)
			{
				nodeOrNull.Hide();
			}
		}
		if (resetSpeed)
		{
			selectedSpeed = 1f;
		}
		gameplaySpeedSuspended = false;
		if (!Mathf.IsEqualApprox(Engine.TimeScale, 1.0))
		{
			Engine.TimeScale = 1.0;
		}
	}

	public static void SuspendGameplaySpeed(bool suspended)
	{
		gameplaySpeedSuspended = suspended;
		if (suspended && !Mathf.IsEqualApprox(Engine.TimeScale, 1.0))
		{
			Engine.TimeScale = 1.0;
		}
	}

	public static bool IsCombatSpeedActive()
	{
		if (!ConfigManager.Current.SpeedControlEnabled)
		{
			return false;
		}
		if (gameplaySpeedSuspended)
		{
			return false;
		}
		if (selectedSpeed <= 1f)
		{
			return false;
		}
		RunManager instance = RunManager.Instance;
		object obj;
		if (instance == null)
		{
			obj = null;
		}
		else
		{
			RunState obj2 = instance.DebugOnlyGetState();
			obj = ((obj2 != null) ? obj2.CurrentRoom : null);
		}
		AbstractRoom val = (AbstractRoom)obj;
		try
		{
			NCombatRoom instance2 = NCombatRoom.Instance;
			return val is CombatRoom && instance2 != null && !((GodotObject)instance2).IsQueuedForDeletion();
		}
		catch
		{
			return false;
		}
	}

	public static void SetMainMenuVisible(bool visible)
	{
		mainMenuVisible = visible;
	}

	private static bool ShouldBeVisible()
	{
		if (!ConfigManager.Current.SpeedControlEnabled)
		{
			return false;
		}
		if (!IsAllowedGameplayScreen())
		{
			return false;
		}
		if (IsBlockingUiOpen())
		{
			return false;
		}
		return true;
	}

	private static bool IsAllowedGameplayScreen()
	{
		try
		{
			NCombatRoom instance = NCombatRoom.Instance;
			if (instance != null && GodotObject.IsInstanceValid((GodotObject)(object)instance) && ((CanvasItem)instance).IsVisibleInTree())
			{
				return true;
			}
		}
		catch
		{
		}
		Type type = RuntimeTypeResolver.FindType("MegaCrit.Sts2.Core.Nodes.Screens.Map.NMapScreen") ?? RuntimeTypeResolver.FindTypeByName("NMapScreen");
		Control val;
		try
		{
			object? obj2 = AccessTools.Property(type, "Instance")?.GetValue(null);
			val = (Control)((obj2 is Control) ? obj2 : null);
		}
		catch
		{
			return false;
		}
		if (val != null && GodotObject.IsInstanceValid((GodotObject)(object)val))
		{
			bool isOpen = (AccessTools.Property(type, "IsOpen")?.GetValue(val) as bool?).GetValueOrDefault();
			if (isOpen)
			{
				return true;
			}
		}
		// Fallback: allow speed controls whenever a run is active
		try
		{
			RunManager runManager = RunManager.Instance;
			if (runManager != null && runManager.DebugOnlyGetState() != null)
			{
				return true;
			}
		}
		catch
		{
		}
		return false;
	}

	private static bool IsBlockingUiOpen()
	{
		MainLoop mainLoop = Engine.GetMainLoop();
		SceneTree val = (SceneTree)(object)((mainLoop is SceneTree) ? mainLoop : null);
		if (val != null && val.Paused)
		{
			return true;
		}
		NOverlayStack val2 = null;
		try
		{
			val2 = NOverlayStack.Instance;
		}
		catch
		{
			return false;
		}
		if (val2 != null && val2.ScreenCount > 0)
		{
			return true;
		}
		return false;
	}

	public static void ApplyTweenSpeed(Tween tween)
	{
		if (tween != null)
		{
			tween.SetSpeedScale(IsCombatSpeedActive() ? selectedSpeed : 1f);
		}
	}

	private static CanvasLayer BuildLayer()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Expected O, but got Unknown
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Expected O, but got Unknown
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Expected O, but got Unknown
		CanvasLayer val = new CanvasLayer
		{
			Name = (StringName)"STS2PlusSpeedLayer",
			Layer = 10
		};
		Control val2 = new Control
		{
			Name = (StringName)"STS2PlusSpeedButtons",
			Size = RowSize,
			AnchorLeft = 1f,
			AnchorRight = 1f,
			AnchorTop = 0f,
			AnchorBottom = 0f,
			OffsetLeft = -376f,
			OffsetTop = 86f,
			OffsetRight = -148f,
			OffsetBottom = 86f + RowSize.Y,
			MouseFilter = (Control.MouseFilterEnum)0
		};
		Panel val3 = new Panel
		{
			Name = (StringName)"Frame",
			MouseFilter = (Control.MouseFilterEnum)2,
			AnchorLeft = 0f,
			AnchorTop = 0f,
			AnchorRight = 1f,
			AnchorBottom = 1f,
			OffsetLeft = 0f,
			OffsetTop = 0f,
			OffsetRight = 0f,
			OffsetBottom = 0f
		};
		((Control)val3).AddThemeStyleboxOverride((StringName)"panel", (StyleBox)(object)CreateFrameStyle());
		HBoxContainer val4 = new HBoxContainer
		{
			Alignment = (BoxContainer.AlignmentMode)1,
			MouseFilter = (Control.MouseFilterEnum)0,
			AnchorLeft = 0f,
			AnchorTop = 0f,
			AnchorRight = 1f,
			AnchorBottom = 1f,
			OffsetLeft = 6f,
			OffsetTop = 6f,
			OffsetRight = -6f,
			OffsetBottom = -6f
		};
		((Control)val4).AddThemeConstantOverride((StringName)"separation", 4);
		((Node)val4).AddChild((Node)(object)CreateButton(1f, 1), false, (Node.InternalMode)0);
		((Node)val4).AddChild((Node)(object)CreateButton(2f, 2), false, (Node.InternalMode)0);
		((Node)val4).AddChild((Node)(object)CreateButton(4f, 3), false, (Node.InternalMode)0);
		((Node)val3).AddChild((Node)(object)val4, false, (Node.InternalMode)0);
		((Node)val2).AddChild((Node)(object)val3, false, (Node.InternalMode)0);
		((Node)val).AddChild((Node)(object)val2, false, (Node.InternalMode)0);
		return val;
	}

	private static Button CreateButton(float speed, int level)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Expected O, but got Unknown
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		Button val = new Button
		{
			Text = string.Empty,
			FocusMode = (Control.FocusModeEnum)0,
			CustomMinimumSize = new Vector2(0f, 28f),
			SizeFlagsHorizontal = (Control.SizeFlags)3,
			MouseDefaultCursorShape = (Control.CursorShape)2
		};
		((GodotObject)val).SetMeta((StringName)"sts2plus_speed", (Variant)(speed));
		SpeedGlyph obj = new SpeedGlyph
		{
			Level = level
		};
		((Control)obj).MouseFilter = (Control.MouseFilterEnum)2;
		((Control)obj).AnchorLeft = 0f;
		((Control)obj).AnchorTop = 0f;
		((Control)obj).AnchorRight = 1f;
		((Control)obj).AnchorBottom = 1f;
		((Control)obj).OffsetLeft = 0f;
		((Control)obj).OffsetTop = 0f;
		((Control)obj).OffsetRight = 0f;
		((Control)obj).OffsetBottom = 0f;
		((Node)val).AddChild((Node)(object)obj, false, (Node.InternalMode)0);
		((BaseButton)val).Pressed += delegate
		{
			selectedSpeed = speed;
			RefreshButtons();
			if (IsCombatSpeedActive())
			{
				Engine.TimeScale = selectedSpeed;
			}
		};
		return val;
	}

	private static void EnsureManager(Node root)
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		SpeedControlManager nodeOrNull = root.GetNodeOrNull<SpeedControlManager>((NodePath)"STS2PlusSpeedManager");
		if (nodeOrNull == null || !GodotObject.IsInstanceValid((GodotObject)(object)nodeOrNull))
		{
			SpeedControlManager speedControlManager = new SpeedControlManager();
			((Node)speedControlManager).Name = (StringName)"STS2PlusSpeedManager";
			nodeOrNull = speedControlManager;
			((GodotObject)root).CallDeferred(Node.MethodName.AddChild, (Variant[])(object)new Variant[1] { (Variant)((GodotObject)(object)nodeOrNull) });
		}
	}

	private static void SyncLayout(CanvasLayer layer)
	{
		Control nodeOrNull = ((Node)layer).GetNodeOrNull<Control>((NodePath)"STS2PlusSpeedButtons");
		if (nodeOrNull != null)
		{
			nodeOrNull.AnchorLeft = 1f;
			nodeOrNull.AnchorRight = 1f;
			nodeOrNull.AnchorTop = 0f;
			nodeOrNull.AnchorBottom = 0f;
			nodeOrNull.OffsetLeft = -376f;
			nodeOrNull.OffsetTop = 86f;
			nodeOrNull.OffsetRight = -148f;
			nodeOrNull.OffsetBottom = 86f + RowSize.Y;
		}
	}

	private static void RefreshButtons()
	{
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		MainLoop mainLoop = Engine.GetMainLoop();
		MainLoop obj = ((mainLoop is SceneTree) ? mainLoop : null);
		Window val = ((obj != null) ? ((SceneTree)obj).Root : null);
		CanvasLayer val2 = ((val != null) ? ((Node)val).GetNodeOrNull<CanvasLayer>((NodePath)"STS2PlusSpeedLayer") : null);
		if (val2 == null)
		{
			return;
		}
		foreach (Node item in ((Node)val2).FindChildren("*", "Button", true, false))
		{
			Button val3 = (Button)(object)((item is Button) ? item : null);
			if (val3 != null)
			{
				float num = (float)((GodotObject)val3).GetMeta((StringName)"sts2plus_speed", (Variant)(1f));
				bool flag = Mathf.IsEqualApprox(num, selectedSpeed);
				((Control)val3).AddThemeStyleboxOverride((StringName)"normal", (StyleBox)(object)(flag ? CreateButtonStyle(new Color(0.36f, 0.24f, 0.09f, 0.98f)) : CreateButtonStyle(new Color(0.1f, 0.15f, 0.22f, 0.55f))));
				((Control)val3).AddThemeStyleboxOverride((StringName)"hover", (StyleBox)(object)CreateButtonStyle(flag ? new Color(0.4f, 0.27f, 0.11f, 0.98f) : new Color(0.16f, 0.22f, 0.31f, 0.78f)));
				((Control)val3).AddThemeStyleboxOverride((StringName)"pressed", (StyleBox)(object)CreateButtonStyle(new Color(0.36f, 0.24f, 0.09f, 0.98f)));
				((Control)val3).AddThemeStyleboxOverride((StringName)"focus", (StyleBox)(object)CreateButtonStyle(flag ? new Color(0.36f, 0.24f, 0.09f, 0.98f) : new Color(0.14f, 0.19f, 0.27f, 0.65f)));
				if (((Node)val3).GetChildCount(false) > 0 && ((Node)val3).GetChild(0, false) is SpeedGlyph speedGlyph)
				{
					speedGlyph.Active = flag;
					((CanvasItem)speedGlyph).QueueRedraw();
				}
			}
		}
	}

	private static StyleBoxFlat CreateFrameStyle()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Expected O, but got Unknown
		return new StyleBoxFlat
		{
			BgColor = new Color(0.03f, 0.05f, 0.08f, 0.92f),
			BorderColor = new Color(0.34f, 0.47f, 0.61f, 0.95f),
			CornerRadiusTopLeft = 14,
			CornerRadiusTopRight = 14,
			CornerRadiusBottomLeft = 14,
			CornerRadiusBottomRight = 14,
			BorderWidthTop = 2,
			BorderWidthRight = 2,
			BorderWidthBottom = 2,
			BorderWidthLeft = 2,
			ContentMarginTop = 4f,
			ContentMarginRight = 4f,
			ContentMarginBottom = 4f,
			ContentMarginLeft = 4f
		};
	}

	private static StyleBoxFlat CreateButtonStyle(Color bg)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Expected O, but got Unknown
		return new StyleBoxFlat
		{
			BgColor = bg,
			CornerRadiusTopLeft = 9,
			CornerRadiusTopRight = 9,
			CornerRadiusBottomLeft = 9,
			CornerRadiusBottomRight = 9,
			BorderWidthTop = 0,
			BorderWidthRight = 0,
			BorderWidthBottom = 0,
			BorderWidthLeft = 0,
			ContentMarginTop = 3f,
			ContentMarginRight = 8f,
			ContentMarginBottom = 3f,
			ContentMarginLeft = 8f
		};
	}
}
