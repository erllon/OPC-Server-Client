using System;
using System.Threading;
using System.Collections.Generic;
 
using Opc.UaFx;
using Opc.UaFx.Client;
using Opc.UaFx.Services;
 
public class Program {
  public static void Main()
  {
    string stringEntered = "";
    bool exitSession= false;
    using (var client = new OpcClient("opc.tcp://localhost:4840"))
    {
      client.Connect();

      while (!exitSession)
      {
        //List<OpcNodeInfo> liste = client.BrowseNodes(new OpcBrowseNode())
        Console.Write("Enter the name of the node you want to see data from, or write \"exit\" to disconnect from server: ");
        stringEntered = Convert.ToString(Console.ReadLine());
        if(stringEntered == "exit")
        {
          client.Disconnect();
          Console.WriteLine("Session is exiting...");
          exitSession = true;
        }
        else if(stringEntered.Contains("edit"))
        {
          string[] splittedString = stringEntered.Split(" ");
          //OpcBrowseNode testNode;
          List<OpcReadNode> listOfCommands = new List<OpcReadNode>();
          foreach(string substring in splittedString)
          {
            listOfCommands.Add(new OpcReadNode($"ns=2;s={substring}"));
            // testNode = new OpcBrowseNode($"s={substring}");
            // Console.WriteLine("\nWRITING INFO ABOUT TESTNODE...\n");
            // Console.WriteLine("TimeStamp: " + testNode.View.Timestamp);
            // Console.WriteLine("ViewID: " + testNode.View.ViewId);
            // Console.WriteLine("Viewversion: " + testNode.View.ViewVersion);

            // if(testNode!= null)
            // {
            //   break;
            // }
          }
          OpcBrowseNode test = new OpcBrowseNode("s=Message");
          IEnumerable<OpcNodeInfo> infoAboutNodes = client.BrowseNodes(test);
          foreach(string element in splittedString)
          {
            List<OpcReadNode> liste = new List<OpcReadNode>();
            liste.Add(new OpcReadNode("s=Message"));
            //OpcNodeInfo info = client.BrowseNode($"ns=2;s=Message");
            List<OpcBrowseNode> liste2 = new List<OpcBrowseNode>();
            //OpcBrowseNode test = new OpcBrowseNode("s=Message");
            IEnumerable<OpcNodeInfo> info = client.BrowseNodes(test);
            Console.WriteLine("\n**********************************");
            Console.WriteLine("Writing the infoElements...");
            Console.WriteLine("**********************************\n");

            foreach(OpcNodeInfo infoElement in info)
            {
              Console.WriteLine("NodeID: " + infoElement.NodeId);
              Console.WriteLine("InfoElement:: " + infoElement.ToString() + "\n");

            }
          }
          var nodeOfInterest = client.ReadNode($"ns=2;s={stringEntered}");
          if(nodeOfInterest.Value != null)
          {
            Console.Write($"The value of the node is: {nodeOfInterest.Value}\t");
            Console.WriteLine($"The ID of the node is: {nodeOfInterest.DataTypeId}\n");
          }
        }
        else
        {
          var nodeOfInterest = client.ReadNode($"ns=2;s={stringEntered}");
          if(nodeOfInterest.Value != null)
          {
            Console.Write($"The value of the node is: {nodeOfInterest.Value}\t");
            Console.WriteLine($"The ID of the node is: {nodeOfInterest.DataTypeId}\n");
          }
          else
          {
            Console.WriteLine("The node you entered does not exist!\n");
          }
        }
        

        
        
        // var temperature = client.ReadNode("ns=2;s=Temperature");
        // var message = client.ReadNode("ns=2;s=Message");
        // var level = client.ReadNode("ns=2;s=Level");
        // Console.WriteLine($"Current Temperature is {temperature} °C");
        // Console.WriteLine($"Current message is {message}");
        // Console.WriteLine($"Level: {level}");
        
        Thread.Sleep(1000);
      }
    }
  }
}