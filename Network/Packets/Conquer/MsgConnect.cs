using System.Linq;
using System.Text;
using YiX.Entities;
using YiX.Enums;
using YiX.Network.Sockets;
using YiX.SelfContainedSystems;

namespace YiX.Network.Packets.Conquer
{
    public static class MsgConnect
    {
        public static string AccountId;
        public static string Password;
        public static byte[] RawPassword;
        public static Player Player;

        public static void Handle(Player player, byte[] packet)
        {
            Player = player;
            AccountId = Encoding.Default.GetString(packet, 4, 16).Trim((char)0x0000);
            RawPassword = packet.Skip(20).Take(16).ToArray();
            Password = Crypto.DecryptPassword(RawPassword);
            Process();
            BufferPool.RecycleBuffer(packet);
        }

        public static void Process()
        {
            var player = SelectorSystem.GetOrCreatePlayer(AccountId, Password);
            player.IP = Player.GameSocket.GetIP();
            player.GameSocket = Player.GameSocket;
            player.GameSocket.Ref = Player;
            Player = player;

            if (Player.LoginType == LoginType.Create)
                NewAccount();
            else
                Authenticate();
        }

        private static void Authenticate()
        {
            if (Player.Password == Password)
                Player.ForceSend(LegacyPackets.MsgTransfer(Player.UniqueId, Player.UniqueId, 9958), 32);
            else
                Player.Disconnect();
        }

        private static void NewAccount() => Player.ForceSend(LegacyPackets.MsgTransfer(Player.UniqueId, Player.UniqueId, 9958),32);
    }
}