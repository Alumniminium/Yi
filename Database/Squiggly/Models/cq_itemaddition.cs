namespace YiX.Database.Squiggly.Models
{
    public class cq_itemaddition
    {
        public int id { get; set; }
        public int typeid { get; set; }
        public byte level { get; set; }
        public short life { get; set; }
        public short attack_max { get; set; }
        public short attack_min { get; set; }
        public short defense { get; set; }
        public short magic_atk { get; set; }
        public short magic_def { get; set; }
        public short dexterity { get; set; }
        public short dodge { get; set; }
    }
}
