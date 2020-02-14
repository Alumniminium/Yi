using System.Windows;

namespace Launcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow This;
        public MainWindow()
        {
            InitializeComponent();
            This = this;
        }
    }
}
