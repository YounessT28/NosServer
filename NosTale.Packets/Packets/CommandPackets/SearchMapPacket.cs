﻿////<auto-generated <- Codemaid exclusion for now (PacketIndex Order is important for maintenance)

using OpenNos.Core;
using OpenNos.Domain;

namespace NosTale.Packets.Packets.CommandPackets
{
    [PacketHeader("$SearchMap", PassNonParseablePacket = true, Authority = AuthorityType.GameMaster)]
    public class SearchMapPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0, SerializeToEnd = true)]
        public string Contents { get; set; }

        public static string ReturnHelp() => "$SearchMap <Name>";

        #endregion
    }
}