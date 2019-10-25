using System;
using System.Threading.Tasks;
using Contracts;
using MassTransit;

namespace MTApi
{
  internal class SendMessageConsumer : IConsumer<Message>
  {
    public Task Consume(ConsumeContext<Message> context)
    {
      Console.WriteLine($"Got: {context.Message.Value}");
      return Task.CompletedTask;
    }
  }
}
