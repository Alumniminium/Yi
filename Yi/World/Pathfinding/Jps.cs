using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Yi.Structures;

namespace Yi.World.Pathfinding
{
    public static class JpsAgent
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static List<Vector2> FindPath(PathingMaster args)
        {
            var priority = args.Priority;
            var startNode = args.Start;
            var endNode = args.Target;

            if (startNode == endNode)
                return null;

            if (!args.ValidateNodeForPath(endNode))
                return null;

            startNode.G = 0;
            startNode.H = startNode.HDistance(endNode);
            startNode.F = startNode.H;

            priority.Add(startNode);
            for (var attempts = 0; !priority.Empty && attempts < 10; attempts++)
            {
                var jumpNode = priority.Top();
                if (jumpNode.Equals(endNode))
                    return ExpandPath(PathingNode.Backtrace(jumpNode));

                IdentifySuccessors(args, jumpNode);
            }
            return null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static List<Vector2> ExpandPath(IReadOnlyList<Position> jumpPath)
        {
            var consecutivePath = new List<Vector2>();

            for (var routeTrav = 0; routeTrav < jumpPath.Count - 1; routeTrav++)
            {
                var fromPos = jumpPath[routeTrav];
                var toPos = jumpPath[routeTrav + 1];
                if (fromPos == toPos)
                    continue;

                consecutivePath.AddRange(fromPos.Line(toPos));
            }
            return consecutivePath;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void IdentifySuccessors(PathingMaster args, PathingNode activeNode)
        {
            activeNode.IsClosed = true;
            activeNode.IsOpen = false;

            var priority = args.Priority;

            var neighbors = FindNeighbors(args, activeNode);
            foreach (var neighbor in neighbors)
            {
                int jX = 0, jY = 0;
                if (!Jump(args, neighbor.X, neighbor.Y, activeNode.X, activeNode.Y, ref jX, ref jY))
                    continue;

                var jumpNode = args.GetNode(jX, jY);
                if (jumpNode.IsClosed)
                    continue;

                var newG = activeNode.G + jumpNode.EuclideanDistance(activeNode);
                if ((jumpNode.IsOpen || jumpNode.IsClosed) && newG >= jumpNode.G)
                    continue;

                jumpNode.G = newG;
                jumpNode.H = jumpNode.HDistance(args.Target);
                jumpNode.F = jumpNode.H + jumpNode.G;
                jumpNode.Parent = activeNode;

                if (jumpNode.IsOpen || jumpNode.IsClosed)
                    priority.Update(jumpNode);
                else
                    priority.Add(jumpNode);

                jumpNode.IsOpen = true;
                jumpNode.IsClosed = false;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool Jump(PathingMaster args, int x, int y, int pX, int pY, ref int jX, ref int jY)
        {
            if (!args.ValidateNodeForPath(x, y))
                return false;

            if (args.Target.Equals(x, y))
            {
                jX = x;
                jY = y;
                return true;
            }

            var dx = x - pX;
            var dy = y - pY;

            if (dx != 0 && dy != 0)
            {
                if (args.ValidateNodeForPath(x - dx, y + dy) && !args.ValidateNodeForPath(x - dx, y) || args.ValidateNodeForPath(x + dx, y - dy) && !args.ValidateNodeForPath(x, y - dy))
                {
                    jX = x;
                    jY = y;
                    return true;
                }
            }
            else if (dx != 0)
            {
                if (args.ValidateNodeForPath(x + dx, y + 1) && !args.ValidateNodeForPath(x, y + 1) || args.ValidateNodeForPath(x + dx, y - 1) && !args.ValidateNodeForPath(x, y - 1))
                {
                    jX = x;
                    jY = y;
                    return true;
                }
            }
            else if (args.ValidateNodeForPath(x + 1, y + dy) && !args.ValidateNodeForPath(x + 1, y) || args.ValidateNodeForPath(x - 1, y + dy) && !args.ValidateNodeForPath(x - 1, y))
            {
                jX = x;
                jY = y;
                return true;
            }

            if (dx != 0 && dy != 0)
            {
                //jump x and y for diagonal neighbors
                var jx = Jump(args, x + dx, y, x, y, ref jX, ref jY);
                var jy = Jump(args, x, y + dy, x, y, ref jX, ref jY);
                if (jx || jy)
                {
                    jX = x;
                    jY = y;
                    return true;
                }
            }

            if (args.ValidateNodeForPath(x + dx, y) || args.ValidateNodeForPath(x, y + dy))
                return Jump(args, x + dx, y + dy, x, y, ref jX, ref jY);

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static IEnumerable<Position> FindNeighbors(PathingMaster args, PathingNode jumpNode)
        {
            var parent = jumpNode.Parent;
            var x = jumpNode.X;
            var y = jumpNode.Y;
            var neighbors = new List<Position>();

            if (parent != null)
            {
                var px = parent.X;
                var py = parent.Y;

                var dx = (x - px) / Math.Max(Math.Abs(x - px), 1);
                var dy = (y - py) / Math.Max(Math.Abs(y - py), 1);

                if (dx != 0 && dy != 0)
                {
                    if (args.ValidateNodeForPath(x, y + dy))
                        neighbors.Add(new Position(x, y + dy));

                    if (args.ValidateNodeForPath(x + dx, y))
                        neighbors.Add(new Position(x + dx, y));

                    if (args.ValidateNodeForPath(x + dx, y + dy) && (args.ValidateNodeForPath(x, y + dy) || args.ValidateNodeForPath(x + dx, y)))
                        neighbors.Add(new Position(x + dx, y + dy));

                    if (args.ValidateNodeForPath(x - dx, y + dy) && args.ValidateNodeForPath(x, y + dy) && !args.ValidateNodeForPath(x - dx, y))
                        neighbors.Add(new Position(x - dx, y + dy));

                    if (!args.ValidateNodeForPath(x + dx, y - dy))
                        return neighbors;

                    if (args.ValidateNodeForPath(x + dx, y) && !args.ValidateNodeForPath(x, y - dy))
                        neighbors.Add(new Position(x + dx, y - dy));
                }

                else if (dx == 0)
                {
                    if (!args.ValidateNodeForPath(x, y + dy))
                        return neighbors;

                    neighbors.Add(new Position(x, y + dy));

                    if (args.ValidateNodeForPath(x + 1, y + dy) && !args.ValidateNodeForPath(x + 1, y))
                        neighbors.Add(new Position(x + 1, y + dy));

                    if (args.ValidateNodeForPath(x - 1, y + dy) && !args.ValidateNodeForPath(x - 1, y))
                        neighbors.Add(new Position(x - 1, y + dy));
                }
                else
                {
                    if (!args.ValidateNodeForPath(x + dx, y))
                        return neighbors;

                    neighbors.Add(new Position(x + dx, y));

                    if (args.ValidateNodeForPath(x + dx, y + 1) && !args.ValidateNodeForPath(x, y + 1))
                        neighbors.Add(new Position(x + dx, y + 1));

                    if (args.ValidateNodeForPath(x + dx, y - 1) && !args.ValidateNodeForPath(x, y - 1))
                        neighbors.Add(new Position(x + dx, y - 1));
                }
            }

            else
            {
                neighbors.AddRange(from neighbor in jumpNode.GetNeighbors() where args.ValidateNodeForPath(neighbor) select new Position(neighbor.X, neighbor.Y));
            }

            return neighbors;
        }
    }
}