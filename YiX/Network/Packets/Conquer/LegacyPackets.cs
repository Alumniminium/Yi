using System;
using System.Collections.Concurrent;
using System.Linq;
using YiX.Calculations;
using YiX.Entities;
using YiX.Items;
using YiX.Network.Sockets;

namespace YiX.Network.Packets.Conquer
{
    public static class LegacyPackets
    {
        public static byte[] WarehouseItems(int storageId, ConcurrentDictionary<int, Item> contents)
        {
            using (var yi = new Packet(1102, 16 + contents.Count * 20))
            {
                yi
                    .Int(storageId)
                    .Byte(0)
                    .Byte(10)
                    .Short(0)
                    .Int(contents.Count);

                //var position = 16;
                foreach (var item in contents.OrderByDescending(kvp => kvp.Value.ItemId))
                {
                    yi
                        .Int(item.Key)
                        .Int(item.Value.ItemId)
                        .Byte(0)
                        .Byte(item.Value.Gem1)
                        .Byte(item.Value.Gem2)
                        .Byte((byte)item.Value.RebornEffect)
                        .Byte(0)
                        .Byte(item.Value.Plus)
                        .Byte(item.Value.Bless)
                        .Byte(0)
                        .Short(item.Value.Enchant)
                        .Short((short)item.Value.CustomTextId);
                    //.Goto(position += 20);
                }

                return yi;
            }
        }

        public static unsafe byte[] SpawnCarpet(YiObj character, int id)
        {
            const ushort packetType = 1109;
            var packet = BufferPool.GetBuffer();

            fixed (byte* p = packet)
            {
                *(ushort*)p = (ushort)(28 + character.Name.Length);
                *(ushort*)(p + 2) = packetType;
                *(int*)(p + 4) = id;
                *(ushort*)(p + 16) = (ushort)(character.Location.X + (ushort)Constants.DeltaX[(sbyte)character.Direction]);
                *(ushort*)(p + 18) = (ushort)(character.Location.Y + (ushort)Constants.DeltaY[(sbyte)character.Direction]);
                *(ushort*)(p + 20) = 400; //TODO Figure out what value this is, set proper direction
                *(ushort*)(p + 22) = 14;
                *(p + 24) = 11;
                *(p + 26) = 1;
                *(p + 27) = (byte)character.Name.Length;
                for (var I = 0; I < character.Name.Length; I++)
                {
                    *(p + 28 + I) = Convert.ToByte(character.Name[I]);
                }
            }
            return packet;
        }


        public static byte[] CharacterInformation(Player player)
        {
#if !ERA
            var length = (ushort)(75 + (player.Name?.Length ?? 0) + (player.Partner?.Length ?? 0));
            using (var yi = new Packet(1006, length))
            {
                yi
                    .Int(player.UniqueId)
                    .Int(player.Look) //Was Int
                  .Short((ushort)player.Hair)
                  .Int(player.Money)
                  .Int(0) //cps
                  .Int(player.Experience)
                  .Int(player.Experience)
                  .Int(player.Experience)
                  .Goto(46)
                  .Short(player.Strength)
                  .Short(player.Agility)
                  .Short(player.Vitality)
                  .Short(player.Spirit)
                  .Short(player.Statpoints)
                  .Short((short)player.CurrentHp)
                  .Short(player.CurrentMp)
                  .Short(player.PkPoints)
                  .Byte(player.Level)
                  .Byte(0)
                  .Byte(player.Class)
                  .Goto(65)
                  .Byte(player.Reborn)
                  .Byte(1)
                  .Byte(2)
                  .String(player.Name)
                  .String(player.Partner);

                return yi;
            }
#else
            var length = (ushort)(74 + (player.Name?.Length ?? 0) + (player.Partner?.Length ?? 0));
            using (var yi = new Packet(1006, length))
            {
                yi
                    .Int(player.UniqueId)
                    .Int(1010005) //Look
                    .Short((ushort)player.Hair)
                    .Byte(0) //Length
                    .Byte(0) //Fat
                    .Int(player.Money)
                    .Int(player.Experience)
                    .Skip(11) //Padding
                    .Short(0) //MercenaryExp
                    .Short(0) //MercenaryLvl
                    .Skip(15) //Padding
                    .Short(player.Strength)
                    .Short(player.Agility)
                    .Short(player.Vitality)
                    .Short(player.Spirit)
                    .Short(player.Statpoints)
                    .Short((short)player.CurrentHp)
                    .Short(player.CurrentMp)
                    .Short(player.PkPoints)
                    .Byte(player.Level)
                    .Byte(player.Class)
                    .Byte(1) //AutoAllot
                    .Byte(1) //Rb
                    .Byte(1) //ShowName
                    .Byte(2)
                    .String(player.Name)
                    .String(player.Partner)
                    .FinishPacket();
                return yi;
            }
#endif
        }

        public static byte[] MsgTransfer(int uid, int key, int port)
        {
#if ERA
            using (var yi = new Packet(1052, 32))
            {
                yi
                    .Int(uid)
                    .Int(key)
                    .StringWithoutLenght(YiCore.ServerIp);

                return yi;
            }
#else
            using (var yi = new Packet(1055, 32))
            {
                yi
                    .Int(uid)
                    .Int(key)
                    .StringWithoutLenght(YiCore.ServerIp)
                    .Goto(28)
                    .Short((short)port);

                return yi;
            }
#endif
        }

        public static byte[] Effect(YiObj entity, string name)
        {
            using (var yi = new Packet(1015, 13 + name.Length))
            {
                yi.Short(entity.Location.X).Short(entity.Location.Y).Byte(10) // Should be 10, because it was 10 on the packet's list. Consider checking it.
                  .Byte(1).String(name);

                return yi;
            }
        }

        //public static byte[] MsgSceneEffect(Player obj)
        //{
        //    if (obj == null)
        //        return null;
        //    var effect = (MapEffect)obj;
        //    using (var Yi = new MergedPacket(1101, 20))
        //    {
        //        Yi
        //            .Int(effect.Targets)
        //            .Int(effect.Look)
        //            .Short(effect.X)
        //            .Short(effect.Y)
        //            .Byte(10);

        //        return Yi;
        //    }
        //}

        public static byte[] NpcFace(ushort faceId)
        {
            using (var yi = new Packet(2032, 16))
            {
                yi.Byte(10, 4)
                    .Byte(10, 6)
                    .Short(faceId, 8)
                    .Byte(0xff)
                    .Byte(4);

                return yi;
            }
        }

        public static byte[] NpcFinish()
        {
            using (var yi = new Packet(2032, 16))
            {
                yi.Goto(10)
                    .Byte(255)
                    .Byte(100);

                return yi;
            }
        }

        public static byte[] NpcInputBox(string text, byte pageId)
        {
            using (var yi = new Packet(2032, (ushort)(16 + text.Length)))
            {
                yi.Goto(10)
                    .Byte(pageId)
                    .Byte(3)
                    .Byte(1)
                    .String(text);

                return yi;
            }
        }

        public static byte[] NpcLink(string text, byte pageId)
        {
            using (var yi = new Packet(2032, (ushort)(16 + text.Length)))
            {
                yi.Goto(10)
                    .Byte(pageId)
                    .Byte(2)
                    .Byte(1)
                    .String(text);

                return yi;
            }
        }

        public static byte[] NpcSay(string text)
        {
            using (var yi = new Packet(2032, (ushort)(16 + text.Length)))
            {
                yi.Goto(10)
                    .Byte(0xff)
                    .Byte(1)
                    .Byte(1)
                    .String(text);

                return yi;
            }
        }
    }
}