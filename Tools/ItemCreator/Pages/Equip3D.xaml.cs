using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media.Imaging;
using ItemCreator.CoreDLL.IO.Ini;
using ItemCreator.Enums;
using ItemCreator.Properties;

namespace ItemCreator.Pages
{
    /// <summary>
    /// Interaction logic for Equip3D.xaml
    /// </summary>
    public partial class Equip3D : INotifyPropertyChanged
    {
        private uint _selectedItemType;
        private EnumToItemsSource _itemTypesSource = new EnumToItemsSource(typeof(ItemType3DEnum));
        private Equipment _equipment = Equipment.Instance;

        public Equipment Equipment
        {
            get { return _equipment; }
            set
            {
                if (Equals(value, _equipment)) return;
                _equipment = value;
                OnPropertyChanged();
            }
        }

        public EnumToItemsSource ItemTypesSource
        {
            get { return _itemTypesSource; }
            set
            {
                if (Equals(value, _itemTypesSource)) return;
                _itemTypesSource = value;
                OnPropertyChanged();
            }
        }

        public ItemType.Entry Entry { get; set; }

        public uint SelectedItemType
        {
            get { return _selectedItemType; }
            set
            {
                if (value == _selectedItemType) return;
                _selectedItemType = value;
                OnPropertyChanged();
            }
        }
        public static Equip3D Instance;

        public Equip3D()
        {
            Instance = this;
            DataContext = this;
            InitializeComponent();
        }

        private void ImportButtonClick(object sender, RoutedEventArgs e)
        {
            var importBrowser = new ItemImportWindow((ItemType3DEnum)SelectedItemType);
            var entry = importBrowser.ShowDialogAndImport();
            Equipment.CopyFrom(entry);
        }
        private void InvIconImgDrop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop, true)) return;
            var droppedFilePaths = e.Data.GetData(DataFormats.FileDrop, true) as string[];

            if (droppedFilePaths == null)
                return;

            Equipment.InvIconImg = droppedFilePaths[0];
            UpdateInvIcon(droppedFilePaths[0]);
        }

        private void MapIconImgDrop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop, true)) return;
            var droppedFilePaths = e.Data.GetData(DataFormats.FileDrop, true) as string[];

            if (droppedFilePaths == null)
                return;

            Equipment.MapIconImg = droppedFilePaths[0];
            UpdateMapIcon(droppedFilePaths[0]);
        }

        private void TextureImgDrop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop, true)) return;
            var droppedFilePaths = e.Data.GetData(DataFormats.FileDrop, true) as string[];

            if (droppedFilePaths == null)
                return;

            Equipment.TextureF = droppedFilePaths[0];
            UpdateTextureF(droppedFilePaths[0]);
        }
        
        public void UpdateInvIcon(string path)
        {
            using (var stream = new MemoryStream(File.ReadAllBytes(path)))
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = stream;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                bitmap.Freeze();
                InvIconImg.Source = bitmap;
            }
        }

        public void UpdateMapIcon(string path)
        {
            using (var stream = new MemoryStream(File.ReadAllBytes(path)))
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = stream;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                bitmap.Freeze();
                MapIconImg.Source = bitmap;
            }
        }

        public void UpdateTextureM(string path)
        {
            using (var stream = new MemoryStream(File.ReadAllBytes(path)))
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = stream;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                bitmap.Freeze();
                TextureMImg.Source = bitmap;
                //TODO add male texture
            }
        }

        public void UpdateTextureF(string path)
        {
            using (var stream = new MemoryStream(File.ReadAllBytes(path)))
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = stream;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                bitmap.Freeze();
                TextureFImg.Source = bitmap;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}