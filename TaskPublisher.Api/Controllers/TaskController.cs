using EasyNetQ;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace TaskPublisher.Api.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class TaskController : ControllerBase
  {
    static List<string> _tasks = new List<string>() { "task1", "task2", "task3", "task4", "task5" };
    
    [HttpGet]
    public List<string> Get()
    {
      return _tasks;
    }

    [HttpPost]
    public void Post([FromBody]string task)
    {
      ConnectionConfiguration connConfig = new ConnectionConfiguration
      {
        VirtualHost = "/",
        UserName = "guest",
        Password = "guest",
        PrefetchCount = 1,
        Timeout = 10,
        PersistentMessages = true,
        Hosts = new List<HostConfiguration> { new HostConfiguration { Host = "rabbitmq" } }
      };

      using (var bus = RabbitHutch.CreateBus(connConfig, r => { }))
      {
        var exchange = bus.Advanced.ExchangeDeclare("task-exchange", "topic");
        var consumerQueue = bus.Advanced.QueueDeclare("task-query", durable: true);
        bus.Advanced.Bind(exchange, consumerQueue, $"#.task-query");

        bus.Advanced.Publish(exchange, "#.task-query", true, new Message<string>(task));
      }

    }
  }

  public class WorkOrder
  {
    public string Name { get; set; }
  }
}