namespace YiX.Database.Squiggly.Models
{
    public class cq_generator
    {
        public long id { get; set; }
        public ushort mapid { get; set; }
        public ushort bound_x { get; set; }
        public ushort bound_y { get; set; }
        public short bound_cx { get; set; }
        public short bound_cy { get; set; }
        public ushort maxnpc { get; set; }
        public ushort rest_secs { get; set; }
        public ushort max_per_gen { get; set; }
        public int npctype { get; set; }
        public ushort timer_begin { get; set; }
        public ushort timer_end { get; set; }
        public ushort born_x { get; set; }
        public ushort born_y { get; set; }
    }
}
