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
using System.Runtime.InteropServices;

namespace CO2_CORE_DLL.Map
{
    public unsafe struct C2DMapTerrainObjPart //Should be a class with few methods, but useless for a server.
    {
        public fixed Byte AniFile[256];
        public fixed Byte AniTitle[64];
        public MyPos PosOffset;
        public UInt32 AniInterval;
        public MySize SizeBase;
        public Int32 Thick;
        public MyPos PosSceneOffset;
        public Int32 Height;

        public LayerInfo[,] Cells;

        public void Load(FileStream Stream)
        {
            if (Stream == null)
                throw new ArgumentNullException();

            fixed (Byte* ptr = AniFile)
                Stream.Read(ptr, 256);
            fixed (Byte* ptr = AniTitle)
                Stream.Read(ptr, 64);
            fixed (MyPos* ptr = &PosOffset)
                Stream.Read(ptr, sizeof(MyPos));
            fixed (UInt32* ptr = &AniInterval)
                Stream.Read(ptr, sizeof(UInt32));
            fixed (MySize* ptr = &SizeBase)
                Stream.Read(ptr, sizeof(MySize));
            fixed (Int32* ptr = &Thick)
                Stream.Read(ptr, sizeof(Int32));
            fixed (MyPos* ptr = &PosSceneOffset)
                Stream.Read(ptr, sizeof(MyPos));
            fixed (Int32* ptr = &Height)
                Stream.Read(ptr, sizeof(Int32));

            Cells = new LayerInfo[SizeBase.Width, SizeBase.Height];
            for (var i = 0; i < SizeBase.Height; i++)
            {
                for (var j = 0; j < SizeBase.Width; j++)
                {
                    var Layer = Cells[j, i];

                    UInt32 Mask;
                    Int32 Terrain;
                    Int32 Altitude;
                    Stream.Read(&Mask, sizeof(UInt32));
                    Stream.Read(&Terrain, sizeof(Int32));
                    Stream.Read(&Altitude, sizeof(Int32));
                    Layer.Terrain = (UInt16)Terrain;
                    Layer.Mask = (UInt16)Mask;
                    Layer.Altitude = (Int16)Altitude;
                }
            }
        }
    };
}

// * ************************************************************
// * * END:                             c2dmapterrainobjpart.cs *
// * ************************************************************