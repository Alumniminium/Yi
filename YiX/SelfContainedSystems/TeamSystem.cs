using System;
using System.Collections.Concurrent;
using System.Linq;
using YiX.Calculations;
using YiX.Entities;
using YiX.Enums;
using YiX.Helpers;
using YiX.Network.Packets.Conquer;
using Player = YiX.Entities.Player;

namespace YiX.SelfContainedSystems
{
    public static class TeamSystem
    {
        public static readonly ConcurrentDictionary<int, TeamData> Teams = new ConcurrentDictionary<int, TeamData>();

        public static void ShareExp(YiObj attacker, YiObj target, uint bonus)
        {
            if (!Teams.ContainsKey(attacker.UniqueId))
                return;

            foreach (var member in Teams[attacker.UniqueId].Members.Values)
            {
                if (!Position.CanSeeBig(attacker, member))
                    continue;

                var exp = AttackCalcs.AdjustExp((int)bonus, attacker, target);

                if (exp > member.Level * 360)
                    exp = (uint)(member.Level * 360);

                if (member.Partner == attacker.Name)
                    exp *= 2;

                if (member.Class >= 133 && member.Class <= 135)
                    exp *= 2;

                //exp = (uint)Math.Round(exp*GameWorld.Maps[attacker.MapId].ExpModifier);

                (member as Player)?.Send(MsgText.Create("TeamData", member.Name, $"You've been awarded {exp} experience!", MsgTextType.Top));
                member.Experience += exp;
            }
        }

        public static void Join(YiObj leader, YiObj joining)
        {
            if (!Teams.ContainsKey(leader.UniqueId))
                return;

            (leader as Player)?.Send(MsgTeamUpdate.JoinLeave(leader, MsgTeamMemberAction.AddMember));
            Teams[leader.UniqueId].Members.AddOrUpdate(joining.UniqueId, joining);
            Teams.AddOrUpdate(joining.UniqueId, Teams[leader.UniqueId]);

            foreach (var member in Teams[leader.UniqueId].Members.Values.Where(member => member.UniqueId != joining.UniqueId))
            {
                (member as Player)?.Send(MsgTeamUpdate.JoinLeave(joining, MsgTeamMemberAction.AddMember));
                (joining as Player)?.Send(MsgTeamUpdate.JoinLeave(member, MsgTeamMemberAction.AddMember));
            }

            (joining as Player)?.Send(MsgTeamUpdate.JoinLeave(joining, MsgTeamMemberAction.AddMember));
        }

        public static void Disband(YiObj leader)
        {
            if (!Teams.ContainsKey(leader.UniqueId))
                return;

            foreach (var member in Teams[leader.UniqueId].Members.Values)
            {
                (member as Player)?.Send(MsgTeam.Kick(member));
                Teams.TryRemove(member.UniqueId);
            }

            (leader as Player)?.Send(MsgTeam.DisbandTeam(leader));
            leader.RemoveStatusEffect(StatusEffect.TeamLeader);
            Teams.TryRemove(leader.UniqueId);
        }

        public static void Leave(YiObj leader, YiObj kicked)
        {
            if (!Teams.ContainsKey(leader.UniqueId))
                return;

            foreach (var member in Teams[leader.UniqueId].Members.Values)
                (member as Player)?.Send(MsgTeam.Kick(kicked));

            Teams.TryRemove(kicked.UniqueId);
            Teams[leader.UniqueId].Members.TryRemove(kicked.UniqueId);
        }

        public class TeamData
        {
            public readonly YiObj Leader;
            public readonly ConcurrentDictionary<int, YiObj> Members;
            public bool Locked;
            public bool ItemsLocked;
            public bool MoneyLocked;

            public TeamData(YiObj owner)
            {
                Leader = owner;
                Members = new ConcurrentDictionary<int, YiObj>();
                Locked = false;
                ItemsLocked = true;
                MoneyLocked = false;
                Members.AddOrUpdate(owner.UniqueId, owner);
                owner.AddStatusEffect(StatusEffect.TeamLeader);
            }

            public void UpdateLeaderPosition(Player player)
            {
                if (player.UniqueId != Leader.UniqueId)
                    return;

                foreach (var member in Teams[player.UniqueId].Members)
                    (member.Value as Player)?.Send(MsgAction.Create(Environment.TickCount, Leader.UniqueId, Leader.UniqueId, Leader.Location.X, Leader.Location.Y, 0, MsgActionType.QueryTeamLeader));
            }
        }

        public static bool MemberOfTeam(int memberUniqueId, int leaderUniqueId, out TeamData data)
        {
            data = null;
            foreach (var kvp in Teams)
            {
                if (kvp.Value.Leader.UniqueId != leaderUniqueId)
                    continue;
                
                if (!kvp.Value.Members.ContainsKey(memberUniqueId))
                    continue;

                data = kvp.Value;
                return true;
            }

            return false;
        }
    }
}