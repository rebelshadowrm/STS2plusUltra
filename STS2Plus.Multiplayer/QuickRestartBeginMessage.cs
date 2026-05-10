using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Multiplayer.Serialization;
using MegaCrit.Sts2.Core.Multiplayer.Transport;

namespace STS2Plus.Multiplayer;

internal struct QuickRestartBeginMessage : INetMessage, IPacketSerializable
{
	public long RestartToken;

	public bool ShouldBroadcast => false;
	
	public bool ShouldBuffer => false;
	
	public NetTransferMode Mode => (NetTransferMode)2;

	public LogLevel LogLevel => (LogLevel)3;

	public void Serialize(PacketWriter writer)
	{
		writer.WriteLong(RestartToken, 64);
	}

	public void Deserialize(PacketReader reader)
	{
		RestartToken = reader.ReadLong(64);
	}
}
