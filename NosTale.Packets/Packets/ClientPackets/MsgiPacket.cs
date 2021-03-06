﻿////<auto-generated <- Codemaid exclusion for now (PacketIndex Order is important for maintenance)

using OpenNos.Core;

namespace NosTale.Packets.Packets.ClientPackets
{
    [PacketHeader("msgi")]
    public class MsgiPacket : PacketDefinition
    {
        #region Properties

        //msgi will replace Serverside Messages, and send messages Clientside
        //ID 1593 
        //DE > Das Schiff fährt in 5 Minuten ab
        //EN > The Ship will depart in 5 Minutes
        //I don't know how this Packet works right now, but i'll figure it out - XV50



        //msgi 0 1593 4 1 0 0 0
        [PacketIndex(0)]
        public byte MessageType { get; set; }

        [PacketIndex(1)]
        public int Message { get; set; }

        [PacketIndex(2)]
        public byte Unidentified { get; set; }

        [PacketIndex(3)]
        public byte TimeCounter { get; set; } //Seems like it is in Minutes, maybe the next Value is in Seconds

        [PacketIndex(4)]
        public byte Unidentified2 { get; set; }

        [PacketIndex(5)]
        public byte Unidentified3 { get; set; }

        [PacketIndex(6)]
        public byte Unidentified4 { get; set; }


        #endregion
    }
}