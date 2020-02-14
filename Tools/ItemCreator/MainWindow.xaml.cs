using System;
using System.Linq;
using System.Windows;
using ItemCreator.ClientFiles;
using ItemCreator.Pages;

namespace ItemCreator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ItemTypeManager.Load();
            ArmetReader.Load();
            WeaponReader.Load();
            MapItemIcons.Load();
            ItemMiniIcons.Load();
            VerifyIntegrity();
        }

        private static void VerifyIntegrity()
        {
            if (MapItemIcons.Entries.Count > ItemMiniIcons.Entries.Count)
            {
                Console.WriteLine("More Entries than Entries.");
                foreach (var entry in MapItemIcons.Entries.Where(entry => !ItemMiniIcons.Entries.ContainsKey(entry.Key)))
                {
                    Console.WriteLine($"{entry.Key} has no ItemMiniIcon.");
                    Console.WriteLine(entry.Value.Frames.Values.FirstOrDefault());
                }
            }
            if (MapItemIcons.Entries.Count < ItemMiniIcons.Entries.Count)
            {
                Console.WriteLine("More Entries than Entries.");
                foreach (var entry in ItemMiniIcons.Entries.Where(entry => !MapItemIcons.Entries.ContainsKey(entry.Key)))
                {
                    Console.WriteLine($"{entry.Key} has no MapItemIcon.");
                    Console.WriteLine(entry.Value.Frames.Values.FirstOrDefault());
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Content.GetType() != typeof (Home))
            {
                Content = new Home();
                e.Cancel = true;
            }
        }
    }
}
