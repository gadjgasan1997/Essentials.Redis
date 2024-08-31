using LanguageExt;
using Essentials.Redis.Options;
using Essentials.Utils.Extensions;
using Microsoft.Extensions.Options;

namespace Essentials.Redis.Extensions;

/// <summary>
/// Методы расширения для <see cref="IOptions{TOptions}" />
/// </summary>
public static class OptionsExtensions
{
    /// <summary>
    /// Возвращает опции хеш таблицы
    /// </summary>
    /// <param name="options">Объект с опциями</param>
    /// <param name="hashTableId">Id хеш таблицы</param>
    /// <returns>Опции хеш таблицы</returns>
    public static Option<HashTableOptions> GetHashTableOptions(
        this IOptions<RedisOptions> options,
        string hashTableId)
    {
        options.CheckNotNull("Опции взаимодействия с редисом не могут быть null");
        hashTableId.CheckNotNullOrEmpty("Id хеш таблицы для получения опций не может быть null");
        
        return options.Value.HashTablesOptions.TryGetValue(hashTableId, out var hashTableOptions)
            ? hashTableOptions
            : Option<HashTableOptions>.None;
    }

    /// <summary>
    /// Возвращает время жизни ключа для хеш таблицы
    /// </summary>
    /// <param name="options">Объект с опциями</param>
    /// <param name="hashTableId">Id хеш таблицы</param>
    /// <returns>Время жизни</returns>
    public static Option<TimeSpan> GetHashTableKeyLifeTime(
        this IOptions<RedisOptions> options,
        string hashTableId)
    {
        return options
            .GetHashTableOptions(hashTableId)
            .Bind(hashTableOptions => hashTableOptions.KeyLifeTime ?? Option<TimeSpan>.None);
    }

    /// <summary>
    /// Возвращает название хеш таблицы
    /// </summary>
    /// <param name="options">Объект с опциями</param>
    /// <param name="hashTableId">Id хеш таблицы</param>
    /// <returns>Название</returns>
    public static Option<string> GetHashTableName(
        this IOptions<RedisOptions> options,
        string hashTableId)
    {
        return options
            .GetHashTableOptions(hashTableId)
            .Map(hashTableOptions => hashTableOptions.TableName);
    }
}