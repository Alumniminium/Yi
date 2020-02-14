using System;
using System.Collections.Generic;
using YiX.Structures;

namespace YiX.World.Pathfinding
{
    /// <summary>
    ///     No more efficient than JPS but uses an enumerator to build real-time paths.
    ///     Most effective for target points that move rapidly, but in areas that aren't largly obstructed/mazes.
    /// </summary>
    public static class SimplePath
    {
        public static IEnumerable<Vector2> FindPath(PathingMaster args)
        {
            var priority = args.Priority;
            var endNode = args.Target;
            var currentNode = args.Start;

            switch (true)
            {
                case true:
                    while (IdentifySuccessors(args, currentNode))
                    {
                        var newNode = priority.Top();

                        newNode.Parent = currentNode;
                        currentNode = newNode;

                        yield return new Vector2(currentNode.X, currentNode.Y);

                        if (currentNode.Equals(endNode))
                            yield break;
                    }
                    goto case false;
                case false:
                    //count current path size
                    var newCurrent = 0;
                    var checkNode = currentNode;
                    for (; checkNode.Parent != null; newCurrent++)
                        checkNode = currentNode.Parent;

                    //go back by half? (skips over closed nodes from last path)
                    newCurrent /= 2;
                    checkNode = currentNode;
                    for (var advanced = 0; advanced <= newCurrent && checkNode.Parent != null; advanced++)
                        currentNode = checkNode.Parent;

                    //retry pathing
                    goto case true;
            }
        }

        private static bool IdentifySuccessors(PathingMaster args, PathingNode activeNode)
        {
            var priority = args.Priority;
            var endNode = args.Target;

            priority.Clear();
            activeNode.IsClosed = true;

            var any = false;
            var neighbors = activeNode.GetNeighbors();
            foreach (var neighbor in neighbors)
            {
                var checkNode = args.GetNode(neighbor.X, neighbor.Y);
                if (checkNode == null)
                    continue;
                if (args.ValidateNodeForPath(checkNode))
                    continue;
                if (checkNode.IsClosed)
                    continue;

                //get angle, active to end
                var activeToEndAngle = activeNode.GetAngleRadian(endNode);
                //check arc
                if (activeNode.HasInArc(neighbor, activeToEndAngle, 2 * Math.PI / 3, 2.0f))
                {
                    //distance(h) + active to end angle difference from active to neighbor * 2(g) = f
                    var activeToNeighborAngle = activeNode.GetRelativeAngleRadian(neighbor, activeToEndAngle);
                    checkNode.G = Math.Abs(activeToEndAngle - activeToNeighborAngle) * 2;
                    checkNode.H = checkNode.HDistance(endNode);
                    checkNode.F = checkNode.G + checkNode.H;

                    //push to priority stack
                    priority.Add(checkNode);
                    any = true;
                }
            }

            return any;
        }
    }
}