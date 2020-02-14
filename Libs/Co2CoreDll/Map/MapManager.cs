using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;
#if SEVENZIP
using SevenZip;
#endif

namespace CO2_CORE_DLL.Map
{
    public unsafe class MapManager
    {
        private Dictionary<Int32, String> Maps;
        private Dictionary<String, GameMap> Data;

        /// <summary>
        /// Load a GameMap.dat file with all the maps data.
        /// </summary>
        public MapManager(String GameMap, String ConquerFolder)
        {
            Maps = new Dictionary<Int32, String>();
            Data = new Dictionary<String, GameMap>();

            using (var Stream = new FileStream(GameMap, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                Int32 Amount;
                Stream.Read(&Amount, sizeof(Int32));
                for (var i = 0; i < Amount; i++)
                {
                    Int32 MapId, Length, PuzzleSize;
                    Stream.Read(&MapId, sizeof(Int32));
                    Stream.Read(&Length, sizeof(Int32));

                    Byte* pFileName = stackalloc Byte[Length + 1];
                    Stream.Read(pFileName, Length);
                    Stream.Read(&PuzzleSize, sizeof(Int32));

                    var FileName = ConquerFolder + "\\" + new String((SByte*)pFileName);
                    FileName = FileName.ToLower();

                    var Extracted = false;
                    #if SEVENZIP
                    if (FileName.EndsWith(".7z"))
                    {
                        using (var SevenZip = new SevenZipExtractor(FileName))
                        {
                            for (Int32 k = 0; k < SevenZip.ArchiveFileData.Count; k++)
                                SevenZip.ExtractFiles(Path.GetDirectoryName(FileName) + "\\", SevenZip.ArchiveFileData[k].Index);
                        }
                        FileName = FileName.Replace(".7z", ".dmap");
                        Extracted = true;
                    }
                    #endif

                    GameMap Map = null;
                    if (!Data.ContainsKey(FileName))
                    {
                        try { Map = new GameMap(FileName); }
                        catch { continue; }
                        Data.Add(FileName, Map);
                    }
                    else
                        Map = Data[FileName];

                    if (!Maps.ContainsKey(MapId))
                        Maps.Add(MapId, FileName);

                    if (Extracted)
                        File.Delete(FileName);
                }
            }
        }

        /// <summary>
        /// Create a dynamic map that will be linked to the other map data.
        /// </summary>
        public Boolean Link(Int32 DynId, Int32 MapId)
        {
            lock (Maps)
            {
                lock (Data)
                {
                    if (!Maps.ContainsKey(MapId) || Maps.ContainsKey(DynId))
                        return false;

                    if (!Data.ContainsKey(Maps[MapId]))
                        return false;

                    Maps.Add(DynId, Maps[MapId]);
                    return true;
                }
            }
        }

        /// <summary>
        /// Get the GameMap object linked to the MapId.
        /// </summary>
        public GameMap GetMap(Int32 MapId)
        {
            lock (Maps)
            {
                lock (Data)
                {
                    if (!Maps.ContainsKey(MapId))
                        return null;

                    if (!Data.ContainsKey(Maps[MapId]))
                        return null;

                    return Data[Maps[MapId]];
                }
            }
        }
    }
}
