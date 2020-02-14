using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using YiX.Entities;

namespace YiX.Scripting
{
    public static class ScriptEngine
    {
        public static Dictionary<int, Script> NpcScripts = new Dictionary<int, Script>();
        public static ScriptOptions Options = ScriptOptions.Default;
        public static Assembly[] Assemblies;

        public static async Task Configure()
        {
            var namespaces = Assembly.GetExecutingAssembly().GetTypes()
            .Select(t => t.Namespace)
            .Where(t => !string.IsNullOrEmpty(t))
            .Distinct()
            .ToList();

            Assemblies = AppDomain.CurrentDomain.GetAssemblies();
            Options = ScriptOptions.Default
            .WithImports(namespaces.ToArray())
            .WithReferences(Assembly.GetExecutingAssembly());

            foreach (var assembly in Assemblies)
            {
                if (assembly.IsDynamic)
                    continue;
                Options = Options.AddReferences(assembly);
            }
            await ActivateNpc(null, 0, 0, null);
        }

        public static bool CompileNpc(string path)
        {
            if (!File.Exists(path))
                return false;

            var scriptCodeLines = File.ReadAllLines(path).ToList();
            var id = int.Parse(scriptCodeLines[0].Replace("#define Id_", ""));
            scriptCodeLines.RemoveAt(0);

            string scriptCode = "";

            var builder = new StringBuilder();
            foreach (var line in scriptCodeLines)
                builder.AppendLine(line);
            scriptCode = builder.ToString();

            scriptCode = scriptCode.Replace("#r \"../YiX.dll\"", $"#r \"{Environment.CurrentDirectory}/YiX.dll\"");
            scriptCode = scriptCode.Replace("#define Editor", "");
            var compiled = CSharpScript.Create(scriptCode, Options, typeof(NpcDialogGlobals));
            var diagnostics = compiled.GetCompilation().GetDiagnostics();

            var success = false;
            if (diagnostics.Any())
                Console.WriteLine(string.Join(Environment.NewLine, diagnostics.Distinct().Select(x => x.ToString())));
            else
                success = true;

            if (success)
                NpcScripts.Add(id, compiled);

            return success;
        }
        public static async Task<bool> ActivateNpc(Player player, int npcId, byte control, string input)
        {
            try
            {
                if (NpcScripts.TryGetValue(npcId, out Script compiled))
                {

                    var globals = new NpcDialogGlobals
                    {
                        player = player,
                        npcId = npcId,
                        control = control,
                        input = input
                    };

                    var result = await compiled.RunAsync(globals);

                    if (result.Exception != null)
                        Console.WriteLine(result.Exception.Message);
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }
    }
}
