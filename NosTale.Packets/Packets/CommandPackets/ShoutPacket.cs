﻿////<auto-generated <- Codemaid exclusion for now (PacketIndex Order is important for maintenance)

using OpenNos.Core;
using OpenNos.Domain;

namespace NosTale.Packets.Packets.CommandPackets
{
    [PacketHeader("$Shout", PassNonParseablePacket = true, Authority = AuthorityType.GameMaster)]
    public class ShoutPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0, SerializeToEnd = true)]
        public string Message { get; set; }

        public static string ReturnHelp() => "$Shout <Message>";

        #endregion
    }
}