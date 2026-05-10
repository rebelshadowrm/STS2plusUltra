using Godot;
using STS2Plus.Patches;
using STS2Plus.Reflection;

namespace STS2Plus.Ui;

internal static class EndlessDebugOverlay
{
	private sealed class EndlessDebugOverlayManager : Node
	{
		public override void _Process(double delta)
		{
			try
			{
				CanvasLayer? layer = GetLayer();
				if (layer == null)
				{
					Show();
					layer = GetLayer();
				}
				if (layer == null)
				{
					return;
				}
				bool visible = ShouldBeVisible();
				if (layer.Visible != visible)
				{
					layer.Visible = visible;
					ModEntry.Logger.Info(visible ? "Debug HUD show" : "Debug HUD hide", 1);
				}
				lastVisible = visible;
				if (visible)
				{
					RefreshText();
				}
			}
			catch (System.Exception ex)
			{
				DebugToolsRuntime.DisableForSession("debug HUD process failed", ex);
				Hide();
			}
		}
	}

	private const string LayerName = "STS2PlusEndlessDebugLayer";

	private const string RootName = "STS2PlusEndlessDebugRoot";

	private const string LabelName = "STS2PlusEndlessDebugText";

	private const string ManagerName = "STS2PlusEndlessDebugManager";

	private static bool collapsed;

	private static bool loggedEnabled;

	private static bool lastVisible;

	private static string lastAction = "idle";

	public static void Show()
	{
		if (!DebugToolsRuntime.IsEnabledForSession)
		{
			Hide();
			return;
		}
		CanvasLayer? layer = GetLayer();
		if (layer == null)
		{
			Window? root = GetRootWindow();
			if (root == null)
			{
				return;
			}
			layer = BuildLayer();
			ModEntry.Logger.Info("Debug HUD recreated after scene change", 1);
			((GodotObject)root).CallDeferred(Node.MethodName.AddChild, new Variant[1] { (Variant)(GodotObject)layer });
		}
		EnsureManager();
		RefreshText();
		layer.Visible = ShouldBeVisible();
		lastVisible = layer.Visible;
		ModEntry.Logger.Info("Debug HUD show", 1);
		if (!loggedEnabled)
		{
			loggedEnabled = true;
			ModEntry.Logger.Info("STS2Plus endless debug tools enabled. Debug HUD active.", 1);
		}
	}

	public static void Refresh(string source)
	{
		if (!DebugToolsRuntime.IsEnabledForSession)
		{
			Hide();
			return;
		}
		Show();
		RefreshText();
		ModEntry.Logger.Info("Debug HUD refresh: " + source, 1);
	}

	public static void SetLastAction(string action)
	{
		lastAction = string.IsNullOrWhiteSpace(action) ? "idle" : action;
		if (DebugToolsRuntime.IsEnabledForSession)
		{
			RefreshText();
		}
	}

	public static void Hide()
	{
		CanvasLayer? layer = GetLayer();
		if (layer != null)
		{
			layer.Visible = false;
			if (lastVisible)
			{
				ModEntry.Logger.Info("Debug HUD hide", 1);
			}
		}
		lastVisible = false;
	}

	public static void ToggleCollapsed()
	{
		if (!DebugToolsRuntime.IsEnabledForSession)
		{
			return;
		}
		collapsed = !collapsed;
		RefreshText();
		ModEntry.Logger.Info("Debug HUD refresh: toggle-collapsed", 1);
	}

	private static bool ShouldBeVisible()
	{
		if (!DebugToolsRuntime.IsEnabledForSession)
		{
			return false;
		}
		return GameReflection.IsRunActive() || EndlessLoopCoordinator.IsLaunching || EndlessModeWinRunPatch.InWinTransition;
	}

	private static void EnsureManager()
	{
		Window? root = GetRootWindow();
		if (root == null)
		{
			return;
		}
		EndlessDebugOverlayManager? manager = ((Node)root).GetNodeOrNull<EndlessDebugOverlayManager>((NodePath)ManagerName);
		if (manager == null || !GodotObject.IsInstanceValid(manager))
		{
			manager = new EndlessDebugOverlayManager
			{
				Name = (StringName)ManagerName
			};
			((GodotObject)root).CallDeferred(Node.MethodName.AddChild, new Variant[1] { (Variant)(GodotObject)manager });
		}
	}

	private static CanvasLayer? GetLayer()
	{
		Window? root = GetRootWindow();
		return (root == null) ? null : ((Node)root).GetNodeOrNull<CanvasLayer>((NodePath)LayerName);
	}

	private static Window? GetRootWindow()
	{
		MainLoop loop = Engine.GetMainLoop();
		SceneTree? tree = loop as SceneTree;
		return tree?.Root;
	}

	private static CanvasLayer BuildLayer()
	{
		CanvasLayer layer = new CanvasLayer
		{
			Name = (StringName)LayerName,
			Layer = 50
		};
		Control root = new Control
		{
			Name = (StringName)RootName,
			AnchorLeft = 1f,
			AnchorTop = 0f,
			AnchorRight = 1f,
			AnchorBottom = 0f,
			OffsetLeft = -374f,
			OffsetTop = 104f,
			OffsetRight = -14f,
			OffsetBottom = 286f,
			MouseFilter = Control.MouseFilterEnum.Ignore
		};
		Panel panel = new Panel
		{
			Name = (StringName)"Panel",
			AnchorLeft = 0f,
			AnchorTop = 0f,
			AnchorRight = 1f,
			AnchorBottom = 1f,
			OffsetLeft = 0f,
			OffsetTop = 0f,
			OffsetRight = 0f,
			OffsetBottom = 0f,
			MouseFilter = Control.MouseFilterEnum.Ignore
		};
		((Control)panel).AddThemeStyleboxOverride((StringName)"panel", (StyleBox)BuildPanelStyle());
		Label label = new Label
		{
			Name = (StringName)LabelName,
			AnchorLeft = 0f,
			AnchorTop = 0f,
			AnchorRight = 1f,
			AnchorBottom = 1f,
			OffsetLeft = 10f,
			OffsetTop = 8f,
			OffsetRight = -10f,
			OffsetBottom = -8f,
			MouseFilter = Control.MouseFilterEnum.Ignore,
			AutowrapMode = TextServer.AutowrapMode.Off,
			HorizontalAlignment = HorizontalAlignment.Left,
			VerticalAlignment = VerticalAlignment.Top
		};
		((Control)label).AddThemeFontSizeOverride((StringName)"font_size", 14);
		((Control)label).AddThemeColorOverride((StringName)"font_color", new Color(1f, 0.96f, 0.87f, 1f));
		root.AddChild(panel);
		root.AddChild(label);
		layer.AddChild(root);
		return layer;
	}

	private static StyleBoxFlat BuildPanelStyle()
	{
		return new StyleBoxFlat
		{
			BgColor = new Color(0.18f, 0.06f, 0.06f, 0.88f),
			BorderColor = new Color(0.93f, 0.45f, 0.18f, 0.95f),
			CornerRadiusTopLeft = 10,
			CornerRadiusTopRight = 10,
			CornerRadiusBottomLeft = 10,
			CornerRadiusBottomRight = 10,
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

	private static void RefreshText()
	{
		CanvasLayer? layer = GetLayer();
		if (layer == null)
		{
			return;
		}
		Label? label = ((Node)layer).GetNodeOrNull<Label>((NodePath)($"{RootName}/{LabelName}"));
		Control? root = ((Node)layer).GetNodeOrNull<Control>((NodePath)RootName);
		if (label == null || root == null)
		{
			return;
		}
		if (collapsed)
		{
			label.Text = "STS2Plus DEV TOOLS ENABLED - Shift+F10 for keys\nLast: " + lastAction;
			root.OffsetLeft = -414f;
			root.OffsetRight = -14f;
			root.OffsetBottom = 164f;
			return;
		}
		label.Text = "STS2Plus DEV TOOLS ENABLED\nF6  Dump state\nF7  Win current combat\nF8  Prepare endless test / TODO\nF9  Trigger endless loop\nF10 Dump map-sync state\nShift+F10  Collapse/expand HUD\nLast: " + lastAction;
		root.OffsetLeft = -374f;
		root.OffsetRight = -14f;
		root.OffsetBottom = 248f;
	}
}
