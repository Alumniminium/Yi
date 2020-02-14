using Yi.Entities;
using Yi.Items;
using Yi.Scripting;

namespace ItemScripts.Items.Scrolls.WindSpells
{
    [Script(Id = 1060027)]
    public class TwinCity2
    {
        public static bool Execute(YiObj entity, Item item)
        {
            try
            {
                if (entity.IsInJail()||!entity.CanUseScroll())
                    return false;
                entity.Teleport(795, 465, 1002);
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