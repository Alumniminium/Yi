using System.Collections.Generic;

namespace ItemCreator.ClientFiles
{
    public class ItemMiniIcon
    {
        public int Id;
        public Dictionary<int, string> Frames;

        public ItemMiniIcon()
        {
            Frames = new Dictionary<int, string>();
        }

        public string Key { get; set; }
    }
}