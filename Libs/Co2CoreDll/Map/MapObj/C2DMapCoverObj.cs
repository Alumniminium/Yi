// * ************************************************************
// * * START:                                 c2dmapcoverobj.cs *
// * ************************************************************

// * ************************************************************
// *                      INFORMATIONS
// * ************************************************************
// * 2DMapCoverObj structure for the library.
// * c2dmapcoverobj.cs
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

using System;
using System.Runtime.InteropServices;

namespace CO2_CORE_DLL.Map
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct C2DMapCoverObj //Should be a class with few methods, but useless for a server.
    {
        public fixed Byte FileName[260];
        public fixed Byte Index[128];
        public MyPos PosCell;
        public MySize SizeBase;
        public MyPos PosOffset;
        public UInt32 FrameInterval;
    };
}

// * ************************************************************
// * * END:                                   c2dmapcoverobj.cs *
// * ************************************************************