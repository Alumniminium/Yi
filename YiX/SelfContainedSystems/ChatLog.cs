using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using YiX.Entities;
using YiX.Enums;
using YiX.Helpers;

namespace YiX.SelfContainedSystems
{
    public static class ChatLog
    {
        private static readonly ConcurrentDictionary<string, List<string>> LogEntries = new ConcurrentDictionary<string, List<string>>();

        public static void WriteLine(Player player, string to, string message, MsgTextType packetChannel)
        {
            if (!LogEntries.ContainsKey(player.Name))
                LogEntries.TryAdd(player.Name, new List<string>());

            LogEntries[player.Name].Add($"{DateTime.Now} | {player.Name.Size16()} -> {to.Size16()} {packetChannel.ToString().Size16()} -> {message}");
        }

        public static async Task Save()
        {
            if (!Directory.Exists("ChatLog"))
                Directory.CreateDirectory("ChatLog");

            foreach (var logEntry in LogEntries)
            {
                using (var writer = new StreamWriter("ChatLog\\" + logEntry.Key.Trim('\0') + ".txt", true))
                {
                    foreach (var line in logEntry.Value)
                        await writer.WriteLineAsync(line);
                    await writer.FlushAsync();
                    logEntry.Value.Clear();
                }
            }
        }
    }
}