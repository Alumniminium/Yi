using System.Collections.Generic;
using System.ComponentModel;

namespace Yi.Scheduler
{
    [Browsable(true), TypeConverter(typeof(ExpandableObjectConverter))]
    internal interface IMultipleItems<T>
    {
        HashSet<T> AllItems { get; }
    }
}