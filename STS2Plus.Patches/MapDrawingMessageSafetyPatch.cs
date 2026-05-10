using System;
using System.Reflection;
using HarmonyLib;
using STS2Plus.Reflection;

namespace STS2Plus.Patches;

[HarmonyPatchCategory("Core")]
[HarmonyPatch]
internal static class MapDrawingMessageSafetyPatch
{
	private static MethodBase? TargetMethod()
	{
		Type type = RuntimeTypeResolver.FindType("MegaCrit.Sts2.Core.Nodes.Screens.Map.NMapDrawings") ?? RuntimeTypeResolver.FindTypeByName("NMapDrawings");
		return (type == null) ? null : AccessTools.Method(type, "HandleDrawingMessage", (Type[])null, (Type[])null);
	}

	private static void Prefix(object __instance, object? message, ulong senderId)
	{
		if (message == null)
		{
			return;
		}
		object obj = AccessTools.Method(__instance.GetType(), "GetDrawingStateForPlayer", (Type[])null, (Type[])null)?.Invoke(__instance, new object[1] { senderId });
		if (obj == null)
		{
			return;
		}
		object drawingMode = AccessTools.Property(obj.GetType(), "CurrentDrawingMode")?.GetValue(obj);
		if (!IsDrawingModeNone(drawingMode))
		{
			return;
		}
		object value = AccessTools.Field(message.GetType(), "drawingMode")?.GetValue(message) ?? AccessTools.Property(message.GetType(), "drawingMode")?.GetValue(message);
		object obj2 = UnwrapNullableEnum(value);
		if (obj2 == null || IsDrawingModeNone(obj2))
		{
			return;
		}
		MethodInfo methodInfo = AccessTools.Method(__instance.GetType(), "SetDrawingMode", (Type[])null, (Type[])null);
		if (methodInfo == null)
		{
			return;
		}
		try
		{
			methodInfo.Invoke(__instance, new object[2] { obj, obj2 });
			ModEntry.Logger.Warn($"STS2Plus synchronized missing map drawing mode for player {senderId}: {obj2}", 1);
		}
		catch
		{
		}
	}

	private static object? UnwrapNullableEnum(object? value)
	{
		if (value == null)
		{
			return null;
		}
		Type type = value.GetType();
		if (type.IsEnum)
		{
			return value;
		}
		PropertyInfo propertyInfo = AccessTools.Property(type, "HasValue");
		PropertyInfo propertyInfo2 = AccessTools.Property(type, "Value");
		if (!(propertyInfo?.GetValue(value) as bool?).GetValueOrDefault() || propertyInfo2 == null)
		{
			return null;
		}
		return propertyInfo2.GetValue(value);
	}

	private static bool IsDrawingModeNone(object? drawingMode)
	{
		return drawingMode == null || string.Equals(drawingMode.ToString(), "None", StringComparison.Ordinal);
	}
}
