using System;
using System.Collections.Concurrent;
using System.IO;

namespace ItemCreator.ClientFiles
{
    public static class WeaponReader
    {
        public static ConcurrentDictionary<int, Weapon> Entries = new ConcurrentDictionary<int, Weapon>();

        public static void Load()
        {
            using (var reader = new StreamReader(Config.ConquerPath + @"ini\Weapon.ini"))
            {
                var line = NextLine(reader);
                while (!reader.EndOfStream)
                {
                    var icon = new Weapon();
                    if (line.StartsWith("["))
                    {
                        line = line.Replace("[", "").Replace("]", "").Trim();
                        int.TryParse(line, out icon.Id);
                        line = NextLine(reader);
                        if (line.StartsWith("part="))
                            line = NextLine(reader);

                        if (line.StartsWith("mesh0="))
                        {
                            line = line.Replace("mesh0=", "").Trim();
                            var mesh = int.Parse(line);
                            icon.ModelId = mesh;
                            line = NextLine(reader);
                            if (line.StartsWith("texture0="))
                            {
                                line = line.Replace("texture0=", "").Trim();
                                var texture = int.Parse(line);
                                icon.TextureId = texture;
                                line = NextLine(reader);
                            }
                            if (Entries.ContainsKey(icon.Id))
                                continue;
                            Entries.TryAdd(icon.Id, icon);
                        }
                    }
                    line = NextLine(reader);
                }
            }
            Console.WriteLine($"Loaded {Entries.Count} Weapon.ini entries.");
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