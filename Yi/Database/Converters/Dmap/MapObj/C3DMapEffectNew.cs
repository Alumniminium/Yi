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

using System.Runtime.InteropServices;

namespace Yi.Database.Converters.Dmap.MapObj
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct C3DMapEffectNew //Should be a class with few methods, but useless for a server.
    {
        public fixed byte Index [64];
        public MyPos PosWorld;
        public readonly float Vertical1;
        public readonly float Vertical2; //Probably an error in TQ code, but eh, they never corrected it... It overwrite the first.
        public readonly float Horizontal;
        public readonly float ScaleX;
        public readonly float ScaleY;
        public readonly float ScaleZ;
    };
}

// * ************************************************************
// * * END:                                  c3dmapeffectnew.cs *
// * ************************************************************