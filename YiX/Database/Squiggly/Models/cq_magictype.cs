namespace YiX.Database.Squiggly.Models
{
    public class cq_magictype
    {
        public long id { get; set; }
        public long type { get; set; }
        public long sort { get; set; }
        public string name { get; set; }
        public byte crime { get; set; }
        public byte ground { get; set; }
        public byte multi { get; set; }
        public long target { get; set; }
        public long level { get; set; }
        public long use_mp { get; set; }
        public int power { get; set; }
        public long intone_speed { get; set; }
        public long percent { get; set; }
        public long step_secs { get; set; }
        public long range { get; set; }
        public long distance { get; set; }
        public long status { get; set; }
        public long need_prof { get; set; }
        public int need_exp { get; set; }
        public long need_level { get; set; }
        public byte use_xp { get; set; }
        public long weapon_subtype { get; set; }
        public long active_times { get; set; }
        public byte auto_active { get; set; }
        public long floor_attr { get; set; }
        public byte auto_learn { get; set; }
        public long learn_level { get; set; }
        public byte drop_weapon { get; set; }
        public long use_ep { get; set; }
        public byte weapon_hit { get; set; }
        public long use_item { get; set; }
        public long? next_magic { get; set; }
        public long delay_ms { get; set; }
        public long use_item_num { get; set; }
    }
}
