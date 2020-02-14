// * ************************************************************
// * * START:                                   serversocket.cs *
// * ************************************************************

// * ************************************************************
// *                      INFORMATIONS
// * ************************************************************
// * Server socket class for the library.
// * serversocket.cs
// * 
// * --
// *
// * Feel free to use this class in your projects, but don't
// * remove the header to keep the paternity of the class.
// * 
// * ************************************************************
// *                      CREDITS
// * ************************************************************
// * Originally created by CptSky (December 19th, 2010)
// * Copyright (C) 2010 CptSky
// * 
// * ************************************************************
// *                      SPECIAL THANKS
// * ************************************************************
// * Hybrid (InfamousNoone @ e*pvp)
// * 
// * ************************************************************

using System;
using System.Net;
using System.Net.Sockets;

namespace CO2_CORE_DLL.Net.Sockets
{
    public class ServerSocket
    {
        private Socket Sock;
        private UInt16 Port;

        private Boolean Listening;

        public NetworkClientConnection OnConnect;
        public NetworkClientReceive OnReceive;
        public NetworkClientConnection OnDisconnect;

        public Socket GetSocket() { return Sock; }
        public UInt16 GetPort() { return Port; }

        /// <summary>
        /// Create a new TCP/IPv4 socket that will act as a server.
        /// </summary>
        public ServerSocket()
        {
            this.Sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.Port = 0;

            this.Listening = false;

            this.OnConnect = null;
            this.OnReceive = null;
            this.OnDisconnect = null;
        }

        ~ServerSocket()
        {
            Sock = null;
            OnConnect = null;
            OnReceive = null;
            OnDisconnect = null;
        }

        /// <summary>
        /// Associate the socket to a local endpoint and place the socket in a listening state.
        /// </summary>
        public void Listen(UInt16 BindPort, Int32 BackLog)
        {
            if (Listening)
                return;

            Listening = true;
            Port = BindPort;

            try
            {
                Sock.Bind(new IPEndPoint(IPAddress.Any, Port));
                Sock.Listen(BackLog);
            }
            catch (Exception Exc) { Listening = false; throw Exc; }
        }

        /// <summary>
        /// Begin the asynchronous operations to accept incoming connections.
        /// </summary>
        public void Accept()
        {
            Sock.BeginAccept(new AsyncCallback(EndAccept), null);
        }

        /// <summary>
        /// Called by the asynchronous callback when there is a new incoming connection.
        /// </summary>
        private void EndAccept(IAsyncResult res)
        {
            Socket Socket = null;

            try { Socket = Sock.EndAccept(res); }
            catch { Accept(); return; }

            Socket.SendBufferSize = Kernel.MAX_BUFFER_SIZE;
            Socket.ReceiveBufferSize = Kernel.MAX_BUFFER_SIZE;

            var Client = new NetworkClient();
            if (!Client.Create(this, Socket, Kernel.MAX_BUFFER_SIZE))
            {
                try { Socket.Close(); }
                catch { Accept(); return; }
            }

            if (OnConnect != null)
                OnConnect(Client);

            Accept();
        }

        /// <summary>
        /// Force the disconnection of the specified client.
        /// </summary>
        public void InvokeDisconnect(NetworkClient Client)
        {
            if (Client == null || !Client.IsAlive)
                return;

            Client.Disconnect();
            if (OnDisconnect != null)
                OnDisconnect(Client);
        }
    }
}

// * ************************************************************
// * * END:                                     serversocket.cs *
// * ************************************************************
