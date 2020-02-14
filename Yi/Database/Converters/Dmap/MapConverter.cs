using System;
using System.IO;
using System.Linq;
using Yi.Enums;
using Yi.Helpers;
using Yi.Structures;
using Yi.World;

namespace Yi.Database.Converters.Dmap
{
    public static unsafe class MapManager
    {
        public static void Load(string gameMap, string conquerFolder)
        {
            using (var stream = new FileStream(gameMap, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                int amount;
                stream.Read(&amount, sizeof (int));
                for (var i = 0; i < amount; i++)
                {
                    int mapId, length, puzzleSize;
                    stream.Read(&mapId, sizeof (int));
                    stream.Read(&length, sizeof (int));
                    if (mapId == 0)
                        return;
                    byte* pFileName = stackalloc byte[length + 1];
                    stream.Read(pFileName, length);
                    stream.Read(&puzzleSize, sizeof (int));

                    var fileName = conquerFolder + "\\" + new string((sbyte*) pFileName);
                    fileName = fileName.ToLower();
                    //if(fileName.Contains("fb-darkatk"))
                    //    Debugger.Break();
                    //fb-darkatk.DMap
                    GameMap map;
                    try
                    {
                        map = new GameMap(fileName);
                    }
                    catch(Exception e)
                    {
                        Output.WriteLine($"DMAP NOT FOUND: {e.Message}");
                        continue;
                    }
                    var yimap = new Map
                    {
                        Id = (ushort) mapId,
                        Height = (ushort) map.Height,
                        Width = (ushort) map.Width
                    };
                    var array = new bool[map.Width*map.Height];
                    if (!GameWorld.Maps.ContainsKey((ushort) mapId))
                        GameWorld.Maps.Add((ushort) mapId, yimap);
                    yimap.Width = (ushort) map.Width;
                    yimap.Height = (ushort) map.Height;
                    for (var j = 0; j < map.Width; j++)
                    {
                        for (var k = 0; k < map.Height; k++)
                        {
                            var cell = map.GetCell(j, k);
                            if (cell != null) array[j*map.Width + k] = cell.Value.Accessible;
                        }
                    }
                    MapAccess.MapData.AddOrUpdate((ushort) mapId, new MapAccess(array));
                    LoadConfig(ref yimap, mapId);
                }
            }
            var removing = (from kvp in GameWorld.Maps where kvp.Key == 0 || MapAccess.MapData[kvp.Key].GroundLayer == null select kvp.Key).ToList();
            foreach (var i in removing)
                GameWorld.Maps.Remove(i);

            #region Image drawer

            //if (!Directory.Exists("Images"))
            //    Directory.CreateDirectory("Images");
            //foreach (var map in GameWorld.Maps)
            //{
            //    Image img = new Bitmap(map.Value.Width, map.Value.Height);
            //    var drawing = Graphics.FromImage(img);

            //    var textBrush = new Pen(Color.Green);
            //    for (ushort x = 0; x < map.Value.Width; x++)
            //    {
            //        for (ushort y = 0; y < map.Value.Height; y++)
            //        {
            //            if (map.Value.GroundValid(x, y))
            //                drawing.DrawRectangle(textBrush, x, y, 1, 1);
            //        }
            //    }
               
            //    img.Save($"Images\\{map.Key}.jpg",ImageFormat.Jpeg);
            //}
        }

        #endregion

            private static
            void LoadConfig(ref Map yimap, int mapId)
        {
            using (var kvf = new KeyValueFormat(@"RAW\MapConfigs\" + mapId + ".ini"))
            {
                yimap.Weather = (WeatherType)kvf.Load<byte>("Weather", 0);
                yimap.Flags = (MapFlags)kvf.Load("Flags", 0);
                var x = kvf.Load<ushort>("SpawnX", 0);
                var y = kvf.Load<ushort>("SpawnY", 0);
                yimap.SpawnVector2 = new Vector2(x, y);
            }
        }
    }
}