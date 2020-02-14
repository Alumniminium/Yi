using System.Runtime.InteropServices;
using Yi.Enums;
using Yi.Items;
using Yi.Network.Sockets;

namespace Yi.Network.Packets.Conquer
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct MsgItemInfoEx
    {
        public readonly ushort Size;//0
        public readonly ushort Id;//2
        public readonly int UniqueId;//4
        public int OwnerUID;//8
        public int Price;//12
        public readonly int ItemId;//16
        public readonly ushort CurrentDurability;//20
        public readonly ushort MaxiumumDurability;//22
        public readonly ItemExType ItemExType;//24
        public readonly byte Ident;//25
        public readonly MsgItemPosition Position;//26
        public readonly int Unknow1;//27
        //public readonly byte Dunno;//31
        public readonly byte Gem1;
        public readonly byte Gem2;
        public readonly RebornItemEffect RebornEffect;
        public readonly byte Magic2;
        public readonly byte Plus;
        public readonly byte Bless;
        public readonly byte Enchant;
        public readonly int Restrain;

        public MsgItemInfoEx(Item item, MsgItemPosition position = MsgItemPosition.Inventory, ItemExType type = ItemExType.None) : this()
        {
            Size = (ushort)sizeof(MsgItemInfoEx);
            Id = 1108;
            OwnerUID = item.OwnerUniqueId;
            UniqueId = item.UniqueId;
            ItemId = item.ItemId;
            Price = item.PriceBaseline;
            CurrentDurability = item.CurrentDurability;
            MaxiumumDurability = item.MaximumDurability;
            ItemExType = type;
            Position = position;
            Gem1 = item.Gem1;
            Gem2 = item.Gem2;
            RebornEffect = item.RebornEffect;
            Plus = item.Plus;
            Bless = item.Bless;
            Enchant = item.Enchant;
            Restrain = item.CustomTextId;
        }

        public static implicit operator byte[] (MsgItemInfoEx msg)
        {
            var buffer = BufferPool.GetBuffer();
            fixed (byte* p = buffer)
                *(MsgItemInfoEx*)p = *&msg;
            return buffer;
        }

        public static MsgItemInfoEx CreateBoothItem(Item item)
        {
            var msg = new MsgItemInfoEx(item, MsgItemPosition.Inventory, ItemExType.Booth)
            {
                OwnerUID = item.Product.ShopId,
                Price = item.Product.Price
            };
            return msg;
        }
    }
}