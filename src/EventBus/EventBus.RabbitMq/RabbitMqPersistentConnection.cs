using System.Net.Sockets;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace EventBus.RabbitMq;

public class RabbitMqPersistentConnection : IDisposable
{
    private IConnection connection;
    public bool IsConnected => connection != null && connection.IsOpen;
    private readonly IConnectionFactory? _connectionFactory;
    private readonly int _retryCount;
    private object lock_object = new object();
    private bool _disposed;
    
    
    
    public RabbitMqPersistentConnection(IConnectionFactory? connectionFactory, int retryCount = 5)
    {
        _connectionFactory = connectionFactory;
        _retryCount = retryCount;
    }

    public IModel CreateModel()
    {
        return connection.CreateModel();
    }

    public bool TryConnect()
    {
        lock (lock_object)
        {
            var policy = Policy.Handle<SocketException>()
                .Or<BrokerUnreachableException>()
                .WaitAndRetry(_retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (ex, time) =>
                    {

                    });

            policy.Execute(() =>
            {
                connection = _connectionFactory.CreateConnection();
            });
            if (IsConnected)
            {
                connection.ConnectionShutdown += ConnectionOnConnectionShutdown;
                connection.CallbackException += ConnectionOnCallbackException;
                connection.ConnectionBlocked += ConnectionOnConnectionBlocked;
                
                //Log
                
                return true;
            }
            return false;
        }
    }

    private void ConnectionOnConnectionBlocked(object? sender, ConnectionBlockedEventArgs e)
    {
        if (_disposed) return;
        TryConnect();
    }

    private void ConnectionOnCallbackException(object? sender, CallbackExceptionEventArgs e)
    {
        if (_disposed) return;
        TryConnect();
    }

    private void ConnectionOnConnectionShutdown(object? sender, ShutdownEventArgs e)
    {
        //log
        if (_disposed) return;
        TryConnect();
    }

    public void Dispose()
    {
        _disposed = true;
        connection.Dispose();
    }
}