using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace ClientUpdaterLib
{
    public class Installer
    {
        public static Action OnCompleted;
        public static Action<string> OnFileChange;
        public static Action<double> OnDownloadProgressChange;
        public static Action<double> OnTotalProgressChange;

        internal readonly Queue<string> DownloadQueue = new Queue<string>();
        internal readonly Queue<string> CopyQueue = new Queue<string>();

        /// <summary>
        /// Creates the Installer
        /// </summary>
        /// <param name="installPath">ABSOLUTE PATH to target directory</param>
        /// <param name="existingClientPath">ABSOLUTE PATH to reference client</param>
        public Installer(string installPath, string existingClientPath)
        {
            InstalledClient.Path = installPath;
            ExistingClient.Path = existingClientPath;
        }

        public static void Main()
        {
            var instance = new Installer(Environment.CurrentDirectory + "\\Test\\", Environment.CurrentDirectory + "\\Test - Copy\\");

            OnFileChange += Console.WriteLine;
            OnDownloadProgressChange += Console.WriteLine;
            OnTotalProgressChange += d => Console.Title = "Total Progress: " + d.ToString("F");


            instance.Setup().Wait();
            instance.Install().Wait();
            
            Console.ReadLine();
        }

        /// <summary>
        /// Verify files of the installed client. Undo's every change the user performed, such as edits and restores deleted files.
        /// </summary>
        /// <returns>awaitable task</returns>
        public Task VerifyClient(bool deleteRedundantFiles=false)
        {
            return Task.Run(() =>
            {
                var fileList = Path.Combine(InstalledClient.Path, "FileList.ini");
                if (File.Exists(fileList))
                    File.Delete(fileList);

                InstalledClient.Setup().Wait();
                InstalledClient.WriteList();

                fileList = Path.Combine(ExistingClient.Path, "FileList.ini");
                if (File.Exists(fileList))
                    File.Delete(fileList);

                ExistingClient.Setup().Wait();
                ExistingClient.WriteList();

                Install().Wait();

                foreach (var checksum in RemoteClient.Checksums)
                {
                    InstalledClient.Checksums.TryRemove(checksum.Key, out string ignored);
                }
                if (deleteRedundantFiles)
                {
                    OnFileChange?.Invoke($"Removing {InstalledClient.Checksums.Count} Orphaned Files");
                    foreach (var checksum in InstalledClient.Checksums)
                    {
                        var path = Path.Combine(InstalledClient.Path, checksum.Key);
                        if (File.Exists(path))
                            File.Delete(path);
                    }
                    InstalledClient.Checksums.Clear();
                    foreach (var checksum in RemoteClient.Checksums)
                        InstalledClient.Checksums.AddOrUpdate(checksum.Key, checksum.Value);
                    InstalledClient.WriteList();
                }

                OnCompleted?.Invoke();
            });
        }

        /// <summary>
        /// Reads or builds and reads Remote Client & Reference Client & Installed Client File List
        /// </summary>
        /// <returns>awaitable Task</returns>
        public Task Setup()
        {
            return Task.Run(() =>
            {
                RemoteClient.Setup().Wait();
                ExistingClient.Setup().Wait();
                InstalledClient.Setup().Wait();
                Install().Wait();
                OnCompleted?.Invoke();
            });
        }

        /// <summary>
        /// Installs the client to target directory by first comparing files, copying matching ones and downloading missing ones in the end - if any.
        /// </summary>
        /// <returns>awaitable task</returns>
        public Task Install()
        {
            return Task.Run(() =>
            {
                CompareFiles().Wait();
                CopyFiles().Wait();
                DownloadFiles().Wait();
                OnCompleted?.Invoke();
            });
        }

        private Task CompareFiles()
        {
            return Task.Run(() =>
            {
                OnFileChange?.Invoke("Comparing Files...");
                foreach (var kvp in RemoteClient.Checksums)
                {
                    if (InstalledClient.Checksums.TryGetValue(kvp.Key, out string checksum))
                    {
                        if (kvp.Value == checksum)
                            continue;
                    }

                    if (ExistingClient.Checksums.TryGetValue(kvp.Key, out string checksum2))
                    {
                        if (kvp.Value == checksum2)
                        {
                            CopyQueue.Enqueue(kvp.Key);
                            continue;
                        }
                    }
                    DownloadQueue.Enqueue(kvp.Key);
                }
                OnFileChange?.Invoke("Comparing Files Finished!");
            });
        }

        private Task CopyFiles()
        {
            return Task.Run(() =>
            {
                var counter = 0;
                var fileCount = CopyQueue.Count;

                while (CopyQueue.Count > 0)
                {
                    var file = CopyQueue.Dequeue();
                    counter++;

                    var directoryName = Path.GetDirectoryName(Path.Combine(InstalledClient.Path, file));
                    var sourcePath = Path.Combine(ExistingClient.Path, file);
                    var targetPath = Path.Combine(InstalledClient.Path, file);

                    if (File.Exists(directoryName))
                        File.Delete(directoryName);
                    if (!Directory.Exists(directoryName))
                        Directory.CreateDirectory(directoryName);
                    
                    OnFileChange?.Invoke($"Copying {counter}/{fileCount} - {((double)counter / fileCount * 100.0).ToString("F")}%");
                    OnTotalProgressChange?.Invoke((double)counter / fileCount * 100.0);

                    File.Copy(sourcePath, targetPath, true);

                    var checksum = Checksum.GetFor(targetPath);

                    InstalledClient.Checksums.AddOrUpdate(file, checksum);
                }
                OnFileChange?.Invoke("Finished Copying Files!");
                OnTotalProgressChange?.Invoke(100);
                OnDownloadProgressChange?.Invoke(100);
                OnCompleted?.Invoke();
            });
        }

        private Task DownloadFiles()
        {
            return Task.Run(() =>
            {
                var resetevent = new AutoResetEvent(true);
                var counter = 0;
                var fileCount = DownloadQueue.Count;

                while (DownloadQueue.Count > 0)
                {
                    var file = DownloadQueue.Dequeue();
                    counter++;

                    if (!Directory.Exists(Path.GetDirectoryName(Path.Combine(InstalledClient.Path, file))))
                        Directory.CreateDirectory(Path.GetDirectoryName(Path.Combine(InstalledClient.Path, file)));

                    OnFileChange?.Invoke($"Downloading {counter}/{fileCount} - {((double) counter/fileCount*100.0).ToString("F")}%");
                    using (var client = new WebClient())
                    {
                        client.DownloadProgressChanged += (sender, args) => OnDownloadProgressChange?.Invoke(args.ProgressPercentage);
                        var counter1 = counter;
                        client.DownloadFileCompleted += (sender, args) =>
                        {
                            var checksum = Checksum.GetFor(Path.Combine(InstalledClient.Path, file));
                            InstalledClient.Checksums.AddOrUpdate(file, checksum);
                            OnTotalProgressChange?.Invoke((double) counter1/fileCount*100.0);
                            resetevent.Set();
                        };

                        client.DownloadFileAsync(new Uri("http://alumni.votc.xyz/Downloads/Conquer/SelectiveDeploy/Client/" + file), Path.Combine(InstalledClient.Path, file));
                        resetevent.WaitOne();
                    }
                }
                OnFileChange?.Invoke("Finished Downloading Files!");
                OnTotalProgressChange?.Invoke(100);
                OnDownloadProgressChange?.Invoke(100);
                InstalledClient.WriteList();
                OnCompleted?.Invoke();
            });
        }
    }
}