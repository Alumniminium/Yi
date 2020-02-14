using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Yi.Calculations;
using Yi.Entities;
using Yi.Enums;
using Yi.Helpers;
using Yi.Items;
using Yi.Network.Packets.Conquer;
using Yi.SelfContainedSystems;

namespace Yi.Structures
{
    [Serializable]
    [DataContract]
    public class Equipment
    {
        [JsonIgnore]
        private YiObj _owner;
        [DataMember]
        public ConcurrentDictionary<MsgItemPosition, Item> Items;
        public Equipment(YiObj owner)
        {
            _owner = owner;
            Items=new ConcurrentDictionary<MsgItemPosition, Item>();
        }

        public void SetOwner(YiObj owner) => _owner = owner;
        public unsafe bool Equip(Item item, MsgItemPosition position)
        {
            if (!_owner.Inventory.TryRemove(item.UniqueId, out item))
                return false;

            if(item.ItemId/ 10000 == 105)
                position= MsgItemPosition.LeftWeapon;

            (_owner as Player)?.Send(MsgItem.Create(item.UniqueId, 0,0, MsgItemType.RemoveInventory));
            if (Unequip(position))
            {
                (_owner as Player)?.ForceSend(new MsgItemInformation(item, position), (ushort)sizeof(MsgItemInformation));
                AddOrUpdate(position, item);
                ScreenSystem.Send(_owner, MsgSpawn.Create(_owner as Player));
                EntityLogic.Recalculate(_owner);
            }
            return true;
        }
        public unsafe bool Unequip(MsgItemPosition position)
        {
            if (_owner.Inventory.Count == 40)
                return false;

            if (TryRemove(position, out var item))
            {
                _owner.Inventory.AddOrUpdate(item.UniqueId, item);
                (_owner as Player)?.ForceSend(new MsgItemInformation(item, MsgItemPosition.Inventory), (ushort) sizeof(MsgItemInformation));
                ScreenSystem.Send(_owner, MsgSpawn.Create(_owner as Player));
                EntityLogic.Recalculate(_owner);
            }
            return true;
        }
        public IEnumerable<Item> Values => Items.Values;

        public IEnumerator<KeyValuePair<MsgItemPosition, Item>> GetEnumerator() => Items.GetEnumerator();

        public bool TryGetValue(MsgItemPosition position, out Item found) => Items.TryGetValue(position, out found);

        public void AddOrUpdate(MsgItemPosition position, Item item) => Items.AddOrUpdate(position, item);

        public bool TryRemove(MsgItemPosition position, out Item item) => Items.TryRemove(position, out item);

        public void RemoveDura(MsgItemPosition position, bool arrows = false)
        {
            if (!arrows && !YiCore.Success(25))
                return;

            if (!Items.TryGetValue(position, out var found))
                return;
            if (found.CurrentDurability > 0)
                found.CurrentDurability -= 1;

            if(found.CurrentDurability < 10 && !arrows)
                Message.SendTo(_owner, "Your " + Enum.GetName(typeof(MsgItemPosition), position) + " is about to break. ("+found.CurrentDurability+" uses left)", MsgTextType.Center);

            var packet = new MsgItemInformation(found, MsgItemInfoAction.Update);
            (_owner as Player)?.ForceSend(packet,packet.Size);
            
            if (found.CurrentDurability == 0)
            {
                if (position == MsgItemPosition.RightWeapon)
                {
                    Unequip(MsgItemPosition.LeftWeapon);
                    (_owner as Player)?.ForceSend(MsgItem.Create(found.UniqueId, 0, (int)MsgItemPosition.LeftWeapon, MsgItemType.RemoveEquipment), 24);
                }

                Unequip(position);
                (_owner as Player)?.ForceSend(MsgItem.Create(found.UniqueId, 0, (int)position, MsgItemType.RemoveEquipment),24);
                (_owner as Player)?.Inventory.RemoveItem(found);
                if (!arrows)
                Message.SendTo(_owner, "Your "+Enum.GetName(typeof(MsgItemPosition),position)+" broke.", MsgTextType.Center);
            }
        }

        public void AddDura(MsgItemPosition position)
        {
            if (!Items.TryGetValue(position, out var found))
                return;

            if (found.CurrentDurability < found.MaximumDurability)
                found.CurrentDurability += 1;
            else
                found.CurrentDurability = found.MaximumDurability;
            var packet = new MsgItemInformation(found, MsgItemInfoAction.Update);
            (_owner as Player)?.ForceSend(packet, packet.Size);
        }
    }
}
