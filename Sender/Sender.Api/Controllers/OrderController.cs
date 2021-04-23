using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sender.Api.Domain;
using Sender.Api.Helpers;

namespace Sender.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private ILogger<OrderController> _logger;
        private IRabbitService _service;

        public OrderController(ILogger<OrderController> logger, IRabbitService service)
        {
            _logger = logger;
            _service = service;
        }

        public IActionResult InsertOrder(Order order)
        {
            try
            {
                _service.Publish(order, "orderExchange", "topic", "orderRoute", "orderQueue");

                return Accepted(order);
            }
            catch (Exception ex)
            {

                _logger.LogError("Erro ao tentar criar um novo pedido!", ex);

                return new StatusCodeResult(500);
            }
        }
    }
}