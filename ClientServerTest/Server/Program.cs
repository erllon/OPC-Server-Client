﻿using System.Threading;
using System;
using Opc.UaFx;
using Opc.UaFx.Server;
using System.Collections.Generic;
using System.Linq;

class Program
{
  static OpcFolderNode mainNode;
  static OpcDataVariableNode<double> mainTemperatureNode;
  static int connectedClients;
  public static void Main()
  {
    
    connectedClients = 0;
    List<OpcDataVariableNode> nodeList = new List<OpcDataVariableNode>();
    var temperatureNode = new OpcDataVariableNode<double>("Temperature", 100.0);
    var messageNode = new OpcDataVariableNode<string>("Message", string.Empty);
    var levelDetectionNode = new OpcDataVariableNode<bool>("Level", false);

    mainNode = new OpcFolderNode("Main");
    var mainIsRunningNode = new OpcDataVariableNode<bool>(mainNode, "IsRunning");
    var mainMessageNode = new OpcDataVariableNode<string>(mainNode,"Message");
    mainTemperatureNode = new OpcDataVariableNode<double>(mainNode,"Temperature");
    var mainAlarmNode = new OpcDataVariableNode<bool>(mainNode,"Alarm");
    var mainLevelNode = new OpcDataVariableNode<double>(mainNode,"Level");
    
    

    bool avslutt= false;

    nodeList.Add(levelDetectionNode);
    nodeList.Add(temperatureNode);
    nodeList.Add(messageNode); 
    MyNodeManager nodeManager = new MyNodeManager();
    nodeManager.MonitoredItemsModified += new OpcMonitoredItemsEventHandler((sender,e) => blablametode(sender,e));
    //nodeManager.CreateNodes(new OpcNodeReferenceCollection());
    //using (var server = new OpcServer("opc.tcp://localhost:4840/", nodeList)) 
    //using (var server = new OpcServer("opc.tcp://localhost:4840/", mainNode))
    using(var server = new OpcServer("opc.tcp://localhost:4840/", nodeManager))
    {   
      server.Started += new EventHandler((sender, e) => ServerStarted(sender, e, nodeList, server));
      // server.RequestProcessing += new OpcRequestProcessingEventHandler((a, b) => reqProcessing(a,b));
      //server.RequestProcessed += new OpcRequestProcessedEventHandler((sender, e) => RequestProcessed(sender, e));
      server.SessionActivated += new OpcSessionEventHandler((sender, e) => SessionMethod(sender,e));
      server.SessionCreated += new OpcSessionEventHandler((serr,rerrr)=>sessionCreatedMethod(serr,rerrr));
      server.SessionClosing += new OpcSessionEventHandler((sender, e) => SessionClosingMethod(sender,e));

      
      server.Start();
      foreach(var node in nodeManager.Nodes)
      {
      }
      Console.WriteLine("The server is running...");
      using(new Timer(UpdateTemperture,server,TimeSpan.Zero,TimeSpan.FromSeconds(1)))
      //Console.ReadKey();
      //var testVar = nodeManager.Nodes.Contains("ns=2;s=Level");
      while (true)
      {
                
        if (temperatureNode.Value == 110)
            temperatureNode.Value = 100;
        else
            temperatureNode.Value++;

        if(messageNode.Value.Length > 5)
          messageNode.Value = "e";
        else
          messageNode.Value += 'a';
        if(levelDetectionNode.Value == true)
          levelDetectionNode.Value = false;
        else
          levelDetectionNode.Value = true;

        Thread.Sleep(1000);
      }
    }
  }

    private static void sdlkad(object s, OpcNodeAccessEventArgs e)
    {
        throw new NotImplementedException();
    }

    private static void blablametode(object sender, OpcMonitoredItemsEventArgs e)
    {
        Console.WriteLine("En node-verdi i samlingen har blitt endret!");
    }

    private static void UpdateTemperture(object state)
    {
      var server = (OpcServer)state;
      var mnode = mainNode;

      mainTemperatureNode.Value = DateTime.Now.Second/3.0;
      mainTemperatureNode.ApplyChanges(server.SystemContext);
      Console.WriteLine("Temperature is: " + mainTemperatureNode.Value.ToString("N2"));
      Thread.Sleep(1000);
    }

    private static void sessionCreatedMethod(object sender, OpcSessionEventArgs eArgs)
    {
        Console.WriteLine("Hello from sessionCreatedMethod!");
    }

    private static void SessionClosingMethod(object sender, OpcSessionEventArgs e)
    {
      connectedClients--;
      Console.WriteLine($"A session is closing...");
      Console.WriteLine($"Number of clients: " + connectedClients);
    }

    private static void reqProcessing(object a, OpcRequestProcessingEventArgs b) => Console.WriteLine("New request received\n");

    private static void SessionMethod(object sender, OpcSessionEventArgs e)
    {
      connectedClients++;
      Console.WriteLine($"New session started");
      Console.WriteLine($"Number of clients: " + connectedClients);
    }

    private static void RequestProcessed(object sender, OpcRequestProcessedEventArgs e)
    {
       Console.WriteLine("New request received");
    }

    private static void ServerStarted(object sender, EventArgs eArgs, List<OpcDataVariableNode> listOfNodes, OpcServer opcServer)
    {
      Console.WriteLine("The server is started!");

      List<OpcDataVariableNode> doubleList = new List<OpcDataVariableNode>();
      List<OpcDataVariableNode> stringList = new List<OpcDataVariableNode>();
      List<OpcDataVariableNode> otherList = new List<OpcDataVariableNode>();

      Console.WriteLine("Adding all nodes to the list...");
      foreach(OpcDataVariableNode node in listOfNodes)
      {
        if(node is OpcDataVariableNode<double>)
        {
          doubleList.Add(node);
        }
        else if(node is OpcDataVariableNode<string>)
        {
          stringList.Add(node);
        }
        else
        {
          otherList.Add(node);
        }
      }
      listOfNodes.Clear();
      listOfNodes.AddRange(doubleList); 
      listOfNodes.AddRange(stringList); 
      listOfNodes.AddRange(otherList); 
      
      Console.WriteLine("Finished adding nodes");
      Console.WriteLine("The nodes added are:");
      foreach(OpcDataVariableNode addednode in listOfNodes)
      {
        Console.WriteLine($"Data type: {addednode.DataType} \t ID: {addednode.Id}");
      }
      Console.WriteLine("-------------------------------------------------------");

      listOfNodes[0].ApplyChanges(opcServer.SystemContext);
    }
}
public class MyNodeManager : OpcNodeManager
{
  public IEnumerable<IOpcNode> noder{get;set;}
 public MyNodeManager() : base("http://mynamespace/")
 {
    noder = this.CreateNodes(new OpcNodeReferenceCollection());
 }
 protected override IEnumerable<IOpcNode> CreateNodes(OpcNodeReferenceCollection references)
  {
    // Define custom root node.
      var mainNode = new OpcFolderNode(new OpcName("Main", this.DefaultNamespaceIndex));
      // Add custom root node to the Objects-Folder (the root of all server nodes):
      references.Add(mainNode, OpcObjectTypes.ObjectsFolder);
      // Add custom sub node beneath of the custom root node:
      var isMachineRunningNode = new OpcDataVariableNode<bool>(mainNode,"IsRunning");
      var mainMessageNode = new OpcDataVariableNode<string>(mainNode,"Message","Meldingsverdi");
      var mainAlarmNode = new OpcDataVariableNode<bool>(mainNode,"Alarm");
      var mainLevelNode = new OpcDataVariableNode<double>(mainNode,"Level");
      // Return each custom root node using yield return.
      yield return mainNode;
      //Mer kode her
  }
}