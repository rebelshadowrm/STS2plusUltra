using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using STS2Plus.Config;
using STS2Plus.Reflection;

namespace STS2Plus.Patches;

[HarmonyPatchCategory("MoreRules")]
[HarmonyPatch]
internal static class CustomModifierRunStateSyncPatch
{
	[CompilerGenerated]
	private sealed class _003CTargetMethods_003Ed__0 : IEnumerable<MethodBase>, IEnumerable, IEnumerator<MethodBase>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private MethodBase _003C_003E2__current;

		private int _003C_003El__initialThreadId;

		private Type _003CcombatRoomType_003E5__1;

		private Type _003CmapScreenType_003E5__2;

		private Type _003CrunType_003E5__3;

		private MethodInfo _003CcombatSetup_003E5__4;

		private MethodInfo _003Cready_003E5__5;

		private MethodInfo _003Cready_003E5__6;

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
			_003CcombatRoomType_003E5__1 = null;
			_003CmapScreenType_003E5__2 = null;
			_003CrunType_003E5__3 = null;
			_003CcombatSetup_003E5__4 = null;
			_003Cready_003E5__5 = null;
			_003Cready_003E5__6 = null;
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
				_003CcombatRoomType_003E5__1 = RuntimeTypeResolver.FindType("MegaCrit.Sts2.Core.Nodes.Rooms.NCombatRoom") ?? RuntimeTypeResolver.FindTypeByName("NCombatRoom");
				if (_003CcombatRoomType_003E5__1 != null)
				{
					_003CcombatSetup_003E5__4 = AccessTools.Method(_003CcombatRoomType_003E5__1, "OnCombatSetUp", (Type[])null, (Type[])null);
					if (_003CcombatSetup_003E5__4 != null)
					{
						_003C_003E2__current = _003CcombatSetup_003E5__4;
						_003C_003E1__state = 1;
						return true;
					}
					goto IL_00af;
				}
				goto IL_00b7;
			case 1:
				_003C_003E1__state = -1;
				goto IL_00af;
			case 2:
				_003C_003E1__state = -1;
				goto IL_012e;
			case 3:
				{
					_003C_003E1__state = -1;
					goto IL_01af;
				}
				IL_012e:
				_003Cready_003E5__5 = null;
				goto IL_0136;
				IL_0136:
				_003CrunType_003E5__3 = RuntimeTypeResolver.FindType("MegaCrit.Sts2.Core.Nodes.NRun") ?? RuntimeTypeResolver.FindTypeByName("NRun");
				if (!(_003CrunType_003E5__3 != null))
				{
					break;
				}
				_003Cready_003E5__6 = AccessTools.Method(_003CrunType_003E5__3, "_Ready", (Type[])null, (Type[])null);
				if (_003Cready_003E5__6 != null)
				{
					_003C_003E2__current = _003Cready_003E5__6;
					_003C_003E1__state = 3;
					return true;
				}
				goto IL_01af;
				IL_01af:
				_003Cready_003E5__6 = null;
				break;
				IL_00af:
				_003CcombatSetup_003E5__4 = null;
				goto IL_00b7;
				IL_00b7:
				_003CmapScreenType_003E5__2 = RuntimeTypeResolver.FindType("MegaCrit.Sts2.Core.Nodes.Screens.Map.NMapScreen") ?? RuntimeTypeResolver.FindTypeByName("NMapScreen");
				if (_003CmapScreenType_003E5__2 != null)
				{
					_003Cready_003E5__5 = AccessTools.Method(_003CmapScreenType_003E5__2, "_Ready", (Type[])null, (Type[])null);
					if (_003Cready_003E5__5 != null)
					{
						_003C_003E2__current = _003Cready_003E5__5;
						_003C_003E1__state = 2;
						return true;
					}
					goto IL_012e;
				}
				goto IL_0136;
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
		if (!ConfigManager.Current.MoreRulesEnabled)
		{
			PlusState.ClearGameplayRuleSelections();
			return;
		}
		PlusState.SyncRuleSelectionsFromRunState();
		MultiplayerRuleSyncCoordinator.BroadcastCurrentSelectionIfHost();
		if (GameReflection.IsMultiplayerRun())
		{
			ModEntry.Logger.Info($"STS2Plus.Net sync source=run-state authoritative={MultiplayerSafety.ShouldApplyAuthoritativeGameplayPatches()} service={MultiplayerReflection.DescribeCurrentService()}", 1);
		}
		CardRuleHelpers.ReapplyBonusesToAllPlayerDecks();
	}
}
