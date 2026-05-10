using System.Collections.Generic;
using Godot;

namespace STS2Plus.Ui;

[ScriptPath("res://src/Ui/PlayerDamageShieldBadge.cs")]
internal sealed class PlayerDamageShieldBadge : Node2D
{
	private static readonly Vector2 BadgeSize = new Vector2(112f, 82f);

	private readonly Polygon2D shadowPolygon;

	private readonly Polygon2D bodyOuterPolygon;

	private readonly Polygon2D bodyInnerPolygon;

	private readonly Polygon2D gemPolygon;

	private readonly Label valueLabel;

	public PlayerDamageShieldBadge()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Expected O, but got Unknown
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Expected O, but got Unknown
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Expected O, but got Unknown
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Expected O, but got Unknown
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Expected O, but got Unknown
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		((CanvasItem)this).Visible = false;
		((CanvasItem)this).ZIndex = 100;
		shadowPolygon = new Polygon2D();
		((Node)this).AddChild((Node)(object)shadowPolygon, false, (Node.InternalMode)0);
		bodyOuterPolygon = new Polygon2D();
		((Node)this).AddChild((Node)(object)bodyOuterPolygon, false, (Node.InternalMode)0);
		bodyInnerPolygon = new Polygon2D();
		((Node)this).AddChild((Node)(object)bodyInnerPolygon, false, (Node.InternalMode)0);
		gemPolygon = new Polygon2D();
		((Node)this).AddChild((Node)(object)gemPolygon, false, (Node.InternalMode)0);
		valueLabel = new Label
		{
			HorizontalAlignment = (HorizontalAlignment)1,
			VerticalAlignment = (VerticalAlignment)1,
			Position = new Vector2(20f, 27f),
			Size = new Vector2(BadgeSize.X - 40f, 30f)
		};
		((Control)valueLabel).AddThemeFontSizeOverride((StringName)"font_size", 20);
		((Control)valueLabel).AddThemeConstantOverride((StringName)"outline_size", 5);
		((Control)valueLabel).AddThemeColorOverride((StringName)"font_outline_color", new Color(0.03f, 0.03f, 0.05f, 1f));
		((Node)this).AddChild((Node)(object)valueLabel, false, (Node.InternalMode)0);
		BuildGeometry();
	}

	public void ApplyStyle(string text, Font? font, Color accent, Color background, bool lethal)
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		if (font != null)
		{
			((Control)valueLabel).AddThemeFontOverride((StringName)"font", font);
		}
		valueLabel.Text = text;
		((Control)valueLabel).AddThemeColorOverride((StringName)"font_color", accent.Lightened(0.12f));
		((Control)valueLabel).AddThemeFontSizeOverride((StringName)"font_size", lethal ? 24 : 20);
		bodyOuterPolygon.Color = accent;
		bodyInnerPolygon.Color = background;
		shadowPolygon.Color = new Color(0f, 0f, 0f, 0.26f);
		gemPolygon.Color = new Color(1f, 0.55f, 0.14f, 0.98f);
		((Node2D)this).Scale = (Vector2)(lethal ? new Vector2(1.12f, 1.12f) : Vector2.One);
	}

	private void BuildGeometry()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		float x = BadgeSize.X;
		float y = BadgeSize.Y;
		Vector2[] array = (Vector2[])(object)new Vector2[9]
		{
			new Vector2(35f, 18f),
			new Vector2(77f, 18f),
			new Vector2(74f, 30f),
			new Vector2(77f, 33f),
			new Vector2(70f, 56f),
			new Vector2(x * 0.5f, y - 5f),
			new Vector2(42f, 56f),
			new Vector2(35f, 33f),
			new Vector2(38f, 30f)
		};
		bodyOuterPolygon.Polygon = array;
		bodyInnerPolygon.Polygon = Inset(array, 5f);
		shadowPolygon.Polygon = Offset(array, new Vector2(0f, 5f));
		Vector2 val = default(Vector2);
		val = new Vector2(x * 0.5f, 20f);
		gemPolygon.Polygon = (Vector2[])(object)new Vector2[8]
		{
			val + new Vector2(0f, -11f),
			val + new Vector2(3.5f, -3.5f),
			val + new Vector2(11f, 0f),
			val + new Vector2(3.5f, 3.5f),
			val + new Vector2(0f, 11f),
			val + new Vector2(-3.5f, 3.5f),
			val + new Vector2(-11f, 0f),
			val + new Vector2(-3.5f, -3.5f)
		};
	}

	private static Vector2[] Offset(Vector2[] points, Vector2 offset)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		Vector2[] array = (Vector2[])(object)new Vector2[points.Length];
		for (int i = 0; i < points.Length; i++)
		{
			array[i] = points[i] + offset;
		}
		return array;
	}

	private static Vector2[] Inset(Vector2[] points, float amount)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = Vector2.Zero;
		foreach (Vector2 val2 in points)
		{
			val += val2;
		}
		val /= (float)points.Length;
		Vector2[] array = (Vector2[])(object)new Vector2[points.Length];
		for (int j = 0; j < points.Length; j++)
		{
			Vector2 val3 = val - points[j];
			Vector2 val4 = val3.Normalized();
			array[j] = points[j] + val4 * amount;
		}
		return array;
	}

}
