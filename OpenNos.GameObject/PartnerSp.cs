using OpenNos.Core;
using OpenNos.DAL;
using OpenNos.Data;
using OpenNos.Data.Enums;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenNos.GameObject
{
    public class PartnerSp
    {
        #region Istantiation

        public PartnerSp(ItemInstance instance)
        {
            Instance = instance;

            Initialize();
        }

        #endregion

        #region Properties

        public ItemInstance Instance { get; }

        private long XpMax => ServerManager.Instance.Configuration.PartnerSpXp;

        private List<PartnerSkill> Skills { get; set; }

        #endregion

        #region Methods

        public bool AddSkill(byte castId)
        {
            Skill skill = GetNewSkill(castId);

            if (Instance.Item.Morph == 2043 && castId == 0)
            {
                PartnerSkillDTO partnerSkillDTO = new PartnerSkillDTO
                {
                    EquipmentSerialId = Instance.EquipmentSerialId,
                    SkillVNum = 1235,
                    Level = ServerManager.RandomNumber<byte>(1, 8)
                };

                if (DAOFactory.PartnerSkillDAO.Insert(partnerSkillDTO) is PartnerSkillDTO result)
                {
                    Skills.Add(new PartnerSkill(result));

                    return true;
                }
            }
            if (Instance.Item.Morph == 2043 && castId == 1)
            {
                PartnerSkillDTO partnerSkillDTO = new PartnerSkillDTO
                {
                    EquipmentSerialId = Instance.EquipmentSerialId,
                    SkillVNum = 1237,
                    Level = ServerManager.RandomNumber<byte>(1, 8)
                };

                if (DAOFactory.PartnerSkillDAO.Insert(partnerSkillDTO) is PartnerSkillDTO result)
                {
                    Skills.Add(new PartnerSkill(result));

                    return true;
                }
            }
            if (Instance.Item.Morph == 2043 && castId == 2)
            {
                PartnerSkillDTO partnerSkillDTO = new PartnerSkillDTO
                {
                    EquipmentSerialId = Instance.EquipmentSerialId,
                    SkillVNum = 1239,
                    Level = ServerManager.RandomNumber<byte>(1, 8)
                };

                if (DAOFactory.PartnerSkillDAO.Insert(partnerSkillDTO) is PartnerSkillDTO result)
                {
                    Skills.Add(new PartnerSkill(result));

                    return true;
                }
            }
            if (Instance.Item.Morph == 2046 && castId == 0)
            {
                PartnerSkillDTO partnerSkillDTO = new PartnerSkillDTO
                {
                    EquipmentSerialId = Instance.EquipmentSerialId,
                    SkillVNum = 1268,
                    Level = ServerManager.RandomNumber<byte>(1, 8)
                };

                if (DAOFactory.PartnerSkillDAO.Insert(partnerSkillDTO) is PartnerSkillDTO result)
                {
                    Skills.Add(new PartnerSkill(result));

                    return true;
                }
            }
            if (Instance.Item.Morph == 2046 && castId == 1)
            {
                PartnerSkillDTO partnerSkillDTO = new PartnerSkillDTO
                {
                    EquipmentSerialId = Instance.EquipmentSerialId,
                    SkillVNum = 1269,
                    Level = ServerManager.RandomNumber<byte>(1, 8)
                };

                if (DAOFactory.PartnerSkillDAO.Insert(partnerSkillDTO) is PartnerSkillDTO result)
                {
                    Skills.Add(new PartnerSkill(result));

                    return true;
                }
            }
            if (Instance.Item.Morph == 2046 && castId == 2)
            {
                PartnerSkillDTO partnerSkillDTO = new PartnerSkillDTO
                {
                    EquipmentSerialId = Instance.EquipmentSerialId,
                    SkillVNum = 1273,
                    Level = ServerManager.RandomNumber<byte>(1, 8)
                };

                if (DAOFactory.PartnerSkillDAO.Insert(partnerSkillDTO) is PartnerSkillDTO result)
                {
                    Skills.Add(new PartnerSkill(result));

                    return true;
                }
            }
            if (Instance.Item.Morph == 2310 && castId == 0)
            {
                PartnerSkillDTO partnerSkillDTO = new PartnerSkillDTO
                {
                    EquipmentSerialId = Instance.EquipmentSerialId,
                    SkillVNum = 1292,
                    Level = ServerManager.RandomNumber<byte>(1, 8)
                };

                if (DAOFactory.PartnerSkillDAO.Insert(partnerSkillDTO) is PartnerSkillDTO result)
                {
                    Skills.Add(new PartnerSkill(result));

                    return true;
                }
            }
            if (Instance.Item.Morph == 2310 && castId == 1)
            {
                PartnerSkillDTO partnerSkillDTO = new PartnerSkillDTO
                {
                    EquipmentSerialId = Instance.EquipmentSerialId,
                    SkillVNum = 1293,
                    Level = ServerManager.RandomNumber<byte>(1, 8)
                };

                if (DAOFactory.PartnerSkillDAO.Insert(partnerSkillDTO) is PartnerSkillDTO result)
                {
                    Skills.Add(new PartnerSkill(result));

                    return true;
                }
            }
            if (Instance.Item.Morph == 2310 && castId == 2)
            {
                PartnerSkillDTO partnerSkillDTO = new PartnerSkillDTO
                {
                    EquipmentSerialId = Instance.EquipmentSerialId,
                    SkillVNum = 1294,
                    Level = ServerManager.RandomNumber<byte>(1, 8)
                };

                if (DAOFactory.PartnerSkillDAO.Insert(partnerSkillDTO) is PartnerSkillDTO result)
                {
                    Skills.Add(new PartnerSkill(result));

                    return true;
                }
            }
            if (Instance.Item.Morph == 2371 && castId == 0)
            {
                PartnerSkillDTO partnerSkillDTO = new PartnerSkillDTO
                {
                    EquipmentSerialId = Instance.EquipmentSerialId,
                    SkillVNum = 1292,
                    Level = ServerManager.RandomNumber<byte>(1, 8)
                };

                if (DAOFactory.PartnerSkillDAO.Insert(partnerSkillDTO) is PartnerSkillDTO result)
                {
                    Skills.Add(new PartnerSkill(result));

                    return true;
                }
            }
            if (Instance.Item.Morph == 2371 && castId == 1)
            {
                PartnerSkillDTO partnerSkillDTO = new PartnerSkillDTO
                {
                    EquipmentSerialId = Instance.EquipmentSerialId,
                    SkillVNum = 1293,
                    Level = ServerManager.RandomNumber<byte>(1, 8)
                };

                if (DAOFactory.PartnerSkillDAO.Insert(partnerSkillDTO) is PartnerSkillDTO result)
                {
                    Skills.Add(new PartnerSkill(result));

                    return true;
                }
            }
            if (Instance.Item.Morph == 2371 && castId == 2)
            {
                PartnerSkillDTO partnerSkillDTO = new PartnerSkillDTO
                {
                    EquipmentSerialId = Instance.EquipmentSerialId,
                    SkillVNum = 1294,
                    Level = ServerManager.RandomNumber<byte>(1, 8)
                };

                if (DAOFactory.PartnerSkillDAO.Insert(partnerSkillDTO) is PartnerSkillDTO result)
                {
                    Skills.Add(new PartnerSkill(result));

                    return true;
                }
            }
            if (Instance.Item.Morph == 2317 && castId == 0)
            {
                PartnerSkillDTO partnerSkillDTO = new PartnerSkillDTO
                {
                    EquipmentSerialId = Instance.EquipmentSerialId,
                    SkillVNum = 1299,
                    Level = ServerManager.RandomNumber<byte>(1, 8)
                };

                if (DAOFactory.PartnerSkillDAO.Insert(partnerSkillDTO) is PartnerSkillDTO result)
                {
                    Skills.Add(new PartnerSkill(result));

                    return true;
                }
            }
            if (Instance.Item.Morph == 2317 && castId == 1)
            {
                PartnerSkillDTO partnerSkillDTO = new PartnerSkillDTO
                {
                    EquipmentSerialId = Instance.EquipmentSerialId,
                    SkillVNum = 1300,
                    Level = ServerManager.RandomNumber<byte>(1, 8)
                };

                if (DAOFactory.PartnerSkillDAO.Insert(partnerSkillDTO) is PartnerSkillDTO result)
                {
                    Skills.Add(new PartnerSkill(result));

                    return true;
                }
            }
            if (Instance.Item.Morph == 2317 && castId == 2)
            {
                PartnerSkillDTO partnerSkillDTO = new PartnerSkillDTO
                {
                    EquipmentSerialId = Instance.EquipmentSerialId,
                    SkillVNum = 1301,
                    Level = ServerManager.RandomNumber<byte>(1, 8)
                };

                if (DAOFactory.PartnerSkillDAO.Insert(partnerSkillDTO) is PartnerSkillDTO result)
                {
                    Skills.Add(new PartnerSkill(result));

                    return true;
                }
            }
            if (Instance.Item.Morph == 2323 && castId == 0)
            {
                PartnerSkillDTO partnerSkillDTO = new PartnerSkillDTO
                {
                    EquipmentSerialId = Instance.EquipmentSerialId,
                    SkillVNum = 1299,
                    Level = ServerManager.RandomNumber<byte>(1, 8)
                };

                if (DAOFactory.PartnerSkillDAO.Insert(partnerSkillDTO) is PartnerSkillDTO result)
                {
                    Skills.Add(new PartnerSkill(result));

                    return true;
                }
            }
            if (Instance.Item.Morph == 2323 && castId == 1)
            {
                PartnerSkillDTO partnerSkillDTO = new PartnerSkillDTO
                {
                    EquipmentSerialId = Instance.EquipmentSerialId,
                    SkillVNum = 1300,
                    Level = ServerManager.RandomNumber<byte>(1, 8)
                };

                if (DAOFactory.PartnerSkillDAO.Insert(partnerSkillDTO) is PartnerSkillDTO result)
                {
                    Skills.Add(new PartnerSkill(result));

                    return true;
                }
            }
            if (Instance.Item.Morph == 2323 && castId == 2)
            {
                PartnerSkillDTO partnerSkillDTO = new PartnerSkillDTO
                {
                    EquipmentSerialId = Instance.EquipmentSerialId,
                    SkillVNum = 1301,
                    Level = ServerManager.RandomNumber<byte>(1, 8)
                };

                if (DAOFactory.PartnerSkillDAO.Insert(partnerSkillDTO) is PartnerSkillDTO result)
                {
                    Skills.Add(new PartnerSkill(result));

                    return true;
                }
            }
            if (Instance.Item.Morph == 2355 && castId == 0)
            {
                PartnerSkillDTO partnerSkillDTO = new PartnerSkillDTO
                {
                    EquipmentSerialId = Instance.EquipmentSerialId,
                    SkillVNum = 1358,
                    Level = ServerManager.RandomNumber<byte>(1, 8)
                };

                if (DAOFactory.PartnerSkillDAO.Insert(partnerSkillDTO) is PartnerSkillDTO result)
                {
                    Skills.Add(new PartnerSkill(result));

                    return true;
                }
            }
            if (Instance.Item.Morph == 2355 && castId == 1)
            {
                PartnerSkillDTO partnerSkillDTO = new PartnerSkillDTO
                {
                    EquipmentSerialId = Instance.EquipmentSerialId,
                    SkillVNum = 1359,
                    Level = ServerManager.RandomNumber<byte>(1, 8)
                };

                if (DAOFactory.PartnerSkillDAO.Insert(partnerSkillDTO) is PartnerSkillDTO result)
                {
                    Skills.Add(new PartnerSkill(result));

                    return true;
                }
            }
            if (Instance.Item.Morph == 2355 && castId == 2)
            {
                PartnerSkillDTO partnerSkillDTO = new PartnerSkillDTO
                {
                    EquipmentSerialId = Instance.EquipmentSerialId,
                    SkillVNum = 1360,
                    Level = ServerManager.RandomNumber<byte>(1, 8)
                };

                if (DAOFactory.PartnerSkillDAO.Insert(partnerSkillDTO) is PartnerSkillDTO result)
                {
                    Skills.Add(new PartnerSkill(result));

                    return true;
                }
            }
            if (Instance.Item.Morph == 2709 && castId == 0)
            {
                PartnerSkillDTO partnerSkillDTO = new PartnerSkillDTO
                {
                    EquipmentSerialId = Instance.EquipmentSerialId,
                    SkillVNum = 1358,
                    Level = ServerManager.RandomNumber<byte>(1, 8)
                };

                if (DAOFactory.PartnerSkillDAO.Insert(partnerSkillDTO) is PartnerSkillDTO result)
                {
                    Skills.Add(new PartnerSkill(result));

                    return true;
                }
            }
            if (Instance.Item.Morph == 2709 && castId == 1)
            {
                PartnerSkillDTO partnerSkillDTO = new PartnerSkillDTO
                {
                    EquipmentSerialId = Instance.EquipmentSerialId,
                    SkillVNum = 1359,
                    Level = ServerManager.RandomNumber<byte>(1, 8)
                };

                if (DAOFactory.PartnerSkillDAO.Insert(partnerSkillDTO) is PartnerSkillDTO result)
                {
                    Skills.Add(new PartnerSkill(result));

                    return true;
                }
            }
            if (Instance.Item.Morph == 2709 && castId == 2)
            {
                PartnerSkillDTO partnerSkillDTO = new PartnerSkillDTO
                {
                    EquipmentSerialId = Instance.EquipmentSerialId,
                    SkillVNum = 1360,
                    Level = ServerManager.RandomNumber<byte>(1, 8)
                };

                if (DAOFactory.PartnerSkillDAO.Insert(partnerSkillDTO) is PartnerSkillDTO result)
                {
                    Skills.Add(new PartnerSkill(result));

                    return true;
                }
            }
            if (Instance.Item.Morph == 2356 && castId == 0)
            {
                PartnerSkillDTO partnerSkillDTO = new PartnerSkillDTO
                {
                    EquipmentSerialId = Instance.EquipmentSerialId,
                    SkillVNum = 1368,
                    Level = ServerManager.RandomNumber<byte>(1, 8)
                };

                if (DAOFactory.PartnerSkillDAO.Insert(partnerSkillDTO) is PartnerSkillDTO result)
                {
                    Skills.Add(new PartnerSkill(result));

                    return true;
                }
            }
            if (Instance.Item.Morph == 2356 && castId == 1)
            {
                PartnerSkillDTO partnerSkillDTO = new PartnerSkillDTO
                {
                    EquipmentSerialId = Instance.EquipmentSerialId,
                    SkillVNum = 1369,
                    Level = ServerManager.RandomNumber<byte>(1, 8)
                };

                if (DAOFactory.PartnerSkillDAO.Insert(partnerSkillDTO) is PartnerSkillDTO result)
                {
                    Skills.Add(new PartnerSkill(result));

                    return true;
                }
            }
            if (Instance.Item.Morph == 2356 && castId == 2)
            {
                PartnerSkillDTO partnerSkillDTO = new PartnerSkillDTO
                {
                    EquipmentSerialId = Instance.EquipmentSerialId,
                    SkillVNum = 1370,
                    Level = ServerManager.RandomNumber<byte>(1, 8)
                };

                if (DAOFactory.PartnerSkillDAO.Insert(partnerSkillDTO) is PartnerSkillDTO result)
                {
                    Skills.Add(new PartnerSkill(result));

                    return true;
                }
            }
            if (Instance.Item.Morph == 2374 && castId == 0)
            {
                PartnerSkillDTO partnerSkillDTO = new PartnerSkillDTO
                {
                    EquipmentSerialId = Instance.EquipmentSerialId,
                    SkillVNum = 1439,
                    Level = ServerManager.RandomNumber<byte>(1, 8)
                };

                if (DAOFactory.PartnerSkillDAO.Insert(partnerSkillDTO) is PartnerSkillDTO result)
                {
                    Skills.Add(new PartnerSkill(result));

                    return true;
                }
            }
            if (Instance.Item.Morph == 2374 && castId == 1)
            {
                PartnerSkillDTO partnerSkillDTO = new PartnerSkillDTO
                {
                    EquipmentSerialId = Instance.EquipmentSerialId,
                    SkillVNum = 1440,
                    Level = ServerManager.RandomNumber<byte>(1, 8)
                };

                if (DAOFactory.PartnerSkillDAO.Insert(partnerSkillDTO) is PartnerSkillDTO result)
                {
                    Skills.Add(new PartnerSkill(result));

                    return true;
                }
            }
            if (Instance.Item.Morph == 2374 && castId == 2)
            {
                PartnerSkillDTO partnerSkillDTO = new PartnerSkillDTO
                {
                    EquipmentSerialId = Instance.EquipmentSerialId,
                    SkillVNum = 1441,
                    Level = ServerManager.RandomNumber<byte>(1, 8)
                };

                if (DAOFactory.PartnerSkillDAO.Insert(partnerSkillDTO) is PartnerSkillDTO result)
                {
                    Skills.Add(new PartnerSkill(result));

                    return true;
                }
            }
            if (Instance.Item.Morph == 2378 && castId == 0)
            {
                PartnerSkillDTO partnerSkillDTO = new PartnerSkillDTO
                {
                    EquipmentSerialId = Instance.EquipmentSerialId,
                    SkillVNum = 1449,
                    Level = ServerManager.RandomNumber<byte>(1, 8)
                };

                if (DAOFactory.PartnerSkillDAO.Insert(partnerSkillDTO) is PartnerSkillDTO result)
                {
                    Skills.Add(new PartnerSkill(result));

                    return true;
                }
            }
            if (Instance.Item.Morph == 2378 && castId == 1)
            {
                PartnerSkillDTO partnerSkillDTO = new PartnerSkillDTO
                {
                    EquipmentSerialId = Instance.EquipmentSerialId,
                    SkillVNum = 1450,
                    Level = ServerManager.RandomNumber<byte>(1, 8)
                };

                if (DAOFactory.PartnerSkillDAO.Insert(partnerSkillDTO) is PartnerSkillDTO result)
                {
                    Skills.Add(new PartnerSkill(result));

                    return true;
                }
            }
            if (Instance.Item.Morph == 2378 && castId == 2)
            {
                PartnerSkillDTO partnerSkillDTO = new PartnerSkillDTO
                {
                    EquipmentSerialId = Instance.EquipmentSerialId,
                    SkillVNum = 1451,
                    Level = ServerManager.RandomNumber<byte>(1, 8)
                };

                if (DAOFactory.PartnerSkillDAO.Insert(partnerSkillDTO) is PartnerSkillDTO result)
                {
                    Skills.Add(new PartnerSkill(result));

                    return true;
                }
            }
            if (Instance.Item.Morph == 2537 && castId == 0)
            {
                PartnerSkillDTO partnerSkillDTO = new PartnerSkillDTO
                {
                    EquipmentSerialId = Instance.EquipmentSerialId,
                    SkillVNum = 1490,
                    Level = ServerManager.RandomNumber<byte>(1, 8)
                };

                if (DAOFactory.PartnerSkillDAO.Insert(partnerSkillDTO) is PartnerSkillDTO result)
                {
                    Skills.Add(new PartnerSkill(result));

                    return true;
                }
            }
            if (Instance.Item.Morph == 2537 && castId == 1)
            {
                PartnerSkillDTO partnerSkillDTO = new PartnerSkillDTO
                {
                    EquipmentSerialId = Instance.EquipmentSerialId,
                    SkillVNum = 1491,
                    Level = ServerManager.RandomNumber<byte>(1, 8)
                };

                if (DAOFactory.PartnerSkillDAO.Insert(partnerSkillDTO) is PartnerSkillDTO result)
                {
                    Skills.Add(new PartnerSkill(result));

                    return true;
                }
            }
            if (Instance.Item.Morph == 2537 && castId == 2)
            {
                PartnerSkillDTO partnerSkillDTO = new PartnerSkillDTO
                {
                    EquipmentSerialId = Instance.EquipmentSerialId,
                    SkillVNum = 1492,
                    Level = ServerManager.RandomNumber<byte>(1, 8)
                };

                if (DAOFactory.PartnerSkillDAO.Insert(partnerSkillDTO) is PartnerSkillDTO result)
                {
                    Skills.Add(new PartnerSkill(result));

                    return true;
                }
            }
            if (Instance.Item.Morph == 2539 && castId == 0)
            {
                PartnerSkillDTO partnerSkillDTO = new PartnerSkillDTO
                {
                    EquipmentSerialId = Instance.EquipmentSerialId,
                    SkillVNum = 1521,
                    Level = ServerManager.RandomNumber<byte>(1, 8)
                };

                if (DAOFactory.PartnerSkillDAO.Insert(partnerSkillDTO) is PartnerSkillDTO result)
                {
                    Skills.Add(new PartnerSkill(result));

                    return true;
                }
            }
            if (Instance.Item.Morph == 2539 && castId == 1)
            {
                PartnerSkillDTO partnerSkillDTO = new PartnerSkillDTO
                {
                    EquipmentSerialId = Instance.EquipmentSerialId,
                    SkillVNum = 1522,
                    Level = ServerManager.RandomNumber<byte>(1, 8)
                };

                if (DAOFactory.PartnerSkillDAO.Insert(partnerSkillDTO) is PartnerSkillDTO result)
                {
                    Skills.Add(new PartnerSkill(result));

                    return true;
                }
            }
            if (Instance.Item.Morph == 2539 && castId == 2)
            {
                PartnerSkillDTO partnerSkillDTO = new PartnerSkillDTO
                {
                    EquipmentSerialId = Instance.EquipmentSerialId,
                    SkillVNum = 1523,
                    Level = ServerManager.RandomNumber<byte>(1, 8)
                };

                if (DAOFactory.PartnerSkillDAO.Insert(partnerSkillDTO) is PartnerSkillDTO result)
                {
                    Skills.Add(new PartnerSkill(result));

                    return true;
                }
            }
            if (Instance.Item.Morph == 2731 && castId == 0)
            {
                PartnerSkillDTO partnerSkillDTO = new PartnerSkillDTO
                {
                    EquipmentSerialId = Instance.EquipmentSerialId,
                    SkillVNum = 660,
                    Level = ServerManager.RandomNumber<byte>(1, 8)
                };

                if (DAOFactory.PartnerSkillDAO.Insert(partnerSkillDTO) is PartnerSkillDTO result)
                {
                    Skills.Add(new PartnerSkill(result));

                    return true;
                }
            }
            if (Instance.Item.Morph == 2731 && castId == 1)
            {
                PartnerSkillDTO partnerSkillDTO = new PartnerSkillDTO
                {
                    EquipmentSerialId = Instance.EquipmentSerialId,
                    SkillVNum = 661,
                    Level = ServerManager.RandomNumber<byte>(1, 8)
                };

                if (DAOFactory.PartnerSkillDAO.Insert(partnerSkillDTO) is PartnerSkillDTO result)
                {
                    Skills.Add(new PartnerSkill(result));

                    return true;
                }
            }
            if (Instance.Item.Morph == 2731 && castId == 2)
            {
                PartnerSkillDTO partnerSkillDTO = new PartnerSkillDTO
                {
                    EquipmentSerialId = Instance.EquipmentSerialId,
                    SkillVNum = 662,
                    Level = ServerManager.RandomNumber<byte>(1, 8)
                };

                if (DAOFactory.PartnerSkillDAO.Insert(partnerSkillDTO) is PartnerSkillDTO result)
                {
                    Skills.Add(new PartnerSkill(result));

                    return true;
                }
            }
            if (Instance.Item.Morph == 2044 && castId == 0)
            {
                PartnerSkillDTO partnerSkillDTO = new PartnerSkillDTO
                {
                    EquipmentSerialId = Instance.EquipmentSerialId,
                    SkillVNum = 1236,
                    Level = ServerManager.RandomNumber<byte>(1, 8)
                };

                if (DAOFactory.PartnerSkillDAO.Insert(partnerSkillDTO) is PartnerSkillDTO result)
                {
                    Skills.Add(new PartnerSkill(result));

                    return true;
                }
            }
            if (Instance.Item.Morph == 2044 && castId == 1)
            {
                PartnerSkillDTO partnerSkillDTO = new PartnerSkillDTO
                {
                    EquipmentSerialId = Instance.EquipmentSerialId,
                    SkillVNum = 1238,
                    Level = ServerManager.RandomNumber<byte>(1, 8)
                };

                if (DAOFactory.PartnerSkillDAO.Insert(partnerSkillDTO) is PartnerSkillDTO result)
                {
                    Skills.Add(new PartnerSkill(result));

                    return true;
                }
            }
            if (Instance.Item.Morph == 2044 && castId == 2)
            {
                PartnerSkillDTO partnerSkillDTO = new PartnerSkillDTO
                {
                    EquipmentSerialId = Instance.EquipmentSerialId,
                    SkillVNum = 1240,
                    Level = ServerManager.RandomNumber<byte>(1, 8)
                };

                if (DAOFactory.PartnerSkillDAO.Insert(partnerSkillDTO) is PartnerSkillDTO result)
                {
                    Skills.Add(new PartnerSkill(result));

                    return true;
                }
            }
            if (Instance.Item.Morph == 2047 && castId == 0)
            {
                PartnerSkillDTO partnerSkillDTO = new PartnerSkillDTO
                {
                    EquipmentSerialId = Instance.EquipmentSerialId,
                    SkillVNum = 1270,
                    Level = ServerManager.RandomNumber<byte>(1, 8)
                };

                if (DAOFactory.PartnerSkillDAO.Insert(partnerSkillDTO) is PartnerSkillDTO result)
                {
                    Skills.Add(new PartnerSkill(result));

                    return true;
                }
            }
            if (Instance.Item.Morph == 2047 && castId == 1)
            {
                PartnerSkillDTO partnerSkillDTO = new PartnerSkillDTO
                {
                    EquipmentSerialId = Instance.EquipmentSerialId,
                    SkillVNum = 1271,
                    Level = ServerManager.RandomNumber<byte>(1, 8)
                };

                if (DAOFactory.PartnerSkillDAO.Insert(partnerSkillDTO) is PartnerSkillDTO result)
                {
                    Skills.Add(new PartnerSkill(result));

                    return true;
                }
            }
            if (Instance.Item.Morph == 2047 && castId == 2)
            {
                PartnerSkillDTO partnerSkillDTO = new PartnerSkillDTO
                {
                    EquipmentSerialId = Instance.EquipmentSerialId,
                    SkillVNum = 1275,
                    Level = ServerManager.RandomNumber<byte>(1, 8)
                };

                if (DAOFactory.PartnerSkillDAO.Insert(partnerSkillDTO) is PartnerSkillDTO result)
                {
                    Skills.Add(new PartnerSkill(result));

                    return true;
                }
            }
            if (Instance.Item.Morph == 2343 && castId == 0)
            {
                PartnerSkillDTO partnerSkillDTO = new PartnerSkillDTO
                {
                    EquipmentSerialId = Instance.EquipmentSerialId,
                    SkillVNum = 1318,
                    Level = ServerManager.RandomNumber<byte>(1, 8)
                };

                if (DAOFactory.PartnerSkillDAO.Insert(partnerSkillDTO) is PartnerSkillDTO result)
                {
                    Skills.Add(new PartnerSkill(result));

                    return true;
                }
            }
            if (Instance.Item.Morph == 2343 && castId == 1)
            {
                PartnerSkillDTO partnerSkillDTO = new PartnerSkillDTO
                {
                    EquipmentSerialId = Instance.EquipmentSerialId,
                    SkillVNum = 1319,
                    Level = ServerManager.RandomNumber<byte>(1, 8)
                };

                if (DAOFactory.PartnerSkillDAO.Insert(partnerSkillDTO) is PartnerSkillDTO result)
                {
                    Skills.Add(new PartnerSkill(result));

                    return true;
                }
            }
            if (Instance.Item.Morph == 2343 && castId == 2)
            {
                PartnerSkillDTO partnerSkillDTO = new PartnerSkillDTO
                {
                    EquipmentSerialId = Instance.EquipmentSerialId,
                    SkillVNum = 1320,
                    Level = ServerManager.RandomNumber<byte>(1, 8)
                };

                if (DAOFactory.PartnerSkillDAO.Insert(partnerSkillDTO) is PartnerSkillDTO result)
                {
                    Skills.Add(new PartnerSkill(result));

                    return true;
                }
            }
            if (Instance.Item.Morph == 2707 && castId == 0)
            {
                PartnerSkillDTO partnerSkillDTO = new PartnerSkillDTO
                {
                    EquipmentSerialId = Instance.EquipmentSerialId,
                    SkillVNum = 1318,
                    Level = ServerManager.RandomNumber<byte>(1, 8)
                };

                if (DAOFactory.PartnerSkillDAO.Insert(partnerSkillDTO) is PartnerSkillDTO result)
                {
                    Skills.Add(new PartnerSkill(result));

                    return true;
                }
            }
            if (Instance.Item.Morph == 2707 && castId == 1)
            {
                PartnerSkillDTO partnerSkillDTO = new PartnerSkillDTO
                {
                    EquipmentSerialId = Instance.EquipmentSerialId,
                    SkillVNum = 1319,
                    Level = ServerManager.RandomNumber<byte>(1, 8)
                };

                if (DAOFactory.PartnerSkillDAO.Insert(partnerSkillDTO) is PartnerSkillDTO result)
                {
                    Skills.Add(new PartnerSkill(result));

                    return true;
                }
            }
            if (Instance.Item.Morph == 2707 && castId == 2)
            {
                PartnerSkillDTO partnerSkillDTO = new PartnerSkillDTO
                {
                    EquipmentSerialId = Instance.EquipmentSerialId,
                    SkillVNum = 1320,
                    Level = ServerManager.RandomNumber<byte>(1, 8)
                };

                if (DAOFactory.PartnerSkillDAO.Insert(partnerSkillDTO) is PartnerSkillDTO result)
                {
                    Skills.Add(new PartnerSkill(result));

                    return true;
                }
            }
            if (Instance.Item.Morph == 2367 && castId == 0)
            {
                PartnerSkillDTO partnerSkillDTO = new PartnerSkillDTO
                {
                    EquipmentSerialId = Instance.EquipmentSerialId,
                    SkillVNum = 1371,
                    Level = ServerManager.RandomNumber<byte>(1, 8)
                };

                if (DAOFactory.PartnerSkillDAO.Insert(partnerSkillDTO) is PartnerSkillDTO result)
                {
                    Skills.Add(new PartnerSkill(result));

                    return true;
                }
            }
            if (Instance.Item.Morph == 2367 && castId == 1)
            {
                PartnerSkillDTO partnerSkillDTO = new PartnerSkillDTO
                {
                    EquipmentSerialId = Instance.EquipmentSerialId,
                    SkillVNum = 1372,
                    Level = ServerManager.RandomNumber<byte>(1, 8)
                };

                if (DAOFactory.PartnerSkillDAO.Insert(partnerSkillDTO) is PartnerSkillDTO result)
                {
                    Skills.Add(new PartnerSkill(result));

                    return true;
                }
            }
            if (Instance.Item.Morph == 2367 && castId == 2)
            {
                PartnerSkillDTO partnerSkillDTO = new PartnerSkillDTO
                {
                    EquipmentSerialId = Instance.EquipmentSerialId,
                    SkillVNum = 1373,
                    Level = ServerManager.RandomNumber<byte>(1, 8)
                };

                if (DAOFactory.PartnerSkillDAO.Insert(partnerSkillDTO) is PartnerSkillDTO result)
                {
                    Skills.Add(new PartnerSkill(result));

                    return true;
                }
            }
            if (Instance.Item.Morph == 2708 && castId == 0)
            {
                PartnerSkillDTO partnerSkillDTO = new PartnerSkillDTO
                {
                    EquipmentSerialId = Instance.EquipmentSerialId,
                    SkillVNum = 1371,
                    Level = ServerManager.RandomNumber<byte>(1, 8)
                };

                if (DAOFactory.PartnerSkillDAO.Insert(partnerSkillDTO) is PartnerSkillDTO result)
                {
                    Skills.Add(new PartnerSkill(result));

                    return true;
                }
            }
            if (Instance.Item.Morph == 2708 && castId == 1)
            {
                PartnerSkillDTO partnerSkillDTO = new PartnerSkillDTO
                {
                    EquipmentSerialId = Instance.EquipmentSerialId,
                    SkillVNum = 1372,
                    Level = ServerManager.RandomNumber<byte>(1, 8)
                };

                if (DAOFactory.PartnerSkillDAO.Insert(partnerSkillDTO) is PartnerSkillDTO result)
                {
                    Skills.Add(new PartnerSkill(result));

                    return true;
                }
            }
            if (Instance.Item.Morph == 2708 && castId == 2)
            {
                PartnerSkillDTO partnerSkillDTO = new PartnerSkillDTO
                {
                    EquipmentSerialId = Instance.EquipmentSerialId,
                    SkillVNum = 1373,
                    Level = ServerManager.RandomNumber<byte>(1, 8)
                };

                if (DAOFactory.PartnerSkillDAO.Insert(partnerSkillDTO) is PartnerSkillDTO result)
                {
                    Skills.Add(new PartnerSkill(result));

                    return true;
                }
            }
            if (Instance.Item.Morph == 2372 && castId == 0)
            {
                PartnerSkillDTO partnerSkillDTO = new PartnerSkillDTO
                {
                    EquipmentSerialId = Instance.EquipmentSerialId,
                    SkillVNum = 1436,
                    Level = ServerManager.RandomNumber<byte>(1, 8)
                };

                if (DAOFactory.PartnerSkillDAO.Insert(partnerSkillDTO) is PartnerSkillDTO result)
                {
                    Skills.Add(new PartnerSkill(result));

                    return true;
                }
            }
            if (Instance.Item.Morph == 2372 && castId == 1)
            {
                PartnerSkillDTO partnerSkillDTO = new PartnerSkillDTO
                {
                    EquipmentSerialId = Instance.EquipmentSerialId,
                    SkillVNum = 1437,
                    Level = ServerManager.RandomNumber<byte>(1, 8)
                };

                if (DAOFactory.PartnerSkillDAO.Insert(partnerSkillDTO) is PartnerSkillDTO result)
                {
                    Skills.Add(new PartnerSkill(result));

                    return true;
                }
            }
            if (Instance.Item.Morph == 2372 && castId == 2)
            {
                PartnerSkillDTO partnerSkillDTO = new PartnerSkillDTO
                {
                    EquipmentSerialId = Instance.EquipmentSerialId,
                    SkillVNum = 1438,
                    Level = ServerManager.RandomNumber<byte>(1, 8)
                };

                if (DAOFactory.PartnerSkillDAO.Insert(partnerSkillDTO) is PartnerSkillDTO result)
                {
                    Skills.Add(new PartnerSkill(result));

                    return true;
                }
            }
            if (Instance.Item.Morph == 2377 && castId == 0)
            {
                PartnerSkillDTO partnerSkillDTO = new PartnerSkillDTO
                {
                    EquipmentSerialId = Instance.EquipmentSerialId,
                    SkillVNum = 1446,
                    Level = ServerManager.RandomNumber<byte>(1, 8)
                };

                if (DAOFactory.PartnerSkillDAO.Insert(partnerSkillDTO) is PartnerSkillDTO result)
                {
                    Skills.Add(new PartnerSkill(result));

                    return true;
                }
            }
            if (Instance.Item.Morph == 2377 && castId == 1)
            {
                PartnerSkillDTO partnerSkillDTO = new PartnerSkillDTO
                {
                    EquipmentSerialId = Instance.EquipmentSerialId,
                    SkillVNum = 1447,
                    Level = ServerManager.RandomNumber<byte>(1, 8)
                };

                if (DAOFactory.PartnerSkillDAO.Insert(partnerSkillDTO) is PartnerSkillDTO result)
                {
                    Skills.Add(new PartnerSkill(result));

                    return true;
                }
            }
            if (Instance.Item.Morph == 2377 && castId == 2)
            {
                PartnerSkillDTO partnerSkillDTO = new PartnerSkillDTO
                {
                    EquipmentSerialId = Instance.EquipmentSerialId,
                    SkillVNum = 1448,
                    Level = ServerManager.RandomNumber<byte>(1, 8)
                };

                if (DAOFactory.PartnerSkillDAO.Insert(partnerSkillDTO) is PartnerSkillDTO result)
                {
                    Skills.Add(new PartnerSkill(result));

                    return true;
                }
            }
            if (Instance.Item.Morph == 2716 && castId == 0)
            {
                PartnerSkillDTO partnerSkillDTO = new PartnerSkillDTO
                {
                    EquipmentSerialId = Instance.EquipmentSerialId,
                    SkillVNum = 1628,
                    Level = ServerManager.RandomNumber<byte>(1, 8)
                };

                if (DAOFactory.PartnerSkillDAO.Insert(partnerSkillDTO) is PartnerSkillDTO result)
                {
                    Skills.Add(new PartnerSkill(result));

                    return true;
                }
            }
            if (Instance.Item.Morph == 2716 && castId == 1)
            {
                PartnerSkillDTO partnerSkillDTO = new PartnerSkillDTO
                {
                    EquipmentSerialId = Instance.EquipmentSerialId,
                    SkillVNum = 1630,
                    Level = ServerManager.RandomNumber<byte>(1, 8)
                };

                if (DAOFactory.PartnerSkillDAO.Insert(partnerSkillDTO) is PartnerSkillDTO result)
                {
                    Skills.Add(new PartnerSkill(result));

                    return true;
                }
            }
            if (Instance.Item.Morph == 2716 && castId == 2)
            {
                PartnerSkillDTO partnerSkillDTO = new PartnerSkillDTO
                {
                    EquipmentSerialId = Instance.EquipmentSerialId,
                    SkillVNum = 1629,
                    Level = ServerManager.RandomNumber<byte>(1, 8)
                };

                if (DAOFactory.PartnerSkillDAO.Insert(partnerSkillDTO) is PartnerSkillDTO result)
                {
                    Skills.Add(new PartnerSkill(result));

                    return true;
                }
            }
            if (Instance.Item.Morph == 2721 && castId == 0)
            {
                PartnerSkillDTO partnerSkillDTO = new PartnerSkillDTO
                {
                    EquipmentSerialId = Instance.EquipmentSerialId,
                    SkillVNum = 1631,
                    Level = ServerManager.RandomNumber<byte>(1, 8)
                };

                if (DAOFactory.PartnerSkillDAO.Insert(partnerSkillDTO) is PartnerSkillDTO result)
                {
                    Skills.Add(new PartnerSkill(result));

                    return true;
                }
            }
            if (Instance.Item.Morph == 2721 && castId == 1)
            {
                PartnerSkillDTO partnerSkillDTO = new PartnerSkillDTO
                {
                    EquipmentSerialId = Instance.EquipmentSerialId,
                    SkillVNum = 1633,
                    Level = ServerManager.RandomNumber<byte>(1, 8)
                };

                if (DAOFactory.PartnerSkillDAO.Insert(partnerSkillDTO) is PartnerSkillDTO result)
                {
                    Skills.Add(new PartnerSkill(result));

                    return true;
                }
            }
            if (Instance.Item.Morph == 2721 && castId == 2)
            {
                PartnerSkillDTO partnerSkillDTO = new PartnerSkillDTO
                {
                    EquipmentSerialId = Instance.EquipmentSerialId,
                    SkillVNum = 1632,
                    Level = ServerManager.RandomNumber<byte>(1, 8)
                };

                if (DAOFactory.PartnerSkillDAO.Insert(partnerSkillDTO) is PartnerSkillDTO result)
                {
                    Skills.Add(new PartnerSkill(result));

                    return true;
                }
            }
            if (Instance.Item.Morph == 2045 && castId == 0)
            {
                PartnerSkillDTO partnerSkillDTO = new PartnerSkillDTO
                {
                    EquipmentSerialId = Instance.EquipmentSerialId,
                    SkillVNum = 1241,
                    Level = ServerManager.RandomNumber<byte>(1, 8)
                };

                if (DAOFactory.PartnerSkillDAO.Insert(partnerSkillDTO) is PartnerSkillDTO result)
                {
                    Skills.Add(new PartnerSkill(result));

                    return true;
                }
            }
            if (Instance.Item.Morph == 2045 && castId == 1)
            {
                PartnerSkillDTO partnerSkillDTO = new PartnerSkillDTO
                {
                    EquipmentSerialId = Instance.EquipmentSerialId,
                    SkillVNum = 1242,
                    Level = ServerManager.RandomNumber<byte>(1, 8)
                };

                if (DAOFactory.PartnerSkillDAO.Insert(partnerSkillDTO) is PartnerSkillDTO result)
                {
                    Skills.Add(new PartnerSkill(result));

                    return true;
                }
            }
            if (Instance.Item.Morph == 2045 && castId == 2)
            {
                PartnerSkillDTO partnerSkillDTO = new PartnerSkillDTO
                {
                    EquipmentSerialId = Instance.EquipmentSerialId,
                    SkillVNum = 1243,
                    Level = ServerManager.RandomNumber<byte>(1, 8)
                };

                if (DAOFactory.PartnerSkillDAO.Insert(partnerSkillDTO) is PartnerSkillDTO result)
                {
                    Skills.Add(new PartnerSkill(result));

                    return true;
                }
            }
            if (Instance.Item.Morph == 2048 && castId == 0)
            {
                PartnerSkillDTO partnerSkillDTO = new PartnerSkillDTO
                {
                    EquipmentSerialId = Instance.EquipmentSerialId,
                    SkillVNum = 1272,
                    Level = ServerManager.RandomNumber<byte>(1, 8)
                };

                if (DAOFactory.PartnerSkillDAO.Insert(partnerSkillDTO) is PartnerSkillDTO result)
                {
                    Skills.Add(new PartnerSkill(result));

                    return true;
                }
            }
            if (Instance.Item.Morph == 2048 && castId == 1)
            {
                PartnerSkillDTO partnerSkillDTO = new PartnerSkillDTO
                {
                    EquipmentSerialId = Instance.EquipmentSerialId,
                    SkillVNum = 1274,
                    Level = ServerManager.RandomNumber<byte>(1, 8)
                };

                if (DAOFactory.PartnerSkillDAO.Insert(partnerSkillDTO) is PartnerSkillDTO result)
                {
                    Skills.Add(new PartnerSkill(result));

                    return true;
                }
            }
            if (Instance.Item.Morph == 2048 && castId == 2)
            {
                PartnerSkillDTO partnerSkillDTO = new PartnerSkillDTO
                {
                    EquipmentSerialId = Instance.EquipmentSerialId,
                    SkillVNum = 1276,
                    Level = ServerManager.RandomNumber<byte>(1, 8)
                };

                if (DAOFactory.PartnerSkillDAO.Insert(partnerSkillDTO) is PartnerSkillDTO result)
                {
                    Skills.Add(new PartnerSkill(result));

                    return true;
                }
            }
            if (Instance.Item.Morph == 2333 && castId == 0)
            {
                PartnerSkillDTO partnerSkillDTO = new PartnerSkillDTO
                {
                    EquipmentSerialId = Instance.EquipmentSerialId,
                    SkillVNum = 1315,
                    Level = ServerManager.RandomNumber<byte>(1, 8)
                };

                if (DAOFactory.PartnerSkillDAO.Insert(partnerSkillDTO) is PartnerSkillDTO result)
                {
                    Skills.Add(new PartnerSkill(result));

                    return true;
                }
            }
            if (Instance.Item.Morph == 2333 && castId == 1)
            {
                PartnerSkillDTO partnerSkillDTO = new PartnerSkillDTO
                {
                    EquipmentSerialId = Instance.EquipmentSerialId,
                    SkillVNum = 1316,
                    Level = ServerManager.RandomNumber<byte>(1, 8)
                };

                if (DAOFactory.PartnerSkillDAO.Insert(partnerSkillDTO) is PartnerSkillDTO result)
                {
                    Skills.Add(new PartnerSkill(result));

                    return true;
                }
            }
            if (Instance.Item.Morph == 2333 && castId == 2)
            {
                PartnerSkillDTO partnerSkillDTO = new PartnerSkillDTO
                {
                    EquipmentSerialId = Instance.EquipmentSerialId,
                    SkillVNum = 1317,
                    Level = ServerManager.RandomNumber<byte>(1, 8)
                };

                if (DAOFactory.PartnerSkillDAO.Insert(partnerSkillDTO) is PartnerSkillDTO result)
                {
                    Skills.Add(new PartnerSkill(result));

                    return true;
                }
            }
            if (Instance.Item.Morph == 2334 && castId == 0)
            {
                PartnerSkillDTO partnerSkillDTO = new PartnerSkillDTO
                {
                    EquipmentSerialId = Instance.EquipmentSerialId,
                    SkillVNum = 1315,
                    Level = ServerManager.RandomNumber<byte>(1, 8)
                };

                if (DAOFactory.PartnerSkillDAO.Insert(partnerSkillDTO) is PartnerSkillDTO result)
                {
                    Skills.Add(new PartnerSkill(result));

                    return true;
                }
            }
            if (Instance.Item.Morph == 2334 && castId == 1)
            {
                PartnerSkillDTO partnerSkillDTO = new PartnerSkillDTO
                {
                    EquipmentSerialId = Instance.EquipmentSerialId,
                    SkillVNum = 1316,
                    Level = ServerManager.RandomNumber<byte>(1, 8)
                };

                if (DAOFactory.PartnerSkillDAO.Insert(partnerSkillDTO) is PartnerSkillDTO result)
                {
                    Skills.Add(new PartnerSkill(result));

                    return true;
                }
            }
            if (Instance.Item.Morph == 2334 && castId == 2)
            {
                PartnerSkillDTO partnerSkillDTO = new PartnerSkillDTO
                {
                    EquipmentSerialId = Instance.EquipmentSerialId,
                    SkillVNum = 1317,
                    Level = ServerManager.RandomNumber<byte>(1, 8)
                };

                if (DAOFactory.PartnerSkillDAO.Insert(partnerSkillDTO) is PartnerSkillDTO result)
                {
                    Skills.Add(new PartnerSkill(result));

                    return true;
                }
            }
            if (Instance.Item.Morph == 2325 && castId == 0)
            {
                PartnerSkillDTO partnerSkillDTO = new PartnerSkillDTO
                {
                    EquipmentSerialId = Instance.EquipmentSerialId,
                    SkillVNum = 1333,
                    Level = ServerManager.RandomNumber<byte>(1, 8)
                };

                if (DAOFactory.PartnerSkillDAO.Insert(partnerSkillDTO) is PartnerSkillDTO result)
                {
                    Skills.Add(new PartnerSkill(result));

                    return true;
                }
            }
            if (Instance.Item.Morph == 2325 && castId == 1)
            {
                PartnerSkillDTO partnerSkillDTO = new PartnerSkillDTO
                {
                    EquipmentSerialId = Instance.EquipmentSerialId,
                    SkillVNum = 1334,
                    Level = ServerManager.RandomNumber<byte>(1, 8)
                };

                if (DAOFactory.PartnerSkillDAO.Insert(partnerSkillDTO) is PartnerSkillDTO result)
                {
                    Skills.Add(new PartnerSkill(result));

                    return true;
                }
            }
            if (Instance.Item.Morph == 2325 && castId == 2)
            {
                PartnerSkillDTO partnerSkillDTO = new PartnerSkillDTO
                {
                    EquipmentSerialId = Instance.EquipmentSerialId,
                    SkillVNum = 1335,
                    Level = ServerManager.RandomNumber<byte>(1, 8)
                };

                if (DAOFactory.PartnerSkillDAO.Insert(partnerSkillDTO) is PartnerSkillDTO result)
                {
                    Skills.Add(new PartnerSkill(result));

                    return true;
                }
            }
            if (Instance.Item.Morph == 2368 && castId == 0)
            {
                PartnerSkillDTO partnerSkillDTO = new PartnerSkillDTO
                {
                    EquipmentSerialId = Instance.EquipmentSerialId,
                    SkillVNum = 1379,
                    Level = ServerManager.RandomNumber<byte>(1, 8)
                };

                if (DAOFactory.PartnerSkillDAO.Insert(partnerSkillDTO) is PartnerSkillDTO result)
                {
                    Skills.Add(new PartnerSkill(result));

                    return true;
                }
            }
            if (Instance.Item.Morph == 2368 && castId == 1)
            {
                PartnerSkillDTO partnerSkillDTO = new PartnerSkillDTO
                {
                    EquipmentSerialId = Instance.EquipmentSerialId,
                    SkillVNum = 1380,
                    Level = ServerManager.RandomNumber<byte>(1, 8)
                };

                if (DAOFactory.PartnerSkillDAO.Insert(partnerSkillDTO) is PartnerSkillDTO result)
                {
                    Skills.Add(new PartnerSkill(result));

                    return true;
                }
            }
            if (Instance.Item.Morph == 2368 && castId == 2)
            {
                PartnerSkillDTO partnerSkillDTO = new PartnerSkillDTO
                {
                    EquipmentSerialId = Instance.EquipmentSerialId,
                    SkillVNum = 1381,
                    Level = ServerManager.RandomNumber<byte>(1, 8)
                };

                if (DAOFactory.PartnerSkillDAO.Insert(partnerSkillDTO) is PartnerSkillDTO result)
                {
                    Skills.Add(new PartnerSkill(result));

                    return true;
                }
            }
            if (Instance.Item.Morph == 2373 && castId == 0)
            {
                PartnerSkillDTO partnerSkillDTO = new PartnerSkillDTO
                {
                    EquipmentSerialId = Instance.EquipmentSerialId,
                    SkillVNum = 1433,
                    Level = ServerManager.RandomNumber<byte>(1, 8)
                };

                if (DAOFactory.PartnerSkillDAO.Insert(partnerSkillDTO) is PartnerSkillDTO result)
                {
                    Skills.Add(new PartnerSkill(result));

                    return true;
                }
            }
            if (Instance.Item.Morph == 2373 && castId == 1)
            {
                PartnerSkillDTO partnerSkillDTO = new PartnerSkillDTO
                {
                    EquipmentSerialId = Instance.EquipmentSerialId,
                    SkillVNum = 1434,
                    Level = ServerManager.RandomNumber<byte>(1, 8)
                };

                if (DAOFactory.PartnerSkillDAO.Insert(partnerSkillDTO) is PartnerSkillDTO result)
                {
                    Skills.Add(new PartnerSkill(result));

                    return true;
                }
            }
            if (Instance.Item.Morph == 2373 && castId == 2)
            {
                PartnerSkillDTO partnerSkillDTO = new PartnerSkillDTO
                {
                    EquipmentSerialId = Instance.EquipmentSerialId,
                    SkillVNum = 1435,
                    Level = ServerManager.RandomNumber<byte>(1, 8)
                };

                if (DAOFactory.PartnerSkillDAO.Insert(partnerSkillDTO) is PartnerSkillDTO result)
                {
                    Skills.Add(new PartnerSkill(result));

                    return true;
                }
            }
            if (Instance.Item.Morph == 2376 && castId == 0)
            {
                PartnerSkillDTO partnerSkillDTO = new PartnerSkillDTO
                {
                    EquipmentSerialId = Instance.EquipmentSerialId,
                    SkillVNum = 1442,
                    Level = ServerManager.RandomNumber<byte>(1, 8)
                };

                if (DAOFactory.PartnerSkillDAO.Insert(partnerSkillDTO) is PartnerSkillDTO result)
                {
                    Skills.Add(new PartnerSkill(result));

                    return true;
                }
            }
            if (Instance.Item.Morph == 2376 && castId == 1)
            {
                PartnerSkillDTO partnerSkillDTO = new PartnerSkillDTO
                {
                    EquipmentSerialId = Instance.EquipmentSerialId,
                    SkillVNum = 1443,
                    Level = ServerManager.RandomNumber<byte>(1, 8)
                };

                if (DAOFactory.PartnerSkillDAO.Insert(partnerSkillDTO) is PartnerSkillDTO result)
                {
                    Skills.Add(new PartnerSkill(result));

                    return true;
                }
            }
            if (Instance.Item.Morph == 2376 && castId == 2)
            {
                PartnerSkillDTO partnerSkillDTO = new PartnerSkillDTO
                {
                    EquipmentSerialId = Instance.EquipmentSerialId,
                    SkillVNum = 1444,
                    Level = ServerManager.RandomNumber<byte>(1, 8)
                };

                if (DAOFactory.PartnerSkillDAO.Insert(partnerSkillDTO) is PartnerSkillDTO result)
                {
                    Skills.Add(new PartnerSkill(result));

                    return true;
                }
            }
            if (Instance.Item.Morph == 2379 && castId == 0)
            {
                PartnerSkillDTO partnerSkillDTO = new PartnerSkillDTO
                {
                    EquipmentSerialId = Instance.EquipmentSerialId,
                    SkillVNum = 1602,
                    Level = ServerManager.RandomNumber<byte>(1, 8)
                };

                if (DAOFactory.PartnerSkillDAO.Insert(partnerSkillDTO) is PartnerSkillDTO result)
                {
                    Skills.Add(new PartnerSkill(result));

                    return true;
                }
            }
            if (Instance.Item.Morph == 2379 && castId == 1)
            {
                PartnerSkillDTO partnerSkillDTO = new PartnerSkillDTO
                {
                    EquipmentSerialId = Instance.EquipmentSerialId,
                    SkillVNum = 1603,
                    Level = ServerManager.RandomNumber<byte>(1, 8)
                };

                if (DAOFactory.PartnerSkillDAO.Insert(partnerSkillDTO) is PartnerSkillDTO result)
                {
                    Skills.Add(new PartnerSkill(result));

                    return true;
                }
            }
            if (Instance.Item.Morph == 2379 && castId == 2)
            {
                PartnerSkillDTO partnerSkillDTO = new PartnerSkillDTO
                {
                    EquipmentSerialId = Instance.EquipmentSerialId,
                    SkillVNum = 1604,
                    Level = ServerManager.RandomNumber<byte>(1, 8)
                };

                if (DAOFactory.PartnerSkillDAO.Insert(partnerSkillDTO) is PartnerSkillDTO result)
                {
                    Skills.Add(new PartnerSkill(result));

                    return true;
                }
            }
            else
            {
                Logger.Warn($"Partner skill not found (Morph: {Instance.Item.Morph}, CastId: {castId})");
            }

            return false;
        }

        public void AddXp(long amount)
        {
            if (Instance.XP < XpMax)
            {
                Instance.XP = Math.Min(Instance.XP + amount, XpMax);
            }
        }

        public void FullXp()
        {
            Instance.XP = XpMax;
        }

        public bool CanLearnSkill() => Instance.XP >= XpMax;

        public void ClearSkills()
        {
            for (byte castId = 0; castId < 3; castId++)
            {
                RemoveSkill(castId);
            }

            ReloadSkills();
        }

        public string GeneratePski()
        {
            string pski = "pski";

            foreach (PartnerSkill partnerSkill in Skills.OrderBy(s => s.Skill.CastId))
            {
                pski += $" {partnerSkill.Skill.SkillVNum}";
            }

            return pski;
        }

        public string GenerateSkills(bool withLevel = true)
        {
            string skills = "";

            for (byte castId = 0; castId < 3; castId++)
            {
                PartnerSkill partnerSkill = GetSkill(castId);

                if (partnerSkill != null)
                {
                    skills += withLevel ? $" {partnerSkill.SkillVNum}.{partnerSkill.Level}" : $" {partnerSkill.SkillVNum}";
                }
                else
                {
                    skills += withLevel ? $" 0.0" : $" 0";
                }
            }

            return skills;
        }

        public int GetCooldown()
        {
            double maxCooldown = 30000;

            foreach (PartnerSkill partnerSkill in Skills.ToList().Where(s => !s.CanBeUsed()))
            {
                double remaining = (partnerSkill.LastUse - DateTime.Now).TotalMilliseconds + (partnerSkill.Skill.Cooldown * 100);

                if (remaining > maxCooldown)
                {
                    maxCooldown = remaining;
                }
            }

            return (int)(maxCooldown / 1000D);
        }

        public string GetName()
        {
            switch (Instance.ItemVNum)
            {
                case 4324: return (Language.Instance.GetMessageFromKey("GUARDIAN_LUCIFER"));
                case 4325: return (Language.Instance.GetMessageFromKey("SHERRIF_CHLOE"));
                case 4326: return (Language.Instance.GetMessageFromKey("BONE_WARRIOR_RAGNAR"));
                case 4343: return (Language.Instance.GetMessageFromKey("PROFESSOR_MAVACITY"));
                case 4349: return (Language.Instance.GetMessageFromKey("ARCHDAEMON_AMON"));
                case 4405: return (Language.Instance.GetMessageFromKey("MAGIC_STUDENT_YUNA"));
                case 4413: return (Language.Instance.GetMessageFromKey("AMORA"));
                case 4417: return (Language.Instance.GetMessageFromKey("MAD_MARCH_HARE"));
                case 4446: return (Language.Instance.GetMessageFromKey("PERTI_DEPLUMAY"));
                case 4800: return (Language.Instance.GetMessageFromKey("AEGIR_THE_ANGRY"));
                case 4802: return (Language.Instance.GetMessageFromKey("BARNI_THE_CLEVER"));
                case 4803: return (Language.Instance.GetMessageFromKey("FREYA_THE_FATEFUL"));
                case 4804: return (Language.Instance.GetMessageFromKey("SHINOBI_THE_SILENT"));
                case 4805: return (Language.Instance.GetMessageFromKey("LOTUS_THE_GRATEFUL"));
                case 4806: return (Language.Instance.GetMessageFromKey("ORKANI_THE_TURBULENT"));
                case 4807: return (Language.Instance.GetMessageFromKey("FOXY"));
                case 4808: return (Language.Instance.GetMessageFromKey("MARU"));
                case 4809: return (Language.Instance.GetMessageFromKey("MARU_THE_MOTHERS_FUR"));
                case 4810: return (Language.Instance.GetMessageFromKey("HONGBI"));
                case 4811: return (Language.Instance.GetMessageFromKey("CHEONGBI"));
                case 4812: return (Language.Instance.GetMessageFromKey("LUCIFER"));
                case 4813: return (Language.Instance.GetMessageFromKey("WITCH_LAURENA"));
                case 4814: return (Language.Instance.GetMessageFromKey("AMON"));
                case 4815: return (Language.Instance.GetMessageFromKey("LUCY_LOPEARS"));
                case 4817: return (Language.Instance.GetMessageFromKey("COWGIRL_CHLOE"));
                case 4818: return (Language.Instance.GetMessageFromKey("FIONA"));
                case 4819: return (Language.Instance.GetMessageFromKey("JINN"));
                case 4820: return (Language.Instance.GetMessageFromKey("ICE_PRINCESS_ELIZA"));
                case 4821: return (Language.Instance.GetMessageFromKey("DANIEL_DUCATS"));
                case 4822: return (Language.Instance.GetMessageFromKey("PALINA_PUPPET_MASTER"));
                case 4823: return (Language.Instance.GetMessageFromKey("HARLEQUIN"));
                case 4824: return (Language.Instance.GetMessageFromKey("NELIA_NYMPH"));
                case 4825: return (Language.Instance.GetMessageFromKey("LITTLE_PRINCESS_VENUS"));
                case 8398: return (Language.Instance.GetMessageFromKey("PERTI_DEPLUMAY"));

            }

            return Instance.Item.Name.Replace(' ', '^');
        }

        private Skill GetNewSkill(byte castId) => ServerManager.GetAllSkill().FirstOrDefault(s => s.CastId == castId && (s.Class == 28 || s.Class == 29)
            && s.UpgradeType == MateHelper.Instance.GetUpgradeType(Instance.Item.Morph));

        public PartnerSkill GetSkill(byte castId)
            => Skills.FirstOrDefault(s => s.Skill.CastId == castId);

        public int GetSkillsCount() => Skills.Count;

        public int GetSkillsLevels()
        {
            int levels = 0;
            Skills.ForEach(p => levels += p.Level);
            levels = levels % 3 == 2 ? levels + 1 : levels;
            return levels;
        }

        public int GetXpPercent() => (int)Math.Floor(100D / XpMax * Instance.XP);

        private void Initialize()
        {
            if (Instance.EquipmentSerialId == Guid.Empty)
            {
                Instance.EquipmentSerialId = Guid.NewGuid();
            }

            LoadSkills();
        }

        private void LoadSkills()
        {
            Skills = DAOFactory.PartnerSkillDAO.LoadByEquipmentSerialId(Instance.EquipmentSerialId)
                .Select(partnerSkillDTO => new PartnerSkill(partnerSkillDTO)).ToList();
        }

        public void ReloadSkills() => LoadSkills();

        public bool RemoveSkill(byte castId)
        {
            PartnerSkill partnerSkill = GetSkill(castId);

            return partnerSkill != null && DAOFactory.PartnerSkillDAO.Remove(partnerSkill.PartnerSkillId) != DeleteResult.Error;
        }

        public void ResetXp()
        {
            Instance.XP = 0;
        }

        #endregion
    }
}
