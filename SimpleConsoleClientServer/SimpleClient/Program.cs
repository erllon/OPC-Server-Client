// // // using System;  
// // // using System.Net;  
// // // using System.Net.Sockets;  
// // // using System.Text;
// // // using System.Xml;


// // // // namespace SimpleClient
// // // // {
// // // //     class Program
// // // //     {
// // // //         static void Main(string[] args)
// // // //         {
// // // //             string filePath = @"C:\VSCodePrograms\SimpleConsoleClientServer\SimpleClient\20190701T114435.xml";
// // // //             XmlDocument doc = new XmlDocument();
// // // //             doc.Load(filePath);

// // // //             //XmlNode node = doc.DocumentElement.SelectSingleNode("/Conductivity/Value");
// // // //             foreach(XmlNode node in doc.DocumentElement.ChildNodes)
// // // //             {
// // // //                 string text = node.InnerText; //or loop through its children as well
// // // //                 System.Console.WriteLine($"Node name: {node.Name}");
// // // //             }
// // // //             System.Console.Write("Trykk en tast for å fortsette...");
// // // //             Console.ReadKey();               
// // // //         }
// // // //     }
// // // // }


// // // // Client app is the one sending messages to a Server/listener.   
// // // // Both listener and client can send messages back and forth once a   
// // // // communication is established.  
// // // public class SocketClient  
// // // {  
// // //     public static int Main(String[] args)  
// // //     {  
// // //         StartClient();  
// // //         return 0;  
// // //     }  
  
  
// // //     public static void StartClient()  
// // //     {  
// // //         byte[] bytes = new byte[1024];  
// // //         char[] chars = new char[1024];
  
// // //         try  
// // //         {  
// // //             // Connect to a Remote server  
// // //             // Get Host IP Address that is used to establish a connection  
// // //             // In this case, we get one IP address of localhost that is IP : 127.0.0.1  
// // //             // If a host has multiple addresses, you will get a list of addresses  
// // //             //IPHostEntry host = Dns.GetHostEntry("localhost");  
// // //             IPAddress ipAddress = IPAddress.Parse("169.254.101.184"); //Static IP-address
// // //             //IPAddress ipAddress = host.AddressList[0];  
// // //             //IPEndPoint remoteEP = new IPEndPoint(ipAddress,61234);  
  
// // //             // Create a TCP/IP  socket.    
// // //             Socket sender = new Socket(ipAddress.AddressFamily,  
// // //                 SocketType.Stream, ProtocolType.Tcp);  


            
// // //             IPEndPoint ep = new IPEndPoint(ipAddress,61234);
// // //             sender.Bind(ep);
// // //             // Connect the socket to the remote endpoint. Catch any errors.    
// // //             try  
// // //             {
// // //                 sender.Listen(128);
// // //                 // Connect to Remote EndPoint  
// // //                 //sender.Connect(remoteEP);  
// // //                 System.Console.WriteLine("Trying to connect to " + sender.RemoteEndPoint.ToString());
  
// // //                 Console.WriteLine("Socket connected to {0}",  
// // //                                     sender.RemoteEndPoint.ToString());  
// // //                 byte[] messageReceived = new byte[1024];
// // //                 byte[] bytesStartSync = new byte[6];
// // //                 byte[] bytesPacketeNr = new byte[4];
// // //                 byte[] bytesType = new byte[1];
// // //                 byte[] bytesMessageSize = new byte[5];
// // //                 byte[] bytesInMessage = new byte[491];
// // //                 byte[] bytesCrc = new byte[4]; 
// // //                 byte[] bytesStopSync = new byte[6];  


// // //             // We receive the messagge using  
// // //             // the method Receive(). This  
// // //             // method returns number of bytes 
// // //             // received, that we'll use to  
// // //             // convert them to string 
// // //             // sender.Listen(12);
// // //             // sender.Accept();
// // //             int byteRecv = sender.Receive(messageReceived); 
// // //             Console.WriteLine("Message from Server -> {0}",  
// // //                   Encoding.ASCII.GetString(messageReceived,  
// // //                                              0, byteRecv));
            
// // //             string startSync = Encoding.ASCII.GetString(bytesStartSync);
// // //             System.Console.WriteLine("StartSync: " + startSync);
            
// // //             string PacketNr = Encoding.ASCII.GetString(bytesPacketeNr);
// // //             System.Console.WriteLine("PacketNr: " + PacketNr);

// // //             string type = Encoding.ASCII.GetString(bytesType);
// // //             System.Console.WriteLine("Type: " + type);

// // //             string messageSize = Encoding.ASCII.GetString(bytesMessageSize);
// // //             System.Console.WriteLine("MessageSize: " + messageSize);

// // //             string message = Encoding.UTF8.GetString(bytes);
// // //             System.Console.WriteLine("Message: " + message);

// // //             string crc = Encoding.ASCII.GetString(bytesCrc);
// // //             System.Console.WriteLine("Crc: " + crc);

// // //             string stopSync = Encoding.ASCII.GetString(bytesStopSync);
// // //             System.Console.WriteLine("StopSync: " + stopSync);

// // //             // string t = XmlDecode(Encoding.ASCII.GetString(messageReceived,0, byteRecv));
// // //             // int retCrc = GenerateCrc(chars,0,16);
// // //             // //int messageSize = 491;
// // //             // Console.WriteLine($" Melding: {Encoding.UTF8.GetString(messageReceived,11,491)}");
            
// // //             // Console.WriteLine($"XmlDecode: {t}");
// // //             // Console.WriteLine($"Crc: {retCrc}");

  
// // //             // Close Socket using  
// // //             // the method Close() 
// // //             sender.Shutdown(SocketShutdown.Both); 
// // //             sender.Close();
// // //             System.Console.WriteLine("Trykk en tast for å fortsette..."); 
// // //             Console.Read();
// // //                 // sender.Bind(remoteEP);
// // //                 // sender.Listen(123);
// // //                 // // Encode the data string into a byte array.    
// // //                 // byte[] msg = Encoding.ASCII.GetBytes("This is a test<EOF>");  
// // //                 // byte[] msge = Encoding.UTF8.GetBytes("This is a test<EOF>");  //ASCII.GetBytes("This is a test<EOF>");  

  
// // //                 // // Send the data through the socket.    
// // //                 // // int bytesSent = sender.Send(msge);  
  
                

// // //                 // // Receive the response from the remote device.    
// // //                 // int bytesRec = sender.Receive(bytes);  
// // //                 // Console.WriteLine("Echoed test = {0}",  
// // //                 //     Encoding.UTF8.GetString(bytes, 0, bytesRec));  
  
// // //                 // // Release the socket.    
// // //                 // sender.Shutdown(SocketShutdown.Both);  
// // //                 // sender.Close();  
// // //                 // Console.Read();
  
// // //             }  
// // //             catch (ArgumentNullException ane)  
// // //             {  
// // //                 Console.WriteLine("ArgumentNullException : {0}", ane.ToString());  
// // //             }  
// // //             catch (SocketException se)  
// // //             {  
// // //                 Console.WriteLine("SocketException : {0}", se.ToString());  
// // //             }  
// // //             catch (Exception e)  
// // //             {  
// // //                 Console.WriteLine("Unexpected exception : {0}", e.ToString());  
// // //             }  
  
// // //         }  
// // //         catch (Exception e)  
// // //         {  
// // //             Console.WriteLine(e.ToString());  
// // //         }  
// // //     } 
// // //     public static string XmlDecode(string value) 
// // //     {
// // //         var xmlDoc = new XmlDocument();
// // //         xmlDoc.LoadXml("<root>" + value + "</root>");
// // //         return xmlDoc.InnerText;        
// // //     } 
// // //     public static int GenerateCrc(char[] buffer, int offset, int count)
// // //     {
// // //         int[] ccitt_h = {
// // //         0x0000, 0x1081, 0x2102, 0x3183, 0x4204, 0x5285, 0x6306, 0x7387,
// // //         0x8408, 0x9489, 0xa50a, 0xb58b, 0xc60c, 0xd68d, 0xe70e, 0xf78f};
// // //         int[] ccitt_l = {
// // //         0x0000, 0x1189, 0x2312, 0x329b, 0x4624, 0x57ad, 0x6536, 0x74bf,
// // //         0x8c48, 0x9dc1, 0xaf5a, 0xbed3, 0xca6c, 0xdbe5, 0xe97e, 0xf8f7};
// // //         int crc = 0xFFFF;
// // //         for (int i = offset; i < (offset + count); i++)
// // //         {
// // //             int n = buffer[i] ^ crc;
// // //             crc = ccitt_l[n & 0x0f] ^ ccitt_h[(n >> 4) & 0x0f] ^ (crc >> 8);
// // //         }
// // //         return crc;
// // //     }
// // // } 

// // using System;  
// // using System.Net;  
// // using System.Net.Sockets;  
// // using System.Text;  
  
// // public class SynchronousSocketClient {  
  
// //     public static void StartClient() {  
// //         // Data buffer for incoming data.  
// //         byte[] bytes = new byte[1024];  
  
// //         // Connect to a remote device.  
// //         try {  
// //             // Establish the remote endpoint for the socket.  
// //             // This example uses port 11000 on the local computer.  
// //             IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());  
// //             IPAddress ipAddress = IPAddress.Parse("169.254.156.64");//ipHostInfo.AddressList[2];  
// //             IPEndPoint remoteEP = new IPEndPoint(ipAddress,11000);  
  
// //             // Create a TCP/IP  socket.  
// //             Socket sender = new Socket(ipAddress.AddressFamily,   
// //                 SocketType.Stream, ProtocolType.Tcp );  
  
// //             // Connect the socket to the remote endpoint. Catch any errors.  
// //             try {  
// //                 sender.Connect(remoteEP);  
  
// //                 Console.WriteLine("Socket connected to {0}",  
// //                     sender.RemoteEndPoint.ToString());  
  
// //                 // Encode the data string into a byte array.  
// //                 byte[] msg = Encoding.ASCII.GetBytes("This is a test<EOF>");  
  
// //                 // Send the data through the socket.  
// //                 int bytesSent = sender.Send(msg);  
  
// //                 // Receive the response from the remote device.  
// //                 int bytesRec = sender.Receive(bytes);  
// //                 Console.WriteLine("Echoed test = {0}",  
// //                     Encoding.ASCII.GetString(bytes,0,bytesRec));  
  
// //                 // Release the socket.  
// //                 sender.Shutdown(SocketShutdown.Both);  
// //                 sender.Close();  
  
// //             } catch (ArgumentNullException ane) {  
// //                 Console.WriteLine("ArgumentNullException : {0}",ane.ToString());  
// //             } catch (SocketException se) {  
// //                 Console.WriteLine("SocketException : {0}",se.ToString());  
// //             } catch (Exception e) {  
// //                 Console.WriteLine("Unexpected exception : {0}", e.ToString());  
// //             }  
  
// //         } catch (Exception e) {  
// //             Console.WriteLine( e.ToString());  
// //         }  
// //     }  
  
// //     public static int Main(String[] args) {  
// //         StartClient();  
// //         return 0;  
// //     }  
// // }  


using System;  
using System.Net;  
using System.Net.Sockets;  
using System.Text;  
  
public class SynchronousSocketClient {  
  
    public static void StartClient() {  
        // Data buffer for incoming data.  
        byte[] bytes = new byte[1024];  
  
        // Connect to a remote device.  
        try {  
            // Establish the remote endpoint for the socket.  
            // This example uses port 11000 on the local computer.  
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());  
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");//ipHostInfo.AddressList[0];
            IPEndPoint remoteEP = new IPEndPoint(ipAddress,11000);  
  
            // Create a TCP/IP  socket.  
            Socket sender = new Socket(ipAddress.AddressFamily,   
                SocketType.Stream, ProtocolType.Tcp );  
  
            // Connect the socket to the remote endpoint. Catch any errors.  
            try {  
                sender.Connect(remoteEP);  
  
                Console.WriteLine("Socket connected to {0}",  
                    sender.RemoteEndPoint.ToString());  
  
                // Encode the data string into a byte array.  
                byte[] msg = Encoding.ASCII.GetBytes("This is a test<EOF>");  
  
                // Send the data through the socket.  
                int bytesSent = sender.Send(msg);  
  
                // Receive the response from the remote device.  
                int bytesRec = sender.Receive(bytes);  
                Console.WriteLine("Echoed test = {0}",  
                    Encoding.ASCII.GetString(bytes,0,bytesRec));  
  
                // Release the socket.  
                sender.Shutdown(SocketShutdown.Both);  
                sender.Close();  
  
            } catch (ArgumentNullException ane) {  
                Console.WriteLine("ArgumentNullException : {0}",ane.ToString());  
            } catch (SocketException se) {  
                Console.WriteLine("SocketException : {0}",se.ToString());  
            } catch (Exception e) {  
                Console.WriteLine("Unexpected exception : {0}", e.ToString());  
            }  
  
        } catch (Exception e) {  
            Console.WriteLine( e.ToString());  
        }  
    }  
  
    public static int Main(String[] args) {  
        StartClient();  
        return 0;  
    }  
}  


//ADIS SIN KLIENT
// using System;
// using System.Net;
// using System.Net.Sockets;
// using System.Text;

// namespace Klient
// {
//     class Program
//     {
//         public static void Main()
//         {
//             byte[] data = new byte[1024];
//             string input, stringData;

//             Socket klientSokkel = new Socket(AddressFamily.InterNetwork,
//                                               SocketType.Stream,
//                                               ProtocolType.Tcp);
//             IPEndPoint serverEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9050);

//             try
//             {
//                 klientSokkel.Connect(serverEP);    // blokkerende metode
//             }
//             catch (SocketException e)
//             {
//                 Console.WriteLine("Fikk ikke forbindelse med server.");
//                 Console.WriteLine(e.ToString());
//                 return;
//             }

//             int nMottatt = klientSokkel.Receive(data);
//             stringData = Encoding.ASCII.GetString(data, 0, nMottatt);
//             Console.WriteLine(stringData);

//             bool ferdig = false;

//             while (!ferdig)
//             {
//                 input = Console.ReadLine();
//                 if (input == "avslutt") ferdig = true;

//                 klientSokkel.Send(Encoding.ASCII.GetBytes(input));

//                 if (!ferdig)
//                 {
//                     data = new byte[1024];
//                     nMottatt = klientSokkel.Receive(data);
//                     stringData = Encoding.ASCII.GetString(data, 0, nMottatt);
//                     Console.WriteLine(stringData);
//                 }
//             }

//             Console.WriteLine("Bryter forbindelsen med serveren ...");
//             klientSokkel.Shutdown(SocketShutdown.Both);
//             klientSokkel.Close();
//         }
//     }
// }
