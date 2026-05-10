using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Multiplayer.Serialization;
using MegaCrit.Sts2.Core.Multiplayer.Transport;

namespace STS2Plus.Multiplayer;

internal struct RuleSelectionSyncMessage : INetMessage, IPacketSerializable
{
	public int SelectionMask;

	public bool ShouldBroadcast => false;
	
	public bool ShouldBuffer => false;
	
	public NetTransferMode Mode => (NetTransferMode)2;

	public LogLevel LogLevel => (LogLevel)3;

	public void Serialize(PacketWriter writer)
	{
		writer.WriteInt(SelectionMask, 32);
	}

	public void Deserialize(PacketReader reader)
	{
		SelectionMask = reader.ReadInt(32);
	}
}
