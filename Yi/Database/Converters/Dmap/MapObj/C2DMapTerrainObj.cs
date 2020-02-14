using System.IO;
using Yi.Helpers;

namespace Yi.Database.Converters.Dmap.MapObj
{
    public unsafe struct C2DMapTerrainObj //Should be a class with few methods, but useless for a server.
    {
        public fixed byte FileName [260];
        public MyPos PosCell;

        public C2DMapTerrainObjPart[] Parts;

        public void Load(string folder)
        {
            string path;
            fixed (byte* pFileName = FileName)
                path = folder + new string((sbyte*)pFileName);

            //Load the Scene file.
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                int amount;
                stream.Read(&amount, sizeof(int));

                Parts = new C2DMapTerrainObjPart[amount];
                for (var i = 0; i < amount; i++)
                {
                    Parts[i] = new C2DMapTerrainObjPart();
                    Parts[i].Load(stream);
                }
            }
        }
    };
}