using System;
using System.Text;
using System.Threading;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Send
{
  class Send
  {
    static void Main()
    {
      Thread.Sleep(20000);

      var factory = new ConnectionFactory() { HostName = "rabbitmq", RequestedHeartbeat = 30 };
      using (var connection = factory.CreateConnection())
      using (var channel = connection.CreateModel())
      {
        channel.QueueDeclare(queue: "hello",
                             durable: false,
                             exclusive: false,
                             autoDelete: false,
                             arguments: null);

        string message = "Hello World!";
        var body = Encoding.UTF8.GetBytes(message);

        channel.BasicPublish(exchange: "",
                             routingKey: "hello",
                             basicProperties: null,
                             body: body);
        Console.WriteLine(" [x] Sent {0}", message);
      }

      Thread.Sleep(20000);
      Console.WriteLine(" Press [enter] to exit.");
      Console.ReadLine();
    }
  }
}
