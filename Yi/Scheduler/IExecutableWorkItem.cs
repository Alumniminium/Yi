using System;
using System.ComponentModel;

namespace Yi.Scheduler
{
    [Browsable(true), TypeConverter(typeof(ExpandableObjectConverter))]
    public interface IExecutableWorkItem : ICanCancel, ICanExecute
    {
        DateTime ExecutionTime { get; }
    }
}