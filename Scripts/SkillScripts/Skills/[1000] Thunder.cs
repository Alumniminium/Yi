using Yi.Entities;
using Yi.Enums;
using Yi.Scripting;

namespace SkillScripts.Skills
{
    [Script(Id=(int)SkillId.Thunder)]
    public static class Thunder
    {
        public static bool Execute(YiObj target, Yi.Structures.Skill skill, bool addSkill) => true;
    }
}