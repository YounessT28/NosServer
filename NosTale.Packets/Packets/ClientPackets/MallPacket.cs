﻿////<auto-generated <- Codemaid exclusion for now (PacketIndex Order is important for maintenance)

using OpenNos.Core;

namespace NosTale.Packets.Packets.ClientPackets
{
    [PacketHeader("mall")]
    public class MallPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public int Type { get; set; }

        #endregion
    }
}