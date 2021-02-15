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
using OpenNos.Domain;
using OpenNos.GameObject.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using OpenNos.GameObject.Networking;

namespace OpenNos.GameObject.Event
{
    public static class FamilyRaid
    {
        #region Methods

        public static void GenerateFamilyRaid(long familyId, string familyName)
        {
            ServerManager.Instance.Broadcast(UserInterfaceHelper.GenerateMsg(familyName + " 's raid will start in 5 minutes. Get ready!", 5));
            Thread.Sleep(4 * 60 * 1000);
            ServerManager.Instance.Broadcast(UserInterfaceHelper.GenerateMsg(familyName + " 's raid will start in 1 minutes. Get ready!", 5));
            Thread.Sleep(30 * 1000);
            ServerManager.Instance.Broadcast(UserInterfaceHelper.GenerateMsg(familyName + " 's raid will start in 30 seconds. Get ready!", 5));
            Thread.Sleep(20 * 1000);
            ServerManager.Instance.Broadcast(UserInterfaceHelper.GenerateMsg(familyName + " 's raid will start in 10 seconds. Get ready!", 5));
            Thread.Sleep(10 * 1000);
            ServerManager.Instance.Broadcast(UserInterfaceHelper.GenerateMsg(familyName + " 's raid is starting", 1));
            ServerManager.Instance.Broadcast($"qnaml 1 #guri^506 Participate to the " + familyName + " 's raid?");
            ServerManager.Instance.EventInWaiting = true;
            Thread.Sleep(30 * 1000);
            ServerManager.Instance.Broadcast(UserInterfaceHelper.GenerateMsg(familyName + " started a Family Raid Boss!!!", 5));
            ServerManager.Instance.Sessions.Where(a => a.Character?.IsWaitingForEvent == false).ToList().ForEach(a => a.SendPacket("esf"));
            ServerManager.Instance.EventInWaiting = false;
            IEnumerable<ClientSession> sessions = ServerManager.Instance.Sessions.Where(s => s.Character?.IsWaitingForEvent == true && s.Character.MapInstance.MapInstanceType == MapInstanceType.BaseMapInstance && s.Character.Family.FamilyId == familyId);
            List<Tuple<MapInstance, byte>> maps = new List<Tuple<MapInstance, byte>>();
            MapInstance map = null;
            int i = -1;
            int level = 0;
            byte instancelevel = 50;
            foreach (ClientSession s in sessions)
            {
                i++;
                if (i == 0)
                {
                    map = ServerManager.GenerateMapInstance(2100, MapInstanceType.FamilyRaidBoss, new InstanceBag());
                    maps.Add(new Tuple<MapInstance, byte>(map, instancelevel));
                    instancelevel = 50;
                }
                if (i == 50)
                {
                    i = -1;
                }
                if (map != null)
                {
                    ServerManager.Instance.TeleportOnRandomPlaceInMap(s, map.MapInstanceId);
                    s.Character.Buff.ClearAll();
                }
                else
                {
                    ServerManager.Instance.Broadcast($"msg 0 Error in teleportation in guild raid");
                }
                level = s.Character.Level;
            }
            ServerManager.Instance.Sessions.Where(s => s.Character != null).ToList().ForEach(s => s.Character.IsWaitingForEvent = false);
            foreach (Tuple<MapInstance, byte> mapinstance in maps)
            {
                FamilyRaidTask task = new FamilyRaidTask();
                Observable.Timer(TimeSpan.FromMinutes(0)).Subscribe(X => FamilyRaidTask.Run(mapinstance));
            }
        }

        #endregion

        #region Classes

        public class FamilyRaidTask
        {

            #region Methods

            public static void Run(Tuple<MapInstance, byte> mapinstance)
            {

                long maxGold = ServerManager.Instance.Configuration.MaxGold;
                Thread.Sleep(10 * 1000);
                //if (!mapinstance.Item1.Sessions.Skip(3-1).Any())
                //{
                //    mapinstance.Item1.Sessions.Where(s => s.Character != null).ToList().ForEach(s => ServerManager.Instance.ChangeMap(s.Character.CharacterId, s.Character.MapId, s.Character.MapX, s.Character.MapY));
                //}
                Observable.Timer(TimeSpan.FromMinutes(12)).Subscribe(X =>
                {
                    for (int d = 0; d < 180; d++)
                    {
                        if (!mapinstance.Item1.Monsters.Any(s => s.CurrentHp > 0))
                        {
                            EventHelper.Instance.ScheduleEvent(TimeSpan.FromMinutes(0), new EventContainer(mapinstance.Item1, EventActionType.SPAWNPORTAL, new Portal { SourceX = 60, SourceY = 70, DestinationMapId = 190 }));
                            mapinstance.Item1.Broadcast(UserInterfaceHelper.GenerateMsg("U WON", 0));
                            foreach (ClientSession cli in mapinstance.Item1.Sessions.Where(s => s.Character != null).ToList())
                            {
                                cli.Character.GenerateFamilyXp(cli.Character.Level * 4);
                                cli.Character.GetReputation(cli.Character.Level * 100);
                                cli.Character.Gold += 100000000;
                                cli.Character.GiftAdd(284, 1);
                                cli.Character.Gold = cli.Character.Gold > maxGold ? maxGold : cli.Character.Gold;
                                cli.Character.SpAdditionPoint += cli.Character.Level * 100;
                                cli.Character.SpAdditionPoint = cli.Character.SpAdditionPoint > 1000000 ? 1000000 : cli.Character.SpAdditionPoint;
                                cli.SendPacket(cli.Character.GenerateSpPoint());
                                cli.SendPacket(cli.Character.GenerateGold());
                                cli.SendPacket(cli.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("WIN_MONEY"), 100000000), 10));
                                cli.SendPacket(cli.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("WIN_REPUT"), cli.Character.Level * 50), 10));
                                cli.SendPacket(cli.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("WIN_FXP"), cli.Character.Level * 4), 10));
                                cli.SendPacket(cli.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("WIN_SP_POINT"), cli.Character.Level * 100), 10));
                            }
                            break;
                        }
                        Thread.Sleep(1000);
                    }
                });

                EventHelper.Instance.ScheduleEvent(TimeSpan.FromMinutes(15), new EventContainer(mapinstance.Item1, EventActionType.DISPOSEMAP, null));
                EventHelper.Instance.ScheduleEvent(TimeSpan.FromMinutes(3), new EventContainer(mapinstance.Item1, EventActionType.SENDPACKET, UserInterfaceHelper.GenerateMsg("Monster Incoming", 0)));
                EventHelper.Instance.ScheduleEvent(TimeSpan.FromMinutes(5), new EventContainer(mapinstance.Item1, EventActionType.SENDPACKET, UserInterfaceHelper.GenerateMsg("10 Minutes", 0)));
                EventHelper.Instance.ScheduleEvent(TimeSpan.FromMinutes(10), new EventContainer(mapinstance.Item1, EventActionType.SENDPACKET, UserInterfaceHelper.GenerateMsg("5 Minutes", 0)));
                EventHelper.Instance.ScheduleEvent(TimeSpan.FromMinutes(11), new EventContainer(mapinstance.Item1, EventActionType.SENDPACKET, UserInterfaceHelper.GenerateMsg("4 Minutes", 0)));
                EventHelper.Instance.ScheduleEvent(TimeSpan.FromMinutes(12), new EventContainer(mapinstance.Item1, EventActionType.SENDPACKET, UserInterfaceHelper.GenerateMsg("3 Minutes", 0)));
                EventHelper.Instance.ScheduleEvent(TimeSpan.FromMinutes(13), new EventContainer(mapinstance.Item1, EventActionType.SENDPACKET, UserInterfaceHelper.GenerateMsg("2 Minutes", 0)));
                EventHelper.Instance.ScheduleEvent(TimeSpan.FromMinutes(14), new EventContainer(mapinstance.Item1, EventActionType.SENDPACKET, UserInterfaceHelper.GenerateMsg("1 Minutes", 0)));
                EventHelper.Instance.ScheduleEvent(TimeSpan.FromMinutes(14.5), new EventContainer(mapinstance.Item1, EventActionType.SENDPACKET, UserInterfaceHelper.GenerateMsg("30 Seconds", 0)));
                EventHelper.Instance.ScheduleEvent(TimeSpan.FromMinutes(0), new EventContainer(mapinstance.Item1, EventActionType.SENDPACKET, UserInterfaceHelper.GenerateMsg("Monster Incoming", 0)));
                EventHelper.Instance.ScheduleEvent(TimeSpan.FromSeconds(10), new EventContainer(mapinstance.Item1, EventActionType.SENDPACKET, UserInterfaceHelper.GenerateMsg("Monster are Here", 0)));

                for (int wave = 0; wave < 4; wave++)
                {
                    EventHelper.Instance.ScheduleEvent(TimeSpan.FromSeconds(130 + (wave * 160)), new EventContainer(mapinstance.Item1, EventActionType.SENDPACKET, UserInterfaceHelper.GenerateMsg("Wave : " + (wave + 1), 0)));
                    EventHelper.Instance.ScheduleEvent(TimeSpan.FromSeconds(160 + (wave * 160)), new EventContainer(mapinstance.Item1, EventActionType.SENDPACKET, UserInterfaceHelper.GenerateMsg("Monster Incoming", 0)));
                    EventHelper.Instance.ScheduleEvent(TimeSpan.FromSeconds(170 + (wave * 160)), new EventContainer(mapinstance.Item1, EventActionType.SENDPACKET, UserInterfaceHelper.GenerateMsg("Monster are Here", 0)));
                    EventHelper.Instance.ScheduleEvent(TimeSpan.FromSeconds(10 + (wave * 160)), new EventContainer(mapinstance.Item1, EventActionType.SPAWNMONSTERS, getInstantBattleMonster(mapinstance.Item1.Map, mapinstance.Item2, wave)));
                    EventHelper.Instance.ScheduleEvent(TimeSpan.FromSeconds(140 + (wave * 160)), new EventContainer(mapinstance.Item1, EventActionType.DROPITEMS, getInstantBattleDrop(mapinstance.Item1.Map, mapinstance.Item2, wave)));
                }
                EventHelper.Instance.ScheduleEvent(TimeSpan.FromSeconds(650), new EventContainer(mapinstance.Item1, EventActionType.SPAWNMONSTERS, getInstantBattleMonster(mapinstance.Item1.Map, mapinstance.Item2, 4)));
            }

            private static IEnumerable<Tuple<short, int, short, short>> generateDrop(Map map, short vnum, int amountofdrop, int amount)
            {
                List<Tuple<short, int, short, short>> dropParameters = new List<Tuple<short, int, short, short>>();
                for (int i = 0; i < amountofdrop; i++)
                {
                    MapCell cell = map.GetRandomPosition();
                    dropParameters.Add(new Tuple<short, int, short, short>(vnum, amount, cell.X, cell.Y));
                }
                return dropParameters;
            }

            private static List<Tuple<short, int, short, short>> getInstantBattleDrop(Map map, short instantbattletype, int wave)
            {
                List<Tuple<short, int, short, short>> dropParameters = new List<Tuple<short, int, short, short>>();
                switch (instantbattletype)
                {
                    default:
                        switch (wave)
                        {
                            case 0:
                                dropParameters.AddRange(generateDrop(map, 1046, 15, 150000));
                                dropParameters.AddRange(generateDrop(map, 1030, 15, 5));
                                dropParameters.AddRange(generateDrop(map, 2282, 15, 10));
                                break;

                            case 1:
                                dropParameters.AddRange(generateDrop(map, 1046, 15, 350000));
                                dropParameters.AddRange(generateDrop(map, 1030, 15, 10));
                                dropParameters.AddRange(generateDrop(map, 2282, 15, 15));
                                break;

                            case 2:
                                dropParameters.AddRange(generateDrop(map, 1046, 15, 750000));
                                dropParameters.AddRange(generateDrop(map, 1030, 15, 15));
                                dropParameters.AddRange(generateDrop(map, 2282, 15, 20));
                                break;

                            case 3:
                                short[] rand_drop = { 2333, 2333, 2333, 9100, 2333, 2333, 2333, 2333, 9120, 2333, 2333, 2333 };
                                short drop_defined = rand_drop[ServerManager.RandomNumber(0, 12)];
                                dropParameters.AddRange(generateDrop(map, 1046, 15, 1150000));
                                dropParameters.AddRange(generateDrop(map, 1030, 15, 20));
                                dropParameters.AddRange(generateDrop(map, 2282, 15, 25));
                                dropParameters.AddRange(generateDrop(map, 1196, 5, 25));
                                dropParameters.AddRange(generateDrop(map, 5871, 1, 25));
                                dropParameters.AddRange(generateDrop(map, 9331, 1, 25));
                                dropParameters.AddRange(generateDrop(map, 9332, 1, 25));
                                dropParameters.AddRange(generateDrop(map, 9333, 1, 25));
                                dropParameters.AddRange(generateDrop(map, 9334, 1, 25));
                                dropParameters.AddRange(generateDrop(map, 9335, 1, 25));
                                dropParameters.AddRange(generateDrop(map, 9336, 1, 25));
                                dropParameters.AddRange(generateDrop(map, 9337, 1, 25));
                                dropParameters.AddRange(generateDrop(map, 9338, 1, 25));
                                dropParameters.AddRange(generateDrop(map, 9339, 1, 25));
                                dropParameters.AddRange(generateDrop(map, 9340, 1, 25));
                                if (drop_defined != 2333)
                                    dropParameters.AddRange(generateDrop(map, drop_defined, 1, 1));
                                else
                                    dropParameters.AddRange(generateDrop(map, drop_defined, 30, 10));
                                break;
                        }
                        break;
                }
                return dropParameters;
            }

            private static List<MonsterToSummon> getInstantBattleMonster(Map map, short instantbattletype, int wave)
            {
                List<MonsterToSummon> SummonParameters = new List<MonsterToSummon>();

                switch (instantbattletype)
                {
                    default:
                        switch (wave)
                        {
                            case 0:
                                SummonParameters.AddRange(map.GenerateMonsters(2619, 1, true, new List<EventContainer>(), true, true, true));
                                SummonParameters.AddRange(map.GenerateMonsters(2043, 40, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(2043, 40, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(2043, 40, true, new List<EventContainer>()));
                                break;

                            case 1:
                                SummonParameters.AddRange(map.GenerateMonsters(2043, 40, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(2043, 40, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(2043, 40, true, new List<EventContainer>()));
                                break;

                            case 2:
                                SummonParameters.AddRange(map.GenerateMonsters(2043, 40, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(2043, 40, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(2043, 40, true, new List<EventContainer>()));
                                break;

                            case 3:
                                SummonParameters.AddRange(map.GenerateMonsters(2043, 40, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(2043, 40, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(2043, 40, true, new List<EventContainer>()));
                                break;

                            case 4:
                                SummonParameters.AddRange(map.GenerateMonsters(2043, 40, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(2043, 40, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(2043, 40, true, new List<EventContainer>()));
                                ServerManager.Instance.StartedEvents.Remove(EventType.FAMILYRAID);
                                break;
                        }
                        break;
                }
                return SummonParameters;
            }

            #endregion
        }

        #endregion
    }
}