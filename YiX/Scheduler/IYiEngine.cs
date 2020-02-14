using System.ComponentModel;

namespace YiX.Scheduler
{
    [Browsable(true), TypeConverter(typeof(ExpandableObjectConverter))]
    internal interface IYiEngine
    {
        ISupportsCount[] WorkItemQueueCounts { get; }
    }
}