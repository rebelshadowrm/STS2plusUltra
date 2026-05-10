using System;
using System.IO;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Saves;

namespace STS2Plus.Patches;

internal static class EndlessSaveLifecycleProtection
{
	internal static bool BeforeDeleteFile(string path)
	{
		try
		{
			if (!ShouldPreserveCurrentRunDuringCloudSync(path))
			{
				return true;
			}
			ModEntry.Logger.Info("STS2Plus endless loop: prevented save delete for '" + path + "' because a local current-run save exists.", 1);
			return false;
		}
		catch (Exception ex)
		{
			ModEntry.Logger.Error("STS2Plus endless loop: failed while evaluating save delete protection for '" + path + "'. " + ex, 1);
			return true;
		}
	}

	internal static bool BeforeWriteFile(string path)
	{
		try
		{
			if (!ShouldPreserveCurrentRunDuringCloudSync(path))
			{
				return true;
			}
			ModEntry.Logger.Info("STS2Plus endless loop: prevented save overwrite for '" + path + "' because a local current-run save exists.", 1);
			return false;
		}
		catch (Exception ex)
		{
			ModEntry.Logger.Error("STS2Plus endless loop: failed while evaluating save write protection for '" + path + "'. " + ex, 1);
			return true;
		}
	}

	internal static bool BeforeQuit()
	{
		if (!EndlessLoopCoordinator.IsLaunching)
		{
			return true;
		}
		ModEntry.Logger.Info("STS2Plus endless loop: blocked NGame.Quit while endless transition is in progress.", 1);
		return false;
	}

	private static bool ShouldPreserveCurrentRunDuringCloudSync(string path)
	{
		if (string.IsNullOrWhiteSpace(path))
		{
			return false;
		}
		string fileName = Path.GetFileName(path.Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar));
		if (!IsProtectedCurrentRunSave(fileName))
		{
			return false;
		}
		string stackTrace = System.Environment.StackTrace;
		bool isCloudSync = stackTrace.Contains("CloudSaveStore.SyncCloudToLocalInternal", StringComparison.OrdinalIgnoreCase)
			|| stackTrace.Contains("SaveManager.SyncCloudToLocal", StringComparison.OrdinalIgnoreCase);
		if (!isCloudSync)
		{
			return false;
		}
		return TryFindExistingLocalSave(path, out _);
	}

	private static bool IsProtectedCurrentRunSave(string fileName)
	{
		return string.Equals(fileName, "current_run.save", StringComparison.OrdinalIgnoreCase)
			|| string.Equals(fileName, "current_run_mp.save", StringComparison.OrdinalIgnoreCase);
	}

	private static bool TryFindExistingLocalSave(string relativePath, out string localPath)
	{
		localPath = string.Empty;
		string normalized = relativePath.Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar).TrimStart(Path.DirectorySeparatorChar);
		string root = Godot.ProjectSettings.GlobalizePath("user://");
		if (string.IsNullOrWhiteSpace(root))
		{
			return false;
		}
		string directPath = Path.Combine(root, normalized);
		if (File.Exists(directPath))
		{
			localPath = directPath;
			return true;
		}
		string steamRoot = Path.Combine(root, "steam");
		if (!Directory.Exists(steamRoot))
		{
			return false;
		}
		foreach (string profileDir in Directory.EnumerateDirectories(steamRoot))
		{
			string candidate = Path.Combine(profileDir, normalized);
			if (File.Exists(candidate))
			{
				localPath = candidate;
				return true;
			}
		}
		return false;
	}
}

[HarmonyPatchCategory("MoreRules")]
[HarmonyPatch(typeof(CloudSaveStore), "DeleteFile", new Type[1] { typeof(string) })]
internal static class EndlessCloudDeleteProtectionPatch
{
	private static bool Prefix(string path)
	{
		return EndlessSaveLifecycleProtection.BeforeDeleteFile(path);
	}
}

[HarmonyPatchCategory("MoreRules")]
[HarmonyPatch(typeof(CloudSaveStore), "WriteFile", new Type[2]
{
	typeof(string),
	typeof(string)
})]
internal static class EndlessCloudWriteTextProtectionPatch
{
	private static bool Prefix(string path)
	{
		return EndlessSaveLifecycleProtection.BeforeWriteFile(path);
	}
}

[HarmonyPatchCategory("MoreRules")]
[HarmonyPatch(typeof(CloudSaveStore), "WriteFile", new Type[2]
{
	typeof(string),
	typeof(byte[])
})]
internal static class EndlessCloudWriteBytesProtectionPatch
{
	private static bool Prefix(string path)
	{
		return EndlessSaveLifecycleProtection.BeforeWriteFile(path);
	}
}

[HarmonyPatchCategory("MoreRules")]
[HarmonyPatch(typeof(NGame), "Quit")]
internal static class EndlessQuitProtectionPatch
{
	private static bool Prefix()
	{
		return EndlessSaveLifecycleProtection.BeforeQuit();
	}
}
