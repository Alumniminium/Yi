using System;
using System.Diagnostics;
using YiX.Calculations;
using YiX.Entities;
using YiX.Enums;
using YiX.Network.Packets.Conquer;
using YiX.Scheduler;
using YiX.SelfContainedSystems;
using YiX.World;

namespace YiX.AttackSystems
{
    public struct Physical
    {
        public static int PhysicalAttack(YiObj attacker, MsgInteractType attackType = MsgInteractType.Physical)
        {
            if (!CanAttackPhysical(attacker, attacker.CurrentTarget))
                return -1;

            if (attacker.HasFlag(StatusEffect.Invisibility))
                attacker.RemoveStatusEffect(StatusEffect.Invisibility);

            if (attacker is Player player)
            {
                if (player.AttackJob != null)
                    player.AttackJob.Cancelled = true;
                if (player.CurrentTarget != null && player.CurrentTarget.Alive)
                {
                    player.AttackJob=YiScheduler.Instance.DoReturn(TimeSpan.FromMilliseconds(attacker.AttackSpeed), () => MsgInteract.Handle(player, MsgInteract.Create(attacker, attacker.CurrentTarget, attackType, 0)));
                }
            }
            return AttackCalcs.GetDamage(attacker, attacker.CurrentTarget, attackType);
        }
        public static bool VerifyPkMode(YiObj attacker, YiObj target)
        {
            if (target == null)
                return false;
            if (target is Monster && target.Look != 900 && target.Look != 910)
                return true;
            var player = attacker as Player;
            if (player == null)
                return true;
            if (target.Look == 900|| target.Look == 910)
                return player.PkMode == PkMode.Kill;
            if (target.HasFlag(StatusEffect.Flashing))
                return player.PkMode != PkMode.Peace;
            if (target.HasFlag(StatusEffect.RedName))
                return player.PkMode == PkMode.Kill;
            if (target.HasFlag(StatusEffect.BlackName))
                return player.PkMode != PkMode.Peace;
            if (player.PkMode == PkMode.Team)
                return !TeamSystem.Teams[player.UniqueId].Members.ContainsKey(target.UniqueId);
            return player.PkMode == PkMode.Kill;
        }

        private static bool CanAttackPhysical(YiObj attacker, YiObj target)
        {
            try
            {
                var conditions = new[]
                {
                    !target.HasFlag(StatusEffect.SpawnProtection),
                    VerifyPkMode(attacker, target),
                    Position.GetDistance(attacker, target) <= attacker.AttackRange,
                    target.Alive,
                    attacker.Alive
                };

                if (GameWorld.Maps.TryGetValue(attacker.MapId, out var map))
                {
                    if (map.Flags.HasFlag(MapFlags.NewbieProtect) && target.Level < 27 && target is Player)
                    {
                        attacker.GetMessage("SYSTEM", attacker.Name.TrimEnd('\0'), "You can't attack players below level 27 here.", MsgTextType.Action);
                        return false;
                    }
                    if (map.Flags.HasFlag(MapFlags.NoPk)&& target is Player)
                    {
                        attacker.GetMessage("SYSTEM", attacker.Name.TrimEnd('\0'), "You can't attack players here.", MsgTextType.Action);
                        return false;
                    }
                }

                return Conditions.True(conditions);
            }
            catch (Exception e)
            {
                if (Debugger.IsAttached)
                    Output.WriteLine($"[CanAttack] {e.Message} \r\n {e.StackTrace}");
                return false;
            }
        }

    }
}