namespace Yi.Structures
{
    public struct Spawn
    {
        public ushort MapId { get; set; }
        public uint MobId { get; set; }
        public ushort Xstart { get; set; }
        public ushort Ystart { get; set; }
        public short Xend { get; set; }
        public short Yend { get; set; }
        public ushort RespawnDelay { get; set; }
        public ushort Amount { get; set; }
    }
}