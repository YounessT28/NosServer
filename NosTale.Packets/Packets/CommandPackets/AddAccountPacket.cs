using OpenNos.Core;
using OpenNos.Domain;

namespace NosTale.Packets.Packets.CommandPackets
{
    [PacketHeader("$AddAccount", PassNonParseablePacket = true, Authority = AuthorityType.GameMaster)]
    public class AddAccountPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public string Name { get; set; }

        [PacketIndex(1)]
        public string Password { get; set; }

        [PacketIndex(2)]
        public int Authority { get; set; }

        public static string ReturnHelp() => "$AddAccount <Name> <Password> <Authority>";

        #endregion
    }
}