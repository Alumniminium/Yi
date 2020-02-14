namespace YiX.Database.Squiggly.Models
{
    public class cq_traptype
    {
        public long id { get; set; }
        public byte sort { get; set; }
        public long look { get; set; }
        public long action_id { get; set; }
        public long level { get; set; }
        public long attack_max { get; set; }
        public long attack_min { get; set; }
        public long dexterity { get; set; }
        public long attack_speed { get; set; }
        public long active_times { get; set; }
        public long magic_type { get; set; }
        public long magic_hitrate { get; set; }
        public long size { get; set; }
        public long atk_mode { get; set; }
    }
}
