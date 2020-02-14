using System;
using System.Windows.Forms;
using Yi.Database;
using Yi.Enums;
using Yi.Network;
using Yi.Scripting;
using Yi.SelfContainedSystems;

namespace Yi
{
    public partial class UserInterface : Form
    {
        public static UserInterface Reference;
        public bool ShuttingDown;
        public UserInterface()
        {
            InitializeComponent();
            Reference = this;
        }

        private async void UI_Load(object sender, EventArgs e)
        {
            Text = @"Yi Server - IP: " + YiCore.ServerIp;
            //await DbConverter.Convert();
            //await Db.SaveAsJsonAsync(SaveType.All);
            //await Database.Converters.MonsterDb.Load();
            //await Database.Converters.MagicTypeConverter.Load();
            //await Database.Converters.LevelExpConverter.Load();
            //Db.Serialize("Skills", Collections.Skills);
            //Db.Serialize("LevelExps", Collections.LevelExps);
            //Db.Serialize("BaseMonsters", Collections.BaseMonsters);
            //Db.Serialize("Monsters", Collections.Monsters);
            await Db.Load();
            BoothSystem.SetUpBooths();
            ScriptWatcher.Start();
            WeatherSystem.Start();
            //DayNightSystem.Start();

            //if (arg.Length > 0)
            //{
            //    Output.WriteLine("Running as 2nd Instance!");
            //    Servers.Start(5816);
            //}
            //else
            //{
            //    Output.WriteLine("Running as 1st Instance!");
            Servers.Start(9958);
            //    Process.Start("yi.exe", "secondInstance");
            //}

            PerformanceMonitor.Start();
        }

        private async void UI_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!ShuttingDown)
            {
                ShuttingDown = true;
                e.Cancel = true;

                await Db.SaveAsJsonAsync(SaveType.All);

                Environment.Exit(0);
            }
        }

        public void WriteLine(object text)
        {
            if (textBox1.InvokeRequired)
            {
                Invoke(new MethodInvoker(delegate
                {
                    WriteLine(text);
                }));
            }
            else
            {
                var oldText = textBox1.Text;
                textBox1.Text = text + Environment.NewLine + oldText;
            }
        }
    }
}
