namespace Essentials.Redis.Dictionaries;

/// <summary>
/// Используемые сериалайзеры информации.
/// Используются при сериализации данных, сохраняемых в редис
/// </summary>
internal static class KnownSerializers
{
    /// <summary>
    /// Сериалайзер
    /// </summary>
    public const string REDIS_INFO_SERIALIZER = "REDIS_INFO_SERIALIZER";
    
    /// <summary>
    /// Десериалайзер
    /// </summary>
    public const string REDIS_INFO_DESERIALIZER = "REDIS_INFO_DESERIALIZER";
}