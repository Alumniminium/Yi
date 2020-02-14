using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Yi.Entities;
using Yi.Enums;
using Yi.Network.Sockets;

#pragma warning disable CS4014

namespace Yi.Network.Packets.Remote
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
                case MsgRemoteMaintenanceType.DownloadDatabase:
                    Task.Run(() =>
                    {
                        YiCore.BackupInProgress = true;
                        using (var reader = new FileStream("Db.json", FileMode.Open))
                        {
                            while (reader.Position < reader.Length)
                            {
                                var filebuffer = new byte[1490];
                                var bytes = reader.Read(filebuffer, 0, filebuffer.Length);

                                var data = new byte[bytes];

                                Buffer.BlockCopy(filebuffer, 0, data, 0, bytes);
                                if (bytes < 1490)
                                    Debugger.Break();
                                account.SendRaw(MsgFileTransfer.Create(data, (int)reader.Length));
                                Thread.Sleep(1);
                            }
                        }
                        account.SendRaw(MsgFileTransfer.Create(new byte[1], 0));
                        YiCore.BackupInProgress = true;
                    });
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