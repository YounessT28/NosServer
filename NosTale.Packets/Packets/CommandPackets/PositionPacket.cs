﻿////<auto-generated <- Codemaid exclusion for now (PacketIndex Order is important for maintenance)

using OpenNos.Core;
using OpenNos.Domain;

namespace NosTale.Packets.Packets.CommandPackets
{
    [PacketHeader("$Position", PassNonParseablePacket = true, Authority = AuthorityType.GameMaster)]
    public class PositionPacket : PacketDefinition
    {
        public static string ReturnHelp() => "$Position";
    }
}