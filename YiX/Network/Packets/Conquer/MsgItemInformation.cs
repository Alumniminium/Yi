using System.Runtime.InteropServices;
using YiX.Entities;
using YiX.Enums;
using YiX.Items;
using YiX.Network.Sockets;

namespace YiX.Network.Packets.Conquer
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct MsgItemInformation
    {
        public readonly ushort Size;
        public readonly ushort Id;
        public readonly int UniqueId;
        public readonly int ItemId;
        public readonly ushort CurrentDurability;
        public readonly ushort MaxiumumDurability;
        public readonly MsgItemInfoAction Action;
        public readonly byte Ident;
        public readonly MsgItemPosition Position;
        public readonly int Unknow1;
        public readonly byte Gem1;
        public readonly byte Gem2;
        public readonly RebornItemEffect RebornEffect;
        public readonly byte Magic2;
        public readonly byte Plus;
        public readonly byte Bless;
        public readonly byte Enchant;
        public readonly int Restrain;

        public MsgItemInformation(Item item, MsgItemPosition position = MsgItemPosition.Inventory) : this()
        {
            Size = (ushort)sizeof(MsgItemInformation);
            Id = 1008;
            UniqueId = item.UniqueId;
            ItemId = item.ItemId;
            CurrentDurability = item.CurrentDurability;
            MaxiumumDurability = item.MaximumDurability;
            Action = MsgItemInfoAction.AddItem;
            Ident = 0;
            Position = position;
            Gem1 = item.Gem1;
            Gem2 = item.Gem2;
            RebornEffect = item.RebornEffect;
            Magic2 = 0;
            Bless = item.Bless;
            Plus = item.Plus;
            Enchant = item.Enchant;
            Restrain = item.CustomTextId;
        }

        public MsgItemInformation(YiObj owner, Item item, MsgItemPosition position, MsgItemInfoAction action = MsgItemInfoAction.OtherPlayerEquipement) : this()
        {
            Size = (ushort)sizeof(MsgItemInformation);
            Id = 1008;
            UniqueId = owner.UniqueId;
            ItemId = item.ItemId;
            CurrentDurability = item.CurrentDurability;
            MaxiumumDurability = item.MaximumDurability;
            Action = action;
            Ident = 0;
            Position = position;
            Gem1 = item.Gem1;
            Gem2 = item.Gem2;
            RebornEffect = item.RebornEffect;
            Magic2 = 0;
            Bless = item.Bless;
            Plus = item.Plus;
            Enchant = item.Enchant;
            Restrain = item.CustomTextId;
        }
        public MsgItemInformation(Item item, MsgItemInfoAction action = MsgItemInfoAction.AddItem, MsgItemPosition position = MsgItemPosition.Inventory) : this()
        {
            Size = (ushort)sizeof(MsgItemInformation);
            Id = 1008;
            UniqueId = item.UniqueId;
            ItemId = item.ItemId;
            CurrentDurability = item.CurrentDurability;
            MaxiumumDurability = item.MaximumDurability;
            Action = action;
            Ident = 0;
            Position = position;
            Gem1 = item.Gem1;
            Gem2 = item.Gem2;
            RebornEffect = item.RebornEffect;
            Magic2 = 0;
            Bless = item.Bless;
            Plus = item.Plus;
            Enchant = item.Enchant;
            Restrain = item.CustomTextId;
        }

        public static implicit operator byte[](MsgItemInformation msg)
        {
            var buffer = BufferPool.GetBuffer();
            fixed (byte* p = buffer)
                *(MsgItemInformation*)p = *&msg;
            return buffer;
        }
    }
}