using System.Collections.Generic;
using YiX.Structures;

namespace YiX.AI.Workers
{
    public class WorkerTask
    {
        public Vector2 TargetLocation;
        public List<WorkerInteraction> Interactions = new List<WorkerInteraction>();
        public bool Finished;
        public bool RequireExactPosition;

        public WorkerTask(Vector2 targetLocation,bool exactLocation, params WorkerInteraction[] interactions)
        {
            TargetLocation = targetLocation;
            RequireExactPosition = exactLocation;
            if (interactions != null)
                Interactions.AddRange(interactions);
        }

    }
}