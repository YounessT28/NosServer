using OpenNos.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OpenNos.GameObject
{
    class RainbowBattleTeamMember
    {
        public ClientSession Session { get; set; }
        public RainbowBattleTeamType RainbowBattleTeamType { get; set; }
        public bool Dead { get; set; }
        public bool Freezed { get; set; }

        public RainbowBattleTeamMember(ClientSession session, RainbowBattleTeamType rainbowbattleteamtype)
        {
            Session = session;
            RainbowBattleTeamType = rainbowbattleteamtype;
        }
    }
}
