using System.Runtime.InteropServices;
using YiX.Entities;
using YiX.Network.Sockets;

namespace YiX.Network.Packets.Remote
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct MsgRemoteLogin
    {
        public ushort Size { get; set; }
        public ushort Id { get; set; }
        public unsafe fixed byte Username[16];
        public unsafe fixed byte Password[16];

        public string GetPw() => "";
        public string GetUser() => "";

        public void SetPw(string input)
        {

        }
        public void SetUser(string input)
        {

        }

        public static unsafe byte[] Create(string user, string pass)
        {
            var packet = new MsgRemoteLogin
            {
                Size = 36,
                Id = 1
            };

            for (var i = 0; i < 16; i++)
                packet.Username[i] = (byte) user[i];

            for (var i = 0; i < 16; i++)
                packet.Password[i] = (byte) pass[i];
            return packet;
        }

        public static void Handle(Player account, byte[] buffer)
        {
            //MsgRemoteLogin packet = buffer;
            //Player acc;
            //if (Collections.Players.TryGetValue(packet.GetUser(), out acc))
            //{
            //    if (acc.Password == packet.GetPw())
            //    {
            //        acc.GameSocket = account.GameSocket;
            //        account = acc;
            //        packet.SetUser("Welcome");
            //        packet.SetPw(acc.AccountId);
            //        account.SendRaw(packet);
            //    }
            //    else
            //        account.SendRaw(packet);
            //}
            //else
            //    account.SendRaw(packet);
        }
        public static unsafe implicit operator byte[] (MsgRemoteLogin msg)
        {
            var buffer = BufferPool.GetBuffer();
            fixed (byte* p = buffer)
                *(MsgRemoteLogin*)p = *&msg;
            return buffer;
        }

        public static unsafe implicit operator MsgRemoteLogin(byte[] buffer)
        {
            MsgRemoteLogin packet;
            fixed (byte* p = buffer)
                packet = *(MsgRemoteLogin*)p;
            BufferPool.RecycleBuffer(buffer);
            return packet;
        }
    }
}