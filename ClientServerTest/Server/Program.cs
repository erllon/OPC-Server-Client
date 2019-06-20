using System.Threading;
using System;
using Opc.UaFx;
using Opc.UaFx.Server;
using System.Collections.Generic;


class Program
{
  static int connectedClients;
  public static void Main()
  {
    connectedClients = 0;
    List<OpcDataVariableNode> nodeList = new List<OpcDataVariableNode>();
    var temperatureNode = new OpcDataVariableNode<double>("Temperature", 100.0);
    var messageNode = new OpcDataVariableNode<string>("Message", string.Empty);
    var levelDetectionNode = new OpcDataVariableNode<bool>("Level", false);
    
    bool avslutt= false;

    nodeList.Add(levelDetectionNode);
    nodeList.Add(temperatureNode);
    nodeList.Add(messageNode); 
    
    using (var server = new OpcServer("opc.tcp://localhost:4840/", nodeList)) 
    {   
      server.Started += new EventHandler((sender, e) => ServerStarted(sender, e, nodeList, server));
      // server.RequestProcessing += new OpcRequestProcessingEventHandler((a, b) => reqProcessing(a,b));
      //server.RequestProcessed += new OpcRequestProcessedEventHandler((sender, e) => RequestProcessed(sender, e));
      server.SessionActivated += new OpcSessionEventHandler((sender, e) => SessionMethod(sender,e));
      server.SessionCreated += new OpcSessionEventHandler((serr,rerrr)=>sessionCreatedMethod(serr,rerrr));
      server.SessionClosing += new OpcSessionEventHandler((sender, e) => SessionClosingMethod(sender,e));

      server.Start();

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