using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace STS2Plus.Modifiers;

internal static class CustomModifierCatalog
{
	[CompilerGenerated]
	private sealed class _003CEnumerateSavedPropertyValues_003Ed__21 : IEnumerable<object>, IEnumerable, IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object? _003C_003E2__current;

		private int _003C_003El__initialThreadId;

		private object props;

		public object _003C_003E3__props;

		private string fieldName;

		public string _003C_003E3__fieldName;

		private FieldInfo _003Cfield_003E5__1;

		private IEnumerable _003CsavedProperties_003E5__2;

		private IEnumerator _003C_003Es__3;

		private object _003CsavedProperty_003E5__4;

		private object _003Cvalue_003E5__5;

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
		public _003CEnumerateSavedPropertyValues_003Ed__21(int _003C_003E1__state)
		{
			this._003C_003E1__state = _003C_003E1__state;
			_003C_003El__initialThreadId = Environment.CurrentManagedThreadId;
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
			_003Cfield_003E5__1 = null;
			_003CsavedProperties_003E5__2 = null;
			_003C_003Es__3 = null;
			_003CsavedProperty_003E5__4 = null;
			_003Cvalue_003E5__5 = null;
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
				{
					_003C_003E1__state = -1;
					_003Cfield_003E5__1 = AccessTools.Field(props.GetType(), fieldName);
					object obj = _003Cfield_003E5__1?.GetValue(props);
					_003CsavedProperties_003E5__2 = obj as IEnumerable;
					if (_003CsavedProperties_003E5__2 == null)
					{
						return false;
					}
					_003C_003Es__3 = _003CsavedProperties_003E5__2.GetEnumerator();
					_003C_003E1__state = -3;
					break;
				}
				case 1:
					_003C_003E1__state = -3;
					_003Cvalue_003E5__5 = null;
					_003CsavedProperty_003E5__4 = null;
					break;
				}
				if (_003C_003Es__3.MoveNext())
				{
					_003CsavedProperty_003E5__4 = _003C_003Es__3.Current;
					_003Cvalue_003E5__5 = AccessTools.Field(_003CsavedProperty_003E5__4?.GetType(), "value")?.GetValue(_003CsavedProperty_003E5__4);
					_003C_003E2__current = _003Cvalue_003E5__5;
					_003C_003E1__state = 1;
					return true;
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
			_003CEnumerateSavedPropertyValues_003Ed__21 _003CEnumerateSavedPropertyValues_003Ed__;
			if (_003C_003E1__state == -2 && _003C_003El__initialThreadId == Environment.CurrentManagedThreadId)
			{
				_003C_003E1__state = 0;
				_003CEnumerateSavedPropertyValues_003Ed__ = this;
			}
			else
			{
				_003CEnumerateSavedPropertyValues_003Ed__ = new _003CEnumerateSavedPropertyValues_003Ed__21(0);
			}
			_003CEnumerateSavedPropertyValues_003Ed__.props = _003C_003E3__props;
			_003CEnumerateSavedPropertyValues_003Ed__.fieldName = _003C_003E3__fieldName;
			return _003CEnumerateSavedPropertyValues_003Ed__;
		}

		[DebuggerHidden]
		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<object>)this).GetEnumerator();
		}
	}

	public const string Category = "STS2PLUS";

	public const string LegacyCategory = "MODIFIER";

	public const string AttackDefenseEntry = "ATTACK_DEFENSE";

	public const string AttackDefensePlusEntry = "ATTACK_DEFENSE_PLUS";

	public const string IronSkinEntry = "IRON_SKIN";

	public const string GiantCreaturesEntry = "GIANT_CREATURES";

	public const string HardElitesEntry = "HARD_ELITES";

	public const string EndlessModeEntry = "ENDLESS_MODE";

	public const string GlassCannonEntry = "GLASS_CANNON";

	public const string UnlimitedGrowthEntry = "UNLIMITED_GROWTH";

	public const string SandboxEntry = "SANDBOX";

	public const string BuildCreatorEntry = "BUILD_CREATOR";

	private static readonly string[] KnownEntries = new string[10] { "ATTACK_DEFENSE", "ATTACK_DEFENSE_PLUS", "IRON_SKIN", "GIANT_CREATURES", "HARD_ELITES", "ENDLESS_MODE", "GLASS_CANNON", "UNLIMITED_GROWTH", "SANDBOX", "BUILD_CREATOR" };

	public static IReadOnlyList<string> GetKnownEntries()
	{
		return KnownEntries;
	}

	public static ModifierModel Create(string entry)
	{
		return GetCanonical(entry).ToMutable();
	}

	public static ModifierModel? TryCreate(SerializableModifier serializable)
	{
		ModelId id = serializable.Id;
		if (id == (ModelId)null)
		{
			return null;
		}
		if (IsKnownId(id.Category, id.Entry))
		{
			return Create(id.Entry);
		}
		string text = TryRecoverDeprecatedEntry(serializable);
		if (text != null)
		{
			ModEntry.Logger.Warn($"STS2Plus.MoreRules recovered deprecated modifier {id.Category}.{id.Entry} -> {text}.", 1);
			return Create(text);
		}
		if (string.Equals(id.Entry, "DEPRECATED_MODIFIER", StringComparison.OrdinalIgnoreCase))
		{
			ModEntry.Logger.Warn("STS2Plus.MoreRules could not recover deprecated modifier. Props=" + DescribeSavedProperties(serializable.Props), 1);
		}
		return null;
	}

	public static bool ContainsEntry(IEnumerable<ModifierModel>? modifiers, string entry)
	{
		string entry2 = entry;
		return modifiers?.Any((ModifierModel modifier) => HasEntry(modifier, entry2)) ?? false;
	}

	public static bool HasEntry(ModifierModel? modifier, string entry)
	{
		ModelId val = ((modifier != null) ? ((AbstractModel)modifier).Id : null);
		return val != (ModelId)null && IsKnownId(val.Category, val.Entry) && string.Equals(val.Entry, entry, StringComparison.Ordinal);
	}

	public static bool IsKnownModifier(ModifierModel? modifier)
	{
		ModelId val = ((modifier != null) ? ((AbstractModel)modifier).Id : null);
		return val != (ModelId)null && IsKnownId(val.Category, val.Entry);
	}

	private static ModifierModel GetCanonical(string entry)
	{
		if (1 == 0)
		{
		}
		Type type = entry switch
		{
			"ATTACK_DEFENSE" => typeof(AttackDefense), 
			"ATTACK_DEFENSE_PLUS" => typeof(AttackDefensePlus), 
			"IRON_SKIN" => typeof(IronSkin), 
			"GIANT_CREATURES" => typeof(GiantCreatures), 
			"HARD_ELITES" => typeof(HardElites), 
			"ENDLESS_MODE" => typeof(EndlessMode), 
			"GLASS_CANNON" => typeof(GlassCannon), 
			"UNLIMITED_GROWTH" => typeof(UnlimitedGrowth), 
			"SANDBOX" => typeof(Sandbox), 
			"BUILD_CREATOR" => typeof(BuildCreator), 
			_ => null, 
		};
		if (1 == 0)
		{
		}
		Type type2 = type;
		if (type2 == null)
		{
			throw new ArgumentOutOfRangeException("entry", entry, "Unknown STS2Plus modifier entry.");
		}
		if (!ModelDb.Contains(type2))
		{
			ModelDb.Inject(type2);
		}
		ModifierModel byIdOrNull = ModelDb.GetByIdOrNull<ModifierModel>(ModelDb.GetId(type2));
		if (byIdOrNull == null)
		{
			throw new InvalidOperationException("STS2Plus.MoreRules could not resolve canonical modifier for entry " + entry + ".");
		}
		((AbstractModel)byIdOrNull).InitId(((AbstractModel)byIdOrNull).Id);
		return byIdOrNull;
	}

	private static bool IsKnownId(string? category, string? entry)
	{
		if (entry == null)
		{
			return false;
		}
		bool flag = string.Equals(category, "STS2PLUS", StringComparison.OrdinalIgnoreCase);
		bool flag2 = string.Equals(category, "MODIFIER", StringComparison.OrdinalIgnoreCase);
		if (!flag && !flag2)
		{
			return false;
		}
		return KnownEntries.Any((string knownEntry) => string.Equals(entry, knownEntry, StringComparison.OrdinalIgnoreCase));
	}

	private static string? TryRecoverDeprecatedEntry(SerializableModifier serializable)
	{
		SavedProperties props = serializable.Props;
		if (props == null)
		{
			return null;
		}
		foreach (object item in EnumerateSavedPropertyValues(props, "modelIds"))
		{
			ModelId val = (ModelId)((item is ModelId) ? item : null);
			if (val != null && IsKnownId(val.Category, val.Entry))
			{
				return val.Entry;
			}
		}
		foreach (object item2 in EnumerateSavedPropertyValues(props, "strings"))
		{
			if (item2 is string text && TryParseKnownId(text, out string entry))
			{
				return entry;
			}
		}
		return null;
	}

	private static bool TryParseKnownId(string text, out string entry)
	{
		entry = string.Empty;
		if (string.IsNullOrWhiteSpace(text))
		{
			return false;
		}
		string[] array = text.Split('.', 2, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
		if (array.Length == 2 && IsKnownId(array[0], array[1]))
		{
			entry = array[1];
			return true;
		}
		if (IsKnownId("STS2PLUS", text) || IsKnownId("MODIFIER", text))
		{
			entry = text;
			return true;
		}
		return false;
	}

	[IteratorStateMachine(typeof(_003CEnumerateSavedPropertyValues_003Ed__21))]
	private static IEnumerable<object?> EnumerateSavedPropertyValues(object props, string fieldName)
	{
		//yield-return decompiler failed: Unexpected instruction in Iterator.Dispose()
		return new _003CEnumerateSavedPropertyValues_003Ed__21(-2)
		{
			_003C_003E3__props = props,
			_003C_003E3__fieldName = fieldName
		};
	}

	private static string DescribeSavedProperties(object? props)
	{
		if (props == null)
		{
			return "<null>";
		}
		List<string> list = new List<string>();
		string[] array = new string[4] { "strings", "modelIds", "ints", "bools" };
		foreach (string text2 in array)
		{
			string[] array2 = (from value in EnumerateSavedPropertyValues(props, text2)
				where value != null
				select value.ToString() into text
				where !string.IsNullOrWhiteSpace(text)
				select text).Take(8).ToArray();
			if (array2.Length != 0)
			{
				list.Add(text2 + "=[" + string.Join(", ", array2) + "]");
			}
		}
		return (list.Count == 0) ? "<empty>" : string.Join(" | ", list);
	}
}
