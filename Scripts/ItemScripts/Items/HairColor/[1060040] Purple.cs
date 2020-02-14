using Yi.Entities;
using Yi.Items;
using Yi.Scripting;
using Player = Yi.Entities.Player;

namespace ItemScripts.Items.HairColor
{
    [Script(Id = 1060040)]
    public class Purple
    {
        public static bool Execute(YiObj entity, Item item)
        {
            try
            {
                entity.Inventory.RemoveItem(item);
                const int color = 9;
                var player = entity as Player;
                if (player != null)
                {
                    var hairStyle = player.Hair % 100;
                    player.Hair = (ushort)(color * 100 + hairStyle);
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