using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;

namespace ItemCreator.ClientFiles
{
    public static class ItemMiniIcons
    {
        public static ConcurrentDictionary<int, ItemMiniIcon> Entries = new ConcurrentDictionary<int, ItemMiniIcon>();

        public static void Load()
        {
            using (var reader = new StreamReader(Config.ConquerPath + @"ani\ItemMinIcon.ani"))
            {
                var line = NextLine(reader);
                while (!reader.EndOfStream)
                {
                    var icon = new ItemMiniIcon();
                    if (line.StartsWith("["))
                    {
                        icon.Key = line;
                        line = line.ToLowerInvariant();
                        line = line.Replace("[", "").Replace("]", "").Trim();
                        line = line.Replace("item", "");
                        int.TryParse(line, out icon.Id);
                        line = NextLine(reader);
                        line = line.ToLowerInvariant();
                        if (line.StartsWith("frameamount="))
                        {
                            line = line.Replace("frameamount=", "").Trim();
                            var amount = int.Parse(line);
                            for (var i = 0; i < amount; i++)
                            {
                                var frame = NextLine(reader);
                                line = line.ToLowerInvariant();
                                frame = frame.Replace($"frame{i}=", "").Trim();
                                if (frame.StartsWith("["))
                                    break;
                                icon.Frames.Add(i, frame.Replace("/", @"\"));
                            }

                            if (icon.Id == 0)
                            {
                                var fileName = Path.GetFileNameWithoutExtension(icon.Frames[0].ToLowerInvariant().Replace("frame0=", ""));
                                if (int.TryParse(fileName, out icon.Id))
                                {
                                    
                                }
                            }

                            if (Entries.ContainsKey(icon.Id))
                                continue;
                            Entries.TryAdd(icon.Id, icon);
                        }
                    }
                    line = NextLine(reader);
                }
            }
            Console.WriteLine($"Loaded {Entries.Count} ItemMiniIcons. Total amount of frames: {Entries.Values.Sum(m => m.Frames.Count)}");
            Save();
        }

        public static void Save()
        {
            using (var writer = new StreamWriter(Config.ConquerPath + @"ani\ItemMinIcon.txt"))
            {
                foreach (var itemMiniIcon in Entries)
                {
                    writer.WriteLine($"{itemMiniIcon.Value.Key}");
                    writer.WriteLine($"FrameAmount={itemMiniIcon.Value.Frames.Count}");
                    foreach (var frame in itemMiniIcon.Value.Frames)
                    {
                        writer.WriteLine($"Frame{frame.Key}={frame.Value}");
                    }
                    writer.WriteLine();
                }
            }
        }

        private static string NextLine(StreamReader reader)
        {
            if (reader.EndOfStream)
                return string.Empty;
            var line = reader.ReadLine();
            return string.IsNullOrEmpty(line) ? NextLine(reader) : line;
        }
    }
}