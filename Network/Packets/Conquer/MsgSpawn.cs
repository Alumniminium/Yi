using System.Runtime.InteropServices;
using YiX.Entities;
using YiX.Enums;
using YiX.Helpers;
using YiX.Network.Sockets;
using YiX.Structures;

namespace YiX.Network.Packets.Conquer
{
    [StructLayout(LayoutKind.Sequential, Pack = 1/*, Size = 101*/)]
    public unsafe struct MsgSpawn
    {
        public ushort Size;
        public ushort Id;
        public int UniqueId;
        public uint Look;
        public StatusEffect StatusEffects;
        public ushort GuildId;
        public readonly byte Unknown1;
        public GuildRanks GuildRank;
        public readonly int Garment;
        public int Head;
        public int Armor;
        public int MainHand;
        public int OffHand;
        public readonly int Unkown2;
        public ushort CurrentHp;
        public ushort Level;
        public ushort X;
        public ushort Y;
        public ushort Hair;
        public Direction Direction;
        public Emote Emote;
        public short Reborn;
        public readonly short Level2;
        public readonly int Unknown4;
        public readonly int NobilityRank;
        public readonly int UniqueId2;
        public readonly int NobilityPosition;
        public byte StringCount;
        public byte NameLen;
        public fixed byte Name [16];

        public static byte[] Create(Player human)
        {
            if (human.HasFlag(StatusEffect.Die))
            {
                if (human.Look % 10000 == 2001 || human.Look % 10000 == 2002)
                    human.AddTransform(99);
                else
                    human.AddTransform(98);
            }

            var packet = stackalloc MsgSpawn[1];
            packet->Size = (ushort) sizeof(MsgSpawn);
            packet->Id = 1014;
            packet->UniqueId = human.UniqueId;
            packet->Look = human.Look;
            packet->StatusEffects = human.StatusEffects;
            packet->GuildRank = human.GuildRank;
            packet->Head = human.GetEquip(MsgItemPosition.Head)?.ItemId ?? 0;
            packet->Armor = human.GetEquip(MsgItemPosition.Armor)?.ItemId ?? 0;
            packet->MainHand = human.GetEquip(MsgItemPosition.RightWeapon)?.ItemId ?? 0;
            packet->OffHand = human.GetEquip(MsgItemPosition.LeftWeapon)?.ItemId ?? 0;
            packet->CurrentHp = (ushort) human.CurrentHp;
            packet->Level = human.Level;
            packet->X = human.Location.X;
            packet->Y = human.Location.Y;
            packet->Hair = (ushort) human.Hair;
            packet->Direction = human.Direction;
            packet->Emote = human.Emote;
            packet->Reborn = (short) (human.Reborn);
            packet->GuildId = (ushort) (human.Guild?.UniqueId ?? 0);
            packet->StringCount = 1;
            if (human is Player player && player.Online )
            {
                packet->NameLen = (byte) human.Name.Length;
                for (byte i = 0; i < human.Name.Length; i++)
                    packet->Name[i] = (byte) human.Name[i];
            }
            else
            {
                packet->NameLen = (byte) human.Name.Size16Offline().Length;
                for (byte i = 0; i < human.Name.Size16Offline().Length; i++)
                    packet->Name[i] = (byte) human.Name.Size16Offline()[i];
            }
            var buffer = BufferPool.GetBuffer();
            fixed (byte* p = buffer)
                *(MsgSpawn*)p = *packet;
            return buffer;
        }

        public static byte[] Create(Monster monster)
        {
            if (!monster.Alive)
                return null;

            var packet = stackalloc MsgSpawn[1];
            packet->Size = (ushort) sizeof(MsgSpawn);
            packet->Id = 1014;
            packet->UniqueId = monster.UniqueId;
            packet->Look = monster.Look;
            packet->StatusEffects = monster.StatusEffects;
            packet->CurrentHp = (ushort) monster.CurrentHp;
            packet->Level = monster.Level;
            packet->Direction = monster.Direction;
            packet->Emote = Emote.Stand;
            packet->StringCount = 1;
            packet->NameLen = (byte) monster.Name.Trim().Length;
            packet->X = monster.Location.X;
            packet->Y = monster.Location.Y;
            for (byte i = 0; i < monster.Name.Trim().Length; i++)
                packet->Name[i] = (byte) monster.Name.Trim()[i];
            var buffer = BufferPool.GetBuffer();
            fixed (byte* p = buffer)
                *(MsgSpawn*)p = *packet;
            return buffer;
        }

        public static byte[] Create(YiObj human, Vector2 point)
        {
            var packet = stackalloc MsgSpawn[1];
            packet->Size = (ushort) sizeof(MsgSpawn);
            packet->Id = 1014;
            packet->UniqueId = human.UniqueId;
            packet->Look = human.Look;
            packet->StatusEffects = human.StatusEffects;
            packet->GuildRank = human.GuildRank;
            packet->GuildId = (ushort) (human.Guild?.UniqueId ?? 0);
            packet->Head = human.GetEquip(MsgItemPosition.Head)?.ItemId ?? 0;
            packet->Armor = human.GetEquip(MsgItemPosition.Armor)?.ItemId ?? 0;
            packet->MainHand = human.GetEquip(MsgItemPosition.RightWeapon)?.ItemId ?? 0;
            packet->OffHand = human.GetEquip(MsgItemPosition.LeftWeapon)?.ItemId ?? 0;
            packet->CurrentHp = (ushort) human.CurrentHp;
            packet->Level = human.Level;
            packet->Hair = (ushort) human.Hair;
            packet->Direction = human.Direction;
            packet->Emote = human.Emote;
            packet->Reborn = (short) (human.Reborn);
            packet->StringCount = 1;
            packet->NameLen = (byte) human.Name.Length;
            packet->X = point.X;
            packet->Y = point.Y;
            for (byte i = 0; i < human.Name.Length; i++)
                packet->Name[i] = (byte) human.Name[i];
            var buffer = BufferPool.GetBuffer();
            fixed (byte* p = buffer)
                *(MsgSpawn*)p = *packet;
            return buffer;
        }

        public static implicit operator byte[](MsgSpawn msg)
        {
            var buffer = BufferPool.GetBuffer();
            fixed (byte* p = buffer)
                *(MsgSpawn*)p = *&msg;
            return buffer;
        }
    }
}