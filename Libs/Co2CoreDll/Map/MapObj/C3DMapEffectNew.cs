// * ************************************************************
// * * START:                                c3dmapeffectnew.cs *
// * ************************************************************

// * ************************************************************
// *                      INFORMATIONS
// * ************************************************************
// * 3DMapEffectNew structure for the library.
// * c3dmapeffectnew.cs
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
    public unsafe struct C3DMapEffectNew //Should be a class with few methods, but useless for a server.
    {
        public fixed Byte Index[64];
        public MyPos PosWorld;
        public Single Vertical1;
        public Single Vertical2; //Probably an error in TQ code, but eh, they never corrected it... It overwrite the first.
        public Single Horizontal;
        public Single ScaleX;
        public Single ScaleY;
        public Single ScaleZ;
    };
}

// * ************************************************************
// * * END:                                  c3dmapeffectnew.cs *
// * ************************************************************