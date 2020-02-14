using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;

namespace Launcher.Pages
{
    /// <summary>
    /// Interaction logic for LoadingPage.xaml
    /// </summary>
    public partial class LoadingPage : Page
    {
        public LoadingPage()
        {
            InitializeComponent();
            Core.Settings=new Settings();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            await Task.Delay(1000);

            if (File.Exists("Config.json"))
                Core.Settings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText("Config.json"));

            if (Core.Settings.InstallPath == string.Empty)
            {
                MainWindow.This.Content = new SettingsPage();
            }
            else
                MainWindow.This.Content = new HomePage();
        }
    }
}
