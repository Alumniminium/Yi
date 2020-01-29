using System;

namespace YiX.SelfContainedSystems
{
    public static class GCMonitor
    {
        public static int Gen0 = 0;
        public static int Gen1 = 0;
        public static int Gen2 = 0;

        public static int OffsetGen0 = 0;
        public static int OffsetGen1 = 0;
        public static int OffsetGen2 = 0;

        public static void Calibrate()
        {
            OffsetGen0 = GC.CollectionCount(0);
            OffsetGen1 = GC.CollectionCount(1);
            OffsetGen2 = GC.CollectionCount(2);
        }

        public static int Get(int gen)
        {
            switch (gen)
            {
                case -1:
                    return GC.CollectionCount(0) + GC.CollectionCount(1) + GC.CollectionCount(2) - OffsetGen0 - OffsetGen1 - OffsetGen2;
                case 0:
                    return GC.CollectionCount(0) - OffsetGen0;
                case 1:
                    return GC.CollectionCount(1) - OffsetGen1;
                case 2:
                    return GC.CollectionCount(2) - OffsetGen2;
            }
            return -1;
        }
    }
}