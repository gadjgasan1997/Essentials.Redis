using LanguageExt;
using LanguageExt.Common;
using StackExchange.Redis;

namespace Essentials.Redis;

/// <summary>
/// Сервис для кеширования данных в редисе
/// </summary>
public interface IRedisCacheService
{
    /// <summary>
    /// Сохраняет данные в хеш таблице
    /// </summary>
    /// <param name="hashTableId">Id хеш таблицы</param>
    /// <param name="record">Запись</param>
    /// <param name="when">Условие, когда необходимо добавлять запись</param>
    /// <returns></returns>
    TryAsync<bool> HashSetAsync(string hashTableId, HashSetRecord record, When when = When.Always);

    /// <summary>
    /// Сохраняет данные в хеш таблице
    /// </summary>
    /// <param name="hashTableId">Id хеш таблицы</param>
    /// <param name="records">Список записей</param>
    /// <param name="when">Условие, когда необходимо добавлять запись</param>
    /// <returns></returns>
    TryAsync<Unit> HashSetAsync(
        string hashTableId,
        IEnumerable<HashSetRecord> records,
        When when = When.Always);

    /// <summary>
    /// Удаляет данные из хеш таблицы
    /// </summary>
    /// <param name="hashTableId">Id хеш таблицы</param>
    /// <param name="key">Ключ</param>
    /// <returns></returns>
    TryAsync<Unit> HashDeleteAsync(string hashTableId, string key);
    
    /// <summary>
    /// Удаляет истекшие записи в хеш таблице
    /// </summary>
    /// <param name="hashTableId">Id хеш таблицы</param>
    /// <returns></returns>
    Task<Validation<Error, int>> DeleteExpiredRecordsAsync(string hashTableId);
    
    /// <summary>
    /// Возвращает данные хеш таблицы
    /// </summary>
    /// <param name="hashTableId">Id хеш таблицы</param>
    /// <param name="key">Ключ</param>
    /// <returns></returns>
    TryOptionAsync<HashSetRecord> HashGetAsync(string hashTableId, string key);
    
    /// <summary>
    /// Возвращает данные хеш таблицы, удаляя их оттуда
    /// </summary>
    /// <param name="hashTableId">Id хеш таблицы</param>
    /// <param name="key">Ключ</param>
    /// <returns></returns>
    TryOptionAsync<HashSetRecord> HashPopAsync(string hashTableId, string key);
}