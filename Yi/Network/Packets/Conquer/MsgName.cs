using System;
using System.Text;
using Yi.Enums;
using Yi.Helpers;
using Yi.Network.Sockets;
using Yi.World;
using Player = Yi.Entities.Player;

namespace Yi.Network.Packets.Conquer
{
    public unsafe struct MsgName
    {
        public ushort Size;
        public ushort Id;
        public int Data;
        public MsgNameType Type;
        public byte Count;
        public fixed byte Params [255];

        public static byte[] Create(int data, string param, byte action)
        {
            if (param == null || param.Length > 255)
                return null;

            var Out = new byte[13 + param.Length];
            fixed (byte* p = Out)
            {
                *(short*)(p + 0) = (short)Out.Length;
                *(short*)(p + 2) = 1015;
                *(int*)(p + 4) = data;
                *(p + 8) = action;
                *(p + 9) = 0x01;
                *(p + 10) = (byte)param.Length;
                for (byte i = 0; i < (byte)param.Length; i++)
                    *(p + 11 + i) = (byte)param[i];
            }
            return Out;
        }

        public static byte[] Create(int data, string[] Params, MsgNameType action)
        {
            if (Params == null || Params.Length < 1)
                return null;

            var strLength = 0;
            for (var i = 0; i < Params.Length; i++)
            {
                if (Params[i] == null || Params[i].Length > 255)
                    return null;

                strLength += Params[i].Length + 1;
            }

            var Out = new byte[12 + strLength];
            fixed (byte* p = Out)
            {
                *(short*)(p + 0) = (short)Out.Length;
                *(short*)(p + 2) = 1015;
                *(int*)(p + 4) = data;
                *(p + 8) = (byte)action;
                *(p + 9) = (byte)Params.Length;

                var pos = 10;
                for (var x = 0; x < Params.Length; x++)
                {
                    *(p + pos) = (byte)Params[x].Length;
                    for (byte i = 0; i < (byte)Params[x].Length; i++)
                        *(p + pos + 1 + i) = (byte)Params[x][i];
                    pos += Params[x].Length + 1;
                }
            }
            return Out;
        }

        public static void Handle(Player player, byte[] buffer)
        {
            try
            {
                fixed (byte* p = buffer)
                {
                    var packet = *(MsgName*) p;
                    BufferPool.RecycleBuffer(buffer);

                    switch (packet.Type)
                    {
                        case MsgNameType.None:
                            break;
                        case MsgNameType.Fireworks:
                            break;
                        case MsgNameType.CreateSyn:
                            break;
                        case MsgNameType.Syndicate:
                            break;
                        case MsgNameType.ChangeTitle:
                            break;
                        case MsgNameType.DelRole:
                            break;
                        case MsgNameType.Spouse:
                            break;
                        case MsgNameType.QueryNpc:
                            break;
                        case MsgNameType.Wanted:
                            break;
                        case MsgNameType.MapEffect:
                            break;
                        case MsgNameType.RoleEffect:
                            break;
                        case MsgNameType.MemberList:
                            if (player.Guild != null)
                            {
                                var list = player.Guild.GetMemberList();
                                if (packet.Data == int.MaxValue)
                                    packet.Data = 0;

                                var amount = list.Length - packet.Data * 10;
                                if (amount > 10)
                                    amount = 10;

                                var tempList = new string[amount];

                                Array.Copy(list, (int) (packet.Data * 10), tempList, 0, (int) amount);

                                player.Send(Create((int) (packet.Data + 1), tempList, MsgNameType.MemberList));
                            }
                            break;
                        case MsgNameType.KickOutSynMem:
                            break;
                        case MsgNameType.QueryWanted:
                            break;
                        case MsgNameType.QueryPoliceWanted:
                            break;
                        case MsgNameType.PoliceWanted:
                            break;
                        case MsgNameType.QuerySpouse: //View equips?
                        {
                            //foreach (var item in player.Equipment.Items)
                            //{
                            //    var ItemInfo = new MsgItemInfoEx(item.Value, item.Key, ItemExType.OtherPlayer_Equipement);
                            //    player.ForceSend(ItemInfo, ItemInfo.Size);
                            //}
                            player.Send(Create(packet.Data + 1, player.Partner, (int) MsgNameType.QuerySpouse));
                        }
                            break;
                        case MsgNameType.AddDicePlayer:
                            break;
                        case MsgNameType.DelDicePlayer:
                            break;
                        case MsgNameType.DiceBonus:
                            break;
                        case MsgNameType.Sound:
                            break;
                        case MsgNameType.SynEnemie:
                            break;
                        case MsgNameType.SynAlly:
                            break;
                        case MsgNameType.Bavarder:
                            var data = Encoding.UTF8.GetString(packet.Params, 255).Trim('\0').Trim('\u0006').Split(new[] {'\t'}, StringSplitOptions.RemoveEmptyEntries);

                            if (data.Length > 0)
                            {
                                if (GameWorld.Find(data[0], out Player found))
                                {
                                    var synName = "No~Guild";
                                    if (found.Guild != null)
                                        synName = found.Guild.Name;

                                    var targetInfo =
                                        found.UniqueId + " " +
                                        found.Level + " " +
                                        0 /*found.Potency*/ + " #" +
                                        synName + " #" +
                                        "YiOnline" + " " +
                                        found.Partner + " " +
                                        0 /*found.Nobility.Rank*/ + " " +
                                        (found.Look % 1000 == 3 ? 1 : 0);

                                    player.Send(Create(0, new[] {found.Name.Trim('\0'), targetInfo}, MsgNameType.Bavarder));
                                }
                            }
                            break;
                        default:
                            Output.WriteLine(((byte[]) packet).HexDump());
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Output.WriteLine(e);
            }
        }

        public static implicit operator byte[](MsgName msg)
        {
            var buffer = BufferPool.GetBuffer();
            fixed (byte* p = buffer)
                *(MsgName*)p = *&msg;
            return buffer;
        }
    }
}