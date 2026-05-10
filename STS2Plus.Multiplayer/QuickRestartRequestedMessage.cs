using System.Runtime.InteropServices;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Multiplayer.Serialization;
using MegaCrit.Sts2.Core.Multiplayer.Transport;

namespace STS2Plus.Multiplayer;

[StructLayout(LayoutKind.Sequential, Size = 1)]
internal struct QuickRestartRequestedMessage : INetMessage, IPacketSerializable
{
	public bool ShouldBroadcast => false;
	
	public bool ShouldBuffer => false;
	
	public NetTransferMode Mode => (NetTransferMode)2;

	public LogLevel LogLevel => (LogLevel)3;

	public void Serialize(PacketWriter writer)
	{
	}

	public void Deserialize(PacketReader reader)
	{
	}
}
