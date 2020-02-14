using System;
using System.Linq;
using System.Text;
using Yi.Calculations;
using Yi.Entities;
using Yi.Enums;
using Yi.Network.Sockets;
using Yi.SelfContainedSystems;

namespace Yi.Network.Packets.Conquer
{
    public static class MsgLogin
    {
        public static Player Player;
        public static uint UniqueId;
        public static uint LoginToken;
        public static uint ClientVersion;
        public static string ClientLanguage;

        public static  void Handle(Player player, byte[] packet)
        {
            UniqueId = BitConverter.ToUInt32(packet, 4); //Works
            LoginToken = BitConverter.ToUInt32(packet, 8); //Works
            ClientLanguage = Encoding.GetEncoding(1252).GetString(packet, 14, 2);
            ClientVersion = BitConverter.ToUInt32(packet, 24); // the number in the res.dat file
            BufferPool.RecycleBuffer(packet);

            if (ClientLanguage != "En")
            {
                player.Disconnect();
                return;
            }
            
            Player = SelectorSystem.GetPlayerByUniqueId(UniqueId); //Collections.Players.Values.First(a => a.UniqueId == UniqueId);

            if (Player.IP != player.GameSocket.GetIP())
            {
                Output.WriteLine("Disconnected! IP Mismatch - Tried to log in " + player.AccountId + " while that account was logged in from a different ip.");
                Output.WriteLine($"Requesting IP: {player.GameSocket.GetIP()} Active IP: {Player.IP}");
                //Message.SendTo(Constants.System,Constants.Allusers, $"ATTENTION! Someone logged in using your AccountID AND YOUR CURRENT PASSWORD from {player.GameSocket.GetIP()}",MsgTextType.LoginInformation);

                //player.Disconnect();
                //return;
            }

            if (Player != null)
            {
                Player.GameSocket = player.GameSocket;
                Player.GameSocket.Ref = Player;
                Process();
            }
            else
                player.Disconnect();
        }

        private static void Process()
        {
            Player.GameSocket.Crypto.SetKeys(UniqueId, UniqueId);
            var characters = SelectorSystem.GetPlayersFor(Player.AccountId).ToList();

            if (characters.Any(c=>c.LoginType== LoginType.Create))
            {
                if (!Player.GameSocket.Socket.Connected)
                    return;
                Player.ForceSend(MsgText.Create(Constants.System, Constants.Allusers, "NEW_ROLE", MsgTextType.LoginInformation), 29 + Constants.System.Length + Constants.Allusers.Length + "NEW_ROLE".Length);
            }
            else
            {
                var availableChars = characters.Where(c => c.Online == false).ToList();

                if (availableChars.Count == 0)
                {
                    characters[0].Disconnect();
                    characters[0].GameSocket = Player.GameSocket;
                    characters[0].GameSocket.Ref = characters[0];
                    Player = characters[0];
                }
                else
                {
                    availableChars[0].GameSocket = Player.GameSocket;
                    availableChars[0].GameSocket.Ref = availableChars[0];
                    Player = availableChars[0];
                }

                Player.ForceSend(MsgText.Create(Constants.System, Constants.Allusers, Constants.AnswerOk, MsgTextType.LoginInformation), 29 + Constants.System.Length + Constants.Allusers.Length + Constants.AnswerOk.Length);
                var p = LegacyPackets.CharacterInformation(Player);
                Player.ForceSend(p, BitConverter.ToUInt16(p, 0));
            }
        }
    }
}