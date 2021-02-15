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
using OpenNos.GameObject.Networking;
using OpenNos.Master.Library.Client;
using OpenNos.Master.Library.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;

namespace OpenNos.GameObject.Event
{
    public static class BlackNightRaid
    {
        #region Properties

        public static int AngelDamage { get; set; }

        public static MapInstance BlackNightInstance { get; set; }

        public static int DemonDamage { get; set; }

        public static bool IsLocked { get; set; }

        public static bool IsRunning { get; set; }

        public static bool winningFaction { get; set; }

        public static int RemainingTime { get; set; }

        public static MapInstance UnknownLandMapInstance { get; set; }

        #endregion

        #region Methods

        public static void Run()
        {
            BlackNightThread raidThread = new BlackNightThread();
            Observable.Timer(TimeSpan.FromMinutes(0)).Subscribe(X => raidThread.Run());
        }

        #endregion
    }

    public class BlackNightThread
    {
        #region Methods

        public void Run()
        {
            BlackNightRaid.RemainingTime = 1800;
            const int interval = 3;

            BlackNightRaid.BlackNightInstance = ServerManager.GenerateMapInstance(185, MapInstanceType.BlackNightInstance, new InstanceBag());
            BlackNightRaid.UnknownLandMapInstance = ServerManager.GetMapInstance(ServerManager.GetBaseMapInstanceIdByMapId(134));

            BlackNightRaid.BlackNightInstance.CreatePortal(new Portal
            {
                SourceMapId = 185,
                SourceX = 143,
                SourceY = 43,
                DestinationMapId = 134,
                DestinationX = 91,
                DestinationY = 189,
                Type = -1
            });
            BlackNightRaid.BlackNightInstance.CreatePortal(new Portal
            {
                SourceMapId = 185,
                SourceX = 158,
                SourceY = 203,
                DestinationMapId = 134,
                DestinationX = 198,
                DestinationY = 189,
                Type = -1
            });


            BlackNightRaid.UnknownLandMapInstance.CreatePortal(new Portal
            {
                SourceMapId = 134,
                SourceX = 91,
                SourceY = 189,
                DestinationMapInstanceId = BlackNightRaid.BlackNightInstance.MapInstanceId,
                DestinationX = 158,
                DestinationY = 203,
                Type = -1
            });
            BlackNightRaid.UnknownLandMapInstance.CreatePortal(new Portal
            {
                SourceMapId = 134,
                SourceX = 198,
                SourceY = 189,
                DestinationMapInstanceId = BlackNightRaid.BlackNightInstance.MapInstanceId,
                DestinationX = 143,
                DestinationY = 43,
                Type = -1
            });


            List<EventContainer> onDeathEvents = new List<EventContainer>
            {
                new EventContainer(BlackNightRaid.BlackNightInstance, EventActionType.SCRIPTEND, (byte)1)
            };

            MapMonster BlackNightMonster = new MapMonster
            {
                MonsterVNum = 601,
                MapY = 83,
                MapX = 119,
                MapId = BlackNightRaid.BlackNightInstance.Map.MapId,
                Position = 0,
                IsMoving = true,
                MapMonsterId = BlackNightRaid.BlackNightInstance.GetNextMonsterId(),
                ShouldRespawn = false,
                IsBoss = true,
                IsHostile = true,
            };
            BlackNightMonster.Initialize(BlackNightRaid.BlackNightInstance);
            BlackNightRaid.BlackNightInstance.AddMonster(BlackNightMonster);

            MapMonster BlackNight = BlackNightRaid.BlackNightInstance.Monsters.Find(s => s.Monster.NpcMonsterVNum == 1);

            if (BlackNight != null)
            {
                BlackNight.BattleEntity.OnDeathEvents = onDeathEvents;
                BlackNight.IsBoss = true;
            }

            try
            {

                CommunicationServiceClient.Instance.SendMessageToCharacter(new SCSCharacterMessage
                {
                    DestinationCharacterId = null,
                    SourceWorldId = ServerManager.Instance.WorldId,
                    Message = $"Le chevalier noir s'est déchanier",
                    Type = MessageType.Shout
                });

            }
            catch (Exception ex)
            {

            }

            RefreshRaid(BlackNightRaid.RemainingTime);

            ServerManager.Instance.Act4RaidStart = DateTime.Now;

            while (BlackNightRaid.RemainingTime > 0)
            {
                BlackNightRaid.RemainingTime -= interval;
                Thread.Sleep(interval * 1000);
                RefreshRaid(BlackNightRaid.RemainingTime);
            }


            EndRaid();
        }

        private void EndRaid()
        {
            ServerManager.Shout(Language.Instance.GetMessageFromKey("CALIGOR_END"), true);

            foreach (Portal p in BlackNightRaid.UnknownLandMapInstance.Portals.Where(s => s.DestinationMapInstanceId == BlackNightRaid.BlackNightInstance.MapInstanceId).ToList())
            {
                p.IsDisabled = true;
                BlackNightRaid.UnknownLandMapInstance.Broadcast(p.GenerateGp());
                BlackNightRaid.UnknownLandMapInstance.Portals.Remove(p);
            }
            foreach (ClientSession sess in BlackNightRaid.BlackNightInstance.Sessions.ToList())
            {
                ServerManager.Instance.ChangeMapInstance(sess.Character.CharacterId, BlackNightRaid.UnknownLandMapInstance.MapInstanceId, sess.Character.MapX, sess.Character.MapY);
                Thread.Sleep(100);
            }
            EventHelper.Instance.RunEvent(new EventContainer(BlackNightRaid.BlackNightInstance, EventActionType.DISPOSEMAP, null));
            BlackNightRaid.IsRunning = false;
            BlackNightRaid.AngelDamage = 0;
            BlackNightRaid.DemonDamage = 0;
            ServerManager.Instance.StartedEvents.Remove(EventType.BLACKNIGHT);
        }

        private void LockRaid()
        {
            foreach (Portal p in BlackNightRaid.UnknownLandMapInstance.Portals.Where(s => s.DestinationMapInstanceId == BlackNightRaid.BlackNightInstance.MapInstanceId).ToList())
            {
                p.IsDisabled = true;
                BlackNightRaid.UnknownLandMapInstance.Broadcast(p.GenerateGp());
                p.IsDisabled = false;
                p.Type = (byte)PortalType.Closed;
                BlackNightRaid.UnknownLandMapInstance.Broadcast(p.GenerateGp());
            }
            ServerManager.Shout(Language.Instance.GetMessageFromKey("CALIGOR_LOCKED"), true);
            BlackNightRaid.IsLocked = true;
        }

        private void RefreshRaid(int remaining)
        {
            int maxHP = ServerManager.GetNpcMonster(601).MaxHP;
            BlackNightRaid.BlackNightInstance.Broadcast(UserInterfaceHelper.GenerateCHDM(maxHP, BlackNightRaid.AngelDamage, BlackNightRaid.DemonDamage, BlackNightRaid.RemainingTime));

            if (((maxHP / 10) * 8 < BlackNightRaid.AngelDamage + BlackNightRaid.DemonDamage) && !BlackNightRaid.IsLocked)
            {
                LockRaid();
            }
        }

        #endregion
    }
}