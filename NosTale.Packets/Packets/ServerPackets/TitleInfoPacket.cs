using OpenNos.Core;
using OpenNos.Domain;

namespace OpenNos.GameObject.Packets.ServerPackets
{
    [PacketHeader("titinfo")]
    public class TitleInfoPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]

        public VisualType VisualType { get; set; }

        [PacketIndex(1)]

        public long VisualId { get; set; }

        [PacketIndex(2)]

        public short VisibleTitle { get; set; }

        [PacketIndex(3)]

        public short EffectiveTitle { get; set; }

        #endregion
    }
}