using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Yi
{
    /// <summary>
    /// Threadsafe Console.
    /// </summary>
    public static class Output
    {
        private static readonly Thread YiConsoleThread = new Thread(QueueProcessor);
        private static readonly ConcurrentQueue<Action> PendingLines = new ConcurrentQueue<Action>();
        private static readonly ManualResetEventSlim Block = new ManualResetEventSlim(false);
        public static readonly List<string> Lines = new List<string>();
        public static readonly StreamWriter Writer = new StreamWriter(DateTime.UtcNow.ToString("dd-MM-yy")+"-Log.txt",true);
        private static void QueueProcessor()
        {
            while (true)
            {
                Block.Wait();

                if (PendingLines.IsEmpty)
                    Block.Reset();

                if (PendingLines.TryDequeue(out var action))
                    action.Invoke();
            }
        }

        public static void WriteLine(string text,bool bypassLines=false)
        {
            if (string.IsNullOrEmpty(text))
                return;
            if (!bypassLines)
            {
                lock (Lines)
                {
                    Lines.Add(text);

                    if (Lines.Count > 15)
                        Lines.RemoveAt(0);
                }
            }

            EnsureThreadAlive();

            PendingLines.Enqueue(() =>
            {
                //Console.ForegroundColor = color;
                UserInterface.Reference.WriteLine(text);
                //GameWorld.SendToRemoteClients(text, color);
                //Console.ResetColor();
            });
            Block.Set();
        }

        internal static void WriteLine(Exception e)
        { 
            EnsureThreadAlive();

            PendingLines.Enqueue(() =>
            {
                //Console.ForegroundColor = ConsoleColor.Red;
                UserInterface.Reference.WriteLine(e);
                Writer.WriteLine();
                Writer.WriteLine("######################## EXCEPTION BEGIN ########################");
                Writer.WriteLine("Exception: "+e.Message);
                Writer.WriteLine("Source: " + e.Source);
                Writer.WriteLine();
                Writer.WriteLine("Stack: "+e.StackTrace);
                Writer.WriteLine("######################## EXCEPTION END ########################");
                Writer.Flush();
                //Console.ResetColor();
                //GameWorld.SendToRemoteClients(e.Message + "\r\n" + e.StackTrace, ConsoleColor.Red);
            });
            Block.Set();
        }
        internal static void EnsureThreadAlive()
        {
            try
            {
                if (!YiConsoleThread.IsAlive)
                    YiConsoleThread.Start();
            }
            catch
            {
                UserInterface.Reference.WriteLine("Threads dead.");
            }
        }
    }
}