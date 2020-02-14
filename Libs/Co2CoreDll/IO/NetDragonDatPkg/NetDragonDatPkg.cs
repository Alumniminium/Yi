// * ************************************************************
// * * START:                                netdragondatpkg.cs *
// * ************************************************************

// * ************************************************************
// *                      INFORMATIONS
// * ************************************************************
// * NetDragonDatPkg (TPI/TPD) class for the library.
// * netdragondatpkg.cs
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
using System.Collections.Generic;
using System.Runtime.InteropServices;
using ComponentAce.Compression.Libs.zlib;

namespace CO2_CORE_DLL.IO
{
    /// <summary>
    /// NetDragon Data Package
    /// </summary>
    public unsafe partial class NetDragonDatPkg
    {
        private TPI Package = null;

        /// <summary>
        /// Create a new NetDragonDatPkg handle.
        /// </summary>
        public NetDragonDatPkg()
        {
            this.Package = new TPI();
        }

        ~NetDragonDatPkg()
        {
            Package.Close();
            Package = null;
        }

        /// <summary>
        /// Open the specified NetDragonDatPkg package. (TPI/TPD)
        /// </summary>
        public void Open(String Source)
        {
            Source = Source.ToLower().Replace(".tpd", ".tpi");
            Package.Open(Source);
        }

        /// <summary>
        /// Close the file, reset the dictionary and free all the used memory.
        /// </summary>
        public void Close() { Package.Close(); }

        /// <summary>
        /// Check if an entry is linked by the specified path.
        /// </summary>
        public Boolean ContainsEntry(String Path) { return Package.ContainsEntry(Path); }

        /// Get the data of the entry linked by the specified path.
        /// All the data will be allocated in memory. It may fail.
        /// 
        /// Return false if the entry does not exist.
        /// </summary>
        public Boolean GetEntryData(String Path, out Byte[] Data) { return Package.GetEntryData(Path, out Data); }

        /// Get the data of the entry linked by the specified path.
        /// All the data will be allocated in memory. It may fail.
        /// 
        /// Return false if the entry does not exist.
        /// </summary>
        public Boolean GetEntryData(String Path, Byte** pData, Int32* pLength) { return Package.GetEntryData(Path, pData, pLength); }

        /// <summary>
        /// Extract all files contained in the package in the folder pointed by the destination path.
        /// </summary>
        public void ExtractAll(String Destination) { Package.ExtractAll(Destination); }

        /// <summary>
        /// Pack the folder pointed by the path (source) in a package pointed by the other path (destination).
        /// </summary>
        public static void Pack(String Source, String Destination) { TPI.Pack(Source, Destination); }
    }
}

// * ************************************************************
// * * END:                                  netdragondatpkg.cs *
// * ************************************************************