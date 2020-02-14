using System.Net.Sockets;
using System.Threading;
using YiX.Network.Sockets;

namespace YiX.Network
{
    public struct Servers
    {
        public static readonly Thread SuperSocketThread = new Thread(RunSuperSocket);
        public static ServerSocket SuperSocket { get; set; }
        public static ushort Port { get; set; }

        public static void Start(ushort port)
        {
            Port = port;
            SuperSocketThread.Start();
        }

        public static void RunSuperSocket()
        {
            SuperSocket = new ServerSocket{Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)};
            ServerSocket.OnReceive = PacketHandler.Handle;
            IncommingPacketQueue.Start();
            SuperSocket.Enable(Port);
        }
    }
}