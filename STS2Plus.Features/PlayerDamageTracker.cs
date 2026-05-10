using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Screens.Capstones;
using MegaCrit.Sts2.Core.Nodes.Screens.Overlays;
using STS2Plus.Config;
using STS2Plus.Reflection;
using STS2Plus.Ui;

namespace STS2Plus.Features;

internal static class PlayerDamageTracker
{
	private static PlayerDamageShieldBadge? badge;

	private static Font? cachedFont;

	private static Node? badgeParent;

	public static void Recalculate()
	{
		try
		{
			ModEntry.Verbose("PlayerDamageTracker: recalculate started");
			if (!ConfigManager.Current.PlayerCombatShieldEnabled)
			{
				Hide();
				return;
			}
			NCapstoneContainer instance = NCapstoneContainer.Instance;
			if (instance != null && instance.InUse)
			{
				Hide();
				return;
			}
			NOverlayStack instance2 = NOverlayStack.Instance;
			if (instance2 != null && instance2.ScreenCount > 0)
			{
				Hide();
				return;
			}
			CombatManager instance3 = CombatManager.Instance;
			CombatState val = ((instance3 != null) ? instance3.DebugOnlyGetState() : null);
			if (val == null || instance3 == null)
			{
				Hide();
				return;
			}
			Creature val2 = ((IEnumerable<Creature>)val.PlayerCreatures).FirstOrDefault((Func<Creature, bool>)((Creature creature) => creature != null && !creature.IsDead && creature.IsPlayer && !creature.IsPet && LocalContext.IsMe(creature)));
			if (val2 == null)
			{
				val2 = ((IEnumerable<Creature>)val.PlayerCreatures).FirstOrDefault((Func<Creature, bool>)((Creature creature) => creature != null && !creature.IsDead && creature.IsPlayer && !creature.IsPet && GameReflection.IsLocalPlayerCreature(creature)));
			}
			if (val2 == null)
			{
				Hide();
				return;
			}
			int num = 0;
			foreach (Creature enemy in val.Enemies)
			{
				if (enemy.IsDead || enemy.Monster == null)
				{
					continue;
				}
				foreach (AbstractIntent intent in enemy.Monster.NextMove.Intents)
				{
					AttackIntent val3 = (AttackIntent)(object)((intent is AttackIntent) ? intent : null);
					if (val3 != null)
					{
						int totalDamage = val3.GetTotalDamage((IEnumerable<Creature>)(object)new Creature[1] { val2 }, enemy);
						if (totalDamage > 0)
						{
							num += totalDamage;
						}
					}
				}
			}
			int num2 = Math.Max(0, val2.Block);
			int ostyHp = 0;
			foreach (Creature pet in val2.Pets)
			{
				if (pet != null && !pet.IsDead && pet.IsPet)
				{
					ostyHp += Math.Max(0, pet.CurrentHp);
				}
			}
			if (num <= 0 && num2 <= 0 && ostyHp <= 0)
			{
				Hide();
			}
			else
			{
				ShowLabel(val2, num, num2, ostyHp);
			}
		}
		catch (Exception value)
		{
			ModEntry.Logger.Error($"PlayerDamageTracker.Recalculate failed: {value}", 1);
		}
	}

	public static void Hide()
	{
		if (badge != null && GodotObject.IsInstanceValid((GodotObject)(object)badge))
		{
			((CanvasItem)badge).Visible = false;
		}
	}

	private static void ShowLabel(Creature creature, int incomingDamage, int block, int ostyHp = 0)
	{
		NCombatRoom instance = NCombatRoom.Instance;
		if (instance == null)
		{
			return;
		}
		if (badgeParent != instance && badge != null && GodotObject.IsInstanceValid((GodotObject)(object)badge))
		{
			((Node)badge).QueueFree();
			badge = null;
		}
		badgeParent = (Node?)(object)instance;
		if (badge == null)
		{
			badge = CreateBadge((Node)(object)instance);
		}
		if (!GodotObject.IsInstanceValid((GodotObject)(object)badge))
		{
			badge = CreateBadge((Node)(object)instance);
		}
		if (badge != null)
		{
			NCreature creatureNode = instance.GetCreatureNode(creature);
			if (creatureNode == null)
			{
				((CanvasItem)badge).Visible = false;
				return;
			}
			// Factor in Osty (pet) HP as a damage buffer after block
			int totalDefense = block + ostyHp;
			int num = totalDefense - incomingDamage;
			int num2 = Math.Max(0, incomingDamage - totalDefense);
			bool lethal = num2 >= creature.CurrentHp;
			Color accent = ResolveColor(num, lethal);
			Color background = ResolveBackgroundColor(num, lethal);
			string text = ((num > 0) ? $"+{num}" : ((num < 0) ? $"-{Math.Abs(num)}" : "0"));
			badge.ApplyStyle(text, cachedFont, accent, background, lethal);
			((Node2D)badge).GlobalPosition = new Vector2(((Control)creatureNode).GlobalPosition.X - 47f, ((Control)creatureNode).GlobalPosition.Y - 336f);
			((CanvasItem)badge).Visible = true;
		}
	}

	private static PlayerDamageShieldBadge? CreateBadge(Node parent)
	{
		if (cachedFont == null)
		{
			cachedFont = FindGameFont(parent);
		}
		PlayerDamageShieldBadge playerDamageShieldBadge = new PlayerDamageShieldBadge();
		parent.AddChild((Node)(object)playerDamageShieldBadge, false, (Node.InternalMode)0);
		return playerDamageShieldBadge;
	}

	private static Font? FindGameFont(Node root)
	{
		foreach (Node child in root.GetChildren(false))
		{
			NCreature val = (NCreature)(object)((child is NCreature) ? child : null);
			if (val == null || val.IntentContainer == null)
			{
				continue;
			}
			foreach (Node child2 in ((Node)val.IntentContainer).GetChildren(false))
			{
				if (child2 == null)
				{
					continue;
				}
				Node val2 = child2;
				if (1 == 0)
				{
					continue;
				}
				Control nodeOrNull = val2.GetNodeOrNull<Control>((NodePath)"%Value");
				RichTextLabel val3 = (RichTextLabel)(object)((nodeOrNull is RichTextLabel) ? nodeOrNull : null);
				if (val3 != null)
				{
					Font themeFont = ((Control)val3).GetThemeFont((StringName)"normal_font", (StringName)null);
					if (themeFont != null)
					{
						return themeFont;
					}
				}
				Label nodeOrNull2 = val2.GetNodeOrNull<Label>((NodePath)"%Value");
				if (nodeOrNull2 != null)
				{
					Font themeFont2 = ((Control)nodeOrNull2).GetThemeFont((StringName)"font", (StringName)null);
					if (themeFont2 != null)
					{
						return themeFont2;
					}
				}
			}
		}
		return FindFontRecursive(root, 3);
	}

	private static Font? FindFontRecursive(Node node, int depth)
	{
		if (depth <= 0)
		{
			return null;
		}
		foreach (Node child in node.GetChildren(false))
		{
			Label val = (Label)(object)((child is Label) ? child : null);
			if (val != null)
			{
				Font themeFont = ((Control)val).GetThemeFont((StringName)"font", (StringName)null);
				if (themeFont != null)
				{
					return themeFont;
				}
			}
			if (child == null)
			{
				continue;
			}
			Node node2 = child;
			if (true)
			{
				Font val2 = FindFontRecursive(node2, depth - 1);
				if (val2 != null)
				{
					return val2;
				}
			}
		}
		return null;
	}

	private static Color ResolveColor(int netResult, bool lethal)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		if (netResult > 0)
		{
			return new Color(0.42f, 0.82f, 1f, 0.98f);
		}
		if (netResult == 0)
		{
			return new Color(0.86f, 0.94f, 1f, 0.98f);
		}
		int num = Math.Abs(netResult);
		if (lethal || num > 20)
		{
			return new Color(0.92f, 0.18f, 0.18f, 0.98f);
		}
		if (num > 10)
		{
			return new Color(0.72f, 0.4f, 0.94f, 0.98f);
		}
		return new Color(0.98f, 0.8f, 0.24f, 0.98f);
	}

	private static Color ResolveBackgroundColor(int netResult, bool lethal)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		if (netResult > 0)
		{
			return new Color(0.05f, 0.18f, 0.3f, 0.92f);
		}
		if (netResult == 0)
		{
			return new Color(0.08f, 0.12f, 0.18f, 0.88f);
		}
		if (lethal)
		{
			return new Color(0.24f, 0.04f, 0.06f, 0.95f);
		}
		return new Color(0.16f, 0.08f, 0.1f, 0.9f);
	}
}
