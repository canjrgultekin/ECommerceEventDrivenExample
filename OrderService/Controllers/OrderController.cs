
using Confluent.Kafka;
using Microsoft.AspNetCore.Mvc;
using Shared;
using System.Text.Json;
using System.Threading.Tasks;

namespace OrderService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IProducer<Null, string> _producer;

        public OrderController(IProducer<Null, string> producer)
        {
            _producer = producer;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] OrderCreatedEvent orderEvent)
        {
            var message = JsonSerializer.Serialize(orderEvent);
            await _producer.ProduceAsync("order_created", new Message<Null, string> { Value = message });
            return Ok(new { Status = "Order Created", OrderId = orderEvent.OrderId });
        }
    }
}
