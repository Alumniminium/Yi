using System;
using System.Collections.Generic;
using YiX.Entities;
using YiX.Enums;
using YiX.Helpers;
using YiX.Network.Packets.Conquer;
using YiX.Network.Packets.Remote;

namespace YiX.Network
{
    public static class PacketHandler
    {
        public static readonly Dictionary<PacketType, Action<Player, byte[]>> PacketHandlers = new Dictionary<PacketType, Action<Player, byte[]>>
        {
            [PacketType.MsgRemoteLogin] = (a, p) => MsgRemoteLogin.Handle(a, p),
            [PacketType.MsgRemoteText] = (a, p) => MsgRemoteText.Handle(a, p),
            [PacketType.MsgRemoteMaintenance] = (a, p) => MsgRemoteMaintenance.Handle(a, p),

            [PacketType.MsgConnect] = (a, p) => MsgConnect.Handle(a, p),
            [PacketType.MsgLogin] = (a, p) => MsgLogin.Handle(a, p),
            [PacketType.MsgRole] = (a, p) => MsgRole.Handle(a, p),
            [PacketType.MsgTick] = (a, p) => MsgTick.Handle(a, p),
            [PacketType.MsgAction] = (a, p) => MsgAction.Handle(a, p),
            [PacketType.MsgInteraction] = (a, p) => MsgInteract.Handle(a, p),
            [PacketType.MsgForge] = (a, p) => MsgForge.Handle(a, p),
            [PacketType.MsgSockEm] = (a, p) => MsgGemSocket.Handle(a, p),
            [PacketType.MsgCompose] = (a, p) => MsgCompose.Handle(a, p),
            [PacketType.MsgName] = (a, p) => MsgName.Handle(a, p),
            [PacketType.MsgFloor] = (a, p) => MsgFloorItem.Handle(a, p),
            [PacketType.MsgTrade] = (a, p) => MsgTrade.Handle(a, p),
            [PacketType.MsgText] = (a, p) => MsgText.Handle(a, p),
            [PacketType.MsgWalk] = (a, p) => MsgWalk.Handle(a, p),
            [PacketType.MsgItem] = (a, p) => MsgItem.Handle(a, p),
            [PacketType.MsgAction] = (a, p) => MsgAction.Handle(a, p),
            [PacketType.MsgTeam] = (a, p) => MsgTeam.Handle(a, p),
            [PacketType.MsgDialog] = (a, p) => MsgDialog.Handle(a, p),
            [PacketType.MsgDialog2] = (a, p) => MsgDialog.HandleContinuation(a, p),
            [PacketType.MsgSyndicate] = (a, p) => MsgSyndicate.Handle(a, p),
            [PacketType.MsgSyndicateInfo] = (a,p)=> MsgSynMemberInfo.Handle(a,p),
            [PacketType.MsgStorage] = (a, p) => MsgStorage.Handle(a, p),
            [PacketType.MsgNpcSpawn] = (a,p)=> MsgNpcSpawn.Handle(a,p),
            [PacketType.MsgFriend] = (a,p) => MsgFriend.Handle(a,p),
        };

        public static void Handle(Player player, byte[] packet, PacketType packetType)
        {
            if (Enum.IsDefined(typeof (PacketType), packetType))
                PacketHandlers[packetType].Invoke(player, packet);
            else
            {
                Output.WriteLine("Undefined PacketId: " + BitConverter.ToInt16(packet, 2));
                Output.WriteLine(packet.HexDump());
            }
        }        
    }
}