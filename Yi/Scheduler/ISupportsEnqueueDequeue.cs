using System.ComponentModel;

namespace Yi.Scheduler
{
    [Browsable(true), TypeConverter(typeof(ExpandableObjectConverter))]
    internal interface ISupportsEnqueueDequeue<TListType>
    {
        bool Dequeue(out TListType item);
    }
}