using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Yi.Entities;
using Yi.Enums;
using Yi.Helpers;
using Yi.Network.Packets.Conquer;
using Yi.Scheduler;
using Yi.SelfContainedSystems;
using Yi.World;

namespace Yi.Items
{
    public class FloorItem : YiObj
    {
        public Item Original { get; set; }
        public int Amount { get; set; }
        public YiObj Owner { get; set; }
        public DateTime DropTime { get; set; }
        [JsonIgnore]
        public List<Job> Jobs { get; set; }
        
        public void Countdown(int number) => ScreenSystem.Send(this, LegacyPackets.Effect(this, "downnumber" + number));

        public void Destroy()
        {
            ScreenSystem.Send(this, MsgFloorItem.Create(this, MsgFloorItemType.Delete));
            FloorItemSystem.FloorItems.TryRemove(UniqueId);
            GameWorld.Maps[MapId].Leave(this);
        }

        public override string ToString() => "[FloorItem] " + UniqueId;
    }
}