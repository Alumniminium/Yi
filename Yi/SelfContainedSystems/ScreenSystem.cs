using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Yi.Calculations;
using Yi.Entities;
using Yi.Enums;
using Yi.Helpers;
using Yi.Items;
using Yi.Network.Packets.Conquer;
using Yi.Network.Sockets;
using Yi.World;

namespace Yi.SelfContainedSystems
{
    public static class ScreenSystem
    {
        public static readonly Dictionary<int, SafeList<int>> Entities = new Dictionary<int, SafeList<int>>();

        public static void Create(YiObj obj)
        {
            Entities.AddOrUpdate(obj.UniqueId, new SafeList<int>());
        }

        public static void Update(YiObj obj)
        {
            try
            {
                if (!Entities.ContainsKey(obj.UniqueId))
                    Create(obj);
                foreach (var u in Entities[obj.UniqueId].Where(u => GameWorld.Maps[obj.MapId].Entities.ContainsKey(u) && !Position.CanSee(obj, GameWorld.Maps[obj.MapId].Entities[u])))
                    Remove(obj, GameWorld.Maps[obj.MapId].Entities[u]);
                foreach (var kvp in GameWorld.Maps[obj.MapId].Entities.Where(kvp => Position.CanSee(kvp.Value, obj)))
                    Add(obj, kvp.Value);
            }
            catch (Exception e)
            {
                Output.WriteLine(e);
            }
        }

        public static void Add(YiObj obj, YiObj target)
        {
            if (!Entities.ContainsKey(obj.UniqueId))
                Create(obj);
            if (target.UniqueId == obj.UniqueId || Entities[obj.UniqueId].Contains(target.UniqueId))
                return;
            

            Entities[obj.UniqueId].Add(target.UniqueId);
            Add(target, obj);

            switch (target)
            {
                case Bot botTarget:
                    //(obj as Player)?.Send(MsgSpawn.Create(botTarget));
                    break;

                case Player playerTarget:
                    (obj as Player)?.Send(MsgSpawn.Create(playerTarget));
                    break;

                case Monster monsterTarget:
                    monsterTarget.Brain.PlayersInSight++;
                    (obj as Player)?.Send(MsgSpawn.Create(monsterTarget));
                    break;

                case Npc npcTarget:
                    (obj as Player)?.Send(MsgNpcSpawn.Create(npcTarget));
                    if (npcTarget.BoothId != 0)
                        (obj as Player)?.Send(LegacyPackets.SpawnCarpet(npcTarget, npcTarget.BoothId));
                    break;

                case DynamicNpc dynNpcTarget:
                    (obj as Player)?.Send(MsgDynNpcSpawn.Create(dynNpcTarget));
                    break;

                case FloorItem floorItemTarget:
                    (obj as Player)?.Send(MsgFloorItem.Create(floorItemTarget, MsgFloorItemType.Create));
                    break;

                case Portal portal:
                    break;

                case null:
                    Debugger.Break();
                    break;

                default:
                    Debugger.Break();
                    break;

            }
        }

        public static void Remove(YiObj obj, YiObj target)
        {
            if (!Entities.ContainsKey(obj.UniqueId))
                Create(obj);
            if (Entities[obj.UniqueId].Remove(target.UniqueId))
                Remove(target, obj);

            if(target is Player && obj is Monster monster)
                monster.Brain.PlayersInSight--;
        }

        public static bool SendJump(YiObj obj, ref MsgAction packet)
        {
            try
            {
                if (!Entities.ContainsKey(obj.UniqueId))
                    Create(obj);
                (obj as Player)?.ForceSend(packet, packet.Size);
                foreach (var o in Entities[obj.UniqueId].Where(o => !Position.CanSee(obj, GameWorld.Maps[obj.MapId].Entities[o])))
                {
                    Remove(obj, GameWorld.Maps[obj.MapId].Entities[o]);
                }
                var ret = false;
                foreach (var o in GameWorld.Maps[obj.MapId].Entities.Values.Where(o => obj.UniqueId != o.UniqueId).Where(o => !Entities[obj.UniqueId].Contains(o.UniqueId)))
                {
                    if (Position.CanSee(obj, o) && !(o is Player))
                        Add(obj, o);
                    if (o is Player player)
                    {
                        if (Position.GetDistance(o.Location.X, o.Location.Y, (ushort)packet.Param, (ushort)(packet.Param >> 16)) >= 18)
                            continue;

                        Add(player, obj);
                        Entities[obj.UniqueId].Add(player.UniqueId);

                        var point = Position.GetBorderCoords((ushort)packet.Param, (ushort)(packet.Param >> 16), player.Location.X, player.Location.Y);

                        player.Send(MsgSpawn.Create(obj, point));
                        player.Send(packet);
                        ret = true;
                    }
                }
                return ret;
            }
            catch (Exception e)
            {
                Output.WriteLine(e);
                return false;
            }
        }

        public static IEnumerable<YiObj> GetEntities(YiObj obj)
        {
            if (!Entities.ContainsKey(obj.UniqueId))
                Create(obj);
            
            if (GameWorld.Maps.ContainsKey(obj.MapId) && !GameWorld.Maps[obj.MapId].Entities.ContainsKey(obj.UniqueId))
                GameWorld.Maps[obj.MapId].Entities.AddOrUpdate(obj.UniqueId, obj);

            foreach (var item in Entities[obj.UniqueId])
            {
                if (GameWorld.Maps.ContainsKey(obj.MapId))
                {
                    if (GameWorld.Maps[obj.MapId].Entities.ContainsKey(item))
                    {
                        yield return GameWorld.Maps[obj.MapId].Entities[item];
                    }
                }
            }
        }

        public static void Send(YiObj obj, byte[] packet, bool includeSelf = false, bool bypassQueue = false, int size = 0)
        {
            try
            {
                if (!Entities.ContainsKey(obj.UniqueId))
                    Create(obj);
                if (packet == null)
                    return;
                foreach (var o in Entities[obj.UniqueId])
                {
                    if (!GameWorld.Maps[obj.MapId].Entities.TryGetValue(o, out var found))
                        continue;

                    if (found is Player player && player.Online)
                    {
                        if (found.UniqueId == obj.UniqueId)
                            continue;

                        if (bypassQueue)
                            player.ForceSend(BufferPool.Clone(packet), size);
                        else
                            player.Send(BufferPool.Clone(packet));
                    }
                }
                if (includeSelf && obj is Player self)
                {
                    if (bypassQueue)
                        self.ForceSend(BufferPool.Clone(packet), size);
                    else
                        self.Send(BufferPool.Clone(packet));
                }
                BufferPool.RecycleBuffer(packet);
            }
            catch (Exception e)
            {
                Output.WriteLine(e);
            }
        }

        public static void SendSpawn(YiObj obj)
        {
            try
            {
                if (obj is Monster monster)
                {
                    if (!Entities.ContainsKey(obj.UniqueId))
                        Create(obj);
                    foreach (var uid in Entities[obj.UniqueId])
                    {
                        if (!GameWorld.Maps[obj.MapId].Entities.TryGetValue(uid, out var found))
                            continue;

                        if (found is Player player)
                        {
                            player.Send(MsgSpawn.Create(monster));
                            player.Send(MsgAction.SpawnEffect(monster));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Output.WriteLine(e);
            }
        }

        public static void ClearFor(YiObj obj)
        {
            try
            {
                if (!Entities.ContainsKey(obj.UniqueId))
                    Create(obj);
                foreach (var o in Entities[obj.UniqueId])
                {
                    if (GameWorld.Maps[obj.MapId].Entities.TryGetValue(o, out var found))
                        Remove(found, obj);
                }
            }
            catch (Exception e)
            {
                Output.WriteLine(e);
            }
        }
    }
}