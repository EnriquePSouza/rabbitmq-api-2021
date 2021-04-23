using System;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.ObjectPool;
using RabbitMQ.Client;
using Sender.Api.Helpers;

public class RabbitService : IRabbitService
{
    private readonly DefaultObjectPool<IModel> _objectPool;

    public RabbitService(IPooledObjectPolicy<IModel> objectPolicy)
    {
        _objectPool = new DefaultObjectPool<IModel>(objectPolicy, Environment.ProcessorCount * 2);
    }

    public void Publish<T>(T message, string exchangeName, string exchangeType, string routeKey, string queueName)
        where T : class
    {
        if (message == null)
            return;

        var channel = _objectPool.Get();

        try
        {
            channel.ExchangeDeclare(exchangeName, exchangeType, false, false, null);

            channel.QueueDeclare(queueName, false, false, false, null);

            channel.QueueBind(queueName, exchangeName, routeKey, null);

            var sendBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;

            channel.BasicPublish(exchangeName, routeKey, properties, sendBytes);
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            _objectPool.Return(channel);
        }
    }
}