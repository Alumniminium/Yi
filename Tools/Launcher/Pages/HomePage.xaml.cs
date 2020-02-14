using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using ClientUpdaterLib;

namespace Launcher.Pages
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class HomePage
    {
        public static readonly Installer Installer = new Installer(Core.Settings.InstallPath, Core.Settings.ExistingClientPath);

        public HomePage()
        {
            InitializeComponent();
            Installer.OnFileChange += OnFileChange;
            Installer.OnDownloadProgressChange += OnDownloadProgressChange;
            Installer.OnTotalProgressChange += OnTotalProgressChange;
            Installer.OnCompleted += OnCompleted;
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            PlayButton.IsEnabled = false;
            await Installer.Setup();
        }
        private void PlayButtonClick(object sender, RoutedEventArgs e)
        {
            var conquerExePath = Core.Settings.InstallPath + "Conquer.exe";

            var info = new ProcessStartInfo(conquerExePath)
            {
                WorkingDirectory = Core.Settings.InstallPath,
                Arguments = "BLACKNULL"
            };

            Process.Start(info);
        }
        private async void VerifyButtonClick(object sender, RoutedEventArgs e)
        {
            PlayButton.IsEnabled = false;
            await Installer.VerifyClient(true);
        }

        private void OnCompleted()
        {
            Dispatcher.Invoke(() =>
            {
                ClientFileItem.SetText("Ready!");
                ClientFileItem.SetProgress(0);
                ClientFileItem.SetTotalProgress(100);
                PlayButton.IsEnabled = true;
            });
        }
        private void OnTotalProgressChange(double d) => Dispatcher.Invoke(() => ClientFileItem.SetTotalProgress(d));
        private void OnDownloadProgressChange(double d) => Dispatcher.Invoke(() => ClientFileItem.SetProgress((int)d));
        private void OnFileChange(string s) => Dispatcher.Invoke(() => ClientFileItem.SetText(s));
    }
}