using System.Collections.Generic;
using System.Linq;
using Yi.Entities;
using Yi.Enums;
using Yi.Helpers;
using Yi.Network.Packets.Conquer;
using Yi.Scheduler;
using Yi.Scripting;
using Yi.SelfContainedSystems;
using Yi.Structures;

namespace DialogScripts.Dialogs.Common
{
    [Script(Id = int.MaxValue)]
    public class StorageSystemScript
    {
        public static bool Execute(Player player, int npcId, byte control, string input)
        {
            //Debugger.Break();
            try
            {
                var storages = StorageSystem.GetStorageListForId(player, player.CurrentNpcId).ToList();
                switch (control)
                {
                    case 0:
                    {
                        if (string.IsNullOrEmpty(input))
                            goto case 2;

                        uint storageId;
                        if (uint.TryParse(input, out storageId))
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
                        uint storageId;
                        if (uint.TryParse(input, out storageId))
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
                        if (storages.Count < 2)
                        {
                            StorageSystem.NewStorage(player, player.CurrentNpcId);
                            player.ForceSend(MsgAction.Create(player, 4, MsgActionType.Dialog), 24);
                            //player.Send(MsgStorage.Create(player.CurrentStorageId, MsgStorageAction.List));
                            YiScheduler.Instance.Do(SchedulerPriority.MediumLow, () => StorageSystem.ShowStock(player));
                            break;
                        }
                        DisplayStorages(player, storages);
                        break;
                    }
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        private static void DisplayStorages(Player player, List<Storage> storages)
        {
            using (var dialog = new NpcDialog(10))
            {
                dialog.Text("Stor UniqueId |             Storage Name             |      Owner       |       Content Access\n");
                foreach (var storage in storages.TakeUpTo(10).OrderBy(s => s.StorageId))
                    dialog.Text($"{storage.ToString(player)}\n");

                dialog.Input("Storage ID", 1);
                dialog.Finish();
                player.Send(dialog);
            }
        }
    }
}