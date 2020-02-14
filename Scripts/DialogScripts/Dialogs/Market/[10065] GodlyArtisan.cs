using Yi.Enums;
using Yi.Helpers;
using Yi.Items;
using Yi.Scripting;
using Player = Yi.Entities.Player;

namespace DialogScripts.Dialogs.Market
{
    [Script(Id = 10065)]
    public class GodlyArtisan
    {
        public static bool Execute(Player player, int npcId, byte control, string input)
        {
            try
            {
                switch (control)
                {
                    case 0:
                    {
                        using (var yi = new NpcDialog(54))
                        {
                            yi
                                .Text($"Hello {player.Name.TrimEnd('\0')}, I can assist you repair your items to their maximum durability for only 5 Meteors, Are you interested?")
                                .Link("Head Gear", 1)
                                .Link("Necklace", 2)
                                .Link("Armour", 3)
                                .Link("RightWeapon Hand", 4)
                                .Link("LeftWeapon Hand", 5)
                                .Link("Ring", 6)
                                .Link("Boots", 7)
                                .Link("Just passing by.", 255)
                                .Finish();

                            player.Send(yi);
                        }
                        break;
                    }
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                    case 6:
                    case 7:
                    {
                        if (player.Inventory.HasItem(ItemNames.Meteor, 5))
                        {
                            Item equipment;
                            if (player.Equipment.TryGetValue((MsgItemPosition)control, out equipment))
                            {
                                if (equipment.CurrentDurability > equipment.MaximumDurability)
                                    equipment.CurrentDurability = equipment.MaximumDurability;
                                if (equipment.CurrentDurability <= 1)
                                {
                                    player.Inventory.RemoveItem(ItemNames.Meteor, 5);
                                    equipment.CurrentDurability = equipment.MaximumDurability;
                                }
                                else
                                {
                                    using (var yi = new NpcDialog(60))
                                    {
                                        yi
                                            .Text("You do not have an item equipped in that slot.")
                                            .Link("I see.", 255)
                                            .Finish();

                                        player.Send(yi);
                                    }
                                }
                            }
                        }
                        else
                        {
                            using (var yi = new NpcDialog(54))
                            {
                                yi
                                    .Text("You do not have enough Meteors.")
                                    .Link("That is ok.", 255)
                                    .Finish();

                                player.Send(yi);
                            }
                        }
                        break;
                    }
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