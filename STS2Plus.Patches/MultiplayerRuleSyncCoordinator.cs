using MegaCrit.Sts2.Core.Multiplayer.Game;
using MegaCrit.Sts2.Core.Runs;
using STS2Plus.Config;
using STS2Plus.Multiplayer;
using STS2Plus.Reflection;

namespace STS2Plus.Patches;

internal static class MultiplayerRuleSyncCoordinator
{
	private static INetGameService? attachedService;

	private static int? lastSentMask;

	private static int? lastReceivedMask;

	public static void AttachCurrentRun()
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Invalid comparison between Unknown and I4
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Invalid comparison between Unknown and I4
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		ModEntry.Verbose("RuleSync: attaching to current run");
		if (!ConfigManager.Current.MoreRulesEnabled)
		{
			Detach();
			return;
		}
		RunManager instance = RunManager.Instance;
		INetGameService val = ((instance != null) ? instance.NetService : null);
		if (val == null || ((int)val.Type != 2 && (int)val.Type != 3))
		{
			Detach();
			return;
		}
		if (attachedService == val)
		{
			BroadcastCurrentSelectionIfHost();
			return;
		}
		Detach();
		attachedService = val;
		attachedService.RegisterMessageHandler<RuleSelectionSyncMessage>((MessageHandlerDelegate<RuleSelectionSyncMessage>)OnRuleSelectionSync);
		ModEntry.Logger.Info($"STS2Plus.Net rule-sync attach type={val.Type} connected={val.IsConnected} netId={val.NetId}", 1);
		BroadcastCurrentSelectionIfHost();
	}

	public static void Detach()
	{
		if (attachedService != null)
		{
			attachedService.UnregisterMessageHandler<RuleSelectionSyncMessage>((MessageHandlerDelegate<RuleSelectionSyncMessage>)OnRuleSelectionSync);
		}
		attachedService = null;
		lastSentMask = null;
		lastReceivedMask = null;
	}

	public static void BroadcastCurrentSelectionIfHost()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Invalid comparison between Unknown and I4
		if (!ConfigManager.Current.MoreRulesEnabled)
		{
			return;
		}
		INetGameService val = attachedService;
		if (val != null && (int)val.Type == 2 && val.IsConnected)
		{
			int ruleSelectionMask = PlusState.GetRuleSelectionMask();
			if (lastSentMask != ruleSelectionMask)
			{
				lastSentMask = ruleSelectionMask;
				val.SendMessage<RuleSelectionSyncMessage>(new RuleSelectionSyncMessage
				{
					SelectionMask = ruleSelectionMask
				});
				ModEntry.Logger.Info($"STS2Plus.Net rule-sync broadcast mask={ruleSelectionMask}", 1);
			}
		}
	}

	private static void OnRuleSelectionSync(RuleSelectionSyncMessage message, ulong senderId)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Invalid comparison between Unknown and I4
		if (!ConfigManager.Current.MoreRulesEnabled)
		{
			return;
		}
		INetGameService val = attachedService;
		if (val == null || ((int)val.Type == 2 && senderId == val.NetId) || lastReceivedMask == message.SelectionMask)
		{
			return;
		}
		lastReceivedMask = message.SelectionMask;
		PlusState.ApplyRuleSelectionMask(message.SelectionMask);
		DeprecatedModifierTopBarPatch.RefreshVisibleTopBarModifiers();
		if (PlusState.IsGlassCannonActive())
		{
			foreach (object player in GameReflection.GetPlayers())
			{
				GameReflection.RepairGlassCannonState(player);
			}
		}
		ModEntry.Logger.Info($"STS2Plus.Net rule-sync received mask={message.SelectionMask} from={senderId}", 1);
	}
}
