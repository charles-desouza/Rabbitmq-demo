using EasyNetQ;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RequestResponseClient
{
  internal class Program
  {
    private static void Main(string[] args)
    {
      ConnectionConfiguration connConfig = new ConnectionConfiguration
      {
        VirtualHost = "/",
        UserName = "guest",
        Password = "guest",
        PrefetchCount = 1,
        Timeout = 10,
        PersistentMessages = true,
        Hosts = new List<HostConfiguration> { new HostConfiguration { Host = "localhost" } }
      };

      IBus bus = RabbitHutch.CreateBus(connConfig, r => { });

      while (true)
      {
        var message = Console.ReadLine();
        Task<string> task = bus.RequestAsync<string, string>(message);
        task.ContinueWith(response =>
        {
          Console.WriteLine("Got response: '{0}'", response.Result);
        });
      }
    }
  }
}
