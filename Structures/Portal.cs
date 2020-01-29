namespace YiX.Structures
{
    public class Portal
    {
        public ushort Y { get; set; }
        public ushort X { get; set; }
        public ushort MapId { get; set; }
        public int Id { get; internal set; }
        public long IdX { get; internal set; }
    }
}