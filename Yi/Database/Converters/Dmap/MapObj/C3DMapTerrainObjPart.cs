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
using Yi.Helpers;

namespace Yi.Database.Converters.Dmap.MapObj
{
    public unsafe struct C2DMapTerrainObjPart //Should be a class with few methods, but useless for a server.
    {
        public fixed byte AniFile [256];
        public fixed byte AniTitle [64];
        public MyPos PosOffset;
        public uint AniInterval;
        public MySize SizeBase;
        public int Thick;
        public MyPos PosSceneOffset;
        public int Height;

        public LayerInfo[,] Cells;

        public void Load(FileStream stream)
        {
            if (stream == null)
                throw new ArgumentNullException();

            fixed (byte* ptr = AniFile)
                stream.Read(ptr, 256);
            fixed (byte* ptr = AniTitle)
                stream.Read(ptr, 64);
            fixed (MyPos* ptr = &PosOffset)
                stream.Read(ptr, sizeof(MyPos));
            fixed (uint* ptr = &AniInterval)
                stream.Read(ptr, sizeof(uint));
            fixed (MySize* ptr = &SizeBase)
                stream.Read(ptr, sizeof(MySize));
            fixed (int* ptr = &Thick)
                stream.Read(ptr, sizeof(int));
            fixed (MyPos* ptr = &PosSceneOffset)
                stream.Read(ptr, sizeof(MyPos));
            fixed (int* ptr = &Height)
                stream.Read(ptr, sizeof(int));

            Cells = new LayerInfo[SizeBase.Width, SizeBase.Height];
            for (var i = 0; i < SizeBase.Height; i++)
            {
                for (var j = 0; j < SizeBase.Width; j++)
                {
                    var layer = Cells[j, i];

                    uint mask;
                    int terrain;
                    int altitude;
                    stream.Read(&mask, sizeof(uint));
                    stream.Read(&terrain, sizeof(int));
                    stream.Read(&altitude, sizeof(int));
                    layer.Terrain = (ushort)terrain;
                    layer.Mask = (ushort)mask;
                    layer.Altitude = (short)altitude;
                }
            }
        }
    };
}

// * ************************************************************
// * * END:                             c2dmapterrainobjpart.cs *
// * ************************************************************