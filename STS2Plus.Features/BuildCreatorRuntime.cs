using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using STS2Plus.Patches;
using STS2Plus.Reflection;

namespace STS2Plus.Features;

internal static class BuildCreatorRuntime
{
	public const int DummyHp = 10000;

	public static bool IsSupportedRun(Player? player = null)
	{
		if (player != null)
		{
			IRunState runState = player.RunState;
			if (((runState == null) ? null : ((IPlayerCollection)runState).Players?.Count) > 1)
			{
				return false;
			}
		}
		return !GameReflection.IsMultiplayerRun();
	}

	public static IReadOnlyList<CardModel> GetSelectableCards(Player player)
	{
		return (from card in (from card in player.Character.CardPool.AllCards.Concat(((CardPoolModel)ModelDb.CardPool<ColorlessCardPool>()).AllCards)
				where card.ShouldShowInCardLibrary
				select card).DistinctBy<CardModel, string>((CardModel card) => ((AbstractModel)card).Id.Entry, StringComparer.Ordinal)
			orderby card.Rarity
			select card).ThenBy(delegate(CardModel card)
		{
			CardPoolModel pool = card.Pool;
			return pool != null && pool.IsColorless;
		}).ThenBy<CardModel, string>((CardModel card) => card.Title, StringComparer.CurrentCultureIgnoreCase).ToList();
	}

	public static IReadOnlyList<RelicModel> GetSelectableRelics()
	{
		return (from relic in ModelDb.AllRelics.Where(IsSelectableRelic)
			orderby relic.Rarity
			select relic).ThenBy<RelicModel, string>((RelicModel relic) => relic.Title.GetFormattedText(), StringComparer.CurrentCultureIgnoreCase).ToList();
	}

	public static async Task<bool> ApplyBuildAsync(Player player, IReadOnlyDictionary<string, BuildCreatorCardSelection> selectedCardSelections, IReadOnlyCollection<string> selectedRelicEntries)
	{
		if (!IsSupportedRun(player) || selectedCardSelections.Values.Sum((BuildCreatorCardSelection selection) => selection.TotalCount) <= 0)
		{
			return false;
		}
		ClearPlayerDeck(player);
		await ClearPlayerRelicsAsync(player);
		List<CardModel> cardsToAdd = new List<CardModel>();
		foreach (CardModel canonicalCard in GetSelectableCards(player))
		{
			if (selectedCardSelections.TryGetValue(((AbstractModel)canonicalCard).Id.Entry, out var selection2) && selection2.HasAny)
			{
				for (int index2 = 0; index2 < selection2.BaseCount; index2++)
				{
					cardsToAdd.Add(CreateSelectedCard(player, canonicalCard));
				}
				for (int index = 0; index < selection2.UpgradedCount; index++)
				{
					cardsToAdd.Add(CreateSelectedCard(player, canonicalCard, 1));
				}
			}
		}
		if (cardsToAdd.Count > 0)
		{
			await CardPileCmd.Add((IEnumerable<CardModel>)cardsToAdd, (PileType)6, (CardPilePosition)1, (AbstractModel)null, true);
		}
		HashSet<string> selectedRelicLookup = new HashSet<string>(selectedRelicEntries, StringComparer.Ordinal);
		foreach (RelicModel canonicalRelic in GetSelectableRelics())
		{
			if (selectedRelicLookup.Contains(((AbstractModel)canonicalRelic).Id.Entry))
			{
				RelicModel relic = canonicalRelic.ToMutable();
				relic.FloorAddedToDeck = 1;
				await RelicCmd.Obtain(relic, player, -1);
			}
		}
		CardRuleHelpers.ReapplyBonusesToAllPlayerDecks();
		if (PlusState.IsGlassCannonActive())
		{
			GameReflection.ApplyGlassCannon(player);
			GameReflection.RepairGlassCannonState(player);
		}
		return true;
	}

	public static EncounterModel ReplaceEncounterIfNeeded(EncounterModel encounter, IRunState? runState)
	{
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Expected I4, but got Unknown
		if (!PlusState.IsBuildCreatorActive() || (runState != null && ((IPlayerCollection)runState).Players?.Count > 1) || encounter is BuildCreatorEncounterBase)
		{
			return encounter;
		}
		EnsureEncounterModelsInjected();
		RoomType roomType = encounter.RoomType;
		if (1 == 0)
		{
		}
		EncounterModel result = (EncounterModel)(((int)roomType - 1) switch
		{
			0 => ((EncounterModel)ModelDb.Encounter<BuildCreatorMonsterEncounter>()).ToMutable(), 
			1 => ((EncounterModel)ModelDb.Encounter<BuildCreatorEliteEncounter>()).ToMutable(), 
			2 => ((EncounterModel)ModelDb.Encounter<BuildCreatorBossEncounter>()).ToMutable(), 
			_ => encounter, 
		});
		if (1 == 0)
		{
		}
		return result;
	}

	public static void NormalizeEnemy(Creature creature)
	{
		if (PlusState.IsBuildCreatorActive() && creature != null && creature.IsEnemy)
		{
			creature.SetMaxHpInternal(10000m);
			creature.SetCurrentHpInternal(10000m);
		}
	}

	public static void RestoreEnemyHpForTurn(CombatState? combatState)
	{
		if (!PlusState.IsBuildCreatorActive() || combatState == null)
		{
			return;
		}
		foreach (Creature item in combatState.Enemies.Where((Creature enemy) => enemy != null && enemy.IsAlive))
		{
			item.SetMaxHpInternal(10000m);
			item.SetCurrentHpInternal(10000m);
		}
	}

	private static void ClearPlayerDeck(Player player)
	{
		foreach (CardModel item in player.Deck.Cards.ToList())
		{
			player.Deck.RemoveInternal(item, true);
			item.RemoveFromState();
		}
	}

	private static async Task ClearPlayerRelicsAsync(Player player)
	{
		foreach (RelicModel relic in player.Relics.ToList())
		{
			await RelicCmd.Remove(relic);
		}
	}

	private static CardModel CreateSelectedCard(Player player, CardModel canonicalCard, int upgradeCount = 0)
	{
		CardModel val = ((ICardScope)player.RunState).CreateCard(canonicalCard, player);
		val.FloorAddedToDeck = 1;
		for (int i = 0; i < upgradeCount; i++)
		{
			if (!val.IsUpgradable)
			{
				break;
			}
			val.UpgradeInternal();
			val.FinalizeUpgradeInternal();
		}
		return val;
	}

	private static bool IsSelectableRelic(RelicModel relic)
	{
		Type type = ((object)relic).GetType();
		string name = type.Name;
		string text = type.FullName ?? string.Empty;
		return !name.StartsWith("Fake", StringComparison.Ordinal) && !name.Contains("Deprecated", StringComparison.Ordinal) && !text.Contains(".Mocks.", StringComparison.Ordinal);
	}

	private static void EnsureEncounterModelsInjected()
	{
		EnsureEncounterModelInjected(typeof(BuildCreatorMonsterEncounter));
		EnsureEncounterModelInjected(typeof(BuildCreatorEliteEncounter));
		EnsureEncounterModelInjected(typeof(BuildCreatorBossEncounter));
	}

	private static void EnsureEncounterModelInjected(Type encounterType)
	{
		if (!ModelDb.Contains(encounterType))
		{
			ModelDb.Inject(encounterType);
		}
	}
}
