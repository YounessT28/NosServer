﻿using OpenNos.Core;
using OpenNos.Data;
using OpenNos.Domain;
using OpenNos.GameObject.Helpers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using OpenNos.GameObject.Networking;
using OpenNos.Master.Library.Client;
using OpenNos.Master.Library.Data;

namespace OpenNos.GameObject.Event.ARENA
{
    internal class ArenaEvent
    {
        #region Methods

        internal static void GenerateTalentArena()
        {
            long groupid = 0;
            int seconds = 0;
            IDisposable obs = Observable.Interval(TimeSpan.FromSeconds(1)).Subscribe(start2 =>
            {
                lock (ServerManager.Instance.ArenaMembers)
                {
                    ServerManager.Instance.ArenaMembers.ToList().Where(s => s.ArenaType == EventType.TALENTARENA).ToList().ForEach(s =>
                    {
                        s.Time -= 1;
                        List<long> groupids = new List<long>();
                        ServerManager.Instance.ArenaMembers.ToList().Where(o => o.GroupId != null).ToList().ForEach(o =>
                        {
                            if (ServerManager.Instance.ArenaMembers.ToList().Count(g => g.GroupId == o.GroupId) != 3)
                            {
                                return;
                            }
                            if (o.GroupId != null)
                            {
                                groupids.Add(o.GroupId.Value);
                            }
                        });

                        if (s.Time > 0)
                        {
                            if (s.GroupId == null)
                            {
                                List<ArenaMember> members = ServerManager.Instance.ArenaMembers.ToList()
                                    .Where(e => e.Session != s.Session && e.ArenaType == EventType.TALENTARENA && e.Session.Character.Level <= s.Session.Character.Level + 5 &&
                                            e.Session.Character.Level >= s.Session.Character.Level - 5).ToList();
                                members.RemoveAll(o => o.GroupId != null && groupids.Contains(o.GroupId.Value));
                                ArenaMember member = members.FirstOrDefault();
                                if (member == null)
                                {
                                    return;
                                }
                                else
                                {
                                    if (member.GroupId == null)
                                    {
                                        groupid++;
                                        member.GroupId = groupid;
                                    }
                                    s.GroupId = member.GroupId;
                                    ServerManager.Instance.ArenaMembers.ToList().Where(e => e.ArenaType == EventType.TALENTARENA && e.GroupId == member.GroupId).ToList().ForEach(o =>
                                    {
                                        o.Session.SendPacket(UserInterfaceHelper.GenerateBSInfo(1, 2, -1, 6));
                                        o.Session.SendPacket(o.Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("ARENA_TEAM_FOUND"), 10));
                                        Observable.Timer(TimeSpan.FromSeconds(1)).Subscribe(time =>
                                        {
                                            s.Time = 300;
                                            if (ServerManager.Instance.ArenaMembers.ToList().Count(g => g.GroupId == s.GroupId) < 3)
                                            {
                                                o.Session.SendPacket(UserInterfaceHelper.GenerateBSInfo(0, 2, s.Time, 8));
                                                o.Session.SendPacket(o.Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("SEARCH_ARENA_TEAM"), 10));
                                            }
                                            else
                                            {
                                                o.Session.SendPacket(UserInterfaceHelper.GenerateBSInfo(0, 2, s.Time, 1));
                                                o.Session.SendPacket(o.Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("SEARCH_RIVAL_ARENA_TEAM"), 10));
                                            }
                                        });
                                    });
                                }
                            }
                            else
                            {
                                if (ServerManager.Instance.ArenaMembers.ToList().Count(g => g.GroupId == s.GroupId) != 3)
                                {
                                    return;
                                }

                                ArenaMember member =
                                    ServerManager.Instance.ArenaMembers.ToList().FirstOrDefault(o => o.GroupId != null && o.GroupId != s.GroupId && groupids.Contains(o.GroupId.Value) &&
                                                                                        o.Session.Character.Level <= s.Session.Character.Level + 5 &&
                                                                                        o.Session.Character.Level >= s.Session.Character.Level - 5);
                                if (member == null)
                                {
                                    return;
                                }

                                MapInstance map = ServerManager.GenerateMapInstance(2015, MapInstanceType.TalentArenaMapInstance, new InstanceBag());
                                map.IsPVP = true;
                                ConcurrentBag<ArenaTeamMember> arenaTeam = new ConcurrentBag<ArenaTeamMember>();
                                ServerManager.Instance.ArenaTeams.Add(arenaTeam);

                                ArenaMember[] arenamembers = ServerManager.Instance.ArenaMembers.ToList().Where(o => o.GroupId == member.GroupId || o.GroupId == s.GroupId).OrderBy(o => o.GroupId)
                                    .ToArray();
                                for (int i = 0; i < 6; i++)
                                {
                                    ItemInstance item = Inventory.InstantiateItemInstance((short)(4433 + (i > 2 ? 5 - i : i)), member.Session.Character.CharacterId);
                                    item.Design = (short)(4433 + (i > 2 ? 5 - i : i));
                                    map.MapDesignObjects.Add(new MapDesignObject
                                    {
                                        ItemInstance = item,
                                        ItemInstanceId = item.Id,
                                        CharacterId = member.Session.Character.CharacterId,
                                        MapX = (short)(i > 2 ? 120 : 19),
                                        MapY = (short)(i > 2 ? 35 + i % 3 * 4 : 36 + i % 3 * 4)
                                    });
                                }
                                map.InstanceBag.Clock.TotalSecondsAmount = 60;
                                map.InstanceBag.Clock.SecondsRemaining = 600;
                                map.InstanceBag.Clock.StartClock();
                                IDisposable obs4 = null;
                                IDisposable obs2 = null;
                                IDisposable obs3 = null;
                                IDisposable obs6 = null;
                                bool WinLoseMsgSent = false;
                                Observable.Timer(TimeSpan.FromSeconds(5)).Subscribe(time2 =>
                                {
                                    obs3 = Observable.Interval(TimeSpan.FromSeconds(5)).Subscribe(effect =>
                                    {
                                        arenamembers.Where(c => !c.Session.Character.Invisible).ToList().ForEach(o => map.Broadcast(o.Session.Character.GenerateEff(o.GroupId == s.GroupId ? 3012 : 3013)));
                                    });
                                });
                                IDisposable obs5 = Observable.Interval(TimeSpan.FromMilliseconds(500)).Subscribe(start3 =>
                                {
                                    map.Broadcast(arenamembers.FirstOrDefault(o => o.Session != null)?.Session.Character.GenerateTaPs());
                                    List<ArenaTeamMember> erenia = arenaTeam.Replace(team => team.ArenaTeamType == ArenaTeamType.ERENIA).ToList();
                                    List<ArenaTeamMember> zenas = arenaTeam.Replace(team => team.ArenaTeamType == ArenaTeamType.ZENAS).ToList();
                                    BuffTeam(erenia);
                                    BuffTeam(zenas);
                                });
                                arenamembers.ToList().ForEach(o =>
                                {
                                    o.Session.SendPacket(UserInterfaceHelper.GenerateBSInfo(0, 2, s.Time, 2));
                                    o.Session.SendPacket(o.Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("RIVAL_ARENA_TEAM_FOUND"), 10));

                                    Observable.Timer(TimeSpan.FromSeconds(1)).Subscribe(time =>
                                    {
                                        o.Session.SendPacket("ta_close");
                                        Observable.Timer(TimeSpan.FromSeconds(5)).Subscribe(time2 =>
                                        {
                                            if (o.Session.Character.IsVehicled)
                                            {
                                                o.Session.Character.RemoveVehicle();
                                            }
                                            Group grp = o.Session.Character.Group;
                                            if (grp != null)
                                            {
                                                grp.LeaveGroup(o.Session);
                                            }
                                            o.Session.Character.Mates.Where(m => m.IsTeamMember).ToList().ForEach(m => m.BackToMiniland());
                                            o.Session.Character.GeneralLogs.Add(new GeneralLogDTO
                                            {
                                                AccountId = o.Session.Account.AccountId,
                                                CharacterId = o.Session.Character.CharacterId,
                                                IpAddress = o.Session.IpAddress,
                                                LogData = "Entry",
                                                LogType = "TalentArena",
                                                Timestamp = DateTime.Now
                                            });
                                            List<BuffType> bufftodisable = new List<BuffType> { BuffType.Bad, BuffType.Good, BuffType.Neutral };
                                            o.Session.Character.DisableBuffs(bufftodisable);
                                            int i = Array.IndexOf(arenamembers, o) + 1;
                                            o.Session.Character.Hp = (int)o.Session.Character.HPLoad();
                                            o.Session.Character.Mp = (int)o.Session.Character.MPLoad();
                                            ServerManager.Instance.ChangeMapInstance(o.Session.Character.CharacterId, map.MapInstanceId, o.GroupId == member.GroupId ? 125 : 14,
                                                (o.GroupId == member.GroupId ? 37 : 38) + i % 3 * 2);
                                            o.Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("SELECT_ORDER_ARENA_TIME"), 0));
                                            o.Session.SendPacket(o.Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("SELECT_ORDER_ARENA_TIME"), 10));
                                            o.Session.SendPacket(o.Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("SELECT_ORDER_ARENA"), 10));
                                            o.Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("SELECT_ORDER_ARENA"), 0));
                                            o.Session.SendPacket(o.Session.Character.GenerateTaM(0));
                                            o.Session.SendPacket("ta_sv 0");
                                            o.Session.SendPacket(UserInterfaceHelper.Instance.GenerateTaSt(TalentArenaOptionType.Watch));
                                            o.Session.SendPacket(o.Session.Character.GenerateTaM(3));

                                            o.Session.SendPacket(o.Session.Character.GenerateSay(Language.Instance.GetMessageFromKey(o.GroupId == s.GroupId ? "ZENAS" : "ERENIA"), 10));
                                            arenaTeam.Add(new ArenaTeamMember(o.Session, o.GroupId == s.GroupId ? ArenaTeamType.ZENAS : ArenaTeamType.ERENIA, null));
                                            o.Session.SendPacket(o.Session.Character.GenerateTaP(0, false));

                                            obs2 = Observable.Interval(TimeSpan.FromMilliseconds(100)).Subscribe(start3 =>
                                            {
                                                bool resettap = false;
                                                map.MapDesignObjects.ToList().ForEach(e =>
                                                {
                                                    if (e.ItemInstance.Design >= 4433 && e.ItemInstance.Design <= 4435)
                                                    {
                                                        Character chara = map.GetCharactersInRange(e.MapX, e.MapY, 0).FirstOrDefault();
                                                        if (chara != null)
                                                        {
                                                            resettap = true;
                                                            ArenaTeamMember teammember = arenaTeam.FirstOrDefault(at => at.Session == chara.Session);
                                                            if (teammember != null &&
                                                                !arenaTeam.Any(at => at.Order == (e.ItemInstance.ItemVNum - 4433) &&
                                                                                     at.ArenaTeamType == (e.MapX == 120 ? ArenaTeamType.ERENIA : ArenaTeamType.ZENAS)))
                                                            {
                                                                if (teammember.Order != null)
                                                                {
                                                                    MapDesignObject obj =
                                                                        map.MapDesignObjects.FirstOrDefault(mapobj => mapobj.ItemInstance.ItemVNum == e.ItemInstance.ItemVNum &&
                                                                                                                      e.MapX == (teammember.ArenaTeamType == ArenaTeamType.ERENIA ? 120 : 19));
                                                                    if (obj != null)
                                                                    {
                                                                        obj.ItemInstance.Design = obj.ItemInstance.ItemVNum;
                                                                    }
                                                                }
                                                                teammember.Order = (byte)(e.ItemInstance.ItemVNum - 4433);
                                                            }
                                                        }
                                                    }
                                                    else if (e.ItemInstance.Design == 4436)
                                                    {
                                                        if (!map.GetCharactersInRange(e.MapX, e.MapY, 0).Any())
                                                        {
                                                            resettap = true;
                                                            ArenaTeamMember teammember = arenaTeam.FirstOrDefault(at =>
                                                                at.Order == (e.ItemInstance.ItemVNum - 4433) && at.ArenaTeamType == (e.MapX == 120 ? ArenaTeamType.ERENIA : ArenaTeamType.ZENAS));
                                                            if (teammember != null)
                                                            {
                                                                teammember.Order = null;
                                                            }
                                                        }
                                                    }
                                                    if (!arenaTeam.Any(at => at.Order == (e.ItemInstance.ItemVNum - 4433) && at.ArenaTeamType == (e.MapX == 120 ? ArenaTeamType.ERENIA : ArenaTeamType.ZENAS)))
                                                    {
                                                        if (e.ItemInstance.Design != 4436)
                                                        {
                                                            return;
                                                        }
                                                        e.ItemInstance.Design = e.ItemInstance.ItemVNum;
                                                        map.Broadcast(e.GenerateEffect(false));
                                                    }
                                                    else if (e.ItemInstance.Design != 4436)
                                                    {
                                                        e.ItemInstance.Design = 4436;
                                                        map.Broadcast(e.GenerateEffect(false));
                                                    }
                                                });

                                                if (resettap)
                                                {
                                                    arenaTeam.ToList().ForEach(arenauser => { arenauser.Session.SendPacket(arenauser.Session.Character.GenerateTaP(2, false)); });
                                                }
                                            });
                                        });
                                    });
                                });
                                Observable.Timer(TimeSpan.FromSeconds(map.InstanceBag.Clock.TotalSecondsAmount)).Subscribe(start =>
                                {
                                    obs2.Dispose();
                                    arenaTeam.ToList().ForEach(arenauser =>
                                    {
                                        if (arenauser.Order == null)
                                        {
                                            for (byte x = 0; x < 3; x++)
                                            {
                                                if (!arenaTeam.Any(at => at.ArenaTeamType == arenauser.ArenaTeamType && at.Order == x))
                                                {
                                                    arenauser.Order = x;
                                                }
                                            }
                                        }
                                        arenauser.Session.SendPacket($"ta_pn {arenauser.Order + 1}");
                                    });
                                    map.MapDesignObjects.ToList().ForEach(md => map.Broadcast(md.GenerateEffect(true)));
                                    map.MapDesignObjects.Clear();

                                    arenaTeam.ToList().ForEach(arenauser => arenauser.Session.SendPacket(arenauser.Session.Character.GenerateTaP(2, true)));

                                    bool newround1 = true;
                                    bool newround2 = true;
                                    int count1 = 0;
                                    int count2 = 0;
                                    IDisposable obs7 = obs4;

                                    ArenaTeamMember beforetm = null;
                                    ArenaTeamMember beforetm2 = null;

                                    obs4 = Observable.Interval(TimeSpan.FromMilliseconds(500)).Subscribe(start3 =>
                                    {
                                        int ereniacount = arenaTeam.Count(at => at.Dead && at.ArenaTeamType == ArenaTeamType.ERENIA);
                                        int zenascount = arenaTeam.Count(at => at.Dead && at.ArenaTeamType == ArenaTeamType.ZENAS);
                                        if (ServerManager.Instance.ArenaTeams.ToList().Find(a => a.Any(b => arenaTeam.Contains(b))) is ConcurrentBag<ArenaTeamMember> updatedTeam)
                                        {
                                            arenaTeam = updatedTeam;
                                        }
                                        else
                                        {
                                            arenaTeam.Clear();
                                            obs2.Dispose();
                                            obs3.Dispose();
                                            obs5.Dispose();
                                            obs7?.Dispose();
                                            map.Dispose();
                                            ServerManager.Instance.ArenaMembers.RemoveAll(o => o.GroupId == member.GroupId || o.GroupId == s.GroupId);
                                            obs4.Dispose();
                                            return;
                                        }
                                        if (count1 != ereniacount || count2 != zenascount)
                                        {
                                            if (count1 != ereniacount)
                                            {
                                                newround1 = true;
                                            }
                                            if (count2 != zenascount)
                                            {
                                                newround2 = true;
                                            }
                                            count1 = ereniacount;
                                            count2 = zenascount;
                                        }

                                        ArenaTeamMember tm = arenaTeam.OrderBy(tm3 => tm3.Order).FirstOrDefault(tm3 => tm3.ArenaTeamType == ArenaTeamType.ERENIA && !tm3.Dead);
                                        ArenaTeamMember tm2 = arenaTeam.OrderBy(tm3 => tm3.Order).FirstOrDefault(tm3 => tm3.ArenaTeamType == ArenaTeamType.ZENAS && !tm3.Dead);

                                        if (tm == beforetm && tm2 == beforetm2)
                                        {
                                            return;
                                        }
                                        else
                                        {
                                            if (tm != null)
                                            {
                                                beforetm = tm;
                                            }
                                            if (tm2 != null)
                                            {
                                                beforetm2 = tm2;
                                            }
                                        }

                                        if (!newround1 && !newround2 && tm != null && tm2 != null)
                                        {
                                            return;
                                        }

                                        int timeToWait = 0;

                                        try
                                        {
                                            timeToWait = tm?.Order == arenaTeam.OrderBy(tm3 => tm3.Order).FirstOrDefault(tm3 => tm3.ArenaTeamType == ArenaTeamType.ERENIA).Order && tm2?.Order == arenaTeam.OrderBy(tm3 => tm3.Order).FirstOrDefault(tm3 => tm3.ArenaTeamType == ArenaTeamType.ZENAS).Order ? 0 : 3;
                                        }
                                        catch (Exception ex)
                                        {
                                            Logger.Error($"Talent Arena error: {ex}");
                                            return;
                                        }

                                        map.InstanceBag.Clock.TotalSecondsAmount = 300 + timeToWait;
                                        map.InstanceBag.Clock.SecondsRemaining = 3000 + timeToWait * 10;

                                        arenaTeam.ToList().ForEach(friends =>
                                        {
                                            friends.Session.SendPacket(friends.Session.Character.GenerateTaM(2));
                                            friends.Session.SendPacket(friends.Session.Character.GenerateTaM(3));
                                        });
                                        map.Sessions.Except(arenaTeam.Select(ss => ss.Session)).ToList().ForEach(o =>
                                        {
                                            o.SendPacket(tm2?.Session.Character.GenerateTaM(2));
                                            o.SendPacket(tm2?.Session.Character.GenerateTaM(3));
                                        });

                                        obs6?.Dispose();
                                        obs6 = Observable.Timer(TimeSpan.FromSeconds(map.InstanceBag.Clock.TotalSecondsAmount)).Subscribe(start4 =>
                                        {
                                            if (tm2 != null && tm != null)
                                            {
                                                tm.Dead = true;
                                                tm2.Dead = true;
                                                tm.Session.Character.PositionX = 120;
                                                tm.Session.Character.PositionY = 39;
                                                tm2.Session.Character.PositionX = 19;
                                                tm2.Session.Character.PositionY = 40;
                                                map.Broadcast(tm2.Session, tm.Session.Character.GenerateTp());
                                                map.Broadcast(tm2.Session, tm2.Session.Character.GenerateTp());
                                                tm.Session.SendPacket(UserInterfaceHelper.Instance.GenerateTaSt(TalentArenaOptionType.Watch));
                                                tm2.Session.SendPacket(UserInterfaceHelper.Instance.GenerateTaSt(TalentArenaOptionType.Watch));

                                                List<BuffType> bufftodisable = new List<BuffType> { BuffType.Bad };

                                                tm.Session.Character.DisableBuffs(bufftodisable);
                                                tm.Session.Character.Hp = (int)tm.Session.Character.HPLoad();
                                                tm.Session.Character.Mp = (int)tm.Session.Character.MPLoad();
                                                tm.Session.SendPacket(tm.Session.Character.GenerateStat());

                                                tm2.Session.Character.DisableBuffs(bufftodisable);
                                                tm2.Session.Character.Hp = (int)tm2.Session.Character.HPLoad();
                                                tm2.Session.Character.Mp = (int)tm2.Session.Character.MPLoad();
                                                tm2.Session.SendPacket(tm2.Session.Character.GenerateStat());

                                                arenaTeam.Replace(friends => friends.ArenaTeamType == tm.ArenaTeamType).ToList().ForEach(friends =>
                                                {
                                                    friends.Session.SendPacket(friends.Session.Character.GenerateTaFc(0));
                                                });
                                            }
                                            newround1 = true;
                                            newround2 = true;
                                            arenaTeam.ToList().ForEach(arenauser => { arenauser.Session.SendPacket(arenauser.Session.Character.GenerateTaP(2, true)); });
                                        });

                                        if (tm != null && tm2 != null)
                                        {
                                            map.IsPVP = false;

                                            arenaTeam.ToList().ForEach(friends =>
                                            {
                                                friends.Session.SendPacket(friends.ArenaTeamType == ArenaTeamType.ERENIA
                                                    ? tm.Session.Character.GenerateTaFc(0)
                                                    : tm2.Session.Character.GenerateTaFc(0));
                                            });

                                            map.Sessions.Except(arenaTeam.Select(ss => ss.Session)).ToList().ForEach(ss =>
                                            {
                                                ss.SendPacket(tm.Session.Character.GenerateTaFc(0));
                                                ss.SendPacket(tm2.Session.Character.GenerateTaFc(1));
                                            });

                                            if (newround1)
                                            {
                                                Observable.Timer(TimeSpan.FromSeconds(timeToWait)).Subscribe(start5 =>
                                                {
                                                    map.Broadcast(tm.Session.Character.GenerateTaFc(1));
                                                    tm.Session.Character.PositionX = 87;
                                                    tm.Session.Character.PositionY = 39;
                                                    map.Broadcast(tm.Session, tm.Session.Character.GenerateTp());
                                                });
                                            }
                                            if (newround2)
                                            {
                                                Observable.Timer(TimeSpan.FromSeconds(timeToWait)).Subscribe(start5 =>
                                                {
                                                    tm2.Session.Character.PositionX = 56;
                                                    tm2.Session.Character.PositionY = 40;
                                                    map.Broadcast(tm2.Session, tm2.Session.Character.GenerateTp());
                                                });
                                            }
                                            Observable.Timer(TimeSpan.FromSeconds(timeToWait)).Subscribe(start5 =>
                                            {
                                                arenaTeam.Replace(at => at.LastSummoned != null).ToList().ForEach(at =>
                                                {
                                                    at.LastSummoned = null;
                                                    at.Session.Character.PositionX = at.ArenaTeamType == ArenaTeamType.ERENIA ? (short)120 : (short)19;
                                                    at.Session.Character.PositionY = at.ArenaTeamType == ArenaTeamType.ERENIA ? (short)39 : (short)40;
                                                    at.Session.CurrentMapInstance.Broadcast(at.Session.Character.GenerateTp());
                                                    at.Session.SendPacket(UserInterfaceHelper.Instance.GenerateTaSt(TalentArenaOptionType.Watch));

                                                    List<BuffType> bufftodisable = new List<BuffType> { BuffType.Bad };
                                                    at.Session.Character.DisableBuffs(bufftodisable);
                                                    at.Session.Character.Hp = (int)at.Session.Character.HPLoad();
                                                    at.Session.Character.Mp = (int)at.Session.Character.MPLoad();
                                                });

                                                tm.Session.SendPacket(UserInterfaceHelper.Instance.GenerateTaSt(TalentArenaOptionType.Call));
                                                tm2.Session.SendPacket(UserInterfaceHelper.Instance.GenerateTaSt(TalentArenaOptionType.Call));

                                                map.Broadcast("ta_s");
                                                Observable.Timer(TimeSpan.FromSeconds(5)).Subscribe(start4 => { map.IsPVP = true; });
                                            });
                                        }
                                        else
                                        {
                                            switch (tm)
                                            {
                                                case null when tm2 == null:
                                                    if (!WinLoseMsgSent)
                                                    {
                                                        map.Broadcast(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("EQUALITY"), 0));
                                                        arenaTeam.ToList().ForEach(arenauser =>
                                                        {
                                                            arenauser.Session.SendPacket(arenauser.Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("EQUALITY"), 10));
                                                            arenauser.Session.SendPacket(arenauser.Session.Character.GenerateTaF(3));
                                                        });
                                                        map.Sessions.Except(arenamembers.Select(x => x.Session)).ToList().ForEach(
                                                            x =>
                                                            {
                                                                ArenaTeamMember arenauser = arenaTeam.FirstOrDefault(se => se.Session != null);
                                                                if (arenauser == null)
                                                                {
                                                                    return;
                                                                }
                                                                x.SendPacket(arenauser.Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("EQUALITY"), 10));
                                                                x.SendPacket(arenauser.Session.Character.GenerateTaF(0));
                                                            }
                                                        );
                                                        WinLoseMsgSent = true;
                                                    }
                                                    break;

                                                case null:
                                                    if (!WinLoseMsgSent)
                                                    {
                                                        map.Broadcast(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("VICTORIOUS_ZENAS"), 0));

                                                        arenaTeam.ToList().ForEach(arenauser =>
                                                        {
                                                            Observable.Timer(TimeSpan.FromSeconds(2)).Subscribe(WinZenas =>
                                                            {
                                                                SendRewards(arenauser, arenauser.ArenaTeamType == ArenaTeamType.ZENAS);
                                                                arenauser.Session.SendPacket(arenauser.Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("VICTORIOUS_ZENAS"), 10));
                                                                arenauser.Session.SendPacket(arenauser.Session.Character.GenerateTaF(1));
                                                            });

                                                        });
                                                        WinLoseMsgSent = true;
                                                    }
                                                    break;

                                                default:
                                                    if (!WinLoseMsgSent)
                                                    {
                                                        map.Broadcast(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("VICTORIOUS_ERENIA"), 0));
                                                        arenaTeam.ToList().ForEach(arenauser =>
                                                        {
                                                            SendRewards(arenauser, arenauser.ArenaTeamType == ArenaTeamType.ERENIA);
                                                            arenauser.Session.SendPacket(arenauser.Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("VICTORIOUS_ERENIA"), 10));
                                                            arenauser.Session.SendPacket(arenauser.Session.Character.GenerateTaF(2));
                                                        });
                                                        WinLoseMsgSent = true;
                                                    }
                                                    break;
                                            }
                                            map.IsPVP = false;
                                            obs3.Dispose();
                                            obs2.Dispose();
                                            obs7?.Dispose();
                                            obs5.Dispose();
                                            Observable.Timer(TimeSpan.FromSeconds(5)).Subscribe(start4 => //Observable.Timer(TimeSpan.FromSeconds(30)).Subscribe(start4 =>
                                            {
                                                arenaTeam.ToList().ForEach(o =>
                                                {
                                                    if (o.Session?.CurrentMapInstance?.MapInstanceType == MapInstanceType.TalentArenaMapInstance)
                                                    {
                                                        o.Session.Character.GetSkills().ForEach(c => c.LastUse = DateTime.Now.AddDays(-1));

                                                        o.Session.SendPacket(o.Session.Character.GenerateSki());
                                                        o.Session.SendPackets(o.Session.Character.GenerateQuicklist());

                                                        List<BuffType> bufftodisable = new List<BuffType> { BuffType.Bad };
                                                        o.Session.Character.DisableBuffs(bufftodisable);
                                                        o.Session.Character.RemoveBuff(491);
                                                        ServerManager.Instance.TeleportOnRandomPlaceInMap(o.Session, ServerManager.Instance.ArenaInstance.MapInstanceId);
                                                    }
                                                });
                                                map.Sessions.ToList().ForEach(sess =>
                                                {
                                                    if (sess.CurrentMapInstance?.MapInstanceType == MapInstanceType.TalentArenaMapInstance)
                                                    {
                                                        List<BuffType> bufftodisable = new List<BuffType> { BuffType.Bad };
                                                        sess.Character.DisableBuffs(bufftodisable);
                                                        sess.Character.RemoveBuff(491);
                                                        ServerManager.Instance.TeleportOnRandomPlaceInMap(sess, ServerManager.Instance.ArenaInstance.MapInstanceId);
                                                    }
                                                });
                                                map.Dispose();
                                            });
                                        }
                                        newround1 = false;
                                        newround2 = false;
                                    });
                                });
                                ServerManager.Instance.ArenaMembers.ToList().Where(o => o.GroupId == member.GroupId || o.GroupId == s.GroupId).ToList()
                                    .ForEach(se => { se.Session.SendPacket(UserInterfaceHelper.GenerateBSInfo(2, 2, 0, 0)); });

                                ServerManager.Instance.ArenaMembers.RemoveAll(o => o.GroupId == member.GroupId || o.GroupId == s.GroupId);
                            }
                        }
                        else
                        {
                            if (s.GroupId == null)
                            {
                                if (s.Time != -1)
                                {
                                    s.Session.SendPacket(UserInterfaceHelper.GenerateBSInfo(1, 2, s.Time, 7));
                                    s.Session.SendPacket(s.Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("NO_TEAM_ARENA"), 10));
                                }
                                s.Time = 300;
                                s.Session.SendPacket(UserInterfaceHelper.GenerateBSInfo(1, 2, s.Time, 5));
                                s.Session.SendPacket(s.Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("SEARCH_ARENA_TEAM"), 10));
                            }
                            else if (ServerManager.Instance.ArenaMembers.ToList().Count(g => g.GroupId == s.GroupId) < 3)
                            {
                                s.Session.SendPacket(UserInterfaceHelper.GenerateBSInfo(1, 2, -1, 4));
                                Observable.Timer(TimeSpan.FromSeconds(1)).Subscribe(time =>
                                {
                                    s.Time = 300;
                                    s.Session.SendPacket(UserInterfaceHelper.GenerateBSInfo(1, 2, s.Time, 8));
                                    s.Session.SendPacket(s.Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("RETRY_SEARCH_ARENA_TEAM"), 10));
                                });
                            }
                            else
                            {
                                s.Session.SendPacket(UserInterfaceHelper.GenerateBSInfo(0, 2, -1, 3));
                                Observable.Timer(TimeSpan.FromSeconds(1)).Subscribe(time =>
                                {
                                    s.Time = 300;
                                    s.Session.SendPacket(UserInterfaceHelper.GenerateBSInfo(0, 2, s.Time, 1));
                                    s.Session.SendPacket(s.Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("SEARCH_RIVAL_ARENA_TEAM"), 10));
                                });
                            }
                        }
                    });
                    seconds++;
                }
            });

            CommunicationServiceClient.Instance.SendMessageToCharacter(new SCSCharacterMessage
            {
                SourceWorldId = ServerManager.Instance.WorldId,
                Message = UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("TALENT_ARENA_OPENED"), ServerManager.Instance.ChannelId), 0),
                Type = MessageType.Broadcast
            });

            //Observable.Timer(TimeSpan.FromHours(7)).Subscribe(start2 =>
            //{
            //    ServerManager.Instance.StartedEvents.Remove(EventType.TALENTARENA);
            //    obs.Dispose();
            //});
        }

        private static void BuffTeam(List<ArenaTeamMember> team)
        {
            //Buff y debuff de arena quitados, ya que no me parece justo
            if (team.Count(ch => ch.Session?.Character.Class == ClassType.Archer) <= 1 ||
                team.Count(ch => ch.Session?.Character.Class == ClassType.Magician) <= 1 ||
                team.Count(ch => ch.Session?.Character.Class == ClassType.Swordsman) <= 1 ||
                team.Count(ch => ch.Session?.Character.Class == ClassType.MartialArtist) <= 1)
            {
                //buff team
                team.ForEach(sess =>
                {
                    if (sess.Session?.Character.Buff.Any(bf => bf.Card.CardId == 491) == false)
                    {
                        if (ServerManager.Instance.Configuration.BCardsInArenaTalent)
                        {
                            sess.Session?.Character.AddBuff(new Buff(491, 1), sess.Session?.Character.BattleEntity);
                        }
                        else
                        {
                            sess.Session.SendPacket(sess.Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("TALENT_ARENA_NOT_BCARDS"), 10));
                        }
                    }
                });
            }

            if (team.Count(ch => ch.Session?.Character.Class == ClassType.Archer) == 3 ||
                team.Count(ch => ch.Session?.Character.Class == ClassType.Magician) == 3 ||
                team.Count(ch => ch.Session?.Character.Class == ClassType.Swordsman) == 3 ||
                team.Count(ch => ch.Session?.Character.Class == ClassType.MartialArtist) == 3)
            {
                //debuff team
                team.ForEach(sess =>
                {
                    if (sess.Session?.Character.Buff.Any(bf => bf.Card.CardId == 490) == false)
                    {
                        if (ServerManager.Instance.Configuration.BCardsInArenaTalent)
                        {
                            sess.Session?.Character.AddBuff(new Buff(490, 1), sess.Session?.Character.BattleEntity);
                        }
                        else
                        {
                            sess.Session.SendPacket(sess.Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("TALENT_ARENA_NOT_BCARDS"), 10));
                        }
                    }
                });
            }
        }

        private static void SendRewards(ArenaTeamMember member, bool win)
        {
            ItemInstance SpInstance = null;
            if (member.Session.Character.Inventory != null && member.Session.Character.UseSp)
            {
                SpInstance = (member.Session.Character.Inventory.LoadBySlotAndType((byte)EquipmentType.Sp, InventoryType.Wear));
            }
            if (win)
            {
                member.Session.Character.GetXp(RewardsHelper.ArenaXpReward(member.Session.Character.Level), false);
                member.Session.Character.GetJobExp(RewardsHelper.ArenaXpReward(SpInstance != null ? SpInstance.SpLevel : member.Session.Character.JobLevel), false);
                member.Session.Character.GetReputation(500);
                member.Session.Character.GiftAdd(2800, 1);
                member.Session.Character.GetGold(member.Session.Character.Level * 1000);
                member.Session.Character.TalentWin++;
            }
            else
            {
                member.Session.Character.GetXp(RewardsHelper.ArenaXpReward(member.Session.Character.Level) / 2, false);
                member.Session.Character.GetJobExp(RewardsHelper.ArenaXpReward(SpInstance != null ? SpInstance.SpLevel : member.Session.Character.JobLevel) / 2, false);
                member.Session.Character.GetReputation(200);
                member.Session.Character.GiftAdd(2801, 3);
                member.Session.Character.GetGold(member.Session.Character.Level * 500);
                member.Session.Character.TalentLose++;
            }
        }

        #endregion
    }
}