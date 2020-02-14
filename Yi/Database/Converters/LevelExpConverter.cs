using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Yi.Helpers;
using Yi.Structures;

namespace Yi.Database.Converters
{
    public static class LevelExpConverter
    {
        public static Task Load()
        {
            return Task.Run(() =>
            {
                var lines = File.ReadAllLines("RAW\\LevelExp\\LevelExp.ini");
                foreach (var split in lines.Select(line => line.Split(' ')))
                {
                    var level = byte.Parse(split[0]);
                    var entry = new LevelExp(ulong.Parse(split[1]), int.Parse(split[2]));
                    Collections.LevelExps.AddOrUpdate(level, entry);
                }
            });
        }
    }
}