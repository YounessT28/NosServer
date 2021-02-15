using NosTale.Packets.Packets.ClientPackets;
using OpenNos.Core;
using OpenNos.Domain;
using OpenNos.GameObject;

namespace OpenNos.Handler.Packets.CharScreenPackets
{
    public class CharacterJobCreatePacketHandler : IPacketHandler
    {
        #region Instantiation

        public CharacterJobCreatePacketHandler(ClientSession session) => Session = session;

        #endregion Instantiation

        #region Properties

        private ClientSession Session { get; }

        #endregion Properties

        public void CreateCharacterJob(CharacterJobCreatePacket characterJobCreatePacket) => Session.CreateCharacterAction(characterJobCreatePacket, ClassType.MartialArtist);
    }
}