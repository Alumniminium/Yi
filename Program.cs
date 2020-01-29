using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using YiX.Database;
using YiX.Network;

namespace YiX
{
    public static class Program
    {
        private static async Task Main(string[] arg)
        {
            Directory.SetCurrentDirectory(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));
            await Db.Load();
            Servers.Start(9958);

            while (true)
                Console.ReadLine();
        }
    }
}