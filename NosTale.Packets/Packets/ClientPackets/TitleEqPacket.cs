﻿////<auto-generated <- Codemaid exclusion for now (PacketIndex Order is important for maintenance)

using OpenNos.Core;

namespace NosTale.Packets.Packets.ClientPackets
{
    [PacketHeader("tit_eq")]
    public class TitleEqPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public byte Option { get; set; }

        [PacketIndex(1)]
        public short TitleVNum { get; set; }

        #endregion
    }
}