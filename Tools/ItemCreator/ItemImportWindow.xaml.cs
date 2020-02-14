using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using ItemCreator.ClientFiles;
using ItemCreator.Controls;
using ItemCreator.CoreDLL.IO.Ini;
using ItemCreator.Enums;
using ItemCreator.Properties;

namespace ItemCreator
{
    /// <summary>
    /// Interaction logic for ItemImportWindow.xaml
    /// </summary>
    public partial class ItemImportWindow : INotifyPropertyChanged
    {
        private uint _selectedItemType;
        private ItemType.Entry _entry;
        private ObservableCollection<ImportableItem> _importableItems = new ObservableCollection<ImportableItem>();
        public ObservableCollection<ImportableItem> ImportableItems
        {
            get { return _importableItems; }
            set
            {
                if (Equals(value, _importableItems)) return;
                _importableItems = value;
                OnPropertyChanged();
            }
        }

        public ItemImportWindow()
        {
            InitializeComponent();
        }
        public ItemImportWindow(ItemType3DEnum selectedItemType)
        {
            InitializeComponent();
            Title = "Import existing Item. Type selected: " + selectedItemType;
            _selectedItemType = (uint)selectedItemType;
        }
        public ItemImportWindow(ItemType2DEnum selectedItemType)
        {
            InitializeComponent();
            Title = "Import existing Item. Type selected: " + selectedItemType;
            _selectedItemType = (uint)selectedItemType;
        }
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var counter = 0;
            foreach (var importableItem in ItemTypeManager.ItemType.Items.OrderBy(i=>i.Value.ID))
            {

                if (_selectedItemType == 0)
                {
                    if (importableItem.Value.RequiredLevel != 0)
                        continue;
                    if (Enum.IsDefined(typeof (ItemType2DEnum), (ItemType2DEnum)(importableItem.Value.ID/1000)))
                        continue;
                    if (Enum.IsDefined(typeof (ItemType3DEnum), (ItemType3DEnum)(importableItem.Value.ID/1000)))
                        continue;
                }
                else
                {
                    if (_selectedItemType != importableItem.Value.ID/1000)
                        continue;
                }

                counter++;
                ImportableItems.Add(new ImportableItem(importableItem.Value));

                //if (counter != Environment.ProcessorCount)
                //    continue;

                await Task.Delay(1);
                counter =0;
            }
        }
        public void Import(ItemType.Entry entry)
        {
            _entry = entry;
        }

        public ItemType.Entry ShowDialogAndImport()
        {
            ShowDialog();
            return _entry;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
