using System.ComponentModel;

namespace Yi.Scheduler
{
    [Browsable(true), TypeConverter(typeof(ExpandableObjectConverter))]
    public interface ICanCancel
    {
        bool Cancelled { get; }
    }
}