using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeDom.Providers.DotNetCompilerPlatform;
using Yi.Enums;

namespace Yi.Scripting
{
    public static class ScriptEngine
    {
        public static readonly Dictionary<ScriptType, ScriptContainer> Scripts = new Dictionary<ScriptType, ScriptContainer>();
        private const string SCRIPT_PATH_ROOT = @"Scripts\";
        public static Assembly[] Assemblies;
        public static string FolderName;
        public static string CachedPath;
        public static string[] Files;

        public static IYiScript CompileFolder(string path)
        {
            if (!Directory.Exists("Temp\\" + Path.GetFileName(path)))
                Directory.CreateDirectory("Temp\\" + Path.GetFileName(path));

            FolderName = Path.GetFileNameWithoutExtension(path);
            Assemblies = AppDomain.CurrentDomain.GetAssemblies();
            Files = Directory.GetFiles(SCRIPT_PATH_ROOT + FolderName, "*.cs", SearchOption.AllDirectories);
            CachedPath = "Cache\\" + FolderName + ".dll";

            if (!VerifyCache())
            {
                if (!TryCompile(path))
                {
                    Output.WriteLine($"Script {FolderName} failed to activate - No 'IScriptAPI' interface found.");
                    return null;
                }
            }
            return CreateInstance(out var compiled) ? compiled : null;
        }


        private static bool CreateInstance(out IYiScript script)
        {
            var appdomain = AppDomainFactory.CreateAppDomain(FolderName);
            script = AppDomainFactory.InstantiatePlugin(Environment.CurrentDirectory + "\\" + CachedPath, appdomain);
            return script != null;
        }

        private static bool TryCompile(string path)
        {
            var snippets = Files.Select(file => new CodeSnippetCompileUnit(File.ReadAllText(file))).ToArray();
            var compilerParameters = new CompilerParameters
            {
                CompilerOptions = "/unsafe /langversion:6",
                GenerateInMemory = false,
                IncludeDebugInformation = true,
                TempFiles = new TempFileCollection("Temp\\" + Path.GetFileName(path), true)
            };

            foreach (var assembly in Assemblies)
            {
                try
                {
                    compilerParameters.ReferencedAssemblies.Add(assembly.Location);
                }
                catch {/*Who cares*/}
            }

            var results = new CSharpCodeProvider().CompileAssemblyFromDom(compilerParameters, snippets);
            if (results.Errors.Count != 0)
            {
                Output.WriteLine("Buggy ScriptContainer: " + FolderName);
                foreach (var error in results.Errors.Cast<CompilerError>().Where(err => !err.IsWarning))
                    Output.WriteLine(error.ErrorText + $" [{FolderName}]");

                return false;
            }
            foreach (var warning in results.Errors.Cast<CompilerError>().Where(err => err.IsWarning))
                Output.WriteLine($"[{FolderName}] -> {warning.ErrorText}");

            CacheAssembly(results.PathToAssembly);
            return true;
        }

        private static bool VerifyCache() => Files.Select(n => new FileInfo(n)).All(f => f.LastWriteTimeUtc <= new FileInfo(CachedPath).LastWriteTimeUtc);

        private static void CacheAssembly(string path)
        {
            if (!Directory.Exists("Cache"))
                Directory.CreateDirectory("Cache");
            if (File.Exists(CachedPath))
                File.Delete(CachedPath);
            File.Move(path, CachedPath);
        }
    }
}
