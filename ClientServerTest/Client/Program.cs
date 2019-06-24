using System;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using Opc.UaFx;
using Opc.UaFx.Client;
using System.IO;
using Opc.UaFx.Services;
 
public class Program 
{
   static string defaultPath = @"C:\VSCodePrograms\ClientServerTest\Client";
  public static async Task Main()
  {
    string stringEntered = "";
    bool exitSession= false;
    using (var client = new OpcClient("opc.tcp://localhost:4840"))
    {
      client.Connect();
      LicenseInfo license = Opc.UaFx.Client.Licenser.LicenseInfo;
 
      if (license.IsExpired)
        Console.WriteLine("The OPA UA Framework Advanced license is expired!");
      Console.WriteLine($"License: {license.ToString()}\n");

      //using(new Timer(UpdateLevel,client,TimeSpan.Zero,TimeSpan.FromSeconds(1)))
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
        //await CheckIfEntered();
        DisplayMenu();
        char menuChoice = Console.ReadKey().KeyChar;
        Console.WriteLine();
        switch(menuChoice)
        {
          case 'a':
          {
            Console.Write("Enter the name of the node you want to subscribe to: ");
            string entered = Console.ReadLine();
            try
            {
              OpcNodeInfo nodeInfo = client.BrowseNode($"ns=2;Main/{entered}");
              if(!nodeInfo.DisplayName.IsNull)//TODO: Se om dette er en god nok test
              {
                client.SubscribeDataChange(nodeInfo.NodeId,HandleDataChanged);
                Console.WriteLine($"Subscribed to {nodeInfo.NodeId.Value}\n");
              }
              else
              {
                Console.WriteLine("The node entered does not exist");
              }
            }
            catch
            {
                Console.WriteLine("Catch...");
              
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
                try
                {
                  var nodeToChange = client.ReadNode(info.NodeId);
                  Console.Write("Enter new value: ");
                  string newValueEntered = Console.ReadLine();
                  Console.WriteLine($"\n\nOld value: {client.ReadNode(info.NodeId)}");
                  switch(nodeToChange.DataType)
                  {
                    case OpcDataType.Boolean:
                    {
                      if(newValueEntered == "1" || newValueEntered.ToUpper()=="TRUE")
                      {
                        client.WriteNode(info.NodeId,true);
                      }
                      else
                      {
                        client.WriteNode(info.NodeId,false);
                      }
                      break;                        
                    }
                    case OpcDataType.Double:
                    {
                      double newValDouble = Convert.ToDouble(newValueEntered);
                      client.WriteNode(info.NodeId,newValDouble);
                      break;
                    }                    
                    case OpcDataType.Integer:
                    {
                      double newValInteger = Convert.ToInt32(newValueEntered);
                      client.WriteNode(info.NodeId,newValInteger);
                      break;
                    }
                    case OpcDataType.String:
                    {
                      client.WriteNode(info.NodeId,newValueEntered);
                      break;
                    }
                  }
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
            List<OpcNodeInfo> returnList = new List<OpcNodeInfo>();
            OpcNodeInfo nodeInfo;
            foreach(OpcSubscription subscription in client.Subscriptions)
            {
            foreach(OpcMonitoredItem item in subscription.MonitoredItems)
            {
              var currentNode = client.ReadNode(item.NodeId);
              var valueOfNode = currentNode.Value;// .DataTypeId.Value;
              Console.WriteLine($"Old value: {client.ReadNode(item.NodeId)}");

              switch(currentNode.DataType)
              {
                case OpcDataType.Boolean:
                {
                  if(Convert.ToBoolean(valueOfNode) == true)
                  {
                    client.WriteNode(item.NodeId, false); 
                  }
                  else
                  {
                    client.WriteNode(item.NodeId, true);                    
                  }

                  break;                        
                }
                case OpcDataType.Double:
                {
                  double value = Convert.ToDouble(valueOfNode);
                  client.WriteNode(item.NodeId, value + 1);     
                  break;
                }                    
                case OpcDataType.Integer:
                {
                  int intValue = Convert.ToInt32(valueOfNode) + 1;
                  client.WriteNode(item.NodeId,intValue);              
                  break;
                }
                case OpcDataType.String:
                {
                  string stringValue = valueOfNode.ToString() + 'a';
                  client.WriteNode(item.NodeId,stringValue);
                  break;
                }
              }
              //client.WriteNode(item.NodeId);
              Console.WriteLine($"New value: {client.ReadNode(item.NodeId)}");

              Console.WriteLine($"NodeID: {item.NodeId}\t\t Value: {item.LastDataChange.Value}");
            }
          }        

            break;
          }
          case 'e':
          {
            var node = client.BrowseNode(OpcObjectTypes.ObjectsFolder);
            //var node = client.BrowseNode(OpcObjectTypes.DataTypesFolder);
            browse(node);
            break;
           
          }    
          case 'f':
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
        Console.WriteLine("*************");
        Console.WriteLine($"{node.NodeId}");
        Console.WriteLine("*************");

    
        level++;
    
        foreach (var childNode in node.Children())
            browse(childNode, level);
  } 
  private static void UpdateLevel(object state)
  {
    var opcClient = (OpcClient)state;
    if(opcClient.State == OpcClientState.Connected)
    {
      try
      {
        var mnode = opcClient.WriteNode("ns=2;s=Main/Level", (Convert.ToDouble(opcClient.ReadNode("ns=2;s=Main/Level").Value))+12);
      }
      catch(Exception ex)
      {
        Console.WriteLine($"ERROR: {ex.Message}");
      }

    }
    Thread.Sleep(1000);
  }
    private static void DisplayMenu()
    {
      Console.WriteLine("Press a key to display the menu...");
      ConsoleKeyInfo key = Console.ReadKey();
      Console.WriteLine("\nWhat do you want to do?");       
      Console.WriteLine("a) Subscribe to node");
      Console.WriteLine("b) Display subscribed nodes");
      Console.WriteLine("c) Write value of node");
      Console.WriteLine("d) Increment all subscribed nodes by 1");
      Console.WriteLine("e) Browse all nodes in foldernodes");
      Console.WriteLine("f) Disconnect from server");
        
    }
    private static void HandleDataChanged(object sender, OpcDataChangeReceivedEventArgs e)
    {
        
        OpcMonitoredItem item = (OpcMonitoredItem)sender;
        WriteChangesToFile(item, e, defaultPath);
        //Console.WriteLine($"Data Change from NodeId '{item.NodeId.Value}': {e.Item.Value}");
    }

    private static void WriteChangesToFile(OpcMonitoredItem item, OpcDataChangeReceivedEventArgs e, string folderPath)
    {
      string formatedNodeName = FormatNodeName(item.NodeId.Value.ToString());
      string filePath = $@"{folderPath}\{formatedNodeName}_log.txt";
      string text = $"{e.MonitoredItem.LastNotification.PublishTime.Value.ToLocalTime()} Data Change from NodeId '{item.NodeId.Value}': {e.Item.Value}";
      if(!File.Exists(filePath))
      {
        using(StreamWriter sw = new StreamWriter(filePath))
        {
          sw.WriteLine(text);
        }
      }
      else
      {
        using(StreamWriter sw = File.AppendText(filePath))
        {
          sw.WriteLine(text);
        }
      }
    }

    private static string FormatNodeName(string completeNodeName)
    {
        string[] splittedStrings = completeNodeName.Split('/');
        string returnString = "";
        foreach(string name in splittedStrings)
        {
          returnString += $@"{name}_";
        }
        return returnString;
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
      Console.Write("Enter the name of the node you want to browse: ");
      string nodeOfInterest = Console.ReadLine();

      OpcNodeInfo mainInfo = opcClient.BrowseNode("ns=2;"+nodeOfInterest);//("ns=2;Main");
      Console.WriteLine("\n**********************");
      Console.WriteLine($"BROWSING NODE: {mainInfo.DisplayName}");
      Console.WriteLine("**********************");

      
      Console.WriteLine("Printing all childnodes of " + mainInfo.DisplayName + ":");
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
            Console.WriteLine($"NodeID: {item.ResolvedNodeId.Value}\t\t Value: {item.LastDataChange.Value}");
          }
        }        
    }
}