using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace STS2Plus.Patches;

internal static class UnlimitedGrowthSafety
{
	[CompilerGenerated]
	private sealed class _003CEnumerateBehaviorMethods_003Ed__15 : IEnumerable<MethodInfo>, IEnumerable, IEnumerator<MethodInfo>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private MethodInfo _003C_003E2__current;

		private int _003C_003El__initialThreadId;

		private Type cardType;

		public Type _003C_003E3__cardType;

		private HashSet<MethodInfo> _003CseenMethods_003E5__1;

		private Type _003Ccurrent_003E5__2;

		private string[] _003C_003Es__3;

		private int _003C_003Es__4;

		private string _003CmethodName_003E5__5;

		private MethodInfo _003Cmethod_003E5__6;

		MethodInfo IEnumerator<MethodInfo>.Current
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
		public _003CEnumerateBehaviorMethods_003Ed__15(int _003C_003E1__state)
		{
			this._003C_003E1__state = _003C_003E1__state;
			_003C_003El__initialThreadId = Environment.CurrentManagedThreadId;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
			_003CseenMethods_003E5__1 = null;
			_003Ccurrent_003E5__2 = null;
			_003C_003Es__3 = null;
			_003CmethodName_003E5__5 = null;
			_003Cmethod_003E5__6 = null;
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
				goto IL_00f2;
			}
			_003C_003E1__state = -1;
			_003CseenMethods_003E5__1 = new HashSet<MethodInfo>();
			_003Ccurrent_003E5__2 = cardType;
			goto IL_013b;
			IL_010f:
			if (_003C_003Es__4 < _003C_003Es__3.Length)
			{
				_003CmethodName_003E5__5 = _003C_003Es__3[_003C_003Es__4];
				_003Cmethod_003E5__6 = AccessTools.DeclaredMethod(_003Ccurrent_003E5__2, _003CmethodName_003E5__5, (Type[])null, (Type[])null);
				if (_003Cmethod_003E5__6 != null && _003CseenMethods_003E5__1.Add(_003Cmethod_003E5__6))
				{
					_003C_003E2__current = _003Cmethod_003E5__6;
					_003C_003E1__state = 1;
					return true;
				}
				goto IL_00f2;
			}
			_003C_003Es__3 = null;
			_003Ccurrent_003E5__2 = _003Ccurrent_003E5__2.BaseType;
			goto IL_013b;
			IL_00f2:
			_003Cmethod_003E5__6 = null;
			_003CmethodName_003E5__5 = null;
			_003C_003Es__4++;
			goto IL_010f;
			IL_013b:
			if (_003Ccurrent_003E5__2 != null && typeof(CardModel).IsAssignableFrom(_003Ccurrent_003E5__2))
			{
				_003C_003Es__3 = new string[5] { "OnPlay", "OnTurnEndInHand", "BeforeHandDraw", "AfterCardDrawn", "ModifyHandDraw" };
				_003C_003Es__4 = 0;
				goto IL_010f;
			}
			_003Ccurrent_003E5__2 = null;
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
		IEnumerator<MethodInfo> IEnumerable<MethodInfo>.GetEnumerator()
		{
			_003CEnumerateBehaviorMethods_003Ed__15 _003CEnumerateBehaviorMethods_003Ed__;
			if (_003C_003E1__state == -2 && _003C_003El__initialThreadId == Environment.CurrentManagedThreadId)
			{
				_003C_003E1__state = 0;
				_003CEnumerateBehaviorMethods_003Ed__ = this;
			}
			else
			{
				_003CEnumerateBehaviorMethods_003Ed__ = new _003CEnumerateBehaviorMethods_003Ed__15(0);
			}
			_003CEnumerateBehaviorMethods_003Ed__.cardType = _003C_003E3__cardType;
			return _003CEnumerateBehaviorMethods_003Ed__;
		}

		[DebuggerHidden]
		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<MethodInfo>)this).GetEnumerator();
		}
	}

	public const int UnlimitedGrowthUpgradeCap = 99;

	private static readonly object SyncRoot;

	private static readonly Dictionary<Type, bool> DrawSensitiveCache;

	private static readonly HashSet<string> LoggedClampWarnings;

	private static readonly OpCode[] SingleByteOpCodes;

	private static readonly OpCode[] MultiByteOpCodes;

	static UnlimitedGrowthSafety()
	{
		SyncRoot = new object();
		DrawSensitiveCache = new Dictionary<Type, bool>();
		LoggedClampWarnings = new HashSet<string>(StringComparer.Ordinal);
		SingleByteOpCodes = new OpCode[256];
		MultiByteOpCodes = new OpCode[256];
		FieldInfo[] fields = typeof(OpCodes).GetFields(BindingFlags.Static | BindingFlags.Public);
		foreach (FieldInfo fieldInfo in fields)
		{
			if (fieldInfo.GetValue(null) is OpCode opCode)
			{
				ushort num = (ushort)opCode.Value;
				if (num < 256)
				{
					SingleByteOpCodes[num] = opCode;
				}
				else if ((num & 0xFF00) == 65024)
				{
					MultiByteOpCodes[num & 0xFF] = opCode;
				}
			}
		}
	}

	public static int PrepareSerializableUpgradeLevel(SerializableCard save)
	{
		int num = Math.Max(0, save.CurrentUpgradeLevel);
		if (num == 0 || save.Id == (ModelId)null)
		{
			return num;
		}
		CardModel val = ResolveCanonicalCard(save.Id);
		if (val == null)
		{
			return num;
		}
		int num2 = Math.Max(0, val.MaxUpgradeLevel);
		if (num <= num2)
		{
			return num;
		}
		if (CanUseUnlimitedGrowth(val, num2))
		{
			return num;
		}
		save.CurrentUpgradeLevel = num2;
		LogClampedUpgradeWarning(val, num, num2);
		return num2;
	}

	public static bool CanUseUnlimitedGrowth(CardModel card, int originalMaxUpgradeLevel)
	{
		if (originalMaxUpgradeLevel <= 0)
		{
			return false;
		}
		return !IsDrawSensitive(((object)card).GetType());
	}

	public static bool ShouldAllowSerializedUpgrade(CardModel card, int originalMaxUpgradeLevel, int savedUpgradeLevel)
	{
		return savedUpgradeLevel > originalMaxUpgradeLevel && CanUseUnlimitedGrowth(card, originalMaxUpgradeLevel);
	}

	public static bool ShouldAllowObservedUpgrade(CardModel card, int originalMaxUpgradeLevel, int currentUpgradeLevel)
	{
		return currentUpgradeLevel > originalMaxUpgradeLevel && CanUseUnlimitedGrowth(card, originalMaxUpgradeLevel);
	}

	private static CardModel? ResolveCanonicalCard(ModelId id)
	{
		return ModelDb.GetByIdOrNull<CardModel>(id);
	}

	private static void LogClampedUpgradeWarning(CardModel card, int savedUpgradeLevel, int clampedUpgradeLevel)
	{
		string value = ((object)((AbstractModel)card).Id).ToString();
		string item = $"{value}:{savedUpgradeLevel}->{clampedUpgradeLevel}";
		lock (SyncRoot)
		{
			if (!LoggedClampWarnings.Add(item))
			{
				return;
			}
		}
		ModEntry.Logger.Warn($"STS2Plus.UnlimitedGrowth clamped saved upgrade level for {value} from {savedUpgradeLevel} to {clampedUpgradeLevel} to avoid multiplayer instability.", 1);
	}

	private static bool IsDrawSensitive(Type cardType)
	{
		lock (SyncRoot)
		{
			if (DrawSensitiveCache.TryGetValue(cardType, out var value))
			{
				return value;
			}
		}
		bool flag = IsDrawSensitiveSlow(cardType);
		lock (SyncRoot)
		{
			DrawSensitiveCache[cardType] = flag;
		}
		return flag;
	}

	private static bool IsDrawSensitiveSlow(Type cardType)
	{
		foreach (MethodInfo item in EnumerateBehaviorMethods(cardType))
		{
			if (string.Equals(item.Name, "BeforeHandDraw", StringComparison.Ordinal) || string.Equals(item.Name, "AfterCardDrawn", StringComparison.Ordinal) || string.Equals(item.Name, "ModifyHandDraw", StringComparison.Ordinal))
			{
				return true;
			}
			if (MethodReferencesDrawFlow(item))
			{
				return true;
			}
		}
		return false;
	}

	[IteratorStateMachine(typeof(_003CEnumerateBehaviorMethods_003Ed__15))]
	private static IEnumerable<MethodInfo> EnumerateBehaviorMethods(Type cardType)
	{
		//yield-return decompiler failed: Unexpected instruction in Iterator.Dispose()
		return new _003CEnumerateBehaviorMethods_003Ed__15(-2)
		{
			_003C_003E3__cardType = cardType
		};
	}

	private static bool MethodReferencesDrawFlow(MethodInfo method)
	{
		byte[] array = method.GetMethodBody()?.GetILAsByteArray();
		if (array == null || array.Length == 0)
		{
			return false;
		}
		int index = 0;
		while (index < array.Length)
		{
			OpCode opCode = ReadOpCode(array, ref index);
			switch (opCode.OperandType)
			{
			case OperandType.InlineMethod:
			{
				int metadataToken2 = BitConverter.ToInt32(array, index);
				index += 4;
				if (TryResolveMethod(method, metadataToken2, out MethodBase resolvedMethod) && IsDrawSensitiveMember(resolvedMethod))
				{
					return true;
				}
				break;
			}
			case OperandType.InlineTok:
			case OperandType.InlineType:
			{
				int metadataToken = BitConverter.ToInt32(array, index);
				index += 4;
				if (TryResolveToken(method, metadataToken, out MemberInfo resolvedMember) && IsDrawSensitiveMember(resolvedMember))
				{
					return true;
				}
				break;
			}
			default:
				index = AdvanceOperand(array, index, opCode.OperandType);
				break;
			}
		}
		return false;
	}

	private static OpCode ReadOpCode(IReadOnlyList<byte> il, ref int index)
	{
		byte b = il[index++];
		if (b != 254)
		{
			return SingleByteOpCodes[b];
		}
		return MultiByteOpCodes[il[index++]];
	}

	private static int AdvanceOperand(IReadOnlyList<byte> il, int index, OperandType operandType)
	{
		if (1 == 0)
		{
		}
		int result = operandType switch
		{
			OperandType.InlineNone => index, 
			OperandType.ShortInlineBrTarget => index + 1, 
			OperandType.ShortInlineI => index + 1, 
			OperandType.ShortInlineVar => index + 1, 
			OperandType.InlineVar => index + 2, 
			OperandType.InlineI => index + 4, 
			OperandType.InlineBrTarget => index + 4, 
			OperandType.InlineField => index + 4, 
			OperandType.InlineMethod => index + 4, 
			OperandType.InlineSig => index + 4, 
			OperandType.InlineString => index + 4, 
			OperandType.InlineTok => index + 4, 
			OperandType.InlineType => index + 4, 
			OperandType.ShortInlineR => index + 4, 
			OperandType.InlineI8 => index + 8, 
			OperandType.InlineR => index + 8, 
			OperandType.InlineSwitch => index + 4 + BitConverter.ToInt32((il is byte[] array) ? array : il.ToArray(), index) * 4, 
			_ => index, 
		};
		if (1 == 0)
		{
		}
		return result;
	}

	private static bool TryResolveMethod(MethodInfo context, int metadataToken, out MethodBase? resolvedMethod)
	{
		resolvedMethod = null;
		try
		{
			resolvedMethod = context.Module.ResolveMethod(metadataToken, context.DeclaringType?.GetGenericArguments(), context.IsGenericMethod ? context.GetGenericArguments() : null);
			return resolvedMethod != null;
		}
		catch
		{
			return false;
		}
	}

	private static bool TryResolveToken(MethodInfo context, int metadataToken, out MemberInfo? resolvedMember)
	{
		resolvedMember = null;
		try
		{
			resolvedMember = context.Module.ResolveMember(metadataToken, context.DeclaringType?.GetGenericArguments(), context.IsGenericMethod ? context.GetGenericArguments() : null);
			return resolvedMember != null;
		}
		catch
		{
			return false;
		}
	}

	private static bool IsDrawSensitiveMember(MemberInfo? member)
	{
		if (1 == 0)
		{
		}
		bool result = ((member is MethodInfo method) ? IsDrawSensitiveMethod(method) : ((member is ConstructorInfo constructorInfo) ? IsDrawSensitiveType(constructorInfo.DeclaringType) : (member is Type type && IsDrawSensitiveType(type))));
		if (1 == 0)
		{
		}
		return result;
	}

	private static bool IsDrawSensitiveMethod(MethodInfo method)
	{
		if (string.Equals(method.Name, "Draw", StringComparison.Ordinal) && string.Equals(method.DeclaringType?.FullName, "MegaCrit.Sts2.Core.Commands.CardPileCmd", StringComparison.Ordinal))
		{
			return true;
		}
		if (IsDrawSensitiveType(method.DeclaringType) || IsDrawSensitiveType(method.ReturnType))
		{
			return true;
		}
		if (method.IsGenericMethod)
		{
			Type[] genericArguments = method.GetGenericArguments();
			foreach (Type type in genericArguments)
			{
				if (IsDrawSensitiveType(type))
				{
					return true;
				}
			}
		}
		ParameterInfo[] parameters = method.GetParameters();
		foreach (ParameterInfo parameterInfo in parameters)
		{
			if (IsDrawSensitiveType(parameterInfo.ParameterType))
			{
				return true;
			}
		}
		return false;
	}

	private static bool IsDrawSensitiveType(Type? type)
	{
		if (type == null)
		{
			return false;
		}
		if (type.HasElementType)
		{
			return IsDrawSensitiveType(type.GetElementType());
		}
		if (type.IsGenericType)
		{
			Type[] genericArguments = type.GetGenericArguments();
			foreach (Type type2 in genericArguments)
			{
				if (IsDrawSensitiveType(type2))
				{
					return true;
				}
			}
		}
		return string.Equals(type.FullName, "MegaCrit.Sts2.Core.Models.Powers.DrawCardsNextTurnPower", StringComparison.Ordinal);
	}
}
