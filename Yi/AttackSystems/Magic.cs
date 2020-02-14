using System;
using System.Collections.Generic;
using System.Linq;
using Yi.AttackSystems.AttackCalculations;
using Yi.Calculations;
using Yi.Database;
using Yi.Entities;
using Yi.Enums;
using Yi.Helpers;
using Yi.Network.Packets.Conquer;
using Yi.Scheduler;
using Yi.Scripting;
using Yi.SelfContainedSystems;
using Yi.Structures;
using Yi.World;

namespace Yi.AttackSystems
{
    internal static class Magic
    {
        public static void ExecuteAttack(Player player,Vector2 targetLocation)
        {
            if (!CostCovered(player))
                return;
            var targets = GetTargets(player, targetLocation, player.CurrentSkill.Info.Type).ToArray();

            foreach (var kvp in targets)
            {
                switch (MagicTypeHelper.Convert(player.CurrentSkill.Info.Type))
                {
                    case UseType.Damage:
                        kvp.Item1.GetHit(player, kvp.Item2);
                        break;
                    case UseType.Heal:
                        kvp.Item1.GetHealed(player, kvp.Item2);
                        break;
                    case UseType.Buff:
                        if (!ScriptEngine.Scripts.TryGetValue(ScriptType.SkillUsage, out var script))
                            continue;
                        if (script.Execute(kvp.Item1, player.CurrentSkill))
                            kvp.Item1.GetBuffed(player, 0);
                        else
                            Output.WriteLine($"Unknown Skill: {player.CurrentSkill.Id} | Lv: {player.CurrentSkill.Info.Level}");

                        break;
                    default:
                        Message.SendTo(player,$"Magic Attack Fail: {player.CurrentSkill.Info.Type} -> {MagicTypeHelper.Convert(player.CurrentSkill.Info.Type)}",
                            MsgTextType.Talk);
                        break;
                }

            }

            foreach (var packet in MsgMagicEffect.Create(player, targets))
            {
                ScreenSystem.Send(player, packet, true);
            }
        }

        private static bool CostCovered(YiObj player)
        {
            if (player.CurrentMp < player.CurrentSkill.Info.MpCost)
                return false;
            if (player.CurrentSkill.Info.StaminaCost > player.Stamina)
                return false;
            if (player.CurrentSkill.Info.WeaponSubType == 500)
            {
                if(player.Equipment.TryGetValue(MsgItemPosition.LeftWeapon, out var arrows))
                {
                    switch ((SkillId) player.CurrentSkill.Id)
                    {
                        case SkillId.Scatter:
                            if (arrows.CurrentDurability < 3)
                                return false;
                            for (int i = 0; i < 3; i++)
                                player.Equipment.RemoveDura(MsgItemPosition.LeftWeapon,true);
                            break;
                        case SkillId.RapidFire:
                            var cost = 2 + player.CurrentSkill.Level / 2;
                            if (arrows.CurrentDurability < cost)
                                return false;
                            for (var i = 0; i < cost; i++)
                                player.Equipment.RemoveDura(MsgItemPosition.LeftWeapon,true);
                            break;
                    }
                }
            }

            player.CurrentMp -= (ushort)player.CurrentSkill.Info.MpCost;
            player.Stamina -= player.CurrentSkill.Info.StaminaCost;

            return true;
        }

        private static IEnumerable<(YiObj, int)> GetTargets(YiObj player,Vector2 loc, MagicTypeEnum magicType)
        {
            var targetList = new List<(YiObj, int)>();

            switch (MagicTypeHelper.ConvertTargetingType(magicType))
            {
                case TargetingType.Single:
                    Single(player, targetList);
                    break;
                case TargetingType.Sector:
                    Sector(player,loc,targetList);
                    break;
                case TargetingType.Circle:
                    Circle(player, targetList);
                    break;
                case TargetingType.Self:
                    targetList.Add((player, 0));
                    break;
            }
            
            return targetList;
        }

        private static void ConfigureAutoAttack(YiObj attacker, int damage)
        {
            if (!(attacker is Player player))
                return;

            if (!attacker.CurrentTarget.Alive) return;
            if (attacker.CurrentTarget.CurrentHp - damage <= 0) return;
            if (!Constants.LoopableSkills.Contains(attacker.CurrentSkill.Id)) return;

            if (player.AttackJob != null)
                player.AttackJob.Cancelled = true;

            var delay = TimeSpan.FromMilliseconds(attacker.CurrentSkill.Info.IntoneDuration * 1.5);
            player.AttackJob = YiScheduler.Instance.DoReturn(delay, () => ExecuteAttack(player, attacker.Location));
        }

        private static void AwardExperience(YiObj attacker, IList<(YiObj Target, int Damage)> targetList)
        {
            if (!(attacker is Player))
                return;

            uint totalExperience = 0;
            foreach (var tuple in targetList)
            {
                totalExperience += AttackCalcs.AdjustExp(tuple.Damage, attacker, tuple.Target);
                TeamSystem.ShareExp(attacker, tuple.Target, (uint)Math.Round(tuple.Target.MaximumHp * 0.05));
            }
            attacker.Experience += totalExperience;
        }

        private static (YiObj target, int damage) CreateTargetEntry(YiObj attacker, YiObj target)
        {
            var skillType = MagicTypeHelper.Convert(attacker.CurrentSkill.Info.Type);
            (YiObj Target, int Damage) targetEntry = (target, 0);

            switch (skillType)
            {
                case UseType.Heal:
                    targetEntry.Damage = attacker.CurrentSkill.Info.Power;
                    break;
                case UseType.Damage:
                {
                    targetEntry.Damage = AttackCalcs.MagicDmg(attacker, target);

                    if (attacker is Player && target is Monster)
                        targetEntry.Damage = (int)AttackCalcs.AdjustPvE(targetEntry.Damage, attacker, (Monster) target);
                }
                    break;
            }

            return targetEntry;
        }

        private static void Single(YiObj attacker, IList<(YiObj, int)> targetList)
        {
            if (attacker.CurrentSkill.Info.Crime && !Physical.VerifyPkMode(attacker, attacker.CurrentTarget))
                return;

            var targetentry = CreateTargetEntry(attacker, attacker.CurrentTarget);
            targetList.Add(targetentry);

            AwardExperience(attacker, targetList);
            ConfigureAutoAttack(attacker, targetentry.damage);
        }

        private static void Circle(YiObj player, ICollection<(YiObj, int)> targetList)
        {
            var UniqueMonsters = new Dictionary<uint, int>();
            uint totalExp = 0;
            foreach (var entity in GameWorld.Maps[player.MapId].Entities.Values.Where(entity => Position.GetDistance(player.Location, entity.Location) <= player.CurrentSkill.Info.Distance))
            {
                if (entity == player || !entity.Alive)
                    continue;

                if (player.CurrentSkill.Info.WeaponSubType != 0)
                {
                    if (Position.GetDistance(entity.Location, player.Location) > Math.Max(2, Math.Min(player.CurrentSkill.Level + 1, 9)))
                        continue;
                }

                if (player.CurrentSkill.Info.Crime && Physical.VerifyPkMode(player, entity) || !player.CurrentSkill.Info.Crime)
                {
                    if (UniqueMonsters.ContainsKey(entity.Look))
                        targetList.Add((entity, UniqueMonsters[entity.Look]));
                    else
                    {
                        var targetentry = CreateTargetEntry(player, entity);
                        targetList.Add(targetentry);
                        UniqueMonsters.Add(entity.Look, targetentry.Item2);

                        totalExp += AttackCalcs.AdjustExp(targetentry.Item2, player, entity);

                    }
                }
                foreach (var kvp in UniqueMonsters)
                {
                    var mob = Collections.BaseMonsters.FirstOrDefault(f => f.Value.Look == kvp.Key).Value;
                    if (mob != null)
                        TeamSystem.ShareExp(player, mob, (uint)Math.Round(targetList.Where(m => m.Item1.Look == kvp.Key).Sum(s => s.Item2) * 0.05));
                }

                player.Experience += totalExp;
            }
        }

        private static void Sector(YiObj player, Vector2 loc, ICollection<(YiObj, int)> targetList)
        {
            var UniqueMonsters = new Dictionary<uint, int>();

            if (player.CurrentTarget != null)
            {
                    loc.X = player.CurrentTarget.Location.X;
                    loc.Y = player.CurrentTarget.Location.Y;
            }

            uint totalExp = 0;
            foreach (var obj in ScreenSystem.Entities[player.UniqueId])
            {
                if (!GameWorld.Maps[player.MapId].Entities.TryGetValue(obj, out var found))
                    continue;
                var entity = found;
                if (entity == null)
                    continue;
                if (entity.UniqueId == player.UniqueId)
                    continue;
                if (!entity.Alive)
                    continue;
                if (!Position.IsInArc(player.Location, loc, entity.Location, (int)Math.Min(player.CurrentSkill.Info.Distance, 18)))
                    continue;
                if (player.CurrentSkill.Info.Crime && Physical.VerifyPkMode(player, entity) || !player.CurrentSkill.Info.Crime)
                {
                    if (player.CurrentSkill.Info.WeaponSubType != 0)
                    {
                        if (Position.GetDistance(entity.Location, player.Location) > player.CurrentSkill.Info.Distance)
                            continue;
                    }

                    if (UniqueMonsters.ContainsKey(entity.Look))
                        targetList.Add((entity, UniqueMonsters[entity.Look]));
                    else
                    {
                        var targetentry = CreateTargetEntry(player, entity);
                        targetList.Add(targetentry);
                        UniqueMonsters.Add(entity.Look, targetentry.Item2);

                        totalExp += AttackCalcs.AdjustExp(targetentry.Item2, player, entity);

                    }
                }
            }

            foreach (var kvp in UniqueMonsters)
            {
                var mob = Collections.BaseMonsters.FirstOrDefault(f => f.Value.Look == kvp.Key).Value;
                if (mob != null)
                    TeamSystem.ShareExp(player, mob, (uint) Math.Round(targetList.Where(m=> m.Item1.Look==kvp.Key).Sum(s => s.Item2) * 0.05));
            }
            player.Experience += totalExp;
        }
    }
}