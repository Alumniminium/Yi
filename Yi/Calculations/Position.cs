using System;
using System.Collections.Generic;
using Yi.Entities;
using Yi.Enums;
using Yi.Structures;

namespace Yi.Calculations
{
    public struct Position
    {
        private static float GetAngle(ushort x, ushort y, ushort x2, ushort y2)
        {
            float xf1 = x, xf2 = x2, yf1 = y, yf2 = y2;
            var result = (float)(90 - Math.Atan((xf1 - xf2) / (yf1 - yf2)) * 180 / Math.PI);
            if (xf1 - xf2 < 0 && yf1 - yf2 < 0)
                result += 180;
            else if (xf1 - xf2 == 0 && yf1 - yf2 < 0)
                result += 180;
            else if (yf1 - yf2 < 0 && xf1 - xf2 > 0)
                result -= 180;
            return result;
        }
        
        public static Vector2 GetBorderCoords(ushort oldX, ushort oldY, ushort targetX, ushort targetY)
        {
            var θ = GetAngle(oldX, oldY, targetX, targetY);
            float w, h;
            var v = new Vector2(0,0);
            byte quadrant = 1;
            if (θ < 0)
                θ += 360;
            else if (θ == 360)
                θ = 0;
            while (θ >= 90)
            {
                θ -= 90;
                quadrant++;
            }
            float screendistance;
            switch (quadrant)
            {
                case 1:
                    screendistance = (float)(18 / Math.Cos(θ * Math.PI / 180));
                    if (screendistance > 25)
                        screendistance = (float)(18 / Math.Sin(θ * Math.PI / 180));
                    else if (θ != 0)
                        v.Y++;
                    h = (float)(screendistance * Math.Sin(θ * Math.PI / 180));
                    w = (float)(screendistance * Math.Cos(θ * Math.PI / 180));
                    v.X += (ushort)(targetX + Math.Round(w));
                    if (θ == 90)
                        v.Y += (ushort)(targetY - Math.Round(h));
                    else
                        v.Y += (ushort)(targetY + Math.Round(h));
                    break;
                case 2:
                    screendistance = (float)(18 / Math.Cos(θ * Math.PI / 180));
                    if (screendistance > 25)
                    {
                        screendistance = (float)(18 / Math.Sin(θ * Math.PI / 180));
                        v.Y++;
                    }
                    w = (float)(screendistance * Math.Sin(θ * Math.PI / 180));
                    h = (float)(screendistance * Math.Cos(θ * Math.PI / 180));
                    v.X += (ushort)(targetX - w);
                    v.Y += (ushort)(targetY + h);
                    break;
                case 3:
                    screendistance = (float)(18 / Math.Cos(θ * Math.PI / 180));
                    if (screendistance > 25)
                        screendistance = (float)(18 / Math.Sin(θ * Math.PI / 180));
                    h = (float)(screendistance * Math.Sin(θ * Math.PI / 180));
                    w = (float)(screendistance * Math.Cos(θ * Math.PI / 180));
                    v.X += (ushort)(targetX - w);
                    v.Y += (ushort)(targetY - h);
                    break;
                case 4:
                    screendistance = (float)(18 / Math.Cos(θ * Math.PI / 180));
                    if (screendistance > 25)
                        screendistance = (float)(18 / Math.Sin(θ * Math.PI / 180));
                    else if (θ > 0)
                        v.X++;
                    w = (float)(screendistance * Math.Sin(θ * Math.PI / 180));
                    h = (float)(screendistance * Math.Cos(θ * Math.PI / 180));
                    v.X += (ushort)(targetX + w);
                    v.Y += (ushort)(targetY - h);
                    break;
            }
            return v;
        }

        public static Direction GetDirection(YiObj source, YiObj destination)
        {
            if (source == null || destination == null)
                return default(Direction);

            return GetDirection(source.Location, destination.Location);
        }

        public static int GetDistance(YiObj source, YiObj destination)
        {
            if (source == null || destination == null || source.MapId != destination.MapId)
                return 255;

            return GetDistance(source.Location, destination.Location);
        }

        public static bool CanSee(YiObj source, YiObj destination)
        {
            if (source == null || destination == null)
                return false;

            return source.MapId == destination.MapId && GetDistance(source, destination) <= 18;
        }

        public static bool CanSee(ushort x1, ushort y1, ushort x2, ushort y2) => GetDistance(x1, y1, x2, y2) <= 18;

        public static bool CanSeeBig(YiObj source, YiObj destination)
        {
            if (source == null || destination == null)
                return false;

            return source.MapId == destination.MapId && GetDistance(source, destination) <= 28;
        }

        public static Direction GetDirection(ushort x1, ushort y1, ushort x2, ushort y2)
        {
            var angle = Math.Atan2(x2 - x1, y2 - y1) * 57.29 + 90;
            angle = angle < 0 ? 270 + (90 - Math.Abs(angle)) : angle;
            var direction = (byte)Math.Round(angle / 45.0);
            return (Direction)(direction == 8 ? 0 : direction);
        }
        public static Direction GetDirection(Vector2 start, Vector2 end)
        {
            var angle = Math.Atan2(end.X - start.X, end.Y - start.Y) * 57.29 + 90;
            angle = angle < 0 ? 270 + (90 - Math.Abs(angle)) : angle;
            var direction = (byte)Math.Round(angle / 45.0);
            return (Direction)(direction == 8 ? 0 : direction);
        }
        public static int GetDirectionCo(int x0, int y0, int x1, int y1)
        {
            var dir = 0;

            var tan = new[] { -241, -41, 41, 241 };
            var deltaX = x1 - x0;
            var deltaY = y1 - y0;

            if (deltaX == 0)
            {
                dir = deltaY > 0 ? 0 : 4;
            }
            else if (deltaY == 0)
            {
                dir = deltaX > 0 ? 6 : 2;
            }
            else
            {
                var flag = Math.Abs(deltaX) / deltaX;

                deltaY *= 100 * flag;
                int i;
                for (i = 0; i < 4; i++)
                    tan[i] *= Math.Abs(deltaX);

                for (i = 0; i < 3; i++)
                    if (deltaY >= tan[i] && deltaY < tan[i + 1])
                        break;

                //** note :
                //   i=0    ------- -241 -- -41
                //   i=1    ------- -41  --  41
                //   i=2    -------  41  -- 241
                //   i=3    -------  241 -- -241

                deltaX = x1 - x0;
                deltaY = y1 - y0;

                if (deltaX > 0)
                {
                    switch (i)
                    {
                        case 0:
                            dir = 5;
                            break;
                        case 1:
                            dir = 6;
                            break;
                        case 2:
                            dir = 7;
                            break;
                        case 3:
                            dir = deltaY > 0 ? 0 : 4;
                            break;
                    }
                }
                else
                {
                    switch (i)
                    {
                        case 0:
                            dir = 1;
                            break;
                        case 1:
                            dir = 2;
                            break;
                        case 2:
                            dir = 3;
                            break;
                        case 3:
                            dir = deltaY > 0 ? 0 : 4;
                            break;
                    }
                }
            }

            dir = (dir + 8) % 8;
            return dir;
        }

        public static int GetDistance(ushort x1, ushort y1, ushort x2, ushort y2)
        {
            var absX = Math.Abs(x1 - x2);
            var absY = Math.Abs(y1 - y2);
            return Math.Max(absX, absY);
        }
        public static int GetDistance(Vector2 start, Vector2 end)
        {
            var absX = Math.Abs(start.X - end.X);
            var absY = Math.Abs(start.Y- end.Y);
            return Math.Max(absX, absY);
        }

        public static bool IsInArc(Vector2 from, Vector2 end, Vector2 target, int range)
        {
            const int defaultMagicArc = 90;
            const float radianDelta = (float)(Math.PI * defaultMagicArc / 180);

            var centerLine = GetRadian(from, end);
            var targetLine = GetRadian(from, target);
            var delta = Math.Abs(centerLine - targetLine);

            return (delta <= radianDelta || delta >= 2 * Math.PI - radianDelta) && GetLineLength(from, target) < range;
        }

        private static float GetRadian(Vector2 source, Vector2 target)
        {
            if (!(source.X != target.X || source.Y != target.Y))
                return 0f;

            var deltaX = target.X - source.X;
            var deltaY = target.Y - source.Y;
            var distance = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);

            if (!(deltaX <= distance && distance > 0))
                return 0f;
            var radian = Math.Asin(deltaX / distance);

            return (float)(deltaY > 0 ? Math.PI / 2 - radian : Math.PI + radian + Math.PI / 2);
        }

        private static int GetLineLength(Vector2 from, Vector2 to)
        {
            var deltaX = Math.Abs(to.X - from.X);
            var deltaY = Math.Abs(to.Y - from.Y);
            return (int)Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
        }

        /// <summary>
        /// Return all points on that line. (From TQ)
        /// </summary>
        public static void DdaLine(int x0, int y0, int x1, int y1, int nRange, ref List<Vector2> vctPoint)
        {
            if (x0 == x1 && y0 == y1)
                return;

            var scale = (float)(1.0f * nRange / Math.Sqrt((x1 - x0) * (x1 - x0) + (y1 - y0) * (y1 - y0)));
            x1 = (int)(0.5f + scale * (x1 - x0) + x0);
            y1 = (int)(0.5f + scale * (y1 - y0) + y0);
            DdaLineEx(x0, y0, x1, y1, ref vctPoint);
        }

        /// <summary>
        /// Return all points on that line. (From TQ)
        /// </summary>
        private static void DdaLineEx(int x0, int y0, int x1, int y1, ref List<Vector2> vctPoint)
        {
            if (x0 == x1 && y0 == y1)
                return;
            if (vctPoint == null)
                vctPoint = new List<Vector2>();
            var dx = x1 - x0;
            var dy = y1 - y0;
            var absDx = Math.Abs(dx);
            var absDy = Math.Abs(dy);
            Vector2 vector2;
            var delta = absDy * (dx > 0 ? 1 : -1);
            if (absDx > absDy)
            {
                delta = absDx * (dy > 0 ? 1 : -1);
                var numerator = dy * 2;
                var denominator = absDx * 2;
                if (dx > 0)
                {
                    // x0 ++
                    for (var i = 1; i <= absDx; i++)
                    {
                        vector2 = new Vector2((ushort)(x0 + i),(ushort)(y0 + (numerator * i + delta) / denominator));
                        vctPoint.Add(vector2);
                    }
                }
                else if (dx < 0)
                {
                    // x0 --
                    for (var i = 1; i <= absDx; i++)
                    {
                        vector2 = new Vector2((ushort) (x0 - i), (ushort) (y0 + (numerator*i + delta)/denominator));
                        vctPoint.Add(vector2);
                    }
                }
            }
            else
            {
                var numerator = dx * 2;
                var denominator = absDy * 2;
                if (dy > 0)
                {
                    // y0 ++
                    for (var i = 1; i <= absDy; i++)
                    {
                        vector2 = new Vector2 ((ushort)(y0 + i), (ushort)(x0 + (numerator * i + delta) / denominator));
                        vctPoint.Add(vector2);
                    }
                }
                else if (dy < 0)
                {
                    // y0 -- 
                    for (var i = 1; i <= absDy; i++)
                    {
                        vector2 = new Vector2 ((ushort)(y0 - i),  (ushort)(x0 + (numerator * i + delta) / denominator));
                        vctPoint.Add(vector2);
                    }
                }
            }
        }

        public static bool InAttackRange(Monster monster, YiObj currentTarget)
        {
            if (currentTarget == null) return false;
            return GetDistance(monster, currentTarget) <= monster.AttackRange;
        }
    }
}