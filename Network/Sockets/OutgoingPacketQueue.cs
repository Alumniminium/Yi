using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using YiX.Entities;
using YiX.Helpers;

namespace YiX.Network.Sockets
{
    public static class OutgoingPacketQueue
    {
        public static readonly ConcurrentDictionary<Player, ConcurrentQueue<byte[]>> PacketQueue = new ConcurrentDictionary<Player, ConcurrentQueue<byte[]>>();
        public static readonly AutoResetEvent Block = new AutoResetEvent(false);
        public static readonly Thread QueueThread = new Thread(WorkLoop);
        public static byte[] MergedPacket;
        public static byte[] Chunk;
        public static int Counter;
        public static int Offset;
        public static int TempLen;

        public static void Add(Player client, byte[] packet)
        {
            if (!QueueThread.IsAlive)
                QueueThread.Start();

            if (!PacketQueue.ContainsKey(client))
                PacketQueue.TryAdd(client, new ConcurrentQueue<byte[]>());

            var size = BitConverter.ToUInt16(packet, 0);

            if (size != packet.Length && packet.Length!=850)
            {
                client.ForceSend(packet, size);
                Output.WriteLine("Forcing packet out. " + size + " | " + packet.Length);
                return;
            }

            PacketQueue[client].Enqueue(packet);
        }

        public static void WorkLoop()
        {
            while (true)
            {
                Block.WaitOne(60);
                foreach (var q in PacketQueue.Where(q => q.Value.Count > 0))
                {
                    Counter = 0;
                    Offset = 0;
                    MergedPacket = new byte[850];
                    while (q.Value.Count > 0 && q.Value.TryPeek(out Chunk) && BitConverter.ToUInt16(Chunk, 0) + Offset <= 850)
                    {
                        if (q.Value.TryDequeue(out Chunk))
                        {
                            TempLen = BitConverter.ToUInt16(Chunk, 0);
                            if (TempLen > Chunk.Length)
                                continue;
                            if (TempLen == 0)
                            {
                                BufferPool.RecycleBuffer(Chunk);
                                continue;
                            }

                            Buffer.BlockCopy(Chunk, 0, MergedPacket, Offset, TempLen);
                            Offset += TempLen;
                            Counter++;
                            BufferPool.RecycleBuffer(Chunk);
                        }
                    }
                    if (Counter > 0)
                    {
                        q.Key.ForceSend(BufferPool.Clone(MergedPacket), Offset);
                    }
                }
            }
        }

        public static void Remove(Player player) => PacketQueue.TryRemove(player);
    }
}