// * ************************************************************
// * * START:                                            tpd.cs *
// * ************************************************************

// * ************************************************************
// *                      INFORMATIONS
// * ************************************************************
// * TPD class for the library.
// * tpd.cs
// * 
// * --
// *
// * Feel free to use this class in your projects, but don't
// * remove the header to keep the paternity of the class.
// * 
// * ************************************************************
// *                      CREDITS
// * ************************************************************
// * Originally created by CptSky (March 13th, 2012)
// * Copyright (C) 2012 CptSky
// *
// * ************************************************************

using System;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;

namespace CO2_CORE_DLL.IO
{
    public partial class NetDragonDatPkg
    {
        /// <summary>
        /// NetDragon Data Package (Data)
        /// </summary>
        private unsafe class TPD
        {
            public const Int32 MAX_IDENTIFIERSIZE = 0x10;

            public const String TPD_IDENTIFIER = "NetDragonDatPkg";
            public const Int64 TPD_VERSION = 1000;

            public const Int32 TPD_UNKNOWN_1 = 0x01;
            public const Int32 TPD_UNKNOWN_2 = 0x03;

            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            public struct Header
            {
                public fixed Byte Identifier[MAX_IDENTIFIERSIZE];
                public Int64 Version;
                public Int32 Unknown1;
                public Int32 Unknown2;
            };

            private Encoding Encoding = Encoding.GetEncoding("Windows-1252");

            private Header* pHeader = null;
            private String Filename = null;

            public String GetFilename() { return Filename; }

            /// <summary>
            /// Create a new TPD handle.
            /// </summary>
            public TPD()
            {
                this.pHeader = (Header*)Kernel.calloc(sizeof(Header));
            }

            ~TPD()
            {
                Close();
                if (pHeader != null)
                    Kernel.free(pHeader);
            }

            /// <summary>
            /// Open the specified TPD package.
            /// </summary>
            public void Open(String Source)
            {
                Close();

                Filename = Source;
                if (!File.Exists(Source.ToLower().Replace(".tpd", ".tpi")))
                    throw new Exception("The TPD file does not have it's TPI equivalent: " + Filename);

                using (var Stream = new FileStream(Filename, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    var Buffer = new Byte[Kernel.MAX_BUFFER_SIZE];

                    Stream.Read(Buffer, 0, sizeof(Header));
                    Kernel.memcpy(pHeader, Buffer, sizeof(Header));

                    if (Kernel.cstring(pHeader->Identifier, MAX_IDENTIFIERSIZE) != TPD_IDENTIFIER)
                        throw new Exception("Invalid TPD Header in file: " + Filename);

                    if (pHeader->Version != TPD_VERSION)
                        throw new Exception("Unsupported TPD version!");
                }
            }

            /// <summary>
            /// Close the file, reset the dictionary and free all the used memory.
            /// </summary>
            public void Close()
            {
                Kernel.memset(pHeader, 0x00, sizeof(Header));
            }
        }
    }
}

// * ************************************************************
// * * END:                                              tpd.cs *
// * ************************************************************