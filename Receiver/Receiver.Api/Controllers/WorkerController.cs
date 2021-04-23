using System;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Receiver.Api.Domain;
using Receiver.Api.Helpers;

namespace Receiver.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WorkerController : ControllerBase
    {
        private ILogger<WorkerController> _logger;
        private IRabbitService _service;

        public WorkerController(ILogger<WorkerController> logger, IRabbitService service)
        {
            _logger = logger;
            _service = service;
        }

        [HttpGet]
        public IActionResult testOrder()
        {
            try
            {
                _service.Receive("orderQueue");

                return Accepted("Itens Recebidos!");
            }
            catch (Exception ex)
            {
                _logger.LogError("Erro ao tentar consumir o pedido!", ex);

                return new StatusCodeResult(500);
            }
        }
    }
}