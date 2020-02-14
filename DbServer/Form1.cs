using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace DbServer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private TcpListener _listener;
        private void Form1_Load(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                try
                {
                    _listener = new TcpListener(IPAddress.Any, 9959);
                    _listener.Start();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                while (true)
                {
                    ReceiveTCP();
                    Thread.Sleep(1);
                }
            });
        }

        public void ReceiveTCP()
        {
            var recData = new byte[1024];

            for (;;)
            {
                try
                {
                    if (_listener.Pending())
                    {
                        var client = _listener.AcceptTcpClient();
                        var netstream = client.GetStream();

                        var compressed = new byte[0];
                        using (var ms = new MemoryStream())
                        {
                            int recBytes;
                            while ((recBytes = netstream.Read(recData, 0, recData.Length)) > 0)
                            {
                                ms.Write(recData, 0, recBytes);
                            }
                            compressed = ms.GetBuffer();
                        }

                        var decompressed = Content.QuickLz.Decompress(compressed, 0);

                        dynamic jsonObj = JsonConvert.DeserializeObject(Encoding.Default.GetString(decompressed));
                        var fileName = "";

                        if (jsonObj["AccountId"] != null)
                            fileName = ((string)jsonObj["Name"]).Replace("\0", "") + ".json";

                        using (var fs = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write))
                        {
                            fs.Write(decompressed, 0, decompressed.Length);
                        }
                        netstream.Close();
                        client.Close();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
