using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EasyNetQ;
using EasyNetQ.Topology;
using Microsoft.Extensions.Hosting;

namespace RequestResponse
{
  public class RabbitMQService : IHostedService
  {
    private IBus _bus;
    private IExchange _exchange;
    public Task StartAsync(CancellationToken cancellationToken)
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

      _bus = RabbitHutch.CreateBus(connConfig, r => { });

      _exchange = _bus.Advanced.ExchangeDeclare("task-exchange", "topic");
      var consumerQueue = _bus.Advanced.QueueDeclare("task-query",durable :true);
      _bus.Advanced.Bind(_exchange, consumerQueue, $"#.task-query");

      _bus.Advanced.Consume(consumerQueue, (body, properties, info) => Task.Factory.StartNew(() =>
      {
        string message = Encoding.UTF8.GetString(body);
        messageHandler(message);
      }));
      return Task.CompletedTask;
    }

    private void messageHandler(string message)
    {
      Console.WriteLine($"Message Received: {message}");
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {

      if(_bus!= null) _bus.Dispose();

      return Task.CompletedTask;
    }
  }
}