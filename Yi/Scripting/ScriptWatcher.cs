using System;
using System.IO;
using System.Linq;
using Yi.Helpers;

namespace Yi.Scripting
{
    public static class ScriptWatcher
    {
        public static FileSystemWatcher Watcher;

        public static void Start()
        {
            Watcher = new FileSystemWatcher(Environment.CurrentDirectory + "\\Scripts\\", "*.cs")
            {
                NotifyFilter = NotifyFilters.LastWrite,
                IncludeSubdirectories = true,
                EnableRaisingEvents = true
            };
            Watcher.Changed += WatcherOnChanged;
            Watcher.Deleted += WatcherOnChanged;
            Watcher.Created += WatcherOnChanged;
        }

        private static void WatcherOnChanged(object sender, FileSystemEventArgs e)
        {
            Watcher.EnableRaisingEvents = false;
            var realFolder = e.FullPath.Replace(Environment.CurrentDirectory, "").Replace(@"\Scripts\ScriptingProjects\", "").Split('\\')[0];
            Output.WriteLine($"{e.ChangeType} -> {e.Name} -> Recompiling: {realFolder}");
            
            if (AppDomainFactory.AppDomains.TryGetValue(realFolder, out var old))
                AppDomainFactory.UnloadPlugin(old);
            
            var container = ScriptEngine.Scripts.FirstOrDefault(c => string.Equals(c.Value.FolderName, realFolder, StringComparison.InvariantCultureIgnoreCase)).Value;
            if (container == null)
            {
                Watcher.EnableRaisingEvents = true;
                return;  
            }

            if (container.Compile())
                ScriptEngine.Scripts.AddOrUpdate(container.ScriptType, container);


            Output.WriteLine($"{realFolder} Recompiled!");
            Watcher.EnableRaisingEvents = true;
        }
    }
}
