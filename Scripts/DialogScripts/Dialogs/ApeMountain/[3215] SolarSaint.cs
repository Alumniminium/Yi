using Yi.Enums;
using Yi.Helpers;
using Yi.Items;
using Yi.Scripting;
using Player = Yi.Entities.Player;

namespace DialogScripts.Dialogs.ApeMountain
{
    [Script(Id = 3215)]
    public class SolarSaint
    {
        public static bool Execute(Player player, int npcId, byte control, string input)
        {
            try
            {
                switch (control)
                {
                    case 0:
                    {
                        using (var yi = new NpcDialog(60))
                        {
                            yi
                                .Text(
                                      "Hello my dear child, Many evil forces has found their way through the shadow realm to this world, I do my best to keep peace in this world.")
                                .Text(
                                      "Though there numbers increase with time so we have to clear them out every once in a while, Do you think you're ready to defend your planet from evil forces?")
                                .Link("When does the shadow realm open?", 1)
                                .Link(
                                      "I would like to take part in defending our world against these evil forces.", 2)
                                .Link("I've got Ultimate Pluto's Dark Horn!", 5)
                                .Link("That is too dangerous.", 255)
                                .Finish();

                            player.Send(yi);
                        }
                        break;
                    }
                    case 1:
                    {
                        using (var yi = new NpcDialog(60))
                        {
                            yi
                                .Text(
                                      "Usually they are at their maximum numbers on Mondays and Wednesdays, So the shadow realm opens on these days. Though I'm here to make sure hat the only")
                                .Text(" people to enter the shadow realm are ready for this dangerous task.")
                                .Link("What are the requirements?", 2)
                                .Link(
                                      "I would like to take part in defending our world against these evil forces.", 2)
                                .Link("Thank you for the information.", 255)
                                .Finish();

                            player.Send(yi);
                        }
                        break;
                    }
                    case 2:
                    {
                        using (var yi = new NpcDialog(60))
                        {
                            yi
                                .Text(
                                      "You need to be atleast level 110 or above, Not to mention that the better quality your armour is the safer you are in there.")
                                .Link("I think I'm ready for it!", 3)
                                .Link("I am too weak at this moment.", 255)
                                .Finish();

                            player.Send(yi);
                        }
                        break;
                    }
                    case 3:
                    {
                        using (var yi = new NpcDialog(60))
                        {
                            yi
                                .Text(
                                      "Since you seem so confident, I'll allow you to enter the shadow realm, either ways I have to warn you my dear child danger awaits.")
                                .Link("Send me in!", 4)
                                .Link("No, thanks.", 255)
                                .Finish();

                            player.Send(yi);
                        }
                        break;
                    }
                    case 4:
                    {
                        /*
                                if(WorldEvents.Discity.IsActive)
                                {
                                if(Player.Level >= 110)
                                {
                                
                                }
                                else
                                {
                                
                                }
                                }
                                else
                                {
                                
                                }
                                */
                        break;
                    }
                    case 5:
                    {
                        if (player.Inventory.HasItem(ItemNames.DarkHorn))
                        {
                            using (var yi = new NpcDialog(60))
                            {
                                yi
                                    .Text(
                                          "Outstanding! I shall reward you for your hard work.")
                                    .Link("Awesome!.", 255)
                                    .Finish();

                                player.Send(yi);
                            }
                            player.Inventory.RemoveItem(ItemNames.DarkHorn);
                            var starSowrd = Item.Factory.Create(ItemNames.StarSword);
                            player.Inventory.AddItem(starSowrd);
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