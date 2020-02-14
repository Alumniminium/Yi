using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using ItemCreator.ClientFiles;
using ItemCreator.CoreDLL.IO.Ini;
using ItemCreator.Enums;

namespace ItemCreator.Controls
{
    /// <summary>
    /// Interaction logic for ImportableItem.xaml
    /// </summary>
    public partial  class ImportableItem : UserControl
    {
        public ItemType.Entry Entry;

        public ImportableItem()
        {
            InitializeComponent();
        }

        public ImportableItem(ItemType.Entry entry)
        {
            InitializeComponent();
            Entry = entry;
            MouseDoubleClick += (sender, args) =>
            {
                var parentWindow = (ItemImportWindow)Window.GetWindow(this);
                parentWindow?.Import(Entry);
                parentWindow?.Close();
            };
            unsafe
            {
                var name = "";
                for (var i = 0; i < 16; i++)
                    name += (char)entry.Name[i];
                
                name=name.Replace("\0", "");
                var quality = entry.ID%10;
                var itemText = $"[{(ItemQuality)quality}] {name}";
                ItemTextField.Text = itemText;
                ItemIdField.Text = entry.ID.ToString();
                AddImage(entry);
            }
        }

        private async void AddImage(ItemType.Entry entry)
        {
            var bmp = await Task.Run(() =>
            {
                GetInvIcon(entry, out BitmapImage bitmapImage);
                return bitmapImage;
            });
            ItemImageField.Source = bmp;
            var bmp2 = await Task.Run(() =>
            {
                GetMapIcon(entry, out BitmapImage bitmapImage);
                return bitmapImage;
            });
            MapImageField.Source = bmp2;
            var bmp3 = await Task.Run(() =>
            {
                GetTextureF(entry, out BitmapImage bitmapImage);
                return bitmapImage;
            });
            TextureImageField.Source = bmp3;
            var bmp4 = await Task.Run(() =>
            {
                GetTextureM(entry, out BitmapImage bitmapImage);
                return bitmapImage;
            });
            TextureMImageField.Source = bmp4;
        }

        private static void GetInvIcon(ItemType.Entry entry, out BitmapImage bitmapImage)
        {
            if (!FindItemMiniImage(entry, out string image))
            {
                bitmapImage = null;
                return;
            }

            using (var stream = new MemoryStream(File.ReadAllBytes(image)))
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = stream;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                bitmap.Freeze();
                bitmapImage = bitmap;
            }
        }
        private static void GetMapIcon(ItemType.Entry entry, out BitmapImage bitmapImage)
        {
            if (!FindMapItemImage(entry, out string image))
            {
                bitmapImage = null;
                return;
            }

            using (var stream = new MemoryStream(File.ReadAllBytes(image)))
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = stream;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                bitmap.Freeze();
                bitmapImage = bitmap;
            }
        }
        private static void GetTextureF(ItemType.Entry entry, out BitmapImage bitmapImage)
        {
            if (!GetFemaleTexturePath(entry, out string image))
            {
                bitmapImage = null;
                return;
            }

            using (var stream = new MemoryStream(File.ReadAllBytes(image)))
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = stream;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                bitmap.Freeze();
                bitmapImage = bitmap;
            }
        }
        private static void GetTextureM(ItemType.Entry entry, out BitmapImage bitmapImage)
        {
            if (!GetMaleTexturePath(entry, out string image))
            {
                bitmapImage = null;
                return;
            }

            using (var stream = new MemoryStream(File.ReadAllBytes(image)))
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = stream;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                bitmap.Freeze();
                bitmapImage = bitmap;
            }
        }

        private static bool GetFemaleTexturePath(ItemType.Entry entry, out string textureF)
        {
            textureF = Config.ConquerPath + $@"\c3\texture\{entry.ID}.dds";
            if (!File.Exists(textureF))
                textureF = Config.ConquerPath + $@"\c3\texture\{entry.ID / 10 * 10}.dds";
            if (!File.Exists(textureF))
                textureF = Config.ConquerPath + $@"\c3\texture\00{entry.ID / 10 + 100000}0.dds";
            if (File.Exists(textureF))
                return true;

            if (WeaponReader.Entries.TryGetValue(entry.ID, out Weapon wep))
                textureF = Config.ConquerPath + $@"\c3\texture\{wep.TextureId}.dds";

            if (File.Exists(textureF))
                return true;

            if (ArmetReader.Entries.TryGetValue(entry.ID / 10 + 1100000, out Armet arm))
                textureF = Config.ConquerPath + $@"\c3\texture\{arm.TextureId}.dds";

            return File.Exists(textureF);
        }
        private static bool GetMaleTexturePath(ItemType.Entry entry, out string textureM)
        {
            textureM = Config.ConquerPath + $@"\c3\texture\{entry.ID}.dds";
            if (!File.Exists(textureM))
                textureM = Config.ConquerPath + $@"\c3\texture\{entry.ID / 10 * 10}.dds";
            if (!File.Exists(textureM))
                textureM = Config.ConquerPath + $@"\c3\texture\00{entry.ID / 10 + 200000}0.dds";
            if (File.Exists(textureM))
                return true;

            if (WeaponReader.Entries.TryGetValue(entry.ID, out Weapon wep))
                textureM = Config.ConquerPath + $@"\c3\texture\{wep.TextureId}.dds";

            if (File.Exists(textureM))
                return true;

            if (ArmetReader.Entries.TryGetValue(entry.ID / 10 + 2100000, out Armet arm))
                textureM = Config.ConquerPath + $@"\c3\texture\{arm.TextureId}.dds";

            return File.Exists(textureM);
        }

        private static bool FindItemMiniImage(ItemType.Entry entry, out string image)
        {
            if (ItemMiniIcons.Entries.TryGetValue(entry.ID, out ItemMiniIcon itemMiniIcon))
            {
                image = Config.ConquerPath + itemMiniIcon.Frames.Values.FirstOrDefault();
                if (File.Exists(image))
                    return true;
                //MessageBox.Show("Item Mini Icon referenced in ItemMinIcon.ani but not found at specified path!\r\n" + image, "FATAL!");
            }
            image = Config.ConquerPath + @"\data\ItemMinIcon\" + entry.ID + ".dds";
            if (File.Exists(image))
                return true;
            image = Config.ConquerPath + @"\data\ItemMinIcon\" + entry.ID / 10 * 10 + ".dds";
            return File.Exists(image);
        }
        private static bool FindMapItemImage(ItemType.Entry entry, out string image)
        {
            if (MapItemIcons.Entries.TryGetValue(entry.ID, out MapItemIcon mapItemIcon))
            {
                image = Config.ConquerPath + mapItemIcon.Frames.Values.FirstOrDefault();
                if (File.Exists(image))
                    return true;
                //MessageBox.Show("Item Map Icon referenced in ItemMinIcon.ani but not found at specified path!\r\n" + image, "FATAL!");
            }
            image = Config.ConquerPath + @"\data\MapItemIcon\" + entry.ID + ".dds";
            if (File.Exists(image))
                return true;
            image = Config.ConquerPath + @"\data\MapItemIcon\" + entry.ID / 10 * 10 + ".dds";
            return File.Exists(image);
        }
    }
}
