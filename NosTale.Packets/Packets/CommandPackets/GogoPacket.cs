﻿////<auto-generated <- Codemaid exclusion for now (PacketIndex Order is important for maintenance)

using OpenNos.Core;
using OpenNos.Domain;

namespace NosTale.Packets.Packets.CommandPackets
{
    [PacketHeader("$Go" , PassNonParseablePacket = true, Authority = AuthorityType.GameMaster)]
    public class GogoPacket : PacketDefinition
    {
        #region Properties
        
        [PacketIndex(0)]
        public short X { get; set; }

        [PacketIndex(1)]
        public short Y { get; set; }

        public static string ReturnHelp() => "$Go <ToX> <ToY>";

        #endregion
    }
}