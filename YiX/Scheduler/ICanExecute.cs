using System.ComponentModel;

namespace YiX.Scheduler
{
    [Browsable(true), TypeConverter(typeof(ExpandableObjectConverter))]
    public interface ICanExecute
    {
        void Execute();
    }
}