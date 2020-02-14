using Yi.Entities;
using Yi.Items;
using Yi.Scripting;

namespace ItemScripts.Items.Scrolls.WindSpells
{
    [Script(Id = 1060029)]
    public class PhoenixCastle2
    {
        public static bool Execute(YiObj entity, Item item)
        {
            try
            {
                if (entity.IsInJail()||!entity.CanUseScroll())
                    return false;
                entity.Teleport(734, 452, 1011);
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