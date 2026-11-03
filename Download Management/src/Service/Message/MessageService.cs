using API.Models.Request;
using API.Models.Utils;
using RabbitMQ.AMQP.Client;
using RabbitMQ.AMQP.Client.Impl;

public interface IMessageService<T> where T : class
{
    /* TODO remove this guy and swap to a Factory Pattern to keep the DI pattern */
    public string ExchangeName { get; set; }
    public Task publishMessage(T message, string key);
}

public class RabbitMessageService<T> : IMessageService<T> where T : class 
{
    private readonly IRabbitMQConnection RabbitMQConnection;
    private IPublisher? _publisher;
    public string ExchangeName { get; set; }

    public RabbitMessageService(IRabbitMQConnection channel)
    {
        RabbitMQConnection = channel;
    }

    public async Task publishMessage(T message, string key)
    {
        ArgumentNullException.ThrowIfNull(message);

        try
        {
            IConnection connection = await RabbitMQConnection.GetConnection();
            _publisher = await connection.PublisherBuilder()
                .Exchange(ExchangeName)
                .Key(key)
                .BuildAsync();

            IMessage amqpMessage = new AmqpMessage(message.ToString());

            var result = await _publisher.PublishAsync(amqpMessage);

            if (result != null)
            {    
                if (result.Outcome.State != OutcomeState.Accepted)
                {
                    throw new Exception(string.Format("Message was not published, {0}", result.Outcome.State));
                }
            } else
            {
                throw new Exception("Result is null");
            }
            
        }
        finally
        {
            if (_publisher is not null)
            {
                await _publisher.CloseAsync();
            }
        }

    }
}