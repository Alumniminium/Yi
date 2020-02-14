// * ************************************************************
// * * START:                                   c3dmapeffect.cs *
// * ************************************************************

// * ************************************************************
// *                      INFORMATIONS
// * ************************************************************
// * 3DMapEffect structure for the library.
// * c3dmapeffect.cs
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
    public unsafe struct C3DMapEffect //Should be a class with few methods, but useless for a server.
    {
        public fixed Byte Index[64];
        public MyPos PosWorld;
    };
}

// * ************************************************************
// * * END:                                     c3dmapeffect.cs *
// * ************************************************************