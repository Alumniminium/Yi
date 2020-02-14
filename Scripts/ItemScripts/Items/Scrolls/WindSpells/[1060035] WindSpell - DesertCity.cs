using Yi.Entities;
using Yi.Items;
using Yi.Scripting;

namespace ItemScripts.Items.Scrolls.WindSpells
{
    [Script(Id = 1060035)]
    public class DesertCity2
    {
        public static bool Execute(YiObj entity, Item item)
        {
            try
            {
                if (entity.IsInJail()||!entity.CanUseScroll())
                    return false;
                entity.Teleport(793, 549, 1000);
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