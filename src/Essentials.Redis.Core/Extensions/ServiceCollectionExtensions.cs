using Newtonsoft.Json;
using Essentials.Redis.Options;
using Essentials.Redis.Connection;
using Essentials.Redis.Converters;
using Essentials.Redis.Implementations;
using Essentials.Configuration.Extensions;
using Essentials.Serialization;
using Essentials.Serialization.Deserializers;
using Essentials.Serialization.Serializers;
using Essentials.Utils.Reflection.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using static Essentials.Redis.Dictionaries.KnownSerializers;

namespace Essentials.Redis.Extensions;

/// <summary>
/// Методы расширения для <see cref="IServiceCollection" />
/// </summary>
public static class ServiceCollectionExtensions
{
    private static uint _redisIsConfigured;

    /// <summary>
    /// Настраивает взаимодействие с редисом
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    public static IServiceCollection ConfigureRedis(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        return services.AtomicConfigureService(
            ref _redisIsConfigured,
            () => services.ConfigureRedisPrivate(configuration));
    }
    
    /// <summary>
    /// Настраивает взаимодействие с редисом
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    private static void ConfigureRedisPrivate(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var section = configuration.GetSection(RedisOptions.Section);
        if (!section.Exists())
        {
            throw new InvalidOperationException(
                "Ошибка конфигурации сервиса. " +
                "Не заполнены обязательные свойства подключения к редису. Проверьте конфигурацию");
        }
        
        var options = new RedisOptions();
        section.Bind(options);

        if (!options.CheckRequiredProperties(out var emptyRedisOptionsProperties))
        {
            throw new InvalidOperationException(
                "Ошибка конфигурации сервиса. " +
                "Не заполнены обязательные свойства подключения к редису. Проверьте конфигурацию. " +
                $"Названия свойств: '{string.Join("','", emptyRedisOptionsProperties.Select(p => p.Name))}'");
        }

        foreach (var (hashTableId, hashTableOptions) in options.HashTablesOptions)
        {
            if (!hashTableOptions.CheckRequiredProperties(out var emptyHashTableOptionsProperties))
            {
                throw new InvalidOperationException(
                    "Ошибка конфигурации сервиса. " +
                    $"Не заполнены обязательные свойства хеш таблицы с Id '{hashTableId}'. Проверьте конфигурацию. " +
                    $"Названия свойств: '{string.Join("','", emptyHashTableOptionsProperties.Select(p => p.Name))}'");
            }
        }

        services.Configure<RedisOptions>(section);
        services.TryAddSingleton<IRedisConnection, ConnectionManager>();
        services.TryAddTransient<IRedisCacheService, RedisCacheService>();
        
        EssentialsSerializersFactory.AddByKey(
            REDIS_INFO_SERIALIZER,
            func: () => new NewtonsoftJsonSerializer(
                serializeOptions: new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto
                }));
        
        EssentialsDeserializersFactory.AddByKey(
            REDIS_INFO_DESERIALIZER,
            func: () => new NewtonsoftJsonDeserializer(
                deserializeOptions: new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto,
                    Converters = new List<JsonConverter>
                    {
                        new HashSetRecordJsonConverter()
                    }
                }));
    }
}