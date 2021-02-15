﻿////<auto-generated <- Codemaid exclusion for now (PacketIndex Order is important for maintenance)

using OpenNos.Core;
using OpenNos.Domain;

namespace NosTale.Packets.Packets.CommandPackets
{
    [PacketHeader("$ChangeReputation", "$Reputation" , PassNonParseablePacket = true, Authority = AuthorityType.GameMaster)]
    public class ChangeReputationPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public long Reputation { get; set; }

        public static string ReturnHelp() => "$ChangeReputation | $Reputation <Value>";

        #endregion
    }
}