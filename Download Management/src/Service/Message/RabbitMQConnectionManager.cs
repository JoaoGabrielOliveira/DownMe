using RabbitMQ.AMQP.Client;
using RabbitMQ.AMQP.Client.Impl;

public interface IRabbitMQConnection : IAsyncDisposable
{
    Task<IConnection> GetConnection(CancellationToken cancellationToken = default);
}

public class RabbitMQConnection : IRabbitMQConnection
{
    private readonly ConnectionSettings _setting;
    private IConnection? _connection;
    private IEnvironment ? _envrioment;
    public RabbitMQConnection(IConfiguration configuration)
    {
        string userName = configuration.GetValue<string>("AMQP_USERNAME") ?? "";
        string password = configuration.GetValue<string>("AMQP_PASSWORD") ?? "";
        string host = configuration.GetValue<string>("AMQP_HOST") ?? "";
        int port = configuration.GetValue<int>("AMQP_PORT");
        string virtualHost = configuration.GetValue<string>("AMQP_VIRTUAL_HOST") ?? "";

        _setting = ConnectionSettingsBuilder.Create()
        .User(userName)
        .Password(password)
        .Host(host)
        .Port(port)
        .VirtualHost(virtualHost)
        .Build();
    }
    public async Task<IConnection> GetConnection(CancellationToken cancellationToken)
    {
        _envrioment ??= AmqpEnvironment.Create(_setting);

        _connection ??= await _envrioment.CreateConnectionAsync(cancellationToken);

        return _connection;
    }

    public async ValueTask DisposeAsync()
    {
        if (_connection is not null)
        {
            await _connection.CloseAsync();
        }

        if (_envrioment is not null)
        {
            await _envrioment.CloseAsync();
        }

        GC.SuppressFinalize(this);
    }
}