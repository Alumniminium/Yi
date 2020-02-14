using System;
using System.Collections.Generic;
using System.Reflection;
using Yi.Entities;
using Yi.Enums;
using Yi.Items;
using Yi.Scripting;
using Yi.Structures;

namespace CraftingScripts
{
    [Serializable]
    public class Script : MarshalByRefObject, IYiScript
    {
        public MarshalByRefObject Obj => this;
        public ScriptType ScriptType { get; set; } = ScriptType.Crafting;
        public static Dictionary<int, MethodInfo> Scripts = new Dictionary<int, MethodInfo>();
        
        public string Activate()
        {
            if (Scripts.Count == 0)
                Scripts = Reflector.GetMethods();
            return "|---> Crafts:       " + Scripts.Count;
        }
        
        bool IYiScript.ExecuteSkill(YiObj target, Skill skill, bool addSkill)
        {
            MethodInfo script;
            if (Scripts.TryGetValue(skill.Id, out script))
                return (bool)script.Invoke(null, new object[] { target, skill, addSkill });
            return false;
        }

        bool IYiScript.ExecuteDialog(Player player, int npcId, byte control, string input)
        {
            MethodInfo script;
            if (Scripts.TryGetValue(player.CurrentNpcId, out script))
                return (bool)script.Invoke(null, new object[] { player, control });
            return false;
        }
        bool IYiScript.ExecuteItem(Player player, Item item)
        {
            MethodInfo script;
            if (Scripts.TryGetValue(item.ItemId, out script))
                return (bool)script.Invoke(null, new object[] { player, item });
            return false;
        }
        public override object InitializeLifetimeService() => null;
    }
}
