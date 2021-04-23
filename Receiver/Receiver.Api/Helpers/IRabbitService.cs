namespace Receiver.Api.Helpers
{
    public interface IRabbitService
    {
        void Receive(string queueName);
    }
}