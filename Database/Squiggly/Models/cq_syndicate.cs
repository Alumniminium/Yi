namespace YiX.Database.Squiggly.Models
{
    public class cq_syndicate
    {
        public long id { get; set; }
        public string name { get; set; }
        public string announce { get; set; }
        public string leader_title { get; set; }
        public string member_title { get; set; }
        public long leader_id { get; set; }
        public string leader_name { get; set; }
        public long money { get; set; }
        public long fealty_syn { get; set; }
        public byte del_flag { get; set; }
        public long amount { get; set; }
        public long enemy0 { get; set; }
        public long enemy1 { get; set; }
        public long enemy2 { get; set; }
        public long enemy3 { get; set; }
        public long enemy4 { get; set; }
        public long ally0 { get; set; }
        public long ally1 { get; set; }
        public long ally2 { get; set; }
        public long ally3 { get; set; }
        public long ally4 { get; set; }
    }
}
