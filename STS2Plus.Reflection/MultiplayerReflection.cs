using System;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Multiplayer.Game;
using MegaCrit.Sts2.Core.Runs;

namespace STS2Plus.Reflection;

internal static class MultiplayerReflection
{
	private enum LocalRole
	{
		Unknown,
		Host,
		Client
	}

	private static LocalRole localRole;

	private static string? lastLoggedServiceState;

	private static LocalRole lastLoggedRole;

	public static void MarkHost()
	{
		localRole = LocalRole.Host;
		LogRoleChange("mark-host");
	}

	public static void MarkClient()
	{
		if (localRole != LocalRole.Host)
		{
			localRole = LocalRole.Client;
			LogRoleChange("mark-client");
		}
	}

	public static void ClearRole()
	{
		localRole = LocalRole.Unknown;
		lastLoggedServiceState = null;
		lastLoggedRole = LocalRole.Unknown;
	}

	public static bool IsInteractionLocked(Node? context = null)
	{
		LocalRole localRole = InferLocalRole(context);
		LogRoleChange("interaction-lock-check");
		return localRole == LocalRole.Client;
	}

	public static bool IsMultiplayerRun()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Invalid comparison between Unknown and I4
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Invalid comparison between Unknown and I4
		try
		{
			RunManager instance = RunManager.Instance;
			INetGameService val = ((instance != null) ? instance.NetService : null);
			if (val != null)
			{
				LogServiceState(val, "run-manager");
				return (int)val.Type == 2 || (int)val.Type == 3;
			}
		}
		catch (Exception ex)
		{
			ModEntry.Logger.Warn("Failed to inspect RunManager.NetService: " + ex.Message, 1);
		}
		Type type = RuntimeTypeResolver.FindType("GodotPlugins.Game") ?? RuntimeTypeResolver.FindType("MegaCrit.Sts2.Core.Multiplayer.Game") ?? RuntimeTypeResolver.FindTypeByName("Game");
		if (type == null)
		{
			return false;
		}
		try
		{
			object obj = AccessTools.Property(type, "Instance")?.GetValue(null);
			if (obj == null)
			{
				return false;
			}
			object obj2 = AccessTools.Property(type, "GameService")?.GetValue(obj) ?? AccessTools.Field(type, "gameService")?.GetValue(obj);
			LogServiceState(obj2, "game-singleton");
			return InferRoleFromObject(obj2) != LocalRole.Unknown;
		}
		catch (Exception ex2)
		{
			ModEntry.Logger.Warn("Failed to infer multiplayer presence: " + ex2.Message, 1);
			return false;
		}
	}

	public static string DescribeCurrentService()
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			RunManager instance = RunManager.Instance;
			INetGameService val = ((instance != null) ? instance.NetService : null);
			if (val != null)
			{
				return $"type={val.Type} connected={val.IsConnected} netId={val.NetId}";
			}
		}
		catch
		{
		}
		return $"cached_role={localRole}";
	}

	private static LocalRole InferLocalRole(Node? context)
	{
		LocalRole localRole = InferRoleFromGameService();
		if (localRole != 0)
		{
			MultiplayerReflection.localRole = localRole;
			LogRoleChange("service");
			return localRole;
		}
		LocalRole localRole2 = InferRoleFromNodeHierarchy(context);
		if (localRole2 != 0)
		{
			MultiplayerReflection.localRole = localRole2;
			LogRoleChange("node");
			return localRole2;
		}
		return MultiplayerReflection.localRole;
	}

	private static LocalRole InferRoleFromGameService()
	{
		try
		{
			RunManager instance = RunManager.Instance;
			INetGameService instance2 = ((instance != null) ? instance.NetService : null);
			LocalRole localRole = InferRoleFromObject(instance2);
			if (localRole != 0)
			{
				return localRole;
			}
		}
		catch
		{
		}
		Type type = RuntimeTypeResolver.FindType("GodotPlugins.Game") ?? RuntimeTypeResolver.FindType("MegaCrit.Sts2.Core.Multiplayer.Game") ?? RuntimeTypeResolver.FindTypeByName("Game");
		if (type == null)
		{
			return LocalRole.Unknown;
		}
		try
		{
			object obj2 = AccessTools.Property(type, "Instance")?.GetValue(null);
			if (obj2 == null)
			{
				return LocalRole.Unknown;
			}
			object instance3 = AccessTools.Property(type, "GameService")?.GetValue(obj2) ?? AccessTools.Field(type, "gameService")?.GetValue(obj2);
			return InferRoleFromObject(instance3);
		}
		catch
		{
			return LocalRole.Unknown;
		}
	}

	private static LocalRole InferRoleFromNodeHierarchy(Node? context)
	{
		for (Node val = context; val != null; val = val.GetParent())
		{
			LocalRole localRole = InferRoleFromObject(val);
			if (localRole != 0)
			{
				return localRole;
			}
		}
		return LocalRole.Unknown;
	}

	private static LocalRole InferRoleFromObject(object? instance)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Invalid comparison between Unknown and I4
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Invalid comparison between Unknown and I4
		if (instance == null)
		{
			return LocalRole.Unknown;
		}
		INetGameService val = (INetGameService)((instance is INetGameService) ? instance : null);
		if (val != null)
		{
			NetGameType type = val.Type;
			if (1 == 0)
			{
			}
			LocalRole result = (((int)type == 2) ? LocalRole.Host : (((int)type == 3) ? LocalRole.Client : LocalRole.Unknown));
			if (1 == 0)
			{
			}
			return result;
		}
		string[] array = new string[4] { "Type", "NetType", "Role", "LocalRole" };
		foreach (string text in array)
		{
			string a = AccessTools.Property(instance.GetType(), text)?.GetValue(instance)?.ToString() ?? AccessTools.Field(instance.GetType(), text)?.GetValue(instance)?.ToString();
			if (string.Equals(a, "Host", StringComparison.OrdinalIgnoreCase))
			{
				return LocalRole.Host;
			}
			if (string.Equals(a, "Client", StringComparison.OrdinalIgnoreCase))
			{
				return LocalRole.Client;
			}
		}
		string[] array2 = new string[5] { "IsHost", "isHost", "_isHost", "LocalPlayerIsHost", "IsLocalPlayerHost" };
		foreach (string memberName in array2)
		{
			if (TryReadBool(instance, memberName, out var value))
			{
				return value ? LocalRole.Host : LocalRole.Client;
			}
		}
		string[] array3 = new string[5] { "IsClient", "isClient", "_isClient", "LocalPlayerIsClient", "IsLocalPlayerClient" };
		foreach (string memberName2 in array3)
		{
			if (TryReadBool(instance, memberName2, out var value2))
			{
				return (!value2) ? LocalRole.Host : LocalRole.Client;
			}
		}
		return LocalRole.Unknown;
	}

	private static bool TryReadBool(object instance, string memberName, out bool value)
	{
		value = false;
		try
		{
			if (AccessTools.Property(instance.GetType(), memberName)?.GetValue(instance) is bool flag)
			{
				value = flag;
				return true;
			}
			if (AccessTools.Field(instance.GetType(), memberName)?.GetValue(instance) is bool flag2)
			{
				value = flag2;
				return true;
			}
		}
		catch
		{
			return false;
		}
		return false;
	}

	private static void LogServiceState(object? service, string source)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		INetGameService val = (INetGameService)((service is INetGameService) ? service : null);
		string text = ((val == null) ? (source + ":" + (service?.GetType().FullName ?? "<null>")) : $"{source}:type={val.Type}:connected={val.IsConnected}:netId={val.NetId}");
		if (!string.Equals(lastLoggedServiceState, text, StringComparison.Ordinal))
		{
			lastLoggedServiceState = text;
			ModEntry.Logger.Info("STS2Plus.Net service-state " + text, 1);
		}
	}

	private static void LogRoleChange(string source)
	{
		if (lastLoggedRole != localRole)
		{
			lastLoggedRole = localRole;
			ModEntry.Logger.Info($"STS2Plus.Net local-role={localRole} source={source}", 1);
		}
	}
}
