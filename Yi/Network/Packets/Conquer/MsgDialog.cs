using System;
using System.Collections.Generic;
using System.Linq;
using Yi.Calculations;
using Yi.Entities;
using Yi.Enums;
using Yi.Helpers;
using Yi.Network.Sockets;
using Yi.Scheduler;
using Yi.Scripting;
using Yi.SelfContainedSystems;
using Yi.Structures;
using Yi.World;

namespace Yi.Network.Packets.Conquer
{
    public static class MsgDialog
    {
        public static Player Player;
        public static byte Control;
        public static int NpcId;
        public static PacketType PacketType;
        public static string Input;

        public static void Handle(Player player, byte[] packet)
        {
            Player = player;
            Control = packet[10];
            PacketType = (PacketType) BitConverter.ToUInt16(packet, 2);
            NpcId = PacketType == PacketType.MsgDialog ? BitConverter.ToInt16(packet, 4) : Player.CurrentNpcId;

            if (packet.Length > 16)
                Input = System.Text.Encoding.ASCII.GetString(packet, 14, packet[13]);

            if (PacketType == PacketType.MsgDialog)
                Player.CurrentNpcId = NpcId;

            if (GameWorld.Find(NpcId, out Npc npc) || NpcId == 1337) //1337 = Selector
            {
                if (npc != null)
                {
                    if (npc.Inventory.Count > 0)
                        npc.Type = 1;

                    switch (npc.Type)
                    {
                        case 3:
                            ProcessStorage();
                            return;
                        case 1:
                            Process(1);
                            return;
                    }
                }
            
                Process();
            }
            else
            {
                Output.WriteLine($"[{NpcId}]NPC not found.");
                Process();
            }

            BufferPool.RecycleBuffer(packet);
        }

        private static void Process(int npcIdoverride = 0)
        {
            if (Control == 255 && Player.Online)
                Player.CurrentNpcId = 0;

            if (Player.CurrentNpcId == 0)
                return;

            var id = Player.CurrentNpcId;
            if (npcIdoverride != 0)
                id = npcIdoverride;

            switch (id)
            {
                case 1337:
                    {
                        switch (Control)
                        {
                            case 255:
                                Player.CurrentNpcId = 1337;
                                using (var packet = new NpcDialog(10))
                                {
                                    packet

                                        .Text("Select Char")
                                        .Link("Next", 1)
                                        .Link("Select", 10)
                                        .Link("New", 100)
                                        .Finish();

                                    Player.Send(packet);
                                }
                                break;
                            case 1:
                                var players = SelectorSystem.GetPlayersFor(Player.AccountId).ToList();

                                var index = players.IndexOf(Player);
                                if (index + 1 > players.Count - 1)
                                    index = 0;
                                else
                                    index++;

                                Player = SelectorSystem.SwapCharacter(Player, players[index]);
                                Player.Send(LegacyPackets.CharacterInformation(Player));
                                Player.Send(MsgAction.MapShowPacket(Player));
                                Player.AddStatusEffect(StatusEffect.Frozen);
                                ScreenSystem.Create(Player);
                                ScreenSystem.Update(Player);
                                break;
                            case 10:
                                Player.RemoveStatusEffect(StatusEffect.Frozen);
                                ScreenSystem.Create(Player);
                                Player.AddSpawnProtection();
                                Player.IncrementXp();

                                foreach (var kvp in Player.Skills)
                                    Player.Send(MsgSkill.Create(kvp.Value));
                                foreach (var prof in Player.Profs)
                                    Player.Send(MsgProf.Create(prof.Value));

                                EntityLogic.Recalculate(Player);
                                Player.CurrentHp = Player.MaximumHp;
                                Player.CurrentMp = Player.MaximumMp;
                                GameWorld.Maps[Player.MapId].Enter(Player);

                                Player.Send(MsgAction.Create(Player, (int)Player.PkMode, MsgActionType.ChangePkMode));
                                Player.Send(MsgUpdate.Create(Player, Player.Stamina, MsgUpdateType.Stamina));
                                Player.Send(MsgUpdate.Create(Player, Player.Class, MsgUpdateType.Class));
                                Player.Online = true;

                                if (Player.HasFlag(StatusEffect.SuperMan))
                                {
                                    BuffSystem.Create(Player);
                                    BuffSystem.AddBuff(Player, new Buff(Player, SkillId.Superman, TimeSpan.FromSeconds(10)));
                                }
                                if (Player.HasFlag(StatusEffect.Cyclone))
                                {
                                    BuffSystem.Create(Player);
                                    BuffSystem.AddBuff(Player, new Buff(Player, SkillId.Cyclone, TimeSpan.FromSeconds(10)));
                                }

                                if (Player.PkPoints > 1)
                                {
                                    Player.PkPJob = new Job(TimeSpan.FromSeconds(15), () => Player.PkPoints--);
                                    YiScheduler.Instance.Do(Player.PkPJob);
                                }
                                ScreenSystem.Send(Player,MsgAction.Create(Player, Player.UniqueId, MsgActionType.EntityRemove));
                                ScreenSystem.Send(Player, MsgSpawn.Create(Player));
                                break;
                            case 100:
                                SelectorSystem.CreateNewCharacterFor(Player.AccountId);
                                Player.Disconnect();
                                break;
                        }
                        break;
                    }
                default:
                    {
                        if (ScriptEngine.Scripts.TryGetValue(ScriptType.NpcDialog, out var sc) && !sc.Execute(Player, id, Control))
                            Message.SendTo(Player, $"[{Player.CurrentNpcId}] Npc has no script.", MsgTextType.Talk);
                        break;
                    }
            }
        }

        private static void ProcessStorage()
        {
            var storages = StorageSystem.GetStorageListForId(Player, Player.CurrentNpcId).ToList();
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
                            Player.CurrentStorageId = found.StorageId;
                            Player.Send(MsgAction.Create(Player, 4, MsgActionType.Dialog));
                            YiScheduler.Instance.Do(SchedulerPriority.MediumLow, () => StorageSystem.ShowStock(Player));
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
                            Player.CurrentStorageId = found.StorageId;
                            Player.Send(MsgAction.Create(Player, 4, MsgActionType.Dialog));
                            YiScheduler.Instance.Do(SchedulerPriority.MediumLow, () => StorageSystem.ShowStock(Player));
                            break;
                        }
                    }
                    DisplayStorages(Player, storages);
                    break;
                }
                case 2:
                {
                    if (storages.Count < 1)
                    {
                        StorageSystem.NewStorage(Player, Player.CurrentNpcId);
                        Player.ForceSend(MsgAction.Create(Player, 4, MsgActionType.Dialog), 24);
                        //Player.Send(MsgStorage.Create(Player.CurrentStorageId, MsgStorageAction.List));
                        YiScheduler.Instance.Do(SchedulerPriority.MediumLow, () => StorageSystem.ShowStock(Player));
                        break;
                    }
                    DisplayStorages(Player, storages);
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