using System;
using System.Net;
using System.Net.Sockets;
using YiX.Entities;
using YiX.Helpers;

namespace YiX.Network.Sockets
{
    [Serializable]
    public class ClientSocket : MarshalByRefObject
    {
        public byte[] ReceiveBuffer = new byte[850];
        public int RecvSize;
        public Socket Socket;
        public readonly Crypto Crypto;
        public Player Ref;

        public ClientSocket(Socket socket)
        {
            Crypto = new Crypto();
            Socket = socket;
            Socket.Blocking = false;
            Socket.NoDelay = true;
            Socket.DontFragment = true;
        }

        public void Disconnect()
        {
            try
            {
                OutgoingPacketQueue.Remove(Ref);
                ServerSocket.InvokeDisconnect(this);
            }
            finally
            {
                Socket = null;
            }
        }

        public void Send(byte[] packet, int size)
        {
            try
            {
                if (packet == null || Socket == null)
                    return;
                //Output.Lines.Clear();
                
                //Output.WriteLine($"Packet: {BitConverter.ToUInt16(packet, 2)} Len: {BitConverter.ToUInt16(packet, 0)} Actual: {size}");
                //Output.WriteLine(packet.Take(size).ToArray().HexDump());

                if (Crypto != null)
                    packet = Crypto.Encrypt(packet, size);

                Socket.BeginSend(packet, 0, size, SocketFlags.None, null, null);
                BufferPool.RecycleBuffer(packet);
            }
            catch (Exception e)
            {
                Output.WriteLine(e);
                Disconnect();
            }
        }

        public void AsyncReceive(IAsyncResult ar)
        {
            ReceiveBuffer = (byte[]) ar.AsyncState;
            try
            {
                RecvSize = Socket.EndReceive(ar, out var error);

                if (error == SocketError.Success && RecvSize>4)
                {
                    if (Crypto != null)
                        ReceiveBuffer = Crypto.Decrypt(ReceiveBuffer, RecvSize);


                    ushort size;
                    for (var i = 0; i < RecvSize; i += size)
                    {
                        size = BitConverter.ToUInt16(ReceiveBuffer, i);

                        if (size == RecvSize)
                        {
                            var packet = new IncommingPacket(this, BufferPool.Clone(ReceiveBuffer));
                            IncommingPacketQueue.Add(packet);
                        }
                        else if (size < RecvSize)
                        {
                            var firstPacket = new byte[size];
                            Buffer.BlockCopy(ReceiveBuffer, i, firstPacket, 0, size);


                            var packet = new IncommingPacket(this, BufferPool.Clone(firstPacket));
                            IncommingPacketQueue.Add(packet);
                        }
                        else
                        {
                            Output.WriteLine($"Packet: {BitConverter.ToUInt16(ReceiveBuffer, 2)} Len: {BitConverter.ToUInt16(ReceiveBuffer, 0)} Actual: {size}");
                            Output.WriteLine(ReceiveBuffer.HexDump());
                        }
                    }

                    Socket.BeginReceive(ReceiveBuffer, 0, ReceiveBuffer.Length, SocketFlags.None, AsyncReceive, ReceiveBuffer);
                }
                else
                    Disconnect();
            }
            catch (Exception ex)
            {
                if (!(ex is ObjectDisposedException))
                {
                    Output.WriteLine($"Disconnect Reason: {ex.Message} \r\n {ex.StackTrace}");
                    Output.WriteLine(ex);
                }
                Disconnect();
            }
        }

        public string GetIP()
        {
            if (Socket == null)
                return "";
            var remoteIpEndPoint = Socket?.RemoteEndPoint as IPEndPoint;
            return remoteIpEndPoint?.Address.ToString();
        }
    }
}