using System;
using System.Collections.Concurrent;
using System.IO;
using System.Security.Cryptography;
using System.Threading;

namespace ClientWebUpdater
{
    class Program
    {
        public static readonly ConcurrentDictionary<string, string> FileChecksums = new ConcurrentDictionary<string, string>();
        public static FileSystemWatcher Watcher = new FileSystemWatcher(@"Y:\Webserver\html\Downloads\Conquer\SelectiveDeploy\Client\");
        public static readonly SHA256Managed Sha256 = new SHA256Managed();
        static void Main(string[] args)
        {
            //var checksumsFile = File.ReadAllText(@"Y:\Webserver\html\Downloads\Conquer\SelectiveDeploy\Checksums.txt");
            //foreach (var line in checksumsFile.Split(Environment.NewLine.ToCharArray()))
            //{
            //    if (string.IsNullOrEmpty(line))
            //        continue;

            //    var key = line.Split('=')[0];
            //    var value = line.Split('=')[1];

            //    FileChecksums.TryAdd(key, value);
            //}

            foreach (var entry in Directory.EnumerateFiles(@"Y:\Webserver\html\Downloads\Conquer\SelectiveDeploy\Client\", "*.*", SearchOption.AllDirectories))
            {
                Console.WriteLine("Calculating Checksum for " + entry.Replace(@"Y:\Webserver\html\Downloads\Conquer\SelectiveDeploy\Client\", ""));
                FileChecksums.TryAdd(entry.Replace(@"Y:\Webserver\html\Downloads\Conquer\SelectiveDeploy\Client\", ""), GetChecksum(entry));
            }
            WriteChecksums();
            Console.Clear();

            Watcher.EnableRaisingEvents = true;
            Watcher.NotifyFilter = NotifyFilters.Attributes | NotifyFilters.CreationTime |NotifyFilters.FileName |NotifyFilters.LastAccess |NotifyFilters.LastWrite |NotifyFilters.Size |NotifyFilters.Security;
            Watcher.IncludeSubdirectories = true;
            Watcher.Filter = "*.*";

            Watcher.Changed += (sender, eventArgs) =>
            {
                Console.WriteLine($"File Changed -> {eventArgs.FullPath.Replace(@"Y:\Webserver\html\Downloads\Conquer\SelectiveDeploy\Client\", "")}");
                FileChecksums[eventArgs.FullPath.Replace(@"Y:\Webserver\html\Downloads\Conquer\SelectiveDeploy\Client\","")] = GetChecksum(eventArgs.FullPath);
                WriteChecksums();
            };
            Watcher.Created += (sender, eventArgs) =>
            {
                Console.WriteLine($"File Created -> {eventArgs.FullPath.Replace(@"Y:\Webserver\html\Downloads\Conquer\SelectiveDeploy\Client\", "")}");
                FileChecksums.TryAdd(eventArgs.FullPath.Replace(@"Y:\Webserver\html\Downloads\Conquer\SelectiveDeploy\Client\", ""), GetChecksum(eventArgs.FullPath));
                WriteChecksums();
            };
            Watcher.Deleted += (sender, eventArgs) =>
            {
                Console.WriteLine($"File Deleted -> {eventArgs.FullPath.Replace(@"Y:\Webserver\html\Downloads\Conquer\SelectiveDeploy\Client\", "")}");
                string ignored;
                FileChecksums.TryRemove(eventArgs.FullPath.Replace(@"Y:\Webserver\html\Downloads\Conquer\SelectiveDeploy\Client\", ""), out ignored);
                WriteChecksums();
            };
            Watcher.Renamed += (sender, eventArgs) =>
            {
                Console.WriteLine($"File Renamed -> {eventArgs.OldFullPath.Replace(@"Y:\Webserver\html\Downloads\Conquer\SelectiveDeploy\Client\", "")}");
                string ignored;
                FileChecksums.TryRemove(eventArgs.OldFullPath.Replace(@"Y:\Webserver\html\Downloads\Conquer\SelectiveDeploy\Client\", ""), out ignored);
                FileChecksums.TryAdd(eventArgs.FullPath.Replace(@"Y:\Webserver\html\Downloads\Conquer\SelectiveDeploy\Client\", ""), GetChecksum(eventArgs.FullPath));
                WriteChecksums();
            };
            
            Console.WriteLine("Watching for changes...");
            while (true)
            {
                Console.ReadLine();
            }
        }

        private static void WriteChecksums()
        {
            using (var writer = new StreamWriter(@"Y:\Webserver\html\Downloads\Conquer\SelectiveDeploy\Checksums.txt", false))
            {
                foreach (var fileChecksum in FileChecksums)
                {
                    if (fileChecksum.Value == string.Empty)
                        continue;
                    writer.WriteLine(fileChecksum.Key + "=" + fileChecksum.Value);
                }
            }
        }

        private static string GetChecksum(string file)
        {
            FileAttributes attr = File.GetAttributes(file);
            if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                return "";
            try
            {
                using (var stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read, 4 * 1024 * 1024))
                    return BitConverter.ToString(Sha256.ComputeHash(stream)).Replace("-", string.Empty);
            }
            catch (Exception)
            {
                Thread.Sleep(1000);
                return GetChecksum(file);
            }
        }
    }
}
