using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using STS2Plus.Reflection;
using STS2Plus.Ui;

namespace STS2Plus.Patches;

[HarmonyPatchCategory("Core")]
[HarmonyPatch]
internal static class SpeedControlRunUiReadyPatch
{
	[CompilerGenerated]
	private sealed class _003CTargetMethods_003Ed__0 : IEnumerable<MethodBase>, IEnumerable, IEnumerator<MethodBase>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private MethodBase _003C_003E2__current;

		private int _003C_003El__initialThreadId;

		private Type _003CmapScreenType_003E5__1;

		private Type _003CrunType_003E5__2;

		private MethodInfo _003Cready_003E5__3;

		private MethodInfo _003Cready_003E5__4;

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
			_003CmapScreenType_003E5__1 = null;
			_003CrunType_003E5__2 = null;
			_003Cready_003E5__3 = null;
			_003Cready_003E5__4 = null;
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
				_003CmapScreenType_003E5__1 = RuntimeTypeResolver.FindType("MegaCrit.Sts2.Core.Nodes.Screens.Map.NMapScreen") ?? RuntimeTypeResolver.FindTypeByName("NMapScreen");
				if (_003CmapScreenType_003E5__1 != null)
				{
					_003Cready_003E5__3 = AccessTools.Method(_003CmapScreenType_003E5__1, "_Ready", (Type[])null, (Type[])null);
					if (_003Cready_003E5__3 != null)
					{
						_003C_003E2__current = _003Cready_003E5__3;
						_003C_003E1__state = 1;
						return true;
					}
					goto IL_00a3;
				}
				goto IL_00ab;
			case 1:
				_003C_003E1__state = -1;
				goto IL_00a3;
			case 2:
				{
					_003C_003E1__state = -1;
					goto IL_0122;
				}
				IL_00ab:
				_003CrunType_003E5__2 = RuntimeTypeResolver.FindType("MegaCrit.Sts2.Core.Nodes.NRun") ?? RuntimeTypeResolver.FindTypeByName("NRun");
				if (!(_003CrunType_003E5__2 != null))
				{
					break;
				}
				_003Cready_003E5__4 = AccessTools.Method(_003CrunType_003E5__2, "_Ready", (Type[])null, (Type[])null);
				if (_003Cready_003E5__4 != null)
				{
					_003C_003E2__current = _003Cready_003E5__4;
					_003C_003E1__state = 2;
					return true;
				}
				goto IL_0122;
				IL_00a3:
				_003Cready_003E5__3 = null;
				goto IL_00ab;
				IL_0122:
				_003Cready_003E5__4 = null;
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
	[HarmonyTargetMethods]
	private static IEnumerable<MethodBase> TargetMethods()
	{
		//yield-return decompiler failed: Unexpected instruction in Iterator.Dispose()
		return new _003CTargetMethods_003Ed__0(-2);
	}

	private static void Postfix()
	{
		SpeedControlOverlay.SetMainMenuVisible(visible: false);
		SpeedControlOverlay.Show();
	}
}
