// * ************************************************************
// * * START:                                  networkclient.cs *
// * ************************************************************

// * ************************************************************
// *                      INFORMATIONS
// * ************************************************************
// * Client for the server socket class for the library.
// * It is not a full client as the client socket class.
// * networkclient.cs
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
    public class NetworkClient
    {
        private Socket Sock;
        private ServerSocket Serv;
        private Byte[] Buffer;

        private Boolean Alive;

        public Object Owner;

        public Socket Socket { get { return this.Sock; } }
        public ServerSocket Server { get { return this.Serv; } }
        public Boolean IsAlive { get { return Alive; } }

        public String IpAddress
        {
            get
            {
                if (Sock != null)
                    return (Sock.RemoteEndPoint as IPEndPoint).Address.ToString();
                else
                    return null;
            }
        }

        public UInt16 Port
        {
            get
            {
                if (Sock != null)
                    return (UInt16)(Sock.RemoteEndPoint as IPEndPoint).Port;
                else
                    return 0;
            }
        }

        /// <summary>
        /// Create the instance, but a call to Create is needed.
        /// </summary>
        public NetworkClient()
        { 
            this.Alive = false;
        }

        /// <summary>
        /// Create the client of the specified server.
        /// </summary>
        public Boolean Create(ServerSocket Server, Socket Socket, Int32 BufferSize)
        {
            if (Alive || Socket == null || Server == null || BufferSize == 0)
                return false;

            Sock = Socket;
            Serv = Server;

            Buffer = new Byte[BufferSize];

            BeginReceive();
            Alive = true;

            return true;
        }

        /// <summary>
        /// Begin the asynchronous operations to receive data from the server.
        /// </summary>
        private void BeginReceive()
        {
            try { Sock.BeginReceive(Buffer, 0, Buffer.Length, SocketFlags.None, new AsyncCallback(EndReceive), null); }
            catch { Serv.InvokeDisconnect(this); }
        }

        /// <summary>
        /// Called by the asynchronous callback when the data is received.
        /// </summary>
        private void EndReceive(IAsyncResult res)
        {
            try
            {
                var Length = Sock.EndReceive(res);
                if (Alive)
                {
                    if (Length > 0)
                    {
                        var Received = new Byte[Length];
                        Array.Copy(Buffer, 0, Received, 0, Length);

                        if (Serv.OnReceive != null)
                            Serv.OnReceive(this, Received);

                        BeginReceive();
                    }
                    else
                        Serv.InvokeDisconnect(this);
                }
            }
            catch (SocketException) { Serv.InvokeDisconnect(this); }
        }

        /// <summary>
        /// Begin the asynchronous operations to send the data to the server.
        /// </summary>
        public void Send(Byte[] Data)
        {
            if (Alive)
            {
                try { Sock.BeginSend(Data, 0, Data.Length, SocketFlags.None, new AsyncCallback(EndSend), null); }
                catch (SocketException) { Server.InvokeDisconnect(this); }
            }
        }

        /// <summary>
        /// Called by the asynchronous callback when the data is sent.
        /// </summary>
        private void EndSend(IAsyncResult res)
        {
            try { Sock.EndSend(res); }
            catch { Serv.InvokeDisconnect(this); }
        }

        /// <summary>
        /// Begin the asynchronous operations to disconnect the socket from the remote endpoint.
        /// </summary>
        public void Disconnect()
        {
            if (Alive)
            {
                Alive = false;
                Sock.BeginDisconnect(false, new AsyncCallback(EndDisconnect), null);
            }
        }

        /// <summary>
        /// Called by the asynchronous callback when the connection is closed.
        /// </summary>
        private void EndDisconnect(IAsyncResult res)
        {
            try { Sock.EndDisconnect(res); Serv.InvokeDisconnect(this); }
            catch { Serv.InvokeDisconnect(this); }
        }
    }
}

// * ************************************************************
// * * END:                                    networkclient.cs *
// * ************************************************************