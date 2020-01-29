using System.Runtime.InteropServices;
using YiX.Network.Sockets;

namespace YiX.Network.Packets.Conquer
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct MsgStatus
    {
        public ushort Size;
        public ushort Id;
        public uint MapId;
        public uint DynMapId;
        public uint Flags;

        public static byte[] Create(uint mapId, uint flags)
        {
            var packet = stackalloc MsgStatus[1];
            packet->Size = (ushort) sizeof(MsgStatus);
            packet->Id = 1110;
            packet->MapId = mapId;
            packet->DynMapId = mapId;
            packet->Flags = flags;
            var buffer = BufferPool.GetBuffer();
            fixed (byte* p = buffer)
                *(MsgStatus*)p = *packet;
            return buffer;
        }

        public static implicit operator byte[](MsgStatus msg)
        {
            var buffer = BufferPool.GetBuffer();
            fixed (byte* p = buffer)
                *(MsgStatus*)p = *&msg;
            return buffer;
        }
    }
}