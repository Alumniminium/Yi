using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media.Imaging;
using ItemCreator.Annotations;
using ItemCreator.Enums;

namespace ItemCreator.Pages
{
    /// <summary>
    /// Interaction logic for Equip2D.xaml
    /// </summary>
    public partial class Equip2D : INotifyPropertyChanged
    {
        private Equipment _equipment = Equipment.Instance;
        private uint _selectedItemType;
        private EnumToItemsSource _itemTypesSource = new EnumToItemsSource(typeof(ItemType2DEnum));

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
        public static Equip2D Instance;

        public Equip2D()
        {
            Instance = this;
            DataContext = this;
            InitializeComponent();
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

        private void ImportButtonClick(object sender, RoutedEventArgs e)
        {
            var importBrowser = new ItemImportWindow((ItemType2DEnum)SelectedItemType);
            var entry = importBrowser.ShowDialogAndImport();
            Equipment.CopyFrom(entry);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
