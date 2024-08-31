using StackExchange.Redis;
using Essentials.Redis.Options;
using Microsoft.Extensions.Options;

namespace Essentials.Redis.Connection;

/// <inheritdoc cref="IRedisConnection" />
internal class ConnectionManager : IRedisConnection
{
    private readonly Lazy<ConnectionMultiplexer> _redisConnection;

    public ConnectionManager(IOptions<RedisOptions> options)
    {
        _redisConnection = new Lazy<ConnectionMultiplexer>(() =>
            ConnectionMultiplexer.Connect(options.Value.ConnectionString));
    }

    /// <inheritdoc cref="IRedisConnection.GetConnectionMultiplexer" />
    public ConnectionMultiplexer GetConnectionMultiplexer() => _redisConnection.Value;

    /// <inheritdoc cref="IRedisConnection.GetRedisDatabase" />
    public IDatabase GetRedisDatabase() => GetConnectionMultiplexer().GetDatabase();
}