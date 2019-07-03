//ADIS SIN SERVER

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Server
{
    class Program
    {
        public static void Main()
        {
            Socket listeningSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            IPEndPoint serverEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9050);

            listeningSocket.Bind(serverEP);
            listeningSocket.Listen(10);

            while (true)
            {
                Console.WriteLine("Waiting for connection...");
                Socket kommSokkel = listeningSocket.Accept(); // blokkerende metode

               
                ThreadPool.QueueUserWorkItem(ClientThread, kommSokkel);
                
            }
            listeningSocket.Close();
        } // av main

        static void ClientThread(object o)
        {
            Socket comSocket = o as Socket;

            IPEndPoint clientInfo = (IPEndPoint)comSocket.RemoteEndPoint;
            IPEndPoint serverInfo = (IPEndPoint)comSocket.LocalEndPoint;

            Console.WriteLine("{0} is connected at port {1}, server use {2} and port {3}",
                               clientInfo.Address, 
                               clientInfo.Port, 
                               serverInfo.Address, 
                               serverInfo.Port);

            int recv;
            string recText;
            string sentText;
            byte[] data = new byte[1024];

            string hilsen = "Welcome to a simple test server";
            data = Encoding.ASCII.GetBytes(hilsen);
            comSocket.Send(data, data.Length, SocketFlags.None);

            bool ferdig = false;
            while (!ferdig)
            {
                data = new byte[1024];
                recv = comSocket.Receive(data);
                if (recv == 0) ferdig = true;
                if (!ferdig)
                {
                    recText = Encoding.ASCII.GetString(data, 0, recv);
                    Console.WriteLine(recText);
                    if (recText == "exit") ferdig = true;
                    // This code reverses the message and sends it back to the client...if (!ferdig){Char[] recArray = recText.ToCharArray();Array.Reverse(recArray); StringBuilder sb = new StringBuilder();foreach (char ch in recArray)sb.Append(ch);sentText = sb.ToString();}
                    
                }
            }
            Console.WriteLine("Connection with {0} is broken", clientInfo.Address);
            comSocket.Close();
        }
    }
}
