using StackExchange.Redis;

namespace Essentials.Redis.Connection;

/// <summary>
/// Соединение с редисом
/// </summary>
public interface IRedisConnection
{
    /// <summary>
    /// Возвращает объект <see cref="ConnectionMultiplexer" />
    /// </summary>
    /// <returns></returns>
    ConnectionMultiplexer GetConnectionMultiplexer();
    
    /// <summary>
    /// Возвращает БД редиса
    /// </summary>
    /// <returns>БД редиса</returns>
    IDatabase GetRedisDatabase();
}