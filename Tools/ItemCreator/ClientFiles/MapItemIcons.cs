using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;

namespace ItemCreator.ClientFiles
{
    public static class MapItemIcons
    {
        public static ConcurrentDictionary<int,MapItemIcon> Entries = new ConcurrentDictionary<int, MapItemIcon>();

        public static void Load()
        {
            using (var reader = new StreamReader(Config.ConquerPath + @"ani\MapItemIcon.ani"))
            {
                var line = NextLine(reader);
                while (!reader.EndOfStream)
                {
                    var icon = new MapItemIcon();
                    if (line.StartsWith("["))
                    {
                        line = line.Replace("[", "").Replace("]", "").Trim();
                        int.TryParse(line, out icon.Id);
                        line = NextLine(reader);
                        if (line.StartsWith("frameamount="))
                        {
                            line = line.Replace("frameamount=", "").Trim();
                            var amount = int.Parse(line);
                            for (var i = 0; i < amount; i++)
                            {
                                var frame = NextLine(reader);
                                frame = frame.Replace($"frame{i}=", "").Trim();
                                if(frame.StartsWith("["))
                                    break;
                                icon.Frames.Add(i, frame.Replace("/",@"\"));
                            }
                            if (Entries.ContainsKey(icon.Id))
                                continue;
                            
                            Entries.TryAdd(icon.Id, icon);
                        }
                    }
                    line = NextLine(reader);
                }
            }
            Console.WriteLine($"Loaded {Entries.Count} MapItemIcons. Total amount of frames: {Entries.Values.Sum(m=>m.Frames.Count)}");
        }

        public static void Save()
        {

        }

        private static string NextLine(StreamReader reader)
        {
            if (reader.EndOfStream)
                return string.Empty;
            var line = reader.ReadLine();
            return string.IsNullOrEmpty(line) ? NextLine(reader) : line.ToLowerInvariant();
        }
    }
}