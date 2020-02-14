using System;
using System.IO;
using System.Windows;
using Newtonsoft.Json;

namespace Launcher.Pages
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage
    {
        public bool InstallPathValid;

        public SettingsPage()
        {
            InitializeComponent();
        }

        private void SaveAndContinueButtonClick(object sender, RoutedEventArgs e)
        {
            if (InstallPathValid)
            {
                if (Core.Settings.ExistingClientPath == null)
                    Core.Settings.ExistingClientPath = "";

                var json = JsonConvert.SerializeObject(Core.Settings);
                File.WriteAllText("Config.json", json);
                MainWindow.This.Content = new HomePage();
            }
            else
                MessageBox.Show("Install Path is not valid!!");
        }

        private void SetExistingClientPathClick(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.ShowDialog();


            if (File.Exists(dialog.SelectedPath + "\\C3_CORE_DLL.dll"))
            {
                Core.Settings.ExistingClientPath = dialog.SelectedPath + "\\";
                ExistingClientPathStatusBlock.Visibility = Visibility.Visible;
            }
            else
            {
                MessageBox.Show("Invalid Directory. No C3_CORE_DLL.dll found. (Will download everything)");
                Core.Settings.ExistingClientPath = "";
            }
        }

        private void SetInstallPathButtonClick(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.ShowDialog();

            try
            {
                File.Create(dialog.SelectedPath + "\\Test.file").Close();
                File.Delete(dialog.SelectedPath + "\\Test.file");
                Core.Settings.InstallPath = dialog.SelectedPath+"\\";
                InstallPathStatusBlock.Visibility = Visibility.Visible;
                InstallPathValid = true;
            }
            catch (Exception)
            {
                InstallPathValid = false;
                MessageBox.Show("Invalid Directory. No write access. (Maybe start the launcher as admin?)");
            }
        }
    }
}
