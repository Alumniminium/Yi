// * ************************************************************
// * * START:                                          event.cs *
// * ************************************************************

// * ************************************************************
// *                      INFORMATIONS
// * ************************************************************
// * Socket events for the library.
// * event.cs
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

namespace CO2_CORE_DLL.Net.Sockets
{
    public delegate void NetworkClientConnection(Object Client);
    public delegate void NetworkClientReceive(Object Client, Byte[] Buffer);
    public delegate void NetworkClientDisconnection(Object Client);
}

// * ************************************************************
// * * END:                                            event.cs *
// * ************************************************************