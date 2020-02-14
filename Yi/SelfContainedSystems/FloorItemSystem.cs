using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Yi.Calculations;
using Yi.Entities;
using Yi.Enums;
using Yi.Helpers;
using Yi.Items;
using Yi.Network.Packets.Conquer;
using Yi.Scheduler;
using Yi.World;

namespace Yi.SelfContainedSystems
{
    public static class FloorItemSystem
    {
        public static ConcurrentDictionary<int, FloorItem> FloorItems = new ConcurrentDictionary<int, FloorItem>();

        public static void SetUpFloorItemSystem()
        {
            foreach (var kvp in FloorItems)
            {
                if (kvp.Value.Jobs == null)
                    kvp.Value.Jobs = new List<Job>();
                ScreenSystem.Create(kvp.Value);
                AddCountdown(kvp.Value);
                ScheduleDestruction(kvp.Value);
                ScheduleOwnerRemove(kvp.Value);
                GameWorld.Maps[kvp.Value.MapId].LoadInEntity(kvp.Value);
            }
        }

        public static void Drop(YiObj owner,YiObj dropper, Item drop)
        {
            var floorItem = new FloorItem
            {
                Original = drop,
                Owner = owner,
                MapId = dropper.MapId,
                DropTime = DateTime.UtcNow,
                Jobs = new List<Job>()
            };
            floorItem.UniqueId = UniqueIdGenerator.GetNext(EntityType.FloorItem);
            floorItem.Location.X = dropper.Location.X;
            floorItem.Location.Y = dropper.Location.Y;
            QueueDrop(floorItem);
        }
        public static void DropMoney(YiObj owner, YiObj dropper, int amount)
        {
            var floorItem = new FloorItem
            {
                Original = Item.Factory.CreateMoney(amount),
                Owner = owner is Player ? owner : null,
                MapId = dropper.MapId,
                Amount = amount,
                DropTime = DateTime.UtcNow,
                Jobs = new List<Job>()
            };

            if (floorItem.Amount > 0 && dropper is Player powner)
            {
                powner.Money -= floorItem.Amount;
            }

            floorItem.UniqueId = UniqueIdGenerator.GetNext(EntityType.FloorItem);
            floorItem.Location.X = dropper.Location.X;
            floorItem.Location.Y = dropper.Location.Y;
            QueueDrop(floorItem);
        }

        public static void Pickup(YiObj picker, int itemId)
        {
            if (!FloorItems.TryGetValue(itemId, out var drop))
                return;

            if (drop.Owner != null)
            {
                if (picker.UniqueId != drop.Owner.UniqueId)
                {
                    if (TeamSystem.MemberOfTeam(picker.UniqueId, drop.Owner.UniqueId, out var teamData2))
                    {
                        if (drop.Money > 0 && teamData2.MoneyLocked)
                        {
                            Message.SendTo(picker, $"This item belongs to {drop.Owner.Name.TrimEnd('\0')}",MsgTextType.Action);
                            return;
                        }

                        if (teamData2.ItemsLocked)
                        {
                            Message.SendTo(picker, $"This item belongs to {drop.Owner.Name.TrimEnd('\0')}",MsgTextType.Action);
                            return;
                        }
                    }
                }
            }

            if (drop.Amount > 0)
            {
                picker.Money += drop.Amount;

                AbortJobs(drop);
                drop.Destroy();
                if (drop.Amount > 2000)
                    ScreenSystem.Send(picker, MsgAction.CashEffect(picker, drop.Amount), true);
                picker.GetMessage(Constants.System,picker.Name,$"You've picked up {drop.Amount:##,###} gold.",MsgTextType.Top);
            }
            else if (picker.Inventory.AddItem(drop.Original))
            {
                AbortJobs(drop);
                drop.Destroy();
            }
        }

        private static void QueueDrop(FloorItem drop)
        {
            YiScheduler.Instance.Do(SchedulerPriority.Medium, () =>
            {
                for (var i = 0; i < 10; i++)
                {
                    if (!GameWorld.Maps[drop.MapId].ItemValid(drop.Location.X, drop.Location.Y))
                    {
                        drop.Location.X += (ushort)YiCore.Random.Next(-1, 2);
                        drop.Location.Y += (ushort)YiCore.Random.Next(-1, 2);
                    }
                    else
                    {
                        AddCountdown(drop);
                        ScheduleDestruction(drop);
                        ScheduleOwnerRemove(drop);
                        ScreenSystem.Create(drop);
                        FloorItems.AddOrUpdate(drop.UniqueId, drop);
                        GameWorld.Maps[drop.MapId].Enter(drop);
                        if (drop.Original != null && drop.Owner is Player player)
                        {
                            var packet = MsgItem.Create(drop.Original.UniqueId, drop.Original.UniqueId, drop.Original.UniqueId, MsgItemType.RemoveInventory);
                            player.Send(packet);
                        }
                        ScreenSystem.Send(drop, MsgFloorItem.Create(drop, MsgFloorItemType.Create), true);
                        drop.AddStatusEffect(StatusEffect.SuperMan);
                        return;
                    }
                }
                if (drop.Original != null && drop.Owner is Player owner)
                {
                    owner.Inventory.AddOrUpdate(drop.Original.UniqueId, drop.Original);
                    owner.Send(new MsgItemInformation(drop.Original, MsgItemPosition.Inventory));
                }
                else
                    drop.Owner.Money += drop.Amount;
            });
        }
        private static void AbortJobs(FloorItem drop)
        {
            foreach (var job in drop.Jobs)
                job.Cancelled = true;
            drop.Jobs.Clear();
        }
        private static void ScheduleOwnerRemove(FloorItem floorItem) => floorItem.Jobs.Add(YiScheduler.Instance.DoReturn(TimeSpan.FromSeconds(30), () => floorItem.Owner = null));
        private static void ScheduleDestruction(FloorItem floorItem) => floorItem.Jobs.Add(YiScheduler.Instance.DoReturn(TimeSpan.FromSeconds(60), floorItem.Destroy));

        private static void AddCountdown(FloorItem floorItem)
        {
            for (var i = 9; i >= 0; i--)
            {
                var number = i;
                floorItem.Jobs.Add(YiScheduler.Instance.DoReturn(TimeSpan.FromSeconds(89 - number), () => floorItem.Countdown(number)));
            }
        }
    }
}
