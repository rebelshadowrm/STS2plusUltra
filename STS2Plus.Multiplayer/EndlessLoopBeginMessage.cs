using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Multiplayer.Serialization;
using MegaCrit.Sts2.Core.Multiplayer.Transport;

namespace STS2Plus.Multiplayer;

internal struct EndlessLoopBeginMessage : INetMessage, IPacketSerializable
{
	public long LaunchToken;

	public int LoopIndex;

	public bool ShouldBroadcast => false;

	public bool ShouldBuffer => false;

	public NetTransferMode Mode => (NetTransferMode)2;

	public LogLevel LogLevel => (LogLevel)3;

	public void Serialize(PacketWriter writer)
	{
		writer.WriteLong(LaunchToken, 64);
		writer.WriteInt(LoopIndex, 32);
	}

	public void Deserialize(PacketReader reader)
	{
		LaunchToken = reader.ReadLong(64);
		LoopIndex = reader.ReadInt(32);
	}
}
