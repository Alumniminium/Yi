using System;
using System.Collections.Concurrent;
using System.Net;
using System.Threading.Tasks;

namespace ClientUpdaterLib
{
    public static class RemoteClient
    {
        internal static readonly ConcurrentDictionary<string, string> Checksums = new ConcurrentDictionary<string, string>();
        private static void DownloadFileList()
        {
            using (var client = new WebClient())
            {
                var checksumsFile = client.DownloadString("http://alumni.votc.xyz/Downloads/Conquer/SelectiveDeploy/Checksums.txt").Split(Environment.NewLine.ToCharArray());

                foreach (var line in checksumsFile)
                {
                    if (string.IsNullOrEmpty(line))
                        continue;

                    var file = line.Split('=')[0];
                    var checksum = line.Split('=')[1];

                    Checksums.TryAdd(file, checksum);
                }
            }
        }

        public static Task Setup() => Task.Run(() => DownloadFileList());
    }
}
