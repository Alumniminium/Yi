using System.Collections.Generic;

namespace YiX.World.Pathfinding
{
    public class PriorityQueue<T>
    {
        private readonly SortedSet<T> _set;

        public PriorityQueue(IComparer<T> comp)
        {
            _set = new SortedSet<T>(comp);
        }

        public bool Empty => Count == 0;

        public int Count => _set.Count;

        public T Top()
        {
            if (_set.Count <= 0) return default(T);
            var result = _set.Min;
            _set.Remove(result);
            return result;
        }

        public T Last()
        {
            if (_set.Count <= 0) return default(T);
            var result = _set.Max;
            _set.Remove(result);
            return result;
        }

        public void Add(T obj) => _set.Add(obj);

        public bool Contains(T obj) => _set.Contains(obj);

        public void Update(T obj)
        {
            _set.Remove(obj);
            _set.Add(obj);
        }

        public void Clear() => _set.Clear();
    }

    public class PathingMaster
    {
        public Map Map;
        public List<Position> Path;

        internal readonly Dictionary<int, PathingNode> Nodes;
        internal readonly PriorityQueue<PathingNode> Priority;
        internal readonly PathingNode Start;
        internal readonly PathingNode Target;

        public PathingMaster(Map map, IPosition startPos, IPosition targetPos) : this(map)
        {
            Start = new PathingNode(startPos.X, startPos.Y);
            Target = new PathingNode(targetPos.X, targetPos.Y);
        }

        public PathingMaster(Map map)
        {
            Priority = new PriorityQueue<PathingNode>(new PathingNode.NodeComparer());
            Nodes = new Dictionary<int, PathingNode>();
            Map = map;
        }

        public static PathingMaster JumpPointSearch(Map map, IPosition startPos, IPosition targetPos) => new PathingMaster(map, startPos, targetPos);

        public PathingNode GetNode(int x, int y)
        {
            if (Map.Width < x || Map.Height < y) return null;
            var id = x | y << 16;

            PathingNode node;
            if (Nodes.TryGetValue(id, out node))
                return node;

            Nodes.Add(id, node = new PathingNode((ushort)x, (ushort)y));
            return node;
        }

        public bool ValidateNodeForPath(int x, int y) => Map.GroundValid((ushort) x, (ushort) y) && Map.MobValid((ushort)x, (ushort)y);

        public bool ValidateNodeForPath(IPosition pos) => ValidateNodeForPath(pos.X, pos.Y);
    }

    public enum PathingType
    {
        Simple,
        JumpPoint
    }
}