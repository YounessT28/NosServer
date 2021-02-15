using OpenNos.Core;
using OpenNos.Core.Handling;
using OpenNos.DAL;
using OpenNos.Data;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.Master.Library.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text.RegularExpressions;
using OpenNos.GameObject.Networking;
using System.Collections.Concurrent;
using NosTale.Packets.Packets.ClientPackets;
using OpenNos.Core.Interfaces.Packets.ClientPackets;
using OpenNos.GameObject.Helpers;

namespace OpenNos.Handler.Packets.CharScreenPackets
{
    public static class CharCreateExtension
    {
        public static void CreateCharacterAction(this ClientSession Session, ICharacterCreatePacket characterCreatePacket, ClassType classType)
        {
            if (Session.HasCurrentMapInstance)
            {
                return;
            }

            Logger.LogUserEvent("CREATECHARACTER", Session.GenerateIdentity(), $"[CreateCharacter]Name: {characterCreatePacket.Name} Slot: {characterCreatePacket.Slot} Gender: {characterCreatePacket.Gender} HairStyle: {characterCreatePacket.HairStyle} HairColor: {characterCreatePacket.HairColor}");

            if (characterCreatePacket.Slot <= 3
                && DAOFactory.CharacterDAO.LoadBySlot(Session.Account.AccountId, characterCreatePacket.Slot) == null
                && characterCreatePacket.Name != null
                && (characterCreatePacket.Gender == GenderType.Male || characterCreatePacket.Gender == GenderType.Female)
                && (characterCreatePacket.HairStyle == HairStyleType.HairStyleA || (classType != ClassType.MartialArtist && characterCreatePacket.HairStyle == HairStyleType.HairStyleB))
                && Enumerable.Range(0, 10).Contains((byte)characterCreatePacket.HairColor)
                && (characterCreatePacket.Name.Length >= 4 && characterCreatePacket.Name.Length <= 14))
            {
                if (classType == ClassType.MartialArtist)
                {
                    IEnumerable<CharacterDTO> characterDTOs = DAOFactory.CharacterDAO.LoadByAccount(Session.Account.AccountId);

                    if (!characterDTOs.Any(s => s.Level >= 80))
                    {
                        return;
                    }

                    if (characterDTOs.Any(s => s.Class == ClassType.MartialArtist))
                    {
                        Session.SendPacket(UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("MARTIAL_ARTIST_ALREADY_EXISTING")));
                        return;
                    }
                }

                Regex regex = new Regex(@"^[A-Za-z0-9_áéíóúÁÉÍÓÚäëïöüÄËÏÖÜ]+$");

                if (regex.Matches(characterCreatePacket.Name).Count != 1)
                {
                    Session.SendPacket(UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("INVALID_CHARNAME")));
                    return;
                }

                if (DAOFactory.CharacterDAO.LoadByName(characterCreatePacket.Name) != null)
                {
                    Session.SendPacket(UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("CHARNAME_ALREADY_TAKEN")));
                    return;
                }

                CharacterDTO characterDTO = new CharacterDTO
                {
                    AccountId = Session.Account.AccountId,
                    Slot = characterCreatePacket.Slot,
                    Class = classType,
                    Gender = characterCreatePacket.Gender,
                    HairStyle = characterCreatePacket.HairStyle,
                    HairColor = characterCreatePacket.HairColor,
                    Name = characterCreatePacket.Name,
                    MapId = 1,
                    MapX = ServerManager.RandomNumber<short>(78, 81),
                    MapY = ServerManager.RandomNumber<short>(114, 118),
                    MaxMateCount = 10,
                    MaxPartnerCount = 3,
                    SpPoint = 10000,
                    SpAdditionPoint = 0,
                    MinilandMessage = (Language.Instance.GetMessageFromKey("MINILAND_WELCOME_MESSAGE")),
                    State = CharacterState.Active,
                    MinilandPoint = 2000
                };

                switch (characterDTO.Class)
                {
                    case ClassType.MartialArtist:
                        {
                            characterDTO.Level = 81;
                            characterDTO.JobLevel = 50;
                            characterDTO.Hp = 9401;
                            characterDTO.Mp = 3156;
                        }
                        break;

                    default:
                        {
                            characterDTO.Level = 1;
                            characterDTO.JobLevel = 1;
                            characterDTO.Hp = 221;
                            characterDTO.Mp = 69;
                        }
                        break;
                }

                DAOFactory.CharacterDAO.InsertOrUpdate(ref characterDTO);

                if (classType != ClassType.MartialArtist)
                {
                    DAOFactory.CharacterQuestDAO.InsertOrUpdate(new CharacterQuestDTO
                    {
                        CharacterId = characterDTO.CharacterId,
                        QuestId = 1997,
                        IsMainQuest = true
                    });


                    DAOFactory.CharacterSkillDAO.InsertOrUpdate(new CharacterSkillDTO { CharacterId = characterDTO.CharacterId, SkillVNum = 200 });
                    DAOFactory.CharacterSkillDAO.InsertOrUpdate(new CharacterSkillDTO { CharacterId = characterDTO.CharacterId, SkillVNum = 201 });
                    DAOFactory.CharacterSkillDAO.InsertOrUpdate(new CharacterSkillDTO { CharacterId = characterDTO.CharacterId, SkillVNum = 209 });

                    using (Inventory inventory = new Inventory(new Character(characterDTO)))
                    {
                        inventory.AddNewToInventory(1, 1, InventoryType.Wear);
                        inventory.AddNewToInventory(8, 1, InventoryType.Wear);
                        inventory.AddNewToInventory(12, 1, InventoryType.Wear);
                        inventory.AddNewToInventory(2024, 10, InventoryType.Etc);
                        inventory.AddNewToInventory(2081, 1, InventoryType.Etc);
                        inventory.AddNewToInventory(884, 1, InventoryType.Equipment);
                        inventory.AddNewToInventory(885, 1, InventoryType.Equipment);
                        inventory.AddNewToInventory(886, 1, InventoryType.Equipment);
                        inventory.AddNewToInventory(887, 1, InventoryType.Equipment);
                        inventory.AddNewToInventory(9087, 1, InventoryType.Main);
                        inventory.ForEach(i => DAOFactory.ItemInstanceDAO.InsertOrUpdate(i));
                        new EntryPointPacketHandler(Session).LoadCharacters(new OpenNosEntryPointPacket { PacketData = characterCreatePacket.OriginalContent });
                    }
                }
                else
                {
                    DAOFactory.CharacterQuestDAO.InsertOrUpdate(new CharacterQuestDTO
                    {
                        CharacterId = characterDTO.CharacterId,
                        QuestId = 6275,
                        IsMainQuest = false
                    });

                    {
                        DAOFactory.CharacterQuestDAO.InsertOrUpdate(new CharacterQuestDTO
                        {
                            CharacterId = characterDTO.CharacterId,
                            QuestId = 3340,
                            IsMainQuest = true
                        });

                        for (short skillVNum = 1525; skillVNum <= 1539; skillVNum++)
                        {
                            DAOFactory.CharacterSkillDAO.InsertOrUpdate(new CharacterSkillDTO
                            {
                                CharacterId = characterDTO.CharacterId,
                                SkillVNum = skillVNum
                            });
                        }

                        DAOFactory.CharacterSkillDAO.InsertOrUpdate(new CharacterSkillDTO { CharacterId = characterDTO.CharacterId, SkillVNum = 1565 });

                        using (Inventory inventory = new Inventory(new Character(characterDTO)))
                        {
                            inventory.AddNewToInventory(5832, 1, InventoryType.Main, 5);
                            inventory.AddNewToInventory(9319, 1, InventoryType.Etc);
                            inventory.ForEach(i => DAOFactory.ItemInstanceDAO.InsertOrUpdate(i));
                            new EntryPointPacketHandler(Session).LoadCharacters(new OpenNosEntryPointPacket { PacketData = characterCreatePacket.OriginalContent });
                        }
                    }
                }
            }
        }
    }
}
