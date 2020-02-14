using System;
using System.Windows.Forms;

namespace Yi
{
    public static class Program
    {
        [STAThread]
        private static void Main(string[] arg)
        {
            Application.Run(new UserInterface());
        }
    }
}