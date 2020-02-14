using System.Collections.Generic;
using Yi.Entities;
using Yi.Enums;
using Yi.Helpers;
using Yi.Items;
using Yi.Scripting;
using Player = Yi.Entities.Player;

namespace CraftingScripts
{
    [Script(Id=1)]
    public class CraftingScript
    {
        private static Player _player;

        public static bool Execute(Player player, List<Item> items)
        {
            _player = player;

            //Add new recipes here, You can add as many id's as you want, they all have to be in the crafting window
            //but the order is ignored.

            return items.ContainsAll(1088001) && CraftDragonball(player, items);
        }

        private static bool CraftDragonball(YiObj player, List<Item> items)
        {
            //Do work here, check for item stats, eg quality, plus,.... 
            //return true if crafting recipe was complete, else return false.

            //Remove all (or some) of the items in the crafting window
            foreach (var item in items)
            {
                player.Inventory.RemoveItem(item);
            }

            //Add item that has been crafted to the Player

            var crafted = Item.Factory.Create(ItemNames.Dragonball);
            //Assign more stats like plus n shit
            player.Inventory.AddItem(crafted);
            return true;
        }
    }
}