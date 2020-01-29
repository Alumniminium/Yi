namespace YiX.Database.Squiggly.Models
{
    public class cq_dynanpc
    {
        public long id { get; set; }
        public long ownerid { get; set; }
        public byte ownertype { get; set; }
        public string name { get; set; }
        public int type { get; set; }
        public long lookface { get; set; }
        public short idxserver { get; set; }
        public long mapid { get; set; }
        public int cellx { get; set; }
        public int celly { get; set; }
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
        public long life { get; set; }
        public long maxlife { get; set; }
        public long @base { get; set; }
        public int sort { get; set; }
        public long itemid { get; set; }
        public int defence { get; set; }
        public int magic_def { get; set; }
    }
}
