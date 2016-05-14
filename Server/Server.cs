using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ServiceModel;
using Server;
using System.Net.Sockets;
using NetComm;
namespace Server
{
    class myServer
    {
        NetComm.Host serv;
        bool isRunning;
        
        public myServer()
        {
            serv = new Host(2020);
            serv.onConnection += new NetComm.Host.onConnectionEventHandler(Server_onConnection);
            serv.lostConnection += new NetComm.Host.lostConnectionEventHandler(Server_lostConnection);
            serv.DataReceived += new NetComm.Host.DataReceivedEventHandler(Server_DataReceived);
            //Speeding up the connection
            serv.SendBufferSize = 800;
            serv.ReceiveBufferSize = 100;
            serv.NoDelay = true;
            
        }

        private void start()
        {


            if (!isRunning)
            {
                serv.StartConnection();
                Console.WriteLine("Server Started...");
                isRunning = true;
            }
            else
            {
                Console.WriteLine("Server is already running...");
            }


        }
        private void stop()
        {
            if (isRunning)
            {
                serv.CloseConnection();
                Console.WriteLine("Server Stopped...");
                isRunning = false;
            }
            else
            {
                Console.WriteLine("Server is not running...");
            }

            
        }

        private void exit()
        {
            stop();
            Environment.Exit(0);
        }
        private void list()
        {
            Console.WriteLine("Currently Connected Users:");
            foreach(string u in serv.Users)
            {
                Console.WriteLine(u);
            }
            
        }

        public string GetLocalIPAddress()
        {
            var host = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("Local IP Address Not Found!");
        }

        static string ConvertBytesToString(byte[] bytes)
        {
            return ASCIIEncoding.ASCII.GetString(bytes);
        }

        static byte[] ConvertStringToBytes(string str)
        {
            return ASCIIEncoding.ASCII.GetBytes(str);
        }

        void Server_onConnection(string id)
        {
            Console.WriteLine(id + " connected!"
            ); //Updates the log textbox when new user joined
            serv.Brodcast(ConvertStringToBytes("SERVER: "+id + " connected!"));
        }
        void Server_lostConnection(string id)
        {
            Console.WriteLine(id + " disconnected!"
            ); //Updates the log textbox when new user joined
            serv.Brodcast(ConvertStringToBytes("SERVER: " + id + " disconnected!"));
        }
        void Server_DataReceived(string ID, byte[] Data)
        {
            Console.WriteLine(ID + ": " + ConvertBytesToString(Data));   //Updates the log when a new message arrived, 
                                                                         //converting the Data bytes to a string
            string newMsg = ID + ": " + ConvertBytesToString(Data);
            say(newMsg);
            
        }

        void say(string line)
        {
            serv.Brodcast(ConvertStringToBytes(line));
            
        }

        static void Main(string[] args)
        {
            myServer s = new myServer();
            string input;
            Console.WriteLine("Welcome!\nCurrent Time: " + DateTime.Now.ToShortTimeString() + "\nLocal Machine IP Address: "+s.GetLocalIPAddress());
            s.isRunning = false;
            s.start();

            do
            {
                Console.Write(">");
                input = Console.ReadLine();
                input.ToLower();
                input.Trim();

                if (input == "start")
                {
                    s.start();
                }
                else if(input=="list")
                {
                    s.list();
                }
                else if(input=="stop")
                {
                    s.stop();
                }
                else if (input.Split(' ')[0]=="say")
                {
                    s.say(input.TrimStart(new []{ 's','a','y',' '}));
                }
                else
                {
                    Console.WriteLine("Command not found!");
                }
                
            } while (input != "exit");
            s.exit();
        }
    }
}
