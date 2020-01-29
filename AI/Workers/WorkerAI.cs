using System.Collections.Generic;
using YiX.Calculations;
using YiX.Entities;
using YiX.Structures;
using YiX.World;

namespace YiX.AI.Workers
{
    public class WorkerAI
    {
        public readonly YiObj Owner;
        public readonly Queue<WorkerTask> WorkerQueue = new Queue<WorkerTask>();
        public WorkerTask CurrentTask;
        public readonly Queue<Vector2> Waypoints=new Queue<Vector2>();

        public WorkerAI(YiObj owner)
        {
            Owner = owner;
        }


        public void Think()
        {
            if (CurrentTask == null || CurrentTask.Finished)
            {
                if (WorkerQueue.Count > 0)
                    CurrentTask = WorkerQueue.Dequeue();
            }
            else
            {
                if (CurrentTask.RequireExactPosition)
                {
                    if (CurrentTask.TargetLocation == Owner.Location)
                        Action();
                    else
                        Move();
                }
                else
                {
                    if (Position.GetDistance(CurrentTask.TargetLocation, Owner.Location) < 8)
                        Action();
                    else
                        Move();
                }
            }
        }

        public void GetPath()
        {
            var path = GameWorld.Maps[Owner.MapId].Path(Owner.Location, CurrentTask.TargetLocation);
            if (path == null)
                return;
            foreach (var vector2 in path)
                Waypoints.Enqueue(vector2);
        }

        public void Move()
        {
            if (Waypoints.Count == 0)
            {
                GetPath();
                return;
            }

            var waypoint = Waypoints.Dequeue();

            Owner.Jump(waypoint);
        }

        public void Action()
        {
            
        }
    }
}
