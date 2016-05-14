using System.Windows;
using System.Windows.Controls;
using System.Text.RegularExpressions;
using NetComm;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media;

namespace mChat
{
    enum entityType
    {
        SERVER,
        SELF,
        OTHER
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Client c;
        static string name;
        public MainWindow()
        {
            InitializeComponent();
            c = new Client();
            c.Connected += new NetComm.Client.ConnectedEventHandler(client_Connected);
            c.Disconnected += new NetComm.Client.DisconnectedEventHandler(client_Disconnected);
            c.DataReceived += new NetComm.Client.DataReceivedEventHandler(client_DataReceived);
            SCG.General.MarkovNameGenerator rng = new SCG.General.MarkovNameGenerator(getnames(),5,4);
            name = rng.NextName;
            //Speeding up the connection
            c.SendBufferSize = 800;
            c.ReceiveBufferSize = 100;
            c.NoDelay = true;
            Closing += new System.ComponentModel.CancelEventHandler((sender, e) => OnWindowClosing(sender, e, c));

        }

        List<string> getnames()
        {
            StreamReader file = new StreamReader("C:\\Users\\Alec\\documents\\visual studio 2015\\Projects\\mChat\\mChat\\Names.txt");
            
            return new List<string> (file.ReadToEnd().Split(' '));
        }
        void OnWindowClosing(object sender,System.ComponentModel.CancelEventArgs e, Client f)
        {
            if (f.isConnected) f.Disconnect();
        }
        void client_Connected()
        {
            sendMsg("SERVER: Connected to host!", entityType.SERVER);
        }
        void client_Disconnected()
        {
            sendMsg("SERVER: Disconnected from host!",entityType.SERVER);
        }
        void client_DataReceived(byte[] Data, string ID)
        {
            
            sendMsg(ConvertBytesToString(Data),typeConverter(ConvertBytesToString(Data).Split(':')[0]));
            
        }

        static entityType typeConverter(string type)
        {
            if(type=="SERVER")
            {
                return entityType.SERVER;
            }
            if(type==name)
            {
                return entityType.SELF;
            }
            else
            {
                return entityType.OTHER;
            }
        }

        static string ConvertBytesToString(byte[] bytes)
        {
            return ASCIIEncoding.ASCII.GetString(bytes);
        }

        static byte[] ConvertStringToBytes(string str)
        {
            return ASCIIEncoding.ASCII.GetBytes(str);
        }


        private void button_Click(object sender, RoutedEventArgs e)
        {
            if (textBox.Text != "")
            {
                c.SendData(ConvertStringToBytes(textBox.Text));
                //sendMsg(textBox.Text, true);
                
            }
            
        }



        void sendMsg(string msg, entityType sender)
        {
            HorizontalAlignment align=HorizontalAlignment.Center;
            string bg= "#9C27B0";

            switch (sender)
            {
                case entityType.SELF:
                    align = HorizontalAlignment.Left;
                    bg = "#9C27B0";
                    break;
                case entityType.OTHER:
                    align = HorizontalAlignment.Right;
                    bg = "#2196F3";
                    break;
                case entityType.SERVER:
                    align = HorizontalAlignment.Center;
                    bg = "#D50000";
                    break;


            }
            
            var bc = new BrushConverter();
            string newString = msg.Substring(msg.IndexOf(":") + 2);
            if (newString.StartsWith("https://www.youtube.com") || newString.StartsWith("https://youtube.com"))
            {
                
                string ur = Regex.Match(newString, @"(?:youtube\.com\/(?:[^\/]+\/.+\/|(?:v|e(?:mbed)?)\/|.*[?&amp;]v=)|youtu\.be\/)([^""&amp;?\/ ]{11})").Groups[1].Value;

                WebBrowser b = new WebBrowser();

                b.HorizontalAlignment = HorizontalAlignment.Stretch;
                b.VerticalAlignment = VerticalAlignment.Stretch;


                
                listView.Items.Add(new Label() { Content = b, HorizontalAlignment = align, Background= (Brush)bc.ConvertFrom(bg) });

                b.Navigate("https://www.youtube.com/embed/" + ur + "?&html5=1");
                textBox.Text = "";

            }
            else {
                listView.Items.Add(new Label() { Content = msg, HorizontalAlignment=align, Background=(Brush) bc.ConvertFrom(bg)});
                textBox.Text = "";
            }
            
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            if(c.isConnected)
                c.Disconnect();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if (!c.isConnected)
                c.Connect("localhost", 2020, name);
        }

        private void listView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
