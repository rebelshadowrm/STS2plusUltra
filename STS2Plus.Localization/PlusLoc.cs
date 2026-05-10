using System;
using System.Collections.Generic;
using Godot;
using MegaCrit.Sts2.Core.Localization;

namespace STS2Plus.Localization;

internal static class PlusLoc
{
	public const string ModifierTable = "modifiers";

	private static readonly IReadOnlyDictionary<string, string> EnglishTexts = new Dictionary<string, string>(StringComparer.Ordinal)
	{
		["RULE_ATTACK_DEFENSE_TITLE"] = "Attack Defense:",
		["RULE_ATTACK_DEFENSE_DESC"] = "All attack cards gain [color=#ffd54a]+2 damage[/color] and all defense cards gain [color=#ffd54a]+2 block[/color].",
		["RULE_ATTACK_DEFENSE_PLUS_TITLE"] = "Warrior:",
		["RULE_ATTACK_DEFENSE_PLUS_DESC"] = "All attack cards gain [color=#ffd54a]+5 damage[/color] and all defense cards gain [color=#ffd54a]+5 block[/color].",
		["RULE_IRON_SKIN_TITLE"] = "Iron Skin:",
		["RULE_IRON_SKIN_DESC"] = "All attack cards lose [color=#b26bff]2 damage[/color] and all defense cards gain [color=#ffd54a]+5 block[/color].",
		["RULE_GIANT_CREATURES_TITLE"] = "Giant Creatures:",
		["RULE_GIANT_CREATURES_DESC"] = "All creatures you encounter have their HP [color=#b26bff]doubled[/color] and combat gold rewards are increased by [color=#ffd54a]100%[/color].",
		["RULE_HARD_ELITES_TITLE"] = "Hard Elites:",
		["RULE_HARD_ELITES_DESC"] = "Enemies in elite rooms gain [color=#b26bff]50% more HP[/color], elite combat gold rewards are increased by [color=#ffd54a]200%[/color], and elites drop [color=#ffd54a]2 relics[/color].",
		["RULE_ENDLESS_TITLE"] = "Endless Mode:",
		["RULE_ENDLESS_DESC"] = "Your run does not end after Act 3 and instead begins [color=#ffd54a]a new loop[/color].",
		["RULE_GLASS_CANNON_TITLE"] = "Glass Cannon:",
		["RULE_GLASS_CANNON_DESC"] = "Start with [color=#b26bff]1 max HP[/color], gain [color=#ffd54a]100% more gold[/color], gain [color=#ffd54a]+1 max HP[/color] after each combat, attack cards gain [color=#ffd54a]+3 damage[/color], you have [color=#ffd54a]4 energy[/color] each turn, and retain up to [color=#ffd54a]15 block[/color] each turn.",
		["RULE_UNLIMITED_GROWTH_TITLE"] = "Unlimited Growth:",
		["RULE_UNLIMITED_GROWTH_DESC"] = "Cards can be upgraded [color=#ffd54a]repeatedly[/color], and each upgrade applies [color=#ffd54a]its normal upgrade again[/color].",
		["RULE_SANDBOX_TITLE"] = "Sandbox:",
		["RULE_SANDBOX_DESC"] = "All enemies start every phase with [color=#ffd54a]1 HP[/color].",
		["RULE_BUILD_CREATOR_TITLE"] = "Build Creator:",
		["RULE_BUILD_CREATOR_DESC"] = "Build your own start: choose any number of cards from your character's pool, add upgraded copies, choose relics, and fight a single immortal dummy.",
		["BUILD_CREATOR_TITLE"] = "Build Creator",
		["BUILD_CREATOR_SUBTITLE"] = "Adjust your starting deck and relics before the run begins. Card copies and upgraded copies are unlimited; relics are unique picks.",
		["BUILD_CREATOR_TAB_CARDS"] = "Cards",
		["BUILD_CREATOR_TAB_RELICS"] = "Relics",
		["BUILD_CREATOR_SEARCH_CARDS"] = "Search cards",
		["BUILD_CREATOR_SEARCH_RELICS"] = "Search relics",
		["BUILD_CREATOR_SELECTED_CARDS"] = "Selected Cards",
		["BUILD_CREATOR_SELECTED_RELICS"] = "Selected Relics",
		["BUILD_CREATOR_CARD_BASE"] = "Base",
		["BUILD_CREATOR_CARD_UPGRADED"] = "Upgraded",
		["BUILD_CREATOR_APPLY"] = "Apply Build",
		["BUILD_CREATOR_ADD"] = "Add",
		["BUILD_CREATOR_REMOVE"] = "Remove",
		["BUILD_CREATOR_EMPTY_CARDS"] = "No cards selected.",
		["BUILD_CREATOR_EMPTY_RELICS"] = "No relics selected.",
		["BUILD_CREATOR_NO_RESULTS"] = "No results.",
		["BUILD_CREATOR_READY"] = "Build ready. Applying replaces your starter deck and relics.",
		["BUILD_CREATOR_NEED_CARD"] = "Select at least one card to continue.",
		["BUILD_CREATOR_APPLY_FAILED"] = "Build could not be applied.",
		["BUILD_CREATOR_CARD_COUNT_SUFFIX"] = "cards",
		["BUILD_CREATOR_RELIC_COUNT_SUFFIX"] = "relics",
		["ROUTE_ADVISOR_TITLE"] = "Route Advisor",
		["ROUTE_ADVISOR_SAFE"] = "Safe:",
		["ROUTE_ADVISOR_AGGRESSIVE"] = "Aggro:",
		["RELIC_DRAWER_LABEL"] = "Relics",
		["RELIC_DRAWER_ALL"] = "All Relics",
		["RELIC_DRAWER_EMPTY"] = "No relics"
	};

	private static readonly IReadOnlyDictionary<string, string> TurkishTexts = new Dictionary<string, string>(StringComparer.Ordinal)
	{
		["RULE_ATTACK_DEFENSE_TITLE"] = "Saldir Savun:",
		["RULE_ATTACK_DEFENSE_DESC"] = "Butun saldiri kartlari [color=#ffd54a]+2 hasar[/color], butun savunma kartlari [color=#ffd54a]+2 blok[/color] kazanir.",
		["RULE_ATTACK_DEFENSE_PLUS_TITLE"] = "Savasci:",
		["RULE_ATTACK_DEFENSE_PLUS_DESC"] = "Butun saldiri kartlari [color=#ffd54a]+5 hasar[/color], butun savunma kartlari [color=#ffd54a]+5 blok[/color] kazanir.",
		["RULE_IRON_SKIN_TITLE"] = "Demir Deri:",
		["RULE_IRON_SKIN_DESC"] = "Butun saldiri kartlari [color=#b26bff]-2 hasar[/color] alir, butun savunma kartlari [color=#ffd54a]+5 blok[/color] kazanir.",
		["RULE_GIANT_CREATURES_TITLE"] = "Dev Yaratiklar:",
		["RULE_GIANT_CREATURES_DESC"] = "Karsilastiginiz butun yaratiklarin cani [color=#b26bff]2 katina cikar[/color] ve savas altin odulleri [color=#ffd54a]%100 artar[/color].",
		["RULE_HARD_ELITES_TITLE"] = "Sert Elitler:",
		["RULE_HARD_ELITES_DESC"] = "Elit odalarindaki dusmanlar [color=#b26bff]%50 daha fazla can[/color] kazanir, elit savas altin odulleri [color=#ffd54a]%200 artar[/color] ve [color=#ffd54a]2 kalinti[/color] dusurur.",
		["RULE_ENDLESS_TITLE"] = "Sonsuz Oyun:",
		["RULE_ENDLESS_DESC"] = "3. sahneden sonra run bitmez, [color=#ffd54a]yeni bir dongu baslar[/color].",
		["RULE_GLASS_CANNON_TITLE"] = "Citkirildim:",
		["RULE_GLASS_CANNON_DESC"] = "Oyuna [color=#b26bff]1 maksimum can[/color] ile baslarsin, [color=#ffd54a]%100 daha fazla altin[/color] kazanirsin, her savas sonunda [color=#ffd54a]+1 maksimum can[/color] kazanirsin, saldiri kartlari [color=#ffd54a]+3 hasar[/color] alir, her tur [color=#ffd54a]4 enerjiye[/color] sahip olursun ve her tur en fazla [color=#ffd54a]15 blok[/color] saklarsin.",
		["RULE_UNLIMITED_GROWTH_TITLE"] = "Sinirsiz Gelisim:",
		["RULE_UNLIMITED_GROWTH_DESC"] = "Kartlar [color=#ffd54a]tekrar tekrar[/color] gelistirilebilir ve her gelistirme [color=#ffd54a]normal gelistirme etkisini yeniden uygular[/color].",
		["RULE_SANDBOX_TITLE"] = "Sandbox:",
		["RULE_SANDBOX_DESC"] = "Tum dusmanlar her phase'e [color=#ffd54a]1 can[/color] ile baslar.",
		["RULE_BUILD_CREATOR_TITLE"] = "Deste Üretici:",
		["RULE_BUILD_CREATOR_DESC"] = "Karakterinin havuzundan istedigin kadar kart sec, gelistirilmis kopyalar ekle, tum kalintilar arasindan secim yap ve tek bir olumsuz dummy ile savas.",
		["BUILD_CREATOR_TITLE"] = "Deste Üretici",
		["BUILD_CREATOR_SUBTITLE"] = "Run baslamadan once baslangic desteni ve kalintilarini ayarla. Kart ve gelistirilmis kart kopyalari sinirsizdir; kalintilar tekil secimdir.",
		["BUILD_CREATOR_TAB_CARDS"] = "Kartlar",
		["BUILD_CREATOR_TAB_RELICS"] = "Kalintilar",
		["BUILD_CREATOR_SEARCH_CARDS"] = "Kart ara",
		["BUILD_CREATOR_SEARCH_RELICS"] = "Kalinti ara",
		["BUILD_CREATOR_SELECTED_CARDS"] = "Secili Kartlar",
		["BUILD_CREATOR_SELECTED_RELICS"] = "Secili Kalintilar",
		["BUILD_CREATOR_CARD_BASE"] = "Normal",
		["BUILD_CREATOR_CARD_UPGRADED"] = "Gelistirilmis",
		["BUILD_CREATOR_APPLY"] = "Buildi Uygula",
		["BUILD_CREATOR_ADD"] = "Ekle",
		["BUILD_CREATOR_REMOVE"] = "Cikar",
		["BUILD_CREATOR_EMPTY_CARDS"] = "Kart secilmedi.",
		["BUILD_CREATOR_EMPTY_RELICS"] = "Kalinti secilmedi.",
		["BUILD_CREATOR_NO_RESULTS"] = "Sonuc yok.",
		["BUILD_CREATOR_READY"] = "Build hazir. Uygulayinca baslangic desten ve kalintilarin degisir.",
		["BUILD_CREATOR_NEED_CARD"] = "Devam etmek icin en az bir kart sec.",
		["BUILD_CREATOR_APPLY_FAILED"] = "Build uygulanamadi.",
		["BUILD_CREATOR_CARD_COUNT_SUFFIX"] = "kart",
		["BUILD_CREATOR_RELIC_COUNT_SUFFIX"] = "kalinti",
		["ROUTE_ADVISOR_TITLE"] = "Rota Onerisi",
		["ROUTE_ADVISOR_SAFE"] = "Guvenli:",
		["ROUTE_ADVISOR_AGGRESSIVE"] = "Agresif:",
		["RELIC_DRAWER_LABEL"] = "Kalintilar",
		["RELIC_DRAWER_ALL"] = "Tum Kalintilar",
		["RELIC_DRAWER_EMPTY"] = "Kalinti yok"
	};

	public static string Text(string key)
	{
		IReadOnlyDictionary<string, string> readOnlyDictionary = (IsTurkish() ? TurkishTexts : EnglishTexts);
		string value;
		return readOnlyDictionary.TryGetValue(key, out value) ? value : key;
	}

	public static string Format(string key, params object[] args)
	{
		return string.Format(Text(key), args);
	}

	public static string ModifierTitleKey(string entry)
	{
		if (1 == 0)
		{
		}
		string result = entry switch
		{
			"ATTACK_DEFENSE" => "RULE_ATTACK_DEFENSE_TITLE", 
			"ATTACK_DEFENSE_PLUS" => "RULE_ATTACK_DEFENSE_PLUS_TITLE", 
			"IRON_SKIN" => "RULE_IRON_SKIN_TITLE", 
			"GIANT_CREATURES" => "RULE_GIANT_CREATURES_TITLE", 
			"HARD_ELITES" => "RULE_HARD_ELITES_TITLE", 
			"ENDLESS_MODE" => "RULE_ENDLESS_TITLE", 
			"GLASS_CANNON" => "RULE_GLASS_CANNON_TITLE", 
			"UNLIMITED_GROWTH" => "RULE_UNLIMITED_GROWTH_TITLE", 
			"SANDBOX" => "RULE_SANDBOX_TITLE", 
			"BUILD_CREATOR" => "RULE_BUILD_CREATOR_TITLE", 
			_ => entry, 
		};
		if (1 == 0)
		{
		}
		return result;
	}

	public static string ModifierDescriptionKey(string entry)
	{
		if (1 == 0)
		{
		}
		string result = entry switch
		{
			"ATTACK_DEFENSE" => "RULE_ATTACK_DEFENSE_DESC", 
			"ATTACK_DEFENSE_PLUS" => "RULE_ATTACK_DEFENSE_PLUS_DESC", 
			"IRON_SKIN" => "RULE_IRON_SKIN_DESC", 
			"GIANT_CREATURES" => "RULE_GIANT_CREATURES_DESC", 
			"HARD_ELITES" => "RULE_HARD_ELITES_DESC", 
			"ENDLESS_MODE" => "RULE_ENDLESS_DESC", 
			"GLASS_CANNON" => "RULE_GLASS_CANNON_DESC", 
			"UNLIMITED_GROWTH" => "RULE_UNLIMITED_GROWTH_DESC", 
			"SANDBOX" => "RULE_SANDBOX_DESC", 
			"BUILD_CREATOR" => "RULE_BUILD_CREATOR_DESC", 
			_ => entry, 
		};
		if (1 == 0)
		{
		}
		return result;
	}

	public static string ActNumber(int actNumber)
	{
		return IsTurkish() ? $"{actNumber}. Sahne" : $"Act {actNumber}";
	}

	public static string GenericModifierTitle()
	{
		return IsTurkish() ? "STS2Plus Kurali:" : "STS2Plus Rule:";
	}

	public static string GenericModifierDescription()
	{
		return IsTurkish() ? "Ozel oyun kurali aktif." : "A custom gameplay rule is active.";
	}

	public static void MergeIntoModifiersTable()
	{
		LocManager instance = LocManager.Instance;
		if (instance != null)
		{
			instance.GetTable("modifiers").MergeWith(new Dictionary<string, string>(StringComparer.Ordinal)
			{
				["DEPRECATED_MODIFIER.title"] = GenericModifierTitle(),
				["DEPRECATED_MODIFIER.description"] = GenericModifierDescription(),
				["ATTACK_DEFENSE.title"] = Text(ModifierTitleKey("ATTACK_DEFENSE")),
				["ATTACK_DEFENSE.description"] = Text(ModifierDescriptionKey("ATTACK_DEFENSE")),
				["ATTACK_DEFENSE_PLUS.title"] = Text(ModifierTitleKey("ATTACK_DEFENSE_PLUS")),
				["ATTACK_DEFENSE_PLUS.description"] = Text(ModifierDescriptionKey("ATTACK_DEFENSE_PLUS")),
				["IRON_SKIN.title"] = Text(ModifierTitleKey("IRON_SKIN")),
				["IRON_SKIN.description"] = Text(ModifierDescriptionKey("IRON_SKIN")),
				["GIANT_CREATURES.title"] = Text(ModifierTitleKey("GIANT_CREATURES")),
				["GIANT_CREATURES.description"] = Text(ModifierDescriptionKey("GIANT_CREATURES")),
				["HARD_ELITES.title"] = Text(ModifierTitleKey("HARD_ELITES")),
				["HARD_ELITES.description"] = Text(ModifierDescriptionKey("HARD_ELITES")),
				["ENDLESS_MODE.title"] = Text(ModifierTitleKey("ENDLESS_MODE")),
				["ENDLESS_MODE.description"] = Text(ModifierDescriptionKey("ENDLESS_MODE")),
				["GLASS_CANNON.title"] = Text(ModifierTitleKey("GLASS_CANNON")),
				["GLASS_CANNON.description"] = Text(ModifierDescriptionKey("GLASS_CANNON")),
				["UNLIMITED_GROWTH.title"] = Text(ModifierTitleKey("UNLIMITED_GROWTH")),
				["UNLIMITED_GROWTH.description"] = Text(ModifierDescriptionKey("UNLIMITED_GROWTH")),
				["SANDBOX.title"] = Text(ModifierTitleKey("SANDBOX")),
				["SANDBOX.description"] = Text(ModifierDescriptionKey("SANDBOX")),
				["BUILD_CREATOR.title"] = Text(ModifierTitleKey("BUILD_CREATOR")),
				["BUILD_CREATOR.description"] = Text(ModifierDescriptionKey("BUILD_CREATOR"))
			});
		}
	}

	private static bool IsTurkish()
	{
		string text = TranslationServer.GetLocale() ?? string.Empty;
		return text.StartsWith("tr", StringComparison.OrdinalIgnoreCase) || text.StartsWith("tur", StringComparison.OrdinalIgnoreCase);
	}
}
