using YiX.World.Pathfinding;

namespace YiX.Structures
{
    public class Vector2
    {
        public ushort X { get; set; }
        public ushort Y { get; set; }

        public Vector2(ushort x, ushort y)
        {
            X = x;
            Y = y;
        }

        public IPosition ToPostion() => new Position(X, Y);
    }
}