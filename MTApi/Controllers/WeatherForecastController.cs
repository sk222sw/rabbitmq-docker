using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MassTransit;
using Contracts;

namespace MTApi.Controllers
{
  [ApiController]
  [Route("")]
  public class WeatherForecastController : ControllerBase
  {
    private readonly IBus _bus;

    public WeatherForecastController(IBus bus)
    {
      _bus = bus;
    }

    [HttpGet]
    public async Task<IActionResult> Get(string message = "No message provided :(")
    {
      var _message = new Message { Value = message };
      await _bus.Publish<Message>(_message);

      return Ok("hej");
    }
  }
}
