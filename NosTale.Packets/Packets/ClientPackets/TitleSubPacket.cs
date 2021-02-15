using OpenNos.Core;

namespace OpenNos.GameObject.Packets.ServerPackets
{
    public class TitleSubPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]

        public short TitleId { get; set; }

        [PacketIndex(1)]

        public byte TitleStatus { get; set; }

        #endregion
    }
}

