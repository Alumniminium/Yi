using System.IO;
using System.Text;
using System.Threading.Tasks;
using Yi.Helpers;
using Yi.Items;

namespace Yi.Database.Converters
{
    public static class ItemBonusConverter
    {
        public static Task Load()
        {
            return Task.Run(() =>
            {
                using (var bReader = new BinaryReader(new FileStream("RAW\\itemaddition.pkg", FileMode.Open, FileAccess.Read, FileShare.Read), Encoding.GetEncoding("iso-8859-1")))
                {
                    bReader.BaseStream.Seek(4, SeekOrigin.Begin);
                    var count = bReader.ReadInt32();

                    for (var I = 0; I < count; I++)
                    {
                        bReader.BaseStream.Seek(4, SeekOrigin.Current);
                        var id = bReader.ReadInt32();
                        var info = new ItemBonus {Life = bReader.ReadInt16(), MaxAtk = bReader.ReadInt16(), MinAtk = bReader.ReadInt16(), Defence = bReader.ReadInt16(), MAtk = bReader.ReadInt16(), MDef = bReader.ReadInt16(), Dexterity = bReader.ReadInt16(), Dodge = bReader.ReadInt16()};
                        Collections.ItemBonus.AddOrUpdate(id, info);
                    }
                }
            });
        }
    }
}