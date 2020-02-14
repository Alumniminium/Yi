using Yi.Entities;
using Yi.Enums;
using Yi.Helpers;
using Yi.Items;
using Yi.Scripting;

namespace ItemScripts.Items.Potions.Mana
{
    [Script(Id = 1001010)]
    public class Tonic
    {
        public static bool Execute(YiObj player, Item item)
        {
            try
            {
                var human = player;
                if (human.CurrentMp == human.MaximumMp)
                {
                    Message.SendTo(player, "Your manapoints are full.", MsgTextType.Action);
                    return false;
                }
                if (human.CurrentMp + 500 > human.MaximumMp)
                    human.CurrentMp = human.MaximumMp;
                else
                    human.CurrentMp += 500;

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