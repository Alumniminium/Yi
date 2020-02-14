using System.Collections.Generic;
using System.IO;

#if SEVENZIP
using SevenZip;
#endif

namespace ItemCreator.CoreDLL.Map
{
    public unsafe class MapManager
    {
        private readonly Dictionary<int, string> _maps;
        private readonly Dictionary<string, GameMap> _data;

        /// <summary>
        /// Load a GameMap.dat file with all the maps data.
        /// </summary>
        public MapManager(string gameMap, string conquerFolder)
        {
            _maps = new Dictionary<int, string>();
            _data = new Dictionary<string, GameMap>();

            using (var stream = new FileStream(gameMap, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                int amount;
                stream.Read(&amount, sizeof(int));
                for (var i = 0; i < amount; i++)
                {
                    int mapId, length, puzzleSize;
                    stream.Read(&mapId, sizeof(int));
                    stream.Read(&length, sizeof(int));

                    byte* pFileName = stackalloc byte[length + 1];
                    stream.Read(pFileName, length);
                    stream.Read(&puzzleSize, sizeof(int));

                    var fileName = conquerFolder + "\\" + new string((sbyte*)pFileName);
                    fileName = fileName.ToLower();

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

                    if (!_data.ContainsKey(fileName))
                    {
                        GameMap map;
                        try { map = new GameMap(fileName); }
                        catch { continue; }
                        _data.Add(fileName, map);
                    }

                    if (!_maps.ContainsKey(mapId))
                        _maps.Add(mapId, fileName);
                }
            }
        }

        /// <summary>
        /// Create a dynamic map that will be linked to the other map data.
        /// </summary>
        public bool Link(int dynId, int mapId)
        {
            lock (_maps)
            {
                lock (_data)
                {
                    if (!_maps.ContainsKey(mapId) || _maps.ContainsKey(dynId))
                        return false;

                    if (!_data.ContainsKey(_maps[mapId]))
                        return false;

                    _maps.Add(dynId, _maps[mapId]);
                    return true;
                }
            }
        }

        /// <summary>
        /// Get the GameMap object linked to the MapId.
        /// </summary>
        public GameMap GetMap(int mapId)
        {
            lock (_maps)
            {
                lock (_data)
                {
                    if (!_maps.ContainsKey(mapId))
                        return null;

                    return !_data.ContainsKey(_maps[mapId]) ? null : _data[_maps[mapId]];
                }
            }
        }
    }
}
