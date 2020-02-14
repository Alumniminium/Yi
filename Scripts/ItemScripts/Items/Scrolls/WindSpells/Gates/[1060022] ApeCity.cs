using Yi.Entities;
using Yi.Enums;
using Yi.Helpers;
using Yi.Items;
using Yi.Scripting;

namespace ItemScripts.Items.Scrolls.Gates
{
    [Script(Id = 1060022)]
    public class ApeCityScroll
    {
        public static bool Execute(YiObj entity, Item item)
        {
            try
            {
                if (entity.IsInJail()||!entity.CanUseScroll())
                    Message.SendTo(entity, "You can not use scrolls in this map.", MsgTextType.Top);
                else
                {
                    entity.Teleport(567, 584, 1020);
                    entity.Inventory.RemoveItem(item);
                }
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}