using System;
using System.Text;
using Microsoft.Extensions.ObjectPool;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Receiver.Api.Domain;
using Receiver.Api.Helpers;

public class RabbitService : IRabbitService
{
    private readonly DefaultObjectPool<IModel> _objectPool;

    public RabbitService(IPooledObjectPolicy<IModel> objectPolicy)
    {
        _objectPool = new DefaultObjectPool<IModel>(objectPolicy, Environment.ProcessorCount * 2);
    }

    public void Receive(string queueName)
    {
        var channel = _objectPool.Get();

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var order = System.Text.Json.JsonSerializer.Deserialize<Order>(message);

                    Console.WriteLine($"Order: {order.OrderNumber}|{order.ItemName}|{order.Price:N2}");

                    channel.BasicAck(ea.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    channel.BasicNack(ea.DeliveryTag, false, true);

                    throw ex;
                }
                finally
                {
                    _objectPool.Return(channel);
                }
            };

        channel.BasicConsume(queue: "orderQueue",
                             autoAck: false,
                             consumer: consumer);


    }
}