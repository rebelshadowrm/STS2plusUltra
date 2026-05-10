using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Map;
using MegaCrit.Sts2.Core.Models;
using STS2Plus.Modifiers;
using STS2Plus.Patches;

namespace STS2Plus.Reflection;

internal static class GameReflection
{
	private sealed class ReferenceEqualityComparer : IEqualityComparer<object>
	{
		public static readonly ReferenceEqualityComparer Instance = new ReferenceEqualityComparer();

		public new bool Equals(object? x, object? y)
		{
			return x == y;
		}

		public int GetHashCode(object obj)
		{
			return RuntimeHelpers.GetHashCode(obj);
		}
	}

	[CompilerGenerated]
	private sealed class _003CEnumerateGlassCannonMaxHpCandidates_003Ed__146 : IEnumerable<object>, IEnumerable, IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object? _003C_003E2__current;

		private int _003C_003El__initialThreadId;

		private object instance;

		public object _003C_003E3__instance;

		private object _003CowningPlayer_003E5__1;

		private IEnumerator<object> _003C_003Es__2;

		private object _003ChealthHolder_003E5__3;

		private IEnumerator<object> _003C_003Es__4;

		private object _003ChealthHolder_003E5__5;

		object? IEnumerator<object>.Current
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
		public _003CEnumerateGlassCannonMaxHpCandidates_003Ed__146(int _003C_003E1__state)
		{
			this._003C_003E1__state = _003C_003E1__state;
			_003C_003El__initialThreadId = System.Environment.CurrentManagedThreadId;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
			switch (_003C_003E1__state)
			{
			case -3:
			case 4:
				try
				{
				}
				finally
				{
					_003C_003Em__Finally1();
				}
				break;
			case -4:
			case 5:
				try
				{
				}
				finally
				{
					_003C_003Em__Finally2();
				}
				break;
			}
			_003CowningPlayer_003E5__1 = null;
			_003C_003Es__2 = null;
			_003ChealthHolder_003E5__3 = null;
			_003C_003Es__4 = null;
			_003ChealthHolder_003E5__5 = null;
			_003C_003E1__state = -2;
		}

		private bool MoveNext()
		{
			try
			{
				switch (_003C_003E1__state)
				{
				default:
					return false;
				case 0:
					_003C_003E1__state = -1;
					_003CowningPlayer_003E5__1 = ResolveOwningPlayer(instance);
					if (_003CowningPlayer_003E5__1 != null)
					{
						_003C_003E2__current = GetPlayerCreature(_003CowningPlayer_003E5__1);
						_003C_003E1__state = 1;
						return true;
					}
					goto IL_00b2;
				case 1:
					_003C_003E1__state = -1;
					_003C_003E2__current = _003CowningPlayer_003E5__1;
					_003C_003E1__state = 2;
					return true;
				case 2:
					_003C_003E1__state = -1;
					goto IL_00b2;
				case 3:
					_003C_003E1__state = -1;
					if (_003CowningPlayer_003E5__1 != null)
					{
						_003C_003Es__2 = ResolveHealthHolders(_003CowningPlayer_003E5__1).GetEnumerator();
						_003C_003E1__state = -3;
						goto IL_0141;
					}
					_003C_003Es__4 = ResolveHealthHolders(instance).GetEnumerator();
					_003C_003E1__state = -4;
					break;
				case 4:
					_003C_003E1__state = -3;
					_003ChealthHolder_003E5__3 = null;
					goto IL_0141;
				case 5:
					{
						_003C_003E1__state = -4;
						_003ChealthHolder_003E5__5 = null;
						break;
					}
					IL_0141:
					if (_003C_003Es__2.MoveNext())
					{
						_003ChealthHolder_003E5__3 = _003C_003Es__2.Current;
						_003C_003E2__current = _003ChealthHolder_003E5__3;
						_003C_003E1__state = 4;
						return true;
					}
					_003C_003Em__Finally1();
					_003C_003Es__2 = null;
					return false;
					IL_00b2:
					_003C_003E2__current = instance;
					_003C_003E1__state = 3;
					return true;
				}
				if (_003C_003Es__4.MoveNext())
				{
					_003ChealthHolder_003E5__5 = _003C_003Es__4.Current;
					_003C_003E2__current = _003ChealthHolder_003E5__5;
					_003C_003E1__state = 5;
					return true;
				}
				_003C_003Em__Finally2();
				_003C_003Es__4 = null;
				return false;
			}
			catch
			{
				//try-fault
				((IDisposable)this).Dispose();
				throw;
			}
		}

		bool IEnumerator.MoveNext()
		{
			//ILSpy generated this explicit interface implementation from .override directive in MoveNext
			return this.MoveNext();
		}

		private void _003C_003Em__Finally1()
		{
			_003C_003E1__state = -1;
			if (_003C_003Es__2 != null)
			{
				_003C_003Es__2.Dispose();
			}
		}

		private void _003C_003Em__Finally2()
		{
			_003C_003E1__state = -1;
			if (_003C_003Es__4 != null)
			{
				_003C_003Es__4.Dispose();
			}
		}

		[DebuggerHidden]
		void IEnumerator.Reset()
		{
			throw new NotSupportedException();
		}

		[DebuggerHidden]
		IEnumerator<object> IEnumerable<object>.GetEnumerator()
		{
			_003CEnumerateGlassCannonMaxHpCandidates_003Ed__146 _003CEnumerateGlassCannonMaxHpCandidates_003Ed__;
			if (_003C_003E1__state == -2 && _003C_003El__initialThreadId == System.Environment.CurrentManagedThreadId)
			{
				_003C_003E1__state = 0;
				_003CEnumerateGlassCannonMaxHpCandidates_003Ed__ = this;
			}
			else
			{
				_003CEnumerateGlassCannonMaxHpCandidates_003Ed__ = new _003CEnumerateGlassCannonMaxHpCandidates_003Ed__146(0);
			}
			_003CEnumerateGlassCannonMaxHpCandidates_003Ed__.instance = _003C_003E3__instance;
			return _003CEnumerateGlassCannonMaxHpCandidates_003Ed__;
		}

		[DebuggerHidden]
		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<object>)this).GetEnumerator();
		}
	}

	[CompilerGenerated]
	private sealed class _003CEnumerateHealthCandidates_003Ed__158 : IEnumerable<object>, IEnumerable, IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		private int _003C_003El__initialThreadId;

		private object instance;

		public object _003C_003E3__instance;

		private string[] _003C_003Es__1;

		private int _003C_003Es__2;

		private string _003CmemberName_003E5__3;

		private object _003Cvalue_003E5__4;

		object IEnumerator<object>.Current
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
		public _003CEnumerateHealthCandidates_003Ed__158(int _003C_003E1__state)
		{
			this._003C_003E1__state = _003C_003E1__state;
			_003C_003El__initialThreadId = System.Environment.CurrentManagedThreadId;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
			_003C_003Es__1 = null;
			_003CmemberName_003E5__3 = null;
			_003Cvalue_003E5__4 = null;
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
				goto IL_018b;
			}
			_003C_003E1__state = -1;
			_003C_003Es__1 = new string[20]
			{
				"Entity", "Creature", "Osty", "CurrentOsty", "CurrentCreature", "CurrentPlayerCreature", "PlayerCreature", "CombatCreature", "ActiveCreature", "CurrentEntity",
				"CanonicalEntity", "Player", "PlayerState", "State", "Health", "HealthState", "Stats", "CharacterState", "Owner", "PetOwner"
			};
			_003C_003Es__2 = 0;
			goto IL_01a8;
			IL_018b:
			_003Cvalue_003E5__4 = null;
			_003CmemberName_003E5__3 = null;
			_003C_003Es__2++;
			goto IL_01a8;
			IL_01a8:
			if (_003C_003Es__2 < _003C_003Es__1.Length)
			{
				_003CmemberName_003E5__3 = _003C_003Es__1[_003C_003Es__2];
				_003Cvalue_003E5__4 = TryGetPropertyValue(instance, _003CmemberName_003E5__3);
				if (_003Cvalue_003E5__4 == null)
				{
					try
					{
						_003Cvalue_003E5__4 = AccessTools.Field(instance.GetType(), _003CmemberName_003E5__3)?.GetValue(instance);
					}
					catch
					{
						_003Cvalue_003E5__4 = null;
					}
				}
				if (_003Cvalue_003E5__4 != null)
				{
					_003C_003E2__current = _003Cvalue_003E5__4;
					_003C_003E1__state = 1;
					return true;
				}
				goto IL_018b;
			}
			_003C_003Es__1 = null;
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
		IEnumerator<object> IEnumerable<object>.GetEnumerator()
		{
			_003CEnumerateHealthCandidates_003Ed__158 _003CEnumerateHealthCandidates_003Ed__;
			if (_003C_003E1__state == -2 && _003C_003El__initialThreadId == System.Environment.CurrentManagedThreadId)
			{
				_003C_003E1__state = 0;
				_003CEnumerateHealthCandidates_003Ed__ = this;
			}
			else
			{
				_003CEnumerateHealthCandidates_003Ed__ = new _003CEnumerateHealthCandidates_003Ed__158(0);
			}
			_003CEnumerateHealthCandidates_003Ed__.instance = _003C_003E3__instance;
			return _003CEnumerateHealthCandidates_003Ed__;
		}

		[DebuggerHidden]
		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<object>)this).GetEnumerator();
		}
	}

	[CompilerGenerated]
	private sealed class _003CEnumerateNestedObjects_003Ed__160 : IEnumerable<object>, IEnumerable, IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object? _003C_003E2__current;

		private int _003C_003El__initialThreadId;

		private object instance;

		public object _003C_003E3__instance;

		private Type _003Ctype_003E5__1;

		private PropertyInfo[] _003C_003Es__2;

		private int _003C_003Es__3;

		private PropertyInfo _003Cproperty_003E5__4;

		private object _003Cvalue_003E5__5;

		private FieldInfo[] _003C_003Es__6;

		private int _003C_003Es__7;

		private FieldInfo _003Cfield_003E5__8;

		private object _003Cvalue_003E5__9;

		object? IEnumerator<object>.Current
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
		public _003CEnumerateNestedObjects_003Ed__160(int _003C_003E1__state)
		{
			this._003C_003E1__state = _003C_003E1__state;
			_003C_003El__initialThreadId = System.Environment.CurrentManagedThreadId;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
			_003Ctype_003E5__1 = null;
			_003C_003Es__2 = null;
			_003Cproperty_003E5__4 = null;
			_003Cvalue_003E5__5 = null;
			_003C_003Es__6 = null;
			_003Cfield_003E5__8 = null;
			_003Cvalue_003E5__9 = null;
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
				_003Ctype_003E5__1 = instance.GetType();
				_003C_003Es__2 = _003Ctype_003E5__1.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				_003C_003Es__3 = 0;
				goto IL_0156;
			case 1:
				_003C_003E1__state = -1;
				goto IL_0139;
			case 2:
				{
					_003C_003E1__state = -1;
					goto IL_0242;
				}
				IL_0156:
				if (_003C_003Es__3 < _003C_003Es__2.Length)
				{
					_003Cproperty_003E5__4 = _003C_003Es__2[_003C_003Es__3];
					if (_003Cproperty_003E5__4.GetIndexParameters().Length != 0 || !_003Cproperty_003E5__4.CanRead || _003Cproperty_003E5__4.PropertyType.IsPrimitive || _003Cproperty_003E5__4.PropertyType == typeof(string) || _003Cproperty_003E5__4.PropertyType.IsEnum)
					{
						goto IL_0148;
					}
					_003Cvalue_003E5__5 = null;
					try
					{
						_003Cvalue_003E5__5 = _003Cproperty_003E5__4.GetValue(instance);
					}
					catch
					{
					}
					if (_003Cvalue_003E5__5 != null)
					{
						_003C_003E2__current = _003Cvalue_003E5__5;
						_003C_003E1__state = 1;
						return true;
					}
					goto IL_0139;
				}
				_003C_003Es__2 = null;
				_003C_003Es__6 = _003Ctype_003E5__1.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				_003C_003Es__7 = 0;
				goto IL_025f;
				IL_0139:
				_003Cvalue_003E5__5 = null;
				_003Cproperty_003E5__4 = null;
				goto IL_0148;
				IL_025f:
				if (_003C_003Es__7 < _003C_003Es__6.Length)
				{
					_003Cfield_003E5__8 = _003C_003Es__6[_003C_003Es__7];
					if (_003Cfield_003E5__8.FieldType.IsPrimitive || _003Cfield_003E5__8.FieldType == typeof(string) || _003Cfield_003E5__8.FieldType.IsEnum)
					{
						goto IL_0251;
					}
					_003Cvalue_003E5__9 = null;
					try
					{
						_003Cvalue_003E5__9 = _003Cfield_003E5__8.GetValue(instance);
					}
					catch
					{
					}
					if (_003Cvalue_003E5__9 != null)
					{
						_003C_003E2__current = _003Cvalue_003E5__9;
						_003C_003E1__state = 2;
						return true;
					}
					goto IL_0242;
				}
				_003C_003Es__6 = null;
				return false;
				IL_0148:
				_003C_003Es__3++;
				goto IL_0156;
				IL_0242:
				_003Cvalue_003E5__9 = null;
				_003Cfield_003E5__8 = null;
				goto IL_0251;
				IL_0251:
				_003C_003Es__7++;
				goto IL_025f;
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
		IEnumerator<object> IEnumerable<object>.GetEnumerator()
		{
			_003CEnumerateNestedObjects_003Ed__160 _003CEnumerateNestedObjects_003Ed__;
			if (_003C_003E1__state == -2 && _003C_003El__initialThreadId == System.Environment.CurrentManagedThreadId)
			{
				_003C_003E1__state = 0;
				_003CEnumerateNestedObjects_003Ed__ = this;
			}
			else
			{
				_003CEnumerateNestedObjects_003Ed__ = new _003CEnumerateNestedObjects_003Ed__160(0);
			}
			_003CEnumerateNestedObjects_003Ed__.instance = _003C_003E3__instance;
			return _003CEnumerateNestedObjects_003Ed__;
		}

		[DebuggerHidden]
		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<object>)this).GetEnumerator();
		}
	}

	[CompilerGenerated]
	private sealed class _003CGetPlayers_003Ed__75 : IEnumerable<object>, IEnumerable, IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		private int _003C_003El__initialThreadId;

		private object _003Cstate_003E5__1;

		private IEnumerable _003Cplayers_003E5__2;

		private IEnumerator _003C_003Es__3;

		private object _003Cplayer_003E5__4;

		object IEnumerator<object>.Current
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
		public _003CGetPlayers_003Ed__75(int _003C_003E1__state)
		{
			this._003C_003E1__state = _003C_003E1__state;
			_003C_003El__initialThreadId = System.Environment.CurrentManagedThreadId;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
			int num = _003C_003E1__state;
			if (num == -3 || num == 1)
			{
				try
				{
				}
				finally
				{
					_003C_003Em__Finally1();
				}
			}
			_003Cstate_003E5__1 = null;
			_003Cplayers_003E5__2 = null;
			_003C_003Es__3 = null;
			_003Cplayer_003E5__4 = null;
			_003C_003E1__state = -2;
		}

		private bool MoveNext()
		{
			try
			{
				int num = _003C_003E1__state;
				if (num != 0)
				{
					if (num != 1)
					{
						return false;
					}
					_003C_003E1__state = -3;
					goto IL_00e4;
				}
				_003C_003E1__state = -1;
				_003Cstate_003E5__1 = GetRunState();
				if (_003Cstate_003E5__1 == null)
				{
					return false;
				}
				object obj = StatePlayersProperty?.GetValue(_003Cstate_003E5__1);
				_003Cplayers_003E5__2 = obj as IEnumerable;
				if (_003Cplayers_003E5__2 == null)
				{
					return false;
				}
				_003C_003Es__3 = _003Cplayers_003E5__2.GetEnumerator();
				_003C_003E1__state = -3;
				goto IL_00ec;
				IL_00e4:
				_003Cplayer_003E5__4 = null;
				goto IL_00ec;
				IL_00ec:
				if (_003C_003Es__3.MoveNext())
				{
					_003Cplayer_003E5__4 = _003C_003Es__3.Current;
					if (_003Cplayer_003E5__4 != null)
					{
						_003C_003E2__current = _003Cplayer_003E5__4;
						_003C_003E1__state = 1;
						return true;
					}
					goto IL_00e4;
				}
				_003C_003Em__Finally1();
				_003C_003Es__3 = null;
				return false;
			}
			catch
			{
				//try-fault
				((IDisposable)this).Dispose();
				throw;
			}
		}

		bool IEnumerator.MoveNext()
		{
			//ILSpy generated this explicit interface implementation from .override directive in MoveNext
			return this.MoveNext();
		}

		private void _003C_003Em__Finally1()
		{
			_003C_003E1__state = -1;
			if (_003C_003Es__3 is IDisposable disposable)
			{
				disposable.Dispose();
			}
		}

		[DebuggerHidden]
		void IEnumerator.Reset()
		{
			throw new NotSupportedException();
		}

		[DebuggerHidden]
		IEnumerator<object> IEnumerable<object>.GetEnumerator()
		{
			if (_003C_003E1__state == -2 && _003C_003El__initialThreadId == System.Environment.CurrentManagedThreadId)
			{
				_003C_003E1__state = 0;
				return this;
			}
			return new _003CGetPlayers_003Ed__75(0);
		}

		[DebuggerHidden]
		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<object>)this).GetEnumerator();
		}
	}

	private static readonly Dictionary<object, bool> PendingGlassCannonEventStarts = new Dictionary<object, bool>(ReferenceEqualityComparer.Instance);

	private static readonly Type? CreatureType = AccessTools.TypeByName("MegaCrit.Sts2.Core.Entities.Creatures.Creature");

	private static readonly Type? CardModelType = AccessTools.TypeByName("MegaCrit.Sts2.Core.Models.CardModel");

	private static readonly Type? RunManagerType = AccessTools.TypeByName("MegaCrit.Sts2.Core.Runs.RunManager");

	private static readonly Type? RunStateType = AccessTools.TypeByName("MegaCrit.Sts2.Core.Runs.RunState");

	private static readonly PropertyInfo? CreatureCurrentHpProperty = AccessTools.Property(CreatureType, "CurrentHp");

	private static readonly PropertyInfo? CreatureMaxHpProperty = AccessTools.Property(CreatureType, "MaxHp");

	private static readonly PropertyInfo? CreatureBlockProperty = AccessTools.Property(CreatureType, "Block");

	private static readonly PropertyInfo? CreatureIsEnemyProperty = AccessTools.Property(CreatureType, "IsEnemy");

	private static readonly PropertyInfo? CreatureIsMonsterProperty = AccessTools.Property(CreatureType, "IsMonster");

	private static readonly PropertyInfo? CreatureIsPlayerProperty = AccessTools.Property(CreatureType, "IsPlayer");

	private static readonly PropertyInfo? CreatureIsPetProperty = AccessTools.Property(CreatureType, "IsPet");

	private static readonly PropertyInfo? CreatureNameProperty = AccessTools.Property(CreatureType, "Name");

	private static readonly PropertyInfo? CreaturePlayerProperty = AccessTools.Property(CreatureType, "Player");

	private static readonly PropertyInfo? CreaturePetOwnerProperty = AccessTools.Property(CreatureType, "PetOwner");

	private static readonly PropertyInfo? CreatureCombatStateProperty = AccessTools.Property(CreatureType, "CombatState");

	private static readonly PropertyInfo? PlayerIsActiveForHooksProperty = AccessTools.Property(AccessTools.TypeByName("MegaCrit.Sts2.Core.Entities.Players.Player"), "IsActiveForHooks");

	private static readonly PropertyInfo? PlayerMaxEnergyProperty = AccessTools.Property(AccessTools.TypeByName("MegaCrit.Sts2.Core.Entities.Players.Player"), "MaxEnergy");

	private static readonly PropertyInfo? PlayerCombatStateProperty = AccessTools.Property(AccessTools.TypeByName("MegaCrit.Sts2.Core.Entities.Players.Player"), "PlayerCombatState");

	private static readonly Type? PlayerCombatStateType = AccessTools.TypeByName("MegaCrit.Sts2.Core.Entities.Players.PlayerCombatState");

	private static readonly PropertyInfo? PlayerCombatStateEnergyProperty = AccessTools.Property(PlayerCombatStateType, "Energy");

	private static readonly PropertyInfo? PlayerCombatStateMaxEnergyProperty = AccessTools.Property(PlayerCombatStateType, "MaxEnergy");

	private static readonly MethodInfo? CreatureSetCurrentHpInternalMethod = AccessTools.Method(CreatureType, "SetCurrentHpInternal", (Type[])null, (Type[])null);

	private static readonly MethodInfo? CreatureSetMaxHpInternalMethod = AccessTools.Method(CreatureType, "SetMaxHpInternal", (Type[])null, (Type[])null);

	private static readonly MethodInfo? PlayerActivateHooksMethod = AccessTools.Method(AccessTools.TypeByName("MegaCrit.Sts2.Core.Entities.Players.Player"), "ActivateHooks", (Type[])null, (Type[])null);

	private static readonly PropertyInfo? CardBaseDamageProperty = AccessTools.Property(CardModelType, "BaseDamage");

	private static readonly PropertyInfo? CardDamageProperty = AccessTools.Property(CardModelType, "Damage");

	private static readonly PropertyInfo? CardBaseBlockProperty = AccessTools.Property(CardModelType, "BaseBlock");

	private static readonly PropertyInfo? CardBlockProperty = AccessTools.Property(CardModelType, "Block");

	private static readonly PropertyInfo? CardIdProperty = AccessTools.Property(CardModelType, "Id");

	private static readonly PropertyInfo? CardTypeProperty = AccessTools.Property(CardModelType, "Type");

	private static readonly PropertyInfo? CardGainsBlockProperty = AccessTools.Property(CardModelType, "GainsBlock");

	private static readonly PropertyInfo? CardTagsProperty = AccessTools.Property(CardModelType, "Tags");

	private static readonly PropertyInfo? DynamicVarNameProperty = AccessTools.Property(AccessTools.TypeByName("MegaCrit.Sts2.Core.Localization.DynamicVars.DynamicVar"), "Name");

	private static readonly PropertyInfo? DynamicVarBaseValueProperty = AccessTools.Property(AccessTools.TypeByName("MegaCrit.Sts2.Core.Localization.DynamicVars.DynamicVar"), "BaseValue");

	private static readonly PropertyInfo? CardDynamicVarsProperty = AccessTools.Property(CardModelType, "DynamicVars");

	private static readonly string[] DeathStateFalseMembers = new string[28]
	{
		"IsDead", "_isDead", "<IsDead>k__BackingField", "Dead", "_dead", "<Dead>k__BackingField", "IsDying", "_isDying", "<IsDying>k__BackingField", "HasDied",
		"_hasDied", "<HasDied>k__BackingField", "DeadThisCombat", "_deadThisCombat", "<DeadThisCombat>k__BackingField", "DeadThisTurn", "_deadThisTurn", "<DeadThisTurn>k__BackingField", "IsHalfDead", "isHalfDead",
		"_isHalfDead", "<IsHalfDead>k__BackingField", "IsLocalPlayerDead", "_isLocalPlayerDead", "<IsLocalPlayerDead>k__BackingField", "IsPlayingDeathAnimation", "_isPlayingDeathAnimation", "<IsPlayingDeathAnimation>k__BackingField"
	};

	private static readonly string[] AliveStateTrueMembers = new string[6] { "IsAlive", "_isAlive", "<IsAlive>k__BackingField", "Alive", "_alive", "<Alive>k__BackingField" };

	private static readonly PropertyInfo? RunManagerInstanceProperty = AccessTools.Property(RunManagerType, "Instance");

	private static readonly PropertyInfo? RunManagerStateProperty = AccessTools.Property(RunManagerType, "State");

	private static readonly FieldInfo? RunManagerStateField = AccessTools.Field(RunManagerType, "_state") ?? AccessTools.Field(RunManagerType, "_runState");

	private static readonly PropertyInfo? RunStateCurrentActIndexProperty = AccessTools.Property(RunStateType, "CurrentActIndex");

	private static readonly PropertyInfo? RunStateActFloorProperty = AccessTools.Property(RunStateType, "ActFloor");

	private static readonly PropertyInfo? RunStateMapProperty = AccessTools.Property(RunStateType, "Map");

	private static readonly PropertyInfo? RunStateCurrentRoomProperty = AccessTools.Property(RunStateType, "CurrentRoom");

	private static readonly PropertyInfo? RunStateCurrentMapPointProperty = AccessTools.Property(RunStateType, "CurrentMapPoint");

	private static readonly PropertyInfo? RunStateRngProperty = AccessTools.Property(RunStateType, "Rng");

	private static readonly PropertyInfo? StatePlayersProperty = AccessTools.Property(RunStateType, "Players");

	private static readonly PropertyInfo? RunStateModifiersProperty = AccessTools.Property(RunStateType, "Modifiers");

	private static readonly Type? RunRngSetType = AccessTools.TypeByName("MegaCrit.Sts2.Core.Runs.RunRngSet");

	private static readonly Type? NGameType = AccessTools.TypeByName("MegaCrit.Sts2.Core.Nodes.NGame");

	private static readonly PropertyInfo? NGameInstanceProperty = NGameType != null ? AccessTools.Property(NGameType, "Instance") : null;

	private static readonly MethodInfo? NGameLoadRunMethod = NGameType != null ? AccessTools.Method(NGameType, "LoadRun", (Type[])null, (Type[])null) : null;

	private static readonly MethodInfo? HookShouldClearBlockMethod = AccessTools.Method(AccessTools.TypeByName("MegaCrit.Sts2.Core.Hooks.Hook"), "ShouldClearBlock", (Type[])null, (Type[])null);

	public static Type? CreatureRuntimeType => CreatureType;

	public static Type? CardRuntimeType => CardModelType;

	public static bool IsEnemyCreature(object? creature)
	{
		if (creature == null)
		{
			return false;
		}
		if (GetBool(CreatureIsPlayerProperty, creature) || GetBool(CreatureIsPetProperty, creature))
		{
			return false;
		}
		return GetBool(CreatureIsEnemyProperty, creature) || GetBool(CreatureIsMonsterProperty, creature);
	}

	public static bool IsPlayerSideCreature(object? creature)
	{
		if (creature == null)
		{
			return false;
		}
		if (GetBool(CreatureIsPlayerProperty, creature) || GetBool(CreatureIsPetProperty, creature))
		{
			return true;
		}
		string name = creature.GetType().Name;
		return name.Contains("Player", StringComparison.OrdinalIgnoreCase) || name.Contains("Pet", StringComparison.OrdinalIgnoreCase) || name.Contains("Companion", StringComparison.OrdinalIgnoreCase);
	}

	public static bool ShouldCountPlayerDamage(object? dealer, object? target)
	{
		if (!IsEnemyCreature(target))
		{
			return false;
		}
		if (dealer == null)
		{
			return true;
		}
		if (IsEnemyCreature(dealer))
		{
			return false;
		}
		object obj = GetCachedPropertyValue(CreaturePlayerProperty, dealer) ?? GetCachedPropertyValue(CreaturePetOwnerProperty, dealer);
		if (obj == null)
		{
			return IsPlayerSideCreature(dealer);
		}
		return (PlayerIsActiveForHooksProperty?.GetValue(obj) as bool?) ?? IsPlayerSideCreature(dealer);
	}

	public static bool IsMultiplayerRun()
	{
		try
		{
			object runState = GetRunState();
			if (runState == null)
			{
				return false;
			}
			object obj = StatePlayersProperty?.GetValue(runState);
			return obj is ICollection collection && collection.Count > 1;
		}
		catch (Exception ex)
		{
			ModEntry.Logger.Warn("Failed to inspect run state: " + ex.Message, 1);
			return false;
		}
	}

	public static int GetCurrentHp(object creature)
	{
		return GetInt(creature, CreatureCurrentHpProperty, "CurrentHp", "_currentHp", "currentHp", "<CurrentHp>k__BackingField", "CurrentHealth", "_currentHealth", "currentHealth");
	}

	public static int GetMaxHp(object creature)
	{
		return GetInt(creature, CreatureMaxHpProperty, "MaxHp", "_maxHp", "maxHp", "<MaxHp>k__BackingField", "MaxHealth", "_maxHealth", "maxHealth");
	}

	public static int GetCurrentBlock(object creature)
	{
		return GetInt(creature, CreatureBlockProperty, "Block", "_block", "block", "CurrentBlock");
	}

	public static void SetCurrentBlock(object creature, int value)
	{
		SetInt(creature, value, CreatureBlockProperty, "Block", "_block", "block", "CurrentBlock");
	}

	public static void SetCurrentHp(object creature, int value)
	{
		SetInt(creature, value, CreatureCurrentHpProperty, "CurrentHp", "_currentHp", "currentHp", "<CurrentHp>k__BackingField", "CurrentHealth", "_currentHealth", "currentHealth");
	}

	public static void SetMaxHp(object creature, int value)
	{
		SetInt(creature, value, CreatureMaxHpProperty, "MaxHp", "_maxHp", "maxHp", "<MaxHp>k__BackingField", "MaxHealth", "_maxHealth", "maxHealth");
	}

	public static string DescribeCreature(object creature)
	{
		try
		{
			if (CreatureNameProperty?.GetValue(creature) is string text && !string.IsNullOrWhiteSpace(text))
			{
				return text;
			}
		}
		catch
		{
		}
		try
		{
			if (AccessTools.Property(creature.GetType(), "Name")?.GetValue(creature) is string text2 && !string.IsNullOrWhiteSpace(text2))
			{
				return text2;
			}
		}
		catch
		{
		}
		return creature.GetType().Name;
	}

	public static object? GetCreatureEntity(object? creatureNode)
	{
		if (creatureNode == null)
		{
			return null;
		}
		return AccessTools.Property(creatureNode.GetType(), "Entity")?.GetValue(creatureNode);
	}

	public static bool IsLocalPlayerCreature(object? creature)
	{
		if (creature == null || !GetBool(CreatureIsPlayerProperty, creature))
		{
			return false;
		}
		object cachedPropertyValue = GetCachedPropertyValue(CreaturePlayerProperty, creature);
		return cachedPropertyValue == null || PlayerIsActiveForHooksProperty?.GetValue(cachedPropertyValue) as bool? != false;
	}

	public static bool IsPlayerCreature(object? creature)
	{
		return creature != null && GetBool(CreatureIsPlayerProperty, creature);
	}

	public static bool IsLocalPlayerObject(object? instance)
	{
		if (instance == null)
		{
			return false;
		}
		if (GetBool(CreatureIsPlayerProperty, instance))
		{
			return IsLocalPlayerCreature(instance);
		}
		PropertyInfo? playerIsActiveForHooksProperty = PlayerIsActiveForHooksProperty;
		if ((object)playerIsActiveForHooksProperty != null && (playerIsActiveForHooksProperty.DeclaringType?.IsInstanceOfType(instance)).GetValueOrDefault())
		{
			return PlayerIsActiveForHooksProperty.GetValue(instance) as bool? != false;
		}
		string[] array = new string[5] { "Player", "Owner", "Creature", "Entity", "PetOwner" };
		foreach (string text in array)
		{
			try
			{
				object obj = AccessTools.Property(instance.GetType(), text)?.GetValue(instance);
				if (obj != null && obj != instance && IsLocalPlayerObject(obj))
				{
					return true;
				}
			}
			catch
			{
			}
		}
		return false;
	}

	public static bool IsAnyPlayerCreature(object? creature)
	{
		if (creature == null)
		{
			return false;
		}
		if (IsPlayerSideCreature(creature))
		{
			return true;
		}
		return ResolveOwningPlayer(creature) != null || creature.GetType().Name.Contains("Player", StringComparison.OrdinalIgnoreCase);
	}

	public static Vector2? GetCreatureIntentAnchor(Node? creatureNode)
	{
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		if (creatureNode == null || !GodotObject.IsInstanceValid((GodotObject)(object)creatureNode))
		{
			return null;
		}
		if (AccessTools.Property(((object)creatureNode).GetType(), "VfxSpawnPosition")?.GetValue(creatureNode) is Vector2 value)
		{
			return value;
		}
		object obj = AccessTools.Property(((object)creatureNode).GetType(), "Visuals")?.GetValue(creatureNode);
		if (obj == null)
		{
			return GetNodeGlobalPosition(creatureNode);
		}
		object? obj2 = AccessTools.Property(obj.GetType(), "IntentPosition")?.GetValue(obj);
		Marker2D val = (Marker2D)((obj2 is Marker2D) ? obj2 : null);
		if (val != null)
		{
			return ((Node2D)val).GlobalPosition;
		}
		Node val2 = (Node)((obj is Node) ? obj : null);
		return (val2 != null) ? GetNodeGlobalPosition(val2) : GetNodeGlobalPosition(creatureNode);
	}

	public static string DescribeCard(object card)
	{
		return (CardIdProperty?.GetValue(card) as string) ?? card.GetType().Name;
	}

	public static int GetCardDisplayedDamage(object? card)
	{
		if (card == null)
		{
			return 0;
		}
		if (TryReadInt(CardDamageProperty, card, out var value) && value > 0)
		{
			return value;
		}
		int value2;
		return TryReadInt(CardBaseDamageProperty, card, out value2) ? Math.Max(0, value2) : 0;
	}

	public static int GetCardMultiPlayCount(object? card)
	{
		if (card == null)
		{
			return 1;
		}
		int num = 1;
		string[] array = new string[15]
		{
			"HitCount", "_hitCount", "hitCount", "Repeats", "_repeats", "repeats", "RepeatCount", "_repeatCount", "repeatCount", "CurrentRepeats",
			"_currentRepeats", "currentRepeats", "Multiplier", "_multiplier", "multiplier"
		};
		foreach (string text in array)
		{
			int @int = GetInt(card, AccessTools.Property(card.GetType(), text), text);
			if (@int > num)
			{
				num = @int;
			}
		}
		return Math.Max(1, num);
	}

	[IteratorStateMachine(typeof(_003CGetPlayers_003Ed__75))]
	public static IEnumerable<object> GetPlayers()
	{
		//yield-return decompiler failed: Unexpected instruction in Iterator.Dispose()
		return new _003CGetPlayers_003Ed__75(-2);
	}

	public static string GetPlayerStateKey(object? instance)
	{
		object obj = ((instance == null) ? null : (ResolveOwningPlayer(instance) ?? instance));
		if (obj == null)
		{
			return string.Empty;
		}
		string[] array = new string[7] { "PlayerId", "NetId", "SteamId", "UserId", "ProfileId", "PlatformId", "Id" };
		foreach (string text in array)
		{
			object obj2 = TryGetPropertyValue(obj, text) ?? AccessTools.Field(obj.GetType(), text)?.GetValue(obj);
			if (obj2 != null)
			{
				string text2 = obj2.ToString();
				if (!string.IsNullOrWhiteSpace(text2) && !string.Equals(text2, "0", StringComparison.Ordinal))
				{
					return text + ":" + text2;
				}
			}
		}
		int num = 0;
		foreach (object player in GetPlayers())
		{
			if (player == obj)
			{
				return $"index:{num}";
			}
			num++;
		}
		string[] array2 = new string[4] { "Character", "CharacterType", "Class", "Name" };
		foreach (string text3 in array2)
		{
			string text4 = TryGetPropertyValue(obj, text3)?.ToString();
			if (!string.IsNullOrWhiteSpace(text4))
			{
				return text3 + ":" + text4;
			}
		}
		return obj.GetType().FullName ?? obj.GetType().Name;
	}

	public static bool IsRunActive()
	{
		return GetRunState() != null;
	}

	public static bool IsCurrentCombatRewardRoom()
	{
		string currentRoomType = GetCurrentRoomType();
		return string.Equals(currentRoomType, "Monster", StringComparison.OrdinalIgnoreCase) || string.Equals(currentRoomType, "Elite", StringComparison.OrdinalIgnoreCase) || string.Equals(currentRoomType, "Boss", StringComparison.OrdinalIgnoreCase);
	}

	public static bool IsCombatRewardRoom(object? room)
	{
		if (room == null)
		{
			return false;
		}
		string a = TryGetPropertyValue(room, "RoomType")?.ToString();
		return string.Equals(a, "Monster", StringComparison.OrdinalIgnoreCase) || string.Equals(a, "Elite", StringComparison.OrdinalIgnoreCase) || string.Equals(a, "Boss", StringComparison.OrdinalIgnoreCase);
	}

	public static bool IsCurrentEliteRoom()
	{
		return string.Equals(GetCurrentRoomType(), "Elite", StringComparison.OrdinalIgnoreCase);
	}

	public static bool IsEliteEncounterContext(object? instance)
	{
		if (instance == null)
		{
			return IsCurrentEliteRoom();
		}
		if (HasEliteRoomType(instance))
		{
			return true;
		}
		object obj = TryGetPropertyValue(instance, "CombatState");
		if (HasEliteRoomType(obj))
		{
			return true;
		}
		object obj2 = TryGetPropertyValue(instance, "Encounter");
		if (obj2 == null && obj != null)
		{
			obj2 = TryGetPropertyValue(obj, "Encounter");
		}
		if (HasEliteRoomType(obj2))
		{
			return true;
		}
		return IsCurrentEliteRoom();
	}

	public static bool HasActiveModifier(string entry)
	{
		object runState = GetRunState();
		if (runState == null)
		{
			return false;
		}
		if (!(RunStateModifiersProperty?.GetValue(runState) is IEnumerable<ModifierModel> source))
		{
			return false;
		}
		ModifierModel[] array = source.ToArray();
		bool flag = CustomModifierCatalog.ContainsEntry(array, entry);
		if (flag)
		{
			string value = string.Join(", ", array.Select((ModifierModel modifier) => ((modifier == null) ? null : ((object)((AbstractModel)modifier).Id)?.ToString()) ?? "<null>"));
			ModEntry.Logger.Info($"STS2Plus.MoreRules active modifier found: {entry}; current modifiers=[{value}]", 1);
		}
		return flag;
	}

	public static bool HasDeprecatedModifier()
	{
		object runState = GetRunState();
		if (runState == null)
		{
			return false;
		}
		if (!(RunStateModifiersProperty?.GetValue(runState) is IEnumerable<ModifierModel> source))
		{
			return false;
		}
		return source.Any(delegate(ModifierModel modifier)
		{
			object a;
			if (modifier == null)
			{
				a = null;
			}
			else
			{
				ModelId id = ((AbstractModel)modifier).Id;
				a = ((id != null) ? id.Entry : null);
			}
			return string.Equals((string?)a, "DEPRECATED_MODIFIER", StringComparison.OrdinalIgnoreCase);
		});
	}

	public static object? GetCurrentRoom()
	{
		object runState = GetRunState();
		return (runState == null) ? null : RunStateCurrentRoomProperty?.GetValue(runState);
	}

	public static object? GetPlayerFromCombatState(object? combatState)
	{
		if (combatState == null)
		{
			return null;
		}
		return AccessTools.Property(combatState.GetType(), "Player")?.GetValue(combatState) ?? AccessTools.Field(combatState.GetType(), "_player")?.GetValue(combatState);
	}

	public static object? GetCurrentActMap()
	{
		object runState = GetRunState();
		return (runState == null) ? null : RunStateMapProperty?.GetValue(runState);
	}

	public static object? GetCurrentMapPoint()
	{
		object runState = GetRunState();
		return (runState == null) ? null : RunStateCurrentMapPointProperty?.GetValue(runState);
	}

	public static object? GetRouteStartPoint()
	{
		return GetCurrentMapPoint() ?? GetStartingMapPoint();
	}

	public static object? GetStartingMapPoint()
	{
		object currentActMap = GetCurrentActMap();
		if (currentActMap == null)
		{
			return null;
		}
		return AccessTools.Property(currentActMap.GetType(), "StartingMapPoint")?.GetValue(currentActMap);
	}

	public static IReadOnlyList<object> GetMapPointChildren(object? point)
	{
		if (point == null)
		{
			return Array.Empty<object>();
		}
		if (AccessTools.Property(point.GetType(), "Children")?.GetValue(point) is IEnumerable source)
		{
			return (from object child in source
				where child != null
				select child).ToArray();
		}
		return Array.Empty<object>();
	}

	public static MapPointType GetMapPointType(object? point)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		if (point == null)
		{
			return (MapPointType)1;
		}
		object obj = AccessTools.Property(point.GetType(), "PointType")?.GetValue(point);
		object obj2 = obj;
		MapPointType result;
		if (obj2 is MapPointType val)
		{
			MapPointType val2 = val;
			result = val2;
		}
		else
		{
			result = ((!Enum.TryParse<MapPointType>(obj?.ToString(), ignoreCase: true, out MapPointType result2)) ? ((MapPointType)1) : result2);
		}
		return result;
	}

	public static string DescribeMapPoint(object? point)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		MapPointType mapPointType = GetMapPointType(point);
		object mapPointCoord = GetMapPointCoord(point);
		object obj = AccessTools.Property(mapPointCoord?.GetType(), "Row")?.GetValue(mapPointCoord);
		object obj2 = AccessTools.Property(mapPointCoord?.GetType(), "Col")?.GetValue(mapPointCoord);
		if (obj != null && obj2 != null)
		{
			return $"{mapPointType} [{obj},{obj2}]";
		}
		return mapPointType.ToString();
	}

	public static object? GetMapPointCoord(object? point)
	{
		if (point == null)
		{
			return null;
		}
		return AccessTools.Property(point.GetType(), "Coord")?.GetValue(point) ?? AccessTools.Field(point.GetType(), "coord")?.GetValue(point);
	}

	public static MapPointType? GetCurrentMapPointType()
	{
		object runState = GetRunState();
		object obj = ((runState == null) ? null : RunStateCurrentMapPointProperty?.GetValue(runState));
		if (obj == null)
		{
			return null;
		}
		return AccessTools.Property(obj.GetType(), "PointType")?.GetValue(obj) as MapPointType?;
	}

	public static object? ResolveCanonicalModel(object? instance)
	{
		if (instance == null)
		{
			return null;
		}
		if (IsModelInstance(instance))
		{
			return instance;
		}
		return ResolveCanonicalModelShallow(instance, 0);
	}

	private static string? GetCurrentRoomType()
	{
		object runState = GetRunState();
		object obj = ((runState == null) ? null : RunStateCurrentRoomProperty?.GetValue(runState));
		if (obj == null)
		{
			return null;
		}
		return AccessTools.Property(obj.GetType(), "RoomType")?.GetValue(obj)?.ToString();
	}

	private static bool IsInitialGlassCannonRoom(object room)
	{
		object obj = TryGetPropertyValue(room, "CanonicalEvent");
		if (obj != null && string.Equals(obj.GetType().Name, "Neow", StringComparison.Ordinal))
		{
			return true;
		}
		object obj2 = TryGetPropertyValue(room, "ModelId");
		string a = ((obj2 == null) ? null : TryGetPropertyValue(obj2, "Entry")?.ToString());
		return string.Equals(a, "NEOW", StringComparison.OrdinalIgnoreCase);
	}

	private static bool HasEliteRoomType(object? instance)
	{
		if (instance == null)
		{
			return false;
		}
		string a = TryGetPropertyValue(instance, "RoomType")?.ToString();
		return string.Equals(a, "Elite", StringComparison.OrdinalIgnoreCase);
	}

	public static int ApplyGoldBonus(int amount, decimal multiplier)
	{
		return (amount <= 0) ? amount : Math.Max(1, (int)Math.Round((decimal)amount * multiplier, MidpointRounding.AwayFromZero));
	}

	public static int GetLoopCount()
	{
		object runState = GetRunState();
		if (runState == null)
		{
			return 0;
		}
		string seedString = GetSeedString(runState);
		if (seedString == null)
		{
			return 0;
		}
		int num = seedString.LastIndexOf("_L", StringComparison.Ordinal);
		if (num < 0)
		{
			return 0;
		}
		string text = seedString;
		int num2 = num + 2;
		int result;
		return int.TryParse(text.Substring(num2, text.Length - num2), out result) ? result : 0;
	}

	public static int GetTotalActNumber()
	{
		object runState = GetRunState();
		if (runState == null)
		{
			return 1;
		}
		int valueOrDefault = (RunStateCurrentActIndexProperty?.GetValue(runState) as int?).GetValueOrDefault();
		return GetLoopCount() * 3 + valueOrDefault + 1;
	}

	public static decimal GetEndlessHpMultiplier()
	{
		int totalActNumber = GetTotalActNumber();
		if (totalActNumber <= 3)
			return 1.0m;
		int num = totalActNumber - 3;
		return (decimal)Math.Pow(1.33, num);
	}

	public static decimal GetEndlessDamageMultiplier()
	{
		int totalActNumber = GetTotalActNumber();
		if (totalActNumber <= 3)
			return 1.0m;
		int num = totalActNumber - 3;
		return 1.0m + (decimal)num * 0.2m;
	}

	public static bool ShouldStartEndlessLoop(object? runManager)
	{
		object obj = ((runManager == null) ? GetRunState() : (RunManagerStateProperty?.GetValue(runManager) ?? RunManagerStateField?.GetValue(runManager)));
		if (obj == null)
		{
			return false;
		}
		int valueOrDefault = (RunStateCurrentActIndexProperty?.GetValue(obj) as int?).GetValueOrDefault();
		return valueOrDefault >= 2;
	}

	public static bool ShouldApplyGlassCannon()
	{
		object runState = GetRunState();
		if (runState == null)
		{
			return false;
		}
		int valueOrDefault = (RunStateCurrentActIndexProperty?.GetValue(runState) as int?).GetValueOrDefault();
		int valueOrDefault2 = (RunStateActFloorProperty?.GetValue(runState) as int?).GetValueOrDefault();
		if (valueOrDefault != 0 || valueOrDefault2 != 0 || GetLoopCount() != 0)
		{
			return false;
		}
		object obj = RunStateCurrentRoomProperty?.GetValue(runState);
		return obj == null || IsInitialGlassCannonRoom(obj);
	}

	public static Task? TriggerEndlessLoop(Node? gameOverScreen, string? nextSeed, int nextLoopIndex)
	{
		object? runManager = GetRunManager();
		object? runState = (runManager == null) ? null : (RunManagerStateProperty?.GetValue(runManager) ?? RunManagerStateField?.GetValue(runManager));
		if (runState == null)
		{
			ModEntry.Logger.Warn("STS2Plus TriggerEndlessLoop: runState is null, aborting.");
			return null;
		}
		Node? screen = gameOverScreen;
		if (screen == null && NGameInstanceProperty?.GetValue(null) is Node ngameNode)
		{
			screen = new Control
			{
				Name = (StringName)"STS2PlusEndlessLoopProxy"
			};
			ngameNode.AddChild(screen);
		}
		if (nextSeed == null)
		{
			ModEntry.Logger.Warn("STS2Plus TriggerEndlessLoop: nextSeed is null, aborting.");
			return null;
		}
		ModEntry.Logger.Info($"STS2Plus endless loop: starting internal save/load reconstruction loopIndex={nextLoopIndex} seed={nextSeed}.", 1);
		return EndlessLoopTransition.StartAsync(screen, nextLoopIndex, nextSeed);
	}

	public static Task? TriggerLegacySingleplayerEndlessLoop(object? runManager = null, string pathLabel = "NATURAL_SINGLEPLAYER")
	{
		object? runManager2 = runManager ?? GetRunManager();
		object? runState = (runManager2 == null) ? null : (RunManagerStateProperty?.GetValue(runManager2) ?? RunManagerStateField?.GetValue(runManager2));
		if (runManager2 == null || runState == null)
		{
			ModEntry.Logger.Warn("STS2Plus legacy singleplayer endless loop: run manager or run state is null, aborting.");
			return null;
		}
		string baseSeedString = GetBaseSeedString(runState);
		int num = GetLoopCount() + 1;
		string nextSeed = baseSeedString + "_L" + num;
		ModEntry.Logger.Info("STS2Plus LOOP PATH: " + pathLabel, 1);
		ModEntry.Logger.Info($"STS2Plus legacy singleplayer endless loop: act/floor/map reset via GenerateRooms + EnterAct loopIndex={num} seed={nextSeed}.", 1);
		if (RunRngSetType != null)
		{
			try
			{
				PropertyInfo? runStateRngProperty = RunStateRngProperty;
				if (runStateRngProperty != null && runStateRngProperty.CanWrite)
				{
					object obj = Activator.CreateInstance(RunRngSetType, new object[1] { nextSeed });
					if (obj != null)
					{
						RunStateRngProperty.SetValue(runState, obj);
					}
				}
			}
			catch (Exception ex)
			{
				ModEntry.Logger.Warn("STS2Plus legacy singleplayer endless loop: failed to set next RNG seed - " + ex.Message, 1);
			}
		}
		RunStateCurrentActIndexProperty?.SetValue(runState, 0);
		RunStateActFloorProperty?.SetValue(runState, 0);
		TryFinishActiveEventForLegacyLoop(runState, num);
		AccessTools.Method(runManager2.GetType(), "GenerateRooms", Type.EmptyTypes, null)?.Invoke(runManager2, Array.Empty<object>());
		object? obj2 = AccessTools.Method(runManager2.GetType(), "EnterAct", new Type[2]
		{
			typeof(int),
			typeof(bool)
		}, null)?.Invoke(runManager2, new object[2] { 0, false });
		ModEntry.Logger.Info($"STS2Plus legacy singleplayer endless loop: currentRoom={GetCurrentRoom()?.GetType().Name ?? "<null>"} mapPoint={DescribeMapPoint(GetCurrentMapPoint())} loopIndex={num}.", 1);
		return obj2 as Task;
	}

	public static bool IsAttackCard(object? card)
	{
		if (card == null)
		{
			return false;
		}
		return string.Equals(CardTypeProperty?.GetValue(card)?.ToString(), "Attack", StringComparison.OrdinalIgnoreCase);
	}

	public static bool IsDefenseCard(object? card)
	{
		if (card == null)
		{
			return false;
		}
		object obj = CardGainsBlockProperty?.GetValue(card);
		bool flag = default(bool);
		int num;
		if (obj is bool)
		{
			flag = (bool)obj;
			num = 1;
		}
		else
		{
			num = 0;
		}
		if (((uint)num & (flag ? 1u : 0u)) != 0)
		{
			return true;
		}
		if (CardTagsProperty?.GetValue(card) is IEnumerable enumerable)
		{
			foreach (object item in enumerable)
			{
				if (string.Equals(item?.ToString(), "Defend", StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
			}
		}
		return false;
	}

	public static bool ApplyAttackDefenseBaseBonus(object? card, decimal attackBonus, decimal defenseBonus)
	{
		if (card == null)
		{
			return false;
		}
		object obj = CardDynamicVarsProperty?.GetValue(card);
		if (obj == null)
		{
			return false;
		}
		if (!(AccessTools.Property(obj.GetType(), "Values")?.GetValue(obj) is IEnumerable enumerable))
		{
			return false;
		}
		bool flag = false;
		foreach (object item in enumerable)
		{
			object obj2 = UnwrapDynamicVar(item);
			if (obj2 != null)
			{
				string dynamicVarName = GetDynamicVarName(obj2);
				if (dynamicVarName == "Damage" && IsAttackCard(card))
				{
					flag |= TryAddDynamicVarBaseValue(obj2, attackBonus);
				}
				else if (dynamicVarName == "Block" && IsDefenseCard(card))
				{
					flag |= TryAddDynamicVarBaseValue(obj2, defenseBonus);
				}
			}
		}
		return flag;
	}

	public static bool ApplyGlassCannon(object? player)
	{
		if (player == null)
		{
			return false;
		}
		int num = ResolveGlassCannonTargetMaxHp(player);
		object obj = ResolveOwningPlayer(player);
		bool flag = ApplyDirectHealthState(player, num, 1);
		flag |= RepairAliveState(player);
		object player2 = obj ?? player;
		flag |= EnsurePlayerBaseEnergy(player2, 4, fillCurrent: false);
		flag |= ApplyGlassCannonToPlayerCreatures(player2, num);
		if (obj != null && obj != player)
		{
			flag |= ApplyDirectHealthState(obj, num, 1);
			flag |= RepairAliveState(obj);
		}
		IReadOnlyList<object> readOnlyList = ResolveHealthHolders(player);
		if (readOnlyList.Count == 0)
		{
			if (!flag)
			{
				ModEntry.Logger.Warn("STS2Plus could not resolve a health holder for " + player.GetType().FullName + ".", 1);
			}
			return flag;
		}
		foreach (object item in readOnlyList)
		{
			int currentHp = GetCurrentHp(item);
			int currentHp2 = ((currentHp <= 0) ? 1 : Math.Min(currentHp, num));
			flag |= ApplyHealthState(item, num, currentHp2);
			flag |= RepairAliveState(item);
			ActivatePlayerHooksIfAlive(obj, item);
		}
		return flag;
	}

	public static bool RepairGlassCannonState(object? player)
	{
		if (player == null)
		{
			return false;
		}
		IReadOnlyList<object> readOnlyList = ResolveHealthHolders(player);
		int num = ResolveGlassCannonTargetMaxHp(player);
		object obj = ResolveOwningPlayer(player);
		bool flag = ApplyDirectHealthState(player, num, 1);
		flag |= RepairAliveState(player);
		object player2 = obj ?? player;
		flag |= EnsurePlayerBaseEnergy(player2, 4, fillCurrent: false);
		flag |= ApplyGlassCannonToPlayerCreatures(player2, num);
		if (obj != null && obj != player)
		{
			flag |= ApplyDirectHealthState(obj, num, 1);
			flag |= RepairAliveState(obj);
		}
		if (readOnlyList.Count == 0)
		{
			return flag;
		}
		foreach (object item in readOnlyList)
		{
			int currentHp = GetCurrentHp(item);
			int currentHp2 = ((currentHp <= 0) ? 1 : Math.Min(currentHp, num));
			flag |= ApplyHealthState(item, num, currentHp2);
			flag |= RepairAliveState(item);
			ActivatePlayerHooksIfAlive(obj, item);
		}
		return flag;
	}

	public static bool RepairGlassCannonPlayerCreature(object? player)
	{
		if (player == null)
		{
			return false;
		}
		int targetMaxHp = ResolveGlassCannonTargetMaxHp(player);
		return ApplyGlassCannonToPlayerCreatures(player, targetMaxHp);
	}

	public static bool NormalizeSerializedGlassCannonPlayer(object? serializedPlayer, object? runtimePlayer = null)
	{
		if (serializedPlayer == null)
		{
			return false;
		}
		int @int = GetInt(serializedPlayer, AccessTools.Property(serializedPlayer.GetType(), "MaxHp"), "MaxHp", "_maxHp", "maxHp", "<MaxHp>k__BackingField", "MaxHealth", "_maxHealth", "maxHealth");
		int num = ((@int > 0) ? @int : GetObservedGlassCannonMaxHp(runtimePlayer));
		int value;
		int num2 = ((runtimePlayer != null && PlusState.TryGetGlassCannonExpectedMaxHp(runtimePlayer, out value)) ? value : PlusState.RememberGlassCannonExpectedMaxHp(runtimePlayer ?? serializedPlayer, ShouldApplyGlassCannon() ? 1 : num));
		int int2 = GetInt(serializedPlayer, AccessTools.Property(serializedPlayer.GetType(), "MaxEnergy"), "MaxEnergy", "_maxEnergy", "maxEnergy", "<MaxEnergy>k__BackingField");
		int value2 = Math.Max(4, (runtimePlayer == null) ? Math.Max(1, int2) : GetPlayerBaseMaxEnergy(runtimePlayer, Math.Max(1, int2)));
		int int3 = GetInt(serializedPlayer, AccessTools.Property(serializedPlayer.GetType(), "CurrentHp"), "CurrentHp", "_currentHp", "currentHp", "<CurrentHp>k__BackingField", "CurrentHealth", "_currentHealth", "currentHealth");
		int value3 = ((int3 <= 0) ? 1 : Math.Min(int3, num2));
		bool flag = false;
		flag |= TryWriteInt(AccessTools.Property(serializedPlayer.GetType(), "MaxHp"), serializedPlayer, num2);
		flag |= TryWriteInt(AccessTools.Property(serializedPlayer.GetType(), "MaxEnergy"), serializedPlayer, value2);
		return flag | TryWriteInt(AccessTools.Property(serializedPlayer.GetType(), "CurrentHp"), serializedPlayer, value3);
	}

	public static object? GetPlayerCreature(object? player)
	{
		if (player == null)
		{
			return null;
		}
		return TryGetPropertyValue(player, "Creature");
	}

	public static string DescribeGlassCannonState(object? player)
	{
		if (player == null)
		{
			return "<null>";
		}
		object playerCreature = GetPlayerCreature(player);
		if (playerCreature == null)
		{
			return "creature=<null>";
		}
		return $"creatureHp={GetCurrentHp(playerCreature)}/{GetMaxHp(playerCreature)} isDead={GetBool(AccessTools.Property(playerCreature.GetType(), "IsDead"), playerCreature)}";
	}

	public static bool ShouldTreatCreatureAsDeadForEvent(object? creature)
	{
		if (PlusState.ShouldForceGlassCannonRepair())
		{
			if (creature != null)
			{
				int maxHp = Math.Max(1, GetMaxHp(creature));
				int currentHp = GetCurrentHp(creature);
				if (currentHp <= 0)
				{
					SetHealthState(creature, maxHp, 1);
				}
			}
			return false;
		}
		if (creature == null)
		{
			return true;
		}
		return GetBool(AccessTools.Property(creature.GetType(), "IsDead"), creature);
	}

	public static void TrackGlassCannonEventStart(object? eventModel, object? player, bool isPreFinished)
	{
		if (eventModel != null && player != null && PlusState.ShouldForceGlassCannonRepair())
		{
			PendingGlassCannonEventStarts[eventModel] = isPreFinished;
		}
	}

	public static void ClearTrackedGlassCannonEvent(object? eventModel)
	{
		if (eventModel != null)
		{
			PendingGlassCannonEventStarts.Remove(eventModel);
		}
	}

	public static bool TryRecoverGlassCannonEventFromDeath(object? eventModel, object? description)
	{
		if (eventModel == null || !PlusState.ShouldForceGlassCannonRepair() || !PendingGlassCannonEventStarts.TryGetValue(eventModel, out var value) || !IsGenericEventDeathDescription(description))
		{
			return false;
		}
		object obj = TryGetPropertyValue(eventModel, "Owner");
		if (obj == null)
		{
			return false;
		}
		ApplyGlassCannon(obj);
		RepairGlassCannonPlayerCreature(obj);
		RepairGlassCannonState(obj);
		MethodInfo methodInfo = AccessTools.Method(eventModel.GetType(), "SetInitialEventState", (Type[])null, (Type[])null);
		if (methodInfo == null)
		{
			return false;
		}
		try
		{
			methodInfo.Invoke(eventModel, new object[1] { value });
			PendingGlassCannonEventStarts.Remove(eventModel);
			ModEntry.Logger.Warn("STS2Plus recovered Glass Cannon event start from generic death. " + DescribeGlassCannonState(obj), 1);
			return true;
		}
		catch (Exception ex)
		{
			ModEntry.Logger.Warn("STS2Plus failed to recover Glass Cannon event start: " + ex.Message, 1);
			return false;
		}
	}

	public static bool IncreaseMaxHp(object? player, int amount, bool healToMax = false, bool healByAmount = false)
	{
		if (player == null || amount <= 0)
		{
			return false;
		}
		IReadOnlyList<object> readOnlyList = ResolveHealthHolders(player);
		if (readOnlyList.Count == 0)
		{
			return false;
		}
		int num = 0;
		if (PlusState.IsGlassCannonActive())
		{
			int num2 = ResolveGlassCannonTargetMaxHp(player);
			num = PlusState.RememberGlassCannonExpectedMaxHp(player, num2 + amount);
		}
		bool flag = false;
		foreach (object item in readOnlyList)
		{
			int maxHp = GetMaxHp(item);
			if (maxHp > 0)
			{
				int num3 = ((num > 0) ? num : (maxHp + amount));
				int currentHp = GetCurrentHp(item);
				int num4 = Math.Max(0, num3 - maxHp);
				int val = (healByAmount ? (currentHp + num4) : currentHp);
				int currentHp2 = (healToMax ? num3 : Math.Max(1, Math.Min(val, num3)));
				flag |= ApplyHealthState(item, num3, currentHp2);
			}
		}
		if (!flag)
		{
			return false;
		}
		if (healToMax || healByAmount)
		{
			object player2 = ResolveOwningPlayer(player);
			foreach (object item2 in readOnlyList)
			{
				ActivatePlayerHooksIfAlive(player2, item2);
			}
		}
		return true;
	}

	public static bool SetPlayerEnergy(object? player, int maxEnergy, bool fillCurrent)
	{
		if (player == null || maxEnergy <= 0)
		{
			return false;
		}
		bool flag = TryWriteInt(PlayerMaxEnergyProperty, player, maxEnergy);
		object obj = PlayerCombatStateProperty?.GetValue(player);
		if (obj != null)
		{
			flag |= TryWriteInt(PlayerCombatStateMaxEnergyProperty, obj, maxEnergy);
			if (fillCurrent)
			{
				flag |= TryWriteInt(PlayerCombatStateEnergyProperty, obj, maxEnergy);
			}
		}
		return flag;
	}

	public static int GetPlayerBaseMaxEnergy(object? player, int fallbackMaxEnergy = 1)
	{
		object obj = ((player == null) ? null : (ResolveOwningPlayer(player) ?? player));
		if (obj == null)
		{
			return Math.Max(1, fallbackMaxEnergy);
		}
		int @int = GetInt(obj, PlayerMaxEnergyProperty, "MaxEnergy", "_maxEnergy", "maxEnergy", "<MaxEnergy>k__BackingField");
		return Math.Max(1, (@int > 0) ? @int : fallbackMaxEnergy);
	}

	public static bool EnsurePlayerBaseEnergy(object? player, int minimumBaseEnergy, bool fillCurrent)
	{
		if (player == null || minimumBaseEnergy <= 0)
		{
			return false;
		}
		object obj = ResolveOwningPlayer(player) ?? player;
		if (obj == null)
		{
			return false;
		}
		int num = Math.Max(minimumBaseEnergy, GetPlayerBaseMaxEnergy(obj, minimumBaseEnergy));
		bool flag = TryWriteInt(PlayerMaxEnergyProperty, obj, num);
		object obj2 = PlayerCombatStateProperty?.GetValue(obj);
		if (obj2 != null && fillCurrent)
		{
			int value;
			int value2 = (TryReadInt(PlayerCombatStateMaxEnergyProperty, obj2, out value) ? Math.Max(1, value) : num);
			flag |= TryWriteInt(PlayerCombatStateEnergyProperty, obj2, value2);
		}
		return flag;
	}

	public static bool SetCombatStateEnergy(object? combatState, int maxEnergy, bool fillCurrent)
	{
		if (combatState == null || maxEnergy <= 0)
		{
			return false;
		}
		bool flag = TryWriteInt(PlayerCombatStateMaxEnergyProperty, combatState, maxEnergy);
		if (fillCurrent)
		{
			flag |= TryWriteInt(PlayerCombatStateEnergyProperty, combatState, maxEnergy);
		}
		return flag;
	}

	public static decimal SumDamageFromResults(object? results)
	{
		if (!(results is IEnumerable enumerable))
		{
			return 0m;
		}
		decimal result = default(decimal);
		foreach (object item in enumerable)
		{
			if (item != null)
			{
				result += ReadDamageResultTotal(item);
			}
		}
		return result;
	}

	public static int GetIntentTotalDamage(object? intent, IEnumerable<object> targets, object? owner)
	{
		if (intent == null || owner == null)
		{
			return 0;
		}
		MethodInfo methodInfo = AccessTools.Method(intent.GetType(), "GetTotalDamage", (Type[])null, (Type[])null);
		if (methodInfo == null)
		{
			return 0;
		}
		try
		{
			Array array = CreateCreatureArray(targets);
			return (methodInfo.Invoke(intent, new object[2] { array, owner }) as int?).GetValueOrDefault();
		}
		catch (Exception ex)
		{
			ModEntry.Logger.Warn("STS2Plus incoming-damage total-damage reflection failed: " + ex.Message, 1);
			return 0;
		}
	}

	public static bool ShouldGamePreventBlockClear(object? creature)
	{
		if (creature == null)
		{
			return false;
		}
		object obj = CreatureCombatStateProperty?.GetValue(creature) ?? TryGetPropertyValue(creature, "CombatState");
		if (obj == null || HookShouldClearBlockMethod == null)
		{
			return false;
		}
		try
		{
			object[] parameters = new object[3] { obj, creature, null };
			bool valueOrDefault = (HookShouldClearBlockMethod.Invoke(null, parameters) as bool?).GetValueOrDefault(true);
			return !valueOrDefault;
		}
		catch
		{
			return false;
		}
	}

	public static object? GetRunManager() => RunManagerInstanceProperty?.GetValue(null);

	private static object? GetRunState()
	{
		object obj = RunManagerInstanceProperty?.GetValue(null);
		return (obj == null) ? null : (RunManagerStateProperty?.GetValue(obj) ?? RunManagerStateField?.GetValue(obj));
	}

	private static bool TryAdd(PropertyInfo? property, object instance, int amount)
	{
		if (property == null || !property.CanRead || !property.CanWrite)
		{
			return false;
		}
		if (!(property.GetValue(instance) is int num) || num <= 0)
		{
			return false;
		}
		property.SetValue(instance, num + amount);
		return true;
	}

	private static decimal ReadDamageValue(object instance, string propertyName)
	{
		object obj = AccessTools.Property(instance.GetType(), propertyName)?.GetValue(instance);
		object obj2 = obj;
		decimal result;
		if (!(obj2 is int num))
		{
			if (!(obj2 is long num2))
			{
				if (!(obj2 is float num3))
				{
					if (!(obj2 is double num4))
					{
						if (obj2 is decimal num5)
						{
							decimal num6 = num5;
							result = num6;
						}
						else
						{
							result = ((!decimal.TryParse(obj?.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out var result2)) ? 0m : result2);
						}
					}
					else
					{
						double num7 = num4;
						result = (decimal)num7;
					}
				}
				else
				{
					float num8 = num3;
					result = (decimal)num8;
				}
			}
			else
			{
				long num9 = num2;
				result = num9;
			}
		}
		else
		{
			int num10 = num;
			result = num10;
		}
		return result;
	}

	private static decimal ReadDamageResultTotal(object instance)
	{
		decimal num = ReadDamageValue(instance, "TotalDamage");
		decimal num2 = ReadDamageValue(instance, "OverkillDamage");
		if (num > 0m || num2 > 0m)
		{
			return num + num2;
		}
		return ReadDamageValue(instance, "UnblockedDamage") + ReadDamageValue(instance, "BlockedDamage");
	}

	private static bool GetBool(PropertyInfo? property, object instance)
	{
		if (property != null)
		{
			try
			{
				if (property.GetValue(instance) is bool result)
				{
					return result;
				}
			}
			catch
			{
			}
		}
		PropertyInfo propertyInfo = AccessTools.Property(instance.GetType(), property?.Name ?? string.Empty);
		if (propertyInfo != null)
		{
			try
			{
				if (propertyInfo.GetValue(instance) is bool result2)
				{
					return result2;
				}
			}
			catch
			{
			}
		}
		FieldInfo fieldInfo = AccessTools.Field(instance.GetType(), property?.Name ?? string.Empty);
		if (fieldInfo != null)
		{
			try
			{
				if (fieldInfo.GetValue(instance) is bool result3)
				{
					return result3;
				}
			}
			catch
			{
			}
		}
		return false;
	}

	private static bool TryReadBool(object instance, string memberName, out bool value)
	{
		value = false;
		PropertyInfo propertyInfo = AccessTools.Property(instance.GetType(), memberName);
		if (propertyInfo != null)
		{
			try
			{
				if (propertyInfo.GetValue(instance) is bool flag)
				{
					value = flag;
					return true;
				}
			}
			catch
			{
			}
		}
		FieldInfo fieldInfo = AccessTools.Field(instance.GetType(), memberName);
		if (fieldInfo != null)
		{
			try
			{
				if (fieldInfo.GetValue(instance) is bool flag2)
				{
					value = flag2;
					return true;
				}
			}
			catch
			{
			}
		}
		return false;
	}

	private static bool TrySetBool(object instance, string memberName, bool value)
	{
		PropertyInfo propertyInfo = AccessTools.Property(instance.GetType(), memberName);
		if ((object)propertyInfo != null && propertyInfo.CanWrite)
		{
			try
			{
				if (propertyInfo.GetValue(instance) is bool flag && flag == value)
				{
					return false;
				}
				propertyInfo.SetValue(instance, value);
				return true;
			}
			catch
			{
			}
		}
		FieldInfo fieldInfo = AccessTools.Field(instance.GetType(), memberName);
		if (fieldInfo?.FieldType == typeof(bool))
		{
			try
			{
				if (fieldInfo.GetValue(instance) is bool flag2 && flag2 == value)
				{
					return false;
				}
				fieldInfo.SetValue(instance, value);
				return true;
			}
			catch
			{
			}
		}
		return false;
	}

	private static void TryFinishActiveEventForLegacyLoop(object runState, int loopIndex)
	{
		try
		{
			object obj = RunStateCurrentRoomProperty?.GetValue(runState);
			object obj2 = (obj == null) ? null : (TryGetPropertyValue(obj, "CanonicalEvent") ?? TryGetPropertyValue(obj, "Event") ?? TryGetPropertyValue(obj, "Model"));
			string text = DescribeEventIdentity(obj2);
			if (obj2 == null)
			{
				ModEntry.Logger.Info($"STS2Plus legacy singleplayer endless loop: no active event to clear before continuation loopIndex={loopIndex}.", 1);
				return;
			}
			bool flag = TryInvokeNoArg(obj2, "SetEventFinished") || TryInvokeNoArg(obj2, "Done");
			ModEntry.Logger.Info($"STS2Plus legacy singleplayer endless loop: active event clear loopIndex={loopIndex} event={text} cleared={flag}.", 1);
		}
		catch (Exception ex)
		{
			ModEntry.Logger.Warn($"STS2Plus legacy singleplayer endless loop: failed to clear active event loopIndex={loopIndex}: {ex.GetType().Name}: {ex.Message}", 1);
		}
	}

	private static bool TryInvokeNoArg(object target, string methodName)
	{
		MethodInfo methodInfo = AccessTools.Method(target.GetType(), methodName, Type.EmptyTypes, null);
		if (methodInfo == null)
		{
			return false;
		}
		try
		{
			methodInfo.Invoke(target, Array.Empty<object>());
			return true;
		}
		catch
		{
			return false;
		}
	}

	private static string DescribeEventIdentity(object? eventModel)
	{
		if (eventModel == null)
		{
			return "<null>";
		}
		object obj = TryGetPropertyValue(eventModel, "Id") ?? TryGetPropertyValue(eventModel, "ModelId");
		string text = TryGetPropertyValue(obj, "Entry")?.ToString() ?? obj?.ToString() ?? eventModel.GetType().Name;
		return text;
	}

	private static bool IsGenericEventDeathDescription(object? description)
	{
		if (description == null)
		{
			return false;
		}
		string a = AccessTools.Property(description.GetType(), "LocTable")?.GetValue(description) as string;
		string a2 = AccessTools.Property(description.GetType(), "LocEntryKey")?.GetValue(description) as string;
		return string.Equals(a, "events", StringComparison.Ordinal) && string.Equals(a2, "GENERIC.youAreDead.description", StringComparison.Ordinal);
	}

	private static bool RepairAliveState(object? instance)
	{
		if (instance == null)
		{
			return false;
		}
		bool flag = false;
		string[] deathStateFalseMembers = DeathStateFalseMembers;
		foreach (string memberName in deathStateFalseMembers)
		{
			flag |= TrySetBool(instance, memberName, value: false);
		}
		string[] aliveStateTrueMembers = AliveStateTrueMembers;
		foreach (string memberName2 in aliveStateTrueMembers)
		{
			flag |= TrySetBool(instance, memberName2, value: true);
		}
		if (IsMarkedDead(instance))
		{
			flag |= TryInvokeParameterless(instance, "ReviveBeforeCombatEnd");
			flag |= TryInvokeParameterless(instance, "Revive");
		}
		return flag;
	}

	private static bool IsMarkedDead(object instance)
	{
		string[] deathStateFalseMembers = DeathStateFalseMembers;
		foreach (string memberName in deathStateFalseMembers)
		{
			if (TryReadBool(instance, memberName, out var value) && value)
			{
				return true;
			}
		}
		return false;
	}

	private static bool TryInvokeParameterless(object instance, string methodName)
	{
		MethodInfo methodInfo = AccessTools.Method(instance.GetType(), methodName, (Type[])null, (Type[])null);
		if (methodInfo == null || methodInfo.GetParameters().Length != 0)
		{
			return false;
		}
		try
		{
			methodInfo.Invoke(instance, Array.Empty<object>());
			return true;
		}
		catch
		{
			return false;
		}
	}

	private static int GetInt(object instance, PropertyInfo? preferredProperty, params string[] candidateNames)
	{
		if (TryReadInt(preferredProperty, instance, out var value))
		{
			return value;
		}
		foreach (string text in candidateNames)
		{
			PropertyInfo property = AccessTools.Property(instance.GetType(), text);
			if (TryReadInt(property, instance, out var value2))
			{
				return value2;
			}
			if (AccessTools.Field(instance.GetType(), text)?.GetValue(instance) is int result)
			{
				return result;
			}
		}
		return 0;
	}

	private static void SetInt(object instance, int value, PropertyInfo? preferredProperty, params string[] candidateNames)
	{
		if (TryWriteInt(preferredProperty, instance, value))
		{
			return;
		}
		foreach (string text in candidateNames)
		{
			PropertyInfo property = AccessTools.Property(instance.GetType(), text);
			if (TryWriteInt(property, instance, value))
			{
				break;
			}
			FieldInfo fieldInfo = AccessTools.Field(instance.GetType(), text);
			if (fieldInfo != null && fieldInfo.FieldType == typeof(int))
			{
				fieldInfo.SetValue(instance, value);
				break;
			}
		}
	}

	private static bool TryReadInt(PropertyInfo? property, object instance, out int value)
	{
		value = 0;
		if (property == null)
		{
			return false;
		}
		try
		{
			if (property.GetValue(instance) is int num)
			{
				value = num;
				return true;
			}
		}
		catch
		{
		}
		return false;
	}

	private static bool TryWriteInt(PropertyInfo? property, object instance, int value)
	{
		if (property == null || !property.CanWrite)
		{
			return false;
		}
		try
		{
			property.SetValue(instance, value);
			return true;
		}
		catch
		{
			return false;
		}
	}

	private static bool ApplyDirectHealthState(object? instance, int maxHp, int preferredCurrentHp)
	{
		if (instance == null)
		{
			return false;
		}
		int currentHp = GetCurrentHp(instance);
		int currentHp2 = ((currentHp <= 0) ? preferredCurrentHp : Math.Min(currentHp, maxHp));
		int maxHp2 = GetMaxHp(instance);
		int num = currentHp;
		SetHealthState(instance, maxHp, currentHp2);
		return maxHp2 != GetMaxHp(instance) || num != GetCurrentHp(instance);
	}

	private static int ResolveGlassCannonTargetMaxHp(object? player, int fallbackMaxHp = 1)
	{
		if (PlusState.TryGetGlassCannonExpectedMaxHp(player, out var value))
		{
			return value;
		}
		int observedGlassCannonMaxHp = GetObservedGlassCannonMaxHp(player, fallbackMaxHp);
		int value2 = (ShouldApplyGlassCannon() ? 1 : observedGlassCannonMaxHp);
		return PlusState.RememberGlassCannonExpectedMaxHp(player, value2);
	}

	private static int GetObservedGlassCannonMaxHp(object? instance, int fallbackMaxHp = 1)
	{
		if (instance == null)
		{
			return Math.Max(1, fallbackMaxHp);
		}
		HashSet<object> hashSet = new HashSet<object>(ReferenceEqualityComparer.Instance);
		foreach (object item in EnumerateGlassCannonMaxHpCandidates(instance))
		{
			if (item != null && hashSet.Add(item))
			{
				int maxHp = GetMaxHp(item);
				if (maxHp > 0)
				{
					return maxHp;
				}
			}
		}
		return Math.Max(1, fallbackMaxHp);
	}

	private static bool ApplyGlassCannonToPlayerCreatures(object player, int targetMaxHp)
	{
		bool flag = false;
		string[] array = new string[2] { "Creature", "Osty" };
		foreach (string propertyName in array)
		{
			object obj = TryGetPropertyValue(player, propertyName);
			if (obj != null)
			{
				int currentHp = GetCurrentHp(obj);
				int currentHp2 = ((currentHp <= 0) ? 1 : Math.Min(currentHp, targetMaxHp));
				flag |= ApplyHealthState(obj, targetMaxHp, currentHp2);
				flag |= RepairAliveState(obj);
				ActivatePlayerHooksIfAlive(player, obj);
			}
		}
		return flag;
	}

	[IteratorStateMachine(typeof(_003CEnumerateGlassCannonMaxHpCandidates_003Ed__146))]
	private static IEnumerable<object?> EnumerateGlassCannonMaxHpCandidates(object instance)
	{
		//yield-return decompiler failed: Unexpected instruction in Iterator.Dispose()
		return new _003CEnumerateGlassCannonMaxHpCandidates_003Ed__146(-2)
		{
			_003C_003E3__instance = instance
		};
	}

	private static IReadOnlyList<object> ResolveHealthHolders(object instance)
	{
		List<object> list = new List<object>();
		HashSet<object> visited = new HashSet<object>(ReferenceEqualityComparer.Instance);
		Queue<(object, int)> queue = new Queue<(object, int)>();
		EnqueueHealthCandidate(instance, 0, list, visited, queue);
		while (queue.Count > 0)
		{
			var (instance2, num) = queue.Dequeue();
			if (num >= 2)
			{
				continue;
			}
			foreach (object item in EnumerateHealthCandidates(instance2))
			{
				EnqueueHealthCandidate(item, num + 1, list, visited, queue);
			}
		}
		return list;
	}

	private static object? ResolveCanonicalModelShallow(object? instance, int depth)
	{
		if (instance == null || depth > 2)
		{
			return null;
		}
		if (IsModelInstance(instance))
		{
			return instance;
		}
		string[] array = new string[7] { "CanonicalModel", "CanonicalEvent", "Model", "Encounter", "Event", "CanonicalInstance", "Entity" };
		foreach (string propertyName in array)
		{
			object instance2 = TryGetPropertyValue(instance, propertyName);
			object obj = ResolveCanonicalModelShallow(instance2, depth + 1);
			if (obj != null)
			{
				return obj;
			}
		}
		if (depth == 0)
		{
			string[] array2 = new string[3] { "HoverTips", "Items", "Entries" };
			foreach (string propertyName2 in array2)
			{
				if (!(TryGetPropertyValue(instance, propertyName2) is IEnumerable enumerable))
				{
					continue;
				}
				foreach (object item in enumerable)
				{
					object obj2 = ResolveCanonicalModelShallow(item, depth + 1);
					if (obj2 != null)
					{
						return obj2;
					}
				}
			}
		}
		return null;
	}

	private static bool IsModelInstance(object instance)
	{
		string text = instance.GetType().FullName ?? string.Empty;
		return text.StartsWith("MegaCrit.Sts2.Core.Models.", StringComparison.Ordinal);
	}

	private static object? TryGetPropertyValue(object instance, string propertyName)
	{
		try
		{
			return AccessTools.Property(instance.GetType(), propertyName)?.GetValue(instance);
		}
		catch
		{
			return null;
		}
	}

	private static bool HasHealthAccess(object instance)
	{
		int @int = GetInt(instance, AccessTools.Property(instance.GetType(), "MaxHp"), "MaxHp", "_maxHp", "maxHp", "<MaxHp>k__BackingField", "MaxHealth", "_maxHealth", "maxHealth");
		return @int > 0;
	}

	private static bool ApplyHealthState(object creature, int maxHp, int currentHp)
	{
		int num = Math.Max(1, maxHp);
		int num2 = Math.Clamp(currentHp, 1, num);
		if (GetMaxHp(creature) == num && GetCurrentHp(creature) == num2)
		{
			return false;
		}
		SetHealthState(creature, num, num2);
		return true;
	}

	private static void SetHealthState(object creature, int maxHp, int currentHp)
	{
		int num = Math.Max(1, maxHp);
		int value = Math.Clamp(currentHp, 1, num);
		if (!TryInvokeHealthMethod(CreatureSetMaxHpInternalMethod, creature, num))
		{
			SetMaxHp(creature, num);
		}
		if (!TryInvokeHealthMethod(CreatureSetCurrentHpInternalMethod, creature, value))
		{
			SetCurrentHp(creature, value);
		}
	}

	private static bool TryInvokeHealthMethod(MethodInfo? method, object creature, int value)
	{
		if (method == null)
		{
			return false;
		}
		try
		{
			method.Invoke(creature, new object[1] { (decimal)value });
			return true;
		}
		catch
		{
			return false;
		}
	}

	private static void ActivatePlayerHooksIfAlive(object? player, object healthHolder)
	{
		if (player == null || GetCurrentHp(healthHolder) <= 0 || PlayerActivateHooksMethod == null)
		{
			return;
		}
		try
		{
			PlayerActivateHooksMethod.Invoke(player, Array.Empty<object>());
		}
		catch
		{
		}
	}

	private static object? ResolveOwningPlayer(object instance)
	{
		PropertyInfo? playerMaxEnergyProperty = PlayerMaxEnergyProperty;
		if ((object)playerMaxEnergyProperty != null && (playerMaxEnergyProperty.DeclaringType?.IsInstanceOfType(instance)).GetValueOrDefault())
		{
			return instance;
		}
		return GetCachedPropertyValue(CreaturePlayerProperty, instance) ?? GetCachedPropertyValue(CreaturePetOwnerProperty, instance) ?? TryGetPropertyValue(instance, "Player");
	}

	private static object? GetCachedPropertyValue(PropertyInfo? property, object? instance)
	{
		if (property == null || instance == null)
		{
			return null;
		}
		Type declaringType = property.DeclaringType;
		if (declaringType != null && !declaringType.IsInstanceOfType(instance))
		{
			return null;
		}
		try
		{
			return property.GetValue(instance);
		}
		catch
		{
			return null;
		}
	}

	[IteratorStateMachine(typeof(_003CEnumerateHealthCandidates_003Ed__158))]
	private static IEnumerable<object> EnumerateHealthCandidates(object instance)
	{
		//yield-return decompiler failed: Unexpected instruction in Iterator.Dispose()
		return new _003CEnumerateHealthCandidates_003Ed__158(-2)
		{
			_003C_003E3__instance = instance
		};
	}

	private static void EnqueueHealthCandidate(object? candidate, int depth, List<object> holders, HashSet<object> visited, Queue<(object Value, int Depth)> queue)
	{
		if (candidate != null && visited.Add(candidate))
		{
			if (HasHealthAccess(candidate))
			{
				holders.Add(candidate);
			}
			queue.Enqueue((candidate, depth));
		}
	}

	[IteratorStateMachine(typeof(_003CEnumerateNestedObjects_003Ed__160))]
	private static IEnumerable<object?> EnumerateNestedObjects(object instance)
	{
		//yield-return decompiler failed: Unexpected instruction in Iterator.Dispose()
		return new _003CEnumerateNestedObjects_003Ed__160(-2)
		{
			_003C_003E3__instance = instance
		};
	}

	private static string? GetDynamicVarName(object dynamicVar)
	{
		return DynamicVarNameProperty?.GetValue(dynamicVar) as string;
	}

	private static Array CreateCreatureArray(IEnumerable<object> targets)
	{
		object[] array = targets.Where((object target) => target != null).ToArray();
		if (CreatureType == null)
		{
			return array;
		}
		Array array2 = Array.CreateInstance(CreatureType, array.Length);
		for (int i = 0; i < array.Length; i++)
		{
			array2.SetValue(array[i], i);
		}
		return array2;
	}

	private static object? UnwrapDynamicVar(object dynamicVarOrPair)
	{
		PropertyInfo? dynamicVarNameProperty = DynamicVarNameProperty;
		if ((object)dynamicVarNameProperty != null && (dynamicVarNameProperty.DeclaringType?.IsInstanceOfType(dynamicVarOrPair)).GetValueOrDefault())
		{
			return dynamicVarOrPair;
		}
		return AccessTools.Property(dynamicVarOrPair.GetType(), "Value")?.GetValue(dynamicVarOrPair);
	}

	private static bool TryAddDynamicVarBaseValue(object dynamicVar, decimal amount)
	{
		if (DynamicVarBaseValueProperty == null || !DynamicVarBaseValueProperty.CanRead || !DynamicVarBaseValueProperty.CanWrite)
		{
			return false;
		}
		if (!(DynamicVarBaseValueProperty.GetValue(dynamicVar) is decimal num) || 1 == 0)
		{
			return false;
		}
		DynamicVarBaseValueProperty.SetValue(dynamicVar, num + amount);
		return true;
	}

	private static string? GetSeedString(object state)
	{
		object obj = RunStateRngProperty?.GetValue(state);
		return (obj == null) ? null : (AccessTools.Property(obj.GetType(), "StringSeed")?.GetValue(obj) as string);
	}

	private static string GetBaseSeedString(object state)
	{
		string text = GetSeedString(state) ?? "STS2PLUS";
		int num = text.LastIndexOf("_L", StringComparison.Ordinal);
		return (num < 0) ? text : text.Substring(0, num);
	}

	private static void CloseSingleton(string typeName, string methodName, params object[] args)
	{
		Type type = RuntimeTypeResolver.FindType(typeName) ?? RuntimeTypeResolver.FindTypeByName(typeName.Split('.').Last());
		object obj = AccessTools.Property(type, "Instance")?.GetValue(null);
		AccessTools.Method(type, methodName, (Type[])null, (Type[])null)?.Invoke(obj, args);
	}

	private static void ClearSingleton(string typeName, string methodName)
	{
		Type type = RuntimeTypeResolver.FindType(typeName) ?? RuntimeTypeResolver.FindTypeByName(typeName.Split('.').Last());
		object obj = AccessTools.Property(type, "Instance")?.GetValue(null);
		AccessTools.Method(type, methodName, (Type[])null, (Type[])null)?.Invoke(obj, Array.Empty<object>());
	}

	private static Vector2? GetNodeGlobalPosition(Node? node)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		Node2D val = (Node2D)(object)((node is Node2D) ? node : null);
		Vector2? result;
		if (val == null)
		{
			Control val2 = (Control)(object)((node is Control) ? node : null);
			result = ((val2 == null) ? null : new Vector2?(val2.GlobalPosition));
		}
		else
		{
			result = val.GlobalPosition;
		}
		return result;
	}
}
