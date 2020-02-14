using System;
using System.Collections.Concurrent;
using System.Threading;
using Yi.Enums;

namespace Yi.Network.Sockets
{
    public class IncommingPacket
    {
        public ClientSocket Socket;
        public byte[] Packet;

        public IncommingPacket(ClientSocket socket, byte[] packet)
        {
            Socket = socket;
            Packet = packet;
        }
    }
    public static class IncommingPacketQueue
    {
        public static ConcurrentQueue<IncommingPacket> Queue = new ConcurrentQueue<IncommingPacket>();
        public static AutoResetEvent SyncBlock = new AutoResetEvent(false);
        public static Thread WorkerThread;
        public static void Add(IncommingPacket packet)
        {
            Queue.Enqueue(packet);
            SyncBlock.Set();
        }

        public static void Start()
        {
            WorkerThread = new Thread(Loop);
            WorkerThread.Start();
        }

        private static void Loop()
        {
            while (true)
            {
                SyncBlock.WaitOne();
                while (!Queue.IsEmpty)
                {
                    Queue.TryDequeue(out var packet);
                    ServerSocket.OnReceive.Invoke(packet.Socket.Ref, packet.Packet, (PacketType)BitConverter.ToUInt16(packet.Packet, 2));
                }
            }
        }
    }
}
