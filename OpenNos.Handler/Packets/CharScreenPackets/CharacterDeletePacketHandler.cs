using NosTale.Packets.Packets.ClientPackets;
using OpenNos.Core;
using OpenNos.DAL;
using OpenNos.Data;
using OpenNos.GameObject;

namespace OpenNos.Handler.Packets.CharScreenPackets
{
    public class CharacterDeletePacketHandler : IPacketHandler
    {
        #region Instantiation

        public CharacterDeletePacketHandler(ClientSession session) => Session = session;

        #endregion Instantiation

        #region Properties

        private ClientSession Session { get; }

        #endregion Properties

        public void DeleteCharacter(CharacterDeletePacket characterDeletePacket)
        {
            if (Session.HasCurrentMapInstance)
            {
                return;
            }

            if (characterDeletePacket.Password == null)
            {
                return;
            }

            Logger.LogUserEvent("DELETECHARACTER", Session.GenerateIdentity(),
                $"[DeleteCharacter]Name: {characterDeletePacket.Slot}");
            AccountDTO account = DAOFactory.AccountDAO.LoadById(Session.Account.AccountId);
            if (account == null)
            {
                return;
            }

            if (account.Password.ToLower() == CryptographyBase.Sha512(characterDeletePacket.Password))
            {
                CharacterDTO character =
                    DAOFactory.CharacterDAO.LoadBySlot(account.AccountId, characterDeletePacket.Slot);
                if (character == null)
                {
                    return;
                }

                //DAOFactory.GeneralLogDAO.SetCharIdNull(Convert.ToInt64(character.CharacterId));
                DAOFactory.CharacterDAO.DeleteByPrimaryKey(account.AccountId, characterDeletePacket.Slot);
                new EntryPointPacketHandler(Session).LoadCharacters(new OpenNosEntryPointPacket
                {
                    PacketData = string.Empty
                }); ;
            }
            else
            {
                Session.SendPacket($"info {Language.Instance.GetMessageFromKey("BAD_PASSWORD")}");
            }
        }
    }
}