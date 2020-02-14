using System.Collections.Generic;

namespace ItemCreator.ClientFiles
{
    public class MapItemIcon
    {
        public int Id;
        public Dictionary<int, string> Frames;

        public MapItemIcon()
        {
            Frames=new Dictionary<int, string>();
        }
    }
}