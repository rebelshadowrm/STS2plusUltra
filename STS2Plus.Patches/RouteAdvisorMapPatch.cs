using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using Godot;
using HarmonyLib;
using STS2Plus.Reflection;
using STS2Plus.Ui;

namespace STS2Plus.Patches;

[HarmonyPatchCategory("Core")]
[HarmonyPatch]
internal static class RouteAdvisorMapPatch
{
	[CompilerGenerated]
	private sealed class _003CTargetMethods_003Ed__0 : IEnumerable<MethodBase>, IEnumerable, IEnumerator<MethodBase>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private MethodBase _003C_003E2__current;

		private int _003C_003El__initialThreadId;

		private Type _003CmapScreenType_003E5__1;

		private string[] _003C_003Es__2;

		private int _003C_003Es__3;

		private string _003CmethodName_003E5__4;

		private MethodInfo _003Cmethod_003E5__5;

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
			_003C_003El__initialThreadId = System.Environment.CurrentManagedThreadId;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
			_003CmapScreenType_003E5__1 = null;
			_003C_003Es__2 = null;
			_003CmethodName_003E5__4 = null;
			_003Cmethod_003E5__5 = null;
			_003C_003E1__state = -2;
		}

		private bool MoveNext()
		{
			int num = _003C_003E1__state;
			if (num != 0)
			{
				if (num != 1)
				{
					return false;
				}
				_003C_003E1__state = -1;
				goto IL_00ed;
			}
			_003C_003E1__state = -1;
			_003CmapScreenType_003E5__1 = RuntimeTypeResolver.FindType("MegaCrit.Sts2.Core.Nodes.Screens.Map.NMapScreen") ?? RuntimeTypeResolver.FindTypeByName("NMapScreen");
			if (_003CmapScreenType_003E5__1 == null)
			{
				return false;
			}
			_003C_003Es__2 = new string[5] { "_Ready", "SetMap", "OnMapPointSelectedLocally", "RefreshAllMapPointVotes", "RefreshAllPointVisuals" };
			_003C_003Es__3 = 0;
			goto IL_010a;
			IL_010a:
			if (_003C_003Es__3 < _003C_003Es__2.Length)
			{
				_003CmethodName_003E5__4 = _003C_003Es__2[_003C_003Es__3];
				_003Cmethod_003E5__5 = AccessTools.Method(_003CmapScreenType_003E5__1, _003CmethodName_003E5__4, (Type[])null, (Type[])null);
				if (_003Cmethod_003E5__5 != null)
				{
					_003C_003E2__current = _003Cmethod_003E5__5;
					_003C_003E1__state = 1;
					return true;
				}
				goto IL_00ed;
			}
			_003C_003Es__2 = null;
			return false;
			IL_00ed:
			_003Cmethod_003E5__5 = null;
			_003CmethodName_003E5__4 = null;
			_003C_003Es__3++;
			goto IL_010a;
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
			if (_003C_003E1__state == -2 && _003C_003El__initialThreadId == System.Environment.CurrentManagedThreadId)
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

	private static void Postfix(object __instance)
	{
		Node val = (Node)((__instance is Node) ? __instance : null);
		if (val != null)
		{
			ModEntry.Verbose("RouteAdvisor: map update triggered");
			RouteAdvisorHighlighter.Refresh(val);
		}
	}
}
