// * ************************************************************
// * * START:                                   clientsocket.cs *
// * ************************************************************

// * ************************************************************
// *                      INFORMATIONS
// * ************************************************************
// * Client socket class for the library.
// * clientsocket.cs
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
    public unsafe class ClientSocket
    {
        private Byte[] Buffer;
        private Socket Sock;

        private Boolean Alive = false;

        public NetworkClientConnection OnConnect;
        public NetworkClientReceive OnReceive;
        public NetworkClientConnection OnDisconnect;

        public Socket GetSocket() { return Sock; }

        /// <summary>
        /// Create a new TCP/IPv4 socket that will act as a client.
        /// </summary>
        public ClientSocket()
        {
            this.Sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.Sock.ReceiveBufferSize = Kernel.MAX_BUFFER_SIZE;
            this.Sock.SendBufferSize = Kernel.MAX_BUFFER_SIZE;

            this.Buffer = new Byte[Kernel.MAX_BUFFER_SIZE];

            this.Alive = false;

            this.OnConnect = null;
            this.OnReceive = null;
            this.OnDisconnect = null;
        }

        /// <summary>
        /// Begin the asynchronous operations to establish a connection with the specified remote endpoint.
        /// </summary>
        public void Connect(String Address, UInt16 Port)
        {
            if (!Alive)
            {
                Alive = true;
                Sock.BeginConnect(IPAddress.Parse(Address), Port, new AsyncCallback(EndConnect), null);
            }
        }

        /// <summary>
        /// Called by the asynchronous callback when the connection is established.
        /// </summary>
        private void EndConnect(IAsyncResult res)
        {
            try
            { 
                Sock.EndConnect(res);
                
                if (OnConnect != null)
                    OnConnect(this);

                Receive();
            }
            catch { Alive = false; }
        }

        /// <summary>
        /// Begin the asynchronous operations to receive data from the server.
        /// </summary>
        private void Receive()
        {
            try { Sock.BeginReceive(Buffer, 0, Buffer.Length, SocketFlags.None, new AsyncCallback(EndReceive), null); }
            catch { Disconnect(); }
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

                        if (OnReceive != null)
                            OnReceive(this, Received);

                        Receive();
                    }
                    else
                        Disconnect();
                }
            }
            catch { Disconnect(); }
        }

        /// <summary>
        /// Begin the asynchronous operations to send the data to the server.
        /// </summary>
        public void Send(Byte[] Data)
        {
            if (Alive)
            {
                try { Sock.BeginSend(Data, 0, Data.Length, SocketFlags.None, new AsyncCallback(EndSend), null); }
                catch { Disconnect(); }
            }
        }

        /// <summary>
        /// Called by the asynchronous callback when the data is sent.
        /// </summary>
        private void EndSend(IAsyncResult res)
        {
            try { Sock.EndSend(res); }
            catch { Disconnect(); }
        }

        /// <summary>
        /// Begin the asynchronous operations to disconnect the socket from the remote endpoint.
        /// </summary>
        public void Disconnect()
        {
            if (Alive)
            {
                Alive = false;
                try { Sock.BeginDisconnect(false, new AsyncCallback(EndDisconnect), null); }
                catch { }
            }
        }

        /// <summary>
        /// Called by the asynchronous callback when the connection is closed.
        /// </summary>
        private void EndDisconnect(IAsyncResult res)
        {
            try
            { 
                Sock.EndDisconnect(res);
                
                if (OnDisconnect != null)
                    OnDisconnect(this);
            }
            catch { }
        }
    }
}
