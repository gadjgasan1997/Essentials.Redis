using Moq;
using LanguageExt;
using StackExchange.Redis;

namespace Essentials.Redis.TestsUtils.Extensions;

/// <summary>
/// Методы расширения для <see cref="IRedisCacheService" />
/// </summary>
public static class RedisCacheServiceExtensions
{
    /// <summary>
    /// Устанавливает для мока поведение метода <see cref="IRedisCacheService.HashSetAsync(string, HashSetRecord, When)" />
    /// </summary>
    /// <param name="mock">Мок</param>
    /// <param name="func">Делегат, устанавливающий поведение для метода</param>
    /// <returns>Мок</returns>
    public static Mock<IRedisCacheService> Setup_HashSetAsync(
        this Mock<IRedisCacheService> mock,
        Func<string, HashSetRecord, When, TryAsync<bool>> func)
    {
        mock
            .Setup(service =>
                service.HashSetAsync(
                    It.IsAny<string>(),
                    It.IsAny<HashSetRecord>(),
                    It.IsAny<When>()))
            .Returns(func);
        
        return mock;
    }
    
    /// <summary>
    /// Устанавливает для мока поведение метода <see cref="IRedisCacheService.HashSetAsync(string, IEnumerable{HashSetRecord}, When)" />
    /// </summary>
    /// <param name="mock">Мок</param>
    /// <param name="func">Делегат, устанавливающий поведение для метода</param>
    /// <returns>Мок</returns>
    public static Mock<IRedisCacheService> Setup_HashSetAsync(
        this Mock<IRedisCacheService> mock,
        Func<string, IEnumerable<HashSetRecord>, When, TryAsync<Unit>> func)
    {
        mock
            .Setup(service =>
                service.HashSetAsync(
                    It.IsAny<string>(),
                    It.IsAny<IEnumerable<HashSetRecord>>(),
                    It.IsAny<When>()))
            .Returns(func);
        
        return mock;
    }
    
    /// <summary>
    /// Устанавливает для мока поведение метода <see cref="IRedisCacheService.HashDeleteAsync" />
    /// </summary>
    /// <param name="mock">Мок</param>
    /// <param name="func">Делегат, устанавливающий поведение для метода</param>
    /// <returns>Мок</returns>
    public static Mock<IRedisCacheService> Setup_HashDeleteAsync(
        this Mock<IRedisCacheService> mock,
        Func<string, string, TryAsync<Unit>> func)
    {
        mock
            .Setup(service =>
                service.HashDeleteAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>()))
            .Returns(func);
        
        return mock;
    }
    
    /// <summary>
    /// Устанавливает для мока поведение метода <see cref="IRedisCacheService.HashGetAsync" />
    /// </summary>
    /// <param name="mock">Мок</param>
    /// <param name="func">Делегат, устанавливающий поведение для метода</param>
    /// <returns>Мок</returns>
    public static Mock<IRedisCacheService> Setup_HashGetAsync(
        this Mock<IRedisCacheService> mock,
        Func<string, string, TryOptionAsync<HashSetRecord>> func)
    {
        mock
            .Setup(service =>
                service.HashGetAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>()))
            .Returns(func);
        
        return mock;
    }
    
    /// <summary>
    /// Устанавливает для мока поведение метода <see cref="IRedisCacheService.HashPopAsync" />
    /// </summary>
    /// <param name="mock">Мок</param>
    /// <param name="func">Делегат, устанавливающий поведение для метода</param>
    /// <returns>Мок</returns>
    public static Mock<IRedisCacheService> Setup_HashPopAsync(
        this Mock<IRedisCacheService> mock,
        Func<string, string, TryOptionAsync<HashSetRecord>> func)
    {
        mock
            .Setup(service =>
                service.HashPopAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>()))
            .Returns(func);
        
        return mock;
    }
}