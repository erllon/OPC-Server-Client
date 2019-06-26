using System;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using Opc.UaFx;
using Opc.UaFx.Client;
using System.IO;
public class Program 
{
   static string defaultPath = @"C:\VSCodePrograms\ClientServerTest\Client";
   static Mutex mutexForNodeVar = new Mutex();
  public static void Main()
  {
    string stringEntered = "";
    bool exitSession= false;
    using (var client = new OpcClient("opc.tcp://localhost:4840"))
    {
      client.Connecting += new EventHandler((sendere,e)=>OnConnecting(sendere,e));
      client.Connected += new EventHandler((sender,e)=>OnConnected(sender,e));
      client.Disconnected += new EventHandler((sender,e)=>OnDisconnected(sender,e));
      client.Disconnecting += new EventHandler((sender,e)=>OnDisconnecting(sender,e));

      client.Connect();
      LicenseInfo license = Opc.UaFx.Client.Licenser.LicenseInfo;
 
      if (license.IsExpired)
      {
        Console.WriteLine("The OPA UA Framework Advanced license is expired!");
      }
      Console.WriteLine($"License: {license.ToString()}\n");

      ThreadPool.QueueUserWorkItem(UpdateLevel,client);
      while(!exitSession)
      {
        while (client.State == OpcClientState.Connected)
        {
          List<OpcNodeId> ids = new List<OpcNodeId>();
          List<OpcValue> values = new List<OpcValue>();
          List<OpcNodeId> listOfIds = new List<OpcNodeId>();
          Dictionary<OpcNodeId,OpcNodeIdFormat> dict = new Dictionary<OpcNodeId, OpcNodeIdFormat>();
          
          DisplayMenu();
          char menuChoice = Console.ReadKey().KeyChar;
          Console.WriteLine();
          switch(menuChoice)
          {
            case 'a':
            {
              var node = client.BrowseNode(OpcObjectTypes.ObjectsFolder);

              OpcNodeId nodeIdSubscr = findNodeToSubscribe(node);
              if(nodeIdSubscr != null)
              {
                try
                {
                  OpcNodeInfo nodeInfo = client.BrowseNode($"2:{nodeIdSubscr.ValueAsString}");
                  if(!nodeInfo.DisplayName.IsNull)//TODO: Se om dette er en god nok test
                  {
                    client.SubscribeDataChange(nodeInfo.NodeId, HandleDataChanged);
                    Console.WriteLine($"Subscribed to {nodeInfo.NodeId.Value}\n");
                  }
                  else
                  {
                    Console.WriteLine("The node entered does not exist");
                  }
                }
                catch(Exception ex)
                {
                  Console.WriteLine($"ERROR: {ex.Message}");                
                }              
              }
              else
              {
                Console.WriteLine("Subscription aborted...");
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
                string enteredNodeName = Console.ReadLine().ToLower().Trim();
                string nodeName = char.ToUpper(enteredNodeName[0]) + enteredNodeName.Substring(1);
                OpcNodeInfo info = client.BrowseNode($"ns=2;Main/{nodeName}");
                if(!info.DisplayName.IsNull)
                {
                  try
                  {
                    mutexForNodeVar.WaitOne();
                    var nodeToChange = client.ReadNode(info.NodeId);
                    mutexForNodeVar.ReleaseMutex();
                    Console.Write("Enter new value: ");
                    string newValueEntered = Console.ReadLine().ToLower().Trim();
                    mutexForNodeVar.WaitOne();
                    Console.WriteLine($"\n\nOld value: {client.ReadNode(info.NodeId)}");
                    mutexForNodeVar.ReleaseMutex();
                    switch(nodeToChange.DataType)
                    {
                      case OpcDataType.Boolean:
                      {
                        if(newValueEntered == "1" || newValueEntered =="true")
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
                        double newValDouble = Convert.ToDouble(newValueEntered, System.Globalization.CultureInfo.InvariantCulture);
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
                    mutexForNodeVar.WaitOne();
                    Console.WriteLine($"New value: {client.ReadNode(info.NodeId)}\n\n");
                    mutexForNodeVar.ReleaseMutex();
                  }
                  catch(Exception ex)
                  {
                    Console.WriteLine($"Exception: {ex.Message}!");
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
              foreach(OpcSubscription subscription in client.Subscriptions)
              {
                foreach(OpcMonitoredItem item in subscription.MonitoredItems)
                {
                  mutexForNodeVar.WaitOne();
                  var currentNode = client.ReadNode(item.NodeId);
                  var valueOfNode = currentNode.Value;
                  Console.WriteLine($"Old value: {client.ReadNode(item.NodeId)}");
                  mutexForNodeVar.ReleaseMutex();

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
                  mutexForNodeVar.WaitOne();
                  Console.WriteLine($"New value: {client.ReadNode(item.NodeId)}");
                  mutexForNodeVar.ReleaseMutex();

                  Console.WriteLine($"NodeID: {item.NodeId}\t\t Value: {item.LastDataChange.Value}");
                }
              } 
              break;
            }
            case 'e':
            {
              var node = client.BrowseNode(OpcObjectTypes.ObjectsFolder);
              browse(node);
              break;              
            }    
            case 'f':
            {
              client.Disconnect();
              Console.WriteLine("Session is exiting...");

              //exitSession = true;
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
        Console.Write("Enter 'reconnect' to reconnect to server, og 'exit' to exit: ");
        string input = Console.ReadLine().ToLower().Trim();
        if(input == "reconnect")
        {
          client.Connect();
        }
        else if(input == "exit")
        {
          System.Console.WriteLine("Exiting...");
          exitSession = true;
        }
        else
        {
          System.Console.WriteLine("You entered an invalid command...");
        }
      }
      System.Console.WriteLine("Exited...");
    }      
  }
    private static void OnDisconnecting(object sender, EventArgs e)
    {
        Console.WriteLine("Disconnecting from server...");
    }
    private static void OnDisconnected(object sender, EventArgs e)
    {
        Console.WriteLine("Disconnected...");
    }
    private static void OnConnected(object sender, EventArgs e)
    {
      var opcClient = (OpcClient)sender;
      Console.WriteLine($"Connected to: {opcClient.ServerAddress}");
    }
    private static void OnConnecting(object sendere, EventArgs e)
    {
        Console.WriteLine("Connecting to server...");
    }
    private static void UpdateLevel(object state)
    {
      var opcClient = (OpcClient)state;

      while(opcClient.State == OpcClientState.Connected) //var if frem til 25.06
      {
        try
        {
          mutexForNodeVar.WaitOne();
          var mnode = opcClient.WriteNode("ns=2;s=Main/Level", (Convert.ToDouble(opcClient.ReadNode("ns=2;s=Main/Level").Value)) + 3);
          mutexForNodeVar.ReleaseMutex();
        }
        catch(Exception ex)
        {
          Console.WriteLine($"ERROR: {ex.Message}");
        }
        Thread.Sleep(1000);
      }
    }
    private static void browse(OpcNodeInfo node, int level = 0)   
    {
      Console.WriteLine("{0}{1}({2})", new string('.', level * 4), node.Attribute(OpcAttribute.DisplayName).Value,node.NodeId);
      Console.WriteLine("*************");
      Console.WriteLine($"{node.NodeId}");
      Console.WriteLine("*************");

      level++;
  
      foreach (var childNode in node.Children())
          browse(childNode, level);
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
        //Console.WriteLine("Hello from HandleDAtaChanged");      
        OpcMonitoredItem item = (OpcMonitoredItem)sender;
        WriteChangesToFile(item, e, defaultPath);
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
        Console.WriteLine("Hello from subscriptionchanged");
    }

    private static void OnDataChangedReceived(object sender, OpcDataChangeReceivedEventArgs e)
    {
        var item = (OpcMonitoredItem)sender;
        Console.WriteLine($"Data Change from NodeId '{item.NodeId}': {e.Item.Value}");
    }
    private static void readNodes(OpcClient opcClient, List<OpcNodeId> idList)
    {
      mutexForNodeVar.WaitOne();
      List<OpcValue> valueList = opcClient.ReadNodes(idList).ToList();
      mutexForNodeVar.ReleaseMutex();

      Console.Write("Value:");
      foreach(var value in valueList)
      {
        Console.Write($"\t{value.Value}");
      }
    }    
    private static OpcNodeId findNodeToSubscribe(OpcNodeInfo node)//(OpcClient opcClient)//, List<OpcNodeId> idList)
    {
      OpcNodeId nodeId;
      PrintNodes(node);
      Console.WriteLine();
      Console.Write("Enter the letter assosiated with the node of interest: ");
      char enteredChar = Console.ReadKey().KeyChar;
      int enteredInt = (int)(enteredChar) - 97;
      Console.WriteLine();
      if (Math.Abs(enteredInt + 1) <= node.Children().Count())
      {
        nodeId = node.Children().ElementAt(enteredInt).NodeId;

        OpcNodeInfo enteredNode = node.Children().ElementAt(enteredInt);
        var ds = enteredNode.Children();
        if (enteredNode.Children().Count() != 0)
        {
          nodeId = findNodeToSubscribe(enteredNode);
        }
        else
        {
          Console.WriteLine($"Do you want to subscribe to: {enteredNode.NodeId.ValueAsString}? (y/n)");
          char choice = Console.ReadKey().KeyChar;
          Console.WriteLine();
          if(choice == 'y')
          {
            return nodeId;
          }
          else
          {
            return null;
          }          
        }
        return nodeId;
      }
      else
      {
        Console.WriteLine("You have entered an invalid value...");
        return null;
      }
    }

    private static void PrintNodes(OpcNodeInfo node)
    {
        Console.WriteLine("Possible nodes:");
        for (int i = 0; i < node.Children().Count(); i++)
        {
            Console.Write($"{(char)('a' + i)}. {node.Children().ElementAt(i).DisplayName}");
            if (i+1 >= 4 && (i+1) % 4 == 0)
            {
                Console.WriteLine();
            }
            else
            {
                Console.Write("\t");
            }
        }
    }

    private static void DisplaySubscribedNodes(OpcClient opcClient)
    {
        List<OpcNodeInfo> returnList = new List<OpcNodeInfo>();
        foreach(OpcSubscription subscription in opcClient.Subscriptions)
        {
          foreach(OpcMonitoredItem item in subscription.MonitoredItems)
          {
            Console.WriteLine($"NodeID: {item.ResolvedNodeId.Value}\t\t Readnode-value: {opcClient.ReadNode(item.NodeId)} Time: {opcClient.ReadNode(item.NodeId).ServerTimestamp.Value.ToLocalTime()}");           
          }
        }        
    }
}