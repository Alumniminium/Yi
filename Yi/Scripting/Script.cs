using System;
using Yi.Entities;
using Yi.Enums;
using Yi.Items;
using Yi.Structures;

namespace Yi.Scripting
{
    public class ScriptContainer
    {
        private IYiScript _script;
        public string FolderName { get; }
        public ScriptType ScriptType => _script.ScriptType;

        public ScriptContainer(string folderName)
        {
            FolderName = folderName;
        }
        public bool Compile()
        {
            _script = ScriptEngine.CompileFolder(Environment.CurrentDirectory + "\\Scripts\\" + FolderName);
            if (_script == null)
            {
                Output.WriteLine($"{FolderName} failed to load.");
                return false;
            }
            Activate();
            YiCore.CompactLoh();
            return true;
        }

        public string Activate() => _script.Activate();
        public bool Execute(Player player, int npcId, byte control, string input =null) => _script.ExecuteDialog(player,npcId, control, input);
        public bool Execute(YiObj target, Skill skill) => _script.ExecuteSkill(target, skill, true);
        public bool Execute(Player player, Item item) => _script.ExecuteItem(player, item);
    }
}