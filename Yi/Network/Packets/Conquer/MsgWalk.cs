using System;
using Yi.Calculations;
using Yi.Enums;
using Yi.Network.Sockets;
using Yi.SelfContainedSystems;
using Player = Yi.Entities.Player;

namespace Yi.Network.Packets.Conquer
{
    public unsafe struct MsgWalk
    {
        public ushort Size;
        public ushort Id;
        public int UniqueId;
        public Direction Direction;
        public bool Running;
        public ushort Unknown;

        public static byte[] Create(int uniqueId, Direction direction, bool running)
        {
            MsgWalk* msgP = stackalloc MsgWalk[1];

            msgP->Size = (ushort) sizeof(MsgWalk);
            msgP->Id = 1005;
            msgP->UniqueId = uniqueId;
            msgP->Direction = direction;
            msgP->Running = running;
            msgP->Unknown = 0;

            var buffer = BufferPool.GetBuffer();
            fixed (byte* p = buffer)
                *(MsgWalk*)p = *msgP;
            return buffer;
        }

        public static void Handle(Player player, byte[] buffer)
        {
            try
            {
                fixed (byte* p = buffer)
                {
                    var packet = *(MsgWalk*) p;

                    BufferPool.RecycleBuffer(buffer);

                    if (player.UniqueId == packet.UniqueId)
                    {
                        player.CurrentTarget = null;
                        player.RemoveSpawnProtection();
                        player.Direction = (Direction) ((byte) packet.Direction % 8);

                        player.Location.X += (ushort) Constants.DeltaX[(sbyte) player.Direction];
                        player.Location.Y += (ushort) Constants.DeltaY[(sbyte) player.Direction];
                        player.Emote = Emote.Stand;
                        ScreenSystem.Send(player, packet, true, true, packet.Size);
                        ScreenSystem.Update(player);
                        if (TeamSystem.Teams.ContainsKey(player.UniqueId))
                            TeamSystem.Teams[player.UniqueId].UpdateLeaderPosition(player);
                    }
                    else if (packet.UniqueId == player.Pet?.UniqueId)
                    {
                        player.Pet.Direction = (Direction) ((byte) packet.Direction % 8);

                        player.Pet.Location.X += (ushort) Constants.DeltaX[(sbyte) player.Direction];
                        player.Pet.Location.Y += (ushort) Constants.DeltaY[(sbyte) player.Direction];
                        ScreenSystem.Send(player.Pet, packet, false, true, packet.Size);
                        ScreenSystem.Update(player.Pet);
                    }
                }
            }
            catch (Exception e)
            {
                Output.WriteLine(e);
            }
        }

        public static implicit operator byte[](MsgWalk msg)
        {
            var buffer = BufferPool.GetBuffer();
            fixed (byte* p = buffer)
                *(MsgWalk*)p = *&msg;
            return buffer;
        }
    }
}