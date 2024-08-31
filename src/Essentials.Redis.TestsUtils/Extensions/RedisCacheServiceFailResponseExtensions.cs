using Moq;
using LanguageExt;
using LanguageExt.Common;
using StackExchange.Redis;
using static LanguageExt.Prelude;

namespace Essentials.Redis.TestsUtils.Extensions;

/// <summary>
/// Методы расширения для <see cref="IRedisCacheService" />, возвращающие ошибочный ответ
/// </summary>
public static class RedisCacheServiceFailResponseExtensions
{
    private static readonly Error _unknownError = new Exception("Unknown error");

    /// <summary>
    /// Устанавливает для мока возврат ошибки из метода <see cref="IRedisCacheService.HashSetAsync(string, HashSetRecord, When)" />
    /// </summary>
    /// <param name="mock">Мок</param>
    /// <param name="error">Требуемая ошибка</param>
    /// <returns>Мок</returns>
    public static Mock<IRedisCacheService> SetupFail_HashSetItemAsync(
        this Mock<IRedisCacheService> mock,
        Error? error = null)
    {
        return mock.Setup_HashSetAsync((string _, HashSetRecord _, When _) =>
        {
            return TryAsync<bool>(() => throw (error ?? _unknownError).ToErrorException());
        });
    }
    
    /// <summary>
    /// Устанавливает для мока возврат ошибки из метода <see cref="IRedisCacheService.HashSetAsync(string, IEnumerable{HashSetRecord}, When)" />
    /// </summary>
    /// <param name="mock">Мок</param>
    /// <param name="error">Требуемая ошибка</param>
    /// <returns>Мок</returns>
    public static Mock<IRedisCacheService> SetupFail_HashSetListAsync(
        this Mock<IRedisCacheService> mock,
        Error? error = null)
    {
        return mock.Setup_HashSetAsync((string _, IEnumerable<HashSetRecord> _, When _) =>
        {
            return TryAsync<Unit>(() => throw (error ?? _unknownError).ToErrorException());
        });
    }
    
    /// <summary>
    /// Устанавливает для мока возврат ошибки из метода <see cref="IRedisCacheService.HashDeleteAsync" />
    /// </summary>
    /// <param name="mock">Мок</param>
    /// <param name="error">Требуемая ошибка</param>
    /// <returns>Мок</returns>
    public static Mock<IRedisCacheService> SetupFail_HashDeleteAsync(
        this Mock<IRedisCacheService> mock,
        Error? error = null)
    {
        return mock.Setup_HashDeleteAsync((_, _) =>
        {
            return TryAsync<Unit>(() => throw (error ?? _unknownError).ToErrorException());
        });
    }
    
    /// <summary>
    /// Устанавливает для мока возврат ошибки из метода <see cref="IRedisCacheService.HashGetAsync" />
    /// </summary>
    /// <param name="mock">Мок</param>
    /// <param name="error">Требуемая ошибка</param>
    /// <returns>Мок</returns>
    public static Mock<IRedisCacheService> SetupFail_HashGetAsync(
        this Mock<IRedisCacheService> mock,
        Error? error = null)
    {
        return mock.Setup_HashGetAsync((_, _) =>
        {
            return TryOptionAsync<HashSetRecord>(() => throw (error ?? _unknownError).ToErrorException());
        });
    }
    
    /// <summary>
    /// Устанавливает для мока возврат ошибки из метода <see cref="IRedisCacheService.HashPopAsync" />
    /// </summary>
    /// <param name="mock">Мок</param>
    /// <param name="error">Требуемая ошибка</param>
    /// <returns>Мок</returns>
    public static Mock<IRedisCacheService> SetupFail_HashPopAsync(
        this Mock<IRedisCacheService> mock,
        Error? error = null)
    {
        return mock.Setup_HashPopAsync((_, _) =>
        {
            return TryOptionAsync<HashSetRecord>(() => throw (error ?? _unknownError).ToErrorException());
        });
    }
}