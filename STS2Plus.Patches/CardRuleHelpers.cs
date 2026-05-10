using System;
using System.Collections;
using HarmonyLib;
using STS2Plus.Features;
using STS2Plus.Reflection;

namespace STS2Plus.Patches;

internal static class CardRuleHelpers
{
	public static bool TryApplyAttackDefenseBonuses(object? card)
	{
		if (card == null)
		{
			return false;
		}
		decimal attackCardBonus = PlusState.GetAttackCardBonus();
		decimal defenseCardBonus = PlusState.GetDefenseCardBonus();
		if (attackCardBonus <= 0m && defenseCardBonus <= 0m)
		{
			return false;
		}
		if (!AppliedTracker.MarkAttackDefenseCard(card))
		{
			return false;
		}
		return GameReflection.ApplyAttackDefenseBaseBonus(card, attackCardBonus, defenseCardBonus);
	}

	public static void ReapplyBonusesToAllPlayerDecks()
	{
		foreach (object player in GameReflection.GetPlayers())
		{
			ReapplyBonusesToPlayerDeck(player);
		}
	}

	public static void ReapplyBonusesToPlayerDeck(object? player)
	{
		if (player != null)
		{
			decimal attackCardBonus = PlusState.GetAttackCardBonus();
			decimal defenseCardBonus = PlusState.GetDefenseCardBonus();
			if ((!(attackCardBonus <= 0m) || !(defenseCardBonus <= 0m)) && TryApplyToDeck(player))
			{
				TryRecalculateCombatCardValues(player);
			}
		}
	}

	private static bool TryApplyToDeck(object player)
	{
		object pile = AccessTools.Property(player.GetType(), "Deck")?.GetValue(player);
		return TryApplyToPile(pile);
	}

	private static void TryRecalculateCombatCardValues(object player)
	{
		object obj = AccessTools.Property(player.GetType(), "PlayerCombatState")?.GetValue(player);
		if (obj != null)
		{
			AccessTools.Method(obj.GetType(), "RecalculateCardValues", (Type[])null, (Type[])null)?.Invoke(obj, null);
		}
	}

	private static bool TryApplyToPile(object? pile)
	{
		if (pile == null)
		{
			return false;
		}
		if (!(AccessTools.Property(pile.GetType(), "Cards")?.GetValue(pile) is IEnumerable enumerable))
		{
			return false;
		}
		bool flag = false;
		foreach (object item in enumerable)
		{
			flag |= TryApplyAttackDefenseBonuses(item);
		}
		return flag;
	}
}
