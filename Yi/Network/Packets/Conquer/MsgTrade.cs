using System;
using System.Runtime.InteropServices;
using Yi.Calculations;
using Yi.Entities;
using Yi.Enums;
using Yi.Helpers;
using Yi.Items;
using Yi.Network.Sockets;
using Yi.Structures;
using Yi.World;
using Player = Yi.Entities.Player;

namespace Yi.Network.Packets.Conquer
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct MsgTrade
    {
        public ushort Size;
        public ushort Id;
        public int UniqueId;
        public TradeMode Mode;

        public static MsgTrade Create(int uniqueId, TradeMode mode)
        {
            var packet = new MsgTrade
            {
                Size = (ushort)sizeof(MsgTrade),
                Id = 1056, UniqueId = uniqueId, Mode = mode,};
            return packet;
        }

        public static void Handle(Player player, byte[] buffer)
        {
            try
            {
                fixed (byte* p = buffer)
                {
                    var packet = *(MsgTrade*) p;
                    BufferPool.RecycleBuffer(buffer);

                    switch (packet.Mode)
                    {
                        case TradeMode.RequestNewTrade:
                            RequestTrade(player, ref packet);
                            break;
                        case TradeMode.RequestCloseTrade:
                            RequestClose(player, ref packet);
                            break;
                        case TradeMode.RequestAddItemToTrade:
                            AddItem(player, ref packet);
                            break;
                        case TradeMode.RequestAddMoneyToTrade:
                            AddMoney(player, ref packet);
                            break;
                        case TradeMode.RequestCompleteTrade:
                            RequestFinish(player, ref packet);
                            break;
                        case TradeMode.ShowTradeWindow:
                            break;
                        case TradeMode.CloseTradeWindowSuccess:
                            break;
                        case TradeMode.CloseTradeWindowFail:
                            break;
                        case TradeMode.DisplayMoney:
                            break;
                        case TradeMode.DisplayMoneyAdd:
                            break;
                        case TradeMode.ReturnItem:
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

        private static void RequestFinish(Player player, ref MsgTrade packet)
        {
            if (packet.UniqueId == 0)
                return;
            if (player.Trade == null)
                return;
            if (player.Trade.Owner.UniqueId == player.UniqueId)
            {
                player.Trade.OwnerOk = true;
                (player.Trade.Partner as Player)?.Send(Create(player.UniqueId, TradeMode.RequestCompleteTrade));
                if (player.Trade.Partner is Npc)
                {
                    player.Trade.PartnerOk = true;
                }
                if (player.Trade.PartnerOk)
                {
                    foreach (var item in player.Trade.PartnerItems)
                    {
                        player.Inventory.AddItem(item.Value);
                        player.Trade.Partner.Inventory.RemoveItem(item.Value);
                    }
                    foreach (var item in player.Trade.OwnerItems)
                    {
                        if (player.Trade.Partner is Npc)
                        {
                            SelfContainedSystems.BoothSystem.Add(player.Trade.Partner, item.Value, item.Value.PriceBaseline);
                        }
                        else
                        {
                            player.Inventory.RemoveItem(item.Value);
                            player.Trade.Partner.Inventory.AddItem(item.Value);
                        }
                    }
                    player.Money += player.Trade.PartnerMoney;
                    player.Trade.Partner.Money -= player.Trade.PartnerMoney;
                    //player.Cps += player.Trade.PartnerCps;
                    //player.Trade.Partner.Cps -= player.Trade.PartnerCps;
                    player.Send(Create(player.Trade.Partner.UniqueId, TradeMode.CloseTradeWindowSuccess));
                    (player.Trade.Partner as Player)?.Send(Create(player.UniqueId, TradeMode.CloseTradeWindowSuccess));
                    player.Trade.Partner.Trade = null;
                    player.Trade = null;
                }
            }
            else
            {
                player.Trade.PartnerOk = true;
                (player.Trade.Owner as Player)?.Send(Create(player.UniqueId, TradeMode.RequestCompleteTrade));
                if (player.Trade.OwnerOk)
                {
                    foreach (var item in player.Trade.OwnerItems)
                    {
                        player.Inventory.AddItem(item.Value);
                        player.Trade.Owner.Inventory.RemoveItem(item.Value);
                    }
                    player.Money += player.Trade.OwnerMoney;
                    player.Trade.Owner.Money -= player.Trade.OwnerMoney;
                    //player.Cps += player.Trade.OwnerCps;
                    //player.Trade.Owner.Cps -= player.Trade.OwnerCps;
                    player.Send(Create(player.Trade.Partner.UniqueId, TradeMode.CloseTradeWindowSuccess));
                    (player.Trade.Owner as Player)?.Send(Create(player.UniqueId, TradeMode.CloseTradeWindowSuccess));
                    player.Trade.Owner.Trade = null;
                    player.Trade = null;
                }
            }
        }

        private static void AddMoney(YiObj player, ref MsgTrade packet)
        {
            if (packet.UniqueId == 0)
                return;
            if (player.Trade == null)
                return;
            if (player.Money < packet.UniqueId)
                return;
            if (player.Trade.Owner.UniqueId == player.UniqueId)
            {
                if (player.Trade.Partner.Money + packet.UniqueId > int.MaxValue)
                    return;
                player.Trade.OwnerMoney += packet.UniqueId;
                (player.Trade.Partner as Player)?.Send(Create(packet.UniqueId, TradeMode.DisplayMoney));
            }
            else
            {
                if (player.Trade.Owner.Money + packet.UniqueId > int.MaxValue)
                    return;
                player.Trade.PartnerMoney += packet.UniqueId;
                (player.Trade.Owner as Player)?.Send(Create(packet.UniqueId, TradeMode.DisplayMoney));
            }
        }

        private static void AddItem(YiObj player, ref MsgTrade packet)
        {
            if (packet.UniqueId == 0)
                return;
            if (player.Trade == null)
                return;

            if (!player.Inventory.Items.TryGetValue(packet.UniqueId, out var found))
                return;
            if (player.Trade.Owner.UniqueId == player.UniqueId)
            {
                if (player.Trade.OwnerItems.Count >= 20)
                    return;
                if (player.Trade.Partner.Inventory.Count + player.Trade.OwnerItems.Count > 40 && !(player.Trade.Partner is Npc))
                    return;
                if (player.Trade.OwnerItems.ContainsKey(found.UniqueId))
                    return;
                player.Trade.OwnerItems.Add(found.UniqueId, found);
                (player.Trade.Partner as Player)?.Send(new MsgItemInformation(found, MsgItemInfoAction.Trade));

                if (player.Trade.Partner is Npc)
                {
                    player.Trade.PartnerMoney += found.PriceBaseline;
                    packet.Mode = TradeMode.DisplayMoney;
                    packet.UniqueId = player.Trade.PartnerMoney;
                    (player as Player)?.ForceSend(packet, packet.Size);

                    (player as Player)?.Send(Create(packet.UniqueId, TradeMode.DisplayMoney));
                }
            }
            else
            {
                if (player.Trade.PartnerItems.Count >= 20)
                    return;
                if (player.Trade.Owner.Inventory.Count + player.Trade.PartnerItems.Count > 40)
                    return;
                if (player.Trade.PartnerItems.ContainsKey(found.UniqueId))
                    return;
                player.Trade.PartnerItems.Add(found.UniqueId, found);
                (player.Trade.Owner as Player)?.Send(new MsgItemInformation(found, MsgItemInfoAction.Trade));
            }
        }

        private static void RequestClose(Player player, ref MsgTrade packet)
        {
            if (packet.UniqueId == 0)
                return;
            if (player.Trade == null)
                return;
            if (player.Trade.Owner.UniqueId == player.UniqueId)
            {
                (player.Trade.Partner as Player)?.Send(Create(player.UniqueId, TradeMode.CloseTradeWindowFail));
                player.Send(Create(player.Trade.Partner.UniqueId, TradeMode.CloseTradeWindowFail));
                player.Trade.Partner.Trade = null;
                player.Trade = null;
            }
            else
            {
                (player.Trade.Owner as Player)?.Send(Create(player.UniqueId, TradeMode.CloseTradeWindowFail));
                player.Send(Create(player.Trade.Owner.UniqueId, TradeMode.CloseTradeWindowFail));
                player.Trade.Owner.Trade = null;
                player.Trade = null;
            }
        }

        private static void RequestTrade(Player player, ref MsgTrade packet)
        {
            if (packet.UniqueId == 0)
                return;
            
            if (!GameWorld.Maps[player.MapId].Entities.TryGetValue(packet.UniqueId, out var found))
                return;
            if (found == null)
                return;
            if (!Position.CanSeeBig(player, found))
                return;

            var target = found;
            player.Trade = target.Trade ?? new Trade(player, target);

            if (target.Trade != null)
            {
                player.Send(Create(target.UniqueId, TradeMode.ShowTradeWindow));
                (target as Player)?.Send(Create(player.UniqueId, TradeMode.ShowTradeWindow));
            }
            else
                (target as Player)?.Send(Create(player.UniqueId, TradeMode.RequestNewTrade));
        }

        public static unsafe implicit operator byte[](MsgTrade msg)
        {
            var buffer = BufferPool.GetBuffer();
            fixed (byte* p = buffer)
                *(MsgTrade*) p = *&msg;
            return buffer;
        }
    }
}