using System.Collections.Generic;
using System.ComponentModel;

namespace YiX.Scheduler
{
    [Browsable(true), TypeConverter(typeof(ExpandableObjectConverter))]
    internal interface IMultipleItems<T>
    {
        HashSet<T> AllItems { get; }
    }
}