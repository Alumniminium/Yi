using System;
using System.ComponentModel;

namespace YiX.Scheduler
{
    [Browsable(true), TypeConverter(typeof(ExpandableObjectConverter))]
    public interface IExecutableWorkItem : ICanCancel, ICanExecute
    {
        DateTime ExecutionTime { get; }
    }
}