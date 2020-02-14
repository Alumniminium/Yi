using Yi.Entities;
using Yi.Enums;
using Yi.Helpers;
using Yi.Items;
using Yi.Scripting;

namespace ItemScripts.Items.Potions.Health
{
    [Script(Id = 1000020)]
    public class Painkiller
    {
        public static bool Execute(YiObj player, Item item)
        {
            try
            {
                if (player.CurrentHp == player.MaximumHp)
                {
                    Message.SendTo(player, "Your hitpoints are full.", MsgTextType.Action);
                    return false;
                }
                if (player.CurrentHp + 250 > player.MaximumHp)
                    player.CurrentHp = player.MaximumHp;
                else
                    player.CurrentHp += 250;

                player.Inventory.RemoveItem(item);
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}