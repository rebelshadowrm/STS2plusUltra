using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using STS2Plus.Features;
using STS2Plus.Reflection;

namespace STS2Plus.Patches;

[HarmonyPatchCategory("MoreRules")]
[HarmonyPatch]
internal static class GlassCannonPlayerCreaturePatch
{
	[CompilerGenerated]
	private sealed class _003CTargetMethods_003Ed__0 : IEnumerable<MethodBase>, IEnumerable, IEnumerator<MethodBase>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private MethodBase _003C_003E2__current;

		private int _003C_003El__initialThreadId;

		private MethodInfo _003CafterAddedToRoom_003E5__1;

		private MethodInfo _003CbeforeCombatStart_003E5__2;

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
			_003CafterAddedToRoom_003E5__1 = null;
			_003CbeforeCombatStart_003E5__2 = null;
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
				if (GameReflection.CreatureRuntimeType == null)
				{
					return false;
				}
				_003CafterAddedToRoom_003E5__1 = AccessTools.Method(GameReflection.CreatureRuntimeType, "AfterAddedToRoom", (Type[])null, (Type[])null);
				if (_003CafterAddedToRoom_003E5__1 != null)
				{
					_003C_003E2__current = _003CafterAddedToRoom_003E5__1;
					_003C_003E1__state = 1;
					return true;
				}
				goto IL_0085;
			case 1:
				_003C_003E1__state = -1;
				goto IL_0085;
			case 2:
				{
					_003C_003E1__state = -1;
					break;
				}
				IL_0085:
				_003CbeforeCombatStart_003E5__2 = AccessTools.Method(GameReflection.CreatureRuntimeType, "BeforeCombatStart", (Type[])null, (Type[])null);
				if (_003CbeforeCombatStart_003E5__2 != null)
				{
					_003C_003E2__current = _003CbeforeCombatStart_003E5__2;
					_003C_003E1__state = 2;
					return true;
				}
				break;
			}
			return false;
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

	private static void Postfix(object __instance)
	{
		if (PlusState.IsGlassCannonActive() && GameReflection.IsAnyPlayerCreature(__instance) && AppliedTracker.MarkGlassCannonPlayer(__instance) && GameReflection.ApplyGlassCannon(__instance))
		{
			ModEntry.Logger.Info("STS2Plus applied Glass Cannon to a player creature.", 1);
		}
	}
}
