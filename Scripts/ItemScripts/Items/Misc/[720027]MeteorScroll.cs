using Yi.Entities;
using Yi.Enums;
using Yi.Helpers;
using Yi.Items;
using Yi.Scripting;

namespace ItemScripts.Items.Misc
{
    [Script(Id = 720027)]
    public class MeteorScroll
       
    {
        public static bool Execute(YiObj entity, Item item)
        {
            try
            {
                if (entity.Inventory.Count <= 30)
                {
                    var meteor = Item.Factory.Create(ItemNames.Meteor);
                    entity.Inventory.AddItem(meteor, 10);
                    entity.Inventory.RemoveItem(item);
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