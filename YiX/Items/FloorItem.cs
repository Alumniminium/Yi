using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using YiX.Entities;
using YiX.Enums;
using YiX.Helpers;
using YiX.Network.Packets.Conquer;
using YiX.Scheduler;
using YiX.SelfContainedSystems;
using YiX.World;

namespace YiX.Items
{
    public class FloorItem : YiObj
    {
        public Item Original { get; set; }
        public int Amount { get; set; }
        [JsonIgnore]
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