using System.Threading;

namespace Yi.Scheduler
{
    internal static class Extensions
    {
        public static Thread CreateAndRunThread(this ThreadStart ts)
        {
            var t = CreateThread(ts);
            t.Start();
            return t;
        }

        public static Thread CreateThread(this ThreadStart ts)
        {
            var t = new Thread(ts);
            t.TrySetApartmentState(ApartmentState.MTA);
            t.Priority = ThreadPriority.Normal;
            t.IsBackground = true;
            return t;
        }

        public static TInterface[] ToInterfaceArray<TInterface, TClass>(this TClass[] items) where TInterface : class where TClass : class, TInterface
        {
            var array = new TInterface[items.Length];
            for (var n = 0; n < items.Length; n++)
                array[n] = items[n];
            return array;
        }
    }
}