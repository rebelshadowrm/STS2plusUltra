using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Godot;
using HarmonyLib;
using STS2Plus.Patches;

namespace STS2Plus.Config;

internal static class ModConfigBridge
{
	private sealed record ToggleDefinition(string Key, string Label, string TurkishLabel, string Description, string TurkishDescription, bool DefaultValue, Func<PlusConfig, bool> Getter, Action<PlusConfig, bool> Setter);

	private const string ExternalConfigPath = "user://ModConfig/STS2Plus.json";

	private static readonly ToggleDefinition[] ToggleDefinitions = new ToggleDefinition[10]
	{
		new ToggleDefinition("more_rules_enabled", "More Rules Pack", "Ek Kurallar Paketi", "Enables the custom run gameplay rule pack and its extra rule rows. Change this before opening Custom Run or starting a new run; active runs are not rewritten.", "Custom Run icin ek oyun kurallari paketini ve ekstra kural satirlarini etkinlestirir. Bunu Custom Run ekranini acmadan veya yeni run baslatmadan once degistirin; aktif run geriye donuk degismez.", DefaultValue: true, (PlusConfig config) => config.MoreRulesEnabled, delegate(PlusConfig config, bool value)
		{
			config.MoreRulesEnabled = value;
		}),
		new ToggleDefinition("compact_relic_drawer_enabled", "Compact Relic Drawer", "Kompakt Kalinti Paneli", "Replaces the default relic strip with the STS2Plus relic drawer. Applies when the run HUD is visible.", "Varsayilan kalinti seridini STS2Plus kalinti paneliyle degistirir. Run HUD gorunuyorken uygulanir.", DefaultValue: false, (PlusConfig config) => config.CompactRelicDrawerEnabled, delegate(PlusConfig config, bool value)
		{
			config.CompactRelicDrawerEnabled = value;
		}),
		new ToggleDefinition("speed_control_enabled", "Speed Control", "Hiz Kontrolu", "Shows the 1x / 2x / 4x speed buttons on supported screens. Applies immediately.", "Desteklenen ekranlarda 1x / 2x / 4x hiz tuslarini gosterir. Aninda uygulanir.", DefaultValue: true, (PlusConfig config) => config.SpeedControlEnabled, delegate(PlusConfig config, bool value)
		{
			config.SpeedControlEnabled = value;
		}),
		new ToggleDefinition("quick_restart_enabled", "Quick Restart", "Hizli Yeniden Baslat", "Lets F5 start a fresh run with the current character and run setup. No game restart is needed; it applies to the next F5 press.", "F5 ile mevcut karakter ve run ayarlariyla sifirdan yeni bir run baslatir. Oyun yeniden baslatma gerekmez; bir sonraki F5 basiminda gecerli olur.", DefaultValue: true, (PlusConfig config) => config.QuickRestartEnabled, delegate(PlusConfig config, bool value)
		{
			config.QuickRestartEnabled = value;
		}),
		new ToggleDefinition("skip_intro_enabled", "Skip Intro Screens", "Giris Ekranlarini Gec", "Skips the intro logo and early access disclaimer on startup. Visible next time the game launches.", "Acilista intro logosunu ve erken erisim uyarisini atlar. Etkisi oyunun bir sonraki acilisinda gorulur.", DefaultValue: true, (PlusConfig config) => config.SkipIntroEnabled, delegate(PlusConfig config, bool value)
		{
			config.SkipIntroEnabled = value;
		}),
		new ToggleDefinition("route_advisor_enabled", "Route Advisor", "Rota Yardimcisi", "Highlights recommended safe and aggressive map paths. Applies immediately while the map screen is open.", "Onerilen guvenli ve agresif harita yollarini vurgular. Harita ekrani acikken aninda uygulanir.", DefaultValue: true, (PlusConfig config) => config.RouteAdvisorEnabled, delegate(PlusConfig config, bool value)
		{
			config.RouteAdvisorEnabled = value;
		}),
		new ToggleDefinition("card_total_damage_preview_enabled", "Total Damage Preview", "Toplam Hasar Onizlemesi", "Adds total multi-hit damage to card hover tips. Reopen a card tooltip to see the change.", "Kart tooltiplerinde cok vuruslu toplam hasari gosterir. Degisikligi gormek icin kart tooltipini yeniden acin.", DefaultValue: true, (PlusConfig config) => config.CardTotalDamagePreviewEnabled, delegate(PlusConfig config, bool value)
		{
			config.CardTotalDamagePreviewEnabled = value;
		}),
		new ToggleDefinition("rarity_tags_enabled", "Rarity Tags", "Nadirlik Etiketleri", "Adds rarity labels to card, relic, and potion hover tips. Reopen a hover tip to see the change.", "Kart, relic ve potion tooltiplerine nadirlik etiketi ekler. Degisikligi gormek icin tooltipi yeniden acin.", DefaultValue: true, (PlusConfig config) => config.RarityTagsEnabled, delegate(PlusConfig config, bool value)
		{
			config.RarityTagsEnabled = value;
		}),
		new ToggleDefinition("player_combat_shield_enabled", "Combat Shield", "Savas Kalkani", "Shows the player block vs incoming damage badge in combat. Applies immediately during combat.", "Savasta oyuncunun block ve gelecek hasar rozetini gosterir. Combat sirasinda aninda uygulanir.", DefaultValue: true, (PlusConfig config) => config.PlayerCombatShieldEnabled, delegate(PlusConfig config, bool value)
		{
			config.PlayerCombatShieldEnabled = value;
		}),
		new ToggleDefinition("enable_endless_debug_tools", "Endless Debug Tools", "Endless Hata Ayiklama Araclari", "Enables temporary endless-mode developer hotkeys and the warning HUD. Leave this off for normal play and multiplayer sessions.", "Gecici endless mod gelistirici kisayollarini ve uyari HUD'unu etkinlestirir. Normal oyun ve multiplayer oturumlari icin bunu kapali tutun.", DefaultValue: false, (PlusConfig config) => config.EnableEndlessDebugTools, delegate(PlusConfig config, bool value)
		{
			config.EnableEndlessDebugTools = value;
		})
	};

	private static bool isRegistered;

	private static bool suppressCallbacks;

	public static void TryRegister()
	{
		if (isRegistered)
		{
			return;
		}
		Type type = AccessTools.TypeByName("ModConfig.ModConfigApi");
		Type type2 = AccessTools.TypeByName("ModConfig.ConfigEntry");
		Type type3 = AccessTools.TypeByName("ModConfig.ConfigType");
		if (type == null || type2 == null || type3 == null)
		{
			return;
		}
		try
		{
			MethodInfo methodInfo = type.GetMethods(BindingFlags.Static | BindingFlags.Public).FirstOrDefault((MethodInfo method) => method.Name == "Register" && method.GetParameters().Length == 3);
			if (methodInfo == null)
			{
				ModEntry.Logger.Warn("STS2Plus could not resolve ModConfigApi.Register.", 1);
				return;
			}
			Array array = BuildEntries(type2, type3);
			methodInfo.Invoke(null, new object[3] { "STS2Plus", "STS2Plus", array });
			if (File.Exists(ProjectSettings.GlobalizePath("user://ModConfig/STS2Plus.json")))
			{
				ImportExternalValues(type);
			}
			else
			{
				SeedExternalValues(type);
			}
			isRegistered = true;
			ModEntry.Logger.Info("STS2Plus registered settings with ModConfig.", 1);
		}
		catch (Exception ex)
		{
			ModEntry.Logger.Warn("STS2Plus failed to register ModConfig settings: " + ex.Message, 1);
		}
	}

	private static Array BuildEntries(Type entryType, Type configType)
	{
		List<object> list = new List<object>
		{
			CreateHeaderEntry(entryType, configType, "rules_header", "Gameplay Pack", "Oynanis Paketi"),
			CreateToggleEntry(entryType, configType, FindDefinition("more_rules_enabled")),
			CreateSeparatorEntry(entryType, configType, "main_separator"),
			CreateHeaderEntry(entryType, configType, "qol_header", "Quality of Life", "Yasam Kalitesi"),
			CreateToggleEntry(entryType, configType, FindDefinition("compact_relic_drawer_enabled")),
			CreateToggleEntry(entryType, configType, FindDefinition("speed_control_enabled")),
			CreateToggleEntry(entryType, configType, FindDefinition("quick_restart_enabled")),
			CreateToggleEntry(entryType, configType, FindDefinition("skip_intro_enabled")),
			CreateToggleEntry(entryType, configType, FindDefinition("route_advisor_enabled")),
			CreateSeparatorEntry(entryType, configType, "combat_separator"),
			CreateHeaderEntry(entryType, configType, "combat_header", "Combat Info", "Savas Bilgisi"),
			CreateToggleEntry(entryType, configType, FindDefinition("card_total_damage_preview_enabled")),
			CreateToggleEntry(entryType, configType, FindDefinition("rarity_tags_enabled")),
			CreateToggleEntry(entryType, configType, FindDefinition("player_combat_shield_enabled")),
			CreateSeparatorEntry(entryType, configType, "dev_separator"),
			CreateHeaderEntry(entryType, configType, "dev_header", "Developer Tools", "Gelistirici Araclari"),
			CreateToggleEntry(entryType, configType, FindDefinition("enable_endless_debug_tools"))
		};
		Array array = Array.CreateInstance(entryType, list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			array.SetValue(list[i], i);
		}
		return array;
	}

	private static ToggleDefinition FindDefinition(string key)
	{
		string key2 = key;
		return ToggleDefinitions.First((ToggleDefinition definition) => string.Equals(definition.Key, key2, StringComparison.Ordinal));
	}

	private static object CreateHeaderEntry(Type entryType, Type configType, string key, string label, string turkishLabel)
	{
		object obj = Activator.CreateInstance(entryType) ?? throw new InvalidOperationException("Could not create ModConfig.ConfigEntry.");
		SetProperty(obj, "Key", key);
		SetProperty(obj, "Label", label);
		SetProperty(obj, "Labels", CreateLocalizedText(turkishLabel));
		SetProperty(obj, "Type", Enum.Parse(configType, "Header"));
		SetProperty(obj, "DefaultValue", false);
		return obj;
	}

	private static object CreateSeparatorEntry(Type entryType, Type configType, string key)
	{
		object obj = Activator.CreateInstance(entryType) ?? throw new InvalidOperationException("Could not create ModConfig.ConfigEntry.");
		SetProperty(obj, "Key", key);
		SetProperty(obj, "Label", string.Empty);
		SetProperty(obj, "Type", Enum.Parse(configType, "Separator"));
		SetProperty(obj, "DefaultValue", false);
		return obj;
	}

	private static object CreateToggleEntry(Type entryType, Type configType, ToggleDefinition definition)
	{
		ToggleDefinition definition2 = definition;
		object obj = Activator.CreateInstance(entryType) ?? throw new InvalidOperationException("Could not create ModConfig.ConfigEntry.");
		SetProperty(obj, "Key", definition2.Key);
		SetProperty(obj, "Label", definition2.Label);
		SetProperty(obj, "Labels", CreateLocalizedText(definition2.TurkishLabel));
		SetProperty(obj, "Description", definition2.Description);
		SetProperty(obj, "Descriptions", CreateLocalizedText(definition2.TurkishDescription));
		SetProperty(obj, "Type", Enum.Parse(configType, "Toggle"));
		SetProperty(obj, "DefaultValue", definition2.DefaultValue);
		SetProperty(obj, "OnChanged", (Action<object>)delegate(object value)
		{
			OnToggleChanged(definition2, value);
		});
		return obj;
	}

	private static void OnToggleChanged(ToggleDefinition definition, object value)
	{
		if (TryConvertToBool(value, out var enabled))
		{
			bool previousValue = definition.Getter(ConfigManager.Current);
			definition.Setter(ConfigManager.Current, enabled);
			if (string.Equals(definition.Key, "enable_endless_debug_tools", StringComparison.Ordinal))
			{
				ModEntry.Logger.Info($"STS2Plus debug tools setting changed old={previousValue} new={enabled} sessionDisabled={DebugToolsRuntime.IsSessionDisabled}.", 1);
			}
			if (!suppressCallbacks)
			{
				ConfigManager.Save();
				LiveSettings.ApplyAll();
				if (string.Equals(definition.Key, "enable_endless_debug_tools", StringComparison.Ordinal))
				{
					ModEntry.Logger.Info("STS2Plus debug tools setting applied HUD=" + (enabled ? "show-requested" : "hide-requested") + ".", 1);
				}
			}
		}
	}

	private static void ImportExternalValues(Type apiType)
	{
		ToggleDefinition[] toggleDefinitions = ToggleDefinitions;
		foreach (ToggleDefinition toggleDefinition in toggleDefinitions)
		{
			toggleDefinition.Setter(ConfigManager.Current, GetExternalValue(apiType, toggleDefinition));
		}
		ConfigManager.Save();
		LiveSettings.ApplyAll();
	}

	private static void SeedExternalValues(Type apiType)
	{
		MethodInfo method = apiType.GetMethod("SetValue", BindingFlags.Static | BindingFlags.Public);
		if (method == null)
		{
			return;
		}
		suppressCallbacks = true;
		try
		{
			ToggleDefinition[] toggleDefinitions = ToggleDefinitions;
			foreach (ToggleDefinition toggleDefinition in toggleDefinitions)
			{
				method.Invoke(null, new object[3]
				{
					"STS2Plus",
					toggleDefinition.Key,
					toggleDefinition.Getter(ConfigManager.Current)
				});
			}
		}
		finally
		{
			suppressCallbacks = false;
		}
		LiveSettings.ApplyAll();
	}

	private static bool GetExternalValue(Type apiType, ToggleDefinition definition)
	{
		MethodInfo methodInfo = apiType.GetMethods(BindingFlags.Static | BindingFlags.Public).First((MethodInfo method) => method.Name == "GetValue" && method.IsGenericMethodDefinition).MakeGenericMethod(typeof(bool));
		try
		{
			return (methodInfo.Invoke(null, new object[2] { "STS2Plus", definition.Key }) is bool flag) ? flag : definition.DefaultValue;
		}
		catch
		{
			return definition.DefaultValue;
		}
	}

	private static Dictionary<string, string> CreateLocalizedText(string turkishText)
	{
		return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
		{
			["tr"] = turkishText,
			["tur"] = turkishText
		};
	}

	private static void SetProperty(object target, string propertyName, object? value)
	{
		target.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public)?.SetValue(target, value);
	}

	private static bool TryConvertToBool(object value, out bool enabled)
	{
		if (!(value is bool flag))
		{
			if (value is string text)
			{
				string value2 = text;
				if (bool.TryParse(value2, out var result))
				{
					enabled = result;
					return true;
				}
			}
			enabled = false;
			return false;
		}
		bool flag2 = flag;
		enabled = flag2;
		return true;
	}
}
