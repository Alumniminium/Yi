using System.Windows;

namespace ItemCreator
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        public Settings()
        {
            InitializeComponent();
        }

        private void SaveButtonClick(object sender, RoutedEventArgs e)
        {
            Config.ConquerPath = textBox.Text;
            Close();
        }
    }
}
