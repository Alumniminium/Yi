// * ************************************************************
// * * START:                                        mapbase.cs *
// * ************************************************************

// * ************************************************************
// *                      INFORMATIONS
// * ************************************************************
// * Map base structures for the library.
// * mapbase.cs
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
    public struct MySize
    {
        public Int32 Width;
        public Int32 Height;

        public MySize(Int32 Width, Int32 Height)
        { this.Width = Width; this.Height = Height; }
    };

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct MyPos
    {
        public Int32 X;
        public Int32 Y;

        public MyPos(Int32 X, Int32 Y)
        { this.X = X; this.Y = Y; }
    };

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct LayerInfo
    {
        public UInt16 Terrain;
        public UInt16 Mask; //Access
        public Int16 Altitude;
    };

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PassageInfo
    {
        public Int32 PosX;
        public Int32 PosY;
        public Int32 Index;
    };

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct RegionInfo
    {
        public UInt32 IdMap;
        public MyPos PosCell;
        public UInt32 Type;
        public UInt32 Cx;
        public UInt32 Cy;
        public fixed Byte String1[128];
        public fixed Byte String2[128];
        public fixed Byte String3[128];
        public Int32 NColor;
        public Int32 NShowType;
        public Int32 DColor;
        public Int32 DShowPos;
        public Int32 DShowType;
        public Int32 DAccess;
        public UInt32 AccessTime;
        public Int32 Access; //Boolean
    };

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Cell
    {
        public Boolean Accessible;
        public Int16 Altitude;

        public Cell(LayerInfo Info)
        { this.Accessible = Info.Mask != 0; this.Altitude = Info.Altitude; }
    }
}

// * ************************************************************
// * * END:                                          mapbase.cs *
// * ************************************************************