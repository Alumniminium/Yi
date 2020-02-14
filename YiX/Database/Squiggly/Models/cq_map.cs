namespace YiX.Database.Squiggly.Models
{
    public class cq_map
    {
        public long id { get; set; }
        public string name { get; set; }
        public string describe_text { get; set; }
        public long mapdoc { get; set; }
        public long type { get; set; }
        public long owner_id { get; set; }
        public long mapgroup { get; set; }
        public int idxserver { get; set; }
        public long weather { get; set; }
        public long bgmusic { get; set; }
        public long bgmusic_show { get; set; }
        public long portal0_x { get; set; }
        public long portal0_y { get; set; }
        public long reborn_map { get; set; }
        public long reborn_portal { get; set; }
        public byte res_lev { get; set; }
        public byte owner_type { get; set; }
        public long link_map { get; set; }
        public int link_x { get; set; }
        public int link_y { get; set; }
        public byte del_flag { get; set; }
        public long color { get; set; }
        public byte[] DMAP { get; set; }
        public ushort Width { get; set; }
        public ushort Height { get; set; }
    }
}
