using OpenNos.Core;
using OpenNos.Domain;

namespace NosTale.Packets.Packets.CommandPackets
{
    [PacketHeader("$Test", PassNonParseablePacket = true, Authority = AuthorityType.GameMaster)]
    public class TestCommandPacket : PacketDefinition
    {
        #region
        #endregion
    }
}