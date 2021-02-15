﻿////<auto-generated <- Codemaid exclusion for now (PacketIndex Order is important for maintenance)

using OpenNos.Core;
using OpenNos.Domain;

namespace NosTale.Packets.Packets.CommandPackets
{
    [PacketHeader("$ChangeShopName" , PassNonParseablePacket = true, Authority = AuthorityType.GameMaster)]
    public class ChangeShopNamePacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public string Name { get; set; }

        public static string ReturnHelp() => "$ChangeShopName <Value>";

        #endregion
    }
}