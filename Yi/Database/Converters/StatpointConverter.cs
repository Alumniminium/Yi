using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Yi.Helpers;
using Yi.Structures;

namespace Yi.Database.Converters
{
    public static class StatpointConverter
    {
        public static Task Load()
        {
            return Task.Run(() =>
            {
                var archer = File.ReadAllLines("RAW\\StatPoints\\Archer.ini");
                var warrior = File.ReadAllLines("RAW\\StatPoints\\Warrior.ini");
                var trojan = File.ReadAllLines("RAW\\StatPoints\\Trojan.ini");
                var tao = File.ReadAllLines("RAW\\StatPoints\\Tao.ini");

                foreach (var split in archer.Select(line => line.Split(' ')))
                {
                    var level = byte.Parse(split[0]);
                    var entry = new Statpoint(short.Parse(split[1]), short.Parse(split[2]), short.Parse(split[3]), short.Parse(split[4]));
                    Collections.Statpoints.AddOrUpdate(4 * 100 + level, entry);
                }
                foreach (var split in warrior.Select(line => line.Split(' ')))
                {
                    var level = byte.Parse(split[0]);
                    var entry = new Statpoint(short.Parse(split[1]), short.Parse(split[2]), short.Parse(split[3]), short.Parse(split[4]));
                    Collections.Statpoints.AddOrUpdate(2 * 100 + level, entry);
                }
                foreach (var split in trojan.Select(line => line.Split(' ')))
                {
                    var level = byte.Parse(split[0]);
                    var entry = new Statpoint(short.Parse(split[1]), short.Parse(split[2]), short.Parse(split[3]), short.Parse(split[4]));
                    Collections.Statpoints.AddOrUpdate(1 * 100 + level, entry);
                }
                foreach (var split in tao.Select(line => line.Split(' ')))
                {
                    var level = byte.Parse(split[0]);
                    var entry = new Statpoint(short.Parse(split[1]), short.Parse(split[2]), short.Parse(split[3]), short.Parse(split[4]));
                    Collections.Statpoints.AddOrUpdate(12 * 100 + level, entry);
                    Collections.Statpoints.AddOrUpdate(13 * 100 + level, entry);
                    Collections.Statpoints.AddOrUpdate(14 * 100 + level, entry);
                }
            });
        }
    }
}