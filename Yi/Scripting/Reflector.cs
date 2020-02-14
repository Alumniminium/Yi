using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Yi.Scripting
{
    public static class Reflector
    {
        public static Dictionary<int, MethodInfo> GetMethods()
        {
            var methods = new Dictionary<int, MethodInfo>();
            foreach (var type in Assembly.GetCallingAssembly().GetTypes())
            {
                if (!type.GetCustomAttributes(false).OfType<ScriptAttribute>().Any())
                    continue;
                var commands = type.GetCustomAttributes(typeof (ScriptAttribute));
                foreach (var command in commands)
                {
                    foreach (var method in type.GetMethods().Where(method => method.Name == "Execute"))
                        methods.Add(((ScriptAttribute)command).Id, method);
                }
            }
            return methods;
        }
    }
}
