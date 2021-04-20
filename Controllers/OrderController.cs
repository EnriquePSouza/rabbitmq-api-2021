using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RabbitMq.Api.Domain;

namespace RabbitMq.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private ILogger<OrderController> _logger;

        public OrderController(ILogger<OrderController> logger)
        {
            _logger = logger;
        }
        
        public IActionResult InsertOrder(Order order)
        {
            try
            {
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