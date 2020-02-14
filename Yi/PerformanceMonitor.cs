using System;
using System.Diagnostics;
using System.Text;
using System.Timers;
using Yi.Database;
using Yi.Network.Sockets;
using Yi.Scheduler;
using Yi.Scripting;
using Yi.SelfContainedSystems;
using Yi.World;

namespace Yi
{
    public static class GCMonitor
    {
        public static int Gen0 = 0;
        public static int Gen1 = 0;
        public static int Gen2 = 0;

        public static int OffsetGen0 = 0;
        public static int OffsetGen1 = 0;
        public static int OffsetGen2 = 0;

        public static void Calibrate()
        {
            OffsetGen0 = GC.CollectionCount(0);
            OffsetGen1 = GC.CollectionCount(1);
            OffsetGen2 = GC.CollectionCount(2);
        }

        public static int Get(int gen)
        {
            switch (gen)
            {
                case -1:
                    return GC.CollectionCount(0) + GC.CollectionCount(1) + GC.CollectionCount(2) - OffsetGen0 - OffsetGen1 - OffsetGen2;
                case 0:
                    return GC.CollectionCount(0) - OffsetGen0;
                case 1:
                    return GC.CollectionCount(1) - OffsetGen1;
                case 2:
                    return GC.CollectionCount(2) - OffsetGen2;
            }
            return -1;
        }
    }
    public static class PerformanceMonitor
    {
        public static Timer PacketTimer { get; set; }

        public static void Start()
        {
            GCMonitor.Calibrate();
            int apps;
            int pps;
            float increase;
            var builder = new StringBuilder();
            int total;
            PacketTimer = new Timer(1000);
            var proc = Process.GetCurrentProcess();
            PacketTimer.Elapsed += (sender, args) =>
            {
                Console.Clear();
                builder.Clear();
                total = 0;

                apps = Math.Max(1, YiCore.ActualPacketsPerSecond);
                pps = Math.Max(1, YiCore.PacketsPerSecond);
                proc.Refresh();
                increase = Math.Max(100, (float)Math.Max(1, Math.Round(pps / (float)apps, 2) * 100));
                builder.AppendLine("|");
                builder.AppendLine("|------------ Statistics -------------");
                builder.AppendLine("|");
                builder.AppendLine($"|---> Scheduled Tasks: {YiScheduler.Instance.WorkItemCount}");
                builder.AppendLine($"|---> Buffer Count:    {BufferPool.Count}");
                builder.AppendLine($"|---> Sockets:         {increase}% ({pps}->{apps})");
                
                builder.AppendLine($"|---> GC Calls:        {GCMonitor.Get(0)},{GCMonitor.Get(1)},{GCMonitor.Get(2)} = {GCMonitor.Get(-1)}");

                builder.Append($"|---> RAM Usage:    {proc.WorkingSet64 / 1024 / 1024} MB Private: {proc.PrivateMemorySize64 / 1024 / 1024} MB");
                Output.WriteLine(builder.ToString(), ConsoleColor.Green);
            };
            PacketTimer.Start();
        }
    }
}