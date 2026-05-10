namespace STS2Plus.Config;

internal sealed class PlusConfig
{
	public bool MoreRulesEnabled { get; set; } = true;


	public bool RouteAdvisorEnabled { get; set; } = true;


	public bool CompactRelicDrawerEnabled { get; set; } = false;


	public bool SpeedControlEnabled { get; set; } = true;


	public bool QuickRestartEnabled { get; set; } = true;


	public bool SkipIntroEnabled { get; set; } = true;


	public bool CardTotalDamagePreviewEnabled { get; set; } = true;


	public bool RarityTagsEnabled { get; set; } = true;


	public bool PlayerCombatShieldEnabled { get; set; } = true;


	public bool VerboseLoggingEnabled { get; set; } = true;


	public bool EnableEndlessDebugTools { get; set; } = false;

}
