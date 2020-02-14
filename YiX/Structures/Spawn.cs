namespace YiX.Structures
{
    public struct Spawn
    {
        public ushort MapId { get; set; }
        public int MobId { get; set; }
        public ushort BornY { get; internal set; }
        public ushort BornX { get; internal set; }
        public ushort Xstart { get; set; }
        public ushort Ystart { get; set; }
        public short Xend { get; set; }
        public short Yend { get; set; }
        public ushort Amount { get; set; }
        public ushort MaxAmount { get; internal set; }
        public ushort TimerBegin { get; internal set; }
        public ushort TimerEnd { get; internal set; }
        public ushort RespawnDelay { get; set; }
    }
}