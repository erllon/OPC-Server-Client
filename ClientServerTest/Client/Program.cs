using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
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

      while (!exitSession)
      {
        List<OpcNodeId> ids = new List<OpcNodeId>();
        List<OpcValue> values = new List<OpcValue>();
        List<OpcBrowseNode> browseList = new List<OpcBrowseNode>();
        List<OpcNodeId> listOfIds = new List<OpcNodeId>();
        //List<OpcNodeInfo> infoList = client.BrowseNode("ns=2;Main");
        browseNodes(client, listOfIds);
        readNodes(client,listOfIds);
        
        //List<OpcNodeInfo> infoList = client.BrowseNodes(browseList);
        //Dictionary<OpcNodeId,Opcvariable>
        // client.BrowseNodes
        // OpcValue lestVerdi;
        //ids.Add("ns=2;s=Main/Level");
        //Console.WriteLine($"--------------------{ids[0].ValueAsString}-----------------");
        values = client.ReadNodes(listOfIds).ToList();
        
        //lestVerdi = client.ReadNode("2:");
        Console.WriteLine("\n\nWriting the content of values-list...");
        foreach(OpcValue value in values)
        {
          Console.WriteLine($"ID: {value.DataTypeId} Data type: {value.DataType} Value: {value.Value}");
        }
        Console.WriteLine("Finished writing the content of values-list...\n\n");

        OpcNodeInfo mainNode = client.BrowseNode("2:Main");

        OpcValue valueRead = client.ReadNode("ns=2;s=Main/Level");
        Console.WriteLine($"ValueRead1: {valueRead.Value}");
        
        client.WriteNode("ns=2;s=Main/Level",2000);
        valueRead = client.ReadNode("ns=2;s=Main/Level");

        Console.WriteLine($"ValueRead2: {valueRead.Value}");

        OpcValue readVal = client.ReadNode("Main/Level");

        Console.WriteLine($"ValueRead: {valueRead.Value}");
        Console.WriteLine($"ReadVal: {readVal.Value}");

        foreach (var childNode in mainNode.Children())
        {
          valueRead = client.ReadNode(childNode.NodeId);
          Console.WriteLine($"Name: {childNode.Name}\t\t\tValue: {valueRead}");
        }


        Console.WriteLine("Available commands: 'view' 'edit' 'disconnect'");
        Console.Write("Enter a command from the above list: ");
        stringEntered = Convert.ToString(Console.ReadLine());
        

        switch(stringEntered)
        {
          case "view":
          {
            Console.WriteLine("Enter the name(s) of the node(s) separated by whitespace(s):");
            string fullString = Console.ReadLine();
            List<OpcNodeInfo> nodeInfoList = FindValidNodes(fullString, client);
            Console.WriteLine("****************************");
            foreach(OpcNodeInfo infoElement in nodeInfoList)
            {
              Console.WriteLine($"ID: {infoElement.NodeId}\tName: {infoElement.Name}\tDisplay name: {infoElement.DisplayName}\tCategory: {infoElement.Category}\tContext: {infoElement.Context}\tReference: {infoElement.Reference}");
            }
            Console.WriteLine("****************************");

            DisplayNodeInfo(nodeInfoList);
            break;
          }
          case "editValue":
          {
            Console.WriteLine("Enter the name(s) of the node(s) separated by whitespace(s):");
            string editString = Console.ReadLine();
            // List<OpcNode> nodeInfoList = FindValidNodes(fullString, client);
            // EditNodeValue();
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
            client.WriteNode("ns=2;s=Main/Level",12);
            break;
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

    //TODO: Lag dictionary med id som nøkkel og opcdatavariablenode som value
    //      Da skal det være lettere å gå gjennom alle nodene i idList
    private static void readNodes(OpcClient opcClient, List<OpcNodeId> idList)
    {
      List<OpcValue> valueList = opcClient.ReadNodes(idList).ToList();
      Console.WriteLine("**********************");
      Console.WriteLine($"READING NODES");
      Console.WriteLine("**********************");
      Console.WriteLine("\nWriting values of all childnodes of " + valueList[0]);
      // foreach(var subNode in mainInfo.)
      // {
      //   Console.WriteLine($"NodeID: {subNode.NodeId}\t\tName: {subNode.Name}\t\tCategory: {subNode.Category}");
      // }
      // Console.WriteLine("Finished writing all childnodes of " + mainInfo.DisplayName);
    }

    private static void browseNodes(OpcClient opcClient, List<OpcNodeId> idList)
    {      
      OpcNodeInfo mainInfo = opcClient.BrowseNode("ns=2;Main");
      Console.WriteLine("**********************");
      Console.WriteLine($"BROWSING NODE: {mainInfo.DisplayName}");
      Console.WriteLine("**********************");

      
      Console.WriteLine("\n\nWriting all childnodes of " + mainInfo.DisplayName + ":");
      foreach(var subNode in mainInfo.Children())
      {
        Console.WriteLine($"NodeID: {subNode.NodeId}\t\tName: {subNode.Name}\t\tCategory: {subNode.Category}");
        idList.Add(subNode.NodeId);
      }
      Console.WriteLine("Finished writing all childnodes of " + mainInfo.DisplayName);
    }

    private static void EditNodeValue(string enteredString, OpcClient client)
    {
        string[] nodeArray = enteredString.Split(' ');
        List<OpcNodeInfo> returnList = new List<OpcNodeInfo>();
        OpcNodeInfo nodeInfo;
        foreach(string substring in nodeArray)
        {
          nodeInfo = client.BrowseNode($"ns=2;s={substring}");
          if(!nodeInfo.Name.IsNull)
          {
            returnList.Add(nodeInfo);
          }
        }
        //return returnList;|
    }

    private static void DisplayNodeInfo(List<OpcNodeInfo> nodeInfoList)
    {

    }

    private static List<OpcNodeInfo> FindValidNodes(string nodesEntered, OpcClient opcClient)
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