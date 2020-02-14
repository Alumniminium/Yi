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

namespace ItemCreator.CoreDLL.Map.MapObj
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct C3DMapEffectNew //Should be a class with few methods, but useless for a server.
    {
        public fixed byte Index[64];
        public MyPos PosWorld;
        public float Vertical1;
        public float Vertical2; //Probably an error in TQ code, but eh, they never corrected it... It overwrite the first.
        public float Horizontal;
        public float ScaleX;
        public float ScaleY;
        public float ScaleZ;
    };
}

// * ************************************************************
// * * END:                                  c3dmapeffectnew.cs *
// * ************************************************************