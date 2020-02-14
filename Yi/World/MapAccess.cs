using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Yi.World
{
    [DataContract]
    public struct MapAccess
    {
        public static Dictionary<ushort, MapAccess> MapData = new Dictionary<ushort, MapAccess>();

        [DataMember]
        public BitArray GroundLayer;
        [DataMember]
        public BitArray MobLayer;
        [DataMember]
        public BitArray ItemLayer;

        public MapAccess(bool[] access)
        {
            GroundLayer = new BitArray(access);
            MobLayer = new BitArray(access);
            ItemLayer = new BitArray(access);
        }
    }
}