using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using MegaCrit.Sts2.Core.Map;
using STS2Plus.Config;
using STS2Plus.Reflection;

namespace STS2Plus.Features;

internal static class RouteAdvisor
{
	[CompilerGenerated]
	private sealed class _003CEnumerateChildPaths_003Ed__3 : IEnumerable<IReadOnlyList<object>>, IEnumerable, IEnumerator<IReadOnlyList<object>>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private IReadOnlyList<object> _003C_003E2__current;

		private int _003C_003El__initialThreadId;

		private object startPoint;

		public object _003C_003E3__startPoint;

		private IEnumerator<object> _003C_003Es__1;

		private object _003Cchild_003E5__2;

		private IEnumerator<IReadOnlyList<object>> _003C_003Es__3;

		private IReadOnlyList<object> _003Cpath_003E5__4;

		IReadOnlyList<object> IEnumerator<IReadOnlyList<object>>.Current
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
		public _003CEnumerateChildPaths_003Ed__3(int _003C_003E1__state)
		{
			this._003C_003E1__state = _003C_003E1__state;
			_003C_003El__initialThreadId = Environment.CurrentManagedThreadId;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
			int num = _003C_003E1__state;
			if ((uint)(num - -4) <= 1u || num == 1)
			{
				try
				{
					if (num == -4 || num == 1)
					{
						try
						{
						}
						finally
						{
							_003C_003Em__Finally2();
						}
					}
				}
				finally
				{
					_003C_003Em__Finally1();
				}
			}
			_003C_003Es__1 = null;
			_003Cchild_003E5__2 = null;
			_003C_003Es__3 = null;
			_003Cpath_003E5__4 = null;
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
					_003C_003E1__state = -4;
					_003Cpath_003E5__4 = null;
					goto IL_00b8;
				}
				_003C_003E1__state = -1;
				_003C_003Es__1 = GameReflection.GetMapPointChildren(startPoint).GetEnumerator();
				_003C_003E1__state = -3;
				goto IL_00db;
				IL_00b8:
				if (_003C_003Es__3.MoveNext())
				{
					_003Cpath_003E5__4 = _003C_003Es__3.Current;
					_003C_003E2__current = _003Cpath_003E5__4;
					_003C_003E1__state = 1;
					return true;
				}
				_003C_003Em__Finally2();
				_003C_003Es__3 = null;
				_003Cchild_003E5__2 = null;
				goto IL_00db;
				IL_00db:
				if (_003C_003Es__1.MoveNext())
				{
					_003Cchild_003E5__2 = _003C_003Es__1.Current;
					_003C_003Es__3 = EnumeratePaths(_003Cchild_003E5__2).GetEnumerator();
					_003C_003E1__state = -4;
					goto IL_00b8;
				}
				_003C_003Em__Finally1();
				_003C_003Es__1 = null;
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
			if (_003C_003Es__1 != null)
			{
				_003C_003Es__1.Dispose();
			}
		}

		private void _003C_003Em__Finally2()
		{
			_003C_003E1__state = -3;
			if (_003C_003Es__3 != null)
			{
				_003C_003Es__3.Dispose();
			}
		}

		[DebuggerHidden]
		void IEnumerator.Reset()
		{
			throw new NotSupportedException();
		}

		[DebuggerHidden]
		IEnumerator<IReadOnlyList<object>> IEnumerable<IReadOnlyList<object>>.GetEnumerator()
		{
			_003CEnumerateChildPaths_003Ed__3 _003CEnumerateChildPaths_003Ed__;
			if (_003C_003E1__state == -2 && _003C_003El__initialThreadId == Environment.CurrentManagedThreadId)
			{
				_003C_003E1__state = 0;
				_003CEnumerateChildPaths_003Ed__ = this;
			}
			else
			{
				_003CEnumerateChildPaths_003Ed__ = new _003CEnumerateChildPaths_003Ed__3(0);
			}
			_003CEnumerateChildPaths_003Ed__.startPoint = _003C_003E3__startPoint;
			return _003CEnumerateChildPaths_003Ed__;
		}

		[DebuggerHidden]
		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<IReadOnlyList<object>>)this).GetEnumerator();
		}
	}

	[CompilerGenerated]
	private sealed class _003CEnumeratePaths_003Ed__4 : IEnumerable<IReadOnlyList<object>>, IEnumerable, IEnumerator<IReadOnlyList<object>>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private IReadOnlyList<object> _003C_003E2__current;

		private int _003C_003El__initialThreadId;

		private object point;

		public object _003C_003E3__point;

		private object[] _003Cchildren_003E5__1;

		private object[] _003C_003Es__2;

		private int _003C_003Es__3;

		private object _003Cchild_003E5__4;

		private IEnumerator<IReadOnlyList<object>> _003C_003Es__5;

		private IReadOnlyList<object> _003Csuffix_003E5__6;

		private object[] _003Cpath_003E5__7;

		private int _003Cindex_003E5__8;

		IReadOnlyList<object> IEnumerator<IReadOnlyList<object>>.Current
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
		public _003CEnumeratePaths_003Ed__4(int _003C_003E1__state)
		{
			this._003C_003E1__state = _003C_003E1__state;
			_003C_003El__initialThreadId = Environment.CurrentManagedThreadId;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
			int num = _003C_003E1__state;
			if (num == -3 || num == 2)
			{
				try
				{
				}
				finally
				{
					_003C_003Em__Finally1();
				}
			}
			_003Cchildren_003E5__1 = null;
			_003C_003Es__2 = null;
			_003Cchild_003E5__4 = null;
			_003C_003Es__5 = null;
			_003Csuffix_003E5__6 = null;
			_003Cpath_003E5__7 = null;
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
					_003Cchildren_003E5__1 = GameReflection.GetMapPointChildren(point).ToArray();
					if (_003Cchildren_003E5__1.Length == 0)
					{
						_003C_003E2__current = new object[1] { point };
						_003C_003E1__state = 1;
						return true;
					}
					_003C_003Es__2 = _003Cchildren_003E5__1;
					_003C_003Es__3 = 0;
					goto IL_01c8;
				case 1:
					_003C_003E1__state = -1;
					return false;
				case 2:
					{
						_003C_003E1__state = -3;
						_003Cpath_003E5__7 = null;
						_003Csuffix_003E5__6 = null;
						goto IL_0194;
					}
					IL_0194:
					if (_003C_003Es__5.MoveNext())
					{
						_003Csuffix_003E5__6 = _003C_003Es__5.Current;
						_003Cpath_003E5__7 = new object[_003Csuffix_003E5__6.Count + 1];
						_003Cpath_003E5__7[0] = point;
						_003Cindex_003E5__8 = 0;
						while (_003Cindex_003E5__8 < _003Csuffix_003E5__6.Count)
						{
							_003Cpath_003E5__7[_003Cindex_003E5__8 + 1] = _003Csuffix_003E5__6[_003Cindex_003E5__8];
							_003Cindex_003E5__8++;
						}
						_003C_003E2__current = _003Cpath_003E5__7;
						_003C_003E1__state = 2;
						return true;
					}
					_003C_003Em__Finally1();
					_003C_003Es__5 = null;
					_003Cchild_003E5__4 = null;
					_003C_003Es__3++;
					goto IL_01c8;
					IL_01c8:
					if (_003C_003Es__3 < _003C_003Es__2.Length)
					{
						_003Cchild_003E5__4 = _003C_003Es__2[_003C_003Es__3];
						_003C_003Es__5 = EnumeratePaths(_003Cchild_003E5__4).GetEnumerator();
						_003C_003E1__state = -3;
						goto IL_0194;
					}
					_003C_003Es__2 = null;
					return false;
				}
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
			if (_003C_003Es__5 != null)
			{
				_003C_003Es__5.Dispose();
			}
		}

		[DebuggerHidden]
		void IEnumerator.Reset()
		{
			throw new NotSupportedException();
		}

		[DebuggerHidden]
		IEnumerator<IReadOnlyList<object>> IEnumerable<IReadOnlyList<object>>.GetEnumerator()
		{
			_003CEnumeratePaths_003Ed__4 _003CEnumeratePaths_003Ed__;
			if (_003C_003E1__state == -2 && _003C_003El__initialThreadId == Environment.CurrentManagedThreadId)
			{
				_003C_003E1__state = 0;
				_003CEnumeratePaths_003Ed__ = this;
			}
			else
			{
				_003CEnumeratePaths_003Ed__ = new _003CEnumeratePaths_003Ed__4(0);
			}
			_003CEnumeratePaths_003Ed__.point = _003C_003E3__point;
			return _003CEnumeratePaths_003Ed__;
		}

		[DebuggerHidden]
		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<IReadOnlyList<object>>)this).GetEnumerator();
		}
	}

	public static RouteAdvice? BuildAdvice()
	{
		if (!ConfigManager.Current.RouteAdvisorEnabled)
		{
			return null;
		}
		object routeStartPoint = GameReflection.GetRouteStartPoint();
		if (routeStartPoint == null)
		{
			return null;
		}
		IReadOnlyList<object>[] array = (from path in EnumerateChildPaths(routeStartPoint)
			where path.Count > 0
			select path).ToArray();
		if (array.Length == 0)
		{
			return null;
		}
		IReadOnlyList<object> readOnlyList = FindBestPath(array, ScoreSafe);
		IReadOnlyList<object> readOnlyList2 = FindBestPath(array, ScoreAggressive);
		if (readOnlyList == null && readOnlyList2 == null)
		{
			return null;
		}
		ModEntry.Verbose($"RouteAdvisor: advice generated, safe={ScoreSafe(readOnlyList ?? readOnlyList2!)} aggro={ScoreAggressive(readOnlyList2 ?? readOnlyList!)}");
		return new RouteAdvice((readOnlyList == null) ? null : CreateSuggestion("Safe", routeStartPoint, readOnlyList, ScoreSafe(readOnlyList)), (readOnlyList2 == null) ? null : CreateSuggestion("Aggro", routeStartPoint, readOnlyList2, ScoreAggressive(readOnlyList2)));
	}

	private static RouteSuggestion CreateSuggestion(string profileId, object startPoint, IReadOnlyList<object> path, int score)
	{
		return new RouteSuggestion(profileId, score, startPoint, path);
	}

	private static IReadOnlyList<object>? FindBestPath(IEnumerable<IReadOnlyList<object>> paths, Func<IReadOnlyList<object>, int> scorer)
	{
		IReadOnlyList<object> readOnlyList = null;
		int num = int.MinValue;
		foreach (IReadOnlyList<object> path in paths)
		{
			int num2 = scorer(path);
			if (readOnlyList == null || num2 > num)
			{
				readOnlyList = path;
				num = num2;
			}
		}
		return readOnlyList;
	}

	[IteratorStateMachine(typeof(_003CEnumerateChildPaths_003Ed__3))]
	private static IEnumerable<IReadOnlyList<object>> EnumerateChildPaths(object startPoint)
	{
		//yield-return decompiler failed: Unexpected instruction in Iterator.Dispose()
		return new _003CEnumerateChildPaths_003Ed__3(-2)
		{
			_003C_003E3__startPoint = startPoint
		};
	}

	[IteratorStateMachine(typeof(_003CEnumeratePaths_003Ed__4))]
	private static IEnumerable<IReadOnlyList<object>> EnumeratePaths(object point)
	{
		//yield-return decompiler failed: Unexpected instruction in Iterator.Dispose()
		return new _003CEnumeratePaths_003Ed__4(-2)
		{
			_003C_003E3__point = point
		};
	}

	private static int ScoreSafe(IReadOnlyList<object> path)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Expected I4, but got Unknown
		int num = 0;
		foreach (object item in path)
		{
			int num2 = num;
			MapPointType mapPointType = GameReflection.GetMapPointType(item);
			if (1 == 0)
			{
			}
			int num3 = ((int)mapPointType - 1) switch
			{
				1 => 7,
				3 => 5,
				2 => 4,
				0 => 2,
				4 => 1,
				5 => -3,
				6 => 3,
				_ => 0,
			};
			if (1 == 0)
			{
			}
			num = num2 + num3;
		}
		return num;
	}

	private static int ScoreAggressive(IReadOnlyList<object> path)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Expected I4, but got Unknown
		int num = 0;
		foreach (object item in path)
		{
			int num2 = num;
			MapPointType mapPointType = GameReflection.GetMapPointType(item);
			if (1 == 0)
			{
			}
			int num3 = ((int)mapPointType - 1) switch
			{
				5 => 8,
				4 => 3,
				2 => 4,
				0 => 2,
				1 => 1,
				3 => 1,
				6 => 5,
				_ => 0,
			};
			if (1 == 0)
			{
			}
			num = num2 + num3;
		}
		return num;
	}
}
