﻿////<auto-generated <- Codemaid exclusion for now (PacketIndex Order is important for maintenance)

using OpenNos.Core;
using OpenNos.Domain;

namespace NosTale.Packets.Packets.CommandPackets
{
    [PacketHeader("$Act6Stat", PassNonParseablePacket = true, Authority = AuthorityType.GameMaster)]
    public class Act6StatPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public byte Faction { get; set; }

        [PacketIndex(1)]
        public int Value { get; set; }

        public static string ReturnHelp()
        {
            return "$Act6Stat FACTION VALUE";
        }

        #endregion
    }
}