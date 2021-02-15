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
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using OpenNos.GameObject.Networking;
using OpenNos.Master.Library.Client;
using System.Net.NetworkInformation;

namespace OpenNos.GameObject.Event
{
    public static class ACT5SHIP
    {
        #region Methods

        public static void GenerateAct5Ship()
        {
            EventHelper.Instance.RunEvent(new EventContainer(ServerManager.GetMapInstance(ServerManager.GetBaseMapInstanceIdByMapId(145)), EventActionType.NPCSEFFECTCHANGESTATE, true));
            DateTime result = Core.Extensions.TimeExtensions.RoundUp(DateTime.Now, TimeSpan.FromMinutes(5));
            Observable.Timer(result - DateTime.Now).Subscribe(X => Act5ShipThread.Run());
        }
        #endregion
    }

    public static class Act5ShipThread
    {
        #region Methods

        public static void Run()
        {
            MapInstance map = ServerManager.GenerateMapInstance(148, MapInstanceType.Act5Ship, new InstanceBag());
            MapNpc mapNpc1 = new MapNpc
            {
                NpcVNum = 1350,
                MapNpcId = map.GetNextNpcId(),
                Dialog = 447,
                MapId = 148,
                MapX = 31,
                MapY = 28,
                IsMoving = false,
                Position = 3,
                IsSitting = false
            };
            mapNpc1.Initialize(map);
            map.AddNPC(mapNpc1);
            while (true)
            {
                OpenShip();
                Thread.Sleep(60 * 1000);
                map.Broadcast(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("SHIP_MINUTES"), 4), 0));
                Thread.Sleep(60 * 1000);
                map.Broadcast(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("SHIP_MINUTES"), 3), 0));
                Thread.Sleep(60 * 1000);
                map.Broadcast(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("SHIP_MINUTES"), 2), 0));
                Thread.Sleep(60 * 1000);
                map.Broadcast(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("SHIP_MINUTE"), 0));
                LockShip();
                Thread.Sleep(30 * 1000);
                map.Broadcast(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("SHIP_SECONDS"), 30), 0));
                Thread.Sleep(20 * 1000);
                map.Broadcast(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("SHIP_SECONDS"), 10), 0));
                Thread.Sleep(10 * 1000);
                map.Broadcast(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("SHIP_SETOFF"), 0));
                List<ClientSession> sessions = map.Sessions.Where(s => s?.Character != null).ToList();
                Observable.Timer(TimeSpan.FromSeconds(0)).Subscribe(X => TeleportPlayers(sessions));
            }
        }

        private static void LockShip() => EventHelper.Instance.RunEvent(new EventContainer(ServerManager.GetMapInstance(ServerManager.GetBaseMapInstanceIdByMapId(145)), EventActionType.NPCSEFFECTCHANGESTATE, true));

        private static void OpenShip() => EventHelper.Instance.RunEvent(new EventContainer(ServerManager.GetMapInstance(ServerManager.GetBaseMapInstanceIdByMapId(145)), EventActionType.NPCSEFFECTCHANGESTATE, false));

        private static void TeleportPlayers(List<ClientSession> sessions)
        {
            foreach (ClientSession session in sessions)
            {
                if (ServerManager.Instance.InShutdown == false)
                {
                    session.Character.Gold -= 30000;
                    session.SendPacket(session.Character.GenerateGold());
                    ServerManager.Instance.ChangeMap(session.Character.CharacterId, 170, (short)(127 + ServerManager.RandomNumber(-2, 2)), (short)(46 + ServerManager.RandomNumber(-2, 2)));
                }

                #endregion
            }
        }
    }
}