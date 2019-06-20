using System.Threading;
using System;
using Opc.UaFx;
using Opc.UaFx.Server;
using System.Collections.Generic;

namespace Server
{
    public class MyNodeManager : OpcNodeManager
    {
        public MyNodeManager(): base("http://mynamespace/")
        {
            
        }
    }
}