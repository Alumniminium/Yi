﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime;

namespace YiX
{
    public static class YiCore
    {
        public static readonly Random Random = new Random();
        public static string ServerIp => GetIP();
        public static double ExpMulti { get; } = 1.25;

        public static bool Success(float chance) => Random.Next(0, 101) <= chance;

        public static void CompactLoh()
        {
            GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
            GC.Collect(2, GCCollectionMode.Forced, true, true);
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
            GC.Collect(2, GCCollectionMode.Forced, true, true);
        }
      
        private static string GetIP()
        {
            return new WebClient().DownloadString("https://wtfismyip.com/text");

            if (Environment.UserName== "Faggot McNigger")
            {
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