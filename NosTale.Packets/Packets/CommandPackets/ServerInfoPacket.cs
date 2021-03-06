﻿////<auto-generated <- Codemaid exclusion for now (PacketIndex Order is important for maintenance)

using OpenNos.Core;
using OpenNos.Domain;

namespace NosTale.Packets.Packets.CommandPackets
{
    [PacketHeader("$ServerInfo", PassNonParseablePacket = true, Authority = AuthorityType.GameMaster)]
    public class ServerInfoPacket : PacketDefinition
    {
        [PacketIndex(0)]
        public int? ChannelId { get; set; }

        public static string ReturnHelp() => "$ServerInfo <?ChannelId>";
    }
}