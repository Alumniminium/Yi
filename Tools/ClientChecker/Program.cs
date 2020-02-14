using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Threading;
using Timer = System.Timers.Timer;

namespace ClientChecker
{
    public static class Program
    {
        public static readonly List<string> FilesToDownload = new List<string>();
        public static readonly List<string> FilesToCopy = new List<string>();
        public static readonly ConcurrentDictionary<string, string> TargetFileChecksums = new ConcurrentDictionary<string, string>();
        public static readonly ConcurrentDictionary<string, string> FileChecksums = new ConcurrentDictionary<string, string>();
        public static readonly SHA256Managed Sha256 = new SHA256Managed();
        public static readonly Timer CheckUpdatesTimer = new Timer();

        public static string ExistingClientPath = "";
        public static string InstallPath = "NewInstance\\";

        private static void Main()
        {
            //Console.Write("Paste Full Path to Client:");
            //ExistingClientPath = Console.ReadLine();

            //Console.Write("Path to new instance:");
            //InstallPath = Console.ReadLine();
            
            GetChecksumFile();

            GetChecksumsForFiles();

            CompareFiles();

            DownloadFiles();

            WriteChecksums();

            Console.WriteLine("Done");
            Console.ReadLine();
        }

        private static void DownloadFiles()
        {
            var resetevent = new ManualResetEvent(true);
            foreach (var file in FilesToDownload)
            {
                using (var client = new WebClient())
                {
                    resetevent.Reset();
                    Console.WriteLine($"Downloading: {file}");
                    client.DownloadDataAsync(new Uri("http://alumni.votc.xyz/Downloads/Conquer/SelectiveDeploy/Client/" + file));
                    client.DownloadProgressChanged += (sender, args) =>
                    {
                        Console.Title = $"{Path.GetFileName(file)} - {args.ProgressPercentage}";
                    };
                    client.DownloadDataCompleted += (sender, args) =>
                    {
                        if (!Directory.Exists(Path.GetDirectoryName(Path.Combine(InstallPath, file))))
                            Directory.CreateDirectory(Path.GetDirectoryName(Path.Combine(InstallPath, file)));
                        File.WriteAllBytes(Path.Combine(InstallPath, file), args.Result);
                        resetevent.Set();
                    };
                    resetevent.WaitOne();
                }
            }
        }

        private static void GetChecksumFile()
        {
            using (var client = new WebClient())
            {
                var checksumsFile = client.DownloadString("http://alumni.votc.xyz/Downloads/Conquer/SelectiveDeploy/Checksums.txt");

                foreach (var line in checksumsFile.Split(Environment.NewLine.ToCharArray()))
                {
                    if (string.IsNullOrEmpty(line))
                        continue;

                    var key = line.Split('=')[0];
                    var value = line.Split('=')[1];

                    TargetFileChecksums.TryAdd(key, value);
                }
            }
        }

        private static void GetChecksumsForFiles()
        {
            if (!Directory.Exists(ExistingClientPath))
                return;
            foreach (var filePath in Directory.EnumerateFiles(ExistingClientPath, "*.*", SearchOption.AllDirectories))
            {
                var normalizedPath = filePath.Replace(ExistingClientPath+"\\", "");

                if (TargetFileChecksums.ContainsKey(normalizedPath))
                {
                    var hash = GetChecksum(filePath);
                    FileChecksums.TryAdd(normalizedPath, hash);
                    Console.WriteLine(normalizedPath + " => " + hash);
                }
            }
        }

        public static void CompareFiles()
        {
            foreach (var checksum in TargetFileChecksums)
            {
                string fileChecksum;
                if (FileChecksums.TryGetValue(checksum.Key, out fileChecksum))
                {
                    if (checksum.Value == fileChecksum)
                    {
                        var targetPath = Path.GetDirectoryName(Path.Combine(InstallPath, checksum.Key));

                        if (!Directory.Exists(targetPath))
                            Directory.CreateDirectory(targetPath);

                        File.Copy(Path.Combine(ExistingClientPath, checksum.Key), Path.Combine(InstallPath, checksum.Key));
                    }
                    else
                    {
                        FilesToDownload.Add(checksum.Key);
                    }
                }
                else
                {
                    FilesToDownload.Add(checksum.Key);
                }
            }
        }

        private static void WriteChecksums()
        {
            using (var writer = new StreamWriter("Checksums.txt"))
            {
                foreach (var fileChecksum in FileChecksums)
                    writer.WriteLine(fileChecksum.Key + "=" + fileChecksum.Value);
            }
        }

        private static string GetChecksum(string file)
        {
            using (var stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read, 4*1024*1024))
                return BitConverter.ToString(Sha256.ComputeHash(stream)).Replace("-", string.Empty);
        }
    }
}
