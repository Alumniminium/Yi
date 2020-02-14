// * ************************************************************
// * * START:                           c2dmapterrainobjpart.cs *
// * ************************************************************

// * ************************************************************
// *                      INFORMATIONS
// * ************************************************************
// * 2DMapTerrainObjPart structure for the library.
// * c2dmapterrainobjpart
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
using System.IO;

namespace ItemCreator.CoreDLL.Map.MapObj
{
    public unsafe struct C2DMapTerrainObjPart //Should be a class with few methods, but useless for a server.
    {
        public fixed byte AniFile[256];
        public fixed byte AniTitle[64];
        public MyPos PosOffset;
        public uint AniInterval;
        public MySize SizeBase;
        public int Thick;
        public MyPos PosSceneOffset;
        public int Height;

        public LayerInfo[,] Cells;

        public void Load(FileStream Stream)
        {
            if (Stream == null)
                throw new ArgumentNullException();

            fixed (byte* ptr = AniFile)
                Stream.Read(ptr, 256);
            fixed (byte* ptr = AniTitle)
                Stream.Read(ptr, 64);
            fixed (MyPos* ptr = &PosOffset)
                Stream.Read(ptr, sizeof(MyPos));
            fixed (uint* ptr = &AniInterval)
                Stream.Read(ptr, sizeof(uint));
            fixed (MySize* ptr = &SizeBase)
                Stream.Read(ptr, sizeof(MySize));
            fixed (int* ptr = &Thick)
                Stream.Read(ptr, sizeof(int));
            fixed (MyPos* ptr = &PosSceneOffset)
                Stream.Read(ptr, sizeof(MyPos));
            fixed (int* ptr = &Height)
                Stream.Read(ptr, sizeof(int));

            Cells = new LayerInfo[SizeBase.Width, SizeBase.Height];
            for (var i = 0; i < SizeBase.Height; i++)
            {
                for (var j = 0; j < SizeBase.Width; j++)
                {
                    var Layer = Cells[j, i];

                    uint Mask;
                    int Terrain;
                    int Altitude;
                    Stream.Read(&Mask, sizeof(uint));
                    Stream.Read(&Terrain, sizeof(int));
                    Stream.Read(&Altitude, sizeof(int));
                    Layer.Terrain = (ushort)Terrain;
                    Layer.Mask = (ushort)Mask;
                    Layer.Altitude = (short)Altitude;
                }
            }
        }
    };
}

// * ************************************************************
// * * END:                             c2dmapterrainobjpart.cs *
// * ************************************************************