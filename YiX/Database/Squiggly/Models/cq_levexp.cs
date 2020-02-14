using System.ComponentModel.DataAnnotations;

namespace YiX.Database.Squiggly.Models
{
    public class cq_levexp
    {
        [Key]
        public long level { get; set; }
        public ulong exp { get; set; }
        public int up_lev_time { get; set; }
    }
}
