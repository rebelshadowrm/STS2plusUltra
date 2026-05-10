using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;

namespace STS2Plus.Patches;

[HarmonyPatchCategory("MoreRules")]
[HarmonyPatch]
internal static class UnlimitedGrowthMaxUpgradePatch
{
	[CompilerGenerated]
	private sealed class _003CTargetMethods_003Ed__0 : IEnumerable<MethodBase>, IEnumerable, IEnumerator<MethodBase>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private MethodBase _003C_003E2__current;

		private int _003C_003El__initialThreadId;

		private MethodInfo _003CbaseGetter_003E5__1;

		private Type[] _003C_003Es__2;

		private int _003C_003Es__3;

		private Type _003Ctype_003E5__4;

		private MethodInfo _003Cgetter_003E5__5;

		MethodBase IEnumerator<MethodBase>.Current
		{
			[DebuggerHidden]
			get
			{
				return _003C_003E2__current;
			}
		}

		object IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return _003C_003E2__current;
			}
		}

		[DebuggerHidden]
		public _003CTargetMethods_003Ed__0(int _003C_003E1__state)
		{
			this._003C_003E1__state = _003C_003E1__state;
			_003C_003El__initialThreadId = Environment.CurrentManagedThreadId;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
			_003CbaseGetter_003E5__1 = null;
			_003C_003Es__2 = null;
			_003Ctype_003E5__4 = null;
			_003Cgetter_003E5__5 = null;
			_003C_003E1__state = -2;
		}

		private bool MoveNext()
		{
			switch (_003C_003E1__state)
			{
			default:
				return false;
			case 0:
				_003C_003E1__state = -1;
				_003CbaseGetter_003E5__1 = AccessTools.PropertyGetter(typeof(CardModel), "MaxUpgradeLevel");
				if (_003CbaseGetter_003E5__1 != null)
				{
					_003C_003E2__current = _003CbaseGetter_003E5__1;
					_003C_003E1__state = 1;
					return true;
				}
				goto IL_0076;
			case 1:
				_003C_003E1__state = -1;
				goto IL_0076;
			case 2:
				{
					_003C_003E1__state = -1;
					goto IL_013f;
				}
				IL_0076:
				_003C_003Es__2 = typeof(CardModel).Assembly.GetTypes();
				_003C_003Es__3 = 0;
				goto IL_015c;
				IL_015c:
				if (_003C_003Es__3 < _003C_003Es__2.Length)
				{
					_003Ctype_003E5__4 = _003C_003Es__2[_003C_003Es__3];
					if (_003Ctype_003E5__4.IsAbstract || !typeof(CardModel).IsAssignableFrom(_003Ctype_003E5__4))
					{
						goto IL_014e;
					}
					_003Cgetter_003E5__5 = AccessTools.PropertyGetter(_003Ctype_003E5__4, "MaxUpgradeLevel");
					if (_003Cgetter_003E5__5 != null && _003Cgetter_003E5__5.DeclaringType == _003Ctype_003E5__4)
					{
						_003C_003E2__current = _003Cgetter_003E5__5;
						_003C_003E1__state = 2;
						return true;
					}
					goto IL_013f;
				}
				_003C_003Es__2 = null;
				return false;
				IL_013f:
				_003Cgetter_003E5__5 = null;
				_003Ctype_003E5__4 = null;
				goto IL_014e;
				IL_014e:
				_003C_003Es__3++;
				goto IL_015c;
			}
		}

		bool IEnumerator.MoveNext()
		{
			//ILSpy generated this explicit interface implementation from .override directive in MoveNext
			return this.MoveNext();
		}

		[DebuggerHidden]
		void IEnumerator.Reset()
		{
			throw new NotSupportedException();
		}

		[DebuggerHidden]
		IEnumerator<MethodBase> IEnumerable<MethodBase>.GetEnumerator()
		{
			if (_003C_003E1__state == -2 && _003C_003El__initialThreadId == Environment.CurrentManagedThreadId)
			{
				_003C_003E1__state = 0;
				return this;
			}
			return new _003CTargetMethods_003Ed__0(0);
		}

		[DebuggerHidden]
		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<MethodBase>)this).GetEnumerator();
		}
	}

	[IteratorStateMachine(typeof(_003CTargetMethods_003Ed__0))]
	private static IEnumerable<MethodBase> TargetMethods()
	{
		//yield-return decompiler failed: Unexpected instruction in Iterator.Dispose()
		return new _003CTargetMethods_003Ed__0(-2);
	}

	private static void Postfix(CardModel __instance, ref int __result)
	{
		if (PlusState.IsUnlimitedGrowthActive())
		{
			if (UnlimitedGrowthSafety.CanUseUnlimitedGrowth(__instance, __result) && __result < 99)
			{
				ModEntry.Verbose($"UnlimitedGrowthMaxUpgrade: removing upgrade cap originalMax={__result}");
				__result = 99;
			}
			return;
		}
		int num = UnlimitedGrowthSerializationContext.Peek();
		if (num > __result && UnlimitedGrowthSafety.ShouldAllowSerializedUpgrade(__instance, __result, num))
		{
			__result = num;
			return;
		}
		int currentUpgradeLevel = __instance.CurrentUpgradeLevel;
		if (currentUpgradeLevel > __result && UnlimitedGrowthSafety.ShouldAllowObservedUpgrade(__instance, __result, currentUpgradeLevel))
		{
			__result = currentUpgradeLevel;
		}
	}
}
