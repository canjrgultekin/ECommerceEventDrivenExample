
using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Shared;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace InventoryService.Services
{
    public class KafkaConsumerService : BackgroundService
    {
        private readonly IConsumer<Null, string> _consumer;

        public KafkaConsumerService()
        {
            var config = new ConsumerConfig
            {
                GroupId = "inventory_service_group",
                BootstrapServers = "localhost:9092",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };
            _consumer = new ConsumerBuilder<Null, string>(config).Build();
            _consumer.Subscribe("order_created");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var cr = _consumer.Consume(stoppingToken);
                var orderEvent = JsonSerializer.Deserialize<OrderCreatedEvent>(cr.Message.Value);
                Console.WriteLine($"Order received: {orderEvent.OrderId}, Product: {orderEvent.ProductId}, Quantity: {orderEvent.Quantity}");
                await Task.CompletedTask;
            }
        }
    }
}
