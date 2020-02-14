using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Yi.Entities;
using Yi.Enums;

namespace Yi.Network.Sockets
{
    [Serializable]
    public class ServerSocket
    {
        public static Action<Player, byte[], PacketType> OnReceive;
        public Socket Socket;
        
        public void Enable(ushort port)
        {
            Socket.Blocking = false;
            Socket.NoDelay = true;
            Socket.DontFragment = true;
            Socket.Bind(new IPEndPoint(IPAddress.Any, port));
            Socket.Listen(50);
            Socket.BeginAccept(AsyncAccept, null);
            Output.WriteLine($"[{port}] PoolServer Started");
        }

        public static void InvokeDisconnect(ClientSocket clientSocket)
        {
            if (clientSocket == null)
                return;

            if (clientSocket.Socket !=null && clientSocket.Socket.Connected)
                clientSocket.Socket.Close();
            else
                clientSocket.Ref = null;
        }

        private async void AsyncAccept(IAsyncResult res)
        {
            try
            {
                await Task.Delay(1000);//Accept Throtteling in order to prioritize finishing logging, over the beginning the login sequence.
                var client = new ClientSocket(Socket.EndAccept(res));
                client.Ref = new Player(client);

                Socket.BeginAccept(AsyncAccept, null);
                client.Socket.BeginReceive(client.ReceiveBuffer, 0, client.ReceiveBuffer.Length, SocketFlags.None, client.AsyncReceive, client.ReceiveBuffer);
            }
            catch (Exception e)
            {
                Socket.BeginAccept(AsyncAccept, null);
                Output.WriteLine(e);
            }
        }
    }
}