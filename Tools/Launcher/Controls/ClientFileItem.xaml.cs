using System.Windows.Controls;

namespace Launcher.Controls
{
    /// <summary>
    /// Interaction logic for ClientFileItem.xaml
    /// </summary>
    public partial class ClientFileItem : UserControl
    {
        public string Url { get; set; }

        public ClientFileItem()
        {
            InitializeComponent();
        }
        public ClientFileItem(string url)
        {
            InitializeComponent();
            Url = url;
            SetText(url);
        }

        public void SetProgress(int progress)
        {
            ProgressBar.Value = progress;
        }
        public void SetTotalProgress(double progress)
        {
            ProgressBarTotal.Value = progress;
        }

        public void SetText(string text)
        {
            FileNameBlock.Text = text;
        }
    }
}
