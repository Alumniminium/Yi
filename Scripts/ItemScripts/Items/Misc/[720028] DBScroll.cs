using Yi.Entities;
using Yi.Enums;
using Yi.Helpers;
using Yi.Items;
using Yi.Scripting;

namespace ItemScripts.Items.Misc
{
    [Script(Id = 720028)]
    public class DragonballScroll
    {
    public static bool Execute(YiObj entity, Item item)
    {
        try
        {
            if (entity.Inventory.Count <= 30)
            {
                entity.Inventory.RemoveItem(item);
                var db = Item.Factory.Create(ItemNames.Dragonball);
                entity.Inventory.AddItem(db, 10);
                return true;
            }
            Message.SendTo(entity, "Not enough space in your inventory.", MsgTextType.Top);
            return false;
        }
        catch
        {
            return false;
        }
    }
}
}