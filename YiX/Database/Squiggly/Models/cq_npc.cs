namespace YiX.Database.Squiggly.Models
{
    public class cq_npc
    {
        public long id { get; set; }
        public long? ownerid { get; set; }
        public long? playerid { get; set; }
        public string name { get; set; }
        public byte type { get; set; }
        public uint lookface { get; set; }
        public int? idxserver { get; set; }
        public ushort mapid { get; set; }
        public ushort cellx { get; set; }
        public ushort celly { get; set; }
        public long task0 { get; set; }
        public long task1 { get; set; }
        public long task2 { get; set; }
        public long task3 { get; set; }
        public long task4 { get; set; }
        public long task5 { get; set; }
        public long task6 { get; set; }
        public long task7 { get; set; }
        public int data0 { get; set; }
        public int data1 { get; set; }
        public int data2 { get; set; }
        public int data3 { get; set; }
        public string datastr { get; set; }
        public long linkid { get; set; }
        public int life { get; set; }
        public int maxlife { get; set; }
        public byte @base { get; set; }
        public byte sort { get; set; }
        public long? itemid { get; set; }
    }
}
