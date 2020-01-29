namespace YiX.Database.Squiggly.Models
{
    public class cq_monstertype
    {
        public int id { get; set; }
        public string name { get; set; }
        public int type { get; set; }
        public uint lookface { get; set; }
        public ushort life { get; set; }
        public int mana { get; set; }
        public int attack_max { get; set; }
        public int attack_min { get; set; }
        public int defence { get; set; }
        public ushort dexterity { get; set; }
        public int dodge { get; set; }
        public int helmet_type { get; set; }
        public int armor_type { get; set; }
        public int weaponr_type { get; set; }
        public int weaponl_type { get; set; }
        public byte attack_range { get; set; }
        public byte view_range { get; set; }
        public int escape_life { get; set; }
        public int attack_speed { get; set; }
        public int move_speed { get; set; }
        public byte level { get; set; }
        public byte attack_user { get; set; }
        public ushort drop_money { get; set; }
        public short drop_itemtype { get; set; }
        public byte size_add { get; set; }
        public int action { get; set; }
        public byte run_speed { get; set; }
        public byte drop_armet { get; set; }
        public byte drop_necklace { get; set; }
        public byte drop_armor { get; set; }
        public byte drop_ring { get; set; }
        public byte drop_weapon { get; set; }
        public byte drop_shield { get; set; }
        public byte drop_shoes { get; set; }
        public int drop_hp { get; set; }
        public int drop_mp { get; set; }
        public short magic_type { get; set; }
        public int magic_def { get; set; }
        public short magic_hitrate { get; set; }
        public byte ai_type { get; set; }
        public int defence2 { get; set; }
        public byte stc_type { get; set; }
        public byte anti_monster { get; set; }
        public byte extra_battlelev { get; set; }
        public short extra_exp { get; set; }
        public short extra_damage { get; set; }
    }
}
