using System;
using System.Collections.Generic;
using System.IO;

namespace Yi.Scripting
{
    public static class AppDomainFactory
    {
        public static readonly Dictionary<string,AppDomain> AppDomains=new Dictionary<string, AppDomain>(); 
        public static AppDomain CreateAppDomain(string dllName)
        {
            if (AppDomains.ContainsKey(dllName))
                UnloadPlugin(AppDomains[dllName]);
            var setup = new AppDomainSetup { ApplicationName = dllName, ConfigurationFile = dllName + ".dll.config", ApplicationBase = AppDomain.CurrentDomain.BaseDirectory };
            var appDomain = AppDomain.CreateDomain(setup.ApplicationName, AppDomain.CurrentDomain.Evidence, setup);
            AppDomains.Add(dllName, appDomain);
            return appDomain;
        }

        public static IYiScript InstantiatePlugin(string dllName, AppDomain domain)
        {
            try
            {
                var plugIn = AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(dllName, Path.GetFileNameWithoutExtension(dllName) + ".Script") as IYiScript;
                return plugIn;

            }
            catch (Exception e)
            {
                Output.WriteLine(e);
                return null;
            }
        }

        public static void UnloadPlugin(AppDomain domain)
        {
            try
            {
                AppDomains.Remove(domain.FriendlyName);
                AppDomain.Unload(domain);
            }
            catch (Exception e)
            {
                Output.WriteLine(e);
            }
        }
    }
}
