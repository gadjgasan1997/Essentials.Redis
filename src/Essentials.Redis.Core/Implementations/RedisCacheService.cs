using LanguageExt;
using LanguageExt.Common;
using StackExchange.Redis;
using Microsoft.Extensions.Options;
using Essentials.Redis.Connection;
using Essentials.Redis.Extensions;
using Essentials.Redis.Options;
using Essentials.Serialization;
using Essentials.Utils.Extensions;
using static Essentials.Redis.Dictionaries.KnownSerializers;

namespace Essentials.Redis.Implementations;

/// <inheritdoc cref="IRedisCacheService" />
internal class RedisCacheService : IRedisCacheService
{
    private readonly IRedisConnection _redisConnection;
    private readonly IOptions<RedisOptions> _redisOptions;
    
    private static readonly IEssentialsSerializer _serializer =
        EssentialsSerializersFactory.TryGet(REDIS_INFO_SERIALIZER);

    private static readonly IEssentialsDeserializer _deserializer =
        EssentialsDeserializersFactory.TryGet(REDIS_INFO_DESERIALIZER);
    
    public RedisCacheService(
        IRedisConnection redisConnection,
        IOptions<RedisOptions> redisOptions)
    {
        _redisConnection = redisConnection.CheckNotNull();
        _redisOptions = redisOptions;
    }

    /// <inheritdoc cref="IRedisCacheService.HashSetAsync(string, HashSetRecord, When)" />
    public TryAsync<bool> HashSetAsync(string hashTableId, HashSetRecord record, When when = When.Always)
    {
        return async () =>
        {
            hashTableId.CheckNotNullOrEmpty("Id хеш таблицы для сохранения элемента не может быть пустым");

            var database = _redisConnection.GetRedisDatabase();
            var hashTableName = _redisOptions.GetHashTableName(hashTableId).IfNone(hashTableId);
            var keyExpiry = _redisOptions.GetHashTableKeyLifeTime(hashTableId).IfNone(TimeSpan.FromDays(1));

            await database.KeyExpireAsync(hashTableName, keyExpiry);
            return await database.HashSetAsync(
                hashTableName,
                record.Key,
                _serializer.Serialize(record),
                when);
        };
    }
    
    /// <inheritdoc cref="IRedisCacheService.HashSetAsync(string, IEnumerable{HashSetRecord}, When)" />
    public TryAsync<Unit> HashSetAsync(
        string hashTableId,
        IEnumerable<HashSetRecord> records,
        When when = When.Always)
    {
        return async () =>
        {
            hashTableId.CheckNotNullOrEmpty("Id хеш таблицы для сохранения элемента не может быть пустым");
            
            var database = _redisConnection.GetRedisDatabase();
            var hashTableName = _redisOptions.GetHashTableName(hashTableId).IfNone(hashTableId);
            var keyExpiry = _redisOptions.GetHashTableKeyLifeTime(hashTableId).IfNone(TimeSpan.FromDays(1));

            await database.KeyExpireAsync(hashTableName, keyExpiry);

            foreach (var record in records)
            {
                await database.HashSetAsync(
                    hashTableName,
                    record.Key,
                    _serializer.Serialize(record),
                    when);
            }

            return Unit.Default;
        };
    }

    /// <inheritdoc cref="IRedisCacheService.HashDeleteAsync" />
    public TryAsync<Unit> HashDeleteAsync(string hashTableId, string key)
    {
        return async () =>
        {
            hashTableId.CheckNotNullOrEmpty("Id хеш таблицы для удаления элемента не может быть пустым");
            key.CheckNotNullOrEmpty("Ключ для удаления элемента из хеш таблицы не может быть пустым");
            
            var database = _redisConnection.GetRedisDatabase();
            var hashTableName = _redisOptions.GetHashTableName(hashTableId).IfNone(hashTableId);
            await database.HashDeleteAsync(hashTableName, key);
            
            return Unit.Default;
        };
    }

    /// <inheritdoc cref="IRedisCacheService.HashGetAsync" />
    public async Task<Validation<Error, int>> DeleteExpiredRecordsAsync(string hashTableId)
    {
        if (string.IsNullOrWhiteSpace(hashTableId))
            return Error.New("Id хеш таблицы для удаления истекших элементов не может быть пустым");
        
        var database = _redisConnection.GetRedisDatabase();
        var hashTableName = _redisOptions.GetHashTableName(hashTableId).IfNone(hashTableId);

        var deletedRecords = 0;
        var errors = new List<Error>();
        
        await foreach (var entry in database.HashScanAsync(hashTableName))
        {
            var result = await DeleteRecordIfExpiredAsync(database, hashTableName, entry);
            result.IfFail(seq => errors.AddRange(seq));
            result.IfSuccess(isDeleted =>
            {
                if (isDeleted)
                    deletedRecords++;
            });
        }

        return errors.Count != 0 ? errors.ToSeq() : deletedRecords;
    }

    /// <inheritdoc cref="IRedisCacheService.HashGetAsync" />
    public TryOptionAsync<HashSetRecord> HashGetAsync(string hashTableId, string key)
    {
        return async () =>
        {
            hashTableId.CheckNotNullOrEmpty("Id хеш таблицы для поиска элемента не может быть пустым");
            key.CheckNotNullOrEmpty("Ключ для поиска элемента в хеш таблице не может быть пустым");
            
            var database = _redisConnection.GetRedisDatabase();
            var hashTableName = _redisOptions.GetHashTableName(hashTableId).IfNone(hashTableId);
            
            var redisValue = await database.HashGetAsync(hashTableName, key);
            if (!redisValue.HasValue)
                return Option<HashSetRecord>.None;

            var essentialsRedisValue = _deserializer.Deserialize<HashSetRecord>(redisValue);
            if (essentialsRedisValue is null)
                return Option<HashSetRecord>.None;

            if (!essentialsRedisValue.IsExpired)
                return essentialsRedisValue;
            
            _ = await HashDeleteAsync(hashTableId, key).Try();
            return Option<HashSetRecord>.None;
        };
    }
    
    /// <inheritdoc cref="IRedisCacheService.HashPopAsync" />
    public TryOptionAsync<HashSetRecord> HashPopAsync(string hashTableId, string key)
    {
        return HashGetAsync(hashTableId, key)
            .MapAsync(async record =>
            {
                _ = await HashDeleteAsync(hashTableId, key).IfFailThrow();
                return record;
            });
    }

    /// <summary>
    /// Удаляет запись из БД, если она уже истекла
    /// </summary>
    /// <param name="database"></param>
    /// <param name="hashTableName">Название хеш таблицы</param>
    /// <param name="entry">Запись</param>
    /// <returns></returns>
    private static async Task<Validation<Error, bool>> DeleteRecordIfExpiredAsync(
        IDatabaseAsync database,
        string hashTableName,
        HashEntry entry)
    {
        return await Prelude
            .TryAsync(async () =>
            {
                var record = _deserializer.Deserialize<HashSetRecord>(entry.Value);
                if (record is null || !record.IsExpired)
                    return false;

                await database.HashDeleteAsync(hashTableName, entry.Name);
                return true;
            })
            .ToValidation(
                Fail: exception =>
                    Error.New(
                        $"Во время удаления записи с названием '{entry.Name}' из таблицы '{hashTableName}' произошло исключение",
                        exception));
    }
}