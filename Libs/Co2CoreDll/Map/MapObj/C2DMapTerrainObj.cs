// * ************************************************************
// * * START:                               c2dmapterrainobj.cs *
// * ************************************************************

// * ************************************************************
// *                      INFORMATIONS
// * ************************************************************
// * 2DMapTerrainObj structure for the library.
// * c2dmapterrainobj
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
    public unsafe struct C2DMapTerrainObj //Should be a class with few methods, but useless for a server.
    {
        public fixed Byte FileName[260];
        public MyPos PosCell;

        public C2DMapTerrainObjPart[] Parts;

        public void Load(String Folder)
        {
            String Path;
            fixed (Byte* pFileName = FileName)
                Path = Folder + new String((SByte*)pFileName);

            //Load the Scene file.
            using (var Stream = new FileStream(Path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                Int32 Amount;
                Stream.Read(&Amount, sizeof(Int32));

                Parts = new C2DMapTerrainObjPart[Amount];
                for (var i = 0; i < Amount; i++)
                {
                    Parts[i] = new C2DMapTerrainObjPart();
                    Parts[i].Load(Stream);
                }
            }
        }
    };
}

// * ************************************************************
// * * END:                                 c2dmapterrainobj.cs *
// * ************************************************************