﻿////<auto-generated <- Codemaid exclusion for now (PacketIndex Order is important for maintenance)

using OpenNos.Core;

namespace NosTale.Packets.Packets.ClientPackets
{
    [PacketHeader("rmvobj")]
    public class RmvobjPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public short Slot { get; set; }

        #endregion
    }
}