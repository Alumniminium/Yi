using System;
using System.Runtime.InteropServices;
using YiX.Entities;
using YiX.Enums;
using YiX.Network.Sockets;

#pragma warning disable CS4014

namespace YiX.Network.Packets.Remote
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct MsgRemoteMaintenance
    {
        public ushort Size { get; set; }
        public ushort Id { get; set; }
        public MsgRemoteMaintenanceType Type;
        public bool Success;

        public static byte[] Create(MsgRemoteMaintenanceType type)
        {
            var packet = new MsgRemoteMaintenance {Size = 6, Id = 3, Type = type, Success = false};
            return packet;
        }

        public static void Handle(Player account, byte[] buffer)
        {
            MsgRemoteMaintenance packet = buffer;
            packet.Success = true;
            switch (packet.Type)
            {
                case MsgRemoteMaintenanceType.StartBackup:
                    //if (!YiCore.BackupInProgress)
                    //    Db.Backup();
                    //else
                    //    packet.Success = false;
                    break;
                case MsgRemoteMaintenanceType.OptimizeMemory:
                    YiCore.CompactLoh();
                    break;
                case MsgRemoteMaintenanceType.Shutdown:
                    packet.Success = false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            account.SendRaw(packet);
        }
        public static unsafe implicit operator byte[] (MsgRemoteMaintenance msg)
        {
            var buffer = BufferPool.GetBuffer();
            fixed (byte* p = buffer)
                *(MsgRemoteMaintenance*)p = *&msg;
            return buffer;
        }

        public static unsafe implicit operator MsgRemoteMaintenance(byte[] buffer)
        {
            MsgRemoteMaintenance packet;
            fixed (byte* p = buffer)
                packet = *(MsgRemoteMaintenance*)p;
            BufferPool.RecycleBuffer(buffer);
            return packet;
        }
    }
}