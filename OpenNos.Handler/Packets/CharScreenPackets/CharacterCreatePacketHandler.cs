using NosTale.Packets.Packets.ClientPackets;
using OpenNos.Core;
using OpenNos.Domain;
using OpenNos.GameObject;

namespace OpenNos.Handler.Packets.CharScreenPackets
{
    public class CharacterCreatePacketHandler : IPacketHandler
    {
        #region Instantiation

        public CharacterCreatePacketHandler(ClientSession session) => Session = session;

        #endregion Instantiation

        #region Properties

        private ClientSession Session { get; }

        #endregion Properties

        public void CreateCharacter(CharacterCreatePacket characterCreatePacket) => Session.CreateCharacterAction(characterCreatePacket, ClassType.Adventurer);
    }
}