using Yi.Entities;
using Yi.Enums;
using Yi.Items;
using Yi.Structures;

namespace Yi.Scripting
{
    public interface IYiScript
    {
        ScriptType ScriptType { get; set; }
        string Activate();
        bool ExecuteSkill(YiObj target, Skill skill, bool addSkill);
        bool ExecuteDialog(Player player, int npcId, byte control, string input);
        bool ExecuteItem(Player player, Item item);
    }
}