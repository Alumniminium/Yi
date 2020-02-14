using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using YiX.Entities;
using YiX.Items;

namespace YiX.SelfContainedSystems
{
    public static class ItemLog
    {
        private static StreamWriter _writer;
        private static readonly ManualResetEvent Mre = new ManualResetEvent(false);
        private static readonly ConcurrentQueue<string> PendingWrites = new ConcurrentQueue<string>();
        private static readonly Thread ItemLogThread = new Thread(BeginWrite);

        public static void OnBuy(Player buyer, YiObj shop, uint price, Item bought)
        {
            Add("|------------------------------------------------------------------------------------------------------------------------------------------------");
            Add("[" + $"{DateTime.UtcNow:d / M / yyyy HH: mm: ss}" + "]" + buyer.Name + " bought " + bought + " from " + shop.Name + "/" + shop.UniqueId + " for " + price);
            Add("|------------------------------------------------------------------------------------------------------------------------------------------------");
        }

        public static void OnSell(Player seller, YiObj to, uint price, Item sold)
        {
            Add("|------------------------------------------------------------------------------------------------------------------------------------------------");
            Add("[" + $"{DateTime.UtcNow:d / M / yyyy HH: mm: ss}" + "]" + seller.Name + " sold " + sold +" to " + to.Name + "/" + to.UniqueId + " for " + price);
            Add("|------------------------------------------------------------------------------------------------------------------------------------------------");
        }

        public static void OnTrade(Player me, Player you, uint meMoney, uint youMoney, List<Item> meItems,List<Item> youItems)
        {
            Add("|------------------------------------------------------------------------------------------------------------------------------------------------");
            Add("[" + $"{DateTime.UtcNow:d / M / yyyy HH: mm: ss}" + "]" + me.Name + "/" + me.UniqueId + " & " +you.Name + "/" + you.UniqueId + " Traded!");
            Add("| " + me.Name + " 's side:");
            foreach (var I in meItems)
                Add("| " + I);
            Add("| Silvers:" + meMoney);
            Add("| " + you.Name + " 's side:");
            foreach (var I in youItems)
                Add("| " + I);
            Add("| Silvers:" + youMoney);
            Add("|------------------------------------------------------------------------------------------------------------------------------------------------");
        }

        public static void OnPickup(Player picker, Item drop)
        {
            Add("|------------------------------------------------------------------------------------------------------------------------------------------------");
            Add("[" + $"{DateTime.UtcNow:d / M / yyyy HH: mm: ss}" + "]" + picker.Name + "/" + picker.UniqueId +" Pickedup " + drop);
            Add("|------------------------------------------------------------------------------------------------------------------------------------------------");
        }
        public static void OnDrop(Player dropper, Item drop)
        {
            Add("|------------------------------------------------------------------------------------------------------------------------------------------------");
            Add("[" + $"{DateTime.UtcNow:d / M / yyyy HH: mm: ss}" + "]" + dropper.Name + "/" + dropper.UniqueId +" Dropped " + drop);
            Add("|------------------------------------------------------------------------------------------------------------------------------------------------");
        }

        public static void OnWithdraw(Player player, YiObj warehouse, Item item)
        {
            Add("|------------------------------------------------------------------------------------------------------------------------------------------------");
            Add("[" + $"{DateTime.UtcNow:d / M / yyyy HH: mm: ss}" + "]" + player.Name + "/" + player.UniqueId +" withdrew " + item);
            Add("|------------------------------------------------------------------------------------------------------------------------------------------------");
        }

        public static void OnDeposit(Player player, YiObj warehouse, Item item)
        {
            Add("|------------------------------------------------------------------------------------------------------------------------------------------------");
            Add("[" + $"{DateTime.UtcNow:d / M / yyyy HH: mm: ss}" + "]" + player.Name + "/" + player.UniqueId +" deposited " + item);
            Add("|------------------------------------------------------------------------------------------------------------------------------------------------");
        }

        private static void Add(string message)
        {
            if (message == null)
                return;
            PendingWrites.Enqueue(message);
            Mre.Set();
        }

        public static void Start()
        {
            _writer = new StreamWriter(@"ItemLogs\ItemLog.txt", true)
            {
                AutoFlush = true
            };
            ItemLogThread.IsBackground = true;
            ItemLogThread.Start();
        }

        private static string TakeJob()
        {
            PendingWrites.TryDequeue(out var message);
            if (PendingWrites.IsEmpty)
                Mre.Reset();

            return message;
        }

        private static void BeginWrite()
        {
            while (true)
            {
                Mre.WaitOne();
                _writer.WriteLine(TakeJob());
            }
        }

        public static void Stop()
        {
            ItemLogThread.Abort();
            _writer.Flush();
            _writer.Dispose();
        }
    }
}