using System.ComponentModel;

namespace YiX.Scheduler
{
    [Browsable(true), TypeConverter(typeof(ExpandableObjectConverter))]
    internal interface ISupportsEnqueueDequeue<TListType>
    {
        bool Dequeue(out TListType item);
    }
}