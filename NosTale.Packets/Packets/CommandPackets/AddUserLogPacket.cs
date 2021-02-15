using OpenNos.Core;
using OpenNos.Domain;

namespace NosTale.Packets.Packets.CommandPackets
{
    [PacketHeader("$AddUserLog", PassNonParseablePacket = true, Authority = AuthorityType.GameMaster)]
    public class AddUserLogPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public string Username { get; set; }

        #endregion

        #region Methods

        public static string ReturnHelp() => "$AddUserLog <Username>";

        #endregion
    }
}
