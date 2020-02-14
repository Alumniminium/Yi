using System;
using System.Collections.Generic;

namespace Yi.World.Pathfinding
{
    public class PathingNode : Position
    {
        public float F;
        public float G;
        public float H;
        public bool IsClosed;
        public bool IsOpen;
        public PathingNode Parent;

        public PathingNode(ushort x, ushort y) : base(x, y)
        {
            X = x;
            Y = y;
        }

        public float HDistance(IPosition otherPos)
        {
            var dx = Math.Pow(X - otherPos.X, 2);
            var dy = Math.Pow(Y - otherPos.Y, 2);
            return (float)Math.Abs(dx + dy + (Math.Sqrt(2) - 2) * Math.Min(dx, dy));
        }

        public override IEnumerable<IPosition> GetNeighbors()
        {
            for (var i = -1; i <= 1; i += 1)
            {
                for (var j = -1; j <= 1; j += 1)
                {
                    if (!(i == 0 && j == 0))
                        yield return new PathingNode((ushort)(X + i), (ushort)(Y + j));
                }
            }
        }

        public static List<Position> Backtrace(PathingNode traceNode)
        {
            var path = new List<Position>
            {
                new Position(traceNode)
            };
            while (traceNode.Parent != null)
            {
                traceNode = traceNode.Parent;
                path.Add(new Position(traceNode));
            }
            path.Reverse();
            return path;
        }

        public bool Equals(int x, int y) => X == x && Y == y;

        public bool Equals(IPosition p)
        {
            if (p == null)
                return false;

            return X == p.X && Y == p.Y;
        }

        public class NodeComparer : IComparer<PathingNode>
        {
            public int Compare(PathingNode node1, PathingNode node2) => Math.Sign(node1.F - node2.F);
        }
    }
}