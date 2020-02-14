using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ClientUpdaterLib
{
    public static class InstalledClient
    {
        internal static string Path;
        internal static readonly ConcurrentDictionary<string, string> Checksums = new ConcurrentDictionary<string, string>();

        private static void CreateFileList()
        {
            if (!Directory.Exists(Path))
                Directory.CreateDirectory(Path);
            var files = Directory.EnumerateFiles(Path, "*.*", SearchOption.AllDirectories).ToList();
            var counter = 0;
            var fileCount = files.Count;
            foreach (var filePath in files)
            {
                counter++;
                var normalizedPath = filePath.Replace(Path, "");

                Installer.OnFileChange?.Invoke($"Checking Installed Client: {counter}/{fileCount} - {((double) counter/fileCount*100.0).ToString("F")}%");
                Installer.OnTotalProgressChange?.Invoke((double) counter/fileCount*100.0);
                var hash = Checksum.GetFor(filePath);
                Checksums.AddOrUpdate(normalizedPath, hash);
            }
            WriteList();
        }

        private static void ReadFileList()
        {
            var checksumsFile = File.ReadAllLines(System.IO.Path.Combine(Path,"FileList.ini"));
            foreach (var line in checksumsFile)
            {
                if (string.IsNullOrEmpty(line))
                    continue;

                var file = line.Split('=')[0];
                var checksum = line.Split('=')[1];

                Checksums.AddOrUpdate(file, checksum);
            }
        }
        public static void WriteList()
        {
            using (var writer = new StreamWriter(System.IO.Path.Combine(Path, "FileList.ini")))
            {
                foreach (var fileChecksum in Checksums)
                {
                    if (fileChecksum.Value == string.Empty)
                        continue;
                    writer.WriteLine(fileChecksum.Key + "=" + fileChecksum.Value);
                }
            }
        }

        public static Task Setup()
        {
            return Task.Run(() =>
            {
                Checksums.Clear();
                if (File.Exists(System.IO.Path.Combine(Path, "FileList.ini")))
                    ReadFileList();
                else
                    CreateFileList();
            });
        }
    }
}