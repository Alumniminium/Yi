using System;

namespace Yi.Scripting
{
    [AttributeUsage(AttributeTargets.Class,AllowMultiple = true)]
    public class ScriptAttribute : Attribute
    {
        public int Id { get; set; }
    }
}
