using System.Runtime.InteropServices;
using YiX.Entities;
using YiX.Enums;
using YiX.Network.Sockets;

namespace YiX.Network.Packets.Conquer
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct MsgUpdate
    {
        public ushort Size;
        public ushort Id;
        public int UniqueId;
        public int Amount;
        public MsgUpdateType Type;
        public long Value;

        public static implicit operator byte[](MsgUpdate msg)
        {
            var buffer = BufferPool.GetBuffer();
            fixed (byte* p = buffer)
                *(MsgUpdate*)p = *&msg;
            return buffer;
        }
        
        public static byte[] Create(YiObj player, long value, MsgUpdateType type)
        {
            var packet = stackalloc MsgUpdate[1];
            packet->Size = (ushort) sizeof(MsgUpdate);
            packet->Id = 1017;
            packet->UniqueId = player.UniqueId;
            packet->Amount = 1;
            packet->Value = value;
            packet->Type = type;
            var buffer = BufferPool.GetBuffer();
            fixed (byte* p = buffer)
                *(MsgUpdate*)p = *packet;
            return buffer;
        }
    }
}