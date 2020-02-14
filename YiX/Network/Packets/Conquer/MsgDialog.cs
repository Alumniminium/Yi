using System;
using System.Collections.Generic;
using System.Linq;
using YiX.Calculations;
using YiX.Database.Squiggly;
using YiX.Entities;
using YiX.Enums;
using YiX.Helpers;
using YiX.Network.Sockets;
using YiX.Scheduler;
using YiX.Scripting;
using YiX.SelfContainedSystems;
using YiX.Structures;
using YiX.World;

namespace YiX.Network.Packets.Conquer
{
    public static class MsgDialog
    {
        public static int Control;
        public static int NpcId;
        public static PacketType PacketType;
        public static string Input;

        public static void Handle(Player player, byte[] packet)
        {
            Control = 0;
            player.NpcTasks = new Dictionary<int, int>();
            NpcId = BitConverter.ToInt16(packet, 4);
            Input = System.Text.Encoding.ASCII.GetString(packet, 14, packet[13]);
            player.CurrentNpcId = NpcId;

            if (GameWorld.Find(NpcId, out Npc npc) || NpcId == 1337) //1337 = Selector
            {
                if (npc != null)
                {
                    if (npc.Inventory.Count > 0)
                        npc.Type = 1;

                    switch (npc.Type)
                    {
                        case 3:
                            ProcessStorage(player);
                            return;
                        case 1:
                            Process(player,1);
                            return;
                    }
                }
            
                Process(player);
            }
            else
            {
                Output.WriteLine($"[{NpcId}]NPC not found.");
                Process(player);
            }

            BufferPool.RecycleBuffer(packet);
        }
        public static void HandleContinuation(Player player, byte[] packet)
        {
            Control = packet[10];
            if (player.NpcTasks == null || player.NpcTasks.Count == 0)
            {
                Process(player);
                return;
            }
            if (GameWorld.Find(player.CurrentNpcId, out Npc npc))
            {
                int task = 0;

                foreach (var kvp in player.NpcTasks)
                {
                    if (kvp.Key == Control)
                        task = kvp.Value;
                }
                player.NpcTasks.Clear();
                if (task == 0)
                    return;
                ConquerActionProcessor.ExecuteAction(npc, player, task);
            }
        }

        private static async void Process(Player player, int npcIdoverride = 0)
        {
            if (Control == 255 && player.Online)
            {
                player.CurrentNpcId = 0;
            }

            if (player.CurrentNpcId == 0)
                return;

            var id = player.CurrentNpcId;
            if (npcIdoverride != 0)
                id = npcIdoverride;

            switch (id)
            {
                case 1337:
                    {
                        switch (Control)
                        {
                            case 255:
                                player.CurrentNpcId = 1337;
                                using (var packet = new NpcDialog(10))
                                {
                                    packet

                                        .Text("Select Char")
                                        .Link("Next", 1)
                                        .Link("Select", 10)
                                        .Link("New", 100)
                                        .Finish();

                                    player.Send(packet);
                                }
                                break;
                            case 1:
                                var players = SelectorSystem.GetPlayersFor(player.AccountId).ToList();

                                var index = players.IndexOf(player);
                                if (index + 1 > players.Count - 1)
                                    index = 0;
                                else
                                    index++;

                                player = SelectorSystem.SwapCharacter(player, players[index]);
                                player.Send(LegacyPackets.CharacterInformation(player));
                                player.Send(MsgAction.MapShowPacket(player));
                                player.AddStatusEffect(StatusEffect.Frozen);
                                ScreenSystem.Create(player);
                                ScreenSystem.Update(player);
                                break;
                            case 10:
                                player.RemoveStatusEffect(StatusEffect.Frozen);
                                ScreenSystem.Create(player);
                                player.AddSpawnProtection();
                                player.IncrementXp();

                                foreach (var kvp in player.Skills)
                                    player.Send(MsgSkill.Create(kvp.Value));
                                foreach (var prof in player.Profs)
                                    player.Send(MsgProf.Create(prof.Value));

                                EntityLogic.Recalculate(player);
                                player.CurrentHp = player.MaximumHp;
                                player.CurrentMp = player.MaximumMp;
                                GameWorld.Maps[player.MapId].Enter(player);

                                player.Send(MsgAction.Create(player, (int)player.PkMode, MsgActionType.ChangePkMode));
                                player.Send(MsgUpdate.Create(player, player.Stamina, MsgUpdateType.Stamina));
                                player.Send(MsgUpdate.Create(player, player.Class, MsgUpdateType.Class));
                                player.Online = true;

                                if (player.HasFlag(StatusEffect.SuperMan))
                                {
                                    BuffSystem.Create(player);
                                    BuffSystem.AddBuff(player, new Buff(player, SkillId.Superman, TimeSpan.FromSeconds(10)));
                                }
                                if (player.HasFlag(StatusEffect.Cyclone))
                                {
                                    BuffSystem.Create(player);
                                    BuffSystem.AddBuff(player, new Buff(player, SkillId.Cyclone, TimeSpan.FromSeconds(10)));
                                }

                                if (player.PkPoints > 1)
                                {
                                    player.PkPJob = new Job(TimeSpan.FromSeconds(15), () => player.PkPoints--);
                                    YiScheduler.Instance.Do(player.PkPJob);
                                }
                                ScreenSystem.Send(player, MsgAction.Create(player, player.UniqueId, MsgActionType.EntityRemove));
                                ScreenSystem.Send(player, MsgSpawn.Create(player));
                                break;
                            case 100:
                                SelectorSystem.CreateNewCharacterFor(player.AccountId);
                                player.Disconnect();
                                break;
                        }
                        break;
                    }
                default:
                {

                    if (!await ScriptEngine.ActivateNpc(player, NpcId, (byte) Control, Input))
                    {
                        if (GameWorld.Find(NpcId, out Npc npc) && Control == 0)
                            ConquerActionProcessor.ExecuteAction(npc, player, Control);
                            //Message.SendTo(player, $"[{player.CurrentNpcId}] Npc has no script.", MsgTextType.Talk);
                    }
                        break;
                    }
            }
        }

        private static void ProcessStorage(Player player)
        {
            var storages = StorageSystem.GetStorageListForId(player, player.CurrentNpcId).ToList();
            switch (Control)
            {
                case 0:
                {
                    if (string.IsNullOrEmpty(Input))
                        goto case 2;
                    
                    if (uint.TryParse(Input, out var storageId))
                    {
                        var found = storages.FirstOrDefault(storage => storage.StorageId == storageId);
                        if (found != null)
                        {
                            player.CurrentStorageId = found.StorageId;
                            player.Send(MsgAction.Create(player, 4, MsgActionType.Dialog));
                            YiScheduler.Instance.Do(SchedulerPriority.MediumLow, () => StorageSystem.ShowStock(player));
                        }
                    }
                    break;
                }
                case 1:
                {
                    if (uint.TryParse(Input, out var storageId))
                    {
                        var found = storages.FirstOrDefault(storage => storage.StorageId == storageId);
                        if (found != null)
                        {
                            player.CurrentStorageId = found.StorageId;
                            player.Send(MsgAction.Create(player, 4, MsgActionType.Dialog));
                            YiScheduler.Instance.Do(SchedulerPriority.MediumLow, () => StorageSystem.ShowStock(player));
                            break;
                        }
                    }
                    DisplayStorages(player, storages);
                    break;
                }
                case 2:
                {
                    if (storages.Count < 1)
                    {
                        StorageSystem.NewStorage(player, player.CurrentNpcId);
                        player.ForceSend(MsgAction.Create(player, 4, MsgActionType.Dialog), 24);
                        //Player.Send(MsgStorage.Create(Player.CurrentStorageId, MsgStorageAction.List));
                        YiScheduler.Instance.Do(SchedulerPriority.MediumLow, () => StorageSystem.ShowStock(player));
                        break;
                    }
                    DisplayStorages(player, storages);
                    break;
                }
            }
        }
        private static void DisplayStorages(Player player, List<Storage> storages)
        {
            using (var dialog = new NpcDialog(10))
            {
                dialog.Text("     Id                            Name                          Owner                     Access\n");
                foreach (var storage in storages.TakeUpTo(10).OrderBy(s => s.StorageId))
                    dialog.Text($"{storage.ToString(player)}\n");

                dialog.Input("Storage ID", 1);
                dialog.Finish();
                player.Send(dialog);
            }
        }
    }
}