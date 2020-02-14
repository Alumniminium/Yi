using System.Collections.Generic;
using Yi.Calculations;
using Yi.Database;
using Yi.Entities;
using Yi.Enums;
using Yi.Items;
using Yi.Network.Packets.Conquer;
using Yi.Structures;

namespace Yi.SelfContainedSystems
{
    public static class PlayerReborn
    {
        public unsafe static void FirstReborn(Player player, byte newClass)
        {
            var oldClass = player.Class;

            player.Reborn = true;
            player.Class = newClass;
            player.Level = 15;
            Reallot(player);
            
            var rbSpells = GetRebornSpells(oldClass, newClass);
            var SpellsToLearn = new Dictionary<SkillId, Skill>();

            if(rbSpells != null)
                foreach (var spell in rbSpells)//get the potential spells to learn
                    if (!SpellsToLearn.ContainsKey((SkillId)spell))
                        SpellsToLearn.Add((SkillId)spell, new Skill(spell, 0, 0));

            if (player.Skills != null)
                foreach (var spell in player.Skills)//add existing spells to the to learn dictionary, then remove them
                {
                    if (!SpellsToLearn.ContainsKey(spell.Key))
                        SpellsToLearn.Add(spell.Key, new Skill((ushort)spell.Key, 0, 0));
                    player.RemoveSkill(spell.Value);//DOES NOT ACTUALLY REMOVE THE ITEM FROM THE DICTIONARY YET..JUST SENDS THE PACKET
                }
            player.Skills.Clear();

            if (SpellsToLearn != null)
                foreach (var spell in SpellsToLearn)//finally readd all of the rb spells and your old spells that have been reset
                    player.AddSkill(spell.Value);

            foreach (var gear in player.Equipment.Items)
            {
                if(gear.Key == MsgItemPosition.Head) Output.WriteLine("Head is real");
                if (gear.Value.ItemId >= 1050000 && gear.Value.ItemId <= 1051000) continue;//because arrow
                if (gear.Value.Level <= 15) { Output.WriteLine("The gear in " + gear.Key + " didn't need to be downgraded because it's level " + gear.Value.Level); continue; }//because we don't need to drop it more
                var jmp = 0;
                var itemId = gear.Value.ItemId;
                var itemType = itemId / 10000;
                CheckValidID:
                itemId -= 10;

                Collections.Items.TryGetValue(itemId, out var newgear);
                if(newgear != null && newgear.Level <= 15)
                {
                    Output.WriteLine("the item should be changed!");
                    var finalgear = Item.Factory.Create(itemId);
                    player.Equipment.TryRemove(gear.Key, out var reAdd);
                    player.Equipment.Items.TryAdd(gear.Key, finalgear);
                    player.ForceSend(new MsgItemInformation(finalgear, gear.Key), (ushort)sizeof(MsgItemInformation));
                }
                else if (jmp < 30)
                {
                    jmp++;
                    goto CheckValidID;
                }
            }
            EntityLogic.Recalculate(player);
        }
        private static void Reallot(Player player)
        {
            player.Strength = 0;
            player.Agility = 0;
            player.Vitality = 0;
            player.Spirit = 0;

            ushort stat = 0;
            if(player.Class == 135)
                switch(player.Level)
                {
                    case 112: stat = 1; break;
                    case 114: stat = 3; break;
                    case 116: stat = 6; break;
                    case 118: stat = 10; break;
                    case 120:
                    case 121: stat = 15; break;
                    case 122:
                    case 123: stat = 21; break;
                    case 124:
                    case 125: stat = 28; break;
                    case 126: 
                    case 127: stat = 36; break;
                    case 128:
                    case 129: stat = 45; break;
                    case 130: stat = 55; break;
                    default: stat = 0; break;
                }
            else switch (player.Level)
                {
                    case 121: stat = 1; break;
                    case 122: stat = 3; break;
                    case 123: stat = 6; break;
                    case 124: stat = 10; break;
                    case 125: stat = 15; break;
                    case 126: stat = 21; break;
                    case 127: stat = 28; break;
                    case 128: stat = 36; break;
                    case 129: stat = 45; break;
                    case 130: stat = 55; break;
                    default: stat = 0; break;
                }
            stat += 50;//IS THIS THE BASE RBN STAT AMOUNT??
            player.Statpoints = stat;
        }
        private static IEnumerable<ushort> GetRebornSpells(byte job, byte rebornInto)
        {
            ushort[] skills = null;
            job /= 10;
            rebornInto /= 10;
            switch (job)
            {
                case 1: // trojan
                    switch (rebornInto)
                    {
                        case 5:
                        case 4: skills = new ushort[] { 1110, 1190 }; break;
                        case 14: skills = new ushort[] { 1110, 1190, 1270 }; break;
                        case 1: skills = new ushort[] { 3050 }; break;
                        case 2: skills = new ushort[] { 1110, 1190, 5100 }; break;
                    }
                    break;
                case 2: // warrior
                    switch (rebornInto)
                    {
                        case 4:
                        case 14: skills = new ushort[] { 1020, 1040 }; break;
                        case 5:
                        case 1: skills = new ushort[] { 1040, 1015, 1320 }; break;
                        case 2: skills = new ushort[] { 3060 }; break;
                        case 13: skills = new ushort[] { 1020, 1040, 1025 }; break;
                    }
                    break;
                case 4: // archer
                    switch (rebornInto)
                    {
                        case 4: skills = new ushort[] { 5000 }; break;
                    }
                    break;
                case 13: // water tao
                    switch (rebornInto)
                    {
                        case 13: skills = new ushort[] { 3090 }; break;
                        case 2:
                        case 5:
                        case 1: skills = new ushort[] { 1005, 1090, 1095, 1195, 1085 }; break;
                        case 14: skills = new ushort[] { 1050, 1175, 1075, 1055 }; break;
                    }
                    break;
                case 14: // fire tao
                    switch (rebornInto)
                    {
                        case 14: skills = new ushort[] { 3080 }; break;
                        case 13: skills = new ushort[] { 1120 }; break;
                        case 1:
                        case 2:
                        case 5:
                        case 4: skills = new ushort[] { 1000, 1001, 1005, 1195 }; break;
                    }
                    break;
            }
            return skills;
        }
    }
}
