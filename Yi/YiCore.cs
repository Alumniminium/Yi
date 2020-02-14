using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Runtime;
using Yi.Database;
using Yi.Database.Converters;
using Yi.Enums;
using Yi.Helpers;
using Yi.Scheduler;
using Yi.SelfContainedSystems;

namespace Yi
{
    public static class YiCore
    {
        public static readonly SafeRandom Random = new SafeRandom();
        public static string ServerIp => GetIP();
        public static int FullBackupFrequency { get; } = 60 * 24;
        public static int DynamicBackupFrequency { get; } = 5;
        public static bool BackupInProgress { get; set; }
        public static int ActualPacketsPerSecond { get; set; }
        public static int PacketsPerSecond { get; set; }
        public static double ExpMulti { get; } = 1.25;

        public static bool Success(float chance) => (float)Random.Next(1, 1000001) / 10000 >= 100 - chance;

        public static void CompactLoh()
        {
            NativeMethods.MinimizeFootprint();
            GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
            GC.Collect(2, GCCollectionMode.Forced, true, true);
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
            GC.Collect(2, GCCollectionMode.Forced, true, true);
        }
        public static void ScheduleFullBackup()
        {
            if (Debugger.IsAttached)
                return;
            YiScheduler.Instance.Do(TimeSpan.FromMinutes(FullBackupFrequency), async () =>
            {
                await Db.SaveAsJsonAsync(SaveType.All);
                ScheduleFullBackup();
                CompactLoh();
            });
        }
        public static void ScheduleDynamicBackup()
        {
            if (Debugger.IsAttached)
                return;
            YiScheduler.Instance.Do(TimeSpan.FromMinutes(DynamicBackupFrequency), async () =>
            {
                await Db.SaveAsJsonAsync(SaveType.Dynamic);
                await ChatLog.Save();
                ScheduleDynamicBackup();
                CompactLoh();
            });
        }

        private static string GetIP()
        {
            if(Environment.UserName== "Faggot McNigger")
            {
                return new WebClient().DownloadString("https://wtfismyip.com/text");
            }
            if (Environment.UserName == "omgno")
            {
                return "70.185.214.175";//yea?
            }
            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("8.8.8.8", 80);
                var endPoint = socket.LocalEndPoint as IPEndPoint;
                socket.Dispose();
                return endPoint?.Address.ToString();
            }
        }
    }
}