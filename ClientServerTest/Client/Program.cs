using System;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using Opc.UaFx;
using Opc.UaFx.Client;
using Opc.UaFx.Services;
 
public class Program {
  public static async Task Main()
  {
    string stringEntered = "";
    bool exitSession= false;
    using (var client = new OpcClient("opc.tcp://localhost:4840"))
    {
      client.Connect();

      using(new Timer(UpdateLevel,client,TimeSpan.Zero,TimeSpan.FromSeconds(1)))
      while (!exitSession)
      {
        List<OpcNodeId> ids = new List<OpcNodeId>();
        List<OpcValue> values = new List<OpcValue>();
        List<OpcNodeId> listOfIds = new List<OpcNodeId>();

        //*En måte å lese og skrive ut nodene på*/
        //browseNodes(client, listOfIds);
        //readNodes(client,listOfIds);

        // client.SubscribeDataChange()
        // client.SubscribeDataChange(client.BrowseNode("Main/Level").NodeId, new OpcDataChangeReceivedEventHandler((sender,e) => OnDataChangedReceived(sender,e)));
        /*
          Dersom man først browser for å se hvilke noder som er tilgjengelig får vi en liste med OpcNodeID-er
          Så leser vi verdiene til hver av disse sub-nodene med client.ReadNode(OpcNodeID). Får da returnert verdien til den enkelte noden
          Må ha:
            En dictionary som med oversikt over alle tilgjengelige noder Key: OpcNodeID Value: OpcNodeInfo
            En dictionary som med oversikt over alle nodeverdiene:       Key: 
        */

        /*
        ************************************************************************
        ** Koden nedenfor går gjennom alle nodene og printer ut navn og verdi **
        ************************************************************************

        OpcNodeInfo mainNode = client.BrowseNode("2:Main");
        OpcValue valueRead = client.ReadNode("ns=2;s=Main/Level");

        Console.WriteLine($"ValueRead2: {valueRead.Value}");

        Console.WriteLine($"ValueRead: {valueRead.Value}");         
        foreach (var childNode in mainNode.Children())
        {
          valueRead = client.ReadNode(childNode.NodeId);
          client.SubscribeNode(childNode.NodeId);
          Console.WriteLine($"Name: {childNode.Name}\t\t\tValue: {valueRead}");
        } 
        */
        await CheckIfEntered();
        DisplayMenu();
        char menuChoice = Console.ReadKey().KeyChar;
        Console.WriteLine();
        switch(menuChoice)
        {
          case 'a':
          {
            Console.Write("Enter the name of the node you want to subscribe to: ");
            string entered = Console.ReadLine();
            OpcNodeInfo nodeInfo = client.BrowseNode($"ns=2;Main/{entered}");
            if(!nodeInfo.DisplayName.IsNull)//TODO: Se om dette er en god nok test
            {
              client.SubscribeDataChange(nodeInfo.NodeId,HandleDataChanged);
            }
            else
            {
              Console.WriteLine("The node entered does not exist");
            }
            break;
          }
          case 'b':
          {
            DisplaySubscribedNodes(client);
            break;
          }
          case 'c':
          {
            Console.Write("Enter name of node: ");
            try
            {
              string nodeName = Console.ReadLine();
              OpcNodeInfo info = client.BrowseNode($"ns=2;Main/{nodeName}");
              if(!info.DisplayName.IsNull)
              {
                Console.Write("Enter new value: ");
                try
                {
                  double newVal = Convert.ToDouble(Console.ReadLine());
                  Console.WriteLine($"\n\nOld value: {client.ReadNode(info.NodeId)}");              
                  client.WriteNode(info.NodeId,newVal);
                  Console.WriteLine($"New value: {client.ReadNode(info.NodeId)}\n\n");
                }
                catch
                {
                  Console.WriteLine("Invalid value entered!");
                }
                break;
              }
              else
              {
                Console.WriteLine("The node entered does not exist!");
              }
            }
            catch
            {
              Console.WriteLine("The node entered does not exist!");
            }
            break;
            
          }
          case 'd':
          {
            var node = client.BrowseNode(OpcObjectTypes.ObjectsFolder);
            browse(node);
            break;
           
          }    
          case 'e':
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
        Thread.Sleep(1000);
      }
    }
  }
   private static void browse(OpcNodeInfo node, int level = 0)
            {
                Console.WriteLine("{0}{1}({2})",
                        new string('.', level * 4),
                        node.Attribute(OpcAttribute.DisplayName).Value,
                        node.NodeId);
            
                level++;
            
                foreach (var childNode in node.Children())
                    browse(childNode, level);
            } 
  private static void UpdateLevel(object state)
    {
      var opcClient = (OpcClient)state;
      var mnode = opcClient.WriteNode("ns=2;s=Main/Level", (Convert.ToDouble(opcClient.ReadNode("ns=2;s=Main/Level").Value))+12);
      Thread.Sleep(1000);
    }
    private static void DisplayMenu()
    {
        Console.WriteLine("What do you want to do?");       
        Console.WriteLine("a) Subscribe to node");
        Console.WriteLine("b) Display subscribed nodes");
        Console.WriteLine("c) Write value of node");
        Console.WriteLine("d) browse all nodes in foldernodes");
        Console.WriteLine("e) Disconnect from server");
    }
    private static void HandleDataChanged(object sender, OpcDataChangeReceivedEventArgs e)
    {
        
        OpcMonitoredItem item = (OpcMonitoredItem)sender;
        Console.WriteLine($"Data Change from NodeId '{item.NodeId.Value}': {e.Item.Value}");
    }

    private static void OnSubscriptionsChanged(object sender, EventArgs e)
    {
        //Console.WriteLine("Hello from subscriptionchanged");
    }

    private static void OnDataChangedReceived(object sender, OpcDataChangeReceivedEventArgs e)
    {
        // Your code to execute on each data change.
        // The 'sender' variable contains the OpcMonitoredItem with the NodeId.
        var item = (OpcMonitoredItem)sender;
        Console.WriteLine($"Data Change from NodeId '{item.NodeId}': {e.Item.Value}");
    }

    //TODO: Lag dictionary med id som nøkkel og opcdatavariablenode som value
    //      Da skal det være lettere å gå gjennom alle nodene i idList
    private static void readNodes(OpcClient opcClient, List<OpcNodeId> idList)
    {
      List<OpcValue> valueList = opcClient.ReadNodes(idList).ToList();
      Console.Write("Value:");
      foreach(var value in valueList)
      {
        Console.Write($"\t{value.Value}");
      }
    }

    static async Task<bool> CheckIfEntered()
    {
      Console.WriteLine("Press a key to display the menu...");
      ConsoleKeyInfo key = Console.ReadKey();
      return true;
    }
    private static void browseNodes(OpcClient opcClient, List<OpcNodeId> idList)
    {      
      OpcNodeInfo mainInfo = opcClient.BrowseNode("ns=2;Main");
      Console.WriteLine("**********************");
      Console.WriteLine($"BROWSING NODE: {mainInfo.DisplayName}");
      Console.WriteLine("**********************");

      
      Console.WriteLine("\n\nPrinting all childnodes of " + mainInfo.DisplayName + ":");
      Console.Write("Name:");
      foreach(var subNode in mainInfo.Children())
      {
        Console.Write($"\t{subNode.Name}");
        
        idList.Add(subNode.NodeId);
      }
      Console.WriteLine();
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
    }

    private static void DisplayNodeInfo(List<OpcNodeInfo> nodeInfoList)
    {

    }

    private static void DisplaySubscribedNodes(OpcClient opcClient)
    {
        List<OpcNodeInfo> returnList = new List<OpcNodeInfo>();
        OpcNodeInfo nodeInfo;
        foreach(OpcSubscription subscription in opcClient.Subscriptions)
        {
          foreach(OpcMonitoredItem item in subscription.MonitoredItems)
          {
            System.Console.WriteLine($"NodeID: {item.ResolvedNodeId.Value}\t\t Value: {item.LastDataChange.Value}");
          }
        }        
    }
}