using System;
using STS2Plus.Config;

namespace STS2Plus.Patches;

internal static class DebugToolsRuntime
{
	private static bool sessionDisabled;

	public static bool IsSessionDisabled => sessionDisabled;

	public static bool IsEnabledForSession => ConfigManager.Current.EnableEndlessDebugTools && !sessionDisabled;

	public static void DisableForSession(string reason, Exception? exception = null)
	{
		if (sessionDisabled)
		{
			return;
		}
		sessionDisabled = true;
		if (exception == null)
		{
			ModEntry.Logger.Warn("STS2Plus debug tools disabled for this session: " + reason, 1);
			return;
		}
		ModEntry.Logger.Warn("STS2Plus debug tools disabled for this session: " + reason + " -> " + exception, 1);
	}
}
