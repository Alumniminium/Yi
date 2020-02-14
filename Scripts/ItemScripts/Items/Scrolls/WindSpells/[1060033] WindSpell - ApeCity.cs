using Yi.Entities;
using Yi.Items;
using Yi.Scripting;

namespace ItemScripts.Items.Scrolls.WindSpells
{
    [Script(Id = 1060033)]
    public class ApeCity2
    {
        public static bool Execute(YiObj entity, Item item)
        {
            try
            {
                if (entity.IsInJail()||!entity.CanUseScroll())
                    return false;
                entity.Teleport(106, 394, 1020);
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