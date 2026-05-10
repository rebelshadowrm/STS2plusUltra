using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Helpers;
using STS2Plus.Modifiers;
using STS2Plus.Reflection;

namespace STS2Plus.Patches;

[HarmonyPatchCategory("MoreRules")]
[HarmonyPatch]
internal static class DeprecatedModifierTopBarPatch
{
	private sealed class EntryHolder
	{
		public required string Entry { get; init; }
	}

	[CompilerGenerated]
	private sealed class _003CEnumerateNodes_003Ed__15 : IEnumerable<Node>, IEnumerable, IEnumerator<Node>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private Node _003C_003E2__current;

		private int _003C_003El__initialThreadId;

		private Node root;

		public Node _003C_003E3__root;

		private IEnumerator<Node> _003C_003Es__1;

		private Node _003Cchild_003E5__2;

		private Node _003Cnode_003E5__3;

		private IEnumerator<Node> _003C_003Es__4;

		private Node _003Cnested_003E5__5;

		Node IEnumerator<Node>.Current
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
		public _003CEnumerateNodes_003Ed__15(int _003C_003E1__state)
		{
			this._003C_003E1__state = _003C_003E1__state;
			_003C_003El__initialThreadId = System.Environment.CurrentManagedThreadId;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
			int num = _003C_003E1__state;
			if ((uint)(num - -4) <= 1u || num == 2)
			{
				try
				{
					if (num == -4 || num == 2)
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
			_003Cnode_003E5__3 = null;
			_003C_003Es__4 = null;
			_003Cnested_003E5__5 = null;
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
					_003C_003E2__current = root;
					_003C_003E1__state = 1;
					return true;
				case 1:
					_003C_003E1__state = -1;
					_003C_003Es__1 = root.GetChildren(false).GetEnumerator();
					_003C_003E1__state = -3;
					goto IL_012d;
				case 2:
					{
						_003C_003E1__state = -4;
						_003Cnested_003E5__5 = null;
						goto IL_0102;
					}
					IL_012d:
					if (_003C_003Es__1.MoveNext())
					{
						_003Cchild_003E5__2 = _003C_003Es__1.Current;
						if (_003Cchild_003E5__2 != null)
						{
							_003Cnode_003E5__3 = _003Cchild_003E5__2;
							if (true)
							{
								_003C_003Es__4 = EnumerateNodes(_003Cnode_003E5__3).GetEnumerator();
								_003C_003E1__state = -4;
								goto IL_0102;
							}
						}
						goto IL_011e;
					}
					_003C_003Em__Finally1();
					_003C_003Es__1 = null;
					return false;
					IL_0102:
					if (_003C_003Es__4.MoveNext())
					{
						_003Cnested_003E5__5 = _003C_003Es__4.Current;
						_003C_003E2__current = _003Cnested_003E5__5;
						_003C_003E1__state = 2;
						return true;
					}
					_003C_003Em__Finally2();
					_003C_003Es__4 = null;
					goto IL_011e;
					IL_011e:
					_003Cnode_003E5__3 = null;
					_003Cchild_003E5__2 = null;
					goto IL_012d;
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
			if (_003C_003Es__1 != null)
			{
				_003C_003Es__1.Dispose();
			}
		}

		private void _003C_003Em__Finally2()
		{
			_003C_003E1__state = -3;
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
		IEnumerator<Node> IEnumerable<Node>.GetEnumerator()
		{
			_003CEnumerateNodes_003Ed__15 _003CEnumerateNodes_003Ed__;
			if (_003C_003E1__state == -2 && _003C_003El__initialThreadId == System.Environment.CurrentManagedThreadId)
			{
				_003C_003E1__state = 0;
				_003CEnumerateNodes_003Ed__ = this;
			}
			else
			{
				_003CEnumerateNodes_003Ed__ = new _003CEnumerateNodes_003Ed__15(0);
			}
			_003CEnumerateNodes_003Ed__.root = _003C_003E3__root;
			return _003CEnumerateNodes_003Ed__;
		}

		[DebuggerHidden]
		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<Node>)this).GetEnumerator();
		}
	}

	private const string GenericEntry = "DEPRECATED_MODIFIER";

	private const string DisplayTitleLabelName = "STS2PlusModifierTitle";

	private const string DisplayDescriptionLabelName = "STS2PlusModifierDescription";

	private static readonly ConditionalWeakTable<object, EntryHolder> DisplayEntries = new ConditionalWeakTable<object, EntryHolder>();

	[HarmonyTargetMethod]
	private static MethodBase? TargetMethod()
	{
		Type type = RuntimeTypeResolver.FindType("MegaCrit.Sts2.Core.Nodes.TopBar.NTopBarModifier") ?? RuntimeTypeResolver.FindTypeByName("NTopBarModifier");
		return (type == null) ? null : AccessTools.Method(type, "_Ready", (Type[])null, (Type[])null);
	}

	private static void Prefix(object __instance)
	{
		if (IsDeprecatedModifierInstance(__instance))
		{
			string text = ResolveDisplayEntry(__instance);
			if (text == null)
			{
				text = "DEPRECATED_MODIFIER";
			}
			RememberDisplayEntry(__instance, text);
			ModEntry.Logger.Warn("STS2Plus.MoreRules replaced top-bar deprecated modifier with " + text + ".", 1);
		}
	}

	private static void Postfix(object __instance)
	{
		if (TryGetDisplayEntry(__instance, out string entry))
		{
			ApplyIcon(__instance, entry);
		}
	}

	private static string? ResolveDisplayEntry(object instance)
	{
		IReadOnlyList<string> displayRuleEntries = PlusState.GetDisplayRuleEntries();
		if (displayRuleEntries.Count == 0)
		{
			return null;
		}
		Node val = (Node)((instance is Node) ? instance : null);
		if (val == null || val.GetParent() == null)
		{
			return displayRuleEntries[0];
		}
		int num = 0;
		foreach (Node child in val.GetParent().GetChildren(false))
		{
			if (child == null)
			{
				continue;
			}
			Node val2 = child;
			if (!(((object)val2).GetType() != ((object)val).GetType()))
			{
				if (val2 == val)
				{
					break;
				}
				if (IsDeprecatedTopBarModifier(val2))
				{
					num++;
				}
			}
		}
		return displayRuleEntries[Math.Min(num, displayRuleEntries.Count - 1)];
	}

	private static bool IsDeprecatedTopBarModifier(Node node)
	{
		return IsDeprecatedModifierInstance(node);
	}

	private static void ApplyIcon(object instance, string entry)
	{
		object? obj = AccessTools.Field(instance.GetType(), "_icon")?.GetValue(instance);
		TextureRect val = (TextureRect)((obj is TextureRect) ? obj : null);
		if (val != null)
		{
			string imagePath = ImageHelper.GetImagePath(SyncedModifierModel.GetBuiltInIconPathForEntry(entry));
			Texture2D val2 = ResourceLoader.Load<Texture2D>(imagePath, (string)null, (ResourceLoader.CacheMode)1);
			if (val2 == null)
			{
				ModEntry.Logger.Warn($"STS2Plus.MoreRules could not load top-bar icon for {entry} from {imagePath}.", 1);
			}
			else
			{
				val.Texture = val2;
				((CanvasItem)val).Visible = true;
			}
		}
	}

	private static bool IsDeprecatedModifierInstance(object instance)
	{
		object obj = AccessTools.Field(instance.GetType(), "_modifier")?.GetValue(instance);
		object obj2 = AccessTools.Property(obj?.GetType(), "Id")?.GetValue(obj);
		string a = AccessTools.Property(obj2?.GetType(), "Entry")?.GetValue(obj2) as string;
		return string.Equals(a, "DEPRECATED_MODIFIER", StringComparison.OrdinalIgnoreCase);
	}

	private static void RememberDisplayEntry(object owner, string entry)
	{
		DisplayEntries.Remove(owner);
		DisplayEntries.Add(owner, new EntryHolder
		{
			Entry = entry
		});
	}

	internal static bool TryGetDisplayEntry(object owner, out string entry)
	{
		if (DisplayEntries.TryGetValue(owner, out EntryHolder value))
		{
			entry = value.Entry;
			return true;
		}
		entry = string.Empty;
		return false;
	}

	internal static void RefreshVisibleTopBarModifiers()
	{
		MainLoop mainLoop = Engine.GetMainLoop();
		MainLoop obj = ((mainLoop is SceneTree) ? mainLoop : null);
		Window val = ((obj != null) ? ((SceneTree)obj).Root : null);
		if (val == null)
		{
			return;
		}
		foreach (Node item in EnumerateNodes((Node)(object)val))
		{
			if (((object)item).GetType().Name.Contains("NTopBarModifier", StringComparison.Ordinal))
			{
				if (IsDeprecatedModifierInstance(item))
				{
					string entry = ResolveDisplayEntry(item) ?? "DEPRECATED_MODIFIER";
					RememberDisplayEntry(item, entry);
				}
				if (TryGetDisplayEntry(item, out string entry2))
				{
					ApplyIcon(item, entry2);
				}
			}
		}
	}

	[IteratorStateMachine(typeof(_003CEnumerateNodes_003Ed__15))]
	private static IEnumerable<Node> EnumerateNodes(Node root)
	{
		//yield-return decompiler failed: Unexpected instruction in Iterator.Dispose()
		return new _003CEnumerateNodes_003Ed__15(-2)
		{
			_003C_003E3__root = root
		};
	}
}
