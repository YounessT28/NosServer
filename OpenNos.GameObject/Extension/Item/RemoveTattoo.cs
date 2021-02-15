using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNos.GameObject.Extension.Item
{
    public static class ItemRemoveTattooInked
    {
        #region Methods

        public static void RemoveTattoo(this CharacterSkill e, ClientSession s)
        {
            var skill = ServerManager.GetSkill(e.SkillVNum);

            if (skill.Class != 27)
            {
                return;
            }

            var msg = $"The {skill.Name} tattoo has been removed";
            s.SendPacket(UserInterfaceHelper.GenerateMsg(msg, 0));
            s.SendPacket(UserInterfaceHelper.GenerateSay(msg, 11));
            s.Character.Inventory.RemoveItemAmount(5790, 1);
            s.Character.Skills.Remove(e.SkillVNum);
            s.SendPacket(s.Character.GenerateSki());
            s.SendPackets(s.Character.GenerateQuicklist());
            s.SendPacket(s.Character.GenerateLev());
        }

        #endregion
    }
}
