using System.Collections.Generic;
using System.IO;
using System.Linq;
using ItemCreator.CoreDLL.IO.Ini;
using ItemCreator.CoreDLL.Security;
using ItemCreator.Enums;

namespace ItemCreator.ClientFiles
{
    public static class ItemTypeManager
    {
        public static readonly ItemType ItemType = new ItemType();
        public static readonly Cofac Crypto = new Cofac();
        public static string ItemTypePath = Config.ConquerPath + @"\ini\itemtype.dat";
        public static void Load()
        {
            Crypto.GenerateKey(0x2537);
            var data = File.ReadAllBytes(ItemTypePath);
            Crypto.Decrypt(ref data);
            File.WriteAllBytes("ItemType.txt", data);
            ItemType.LoadFromTxt("ItemType.txt");
            ItemType.SaveToTxt("ItemType.txt");
            //int id;
            //if (TryGetNextIdForType(ItemType3DEnum.Boots, out id))
            //{

            //}
        }

        public static void Save()
        {
            ItemType.SaveToTxt("ItemType.txt");
            var data = File.ReadAllBytes("ItemType.txt");
            Crypto.Encrypt(ref data);
            File.WriteAllBytes(ItemTypePath, data);
        }

        public static bool TryGetNextIdForType(ItemType3DEnum type3D, out int freeItemId)
        {
            var takenIds = new List<int>();
            foreach (var id in from entry in ItemType.Items let entryType = entry.Key/1000 where entryType == (int) type3D select (entry.Key - (int) type3D*1000)/10 into id where !takenIds.Contains(id) select id)
                takenIds.Add(id);

            takenIds.Sort();
            freeItemId = (int) type3D*10000 + 3;

            if (takenIds.Count == 0)
                freeItemId += 10;
            else if (takenIds.Count > 0)
                freeItemId += takenIds.Last()*10;
            else
            {
                freeItemId = 0;
                return false;
            }
            return true;
        }
    }
}
