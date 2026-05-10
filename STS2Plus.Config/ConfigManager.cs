using System;
using System.IO;
using System.Text.Json;

namespace STS2Plus.Config;

internal static class ConfigManager
{
	private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
	{
		WriteIndented = true
	};

	private static readonly string ConfigDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SlayTheSpire2", "ModConfig");

	private static readonly string ConfigPath = Path.Combine(ConfigDirectory, "STS2Plus.json");

	public static PlusConfig Current { get; private set; } = new PlusConfig();

	public static void Load()
	{
		try
		{
			Directory.CreateDirectory(ConfigDirectory);
			if (!File.Exists(ConfigPath))
			{
				Current = new PlusConfig();
				Save();
			}
			else
			{
				Current = JsonSerializer.Deserialize<PlusConfig>(File.ReadAllText(ConfigPath), JsonOptions) ?? new PlusConfig();
			}
			ModEntry.Verbose($"Config: loaded - Speed={Current.SpeedControlEnabled} RouteAdvisor={Current.RouteAdvisorEnabled} QuickRestart={Current.QuickRestartEnabled} SkipIntro={Current.SkipIntroEnabled} MoreRules={Current.MoreRulesEnabled} EndlessDebugTools={Current.EnableEndlessDebugTools}");
		}
		catch (Exception ex)
		{
			Current = new PlusConfig();
			ModEntry.Logger.Warn("Failed to load STS2Plus config: " + ex.Message, 1);
		}
	}

	public static void Save()
	{
		try
		{
			Directory.CreateDirectory(ConfigDirectory);
			File.WriteAllText(ConfigPath, JsonSerializer.Serialize(Current, JsonOptions));
		}
		catch (Exception ex)
		{
			ModEntry.Logger.Warn("Failed to save STS2Plus config: " + ex.Message, 1);
		}
	}
}
