using System.ComponentModel;

namespace Yi.Scheduler
{
    [Browsable(true), TypeConverter(typeof(ExpandableObjectConverter))]
    public interface ICanExecute
    {
        void Execute();
    }
}