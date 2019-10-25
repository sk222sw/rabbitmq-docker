using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MassTransit;
using GreenPipes;
using System.Threading;

namespace MTApi
{
  public class BusService : IHostedService
  {
    private readonly IBusControl _busControl;

    public BusService(IBusControl busControl)
    {
      _busControl = busControl;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
      return _busControl.StartAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
      return _busControl.StopAsync(cancellationToken);
    }
  }

  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      // services.AddHealthChecks();
      services.AddControllers();

      services.AddMassTransit(x =>
      {
        x.AddConsumer<SendMessageConsumer>();
        x.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
        {
          var host = cfg.Host(new Uri("rabbitmq://guest:guest@rabbitmq:5672"), hostConfigurator =>
          {
            hostConfigurator.Username("guest");
            hostConfigurator.Password("guest");
          });

          cfg.ReceiveEndpoint(host, "test_queue", endpoint =>
          {
            endpoint.PrefetchCount = 16;
            endpoint.UseMessageRetry(x => x.Interval(50, 1000));
            endpoint.ConfigureConsumer<SendMessageConsumer>(provider);
          });
        }));

        var timeout = TimeSpan.FromSeconds(10);
        var serviceAddress = new Uri("rabbitmq://localhost");

        // services.AddScoped<IRequestClient<Message, MyResponse>>(x =>
        //   new MessageRequestClient<Message, MyResponse>(x.GetRequiredService<IBus>(), serviceAddress, timeout, timeout));

        services.AddSingleton<IBus>(provider => provider.GetRequiredService<IBusControl>());
        services.AddSingleton<IHostedService, BusService>();

      });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }

      // app.UseHttpsRedirection();

      app.UseRouting();

      app.UseAuthorization();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();
      });
    }
  }
}
