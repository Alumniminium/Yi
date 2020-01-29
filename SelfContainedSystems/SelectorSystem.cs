using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using YiX.Entities;
using YiX.Enums;
using YiX.Helpers;
using YiX.World;

namespace YiX.SelfContainedSystems
{
    public static class SelectorSystem
    {
        private static ConcurrentDictionary<string, List<Player>> _players = new ConcurrentDictionary<string, List<Player>>();

        public static ConcurrentDictionary<string, List<Player>> Players
        {
            get => _players;
            set
            {
                _players = value;
                foreach (var value1 in value.Values)
                {
                    foreach (var player in value1)
                    {
                        if (GameWorld.Maps.ContainsKey(player.MapId))
                            GameWorld.Maps[player.MapId].LoadInEntity(player);
                    }
                }
            }
        }

        public static Player GetOrCreatePlayer(string accountId, string password)
        {
            if (!Players.TryGetValue(accountId, out var players)||players.Count==0)
                Players.AddOrUpdate(accountId, new List<Player>());
            else
                return players[0];

            var player = new Player(true)
            {
                AccountId = accountId,
                Password = password
            };
            Players[accountId].Add(player);
            Players[accountId][0].LoginType = LoginType.Create;
            return Players[accountId][0];
        }

        public static Player GetPlayerByUniqueId(uint uniqueId) => Players.SelectMany(kvp => kvp.Value).FirstOrDefault(player => player.UniqueId == uniqueId);

        public static void AddCharacterTo(string accountId, Player character)
        {
            if (!Players.TryGetValue(accountId, out var  characterList))
                return;

            var found = characterList.FirstOrDefault(c => c.LoginType == LoginType.Create);
            var index = Players[accountId].IndexOf(found);
            Players[accountId][index] = character;
        }

        public static void CreateNewCharacterFor(string accountId) => Players[accountId].Add(new Player(true));

        public static List<Player> GetPlayersFor(string accountId)
        {
            var charList = new List<Player>();
            if (!Players.TryGetValue(accountId, out var characterList))
                return null;
           
            foreach (var player in characterList)
                charList.Add(player);
            return charList;
        }

        public static Player SwapCharacter(Player a, Player b)
        {
            if(b.Online)
                b.Disconnect();

            var player = b;
            player.GameSocket = a.GameSocket;
            player.GameSocket.Ref = player;
            return player;
        }
        public static void RemoveCharacter(Player player)
        {
            if (Players.TryGetValue(player.AccountId, out var characterList))
                characterList.Remove(player);
        }
    }
}
