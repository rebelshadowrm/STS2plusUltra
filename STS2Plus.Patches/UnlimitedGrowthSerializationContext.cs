using System;
using System.Collections.Generic;

namespace STS2Plus.Patches;

internal static class UnlimitedGrowthSerializationContext
{
	[ThreadStatic]
	private static Stack<int>? serializedUpgradeLevels;

	public static void Push(int upgradeLevel)
	{
		(serializedUpgradeLevels ?? (serializedUpgradeLevels = new Stack<int>())).Push(upgradeLevel);
	}

	public static void Pop()
	{
		Stack<int> stack = serializedUpgradeLevels;
		if (stack != null && stack.Count > 0)
		{
			serializedUpgradeLevels.Pop();
		}
	}

	public static int Peek()
	{
		Stack<int> stack = serializedUpgradeLevels;
		return (stack != null && stack.Count > 0) ? serializedUpgradeLevels.Peek() : 0;
	}
}
