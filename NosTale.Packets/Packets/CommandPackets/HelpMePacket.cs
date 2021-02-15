﻿////<auto-generated <- Codemaid exclusion for now (PacketIndex Order is important for maintenance)

using OpenNos.Core;
using OpenNos.Domain;

namespace NosTale.Packets.Packets.CommandPackets
{
    [PacketHeader("$HelpMe", PassNonParseablePacket = true, Authority = AuthorityType.GameMaster)]
    public class HelpMePacket : PacketDefinition
    {

        [PacketIndex(0, SerializeToEnd = true)]
        public string Message { get; set; }

        #region Methods

        public static string ReturnHelp() => "$HelpMe <Message>";

        #endregion
    }
}