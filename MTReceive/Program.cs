using System;
using System.Threading;
using GreenPipes;
using MassTransit;

namespace MTReceive
{
  class Program
  {
    static void Main(string[] args)
    {

      var bus = Bus.Factory.CreateUsingRabbitMq(sbc =>
      {
        var host = sbc.Host(new Uri("rabbitmq://guest:guest@rabbitmq:5672"), h =>
        {
          h.Username("guest");
          h.Password("guest");
        });

        sbc.ReceiveEndpoint(host, "test_queue", endpoint =>
        {
          endpoint.PrefetchCount = 16;
          endpoint.UseMessageRetry(x => x.Interval(50, 1000));
          endpoint.Handler<Message>(context =>
          {
            return Console.Out.WriteLineAsync($"Received: {context.Message.Value}");
          });
        });
      });

      bus.Start();

      // bus.Publish(new Message { Value = "Hi" });

      Console.WriteLine("Press any key to exit");
      Console.Read();

      bus.Stop();
    }
  }

  internal class Message
  {
    public string Value { get; set; }
  }
}
