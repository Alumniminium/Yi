using Yi.Entities;
using Yi.Enums;
using Yi.Helpers;
using Yi.Items;
using Yi.Scripting;

namespace ItemScripts.Items.Potions.Packs
{
    [Script(Id = 720010)]
    public class AmritaPack
    {
        public static bool Execute(YiObj entity, Item item)
        {
            try
            {
                if (entity.Inventory.Count <= 37)
                {
                    var pot = Item.Factory.Create(ItemNames.Amrita);
                    entity.Inventory.AddItem(pot, 3);
                }
                else
                {
                    Message.SendTo(entity, "Not enough space in your invetory.", MsgTextType.Top);
                    return false;
                }

                entity.Inventory.RemoveItem(item);
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}