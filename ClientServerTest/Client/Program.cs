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
    List<OpcNodeInfo> listOfNodes;
    using (var client = new OpcClient("opc.tcp://localhost:4840"))
    {
      client.Connect();

      while (true)//(!exitSession)
      {
        Console.WriteLine("Available commands: 'view' 'edit' 'disconnect'");
        Console.Write("Enter a command from the above list: ");
        stringEntered = Convert.ToString(Console.ReadLine());

        switch(stringEntered)
        {
          case "view":
          {
            Console.WriteLine("Enter the name(s) of the node(s) separated by whitespace(s):");
            string fullString = Console.ReadLine();
            List<OpcNodeInfo> nodeInfoList = AddNodesToList(fullString, client);
            Console.WriteLine("****************************");
            foreach(OpcNodeInfo infoElement in nodeInfoList)
            {
              //Start here!
            }
            Console.WriteLine("****************************");

            DisplayNodeInfo(nodeInfoList);
            break;
          }
          case "editValue":
          {
           //EditNodeValue();
            break;
          }          
          case "disconnect":
          {
            client.Disconnect();
            Console.WriteLine("Session is exiting...");
            exitSession = true;
            break;
          }            
          default:
          {
            Console.WriteLine(stringEntered + " is not an accepted command");
            break;
          }            
        }
        string[] splittedString = stringEntered.Split(" ");
        listOfNodes = new List<OpcNodeInfo>(); //or initialize it when declared and clear the list here?
        OpcNodeInfo machineNode;
        foreach(string substring in splittedString)
        {
          machineNode = client.BrowseNode($"ns=2;");
          // if(!machineNode.Name.IsNull)
          // {
            OpcNodeInfo jobnode = machineNode.Child("Job");
            listOfNodes.Add(machineNode);
          //}
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
          var nodeOfInterest = client.ReadNode($"ns=2;s={stringEntered}");
          if(nodeOfInterest.Value != null)
          {
            Console.Write($"The value of the node is: {nodeOfInterest.Value}\t");
            Console.WriteLine($"The ID of the node is: {nodeOfInterest.DataTypeId}\n");
          }
        }
        
        if(client.State == OpcClientState.Connected)
        {
          var temperature = client.ReadNode("ns=2;s=Temperature");
          var message = client.ReadNode("ns=2;s=Message");
          var level = client.ReadNode("ns=2;s=Level");
          Console.WriteLine($"Current Temperature is {temperature} °C");
          Console.WriteLine($"Current message is {message}");
          Console.WriteLine($"Level: {level}");
        }

        Thread.Sleep(1000);
      }
    }
  }

    private static void DisplayNodeInfo(List<OpcNodeInfo> nodeInfoList)
    {
        throw new NotImplementedException();
    }

    private static List<OpcNodeInfo> AddNodesToList(string nodesEntered, OpcClient opcClient)
    {
        string[] nodeArray = nodesEntered.Split(' ');
        List<OpcNodeInfo> returnList = new List<OpcNodeInfo>();
        OpcNodeInfo nodeInfo;
        foreach(string substring in nodeArray)
        {
          nodeInfo = opcClient.BrowseNode($"ns=2;s={substring}");
          if(!nodeInfo.Name.IsNull)
          {
            returnList.Add(nodeInfo);
          }
        }
        return returnList;
    }
}