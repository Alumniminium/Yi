using System;
using System.IO;
using System.Threading.Tasks;
using Yi.Database.Converters;
using Yi.Database.Converters.Dmap;
using Yi.Enums;
using Yi.Items;
using Yi.SelfContainedSystems;

namespace Yi.Database
{
    public static class DbConverter
    {
        public static async Task Convert()
        {
            try
            {
                if (!Directory.Exists("RAW"))
                {
                    Output.WriteLine("RAW not found! Restoring!");
                    //await Content.Content.RestoreAsync();
                    Output.WriteLine("RAW Restored!");
                }
                if (!Directory.Exists("Database"))
                    Directory.CreateDirectory("Database");

                Output.WriteLine("Rebuilding");
                UniqueIdGenerator.Load();
                MapManager.Load(@"RAW\ini\GameMap.dat", Environment.CurrentDirectory + "\\RAW\\");
                Item.ItemFactory.LoadDb();
                await Task.WhenAll(MonsterDb.Load(), PortalDb.Load(), MagicTypeConverter.Load(), ItemBonusConverter.Load(), StatpointConverter.Load(), LevelExpConverter.Load());
                NpcDb.Load();
                await Db.SaveAsJsonAsync(SaveType.All);
            }
            catch (Exception e)
            {
                Output.WriteLine(e);
                Output.WriteLine("Nigga, I couldn't load the Db, nor convert your old db. What the fuck are you doing?");
            }
        }
    }
}