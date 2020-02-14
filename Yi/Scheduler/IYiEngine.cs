using System.ComponentModel;

namespace Yi.Scheduler
{
    [Browsable(true), TypeConverter(typeof(ExpandableObjectConverter))]
    internal interface IYiEngine
    {
        ISupportsCount[] WorkItemQueueCounts { get; }
    }
}