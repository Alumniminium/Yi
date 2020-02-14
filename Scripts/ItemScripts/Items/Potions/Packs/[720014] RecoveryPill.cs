using Yi.Entities;
using Yi.Enums;
using Yi.Helpers;
using Yi.Items;
using Yi.Scripting;

namespace ItemScripts.Items.Potions.Packs
{
    [Script(Id = 720014)]
    public class RecoveryPillPack
    {
        public static bool Execute(YiObj entity, Item item)
        {
            try
            {
                if (entity.Inventory.Count <= 37)
                {
                    var pot = Item.Factory.Create(ItemNames.RecoveryPill);
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