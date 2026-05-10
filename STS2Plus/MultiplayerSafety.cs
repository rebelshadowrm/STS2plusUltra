using Godot;
using STS2Plus.Reflection;

namespace STS2Plus;

internal static class MultiplayerSafety
{
	public static bool IsGameplayRuleSelectionLocked(Node? context = null)
	{
		return MultiplayerReflection.IsInteractionLocked(context);
	}

	public static bool ShouldInjectGameplayRules(Node? context = null)
	{
		return !MultiplayerReflection.IsInteractionLocked(context);
	}

	public static bool ShouldApplyAuthoritativeGameplayPatches(Node? context = null)
	{
		return !MultiplayerReflection.IsMultiplayerRun() || !MultiplayerReflection.IsInteractionLocked(context);
	}

	public static bool ShouldApplyLocalPlayerGameplayPatches(object? target, Node? context = null)
	{
		return ShouldApplyAuthoritativeGameplayPatches(context) || GameReflection.IsLocalPlayerObject(target);
	}
}
