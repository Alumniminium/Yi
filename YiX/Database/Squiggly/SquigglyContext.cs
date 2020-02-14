using Microsoft.EntityFrameworkCore;
using YiX.Database.Squiggly.Models;

namespace YiX.Database.Squiggly
{
    public class SquigglyContext : DbContext
    {
        public DbSet<cq_action> cq_action { get; set; }
        public DbSet<cq_bonus> cq_bonus { get; set; }
        public DbSet<cq_deluser> cq_deluser { get; set; }
        public DbSet<cq_disdain> cq_disdain { get; set; }
        public DbSet<cq_dynamap> cq_dynamap { get; set; }
        public DbSet<cq_dynanpc> cq_dynanpc { get; set; }
        public DbSet<cq_fuse> cq_fuse { get; set; }
        public DbSet<cq_generator> cq_generator { get; set; }
        public DbSet<cq_goods> cq_goods { get; set; }
        public DbSet<cq_itemaddition> cq_itemaddition { get; set; }
        public DbSet<cq_levexp> cq_levexp { get; set; }
        public DbSet<cq_lottery> cq_lottery { get; set; }
        public DbSet<cq_magictype> cq_magictype { get; set; }
        public DbSet<cq_map> cq_map { get; set; }
        public DbSet<cq_monstertype> cq_monstertype { get; set; }
        public DbSet<cq_npc> cq_npc { get; set; }
        public DbSet<cq_passway> cq_passway { get; set; }
        public DbSet<cq_point_allot> cq_point_allot { get; set; }
        public DbSet<cq_portal> cq_portal { get; set; }
        public DbSet<cq_rebirth> cq_rebirth { get; set; }
        public DbSet<cq_region> cq_bocq_regionnus { get; set; }
        public DbSet<cq_superman> cq_superman { get; set; }
        public DbSet<cq_synattr> cq_synattr { get; set; }
        public DbSet<cq_syndicate> cq_syndicate { get; set; }
        public DbSet<cq_task> cq_task { get; set; }
        public DbSet<cq_trap> cq_trap { get; set; }
        public DbSet<cq_traptype> cq_traptype { get; set; }
        public DbSet<cq_wanted> cq_wanted { get; set; }
        public DbSet<Dmap_Portals> Dmap_Portals { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder oB) => oB.UseSqlite("Data Source=YiSquiggly.db");
    }
}