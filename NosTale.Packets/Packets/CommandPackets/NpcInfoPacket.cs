﻿////<auto-generated <- Codemaid exclusion for now (PacketIndex Order is important for maintenance)

using OpenNos.Core;
using OpenNos.Domain;

namespace NosTale.Packets.Packets.CommandPackets
{
    [PacketHeader("$NpcInfo", PassNonParseablePacket = true, Authority = AuthorityType.GameMaster)]
    public class NpcInfoPacket : PacketDefinition
    {
        public static string ReturnHelp() => "$NpcInfo";
    }
}