using System.Windows;
using System.Windows.Controls;

namespace ItemCreator.Pages
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : Page
    {
        private readonly Equip2D _equip2D = new Equip2D();
        private readonly Equip3D _equip3D = new Equip3D();
        private readonly QuestItem _questItem = new QuestItem();
        public Home()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            EquipImg.MouseLeftButtonUp += (o, args) => Application.Current.MainWindow.Content = _equip3D;
            Equip2DImg.MouseLeftButtonUp += (o, args) => Application.Current.MainWindow.Content = _equip2D;
            QuestItemImg.MouseLeftButtonUp += (o, args) => Application.Current.MainWindow.Content = _questItem;
            //UsableItemImg.MouseLeftButtonUp += (o, args) => Application.Current.MainWindow.Content = new QuestItem();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var settings = new Settings();
            settings.ShowDialog();
        }
    }
}
