using System;
using System.Collections.Generic;
using MegaCrit.Sts2.Core.Models;
using STS2Plus.Config;
using STS2Plus.Modifiers;
using STS2Plus.Reflection;

namespace STS2Plus;

internal static class PlusState
{
	private const int AttackDefenseFlag = 1;

	private const int AttackDefensePlusFlag = 2;

	private const int IronSkinFlag = 4;

	private const int GiantCreaturesFlag = 8;

	private const int HardElitesFlag = 16;

	private const int EndlessModeFlag = 32;

	private const int GlassCannonFlag = 64;

	private const int UnlimitedGrowthFlag = 128;

	private const int SandboxFlag = 256;

	private const int BuildCreatorFlag = 512;

	private static readonly List<string> cachedDisplayRuleEntries = new List<string>();

	private static readonly Dictionary<string, int> glassCannonMaxHpByPlayer = new Dictionary<string, int>(StringComparer.Ordinal);

	private static bool pendingAttackDefenseRuleSelected;

	private static bool pendingAttackDefensePlusRuleSelected;

	private static bool pendingIronSkinRuleSelected;

	private static bool pendingGiantCreaturesRuleSelected;

	private static bool pendingHardElitesRuleSelected;

	private static bool pendingEndlessModeSelected;

	private static bool pendingGlassCannonRuleSelected;

	private static bool pendingUnlimitedGrowthRuleSelected;

	private static bool pendingSandboxRuleSelected;

	private static bool pendingBuildCreatorRuleSelected;

	public static bool AttackDefenseRuleSelected { get; private set; }

	public static bool AttackDefensePlusRuleSelected { get; private set; }

	public static bool IronSkinRuleSelected { get; private set; }

	public static bool GiantCreaturesRuleSelected { get; private set; }

	public static bool HardElitesRuleSelected { get; private set; }

	public static bool EndlessModeSelected { get; private set; }

	public static bool GlassCannonRuleSelected { get; private set; }

	public static bool UnlimitedGrowthRuleSelected { get; private set; }

	public static bool SandboxRuleSelected { get; private set; }

	public static bool BuildCreatorRuleSelected { get; private set; }

	public static decimal CombatDamageTotal { get; private set; }

	public static bool AreGameplayRulesEnabled()
	{
		return ConfigManager.Current.MoreRulesEnabled;
	}

	public static void SetAttackDefenseRule(bool enabled)
	{
		if (!AreGameplayRulesEnabled())
		{
			enabled = false;
		}
		AttackDefenseRuleSelected = enabled;
		pendingAttackDefenseRuleSelected = enabled;
		CaptureCachedDisplayEntries();
	}

	public static void SetAttackDefensePlusRule(bool enabled)
	{
		if (!AreGameplayRulesEnabled())
		{
			enabled = false;
		}
		AttackDefensePlusRuleSelected = enabled;
		pendingAttackDefensePlusRuleSelected = enabled;
		CaptureCachedDisplayEntries();
	}

	public static void SetIronSkinRule(bool enabled)
	{
		if (!AreGameplayRulesEnabled())
		{
			enabled = false;
		}
		IronSkinRuleSelected = enabled;
		pendingIronSkinRuleSelected = enabled;
		CaptureCachedDisplayEntries();
	}

	public static void SetGiantCreaturesRule(bool enabled)
	{
		if (!AreGameplayRulesEnabled())
		{
			enabled = false;
		}
		GiantCreaturesRuleSelected = enabled;
		pendingGiantCreaturesRuleSelected = enabled;
		CaptureCachedDisplayEntries();
	}

	public static void SetHardElitesRule(bool enabled)
	{
		if (!AreGameplayRulesEnabled())
		{
			enabled = false;
		}
		HardElitesRuleSelected = enabled;
		pendingHardElitesRuleSelected = enabled;
		CaptureCachedDisplayEntries();
	}

	public static void SetEndlessModeRule(bool enabled)
	{
		if (!AreGameplayRulesEnabled())
		{
			enabled = false;
		}
		EndlessModeSelected = enabled;
		pendingEndlessModeSelected = enabled;
		CaptureCachedDisplayEntries();
	}

	public static void SetGlassCannonRule(bool enabled)
	{
		if (!AreGameplayRulesEnabled())
		{
			enabled = false;
		}
		GlassCannonRuleSelected = enabled;
		pendingGlassCannonRuleSelected = enabled;
		CaptureCachedDisplayEntries();
	}

	public static void SetUnlimitedGrowthRule(bool enabled)
	{
		if (!AreGameplayRulesEnabled())
		{
			enabled = false;
		}
		UnlimitedGrowthRuleSelected = enabled;
		pendingUnlimitedGrowthRuleSelected = enabled;
		CaptureCachedDisplayEntries();
	}

	public static void SetSandboxRule(bool enabled)
	{
		if (!AreGameplayRulesEnabled())
		{
			enabled = false;
		}
		SandboxRuleSelected = enabled;
		pendingSandboxRuleSelected = enabled;
		CaptureCachedDisplayEntries();
	}

	public static void SetBuildCreatorRule(bool enabled)
	{
		if (!AreGameplayRulesEnabled())
		{
			enabled = false;
		}
		BuildCreatorRuleSelected = enabled;
		pendingBuildCreatorRuleSelected = enabled;
		CaptureCachedDisplayEntries();
	}

	public static void SyncRuleSelections(IEnumerable<ModifierModel> modifiers)
	{
		if (!AreGameplayRulesEnabled())
		{
			ClearGameplayRuleSelections();
			return;
		}
		AttackDefenseRuleSelected = CustomModifierCatalog.ContainsEntry(modifiers, "ATTACK_DEFENSE");
		AttackDefensePlusRuleSelected = CustomModifierCatalog.ContainsEntry(modifiers, "ATTACK_DEFENSE_PLUS");
		IronSkinRuleSelected = CustomModifierCatalog.ContainsEntry(modifiers, "IRON_SKIN");
		GiantCreaturesRuleSelected = CustomModifierCatalog.ContainsEntry(modifiers, "GIANT_CREATURES");
		HardElitesRuleSelected = CustomModifierCatalog.ContainsEntry(modifiers, "HARD_ELITES");
		EndlessModeSelected = CustomModifierCatalog.ContainsEntry(modifiers, "ENDLESS_MODE");
		GlassCannonRuleSelected = CustomModifierCatalog.ContainsEntry(modifiers, "GLASS_CANNON");
		UnlimitedGrowthRuleSelected = CustomModifierCatalog.ContainsEntry(modifiers, "UNLIMITED_GROWTH");
		SandboxRuleSelected = CustomModifierCatalog.ContainsEntry(modifiers, "SANDBOX");
		BuildCreatorRuleSelected = CustomModifierCatalog.ContainsEntry(modifiers, "BUILD_CREATOR");
		CapturePendingSelections();
		CaptureCachedDisplayEntries();
		ModEntry.Logger.Info($"STS2Plus.MoreRules synced selections: attack_defense={AttackDefenseRuleSelected}, attack_defense_plus={AttackDefensePlusRuleSelected}, iron_skin={IronSkinRuleSelected}, giant_creatures={GiantCreaturesRuleSelected}, hard_elites={HardElitesRuleSelected}, endless_mode={EndlessModeSelected}, glass_cannon={GlassCannonRuleSelected}, unlimited_growth={UnlimitedGrowthRuleSelected}, sandbox={SandboxRuleSelected}, build_creator={BuildCreatorRuleSelected}", 1);
	}

	public static void SyncRuleSelectionsFromRunState()
	{
		if (!AreGameplayRulesEnabled())
		{
			ClearGameplayRuleSelections();
			return;
		}
		AttackDefenseRuleSelected = GameReflection.HasActiveModifier("ATTACK_DEFENSE");
		AttackDefensePlusRuleSelected = GameReflection.HasActiveModifier("ATTACK_DEFENSE_PLUS");
		IronSkinRuleSelected = GameReflection.HasActiveModifier("IRON_SKIN");
		GiantCreaturesRuleSelected = GameReflection.HasActiveModifier("GIANT_CREATURES");
		HardElitesRuleSelected = GameReflection.HasActiveModifier("HARD_ELITES");
		EndlessModeSelected = GameReflection.HasActiveModifier("ENDLESS_MODE");
		GlassCannonRuleSelected = GameReflection.HasActiveModifier("GLASS_CANNON");
		UnlimitedGrowthRuleSelected = GameReflection.HasActiveModifier("UNLIMITED_GROWTH");
		SandboxRuleSelected = GameReflection.HasActiveModifier("SANDBOX");
		BuildCreatorRuleSelected = GameReflection.HasActiveModifier("BUILD_CREATOR");
		if (!HasAnySelectedRule() && HasAnyPendingRule())
		{
			RestorePendingSelections();
			ModEntry.Logger.Warn("STS2Plus.MoreRules restored pending selections (modifiers not found in run state).", 1);
		}
		CaptureCachedDisplayEntries();
		ModEntry.Logger.Info($"STS2Plus.MoreRules run-state sync: attack_defense={AttackDefenseRuleSelected}, attack_defense_plus={AttackDefensePlusRuleSelected}, iron_skin={IronSkinRuleSelected}, giant_creatures={GiantCreaturesRuleSelected}, hard_elites={HardElitesRuleSelected}, endless_mode={EndlessModeSelected}, glass_cannon={GlassCannonRuleSelected}, unlimited_growth={UnlimitedGrowthRuleSelected}, sandbox={SandboxRuleSelected}, build_creator={BuildCreatorRuleSelected}", 1);
	}

	public static bool IsAttackDefenseActive()
	{
		return AreGameplayRulesEnabled() && (AttackDefenseRuleSelected || GameReflection.HasActiveModifier("ATTACK_DEFENSE"));
	}

	public static bool IsAttackDefensePlusActive()
	{
		return AreGameplayRulesEnabled() && (AttackDefensePlusRuleSelected || GameReflection.HasActiveModifier("ATTACK_DEFENSE_PLUS"));
	}

	public static bool IsIronSkinActive()
	{
		return AreGameplayRulesEnabled() && (IronSkinRuleSelected || GameReflection.HasActiveModifier("IRON_SKIN"));
	}

	public static bool IsGiantCreaturesActive()
	{
		return AreGameplayRulesEnabled() && (GiantCreaturesRuleSelected || GameReflection.HasActiveModifier("GIANT_CREATURES"));
	}

	public static bool IsHardElitesActive()
	{
		return AreGameplayRulesEnabled() && (HardElitesRuleSelected || GameReflection.HasActiveModifier("HARD_ELITES"));
	}

	public static bool IsEndlessModeActive()
	{
		return AreGameplayRulesEnabled() && (EndlessModeSelected || GameReflection.HasActiveModifier("ENDLESS_MODE"));
	}

	public static bool IsGlassCannonActive()
	{
		return AreGameplayRulesEnabled() && (GlassCannonRuleSelected || GameReflection.HasActiveModifier("GLASS_CANNON"));
	}

	public static bool ShouldForceGlassCannonRepair()
	{
		return AreGameplayRulesEnabled() && (GlassCannonRuleSelected || pendingGlassCannonRuleSelected);
	}

	public static bool IsUnlimitedGrowthActive()
	{
		return AreGameplayRulesEnabled() && (UnlimitedGrowthRuleSelected || GameReflection.HasActiveModifier("UNLIMITED_GROWTH"));
	}

	public static bool IsSandboxActive()
	{
		return AreGameplayRulesEnabled() && (SandboxRuleSelected || GameReflection.HasActiveModifier("SANDBOX"));
	}

	public static bool IsBuildCreatorActive()
	{
		return AreGameplayRulesEnabled() && (BuildCreatorRuleSelected || GameReflection.HasActiveModifier("BUILD_CREATOR"));
	}

	public static decimal GetAttackCardBonus()
	{
		decimal result = default(decimal);
		if (IsAttackDefenseActive())
		{
			result += 2m;
		}
		if (IsAttackDefensePlusActive())
		{
			result += 5m;
		}
		if (IsIronSkinActive())
		{
			result -= 2m;
		}
		if (IsGlassCannonActive())
		{
			result += 3m;
		}
		return result;
	}

	public static decimal GetDefenseCardBonus()
	{
		decimal result = default(decimal);
		if (IsAttackDefenseActive())
		{
			result += 2m;
		}
		if (IsAttackDefensePlusActive())
		{
			result += 5m;
		}
		if (IsIronSkinActive())
		{
			result += 5m;
		}
		return result;
	}

	public static void AddCombatDamage(decimal amount)
	{
		if (amount > 0m)
		{
			CombatDamageTotal += amount;
		}
	}

	public static void ResetCombatDamage()
	{
		CombatDamageTotal = 0m;
	}

	public static int EnsureGlassCannonExpectedMaxHp(object? player, int fallbackMaxHp = 1)
	{
		string playerStateKey = GameReflection.GetPlayerStateKey(player);
		int num = Math.Max(1, fallbackMaxHp);
		if (string.IsNullOrWhiteSpace(playerStateKey))
		{
			return num;
		}
		if (glassCannonMaxHpByPlayer.TryGetValue(playerStateKey, out var value))
		{
			return Math.Max(1, value);
		}
		glassCannonMaxHpByPlayer[playerStateKey] = num;
		return num;
	}

	public static int RememberGlassCannonExpectedMaxHp(object? player, int value)
	{
		string playerStateKey = GameReflection.GetPlayerStateKey(player);
		if (string.IsNullOrWhiteSpace(playerStateKey))
		{
			return Math.Max(1, value);
		}
		int num = Math.Max(1, value);
		glassCannonMaxHpByPlayer[playerStateKey] = num;
		return num;
	}

	public static bool TryGetGlassCannonExpectedMaxHp(object? player, out int value)
	{
		string playerStateKey = GameReflection.GetPlayerStateKey(player);
		if (!string.IsNullOrWhiteSpace(playerStateKey) && glassCannonMaxHpByPlayer.TryGetValue(playerStateKey, out var value2))
		{
			value = Math.Max(1, value2);
			return true;
		}
		value = 0;
		return false;
	}

	public static int IncreaseGlassCannonExpectedMaxHp(object? player, int amount)
	{
		if (amount <= 0)
		{
			return EnsureGlassCannonExpectedMaxHp(player);
		}
		string playerStateKey = GameReflection.GetPlayerStateKey(player);
		if (string.IsNullOrWhiteSpace(playerStateKey))
		{
			return Math.Max(1, amount + 1);
		}
		int value;
		int num = ((!glassCannonMaxHpByPlayer.TryGetValue(playerStateKey, out value)) ? 1 : value);
		int num2 = Math.Max(1, num + amount);
		glassCannonMaxHpByPlayer[playerStateKey] = num2;
		return num2;
	}

	public static int GetGlassCannonExpectedMaxHp(object? player, int fallbackMaxHp = 1)
	{
		string playerStateKey = GameReflection.GetPlayerStateKey(player);
		if (string.IsNullOrWhiteSpace(playerStateKey))
		{
			return Math.Max(1, fallbackMaxHp);
		}
		int value;
		return glassCannonMaxHpByPlayer.TryGetValue(playerStateKey, out value) ? Math.Max(1, value) : EnsureGlassCannonExpectedMaxHp(player, fallbackMaxHp);
	}

	public static void ResetRunRules()
	{
		ClearGameplayRuleSelections();
		glassCannonMaxHpByPlayer.Clear();
		ResetCombatDamage();
	}

	public static void ClearGameplayRuleSelections()
	{
		AttackDefenseRuleSelected = false;
		AttackDefensePlusRuleSelected = false;
		IronSkinRuleSelected = false;
		GiantCreaturesRuleSelected = false;
		HardElitesRuleSelected = false;
		EndlessModeSelected = false;
		GlassCannonRuleSelected = false;
		UnlimitedGrowthRuleSelected = false;
		SandboxRuleSelected = false;
		BuildCreatorRuleSelected = false;
		pendingAttackDefenseRuleSelected = false;
		pendingAttackDefensePlusRuleSelected = false;
		pendingIronSkinRuleSelected = false;
		pendingGiantCreaturesRuleSelected = false;
		pendingHardElitesRuleSelected = false;
		pendingEndlessModeSelected = false;
		pendingGlassCannonRuleSelected = false;
		pendingUnlimitedGrowthRuleSelected = false;
		pendingSandboxRuleSelected = false;
		pendingBuildCreatorRuleSelected = false;
		cachedDisplayRuleEntries.Clear();
	}

	public static int GetRuleSelectionMask()
	{
		if (!AreGameplayRulesEnabled())
		{
			return 0;
		}
		int num = 0;
		num |= (AttackDefenseRuleSelected ? 1 : 0);
		num |= (AttackDefensePlusRuleSelected ? 2 : 0);
		num |= (IronSkinRuleSelected ? 4 : 0);
		num |= (GiantCreaturesRuleSelected ? 8 : 0);
		num |= (HardElitesRuleSelected ? 16 : 0);
		num |= (EndlessModeSelected ? 32 : 0);
		num |= (GlassCannonRuleSelected ? 64 : 0);
		num |= (UnlimitedGrowthRuleSelected ? 128 : 0);
		num |= (SandboxRuleSelected ? 256 : 0);
		return num | (BuildCreatorRuleSelected ? 512 : 0);
	}

	public static void ApplyRuleSelectionMask(int mask)
	{
		if (!AreGameplayRulesEnabled())
		{
			ClearGameplayRuleSelections();
			return;
		}
		AttackDefenseRuleSelected = (mask & 1) != 0;
		AttackDefensePlusRuleSelected = (mask & 2) != 0;
		IronSkinRuleSelected = (mask & 4) != 0;
		GiantCreaturesRuleSelected = (mask & 8) != 0;
		HardElitesRuleSelected = (mask & 0x10) != 0;
		EndlessModeSelected = (mask & 0x20) != 0;
		GlassCannonRuleSelected = (mask & 0x40) != 0;
		UnlimitedGrowthRuleSelected = (mask & 0x80) != 0;
		SandboxRuleSelected = (mask & 0x100) != 0;
		BuildCreatorRuleSelected = (mask & 0x200) != 0;
		CapturePendingSelections();
		CaptureCachedDisplayEntries();
	}

	public static IReadOnlyList<string> GetDisplayRuleEntries()
	{
		if (!AreGameplayRulesEnabled())
		{
			return Array.Empty<string>();
		}
		IReadOnlyList<string> ruleEntries = GetRuleEntries(AttackDefenseRuleSelected, AttackDefensePlusRuleSelected, IronSkinRuleSelected, GiantCreaturesRuleSelected, HardElitesRuleSelected, EndlessModeSelected, GlassCannonRuleSelected, UnlimitedGrowthRuleSelected, SandboxRuleSelected, BuildCreatorRuleSelected);
		if (ruleEntries.Count > 0)
		{
			return ruleEntries;
		}
		IReadOnlyList<string> ruleEntries2 = GetRuleEntries(pendingAttackDefenseRuleSelected, pendingAttackDefensePlusRuleSelected, pendingIronSkinRuleSelected, pendingGiantCreaturesRuleSelected, pendingHardElitesRuleSelected, pendingEndlessModeSelected, pendingGlassCannonRuleSelected, pendingUnlimitedGrowthRuleSelected, pendingSandboxRuleSelected, pendingBuildCreatorRuleSelected);
		if (ruleEntries2.Count > 0)
		{
			return ruleEntries2;
		}
		return cachedDisplayRuleEntries;
	}

	private static void CapturePendingSelections()
	{
		pendingAttackDefenseRuleSelected = AttackDefenseRuleSelected;
		pendingAttackDefensePlusRuleSelected = AttackDefensePlusRuleSelected;
		pendingIronSkinRuleSelected = IronSkinRuleSelected;
		pendingGiantCreaturesRuleSelected = GiantCreaturesRuleSelected;
		pendingHardElitesRuleSelected = HardElitesRuleSelected;
		pendingEndlessModeSelected = EndlessModeSelected;
		pendingGlassCannonRuleSelected = GlassCannonRuleSelected;
		pendingUnlimitedGrowthRuleSelected = UnlimitedGrowthRuleSelected;
		pendingSandboxRuleSelected = SandboxRuleSelected;
		pendingBuildCreatorRuleSelected = BuildCreatorRuleSelected;
	}

	private static void CaptureCachedDisplayEntries()
	{
		cachedDisplayRuleEntries.Clear();
		cachedDisplayRuleEntries.AddRange(GetRuleEntries(AttackDefenseRuleSelected, AttackDefensePlusRuleSelected, IronSkinRuleSelected, GiantCreaturesRuleSelected, HardElitesRuleSelected, EndlessModeSelected, GlassCannonRuleSelected, UnlimitedGrowthRuleSelected, SandboxRuleSelected, BuildCreatorRuleSelected));
	}

	private static void RestorePendingSelections()
	{
		AttackDefenseRuleSelected = pendingAttackDefenseRuleSelected;
		AttackDefensePlusRuleSelected = pendingAttackDefensePlusRuleSelected;
		IronSkinRuleSelected = pendingIronSkinRuleSelected;
		GiantCreaturesRuleSelected = pendingGiantCreaturesRuleSelected;
		HardElitesRuleSelected = pendingHardElitesRuleSelected;
		EndlessModeSelected = pendingEndlessModeSelected;
		GlassCannonRuleSelected = pendingGlassCannonRuleSelected;
		UnlimitedGrowthRuleSelected = pendingUnlimitedGrowthRuleSelected;
		SandboxRuleSelected = pendingSandboxRuleSelected;
		BuildCreatorRuleSelected = pendingBuildCreatorRuleSelected;
	}

	private static bool HasAnySelectedRule()
	{
		return AttackDefenseRuleSelected || AttackDefensePlusRuleSelected || IronSkinRuleSelected || GiantCreaturesRuleSelected || HardElitesRuleSelected || EndlessModeSelected || GlassCannonRuleSelected || UnlimitedGrowthRuleSelected || SandboxRuleSelected || BuildCreatorRuleSelected;
	}

	private static bool HasAnyPendingRule()
	{
		return pendingAttackDefenseRuleSelected || pendingAttackDefensePlusRuleSelected || pendingIronSkinRuleSelected || pendingGiantCreaturesRuleSelected || pendingHardElitesRuleSelected || pendingEndlessModeSelected || pendingGlassCannonRuleSelected || pendingUnlimitedGrowthRuleSelected || pendingSandboxRuleSelected || pendingBuildCreatorRuleSelected;
	}

	private static IReadOnlyList<string> GetRuleEntries(bool attackDefense, bool attackDefensePlus, bool ironSkin, bool giantCreatures, bool hardElites, bool endlessMode, bool glassCannon, bool unlimitedGrowth, bool sandbox, bool buildCreator)
	{
		List<string> list = new List<string>(10);
		AppendIfSelected(list, attackDefense, "ATTACK_DEFENSE");
		AppendIfSelected(list, attackDefensePlus, "ATTACK_DEFENSE_PLUS");
		AppendIfSelected(list, ironSkin, "IRON_SKIN");
		AppendIfSelected(list, giantCreatures, "GIANT_CREATURES");
		AppendIfSelected(list, hardElites, "HARD_ELITES");
		AppendIfSelected(list, endlessMode, "ENDLESS_MODE");
		AppendIfSelected(list, glassCannon, "GLASS_CANNON");
		AppendIfSelected(list, unlimitedGrowth, "UNLIMITED_GROWTH");
		AppendIfSelected(list, sandbox, "SANDBOX");
		AppendIfSelected(list, buildCreator, "BUILD_CREATOR");
		return list;
	}

	private static void AppendIfSelected(List<string> entries, bool enabled, string entry)
	{
		if (enabled)
		{
			entries.Add(entry);
		}
	}
}
