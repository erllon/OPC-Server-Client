// //ADIS SIN SERVER

// using System;
// using System.Net;
// using System.Net.Sockets;
// using System.Text;
// using System.Threading;

// namespace Server
// {
//     class Program
//     {
//         public static void Main()
//         {
//             Socket listeningSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

//             IPEndPoint serverEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9050);

//             listeningSocket.Bind(serverEP);
//             listeningSocket.Listen(10);

//             while (true)
//             {
//                 Console.WriteLine("Waiting for connection...");
//                 Socket kommSokkel = listeningSocket.Accept(); // blokkerende metode

               
//                 ThreadPool.QueueUserWorkItem(ClientThread, kommSokkel);
                
//             }
//             listeningSocket.Close();
//         } // av main

//         static void ClientThread(object o)
//         {
//             Socket comSocket = o as Socket;

//             IPEndPoint clientInfo = (IPEndPoint)comSocket.RemoteEndPoint;
//             IPEndPoint serverInfo = (IPEndPoint)comSocket.LocalEndPoint;

//             Console.WriteLine("{0} is connected at port {1}, server use {2} and port {3}",
//                                clientInfo.Address, 
//                                clientInfo.Port, 
//                                serverInfo.Address, 
//                                serverInfo.Port);

//             int recv;
//             string recText;
//             // string sentText;  Used in combination with the commented if-statement...
//             // The following three lines is not used...
//             // string hilsen = "Welcome to a simple test server";
//             // data = Encoding.ASCII.GetBytes(hilsen);
//             // comSocket.Send(data, data.Length, SocketFlags.None);

//             byte[] data = new byte[1024];

//             bool finished = false;
//             while (!finished)
//             {
//                 data = new byte[1024];
//                 recv = comSocket.Receive(data);
//                 if (recv == 0) finished = true;
//                 if (!finished)
//                 {
                    
//                     recText = Encoding.ASCII.GetString(data, 0, recv);
                    
//                     // if(recText.StartsWith('\0'))
//                     // {
//                     //     recText = recText.Substring(1);
//                     // }

//                     // string startSync = recText.Substring(0,6);
//                     // string packetNoString = recText.Substring(6,4);
//                     // int packetNo = Convert.ToInt32(packetNoString,16);//Int32.Parse(packetNoString,);
//                     // string type = recText[10].ToString();
//                     // string test = recText.Substring(11,4);
//                     // string messageSizeString = recText.Substring(11,5);
//                     // int messageSize = Convert.ToInt32(messageSizeString,16);
//                     // //string message = recText.Substring(16,messageSize);
//                     // string crc = recText.Substring((recv-1)-9,4);
//                     // string endSync = recText.Substring((recv-1)-6,6);


//                     // System.Console.WriteLine($"StartSync: {startSync}");
//                     // System.Console.WriteLine($"PacketNo: {packetNo}");
//                     // System.Console.WriteLine($"Type: {type}");
//                     // System.Console.WriteLine($"MessageSize: {messageSize}");
//                     // //System.Console.WriteLine($"Message: {message}");
//                     // System.Console.WriteLine($"Crc: {crc}");
//                     // System.Console.WriteLine($"EndSync: {endSync}");


//                     Console.WriteLine(recText);
//                     if (recText == "exit") finished = true;
//                     // This code reverses the message and sends it back to the client...if (!ferdig){Char[] recArray = recText.ToCharArray();Array.Reverse(recArray); StringBuilder sb = new StringBuilder();foreach (char ch in recArray)sb.Append(ch);sentText = sb.ToString();}

//                 }
//             }
//             Console.WriteLine("Connection with {0} is broken", clientInfo.Address);
//             comSocket.Close();
//         }
//     }
// }


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
            Socket lytteSokkel = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            IPEndPoint serverEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9050);

            lytteSokkel.Bind(serverEP);
            lytteSokkel.Listen(10);

            while (true)
            {
                Console.WriteLine("Venter på en klient ...");
                Socket kommSokkel = lytteSokkel.Accept(); // blokkerende metode

               
                ThreadPool.QueueUserWorkItem(KlientTraad, kommSokkel);
                
            }
            lytteSokkel.Close();
        } // av main

        static void KlientTraad(object o)
        {
            Socket kommSokkel = o as Socket;

            IPEndPoint klientInfo = (IPEndPoint)kommSokkel.RemoteEndPoint;
            IPEndPoint serverInfo = (IPEndPoint)kommSokkel.LocalEndPoint;

            Console.WriteLine("Har forbindelse med {0} på port {1}, server bruker {2} og port {3}",
                               klientInfo.Address, 
                               klientInfo.Port, 
                               serverInfo.Address, 
                               serverInfo.Port);

            int recv;
            string mottattTekst;
            string sendtTekst;
            byte[] data = new byte[1024];

            // string hilsen = "Velkommen til en enkel testserver";
            // data = Encoding.ASCII.GetBytes(hilsen);
            // kommSokkel.Send(data, data.Length, SocketFlags.None);

            bool ferdig = false;
            int i = 1;
            int startIndeks;
            int stopIndeks;
            string crcInMessageString;
            while (!ferdig)
            {
                data = new byte[kommSokkel.Available];
                recv = kommSokkel.Receive(data);
                //kommSokkel.Available;
                if (recv == 0) ferdig = true;
                if (!ferdig)
                {
                    mottattTekst = Encoding.ASCII.GetString(data, 0, recv);
                    char[] arr = mottattTekst.ToCharArray();
                    if(mottattTekst.Contains("{{++!!"))
                    {
                        startIndeks = mottattTekst.IndexOf("{{++!!");
                        if(mottattTekst.Contains("!!--}}"))
                        {
                            stopIndeks = mottattTekst.IndexOf("!!--}}");
                            //Console.WriteLine(mottattTekst);
                            if (mottattTekst == "avslutt") ferdig = true;
                            if (!ferdig)
                            {
                                // Char[] mottattArray = mottattTekst.ToCharArray();
                                // Array.Reverse(mottattArray);
                                // StringBuilder sb = new StringBuilder();
                                // foreach (char ch in mottattArray)
                                //     sb.Append(ch);
                                // sendtTekst = sb.ToString();
                                // kommSokkel.Send(Encoding.ASCII.GetBytes(sendtTekst),
                                //                 recv, SocketFlags.None);
                            }
                            Console.WriteLine(mottattTekst);
                            Console.WriteLine(mottattTekst[mottattTekst.Length-10]);
                            Console.WriteLine(mottattTekst[mottattTekst.Length-9]);
                            Console.WriteLine(mottattTekst[mottattTekst.Length-8]);
                            Console.WriteLine(mottattTekst[mottattTekst.Length-7]);         

                            crcInMessageString = GetCrcInMessage(mottattTekst);
                            int crcInMessageInt = Convert.ToInt32(crcInMessageString,16);
                            System.Console.WriteLine($"CRC in message: {crcInMessageInt}");

                            int count = stopIndeks - 10; //count for elements to read for generating CRC-16
                            int crcCalculated  = GenerateCrc(mottattTekst.ToCharArray(), 6, count);
                            //Reading char-array, starts at index: 6, reads count number of char-elements
                            Console.WriteLine($"Generated CRC: {crcCalculated.ToString()}");
                        }
                    }
                    
                }
                
                System.Console.WriteLine($"Iteration: {i}");
                i++;
                Thread.Sleep(2000);
                
            }
            Console.WriteLine("Forbindelsen med {0} er brutt", klientInfo.Address);
            kommSokkel.Close();
        }

        private static string GetCrcInMessage(string mottattTekst)
        {
            string returnString = mottattTekst.Substring(mottattTekst.Length-10,4);
            return returnString;
        }

        //buffer: what you want to read, offset: startindex for reading, count: char-elements to read
        private static int GenerateCrc(char[] buffer, int offset, int count)   
        {
            int[] ccitt_h = 
            {
                0x0000, 0x1081, 0x2102, 0x3183, 0x4204, 0x5285, 0x6306, 0x7387,
                0x8408, 0x9489, 0xa50a, 0xb58b, 0xc60c, 0xd68d, 0xe70e, 0xf78f
            };
            int[] ccitt_l = 
            {
                0x0000, 0x1189, 0x2312, 0x329b, 0x4624, 0x57ad, 0x6536, 0x74bf,
                0x8c48, 0x9dc1, 0xaf5a, 0xbed3, 0xca6c, 0xdbe5, 0xe97e, 0xf8f7
            };
            int crc = 0xFFFF;
            for (int i = offset; i < (offset + count); i++)
            {
                int n = buffer[i] ^ crc;
                crc = ccitt_l[n & 0x0f] ^ ccitt_h[(n >> 4) & 0x0f] ^ (crc >> 8);
            }
            return crc;
        }
    }
            
}
