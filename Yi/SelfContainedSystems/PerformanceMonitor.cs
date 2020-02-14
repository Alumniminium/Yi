using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Timers;
using System.Windows.Forms;
using Yi.Scheduler;
using Timer = System.Timers.Timer;

namespace Yi.SelfContainedSystems
{
    public static class PerformanceMonitor
    {
        public static Timer PacketTimer { get; set; }
        public static readonly List<float> CpuUsageSamples = new List<float>();
        public static float HighestCpuUsage;
        public static readonly Process ServerProcess = Process.GetCurrentProcess();

        public static float AverageCpuUsage
        {
            get
            {
                var tempList = new float[CpuUsageSamples.Count+1];
                CpuUsageSamples.CopyTo(tempList);

                var average = tempList.Sum();
                average= average / tempList.Length;

                return average;
            }
        }
        
        public static void Start()
        {
            GCMonitor.Calibrate();
            PacketTimer = new Timer(1000);
            PacketTimer.Elapsed += TimerElapsed;
            PacketTimer.Start();
        }

        private static void TimerElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            var cpuUsage = CpuMonitor.Instance.GetUsage();

            if (cpuUsage > HighestCpuUsage)
                HighestCpuUsage = cpuUsage;

            if (CpuUsageSamples.Count > 60*60)
                CpuUsageSamples.RemoveAt(0);

            CpuUsageSamples.Insert(0, CpuMonitor.Instance.GetUsage());

            var apps = Math.Max(1, YiCore.ActualPacketsPerSecond);
            var pps = Math.Max(1, YiCore.PacketsPerSecond);
            ServerProcess.Refresh();
            var increase = Math.Max(100, (float) Math.Max(1, Math.Round(pps / (float) apps, 2) * 100));
            
            UserInterface.Reference.Invoke(new MethodInvoker(delegate
            {
                UserInterface.Reference.label8.Text = YiScheduler.Instance.WorkItemCount.ToString();
                UserInterface.Reference.label10.Text = $@"{increase}% ({pps}->{apps})";
                UserInterface.Reference.label12.Text = $@"{GCMonitor.Get(0)},{GCMonitor.Get(1)},{GCMonitor.Get(2)} = {GCMonitor.Get(-1)}";
                UserInterface.Reference.label11.Text = $@"{ServerProcess.WorkingSet64 / 1024 / 1024} MB Private: {ServerProcess.PrivateMemorySize64 / 1024 / 1024} MB";
                UserInterface.Reference.label13.Text = $@"Current: {cpuUsage:00.00}%    Avg: {AverageCpuUsage:00.00}%   Max: {HighestCpuUsage:00.00}%";
                UserInterface.Reference.label14.Text = $@"Current: {cpuUsage / Environment.ProcessorCount:00.00}%    Avg: {AverageCpuUsage / Environment.ProcessorCount:00.00}%   Max: {HighestCpuUsage / Environment.ProcessorCount:00.00}%";
            }));
        }
    }
}