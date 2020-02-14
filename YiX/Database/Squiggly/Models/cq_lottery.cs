namespace YiX.Database.Squiggly.Models
{
    public class cq_lottery
    {
        public long id { get; set; }
        public long type { get; set; }
        public byte rank { get; set; }
        public long chance { get; set; }
        public string prize_name { get; set; }
        public long prize_item { get; set; }
        public byte color { get; set; }
        public byte hole_num { get; set; }
        public byte addition_lev { get; set; }
    }
}
