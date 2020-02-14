using System.Collections.Generic;
using System.IO;
using Yi.Entities;
using Yi.Items;
using Yi.SelfContainedSystems;

namespace Yi.Database.Converters
{
    public static class ShopDb
    {
        public static void LoadShops()
        {
            using (var sr = new StreamReader("RAW\\Shop.dat"))
            {
                var items = new HashSet<int>(); ;
                while (!sr.EndOfStream)
                {
                    items.Clear();
                    var line = "";

                    while (line != null && !line.Contains("ID="))
                        line = sr.ReadLine();

                    var id = 0;
                    if (line != null)
                        id = int.Parse(line.Split('=')[1]);

                    while (line != null && !line.Contains("ItemAmount="))
                        line = sr.ReadLine();

                    byte amount = 0;
                    if (line != null)
                        amount = byte.Parse(line.Split('=')[1]);

                    for (var I = 0; I < amount; I++)
                    {
                        var line1 = sr.ReadLine();
                        if (line1 == null)
                            continue;
                        if (line1.Contains("Name="))
                            break;
                        items.Add(int.Parse(line1.Split('=')[1]));
                    }
                    YiObj npc;
                    
                    if (Collections.Npcs.TryGetValue(id, out npc))
                    {
                        foreach (var itemId in items)
                        {
                            var item = Item.Factory.Create(itemId);
                            item.UniqueId = item.ItemId;
                            npc.Inventory.AddBypass(item);
                        }
                        BoothSystem.Create(npc);
                    }
                }
            }
        }
    }
}