namespace YiX.Database.Squiggly.Models
{
    public class cq_task
    {
        public long id { get; set; }
        public long? id_next { get; set; }
        public long? id_nextfail { get; set; }
        public string itemname1 { get; set; }
        public string itemname2 { get; set; }
        public long? money { get; set; }
        public long? profession { get; set; }
        public long? sex { get; set; }
        public int? min_pk { get; set; }
        public int? max_pk { get; set; }
        public long? team { get; set; }
        public long? metempsychosis { get; set; }
        public byte query { get; set; }
        public short marriage { get; set; }
        public byte client_active { get; set; }
    }
}
