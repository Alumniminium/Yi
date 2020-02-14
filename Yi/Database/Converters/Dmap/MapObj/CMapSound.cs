// * ************************************************************
// * * START:                                      cmapsound.cs *
// * ************************************************************

// * ************************************************************
// *                      INFORMATIONS
// * ************************************************************
// * MapSound structure for the library.
// * cmapsound.cs
// * 
// * --
// *
// * Feel free to use this class in your projects, but don't
// * remove the header to keep the paternity of the class.
// * 
// * ************************************************************
// *                      CREDITS
// * ************************************************************
// * Originally created by CptSky (May 20th, 2012)
// * Copyright (C) 2012 CptSky
// *
// * ************************************************************

using System.Runtime.InteropServices;

namespace Yi.Database.Converters.Dmap.MapObj
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct CMapSound //Should be a class with few methods, but useless for a server.
    {
        public fixed byte File [260];
        public MyPos PosWorld;
        public int Range;
        public int Volume;
        public uint Interval;
    };
}

// * ************************************************************
// * * END:                                        cmapsound.cs *
// * ************************************************************