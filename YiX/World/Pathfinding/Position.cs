using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using YiX.Structures;

// ReSharper disable All

namespace YiX.World.Pathfinding
{
    public interface IPosition
    {
        ushort X { get; set; }
        ushort Y { get; set; }
        float EuclideanDistance(IPosition otherPos);
        IEnumerable<IPosition> GetNeighbors();
    }

    [StructLayout(LayoutKind.Sequential)]
    public class Position : IPosition
    {
        public ushort X { get; set; }
        public ushort Y { get; set; }

        public ushort LastX;
        public ushort LastY;

        public Position(int x, int y)
        {
            X = (ushort)x;
            Y = (ushort)y;
            LastX = X;
            LastY = Y;
        }

        public Position(IPosition pos)
        {
            X = pos.X;
            Y = pos.Y;
            LastX = pos.X;
            LastY = pos.Y;
        }
        
        public float EuclideanDistance(IPosition otherPos)
        {
            return (float)Math.Abs(Math.Sqrt(Math.Abs(X - otherPos.X) + Math.Abs(Y - otherPos.Y)));
        }

        /// <summary>
        ///     Gets all 8 neighboring positions relative to this position.
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<IPosition> GetNeighbors()
        {
            for (var i = -1; i <= 1; i += 1)
            {
                for (var j = -1; j <= 1; j += 1)
                {
                    if (!(i == 0 && j == 0))
                        yield return new Position((short)(X + i), (short)(Y + j));
                }
            }
        }

        /// <summary>
        ///     Traces from this position to the other.
        /// </summary>
        public IEnumerable<Vector2> Line(Position toPos)
        {
            var fromPos = new Position(X, Y);
            var x0 = X;
            var y0 = Y;
            var x1 = toPos.X;
            var y1 = toPos.Y;

            var dX = x1 - x0;
            var dY = y1 - y0;

            if (dX != 0 && dY != 0)
            {
                while (fromPos != toPos)
                {
                    fromPos.X += (ushort)(dX / Math.Abs(dX));
                    fromPos.Y += (ushort)(dY / Math.Abs(dY));
                    yield return new Vector2(fromPos.X, fromPos.Y);
                }
            }
            else if (dX == 0)
            {
                while (fromPos != toPos)
                {
                    fromPos.Y += (ushort)(dY / Math.Abs(dY));
                    yield return new Vector2(fromPos.X, fromPos.Y);
                }
            }
            else
            {
                while (fromPos != toPos)
                {
                    fromPos.X += (ushort)(dX / Math.Abs(dX));
                    yield return new Vector2(fromPos.X, fromPos.Y);
                }
            }
        }

        public bool HasInArc(IPosition otherPos, float orientation, double arc, float border)
        {
            var angle = GetRelativeAngleRadian(otherPos, orientation);

            if (angle > Math.PI)
                angle -= (float)Math.PI * 2;

            var leftSide = -1 * (arc / border);
            var rightSide = (arc / border);
            return ((angle >= leftSide) && (angle <= rightSide));
        }

        public float GetAngleRadian(IPosition otherPos)
        {
            var deltaX = otherPos.X - X;
            var deltaY = otherPos.Y - Y;

            var radian = Math.Atan2(deltaY, deltaX);
            radian -= Math.PI / 2;
            if (radian < 0)
                radian += Math.PI * 2;
            return (float)radian;
        }

        public float GetRelativeAngleRadian(IPosition otherPos, float orientation)
        {
            return GetAngleRadian(otherPos) - orientation;
        }

        public static bool operator ==(Position pos1, Position pos2)
        {
            var objThis = pos1 as object;
            var objOther = pos2 as object;
            if (objThis == null && objOther == null)
                return true;

            if (objThis == null)
                return false;
            if (objOther == null)
                return false;

            return pos1.GetHashCode() == pos2.GetHashCode();
        }

        public static bool operator !=(Position pos1, Position pos2)
        {
            return !(pos1 == pos2);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return X | Y << 16;
            }
        }

        public override bool Equals(object obj)
        {
            return obj is Position && this == (Position)obj;
        }
    }
}