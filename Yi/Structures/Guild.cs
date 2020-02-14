using System;
using System.Collections.Generic;
using Yi.Database;
using Yi.Entities;
using Yi.Enums;
using Yi.Helpers;
using Yi.Network.Packets.Conquer;
using Yi.SelfContainedSystems;
using Yi.World;

namespace Yi.Structures
{
    [Serializable]
    public class Guild
    {
        public int UniqueId;
        public string Name;
        public int Leader;
        public string Bulletin;
        public int Funds;
        public List<int> Members;
        public List<Guild> Allies;
        public List<Guild> Enemies;
        public Guild()
        {

        }
        public Guild(Player leader, string name)
        {
            UniqueId = Collections.Guilds.Count + 1;
            Name = name;
            Leader = leader.UniqueId;
            Bulletin = $"{leader.Name.TrimEnd('\0')}'s Guild";
            Funds = 50000;
            Members = new List<int>();
            Allies = new List<Guild>();
            Enemies = new List<Guild>();
            Collections.Guilds.Add(UniqueId, this);
            Add(leader);
            leader.GuildRank = GuildRanks.Leader;
        }

        public void Add(YiObj human)
        {
            if (Members.Contains(human.UniqueId))
                return;

            Members.Add(human.UniqueId);
            human.Guild = this;
            human.GuildRank = GuildRanks.Member;

            if (human is Player player)
            {
                player.Send(MsgSyndicateSpawn.Create(player));
                Message.SendTo(player, $"{player.Name} has joined our guild!", MsgTextType.Guild);

                foreach (var guild in Allies)
                    player.Send(MsgSyndicate.Create(guild.UniqueId, GuildRequest.SetAlly));
                foreach (var guild in Enemies)
                    player.Send(MsgSyndicate.Create(guild.UniqueId, GuildRequest.SetEnemy));
            }

            ScreenSystem.Send(human, MsgSpawn.Create(human as Player), true);
        }

        public void Leave(YiObj human)
        {
            human.Guild = null;
            if (human is Player player)
                player.Send(MsgSyndicateSpawn.Create(player));
            Members.Remove(human.UniqueId);
        }

        public bool IsEnemy(Guild other) => Enemies.Contains(other);
        public bool IsAlly(Guild other) => Allies.Contains(other);

        public string[] GetMemberList()
        {
            var memberList = new string[Members.Count];
            for (var i = 0; i < Members.Count; i++)
            {
                if (GameWorld.Find(Members[i], out Player found))
                {
                    memberList[i] = found.Name.Replace("\0", "") + $" {found.Level} " + $"{(found.Online ? 1 : 0)}";
                }
            }
            return memberList;
        }
    }
}