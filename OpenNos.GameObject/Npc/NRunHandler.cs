/*
 * This file is part of the OpenNos Emulator Project. See AUTHORS file for Copyright information
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 */

using OpenNos.Core;
using OpenNos.DAL;
using OpenNos.Data;
using OpenNos.Domain;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;
using OpenNos.Master.Library.Client;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using NosTale.Packets.Packets.ClientPackets;
using OpenNos.GameObject.Extension;
using OpenNos.GameObject.Event;

namespace OpenNos.GameObject 

{
    
    public static class NRunHandler
    {
        private static UpgradeProtection protection;
        public static int Upgrade { get; private set; }
        public static object Id { get; private set; }
        #region Methods

        public static void NRun(ClientSession Session, NRunPacket packet)
        {
            if (!Session.HasCurrentMapInstance)
            {
                return;
            }

            MapNpc npc = Session.CurrentMapInstance.Npcs.Find(s => s.MapNpcId == packet.NpcId);

            TeleporterDTO tp;

            var rand = new Random();
            switch (packet.Runner)
            {
                case 1:
                    if (Session.Character.Class != (byte)ClassType.Adventurer)
                    {
                        Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NOT_ADVENTURER"), 0));
                        return;
                    }
                    if (Session.Character.Level < 15 || Session.Character.JobLevel < 20)
                    {
                        Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("LOW_LVL"), 0));
                        return;
                    }
                    if (packet.Type > 3 || packet.Type < 1)
                    {
                        return;
                    }
                    if (packet.Type == (byte)Session.Character.Class)
                    {
                        return;
                    }
                    if (Session.Character.Inventory.All(i => i.Type != InventoryType.Wear))
                    {
                        Session.Character.Inventory.AddNewToInventory((short)(4 + (packet.Type * 14)), 1, InventoryType.Wear);
                        Session.Character.Inventory.AddNewToInventory((short)(81 + (packet.Type * 13)), 1, InventoryType.Wear);

                        switch (packet.Type)
                        {
                            case 1:
                                Session.Character.Inventory.AddNewToInventory(68, 1, InventoryType.Wear);
                                break;

                            case 2:
                                Session.Character.Inventory.AddNewToInventory(78, 1, InventoryType.Wear);
                                break;

                            case 3:
                                Session.Character.Inventory.AddNewToInventory(86, 1, InventoryType.Wear);
                                break;
                        }

                        Session.CurrentMapInstance?.Broadcast(Session.Character.GenerateEq());
                        Session.SendPacket(Session.Character.GenerateEquipment());
                        Session.Character.ChangeClass((ClassType)packet.Type, false);
                    }
                    else
                    {
                        Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("EQ_NOT_EMPTY"), 0));
                    }
                    break;

                case 2:
                    Session.SendPacket("wopen 1 0");
                    break;

                case 3:
                    NpcMonster heldMonster = ServerManager.GetNpcMonster(packet.Type);
                    if (heldMonster != null && !Session.Character.Mates.Any(m => m.NpcMonsterVNum == heldMonster.NpcMonsterVNum && !m.IsTemporalMate) && Session.Character.Mates.FirstOrDefault(s => s.NpcMonsterVNum == heldMonster.NpcMonsterVNum && s.IsTemporalMate && s.IsTsReward) is Mate partnerToReceive)
                    {
                        Session.Character.RemoveTemporalMates();
                        Mate partner = new Mate(Session.Character, heldMonster, heldMonster.Level, MateType.Partner);
                        partner.Experience = partnerToReceive.Experience;
                        if (!Session.Character.Mates.Any(s => s.MateType == MateType.Partner && s.IsTeamMember))
                        {
                            partner.IsTeamMember = true;
                        }
                        Session.Character.AddPet(partner);
                    }
                    break;

                case 4:
                    Mate mate = Session.Character.Mates.Find(s => s.MateTransportId == packet.NpcId);
                    switch (packet.Type)
                    {
                        case 2:
                            if (mate != null)
                            {
                                if (Session.Character.Miniland == Session.Character.MapInstance)
                                {
                                    if (Session.Character.Level >= mate.Level)
                                    {
                                        Mate teammate = Session.Character.Mates.Where(s => s.IsTeamMember).FirstOrDefault(s => s.MateType == mate.MateType);
                                        if (teammate != null)
                                        {
                                            teammate.RemoveTeamMember();
                                        }
                                        mate.AddTeamMember();
                                    }
                                    else
                                    {
                                        Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("PET_HIGHER_LEVEL"), 0));
                                    }
                                }
                            }
                            break;

                        case 3:
                            if (mate != null && Session.Character.Miniland == Session.Character.MapInstance)
                            {
                                mate.RemoveTeamMember();
                            }
                            break;

                        case 4:
                            if (mate != null)
                            {
                                if (Session.Character.Miniland == Session.Character.MapInstance)
                                {
                                    mate.RemoveTeamMember(false);
                                    mate.MapX = mate.PositionX;
                                    mate.MapY = mate.PositionY;
                                }
                                else
                                {
                                    Session.SendPacket($"qna #n_run^4^5^3^{mate.MateTransportId} {Language.Instance.GetMessageFromKey("ASK_KICK_PET")}");
                                }
                                break;
                            }
                            break;

                        case 5:
                            if (mate != null)
                            {
                                Session.SendPacket(UserInterfaceHelper.GenerateDelay(3000, 10, $"#n_run^4^6^3^{mate.MateTransportId}"));
                            }
                            break;

                        case 6:
                            if (mate != null && Session.Character.Miniland != Session.Character.MapInstance)
                            {
                                mate.BackToMiniland();
                                #region Remove Partner Buff
                                Session.Character.RemoveBuff(3000);
                                Session.Character.RemoveBuff(3001);
                                Session.Character.RemoveBuff(3002);
                                Session.Character.RemoveBuff(3003);
                                Session.Character.RemoveBuff(3004);
                                Session.Character.RemoveBuff(3005);
                                Session.Character.RemoveBuff(3006);
                                Session.Character.RemoveBuff(3007);
                                Session.Character.RemoveBuff(3008);
                                Session.Character.RemoveBuff(3009);
                                Session.Character.RemoveBuff(3010);
                                Session.Character.RemoveBuff(3011);
                                Session.Character.RemoveBuff(3012);
                                Session.Character.RemoveBuff(3013);
                                Session.Character.RemoveBuff(3014);
                                Session.Character.RemoveBuff(3015);
                                Session.Character.RemoveBuff(3016);
                                Session.Character.RemoveBuff(3017);
                                Session.Character.RemoveBuff(3018);
                                Session.Character.RemoveBuff(3019);
                                Session.Character.RemoveBuff(3020);
                                Session.Character.RemoveBuff(3021);
                                Session.Character.RemoveBuff(3022);
                                Session.Character.RemoveBuff(3023);
                                Session.Character.RemoveBuff(3024);
                                Session.Character.RemoveBuff(3025);
                                Session.Character.RemoveBuff(3026);
                                Session.Character.RemoveBuff(3027);
                                Session.Character.RemoveBuff(3028);
                                Session.Character.RemoveBuff(3029);
                                Session.Character.RemoveBuff(3030);
                                Session.Character.RemoveBuff(3031);
                                Session.Character.RemoveBuff(3032);
                                Session.Character.RemoveBuff(3033);
                                Session.Character.RemoveBuff(3034);
                                #endregion
                            }
                            break;

                        case 7:
                            if (mate != null)
                            {
                                if (Session.Character.Mates.Any(s => s.MateType == mate.MateType && s.IsTeamMember))
                                {
                                    Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("ALREADY_PET_IN_TEAM"), 11));
                                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("ALREADY_PET_IN_TEAM"), 0));
                                }
                                else
                                {
                                    mate.RemoveTeamMember();
                                    Session.SendPacket(UserInterfaceHelper.GenerateDelay(3000, 10, $"#n_run^4^9^3^{mate.MateTransportId}"));
                                }
                            }
                            break;

                        case 9:
                            if (mate != null && mate.IsSummonable && Session.Character.MapInstance.MapInstanceType != MapInstanceType.TalentArenaMapInstance)
                            {
                                if (Session.Character.Level >= mate.Level)
                                {
                                    mate.PositionX = (short)(Session.Character.PositionX + (mate.MateType == MateType.Partner ? -1 : 1));
                                    mate.PositionY = (short)(Session.Character.PositionY + 1);
                                    mate.AddTeamMember();
                                    Parallel.ForEach(Session.CurrentMapInstance.Sessions.Where(s => s.Character != null), s =>
                                    {
                                        if (ServerManager.Instance.ChannelId != 51 || Session.Character.Faction == s.Character.Faction)
                                        {
                                            s.SendPacket(mate.GenerateIn(false, ServerManager.Instance.ChannelId == 51));
                                        }
                                        else
                                        {
                                            s.SendPacket(mate.GenerateIn(true, ServerManager.Instance.ChannelId == 51, s.Account.Authority));
                                        }
                                    });
                                }
                                else
                                {
                                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("PET_HIGHER_LEVEL"), 0));
                                }
                            }
                            break;
                    }
                    Session.SendPacket(Session.Character.GeneratePinit());
                    Session.SendPackets(Session.Character.GeneratePst());
                    break;

                case 10:
                    Session.SendPacket("wopen 3 0");
                    break;

                case 12:
                    Session.SendPacket($"wopen {packet.Type} 0");
                    break;

                case 14:
                    Session.SendPacket("wopen 27 0");
                    string recipelist = "m_list 2";
                    if (npc != null)
                    {
                        List<Recipe> tps = npc.Recipes;
                        recipelist = tps.Where(s => s.Amount > 0).Aggregate(recipelist, (current, s) => current + $" {s.ItemVNum}");
                        recipelist += " -100";
                        Session.SendPacket(recipelist);
                    }
                    break;

                case 15:
                    if (npc != null)
                    {
                        if (packet.Value == 2)
                        {
                            Session.SendPacket($"qna #n_run^15^1^1^{npc.MapNpcId} {Language.Instance.GetMessageFromKey("ASK_CHANGE_SPAWNLOCATION")}");
                        }
                        else
                        {
                            switch (npc.MapId)
                            {
                                case 1:
                                    Session.Character.SetRespawnPoint(1, 79, 116);
                                    break;

                                case 20:
                                    Session.Character.SetRespawnPoint(20, 9, 92);
                                    break;

                                case 145:
                                    Session.Character.SetRespawnPoint(145, 13, 110);
                                    break;

                                case 170:
                                    Session.Character.SetRespawnPoint(170, 79, 47);
                                    break;

                                case 177:
                                    Session.Character.SetRespawnPoint(177, 149, 74);
                                    break;

                                case 189:
                                    Session.Character.SetRespawnPoint(189, 58, 166);
                                    break;

                                case 228:
                                    Session.Character.SetRespawnPoint(228, 80, 98);
                                    break;
                            }
                            Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("RESPAWNLOCATION_CHANGED"), 0));
                        }
                    }
                    break;

                case 16:
                    tp = npc?.Teleporters?.FirstOrDefault(s => s.Index == packet.Type);
                    if (tp != null)
                    {
                        if (packet.Type >= 0 && Session.Character.Gold >= 1000 * packet.Type)
                        {
                            Session.Character.Gold -= 1000 * packet.Type;
                            Session.SendPacket(Session.Character.GenerateGold());
                            ServerManager.Instance.ChangeMap(Session.Character.CharacterId, tp.MapId, (short)(tp.MapX + ServerManager.RandomNumber(-2, 2)), (short)(tp.MapY + ServerManager.RandomNumber(2, 2)));
                        }
                        else
                        {
                            Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("NOT_ENOUGH_MONEY"), 10));
                        }
                    }
                    break;

                case 17:
                    switch (packet.Type)
                    {
                        case 0:
                            if (Session.Character.MapInstance.MapInstanceType != MapInstanceType.BaseMapInstance)
                            {
                                return;
                            }
                            if (packet.Value == 1)
                            {
                                Session.SendPacket($"qna #n_run^{packet.Runner}^{packet.Type}^2^{packet.NpcId} {string.Format(Language.Instance.GetMessageFromKey("ASK_ENTER_GOLD_NORM"), 500 * (1 + packet.Type))}");
                            }
                            else
                            {
                                double currentRunningSeconds = (DateTime.Now - Process.GetCurrentProcess().StartTime.AddSeconds(-50)).TotalSeconds;
                                double timeSpanSinceLastPortal = currentRunningSeconds - Session.Character.LastPortal;
                                if (!(timeSpanSinceLastPortal >= 4) || !Session.HasCurrentMapInstance || ServerManager.Instance.ChannelId == 51 || Session.CurrentMapInstance.MapInstanceId == ServerManager.Instance.ArenaInstance.MapInstanceId || Session.CurrentMapInstance.MapInstanceId == ServerManager.Instance.FamilyArenaInstance.MapInstanceId)
                                {
                                    Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("CANT_MOVE"), 10));
                                    return;
                                }
                                if (packet.Type >= 0 && Session.Character.Gold >= 500 * (1 + packet.Type))
                                {
                                    Session.Character.LastPortal = currentRunningSeconds;
                                    Session.Character.Gold -= 500 * (1 + packet.Type);
                                    Session.SendPacket(Session.Character.GenerateGold());
                                    Session.SendPacket(Session.Character.GenerateAscr());
                                    Session.SendPacket("msg 3 PvP will be available in 5 Seconds.");
                                    MapCell pos = packet.Type == 0 ? ServerManager.Instance.ArenaInstance.Map.GetRandomPosition() : ServerManager.Instance.FamilyArenaInstance.Map.GetRandomPosition();
                                    ServerManager.Instance.ChangeMapInstance(Session.Character.CharacterId, packet.Type == 0 ? ServerManager.Instance.ArenaInstance.MapInstanceId : ServerManager.Instance.FamilyArenaInstance.MapInstanceId, pos.X, pos.Y);
                                }
                                else
                                {
                                    Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("NOT_ENOUGH_MONEY"), 10));
                                }
                            }
                            break;

                        case 1:
                            if (Session.Character.MapInstance.MapInstanceType != MapInstanceType.BaseMapInstance)
                            {
                                return;
                            }
                            if (packet.Value == 1)
                            {
                                Session.SendPacket($"qna #n_run^{packet.Runner}^{packet.Type}^2^{packet.NpcId} {string.Format(Language.Instance.GetMessageFromKey("ASK_ENTER_GOLD_FAM"), 1000 * (1 + packet.Type))}");
                            }
                            else
                            {
                                double currentRunningSeconds = (DateTime.Now - Process.GetCurrentProcess().StartTime.AddSeconds(-50)).TotalSeconds;
                                double timeSpanSinceLastPortal = currentRunningSeconds - Session.Character.LastPortal;
                                if (!(timeSpanSinceLastPortal >= 4) || !Session.HasCurrentMapInstance || ServerManager.Instance.ChannelId == 51 || Session.CurrentMapInstance.MapInstanceId == ServerManager.Instance.ArenaInstance.MapInstanceId || Session.CurrentMapInstance.MapInstanceId == ServerManager.Instance.FamilyArenaInstance.MapInstanceId)
                                {
                                    Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("CANT_MOVE"), 10));
                                    return;
                                }
                                if (packet.Type >= 0 && Session.Character.Gold >= 1000 * (1 + packet.Type))
                                {
                                    Session.Character.LastPortal = currentRunningSeconds;
                                    Session.Character.Gold -= 1000 * (1 + packet.Type);
                                    Session.SendPacket(Session.Character.GenerateGold());
                                    Session.SendPacket(Session.Character.GenerateAscr());
                                    Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("PVP_ACTIVED_ON_MAP"), 10));
                                    MapCell pos = packet.Type == 0 ? ServerManager.Instance.ArenaInstance.Map.GetRandomPosition() : ServerManager.Instance.FamilyArenaInstance.Map.GetRandomPosition();
                                    ServerManager.Instance.ChangeMapInstance(Session.Character.CharacterId, packet.Type == 0 ? ServerManager.Instance.ArenaInstance.MapInstanceId : ServerManager.Instance.FamilyArenaInstance.MapInstanceId, pos.X, pos.Y);
                                }
                                else
                                {
                                    Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("NOT_ENOUGH_MONEY"), 10));
                                }
                            }
                            break;
                    }
                    break;

                case 18:
                    if (Session.Character.MapInstance.MapInstanceType != MapInstanceType.BaseMapInstance)
                    {
                        return;
                    }
                    Session.SendPacket(Session.Character.GenerateNpcDialog(17));
                    break;

                case 23:
                    if (packet.Type == 0)
                    {
                        if (Session.Character.Group?.SessionCount == 3)
                        {
                            foreach (ClientSession s in Session.Character.Group.Sessions.GetAllItems())
                            {
                                if (s.Character.Family != null)
                                {
                                    Session.SendPacket(UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("GROUP_MEMBER_ALREADY_IN_FAMILY")));
                                    return;
                                }
                            }
                        }
                        if (Session.Character.Group == null || Session.Character.Group.SessionCount != 3)
                        {
                            Session.SendPacket(UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("FAMILY_GROUP_NOT_FULL")));
                            return;
                        }
                        Session.SendPacket(UserInterfaceHelper.GenerateInbox($"#glmk^ {14} 1 {Language.Instance.GetMessageFromKey("CREATE_FAMILY").Replace(' ', '^')}"));
                    }
                    else
                    {
                        if (Session.Character.Family == null)
                        {
                            Session.SendPacket(UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("NOT_IN_FAMILY")));
                            return;
                        }
                        if (Session.Character.Family != null && Session.Character.FamilyCharacter != null && Session.Character.FamilyCharacter.Authority != FamilyAuthority.Head)
                        {
                            Session.SendPacket(UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("NOT_FAMILY_HEAD")));
                            return;
                        }
                        Session.SendPacket($"qna #glrm^1 {Language.Instance.GetMessageFromKey("DISSOLVE_FAMILY")}");
                    }

                    break;

                case 26:
                    tp = npc?.Teleporters?.FirstOrDefault(s => s.Index == packet.Type);
                    if (tp != null)
                    {
                        if (Session.Character.Gold >= 5000 * packet.Type && packet.Type >= 0)
                        {
                            Session.Character.Gold -= 5000 * packet.Type;
                            Session.SendPacket(Session.Character.GenerateGold());
                            ServerManager.Instance.ChangeMap(Session.Character.CharacterId, tp.MapId, (short)(tp.MapX + ServerManager.RandomNumber(-3, 3)), (short)(tp.MapY + ServerManager.RandomNumber(-3, 3)));
                        }
                        else
                        {
                            Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("NOT_ENOUGH_MONEY"), 10));
                        }
                    }
                    break;

                case 45:
                    tp = npc?.Teleporters?.FirstOrDefault(s => s.Index == packet.Type);
                    if (tp != null)
                    {
                        if (Session.Character.Gold >= 500)
                        {
                            Session.Character.Gold -= 500;
                            Session.SendPacket(Session.Character.GenerateGold());
                            ServerManager.Instance.ChangeMap(Session.Character.CharacterId, tp.MapId, (short)(tp.MapX + ServerManager.RandomNumber(-3, 3)), (short)(tp.MapY + ServerManager.RandomNumber(-3, 3)));
                        }
                        else
                        {
                            Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("NOT_ENOUGH_MONEY"), 10));
                        }
                    }
                    break;

                case 60:
                    {
                        if (!Session.Character.CanUseNosBazaar())
                        {
                            Session.SendPacket(UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("INFO_BAZAAR")));
                            return;
                        }

                        MedalType medalType = 0;
                        int time = 0;

                        StaticBonusDTO medal = Session.Character.StaticBonusList.Find(s => s.StaticBonusType == StaticBonusType.BazaarMedalGold || s.StaticBonusType == StaticBonusType.BazaarMedalSilver);

                        if (medal != null)
                        {
                            time = (int)(medal.DateEnd - DateTime.Now).TotalHours;

                            switch (medal.StaticBonusType)
                            {
                                case StaticBonusType.BazaarMedalGold:
                                    medalType = MedalType.Gold;
                                    break;
                                case StaticBonusType.BazaarMedalSilver:
                                    medalType = MedalType.Silver;
                                    break;
                            }
                        }

                        Session.SendPacket($"wopen 32 {(byte)medalType} {time}");
                    }
                    break;

                case 61:
                    if (npc == null)
                    {
                        return;
                    }
                    if (packet.Type == 0)
                    {
                        Session.SendPacket($"qna #n_run^{packet.Runner}^56^{packet.Value}^{packet.NpcId} {Language.Instance.GetMessageFromKey("ASK_TRADE")}");
                    }
                    else
                    {
                        if (Session.Character.Inventory.CountItem(5917) < 1 || Session.Character.Inventory.CountItem(5918) < 1)
                        {
                            Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NOT_ENOUGH_INGREDIENTS"), 0));
                            return;
                        }
                        Session.Character.GiftAdd(5922, 1);
                        Session.Character.Inventory.RemoveItemAmount(5917);
                        Session.Character.Inventory.RemoveItemAmount(5918);
                    }
                    break;

                case 62:
                    if (npc == null)
                    {
                        return;
                    }
                    if (packet.Type == 0)
                    {
                        Session.SendPacket($"qna #n_run^{packet.Runner}^56^{packet.Value}^{packet.NpcId} {Language.Instance.GetMessageFromKey("ASK_TRADE")}");
                    }
                    else
                    {
                        if (Session.Character.Inventory.CountItem(5919) < 1)
                        {
                            Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NOT_ENOUGH_INGREDIENTS"), 0));
                            return;
                        }
                        ServerManager.Instance.ChangeMap(Session.Character.CharacterId, 2536, (short)(26 + ServerManager.RandomNumber(-3, 3)), (short)(31 + ServerManager.RandomNumber(-3, 3)));
                        Session.Character.Inventory.RemoveItemAmount(5919);
                    }
                    break;

                case 65:
                    {
                        if (npc != null)
                        {
                            Session.Character.AddQuest(5514);
                        }
                    }
                    break;

                case 66:
                    {
                        if (npc != null)
                        {
                            Session.Character.AddQuest(5914);
                        }
                    }
                    break;

                case 67:
                    {
                        if (npc != null)
                        {
                            Session.Character.AddQuest(5908);
                        }
                    }
                    break;

                case 68:
                    {
                        if (npc != null)
                        {
                            Session.Character.AddQuest(5919);
                        }
                    }
                    break;

                case 69:
                    if (npc == null)
                    {
                        return;
                    }
                    if (packet.Type == 0)
                    {
                        Session.SendPacket($"qna #n_run^{packet.Runner}^56^{packet.Value}^{packet.NpcId} {Language.Instance.GetMessageFromKey("ASK_TRADE")}");
                    }
                    else
                    {
                        if (Session.Character.Inventory.CountItem(5910) < 5)
                        {
                            Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NOT_ENOUGH_INGREDIENTS"), 0));
                            return;
                        }
                        Session.Character.GiftAdd(5929, 10);
                        Session.Character.Inventory.RemoveItemAmount(5910, 5);
                    }
                    break;

                case 70:
                    if (npc == null)
                    {
                        return;
                    }
                    if (packet.Type == 0)
                    {
                        Session.SendPacket($"qna #n_run^{packet.Runner}^56^{packet.Value}^{packet.NpcId} {Language.Instance.GetMessageFromKey("ASK_TRADE")}");
                    }
                    else
                    {
                        if (Session.Character.Inventory.CountItem(5910) < 90)
                        {
                            Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NOT_ENOUGH_INGREDIENTS"), 0));
                            return;
                        }
                        Session.Character.GiftAdd(5923, 1);
                        Session.Character.Inventory.RemoveItemAmount(5910, 90);
                    }
                    break;

                case 71:
                    if (npc == null)
                    {
                        return;
                    }
                    if (packet.Type == 0)
                    {
                        Session.SendPacket($"qna #n_run^{packet.Runner}^56^{packet.Value}^{packet.NpcId} {Language.Instance.GetMessageFromKey("ASK_TRADE")}");
                    }
                    else
                    {
                        if (Session.Character.Inventory.CountItem(5910) < 300)
                        {
                            Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NOT_ENOUGH_INGREDIENTS"), 0));
                            return;
                        }
                        Session.Character.GiftAdd(5914, 1);
                        Session.Character.Inventory.RemoveItemAmount(5910, 300);
                    }
                    break;

                case 72: // Exchange 10 Yellow Pumpkin Sweets (2322) for a Halloween Costume Scroll (1915)
                    if (npc == null || !ServerManager.Instance.Configuration.HalloweenEvent)
                    {
                        return;
                    }
                    if (packet.Type == 0)
                    {
                        Session.SendPacket($"qna #n_run^{packet.Runner}^56^{packet.Value}^{packet.NpcId} {Language.Instance.GetMessageFromKey("ASK_TRADE")}");
                    }
                    else
                    {
                        if (Session.Character.Inventory.CountItem(2322) < 10)
                        {
                            Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NOT_ENOUGH_INGREDIENTS"), 0));
                            return;
                        }
                        Session.Character.GiftAdd(1915, 1);
                        Session.Character.Inventory.RemoveItemAmount(2322, 10);
                    }
                    break;

                case 73: // Exchange 10 Black Pumpkin Sweets (2324) for a Halloween Costume Scroll (1915)
                    if (npc == null || !ServerManager.Instance.Configuration.HalloweenEvent)
                    {
                        return;
                    }
                    if (packet.Type == 0)
                    {
                        Session.SendPacket($"qna #n_run^{packet.Runner}^56^{packet.Value}^{packet.NpcId} {Language.Instance.GetMessageFromKey("ASK_TRADE")}");
                    }
                    else
                    {
                        if (Session.Character.Inventory.CountItem(2324) < 10)
                        {
                            Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NOT_ENOUGH_INGREDIENTS"), 0));
                            return;
                        }
                        Session.Character.GiftAdd(1915, 1);
                        Session.Character.Inventory.RemoveItemAmount(2324, 10);
                    }
                    break;

                case 74: // Exchange 30 Yellow Pumpkin Sweets (2322) for Jack O'Lantern's Seal (1916)
                    if (npc == null || !ServerManager.Instance.Configuration.HalloweenEvent)
                    {
                        return;
                    }
                    if (packet.Type == 0)
                    {
                        Session.SendPacket($"qna #n_run^{packet.Runner}^56^{packet.Value}^{packet.NpcId} {Language.Instance.GetMessageFromKey("ASK_TRADE")}");
                    }
                    else
                    {
                        if (Session.Character.Inventory.CountItem(2322) < 30)
                        {
                            Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NOT_ENOUGH_INGREDIENTS"), 0));
                            return;
                        }
                        Session.Character.GiftAdd(1916, 1);
                        Session.Character.Inventory.RemoveItemAmount(2322, 30);
                    }
                    break;

                case 75: // Exchange 30 Black Pumpkin Sweets (2324) for Jack O'Lantern's Seal (1916)
                    if (npc == null || !ServerManager.Instance.Configuration.HalloweenEvent)
                    {
                        return;
                    }
                    if (packet.Type == 0)
                    {
                        Session.SendPacket($"qna #n_run^{packet.Runner}^56^{packet.Value}^{packet.NpcId} {Language.Instance.GetMessageFromKey("ASK_TRADE")}");
                    }
                    else
                    {
                        if (Session.Character.Inventory.CountItem(2324) < 30 || npc == null || !ServerManager.Instance.Configuration.HalloweenEvent)
                        {
                            Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NOT_ENOUGH_INGREDIENTS"), 0));
                            return;
                        }
                        Session.Character.GiftAdd(1916, 1);
                        Session.Character.Inventory.RemoveItemAmount(2324, 30);
                    }
                    break;

                case 76: // Exchange Bag of Sweets (1917) for Jack O'Lantern's Seal (1916)
                    if (npc == null || !ServerManager.Instance.Configuration.HalloweenEvent)
                    {
                        return;
                    }
                    if (packet.Type == 0)
                    {
                        Session.SendPacket($"qna #n_run^{packet.Runner}^56^{packet.Value}^{packet.NpcId} {Language.Instance.GetMessageFromKey("ASK_TRADE")}");
                    }
                    else
                    {
                        if (Session.Character.Inventory.CountItem(1917) < 1)
                        {
                            Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NOT_ENOUGH_INGREDIENTS"), 0));
                            return;
                        }
                        Session.Character.GiftAdd(1916, 1);
                        Session.Character.Inventory.RemoveItemAmount(1917, 1);
                    }
                    break;

                case 77:
                    {
                        if (npc != null && ServerManager.Instance.Configuration.HalloweenEvent)
                        {
                            Session.Character.AddQuest(5924);
                        }
                    }
                    break;

                case 78:
                    {
                        if (npc != null && ServerManager.Instance.Configuration.HalloweenEvent)
                        {
                            Session.Character.AddQuest(5926);
                        }
                    }
                    break;

                case 79:
                    {
                        if (npc != null && ServerManager.Instance.Configuration.HalloweenEvent)
                        {
                            Session.Character.AddQuest(5928);
                        }
                    }
                    break;

                case 80:
                    {
                        if (npc != null && ServerManager.Instance.Configuration.HalloweenEvent)
                        {
                            Session.Character.AddQuest(5930);
                        }
                    }
                    break;

                case 81:
                    {
                        if (npc != null && ServerManager.Instance.Configuration.HalloweenEvent)
                        {
                            Session.Character.AddQuest(5922);
                        }
                    }
                    break;

                case 82:
                    {
                        if (npc != null && ServerManager.Instance.Configuration.ChristmasEvent)
                        {
                            Session.Character.AddQuest(5932);
                        }
                    }
                    break;

                case 83:
                    {
                        if (npc != null && ServerManager.Instance.Configuration.ChristmasEvent)
                        {
                            //Session.Character.AddQuest(unknown);
                        }
                    }
                    break;

                case 84:
                    {
                        if (npc == null || !ServerManager.Instance.Configuration.ChristmasEvent)
                        {
                            return;
                        }

                        if (packet.Type == 0)
                        {
                            Session.SendPacket($"qna #n_run^{packet.Runner}^56^{packet.Value}^{packet.NpcId} {Language.Instance.GetMessageFromKey("ASK_TRADE")}");
                        }
                        else
                        {
                            if (Session.Character.Inventory.CountItem(2327) < 30)
                            {
                                Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NOT_ENOUGH_INGREDIENTS"), 0));
                                return;
                            }

                            Session.Character.GiftAdd(5064, 1);
                            Session.Character.Inventory.RemoveItemAmount(2327, 30);
                        }
                    }
                    break;

                case 1897:
                    {
                        try
                        {
                            ItemInstance specialistInstance1 = Session.Character.Inventory.LoadBySlotAndType((byte)EquipmentType.Sp, InventoryType.Wear);
                            if (specialistInstance1 != null)
                            {

                                short[] upsuccess = { 50, 40, 30, 20, 10 };

                                int[] goldprice = { 5000, 10000, 20000, 50000, 100000 };
                                byte[] stoneprice = { 1, 2, 3, 4, 5 };
                                short stonevnum;
                                byte upmode = 1;
                                short SpDamage = 0;
                                short SpDefence = 0;
                                short SpElement = 0;
                                short SpHP = 0;
                                short SpFire = 0;
                                short SpWater = 0;
                                short SpLight = 0;
                                short SpDark = 0;
                                short Fallimenti = 0;
                                short Successi = 0;

                                switch (specialistInstance1.Item.Morph)
                                {
                                    case 2:
                                    case 6:
                                    case 9:
                                    case 12:
                                        stonevnum = 2514;
                                        break;

                                    case 3:
                                    case 4:
                                    case 14:
                                        stonevnum = 2515;
                                        break;

                                    case 5:
                                    case 11:
                                    case 15:
                                        stonevnum = 2516;
                                        break;

                                    case 10:
                                    case 13:
                                    case 7:
                                        stonevnum = 2517;
                                        break;

                                    case 17:
                                    case 18:
                                    case 19:
                                        stonevnum = 2518;
                                        break;

                                    case 20:
                                    case 21:
                                    case 22:
                                        stonevnum = 2519;
                                        break;

                                    case 23:
                                    case 24:
                                    case 25:
                                        stonevnum = 2520;
                                        break;

                                    case 26:
                                    case 27:
                                    case 28:
                                        stonevnum = 2521;
                                        break;

                                    default:
                                        return;
                                }

                                while (Session.Character.Inventory.CountItem(stonevnum) > 0)
                                {
                                    if (specialistInstance1.SpStoneUpgrade > 99)
                                    {
                                        break;
                                    }
                                    if (specialistInstance1.SpStoneUpgrade > 80)
                                    {
                                        upmode = 5;
                                    }
                                    else if (specialistInstance1.SpStoneUpgrade > 60)
                                    {
                                        upmode = 4;
                                    }
                                    else if (specialistInstance1.SpStoneUpgrade > 40)
                                    {
                                        upmode = 3;
                                    }
                                    else if (specialistInstance1.SpStoneUpgrade > 20)
                                    {
                                        upmode = 2;
                                    }

                                    if (Session.Character.Gold < goldprice[upmode - 1])
                                    {
                                        break;
                                    }
                                    if (Session.Character.Inventory.CountItem(stonevnum) < stoneprice[upmode - 1])
                                    {
                                        break;
                                    }
                                    int rnd1 = ServerManager.RandomNumber();
                                    if (rnd1 < upsuccess[upmode - 1])
                                    {
                                        byte type = (byte)ServerManager.RandomNumber(0, 16), count = 1;
                                        if (upmode == 4)
                                        {
                                            count = 2;
                                        }
                                        if (upmode == 5)
                                        {
                                            count = (byte)ServerManager.RandomNumber(3, 6);
                                        }
                                        if (type < 3)
                                        {
                                            specialistInstance1.SpDamage += count;
                                            SpDamage += count;
                                        }
                                        else if (type < 6)
                                        {
                                            specialistInstance1.SpDefence += count;
                                            SpDefence += count;
                                        }
                                        else if (type < 9)
                                        {
                                            specialistInstance1.SpElement += count;
                                            SpElement += count;
                                        }
                                        else if (type < 12)
                                        {
                                            specialistInstance1.SpHP += count;
                                            SpHP += count;
                                        }
                                        else if (type == 12)
                                        {
                                            specialistInstance1.SpFire += count;
                                            SpFire += count;
                                        }
                                        else if (type == 13)
                                        {
                                            specialistInstance1.SpWater += count;
                                            SpWater += count;
                                        }
                                        else if (type == 14)
                                        {
                                            specialistInstance1.SpLight += count;
                                            SpLight += count;
                                        }
                                        else if (type == 15)
                                        {
                                            specialistInstance1.SpDark += count;
                                            SpDark += count;
                                        }
                                        specialistInstance1.SpStoneUpgrade++;
                                        Successi++;
                                    }
                                    else
                                    {
                                        Fallimenti++;
                                    }
                                    Session.SendPacket(specialistInstance1.GenerateInventoryAdd());
                                    Session.Character.Gold -= goldprice[upmode - 1];
                                    Session.SendPacket(Session.Character.GenerateGold());
                                    Session.Character.Inventory.RemoveItemAmount(stonevnum, stoneprice[upmode - 1]);
                                }
                                if (Successi > 0 || Fallimenti > 0)
                                {
                                    Session.CurrentMapInstance.Broadcast(StaticPacketHelper.GenerateEff(UserType.Player, Session.Character.CharacterId, 3005), Session.Character.MapX, Session.Character.MapY);
                                    Session.SendPacket(Session.Character.GenerateSay("---------------Perfection Status---------------", 11));
                                    Session.SendPacket(Session.Character.GenerateSay("Succès: " + Successi, 11));
                                    Session.SendPacket(Session.Character.GenerateSay("Fails: " + Fallimenti, 11));
                                    Session.SendPacket(Session.Character.GenerateSay("Attaque: " + SpDamage, 11));
                                    Session.SendPacket(Session.Character.GenerateSay("Défense: " + SpDefence, 11));
                                    Session.SendPacket(Session.Character.GenerateSay("HP: " + SpHP, 11));
                                    Session.SendPacket(Session.Character.GenerateSay("Element " + SpElement, 11));
                                    Session.SendPacket(Session.Character.GenerateSay("Feu: " + SpFire, 11));
                                    Session.SendPacket(Session.Character.GenerateSay("Eau: " + SpWater, 11));
                                    Session.SendPacket(Session.Character.GenerateSay("Lumière: " + SpLight, 11));
                                    Session.SendPacket(Session.Character.GenerateSay("Obscure: " + SpDark, 11));
                                    Session.SendPacket(Session.Character.GenerateSay("-----------------------------------------------", 11));
                                }
                                else
                                {
                                    Session.SendPacket(Session.Character.GenerateSay("NOT_ENOUGH_ITEM", 10));
                                }
                            }
                            else
                            {
                                Session.SendPacket(Session.Character.GenerateSay("WEAR_SP", 10));
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(ex);
                        }

                    }
                    break;

                case 19440:
                    byte[] upfail1;
                    upfail1 = ItemHelper.R8ItemUpgradeFailRate;
                    upfail1 = ItemHelper.ItemUpgradeFailRate;
                    const short cella = 1014;
                    const short kamiendusz = 1015;
                    const short kamiendusz1 = 1016;
                    const short shield = 1218;
                    int rnd = ServerManager.RandomNumber();

                    ItemInstance eq1 =
            Session.Character.Inventory.LoadBySlotAndType((byte)EquipmentType.MainWeapon, InventoryType.Wear);
                    {
                        if (eq1 != null)
                        {
                            for (int i = 0; i < 25; i++)
                            {
                                int rndvalue = ServerManager.RandomNumber(0, 100);
                                {
                                    if (eq1.Upgrade == 0)
                                    {
                                        if (Session.Character.Inventory.CountItem(cella) >= 20 && Session.Character.Inventory.CountItem(kamiendusz) >= 1 && Session.Character.Gold >= 500 && Session.Character.Inventory.CountItem(shield) >= 1)
                                        {
                                            Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("UPGRADE_SUCCESS"), 12));
                                            Session.CurrentMapInstance.Broadcast(StaticPacketHelper.GenerateEff(UserType.Player, Session.Character.CharacterId, 3005), Session.Character.MapX, Session.Character.MapY);
                                            eq1.Upgrade = 1;
                                            Session.Character.Inventory.RemoveItemAmount(cella, 20);
                                            Session.Character.Inventory.RemoveItemAmount(kamiendusz, 1);
                                            Session.Character.Inventory.RemoveItemAmount(shield, 1);
                                            Session.Character.Gold -= 500;
                                            Session.SendPacket(Session.Character.GenerateGold());
                                        }
                                    }

                                    else if (eq1.Upgrade == 1)
                                    {
                                        if (Session.Character.Inventory.CountItem(cella) >= 50 && Session.Character.Inventory.CountItem(kamiendusz) >= 1 && Session.Character.Gold >= 1500 && Session.Character.Inventory.CountItem(shield) >= 1)
                                        {
                                            Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("UPGRADE_SUCCESS"), 12));
                                            Session.CurrentMapInstance.Broadcast(StaticPacketHelper.GenerateEff(UserType.Player, Session.Character.CharacterId, 3005), Session.Character.MapX, Session.Character.MapY);
                                            eq1.Upgrade = 2;
                                            Session.Character.Inventory.RemoveItemAmount(cella, 50);
                                            Session.Character.Inventory.RemoveItemAmount(kamiendusz, 1);
                                            Session.Character.Inventory.RemoveItemAmount(shield, 1);
                                            Session.Character.Gold -= 1500;
                                            Session.SendPacket(Session.Character.GenerateGold());
                                        }
                                    }
                                    else if (eq1.Upgrade == 2)
                                    {
                                        if (Session.Character.Inventory.CountItem(cella) >= 80 && Session.Character.Inventory.CountItem(kamiendusz) >= 2 && Session.Character.Gold >= 3000 && Session.Character.Inventory.CountItem(shield) >= 1)
                                        {
                                            if (rndvalue <= 90)
                                            {
                                                Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("UPGRADE_SUCCESS"), 12));
                                                Session.CurrentMapInstance.Broadcast(StaticPacketHelper.GenerateEff(UserType.Player, Session.Character.CharacterId, 3005), Session.Character.MapX, Session.Character.MapY);
                                                eq1.Upgrade = 3;
                                                Session.Character.Inventory.RemoveItemAmount(cella, 80);
                                                Session.Character.Inventory.RemoveItemAmount(kamiendusz, 2);
                                                Session.Character.Inventory.RemoveItemAmount(shield, 1);
                                                Session.Character.Gold -= 3000;
                                                Session.SendPacket(Session.Character.GenerateGold());
                                            }
                                            else
                                            {
                                                Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("UPGRADE_FALSE"), 11));
                                                Session.CurrentMapInstance.Broadcast(StaticPacketHelper.GenerateEff(UserType.Player, Session.Character.CharacterId, 3004), Session.Character.MapX, Session.Character.MapY);
                                                Session.Character.Inventory.RemoveItemAmount(cella, 80);
                                                Session.Character.Inventory.RemoveItemAmount(kamiendusz, 2);
                                                Session.Character.Inventory.RemoveItemAmount(shield, 1);
                                                Session.Character.Gold -= 3000;
                                                Session.SendPacket(Session.Character.GenerateGold());
                                            }
                                        }
                                        else
                                        {
                                            Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NOT_ENOUGHT_ITEM"), 0));
                                            break;
                                        }
                                    }
                                    else if (eq1.Upgrade == 3)
                                    {
                                        if (Session.Character.Inventory.CountItem(cella) >= 120 && Session.Character.Inventory.CountItem(kamiendusz) >= 2 && Session.Character.Gold >= 10000 && Session.Character.Inventory.CountItem(shield) >= 1)
                                        {
                                            if (rndvalue <= 80)
                                            {
                                                Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("UPGRADE_SUCCESS"), 12));
                                                Session.CurrentMapInstance.Broadcast(StaticPacketHelper.GenerateEff(UserType.Player, Session.Character.CharacterId, 3005), Session.Character.MapX, Session.Character.MapY);
                                                eq1.Upgrade = 4;
                                                Session.Character.Inventory.RemoveItemAmount(cella, 120);
                                                Session.Character.Inventory.RemoveItemAmount(kamiendusz, 2);
                                                Session.Character.Inventory.RemoveItemAmount(shield, 1);
                                                Session.Character.Gold -= 10000;
                                                Session.SendPacket(Session.Character.GenerateGold());
                                            }
                                            else
                                            {
                                                Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("UPGRADE_FALSE"), 11));
                                                Session.CurrentMapInstance.Broadcast(StaticPacketHelper.GenerateEff(UserType.Player, Session.Character.CharacterId, 3004), Session.Character.MapX, Session.Character.MapY);
                                                Session.Character.Inventory.RemoveItemAmount(cella, 120);
                                                Session.Character.Inventory.RemoveItemAmount(kamiendusz, 2);
                                                Session.Character.Inventory.RemoveItemAmount(shield, 1);
                                                Session.Character.Gold -= 10000;
                                                Session.SendPacket(Session.Character.GenerateGold());
                                            }
                                        }
                                        else
                                        {
                                            Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NOT_ENOUGHT_ITEM"), 0));
                                            break;
                                        }
                                    }
                                    else if (eq1.Upgrade == 4)
                                    {
                                        if (Session.Character.Inventory.CountItem(cella) >= 160 && Session.Character.Inventory.CountItem(kamiendusz) >= 3 && Session.Character.Gold >= 30000 && Session.Character.Inventory.CountItem(shield) >= 1)
                                        {
                                            if (rndvalue <= 60)
                                            {
                                                Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("UPGRADE_SUCCESS"), 12));
                                                Session.CurrentMapInstance.Broadcast(StaticPacketHelper.GenerateEff(UserType.Player, Session.Character.CharacterId, 3005), Session.Character.MapX, Session.Character.MapY);
                                                eq1.Upgrade = 5;
                                                Session.Character.Inventory.RemoveItemAmount(cella, 160);
                                                Session.Character.Inventory.RemoveItemAmount(kamiendusz, 3);
                                                Session.Character.Inventory.RemoveItemAmount(shield, 1);
                                                Session.Character.Gold -= 30000;
                                                Session.SendPacket(Session.Character.GenerateGold());
                                            }
                                            else
                                            {
                                                Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("UPGRADE_FALSE"), 11));
                                                Session.CurrentMapInstance.Broadcast(StaticPacketHelper.GenerateEff(UserType.Player, Session.Character.CharacterId, 3004), Session.Character.MapX, Session.Character.MapY);
                                                Session.Character.Inventory.RemoveItemAmount(cella, 160);
                                                Session.Character.Inventory.RemoveItemAmount(kamiendusz, 3);
                                                Session.Character.Inventory.RemoveItemAmount(shield, 1);
                                                Session.Character.Gold -= 30000;
                                                Session.SendPacket(Session.Character.GenerateGold());
                                            }
                                        }
                                        else
                                        {
                                            Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NOT_ENOUGHT_ITEM"), 0));
                                            break;
                                        }
                                    }
                                    else if (eq1.Upgrade == 5)
                                    {
                                        if (Session.Character.Inventory.CountItem(cella) >= 200 && Session.Character.Inventory.CountItem(kamiendusz1) >= 1 && Session.Character.Gold >= 80000 && Session.Character.Inventory.CountItem(shield) >= 1)
                                        {
                                            if (rndvalue <= 40)
                                            {
                                                Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("UPGRADE_SUCCESS"), 12));
                                                Session.CurrentMapInstance.Broadcast(StaticPacketHelper.GenerateEff(UserType.Player, Session.Character.CharacterId, 3005), Session.Character.MapX, Session.Character.MapY);
                                                eq1.Upgrade = 6;
                                                Session.Character.Inventory.RemoveItemAmount(cella, 200);
                                                Session.Character.Inventory.RemoveItemAmount(kamiendusz1, 1);
                                                Session.Character.Inventory.RemoveItemAmount(shield, 1);
                                                Session.Character.Gold -= 80000;
                                                Session.SendPacket(Session.Character.GenerateGold());
                                            }
                                            else
                                            {
                                                Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("UPGRADE_FALSE"), 11));
                                                Session.CurrentMapInstance.Broadcast(StaticPacketHelper.GenerateEff(UserType.Player, Session.Character.CharacterId, 3004), Session.Character.MapX, Session.Character.MapY);
                                                Session.Character.Inventory.RemoveItemAmount(cella, 200);
                                                Session.Character.Inventory.RemoveItemAmount(kamiendusz1, 1);
                                                Session.Character.Inventory.RemoveItemAmount(shield, 1);
                                                Session.Character.Gold -= 80000;
                                                Session.SendPacket(Session.Character.GenerateGold());
                                            }
                                        }
                                        else
                                        {
                                            Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NOT_ENOUGHT_ITEM"), 0));
                                            break;
                                        }
                                    }
                                    else if (eq1.Upgrade == 6)
                                    {
                                        if (Session.Character.Inventory.CountItem(cella) >= 280 && Session.Character.Inventory.CountItem(kamiendusz1) >= 1 && Session.Character.Gold >= 150000 && Session.Character.Inventory.CountItem(shield) >= 1)
                                        {
                                            if (rndvalue <= 20)
                                            {
                                                Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("UPGRADE_SUCCESS"), 12));
                                                Session.CurrentMapInstance.Broadcast(StaticPacketHelper.GenerateEff(UserType.Player, Session.Character.CharacterId, 3005), Session.Character.MapX, Session.Character.MapY);
                                                eq1.Upgrade = 7;
                                                Session.Character.Inventory.RemoveItemAmount(cella, 280);
                                                Session.Character.Inventory.RemoveItemAmount(kamiendusz1, 1);
                                                Session.Character.Inventory.RemoveItemAmount(shield, 1);
                                                Session.Character.Gold -= 150000;
                                                Session.SendPacket(Session.Character.GenerateGold());
                                            }
                                            else
                                            {
                                                Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("UPGRADE_FALSE"), 11));
                                                Session.CurrentMapInstance.Broadcast(StaticPacketHelper.GenerateEff(UserType.Player, Session.Character.CharacterId, 3004), Session.Character.MapX, Session.Character.MapY);
                                                Session.Character.Inventory.RemoveItemAmount(cella, 280);
                                                Session.Character.Inventory.RemoveItemAmount(kamiendusz1, 1);
                                                Session.Character.Inventory.RemoveItemAmount(shield, 1);
                                                Session.Character.Gold -= 150000;
                                                Session.SendPacket(Session.Character.GenerateGold());
                                            }
                                        }
                                        else
                                        {
                                            Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NOT_ENOUGHT_ITEM"), 0));
                                            break;
                                        }
                                    }
                                    else if (eq1.Upgrade == 7)
                                    {
                                        if (Session.Character.Inventory.CountItem(cella) >= 380 && Session.Character.Inventory.CountItem(kamiendusz1) >= 2 && Session.Character.Gold >= 400000 && Session.Character.Inventory.CountItem(shield) >= 1)
                                        {
                                            if (rndvalue <= 10)
                                            {
                                                Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("UPGRADE_SUCCESS"), 12));
                                                Session.CurrentMapInstance.Broadcast(StaticPacketHelper.GenerateEff(UserType.Player, Session.Character.CharacterId, 3005), Session.Character.MapX, Session.Character.MapY);
                                                eq1.Upgrade = 8;
                                                Session.Character.Inventory.RemoveItemAmount(cella, 380);
                                                Session.Character.Inventory.RemoveItemAmount(kamiendusz1, 2);
                                                Session.Character.Inventory.RemoveItemAmount(shield, 1);
                                                Session.Character.Gold -= 400000;
                                                Session.SendPacket(Session.Character.GenerateGold());
                                            }
                                            else
                                            {
                                                Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("UPGRADE_FALSE"), 11));
                                                Session.CurrentMapInstance.Broadcast(StaticPacketHelper.GenerateEff(UserType.Player, Session.Character.CharacterId, 3004), Session.Character.MapX, Session.Character.MapY);
                                                Session.Character.Inventory.RemoveItemAmount(cella, 380);
                                                Session.Character.Inventory.RemoveItemAmount(kamiendusz1, 2);
                                                Session.Character.Inventory.RemoveItemAmount(shield, 1);
                                                Session.Character.Gold -= 400000;
                                                Session.SendPacket(Session.Character.GenerateGold());
                                            }
                                        }
                                        else
                                        {
                                            Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NOT_ENOUGHT_ITEM"), 0));
                                            break;
                                        }
                                    }
                                    else if (eq1.Upgrade == 8)
                                    {
                                        if (Session.Character.Inventory.CountItem(cella) >= 480 && Session.Character.Inventory.CountItem(kamiendusz1) >= 2 && Session.Character.Gold >= 700000 && Session.Character.Inventory.CountItem(shield) >= 1)
                                        {
                                            if (rndvalue <= 5)
                                            {
                                                Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("UPGRADE_SUCCESS"), 12));
                                                Session.CurrentMapInstance.Broadcast(StaticPacketHelper.GenerateEff(UserType.Player, Session.Character.CharacterId, 3005), Session.Character.MapX, Session.Character.MapY);
                                                eq1.Upgrade = 9;
                                                Session.Character.Inventory.RemoveItemAmount(cella, 480);
                                                Session.Character.Inventory.RemoveItemAmount(kamiendusz1, 2);
                                                Session.Character.Inventory.RemoveItemAmount(shield, 1);
                                                Session.Character.Gold -= 700000;
                                                Session.SendPacket(Session.Character.GenerateGold());
                                            }
                                            else
                                            {
                                                Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("UPGRADE_FALSE"), 11));
                                                Session.CurrentMapInstance.Broadcast(StaticPacketHelper.GenerateEff(UserType.Player, Session.Character.CharacterId, 3004), Session.Character.MapX, Session.Character.MapY);
                                                Session.Character.Inventory.RemoveItemAmount(cella, 480);
                                                Session.Character.Inventory.RemoveItemAmount(kamiendusz1, 2);
                                                Session.Character.Inventory.RemoveItemAmount(shield, 1);
                                                Session.Character.Gold -= 700000;
                                                Session.SendPacket(Session.Character.GenerateGold());
                                            }
                                        }
                                        else
                                        {
                                            Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NOT_ENOUGHT_ITEM"), 0));
                                            break;
                                        }
                                    }
                                    else if (eq1.Upgrade == 9)
                                    {
                                        if (Session.Character.Inventory.CountItem(cella) >= 600 && Session.Character.Inventory.CountItem(kamiendusz1) >= 3 && Session.Character.Gold >= 1000000 && Session.Character.Inventory.CountItem(shield) >= 1)
                                        {
                                            if (rndvalue <= 3)
                                            {
                                                Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("UPGRADE_SUCCESS"), 12));
                                                Session.CurrentMapInstance.Broadcast(StaticPacketHelper.GenerateEff(UserType.Player, Session.Character.CharacterId, 3005), Session.Character.MapX, Session.Character.MapY);
                                                eq1.Upgrade = 10;
                                                Session.Character.Inventory.RemoveItemAmount(cella, 600);
                                                Session.Character.Inventory.RemoveItemAmount(kamiendusz1, 3);
                                                Session.Character.Inventory.RemoveItemAmount(shield, 1);
                                                Session.Character.Gold -= 1000000;
                                                Session.SendPacket(Session.Character.GenerateGold());
                                            }
                                            else
                                            {
                                                Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("UPGRADE_FALSE"), 11));
                                                Session.CurrentMapInstance.Broadcast(StaticPacketHelper.GenerateEff(UserType.Player, Session.Character.CharacterId, 3004), Session.Character.MapX, Session.Character.MapY);
                                                Session.Character.Inventory.RemoveItemAmount(cella, 600);
                                                Session.Character.Inventory.RemoveItemAmount(kamiendusz1, 3);
                                                Session.Character.Inventory.RemoveItemAmount(shield, 1);
                                                Session.Character.Gold -= 1000000;
                                                Session.SendPacket(Session.Character.GenerateGold());
                                            }
                                        }
                                        else
                                        {
                                            Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NOT_ENOUGHT_ITEM"), 0));
                                            break;
                                        }
                                    }
                                    else if (eq1.Upgrade == 10)
                                    {
                                        Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("EQ_MAX_UPGRADE"), 0));
                                        break;
                                    }
                                    else if (rnd < upfail1[Upgrade])
                                    {
                                        Logger.LogUserEvent("UPGRADE_ITEM", Session.GenerateIdentity(), $"[UpgradeItem]ItemType: {eq1.Item.ItemType} Protection: {protection.ToString()} IIId: {Id} Upgrade: {eq1.Upgrade} Result: Fail");

                                        if (protection == UpgradeProtection.None)
                                        {
                                            Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("UPGRADE_FAILED"), 11));
                                            Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("UPGRADE_FAILED"), 0));
                                        }
                                        else
                                        {
                                            Session.CurrentMapInstance.Broadcast(StaticPacketHelper.GenerateEff(UserType.Player, Session.Character.CharacterId, 3004), Session.Character.PositionX, Session.Character.PositionY);
                                            Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("SCROLL_PROTECT_USED"), 11));
                                            Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("UPGRADE_FAILED_ITEM_SAVED"), 0));
                                        }
                                    }
                                }

                            }
                        }
                        else
                        {
                            Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("WEAPON_CANT_BENULL"), 0));
                        }
                    }
                    break;


                case 85:
                    {
                        if (npc == null || !ServerManager.Instance.Configuration.ChristmasEvent)
                        {
                            return;
                        }

                        if (packet.Type == 0)
                        {
                            Session.SendPacket($"qna #n_run^{packet.Runner}^56^{packet.Value}^{packet.NpcId} {Language.Instance.GetMessageFromKey("ASK_TRADE")}");
                        }
                        else
                        {
                            if (Session.Character.Inventory.CountItem(2326) < 30)
                            {
                                Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NOT_ENOUGH_INGREDIENTS"), 0));
                                return;
                            }

                            Session.Character.GiftAdd(5064, 1);
                            Session.Character.Inventory.RemoveItemAmount(2326, 30);
                        }
                    }
                    break;

                case 86:
                    {
                        if (npc == null || !ServerManager.Instance.Configuration.ChristmasEvent)
                        {
                            return;
                        }

                        if (packet.Type == 0)
                        {
                            Session.SendPacket($"qna #n_run^{packet.Runner}^56^{packet.Value}^{packet.NpcId} {Language.Instance.GetMessageFromKey("ASK_TRADE")}");
                        }
                        else
                        {
                            if (Session.Character.Inventory.CountItem(2327) < 30)
                            {
                                Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NOT_ENOUGH_INGREDIENTS"), 0));
                                return;
                            }

                            Session.Character.GiftAdd(1371, 1);
                            Session.Character.Inventory.RemoveItemAmount(2327, 30);
                        }
                    }
                    break;

                case 87:
                    {
                        if (npc == null || !ServerManager.Instance.Configuration.ChristmasEvent)
                        {
                            return;
                        }

                        if (packet.Type == 0)
                        {
                            Session.SendPacket($"qna #n_run^{packet.Runner}^56^{packet.Value}^{packet.NpcId} {Language.Instance.GetMessageFromKey("ASK_TRADE")}");
                        }
                        else
                        {
                            if (Session.Character.Inventory.CountItem(2326) < 30)
                            {
                                Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NOT_ENOUGH_INGREDIENTS"), 0));
                                return;
                            }

                            Session.Character.GiftAdd(1371, 1);
                            Session.Character.Inventory.RemoveItemAmount(2326, 30);
                        }
                    }
                    break;

                case 88:
                    {
                        if (npc == null || !ServerManager.Instance.Configuration.ChristmasEvent)
                        {
                            return;
                        }

                        if (packet.Type == 0)
                        {
                            Session.SendPacket($"qna #n_run^{packet.Runner}^56^{packet.Value}^{packet.NpcId} {Language.Instance.GetMessageFromKey("ASK_TRADE")}");
                        }
                        else
                        {
                            if (Session.Character.Inventory.CountItem(1367) < 5)
                            {
                                Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NOT_ENOUGH_INGREDIENTS"), 0));
                                return;
                            }

                            Session.Character.GiftAdd(5206, 1);
                            Session.Character.Inventory.RemoveItemAmount(1367, 5);
                        }
                    }
                    break;

                case 89:
                    {
                        if (npc != null && ServerManager.Instance.Configuration.ChristmasEvent)
                        {
                            Session.Character.AddQuest(5934);
                        }
                    }
                    break;

                case 90:
                    {
                        if (npc != null && ServerManager.Instance.Configuration.ChristmasEvent)
                        {
                            Session.Character.AddQuest(5936);
                        }
                    }
                    break;

                case 91:
                    {
                        if (npc != null && ServerManager.Instance.Configuration.ChristmasEvent)
                        {
                            Session.Character.AddQuest(5938);
                        }
                    }
                    break;

                case 92:
                    {
                        if (npc != null && ServerManager.Instance.Configuration.ChristmasEvent)
                        {
                            Session.Character.AddQuest(5940);
                        }
                    }
                    break;

                case 93:
                    {
                        if (npc != null && ServerManager.Instance.Configuration.ChristmasEvent)
                        {
                            Session.Character.AddQuest(5942);
                        }
                    }
                    break;

                case 94:
                    // Quest Easter Mimi 
                    if (npc == null)
                    {
                        return;
                    }
                    if (npc != null && ServerManager.Instance.Configuration.EasterEvent)
                    {
                        Session.Character.AddQuest(5946);
                    }
                    break;

                case 95:
                    if (npc == null)
                    {
                        return;
                    }
                    // 5 GoldenEggs vs 1 Box
                    const short GoldenEggs = 5258;
                    const short BoxPascal = 5261;
                    switch (packet.Type)
                    {
                        case 0:
                            Session.SendPacket($"qna #n_run^{packet.Runner}^61^{packet.Value}^{packet.NpcId} {Language.Instance.GetMessageFromKey("EXCHANGE_MATERIAL")}");
                            break;
                        case 61:
                            if (Session.Character.Inventory.CountItem(GoldenEggs) <= 5)
                            {
                                // No GoldenEggs                   
                                Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("NOT_ENOUGH_INGREDIENT"), 11));
                                return;
                            }
                            Session.Character.GiftAdd(BoxPascal, 1);
                            Session.Character.Inventory.RemoveItemAmount(GoldenEggs, 5);
                            break;
                    }
                    break;

                case 96:
                    if (npc == null)
                    {
                        return;
                    }
                    // 30 Rabbits vs 1 Seal Chicken king
                    const short ChocolateRabbits = 2405;
                    const short SealChik = 5109;
                    switch (packet.Type)
                    {
                        case 0:
                            Session.SendPacket($"qna #n_run^{packet.Runner}^61^{packet.Value}^{packet.NpcId} {Language.Instance.GetMessageFromKey("EXCHANGE_MATERIAL")}");
                            break;
                        case 61:
                            if (Session.Character.Inventory.CountItem(ChocolateRabbits) <= 30)
                            {
                                // No Lapin                  
                                Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("NOT_ENOUGH_INGREDIENT"), 11));
                                return;
                            }
                            Session.Character.GiftAdd(SealChik, 1);
                            Session.Character.Inventory.RemoveItemAmount(ChocolateRabbits, 30);
                            break;
                    }
                    break;

                case 97:
                    // Quest Easter Slugg
                    if (npc == null)
                    {
                        return;
                    }
                    if (npc != null && ServerManager.Instance.Configuration.EasterEvent)
                    {
                        Session.Character.AddQuest(5948);
                    }
                    break;

                case 98:
                    // Quest Easter Calvin
                    if (npc == null)
                    {
                        return;
                    }

                    if (npc != null && ServerManager.Instance.Configuration.EasterEvent)
                    {
                        Session.Character.AddQuest(5950);
                    }
                    break;

                case 99:
                    // Quest Eva Easter
                    if (npc == null)
                    {
                        return;
                    }
                    if (npc != null && ServerManager.Instance.Configuration.EasterEvent)
                    {
                        Session.Character.AddQuest(5953);
                    }
                    break;

                case 100:
                    // Quest Easter Malcolm
                    if (npc == null)
                    {
                        return;
                    }
                    if (npc != null && ServerManager.Instance.Configuration.EasterEvent)
                    {
                        Session.Character.AddQuest(5945);
                    }
                    break;

                case 110:
                    {
                        if (npc != null)
                        {
                            Session.Character.AddQuest(5954);
                        }
                    }
                    break;

                case 111:
                    if (npc == null)
                    {
                        return;
                    }
                    if (packet.Type == 0)
                    {
                        Session.SendPacket($"qna #n_run^{packet.Runner}^56^{packet.Value}^{packet.NpcId} {Language.Instance.GetMessageFromKey("ASK_TRADE")}");
                    }
                    else
                    {
                        if (Session.Character.Inventory.CountItem(1012) < 20 || Session.Character.Inventory.CountItem(1013) < 20 || Session.Character.Inventory.CountItem(1027) < 20)
                        {
                            Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NOT_ENOUGH_INGREDIENTS"), 0));
                            return;
                        }
                        Session.Character.GiftAdd(5500, 1);
                        Session.Character.Inventory.RemoveItemAmount(1012, 20);
                        Session.Character.Inventory.RemoveItemAmount(1013, 20);
                        Session.Character.Inventory.RemoveItemAmount(1027, 20);
                    }
                    break;

                    //Flake Christmas quest
                case 128:
                    {
                        if (npc != null && ServerManager.Instance.Configuration.ChristmasEvent)
                        {
                            Session.Character.AddQuest(5972);
                        }
                    }
                    break;

                case 129:
                    if (!ServerManager.Instance.Configuration.ChristmasEvent)
                    {
                        return;
                    }
                    if (npc == null)
                    {
                        return;
                    }
                    if (packet.Type == 0)
                    {
                        Session.SendPacket($"qna #n_run^{packet.Runner}^56^{packet.Value}^{packet.NpcId} {Language.Instance.GetMessageFromKey("ASK_TRADE")}");
                    }
                    else
                    {
                        if (Session.Character.Inventory.CountItem(1611) < 10 || Session.Character.Inventory.CountItem(1612) < 10
                         || Session.Character.Inventory.CountItem(1613) < 20 || Session.Character.Inventory.CountItem(1614) < 10)
                        {
                            Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NOT_ENOUGH_INGREDIENTS"), 0));
                            return;
                        }
                        Session.Character.GiftAdd(1621, 10);
                        Session.Character.Inventory.RemoveItemAmount(1611, 10);
                        Session.Character.Inventory.RemoveItemAmount(1612, 10);
                        Session.Character.Inventory.RemoveItemAmount(1613, 20);
                        Session.Character.Inventory.RemoveItemAmount(1614, 10);
                    }
                    break;

                case 130:
                    if (!ServerManager.Instance.Configuration.ChristmasEvent)
                    {
                        return;
                    }
                    if (npc == null)
                    {
                        return;
                    }
                    if (packet.Type == 0)
                    {
                        Session.SendPacket($"qna #n_run^{packet.Runner}^56^{packet.Value}^{packet.NpcId} {Language.Instance.GetMessageFromKey("ASK_TRADE")}");
                    }
                    else
                    {
                        if (Session.Character.Inventory.CountItem(1615) < 10 || Session.Character.Inventory.CountItem(1616) < 20 || Session.Character.Inventory.CountItem(1617) < 10
                         || Session.Character.Inventory.CountItem(1618) < 10 || Session.Character.Inventory.CountItem(1619) < 10 || Session.Character.Inventory.CountItem(1620) < 10)
                        {
                            Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NOT_ENOUGH_INGREDIENTS"), 0));
                            return;
                        }
                        Session.Character.GiftAdd(1622, 10);
                        Session.Character.Inventory.RemoveItemAmount(1615, 10);
                        Session.Character.Inventory.RemoveItemAmount(1616, 20);
                        Session.Character.Inventory.RemoveItemAmount(1617, 10);
                        Session.Character.Inventory.RemoveItemAmount(1618, 10);
                        Session.Character.Inventory.RemoveItemAmount(1619, 10);
                        Session.Character.Inventory.RemoveItemAmount(1620, 10);
                    }
                    break;

                case 131:
                    {
                        if (npc != null)
                        {
                            Session.Character.AddQuest(5982);
                        }
                    }
                    break;

                case 132:
                    tp = npc?.Teleporters?.FirstOrDefault(s => s.Index == packet.Type);
                    if (tp != null)
                    {
                        ServerManager.Instance.ChangeMap(Session.Character.CharacterId, tp.MapId, (short)(tp.MapX + ServerManager.RandomNumber(-3, 3)), (short)(tp.MapY + ServerManager.RandomNumber(-3, 3)));
                    }
                    break;

                case 133:
                    if (npc == null)
                    {
                        return;
                    }
                    if (packet.Type == 0)
                    {
                        Session.SendPacket($"qna #n_run^{packet.Runner}^56^{packet.Value}^{packet.NpcId} {Language.Instance.GetMessageFromKey("ASK_TRADE")}");
                    }
                    else
                    {
                        if (Session.Character.Inventory.CountItem(1012) < 20 || Session.Character.Inventory.CountItem(2307) < 20 || Session.Character.Inventory.CountItem(5911) < 20)
                        {
                            Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NOT_ENOUGH_INGREDIENTS"), 0));
                            return;
                        }
                        Session.Character.GiftAdd(5512, 1);
                        Session.Character.Inventory.RemoveItemAmount(1012, 20);
                        Session.Character.Inventory.RemoveItemAmount(2307, 20);
                        Session.Character.Inventory.RemoveItemAmount(5911, 20);
                    }
                    break;

                case 134:
                    if (npc == null || !Session.Character.Quests.Any(s => s.Quest.QuestObjectives.Any(o => o.SpecialData == 5518)))
                    {
                        return;
                    }
                    short vNum = 0;
                    for (short i = 4494; i <= 4496; i++)
                    {
                        if (Session.Character.Inventory.CountItem(i) > 0)
                        {
                            vNum = i;
                            break;
                        }
                    }
                    if (vNum > 0)
                    {
                        Session.Character.GiftAdd(5518, 1);
                        Session.Character.GiftAdd(4504, 1);
                        Session.Character.Inventory.RemoveItemAmount(vNum, 1);
                    }
                    else
                    {
                        Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NOT_ENOUGH_INGREDIENTS"), 0));
                    }
                    break;

                case 135:
                    if (!ServerManager.Instance.StartedEvents.Contains(EventType.TALENTARENA))
                    {
                        TimeSpan time = ServerManager.Instance.Schedules.ToList().FirstOrDefault(s => s.Event == EventType.TALENTARENA)?.Time ?? TimeSpan.FromSeconds(0);
                        Session.SendPacket(npc?.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("ARENA_NOT_OPEN"), string.Format("{0:D2}:{1:D2} - {2:D2}:{3:D2}", time.Hours, time.Minutes, (time.Hours + 4) % 24, time.Minutes)), 10));
                    }
                    else
                    {
                        if (Session.Character.Level < 30)
                        {
                            Session.SendPacket(UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("LOW_LVL_30")));
                            return;
                        }

                        var tickets = 20 - Session.Character.GeneralLogs.CountLinq(s => s.LogType == "TalentArena" && s.Timestamp.Date == DateTime.Today);

                        if (tickets > 0)
                        {
                            if (ServerManager.Instance.ArenaMembers.ToList().All(s => s.Session != Session))
                            {
                                if (ServerManager.Instance.IsCharacterMemberOfGroup(Session.Character.CharacterId))
                                {
                                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("TALENT_ARENA_GROUP"), 0));
                                    Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("TALENT_ARENA_GROUP"), 10));
                                }
                                else
                                {
                                    Session.SendPacket(Session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("ARENA_TICKET_LEFT"), tickets), 10));

                                    ServerManager.Instance.ArenaMembers.Add(new ArenaMember
                                    {
                                        ArenaType = EventType.TALENTARENA,
                                        Session = Session,
                                        GroupId = null,
                                        Time = 0
                                    });
                                }
                            }
                        }
                        else
                        {
                            Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("TALENT_ARENA_NO_MORE_TICKET"), 0));
                            Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("TALENT_ARENA_NO_MORE_TICKET"), 10));
                        }
                    }
                    break;

                case 137:
                    Session.SendPacket("taw_open");
                    break;

                case 138:
                    ConcurrentBag<ArenaTeamMember> at = ServerManager.Instance.ArenaTeams.ToList().Where(s => s.Any(c => c.Session?.CurrentMapInstance != null)).OrderBy(s => rand.Next()).FirstOrDefault();
                    if (at != null)
                    {
                        ServerManager.Instance.ChangeMapInstance(Session.Character.CharacterId, at.FirstOrDefault().Session.CurrentMapInstance.MapInstanceId, (short)(69 + ServerManager.RandomNumber(-2, 2)), (short)(100 + ServerManager.RandomNumber(-2, 2)));

                        var zenas = at.OrderBy(s => s.Order).FirstOrDefault(s => s.Session != null && !s.Dead && s.ArenaTeamType == ArenaTeamType.ZENAS);
                        var erenia = at.OrderBy(s => s.Order).FirstOrDefault(s => s.Session != null && !s.Dead && s.ArenaTeamType == ArenaTeamType.ERENIA);
                        Session.SendPacket(erenia?.Session?.Character?.GenerateTaM(0));
                        Session.SendPacket(erenia?.Session?.Character?.GenerateTaM(3));
                        Session.SendPacket("taw_sv 0");
                        Session.SendPacket(zenas?.Session?.Character?.GenerateTaP(0, true));
                        Session.SendPacket(erenia?.Session?.Character?.GenerateTaP(2, true));
                        Session.SendPacket(zenas?.Session?.Character?.GenerateTaFc(0));
                        Session.SendPacket(erenia?.Session?.Character?.GenerateTaFc(1));
                    }
                    else
                    {
                        Session.SendPacket(UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("NO_TEAM_ARENA")));
                    }

                    break;


                case 19:
                case 144:
                    if (Session.Character.Timespace != null)
                    {
                        if (Session.Character.MapInstance.InstanceBag.EndState == 10)
                        {
                            EventHelper.Instance.RunEvent(new EventContainer(Session.Character.MapInstance, EventActionType.SCRIPTEND, (byte)5));
                        }
                    }
                    break;

                case 145:
                    if (npc == null)
                    {
                        return;
                    }
                    if (packet.Type == 0)
                    {
                        Session.SendPacket($"qna #n_run^{packet.Runner}^56^{packet.Value}^{packet.NpcId} {Language.Instance.GetMessageFromKey("ASK_TRADE")}");
                    }
                    else
                    {
                        if (Session.Character.Inventory.CountItem(2522) < 50)
                        {
                            Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NOT_ENOUGH_INGREDIENTS"), 0));
                            return;
                        }
                        switch (Session.Character.Class)
                        {
                            case ClassType.Swordsman:
                                Session.Character.GiftAdd(4500, 1);
                                break;

                            case ClassType.Archer:
                                Session.Character.GiftAdd(4501, 1);
                                break;

                            case ClassType.Magician:
                                Session.Character.GiftAdd(4502, 1);
                                break;
                        }
                        Session.Character.Inventory.RemoveItemAmount(2522, 50);
                    }
                    break;

                case 146:
                    if (npc == null)
                    {
                        return;
                    }
                    if (packet.Type == 0)
                    {
                        Session.SendPacket($"qna #n_run^{packet.Runner}^56^{packet.Value}^{packet.NpcId} {Language.Instance.GetMessageFromKey("ASK_TRADE")}");
                    }
                    else
                    {
                        if (Session.Character.Inventory.CountItem(2522) < 50)
                        {
                            Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NOT_ENOUGH_INGREDIENTS"), 0));
                            return;
                        }
                        Session.Character.GiftAdd(2518, 1);
                        Session.Character.Inventory.RemoveItemAmount(2522, 50);
                    }
                    break;

                case 147:
                    if (npc == null)
                    {
                        return;
                    }
                    if (packet.Type == 0)
                    {
                        Session.SendPacket($"qna #n_run^{packet.Runner}^56^{packet.Value}^{packet.NpcId} {Language.Instance.GetMessageFromKey("ASK_TRADE")}");
                    }
                    else
                    {
                        if (Session.Character.Inventory.CountItem(2523) < 50)
                        {
                            Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NOT_ENOUGH_INGREDIENTS"), 0));
                            return;
                        }
                        switch (Session.Character.Class)
                        {
                            case ClassType.Swordsman:
                                Session.Character.GiftAdd(4497, 1);
                                break;

                            case ClassType.Archer:
                                Session.Character.GiftAdd(4498, 1);
                                break;

                            case ClassType.Magician:
                                Session.Character.GiftAdd(4499, 1);
                                break;
                        }
                        Session.Character.Inventory.RemoveItemAmount(2523, 50);
                    }
                    break;

                case 148:
                    if (npc == null)
                    {
                        return;
                    }
                    if (packet.Type == 0)
                    {
                        Session.SendPacket($"qna #n_run^{packet.Runner}^56^{packet.Value}^{packet.NpcId} {Language.Instance.GetMessageFromKey("ASK_TRADE")}");
                    }
                    else
                    {
                        if (Session.Character.Inventory.CountItem(2523) < 50)
                        {
                            Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NOT_ENOUGH_INGREDIENTS"), 0));
                            return;
                        }
                        Session.Character.GiftAdd(2519, 1);
                        Session.Character.Inventory.RemoveItemAmount(2523, 50);
                    }
                    break;

                case 150:
                    if (npc != null)
                    {
                        if (Session.Character.Family != null)
                        {
                            if (Session.Character.Family.LandOfDeath != null && ServerManager.Instance.StartedEvents.Contains(EventType.LOD) && npc.Effect != 0)
                            {
                                if (Session.Character.Level >= 55)
                                {
                                    ServerManager.Instance.ChangeMapInstance(Session.Character.CharacterId, Session.Character.Family.LandOfDeath.MapInstanceId, (short)(153 + ServerManager.RandomNumber(-3, 2)), (short)(145 + ServerManager.RandomNumber(-3, 3)));                                }
                                else
                                {
                                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("LOD_REQUIERE_LVL"), 0));
                                }
                            }
                            else
                            {
                                Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("LOD_CLOSED"), 0));
                            }
                        }
                        else
                        {
                            Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NEED_FAMILY"), 0));
                        }
                    }
                    break;

                //3rd son of Mister Park : Halloween Quest
                case 183:
                    {
                        if (npc != null && ServerManager.Instance.Configuration.HalloweenEvent == true)
                        {
                            Session.Character.AddQuest(6007);
                        }
                    }
                    break;

                // Walker quest : Christmas quest
                case 184:
                    {
                        if (npc != null && ServerManager.Instance.Configuration.ChristmasEvent == true)
                        {
                            Session.Character.AddQuest(6009);
                        }
                    }
                    break;

                // Walker quest : Christmas quest
                case 186:
                    {
                        if (npc != null && ServerManager.Instance.Configuration.ChristmasEvent == true)
                        {
                            Session.Character.AddQuest(6013);
                        }
                    }
                    break;

                // Walker quest : Christmas quest
                case 190:
                    {
                        if (npc != null && ServerManager.Instance.Configuration.ChristmasEvent == true)
                        {
                            //  Session.Character.AddQuest(notKnown);
                        }
                    }
                    break;

                case 193:
                    {
                        if (npc != null)
                        {
                            Session.Character.AddQuest(6021);
                        }
                    }
                    break;

                case 194:
                    if (npc == null)
                    {
                        return;
                    }
                    if (packet.Type == 0)
                    {
                        Session.SendPacket($"qna #n_run^{packet.Runner}^56^{packet.Value}^{packet.NpcId} {Language.Instance.GetMessageFromKey("ASK_TRADE")}");
                    }
                    else
                    {
                        if (Session.Character.Inventory.CountItem(5986) < 3)
                        {
                            Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NOT_ENOUGH_INGREDIENTS"), 0));
                            return;
                        }
                        Session.Character.GiftAdd(5984, 3);
                        Session.Character.Inventory.RemoveItemAmount(5986, 3);
                    }
                    break;

                case 195:
                    if (npc == null)
                    {
                        return;
                    }
                    if (packet.Type == 0)
                    {
                        Session.SendPacket($"qna #n_run^{packet.Runner}^56^{packet.Value}^{packet.NpcId} {Language.Instance.GetMessageFromKey("ASK_TRADE")}");
                    }
                    else
                    {
                        if (Session.Character.Inventory.CountItem(5987) < 5)
                        {
                            Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NOT_ENOUGH_INGREDIENTS"), 0));
                            return;
                        }
                        Session.Character.GiftAdd(5977, 2);
                        Session.Character.Inventory.RemoveItemAmount(5987, 5);
                    }
                    break;

                //Raphael the turttle first quest : Summer Event quest
                case 197:
                    {
                        if (npc != null)
                        {
                            //  Session.Character.AddQuest(notKnown);
                        }
                    }
                    break;

                case 200:
                    {
                        if (npc != null)
                        {
                            if (Session.Character.Quests.Any(s => s.Quest.QuestType == (int)QuestType.Dialog2 && s.Quest.QuestObjectives.Any(b => b.Data == npc.NpcVNum)))
                            {
                                Session.Character.AddQuest(packet.Type);
                                Session.Character.IncrementQuests(QuestType.Dialog2, npc.NpcVNum);
                            }
                        }
                    }
                    break;

                //Raphael the turttle second quest :  Summer Event quest
                case 201:
                    {
                        if (npc != null)
                        {
                            //  Session.Character.AddQuest(notKnown);
                        }
                    }
                    break;

                case 300:
                    {
                        if (npc != null)
                        {
                            Session.Character.AddQuest(6040);
                        }
                    }
                    break;

                case 301:
                    if (Session.Character.Level < 85)
                    {
                        Session.SendPacket("msg 3 You need to be at least Level 85 to enter");
                        return;
                    }

                    if (Session.Character.Level > 84)
                    {
                        ServerManager.Instance.ChangeMap(Session.Character.CharacterId, 228, (short)(68 + ServerManager.RandomNumber(-3, 3)), (short)(102 + ServerManager.RandomNumber(-3, 3)));
                    }

                    if (Session.Character.Level > 84 && Session.Character.HeroLevel == 0)
                    {
                        Session.Character.HeroLevel = 1;
                        Session.SendPacket("msg 4 Hero Level unlocked!");
                        ServerManager.Instance.ChangeMap(Session.Character.CharacterId, 228, (short)(68 + ServerManager.RandomNumber(-3, 3)), (short)(102 + ServerManager.RandomNumber(-3, 3)));
                    }
                    break;

                case 305:
                    if (npc != null)
                    {
                        ServerManager.Instance.ChangeMap(Session.Character.CharacterId, 2526, 32, 40);
                    }
                    break;

                case 306:
                    if (Session.Character.MapId == 2526)
                    {
                        ServerManager.Instance.ChangeMap(Session.Character.CharacterId, 2527, (short)(29 + ServerManager.RandomNumber(-3, 3)), (short)(43 + ServerManager.RandomNumber(-3, 3)));
                    }
                    break;

                case 307:
                    if (Session.Character.MapId == 2527)
                    {
                        ServerManager.Instance.ChangeMap(Session.Character.CharacterId, 2526, (short)(32 + ServerManager.RandomNumber(-2, 2)), (short)(42 + ServerManager.RandomNumber(-2, 2)));
                    }
                    break;

                //todo : add frigg & ragnar pnj
                case 308:
                    if (npc != null & Session.Character.Level >= 50)
                    {
                        Session.Character.AddQuest(6134);
                    }
                    else
                    {
                        Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("LEVEL_50_REQUIERED_QUEST"), 0));
                    }
                    break;

                case 312:
                    if (npc != null)
                    {
                        Session.Character.AddQuest(6208);
                    }
                    break;

                case 313:
                    if (npc != null && Session.Character.Gold >= 50000 && Session.Character.Level >= 70)
                    {
                        Session.Character.Gold -= 50000;
                        Session.SendPacket(Session.Character.GenerateGold());
                        ServerManager.Instance.ChangeMap(Session.Character.CharacterId, 261, (short)(181 + ServerManager.RandomNumber(-3, 3)), (short)(212 + ServerManager.RandomNumber(-3, 3)));
                    }
                    else if (Session.Character.Gold < 50000)
                    {
                        Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("NOT_ENOUGH_MONEY"), 10));
                    }
                    else
                    {
                        Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("LEVEL_70_REQUIERED"), 10));
                    }
                    break;

                case 314:
                    if (npc != null)
                    {
                        ServerManager.Instance.ChangeMap(Session.Character.CharacterId, 145, (short)(52 + ServerManager.RandomNumber(-3, 3)), (short)(40 + ServerManager.RandomNumber(-3, 3)));
                    }
                    break;

                    //Jennifer quest
                case 315:
                    if (npc != null)
                    {
                        Session.Character.AddQuest(6242);
                    }
                    break;

                    //Professor macavity
                case 316:
                    if (npc != null)
                    {
                        Session.Character.AddQuest(6242);
                    }
                    break;

                //Malcolm 10th anniversary 
                case 319:
                    if (npc != null)
                    {

                    }
                    break;

                case 321://Dialog = 536
                    {
                        Session.Character.OpenBank();
                    }
                    break;

                case 322://Dialog = 356
                    {
                        if (packet.Type == 0 && packet.Value == 2)
                        {
                            var Item = Session.Character.Inventory.CountItem(5836);
                            if (Item == 0)
                            {
                                var iteminfo = ServerManager.GetItem(5836);
                                var inv = Session.Character.Inventory.AddNewToInventory(5836).FirstOrDefault();
                                if (inv != null)
                                {
                                    Session.SendPacket(UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("BANK_BOOK_RECEIVED")));
                                }
                                else
                                {
                                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NOT_ENOUGH_PLACE"), 0));
                                }
                            }
                            else
                            {
                                Session.SendPacket(UserInterfaceHelper.GenerateSay(Language.Instance.GetMessageFromKey("ALREADY_RECEIVED_BANK_BOOK"), 10));
                            }
                        }
                    }
                    break;

                case 323: // Guard quest from Tart Hapendam
                    {
                        if (npc != null)
                        {
                        }
                        else
                        {
                            Session.SendPacket(UserInterfaceHelper.GenerateSay(Language.Instance.GetMessageFromKey(""), 10));
                        }
                    }
                    break;

                case 324: // MA Quest SP2
                    {
                        if (npc != null && Session.Character.Class == ClassType.MartialArtist)
                        {
                            Session.Character.AddQuest(6307);
                        }
                        else
                        {
                            Session.SendPacket(UserInterfaceHelper.GenerateSay(Language.Instance.GetMessageFromKey("QUEST_ONLY_FOR_MARTIAL_ARTIST"), 10));
                        }
                    }
                    break;

                case 325:
                    {
                        if (npc == null || !ServerManager.Instance.Configuration.ChristmasEvent)
                        {
                            return;
                        }

                        if (packet.Type == 0)
                        {
                            Session.SendPacket($"qna #n_run^{packet.Runner}^56^{packet.Value}^{packet.NpcId} {Language.Instance.GetMessageFromKey("ASK_TRADE")}");
                        }
                        else
                        {
                            if ((Session.Character.Inventory.CountItem(5712) < 1 && Session.Character.Inventory.CountItem(9138) < 1) || (Session.Character.Inventory.CountItem(4406) < 1 && Session.Character.Inventory.CountItem(8369) < 1))
                            {
                                Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NOT_ENOUGH_INGREDIENTS"), 0));
                                return;
                            }

                            if (Session.Character.Inventory.CountItem(9138) > 0)
                            {
                                Session.Character.Inventory.RemoveItemAmount(9138, 1);
                                Session.Character.GiftAdd(9140, 1);
                            }
                            else
                            {
                                Session.Character.Inventory.RemoveItemAmount(5712, 1);
                                Session.Character.GiftAdd(5714, 1);
                            }

                            if (Session.Character.Inventory.CountItem(8369) > 0)
                            {
                                Session.Character.Inventory.RemoveItemAmount(8369, 1);
                            }
                            else
                            {
                                Session.Character.Inventory.RemoveItemAmount(4406, 1);
                            }
                        }
                    }
                    break;

                case 326:
                    {
                        if (npc != null && ServerManager.Instance.Configuration.ChristmasEvent)
                        {
                            Session.Character.AddQuest(6325);
                        }
                    }
                    break;

                    //Amora's valentine (Event) quest
                case 327:
                    {
                        if (npc != null && ServerManager.Instance.Configuration.ValentineEvent == true)
                        {
                            Session.Character.AddQuest(6326);
                        }
                    }
                    break;

                //Eva Easter (Event) quest
                case 328:
                    {
                        if (npc != null && ServerManager.Instance.Configuration.EasterEvent == true)
                        {
                            Session.Character.AddQuest(7550);
                        }
                    }
                    break;

                //Soraya Easter (Event) quest
                case 329:
                    {
                        if (npc != null && ServerManager.Instance.Configuration.EasterEvent == true)
                        {
                            Session.Character.AddQuest(7551);
                        }
                    }
                    break;

                //Malcolm mix Easter (Event) quest
                case 330:
                    {
                        if (npc != null && ServerManager.Instance.Configuration.EasterEvent == true)
                        {
                           // Session.Character.AddQuest(???);
                        }
                    }
                    break;

                //Malcolm mix Easter (Event) quest
                case 331:
                    {
                        if (npc != null && ServerManager.Instance.Configuration.EasterEvent == true)
                        {
                             Session.Character.AddQuest(6331);
                        }
                    }
                    break;

                //First quest act7
                case 332:
                    {
                        if (npc != null && Session.Character.HeroLevel > 9 && Session.Character.Level > 84)
                        {
                            Session.Character.AddQuest(6500);
                        }
                        else
                        {
                        //Add key : Your character level is too low
                        }
                    }
                    break;

                    //Quest sp4 martial artist
                case 333:
                    {
                        if (npc != null && Session.Character.Class == ClassType.MartialArtist)
                        {
                            Session.Character.AddQuest(6346);
                        }
                        else
                        {
                            //Add key : Your character level is too low
                            Session.SendPacket(UserInterfaceHelper.GenerateSay(Language.Instance.GetMessageFromKey("QUEST_ONLY_FOR_MARTIAL_ARTIST"), 10));
                        }
                    }
                    break;

                //Tp Desert to act7
                case 334:
                    if (npc != null)
                    {
                        MapInstance map = null;
                        switch (Session.Character.Faction)
                        {
                            case FactionType.None:
                            case FactionType.Angel:
                            case FactionType.Demon:
                                map = ServerManager.GetAllMapInstances().Find(s => s.MapInstanceType.Equals(MapInstanceType.Act7Ship));
                                break;
                        }
                        if (map == null)
                        {
                            Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("SHIP_NOTARRIVED"), 0));
                            return;
                        }
                        if (Session.Character.Level < 88 || Session.Character.HeroLevel < 10)
                        {
                            Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("ACT7_REQUIERED_LEVEL_TOO_LOW"), 0));
                            return;
                        }
                        if (Session.Character.Gold < 25000)
                        {
                            Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("NOT_ENOUGH_MONEY"), 10));
                            return;
                        }
                        Session.Character.Gold -= 25000;
                        Session.SendPacket(Session.Character.GenerateGold());
                        MapCell pos = map.Map.GetRandomPosition();
                        ServerManager.Instance.ChangeMapInstance(Session.Character.CharacterId, map.MapInstanceId, pos.X, pos.Y);
                    }
                    break;

                    //Guardswoman Celestial Spire Ground Entrance tp
                case 335:
                    if (npc != null)
                    {
                        // dialog 11028 test it and change mapid mapy mapy
                       // ServerManager.Instance.ChangeMap(Session.Character.CharacterId, 2643, 52, 40);
                    }
                    break;

                //Alveus to act7
                case 336:
                    if (npc != null && packet.Type > 0)
                    { 
                        if (Session.Character.Level < 88 || Session.Character.HeroLevel < 10)
                        {
                            Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("ACT7_REQUIERED_LEVEL_TOO_LOW"), 0));
                            return;
                        }
                        if (Session.Character.Gold < 30000 && packet.Type == 145)
                        {
                            Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("NOT_ENOUGH_MONEY"), 10));
                            return;
                        }
                        if (Session.Character.Gold < 25000 && packet.Type == 170)
                        {
                            Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("NOT_ENOUGH_MONEY"), 10));
                            return;
                        }
                        if (packet.Type == 145)
                        {
                            ServerManager.Instance.ChangeMap(Session.Character.CharacterId, 145, (short)(55 + ServerManager.RandomNumber(-2, 2)), (short)(28 + ServerManager.RandomNumber(-2, 2)));
                            Session.Character.Gold -= 30000;
                            Session.SendPacket(Session.Character.GenerateGold());
                        }
                        if (packet.Type == 170)
                        {
                            ServerManager.Instance.ChangeMap(Session.Character.CharacterId, 170, (short)(123 + ServerManager.RandomNumber(-3, 3)), (short)(83 + ServerManager.RandomNumber(-3, 3)));
                            Session.Character.Gold -= 25000;
                            Session.SendPacket(Session.Character.GenerateGold());
                        }
                    }
                    break;

                case 337:
                    if (npc != null && ServerManager.Instance.Configuration.ValentineEvent == true)
                    {
                        Session.Character.AddQuest(6360);
                    }
                    break;

                case 340: // MA Quest SP3
                    {
                        if (npc != null)
                        {
                            Session.Character.AddQuest(6332);
                        }
                    }
                    break;

                case 666: // Hero Equipment Downgrade
                    {
                        // 4949 ~ 4966 = c25/c28
                        // 4978 ~ 4986 = c45/c48

                        const long price = 10000000;

                        ItemInstance itemInstance = Session?.Character?.Inventory?.LoadBySlotAndType(0, InventoryType.Equipment);

                        if (itemInstance?.Item != null && ((itemInstance.ItemVNum >= 4949 && itemInstance.ItemVNum <= 4966) || (itemInstance.ItemVNum >= 4978 && itemInstance.ItemVNum <= 4986)) && itemInstance.Rare == 8)
                        {
                            if (Session.Character.Gold < price)
                            {
                                Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("NOT_ENOUGH_MONEY"), 10));
                                return;
                            }

                            Session.Character.Gold -= price;
                            Session.SendPacket(Session.Character.GenerateGold());

                            itemInstance.RarifyItem(Session, RarifyMode.HeroEquipmentDowngrade, RarifyProtection.None);

                            Session.SendPacket(itemInstance.GenerateInventoryAdd());
                        }
                    }
                    break;

                case 1000:
                    if (npc == null)
                    {
                        return;
                    }

                    if (Session.Character.Quests.Any(s => s.Quest.DialogNpcVNum == npc.NpcVNum && s.Quest.QuestObjectives.Any(o => o.SpecialData == packet.Type)))
                    {
                        if (ServerManager.Instance.TimeSpaces.FirstOrDefault(s => s.QuestTimeSpaceId == packet.Type) is ScriptedInstance timeSpace)
                        {
                            Session.Character.EnterInstance(timeSpace);
                        }
                    }

                    break;

                case 1500:
                    {
                        if (npc != null)
                        {
                            Session.Character.AddQuest(2255);
                        }
                    }
                    break;

                case 1503:
                    if (!ServerManager.Instance.Configuration.HalloweenEvent)
                    {
                        return;
                    }

                    if (Session.Character.Level < 20)
                    {
                        Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("TOO_LOW_LVL"), 0));
                        return;
                    }

                    if (!Session.Character.GeneralLogs.Any(s => s.LogType == "DailyReward" && short.Parse(s.LogData) == 1917 && s.Timestamp.Date == DateTime.Today))
                    {
                        Session.Character.GeneralLogs.Add(new GeneralLogDTO
                        {
                            AccountId = Session.Account.AccountId,
                            CharacterId = Session.Character.CharacterId,
                            IpAddress = Session.IpAddress,
                            LogData = "1917",
                            LogType = "DailyReward",
                            Timestamp = DateTime.Now
                        });
                        short amount = 1;
                        if (Session.Character.IsMorphed)
                        {
                            amount *= 2;
                        }
                        Session.Character.GiftAdd(1917, amount);
                    }
                    else
                    {
                        Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("QUEST_ALREADY_DONE"), 0));
                    }
                    break;

                case 1600:
                    {
                        if (!Session.Character.VerifiedLock)
                        {
                            Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("ACTION_NOT_POSSIBLE_USE_UNLOCK"), 0));
                            return;
                        }


                        Session.SendPacket(Session.Character.OpenFamilyWarehouse());
                    }
                    break;

                case 1601:
                    Session.SendPackets(Session.Character.OpenFamilyWarehouseHist());
                    break;

                case 1602:
                    if (Session.Character.Family?.FamilyLevel >= 2 && Session.Character.Family.WarehouseSize < 21)
                    {
                        if (Session.Character.FamilyCharacter.Authority == FamilyAuthority.Head)
                        {
                            if (500000 >= Session.Character.Gold)
                            {
                                Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("NOT_ENOUGH_MONEY"), 10));
                                return;
                            }
                            Session.Character.Family.WarehouseSize = 21;
                            Session.Character.Gold -= 500000;
                            Session.SendPacket(Session.Character.GenerateGold());
                            FamilyDTO fam = Session.Character.Family;
                            DAOFactory.FamilyDAO.InsertOrUpdate(ref fam);
                            ServerManager.Instance.FamilyRefresh(Session.Character.Family.FamilyId);
                        }
                        else
                        {
                            Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("NO_FAMILY_HEAD"), 10));
                            Session.SendPacket(UserInterfaceHelper.GenerateModal(Language.Instance.GetMessageFromKey("NO_FAMILY_HEAD"), 1));
                        }
                    }
                    break;

                case 1603:
                    if (Session.Character.Family?.FamilyLevel >= 7 && Session.Character.Family.WarehouseSize < 49)
                    {
                        if (Session.Character.FamilyCharacter.Authority == FamilyAuthority.Head)
                        {
                            if (2000000 >= Session.Character.Gold)
                            {
                                Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("NOT_ENOUGH_MONEY"), 10));
                                return;
                            }
                            Session.Character.Family.WarehouseSize = 49;
                            Session.Character.Gold -= 2000000;
                            Session.SendPacket(Session.Character.GenerateGold());
                            FamilyDTO fam = Session.Character.Family;
                            DAOFactory.FamilyDAO.InsertOrUpdate(ref fam);
                            ServerManager.Instance.FamilyRefresh(Session.Character.Family.FamilyId);
                        }
                        else
                        {
                            Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("NO_FAMILY_HEAD"), 10));
                            Session.SendPacket(UserInterfaceHelper.GenerateModal(Language.Instance.GetMessageFromKey("NO_FAMILY_HEAD"), 1));
                        }
                    }
                    break;

                case 1604:
                    if (Session.Character.Family?.FamilyLevel >= 5 && Session.Character.Family.MaxSize < 70)
                    {
                        if (Session.Character.FamilyCharacter.Authority == FamilyAuthority.Head)
                        {
                            if (5000000 >= Session.Character.Gold)
                            {
                                Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("NOT_ENOUGH_MONEY"), 10));
                                return;
                            }
                            Session.Character.Family.MaxSize = 70;
                            Session.Character.Gold -= 5000000;
                            Session.SendPacket(Session.Character.GenerateGold());
                            FamilyDTO fam = Session.Character.Family;
                            DAOFactory.FamilyDAO.InsertOrUpdate(ref fam);
                            ServerManager.Instance.FamilyRefresh(Session.Character.Family.FamilyId);
                        }
                        else
                        {
                            Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("NO_FAMILY_HEAD"), 10));
                            Session.SendPacket(UserInterfaceHelper.GenerateModal(Language.Instance.GetMessageFromKey("NO_FAMILY_HEAD"), 1));
                        }
                    }
                    break;

                case 1605:
                    if (Session.Character.Family?.FamilyLevel >= 9 && Session.Character.Family.MaxSize < 100)
                    {
                        if (Session.Character.FamilyCharacter.Authority == FamilyAuthority.Head)
                        {
                            if (10000000 >= Session.Character.Gold)
                            {
                                Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("NOT_ENOUGH_MONEY"), 10));
                                return;
                            }
                            Session.Character.Family.MaxSize = 100;
                            Session.Character.Gold -= 10000000;
                            Session.SendPacket(Session.Character.GenerateGold());
                            FamilyDTO fam = Session.Character.Family;
                            DAOFactory.FamilyDAO.InsertOrUpdate(ref fam);
                            ServerManager.Instance.FamilyRefresh(Session.Character.Family.FamilyId);
                        }
                        else
                        {
                            Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("NO_FAMILY_HEAD"), 10));
                            Session.SendPacket(UserInterfaceHelper.GenerateModal(Language.Instance.GetMessageFromKey("NO_FAMILY_HEAD"), 1));
                        }
                    }
                    break;

                case 2000:
                    {
                        if (npc != null)
                        {
                            if (packet.Type == 2000 && npc.NpcVNum == 932 && !Session.Character.Quests.Any(s => s.QuestId >= 2000 && s.QuestId <= 2007) // Pajama
                                || packet.Type == 2008 && npc.NpcVNum == 933 && !Session.Character.Quests.Any(s => s.QuestId >= 2008 && s.QuestId <= 2013) // SP 1
                                || packet.Type == 2014 && npc.NpcVNum == 934 && !Session.Character.Quests.Any(s => s.QuestId >= 2014 && s.QuestId <= 2020) // SP 2
                                || packet.Type == 2060 && npc.NpcVNum == 948 && !Session.Character.Quests.Any(s => s.QuestId >= 2060 && s.QuestId <= 2095) // SP 3
                                || packet.Type == 2100 && npc.NpcVNum == 954 && !Session.Character.Quests.Any(s => s.QuestId >= 2100 && s.QuestId <= 2134) // SP 4
                                || packet.Type == 2030 && npc.NpcVNum == 422 && !Session.Character.Quests.Any(s => s.QuestId >= 2030 && s.QuestId <= 2046)
                                || packet.Type == 2048 && npc.NpcVNum == 303 && !Session.Character.Quests.Any(s => s.QuestId >= 2048 && s.QuestId <= 2050))
                            {
                                Session.Character.AddQuest(packet.Type);
                            }
                        }
                    }
                    break;

                case 2001:
                    {
                        switch (packet.Type)
                        {
                            case 1: // Pajama
                                {
                                    if (Session.Character.MapInstance.Npcs.Any(s => s.NpcVNum == 932))
                                    {
                                        //Session.Character.GiftAdd(900, 1);
                                        return;
                                    }
                                }
                                break;
                            case 2: // SP 1
                                {
                                    if (Session.Character.MapInstance.Npcs.Any(s => s.NpcVNum == 933))
                                    {
                                        switch (Session.Character.Class)
                                        {
                                            case ClassType.Swordsman:
                                               // Session.Character.GiftAdd(901, 1);
                                                break;
                                            case ClassType.Archer:
                                                //Session.Character.GiftAdd(903, 1);
                                                break;
                                            case ClassType.Magician:
                                               // Session.Character.GiftAdd(905, 1);
                                                break;
                                        }
                                        return;
                                    }
                                }
                                break;
                            case 3: // SP 2
                                {
                                    if (Session.Character.MapInstance.Npcs.Any(s => s.NpcVNum == 934))
                                    {
                                        switch (Session.Character.Class)
                                        {
                                            case ClassType.Swordsman:
                                                //Session.Character.GiftAdd(902, 1);
                                                break;
                                            case ClassType.Archer:
                                                //Session.Character.GiftAdd(904, 1);
                                                break;
                                            case ClassType.Magician:
                                               // Session.Character.GiftAdd(906, 1);
                                                break;
                                        }
                                        return;
                                    }
                                }
                                break;
                        }
                    }
                    break;

                case 5:
                    if (packet.Type == 0 && packet.Value == 1)
                    {
                        if (Session.Character.MapInstance.Npcs.Any(s => s.NpcVNum == 948 /* SP 3 */ || s.NpcVNum == 954 /* SP 4 */))
                        {
                            //SP 3
                            switch (Session.Character.Class)
                            {
                                case ClassType.Swordsman:
                                   // Session.Character.GiftAdd(909, 1);
                                    break;
                                case ClassType.Archer:
                                   // Session.Character.GiftAdd(911, 1);
                                    break;
                                case ClassType.Magician:
                                  //  Session.Character.GiftAdd(913, 1);
                                    break;
                            }

                            //SP 4
                            switch (Session.Character.Class)
                            {
                                case ClassType.Swordsman:
                                   // Session.Character.GiftAdd(910, 1);
                                    break;
                                case ClassType.Archer:
                                   // Session.Character.GiftAdd(912, 1);
                                    break;
                                case ClassType.Magician:
                                   // Session.Character.GiftAdd(914, 1);
                                    break;
                            }
                            return;
                        }
                    }
                    break;

                case 2002:
                    if (npc != null)
                    {
                        int gemNpcVnum = 0;

                        switch (npc.NpcVNum)
                        {
                            case 935:
                                gemNpcVnum = 932;
                                break;
                            case 936:
                                gemNpcVnum = 933;
                                break;
                            case 937:
                                gemNpcVnum = 934;
                                break;
                            case 952:
                                gemNpcVnum = 948;
                                break;
                            case 953:
                                gemNpcVnum = 954;
                                break;
                        }

                        if (ServerManager.Instance.SpecialistGemMapInstances?.FirstOrDefault(s => s.Npcs.Any(n => n.NpcVNum == gemNpcVnum)) is MapInstance specialistGemMapInstance)
                        {
                            ServerManager.Instance.ChangeMapInstance(Session.Character.CharacterId, specialistGemMapInstance.MapInstanceId, (short)(7 + ServerManager.RandomNumber(-3, 2)), (short)(5 + ServerManager.RandomNumber(-2, 3))); ;
                        }
                    }
                    break;

                case 3000:
                    {
                        if (npc != null)
                        {
                            Session.Character.AddQuest(5478, true);
                        }
                    }
                    break;

                case 3006:
                    {
                        if (npc != null)
                        {
                            Session.Character.AddQuest(packet.Type);
                        }
                    }
                    break;

                case 4000:
                    if (npc != null)
                    {
                        if (ServerManager.Instance.CanRegisterRainbowBattle)
                        {
                            if (Session.Character.Group != null)
                            {
                                if (Session.Character.Group.Raid == null)
                                {
                                    if (Session.Character.Group.GroupType == GroupType.BigTeam)
                                    {
                                        if (Session.Character.Group.IsLeader(Session))
                                        {
                                            if (ServerManager.Instance.RainbowBattleMembersRegistered.Count() > 0)
                                            {
                                                if (ServerManager.Instance.RainbowBattleMembersRegistered?.Where(s => s.Session == Session).Count() > 0)
                                                {
                                                    return;
                                                }
                                            }
                                            Parallel.ForEach(
                                                                                ServerManager.Instance.RainbowBattleMembers.Where(s => s.GroupId == Session.Character.Group.GroupId), s =>
                                                                                {
                                                                                    ServerManager.Instance.RainbowBattleMembersRegistered.Add(s);
                                                                                }
                                                                                );
                                            Session.SendPacket(npc?.GenerateSay(Language.Instance.GetMessageFromKey("BA_REGISTERED"), 10));

                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            Session.SendPacket(npc?.GenerateSay(Language.Instance.GetMessageFromKey("BA_NOT_OPEN"), 10));
                        }
                    }
                    break;

                case 5001:
                    if (npc != null)
                    {
                        MapInstance map = null;
                        switch (Session.Character.Faction)
                        {
                            case FactionType.None:
                                Session.SendPacket(UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("FACTION_NEEDED_JOIN_ACT4")));
                                return;

                            case FactionType.Angel:
                                map = ServerManager.GetAllMapInstances().Find(s => s.MapInstanceType.Equals(MapInstanceType.Act4ShipAngel));

                                break;

                            case FactionType.Demon:
                                map = ServerManager.GetAllMapInstances().Find(s => s.MapInstanceType.Equals(MapInstanceType.Act4ShipDemon));

                                break;
                        }
                        if (map == null || npc.EffectActivated)
                        {
                            Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("SHIP_NOTARRIVED"), 0));
                            return;
                        }
                        if (Session.Character.Gold < 3000)
                        {
                            Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("NOT_ENOUGH_MONEY"), 10));
                            return;
                        }
                        Session.Character.Gold -= 3000;
                        Session.SendPacket(Session.Character.GenerateGold());
                        MapCell pos = map.Map.GetRandomPosition();
                        ServerManager.Instance.ChangeMapInstance(Session.Character.CharacterId, map.MapInstanceId, pos.X, pos.Y);
                    }
                    break;

                case 5002:
                    tp = npc?.Teleporters?.FirstOrDefault(s => s.Index == packet.Type);
                    if (tp != null)
                    {
                        Session.SendPacket("it 3");
                        if (ServerManager.Instance.ChannelId == 51)
                        {
                            string connection = CommunicationServiceClient.Instance.RetrieveOriginWorld(Session.Account.AccountId);
                            if (string.IsNullOrWhiteSpace(connection))
                            {
                                return;
                            }
                            Session.Character.MapId = 145;
                            Session.Character.MapX = 60;
                            Session.Character.MapY = 60;
                            int port = Convert.ToInt32(connection.Split(':')[1]);
                            Session.Character.ChangeChannel(connection.Split(':')[0], port, 3);
                        }
                        else
                        {
                            ServerManager.Instance.ChangeMap(Session.Character.CharacterId, tp.MapId, (short)(tp.MapX + ServerManager.RandomNumber(-3, 3)), (short)(tp.MapY + ServerManager.RandomNumber(-3, 3))); ;
                        }
                    }
                    break;

                case 5004:
                    if (npc != null)
                    {
                        ServerManager.Instance.ChangeMap(Session.Character.CharacterId, 145, (short)(50 + ServerManager.RandomNumber(-3, 3)), (short)(41 + ServerManager.RandomNumber(-3, 3)));
                    }
                    break;

                case 5011:
                    if (npc != null)
                    {
                        MapInstance map = null;
                        switch (Session.Character.Faction)
                        {
                            case FactionType.None:
                            case FactionType.Angel:
                            case FactionType.Demon:
                                map = ServerManager.GetAllMapInstances().Find(s => s.MapInstanceType.Equals(MapInstanceType.Act5Ship));
                                break;
                        }
                        if (map == null || npc.EffectActivated)
                        {
                            Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("SHIP_NOTARRIVED"), 0));
                            return;
                        }
                        if (Session.Character.Gold < 30000)
                        {
                            Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("NOT_ENOUGH_MONEY"), 10));
                            return;
                        }            
                        MapCell pos = map.Map.GetRandomPosition();
                        
                        ServerManager.Instance.ChangeMapInstance(Session.Character.CharacterId, map.MapInstanceId, pos.X, pos.Y);
                    }
                    break;

                case 5012:
                    tp = npc?.Teleporters?.FirstOrDefault(s => s.Index == packet.Type);
                    if (tp != null)
                    {
                        ServerManager.Instance.ChangeMap(Session.Character.CharacterId, tp.MapId, (short)(tp.MapX + ServerManager.RandomNumber(-3, 3)), (short)(tp.MapY + ServerManager.RandomNumber(-3, 3)));
                    }
                    break;

                case 5014:
                    tp = npc?.Teleporters?.FirstOrDefault(s => s.Index == packet.Type);
                    if (tp != null)
                    {
                        ServerManager.Instance.ChangeMap(Session.Character.CharacterId, tp.MapId, (short)(tp.MapX + ServerManager.RandomNumber(-3, 3)), (short)(tp.MapY + ServerManager.RandomNumber(-3, 3)));
                    }
                    if (npc.NpcVNum == 1350)
                    {
                        ServerManager.Instance.ChangeMap(Session.Character.CharacterId, 145, (short)(49 + ServerManager.RandomNumber(-3, 3)), (short)(44 + ServerManager.RandomNumber(-3, 3)));
                    }
                    break;

                case 6013:
                    if (!ServerManager.Instance.Configuration.ChristmasEvent)
                    {
                        return;
                    }
                    if (npc == null)
                    {
                        return;
                    }
                    if (packet.Type == 0)
                    {
                        Session.SendPacket($"qna #n_run^{packet.Runner}^56^{packet.Value}^{packet.NpcId} {Language.Instance.GetMessageFromKey("ASK_TRADE")}");
                    }
                    else
                    {
                        if (Session.Character.Inventory.CountItem(1611) < 1 || Session.Character.Inventory.CountItem(1612) < 1
                         || Session.Character.Inventory.CountItem(1613) < 2 || Session.Character.Inventory.CountItem(1614) < 1)
                        {
                            Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NOT_ENOUGH_INGREDIENTS"), 0));
                            return;
                        }
                        Session.Character.GiftAdd(1621, 1);
                        Session.Character.Inventory.RemoveItemAmount(1611);
                        Session.Character.Inventory.RemoveItemAmount(1612);
                        Session.Character.Inventory.RemoveItemAmount(1613, 2);
                        Session.Character.Inventory.RemoveItemAmount(1614);
                    }
                    break;


                case 6014:
                    if (!ServerManager.Instance.Configuration.ChristmasEvent)
                    {
                        return;
                    }
                    if (npc == null)
                    {
                        return;
                    }
                    if (packet.Type == 0)
                    {
                        Session.SendPacket($"qna #n_run^{packet.Runner}^56^{packet.Value}^{packet.NpcId} {Language.Instance.GetMessageFromKey("ASK_TRADE")}");
                    }
                    else
                    {
                        if (Session.Character.Inventory.CountItem(1615) < 1 || Session.Character.Inventory.CountItem(1616) < 2 || Session.Character.Inventory.CountItem(1617) < 1
                         || Session.Character.Inventory.CountItem(1618) < 1 || Session.Character.Inventory.CountItem(1619) < 1 || Session.Character.Inventory.CountItem(1620) < 1)
                        {
                            Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NOT_ENOUGH_INGREDIENTS"), 0));
                            return;
                        }
                        Session.Character.GiftAdd(1622, 1);
                        Session.Character.Inventory.RemoveItemAmount(1615);
                        Session.Character.Inventory.RemoveItemAmount(1616, 2);
                        Session.Character.Inventory.RemoveItemAmount(1617);
                        Session.Character.Inventory.RemoveItemAmount(1618);
                        Session.Character.Inventory.RemoveItemAmount(1619);
                        Session.Character.Inventory.RemoveItemAmount(1620);
                    }
                    break;

                case 6017:
                    if (!ServerManager.Instance.Configuration.LunarNewYearEvent)
                    {
                        return;
                    }
                    if (npc == null)
                    {
                        return;
                    }
                    if (packet.Type == 0)
                    {
                        Session.SendPacket($"qna #n_run^{packet.Runner}^56^{packet.Value}^{packet.NpcId} {Language.Instance.GetMessageFromKey("ASK_TRADE")}");
                    }
                    else
                    {
                        if (Session.Character.Inventory.CountItem(5085) < 5 || Session.Character.Inventory.CountItem(5086) < 5)
                        {
                            Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NOT_ENOUGH_INGREDIENTS"), 0));
                            return;
                        }
                        Session.Character.GiftAdd(5083, 1);
                        Session.Character.Inventory.RemoveItemAmount(5085, 5);
                        Session.Character.Inventory.RemoveItemAmount(5086, 5);
                    }
                    break;

                default:
                    {
                        Logger.Warn(string.Format(Language.Instance.GetMessageFromKey("NO_NRUN_HANDLER"), packet.Runner));
                    }
                    break;
            }
        }

        #endregion
    }
}