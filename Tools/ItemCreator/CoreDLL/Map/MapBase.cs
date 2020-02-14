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

using System.Runtime.InteropServices;

namespace ItemCreator.CoreDLL.Map
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct MySize
    {
        public int Width;
        public int Height;

        public MySize(int Width, int Height)
        { this.Width = Width; this.Height = Height; }
    };

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct MyPos
    {
        public int X;
        public int Y;

        public MyPos(int X, int Y)
        { this.X = X; this.Y = Y; }
    };

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct LayerInfo
    {
        public ushort Terrain;
        public ushort Mask; //Access
        public short Altitude;
    };

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PassageInfo
    {
        public int PosX;
        public int PosY;
        public int Index;
    };

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct RegionInfo
    {
        public uint IdMap;
        public MyPos PosCell;
        public uint Type;
        public uint Cx;
        public uint Cy;
        public fixed byte String1[128];
        public fixed byte String2[128];
        public fixed byte String3[128];
        public int NColor;
        public int NShowType;
        public int DColor;
        public int DShowPos;
        public int DShowType;
        public int DAccess;
        public uint AccessTime;
        public int Access; //Boolean
    };

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Cell
    {
        public bool Accessible;
        public short Altitude;

        public Cell(LayerInfo Info)
        { Accessible = Info.Mask != 0; Altitude = Info.Altitude; }
    }
}

// * ************************************************************
// * * END:                                          mapbase.cs *
// * ************************************************************