using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static System.StringComparison;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace Essentials.Redis.Converters;

/// <summary>
/// Конвертер для <see cref="HashSetRecord" />
/// </summary>
internal class HashSetRecordJsonConverter : JsonConverter<HashSetRecord>
{
    private const string WRAPPER_NAME = nameof(HashSetRecord);

    private static readonly JsonSerializer _recordValueDeserializer = JsonSerializer.Create(
        new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto
        });
    
    public override void WriteJson(JsonWriter writer, HashSetRecord? value, JsonSerializer serializer) =>
        serializer.Serialize(writer, value);

    public override HashSetRecord ReadJson(
        JsonReader reader,
        Type objectType,
        HashSetRecord? existingValue,
        bool hasExistingValue,
        JsonSerializer serializer)
    {
        var jObject = JObject.Load(reader);

        var key = GetKey(jObject);
        var typeName = GetTypeName(jObject);
        var value = GetValue(jObject, typeName);
        var setDate = GetSetDate(jObject);
        var expiryDate = GetExpiryDate(jObject);
        var valueLifeTime = GetValueLifeTime(jObject);

        return new HashSetRecord(
            key,
            value,
            typeName,
            setDate,
            expiryDate,
            valueLifeTime);
    }
    
    /// <summary>
    /// Возвращает из json-а значение для свойства <see cref="HashSetRecord.Key" />
    /// </summary>
    /// <param name="jObject">Json объект</param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    private static string GetKey(JObject jObject)
    {
        const string propertyName = nameof(HashSetRecord.Key);

        var value = GetRequiredValue(jObject, propertyName);
        CheckValueType(value, JTokenType.String, propertyName);
        
        var key = value.Value<string>();
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new InvalidOperationException(
                $"В json не заполнено значение для обязательного поля '{propertyName}'");
        }

        return key;
    }

    /// <summary>
    /// Возвращает из json-а значение для свойства <see cref="HashSetRecord.Value" />
    /// </summary>
    /// <param name="jObject">Json объект</param>
    /// <param name="typeName">Название типа</param>
    /// <returns></returns>
    private static object? GetValue(JObject jObject, string typeName)
    {
        const string propertyName = nameof(HashSetRecord.Value);
        
        var valueToken = GetRequiredValue(jObject, propertyName);
        var valueType = Type.GetType(typeName);
        
        return valueType is null ? null : valueToken.ToObject(valueType, _recordValueDeserializer);
    }
    
    /// <summary>
    /// Возвращает из json-а значение для свойства <see cref="HashSetRecord.TypeName" />
    /// </summary>
    /// <param name="jObject">Json объект</param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    private static string GetTypeName(JObject jObject)
    {
        const string propertyName = nameof(HashSetRecord.TypeName);

        var value = GetRequiredValue(jObject, propertyName);
        CheckValueType(value, JTokenType.String, propertyName);
        
        var typeName = value.Value<string>();
        if (string.IsNullOrWhiteSpace(typeName))
        {
            throw new InvalidOperationException(
                $"В json не заполнено значение для обязательного поля '{propertyName}'");
        }

        return typeName;
    }

    /// <summary>
    /// Возвращает из json-а значение для свойства <see cref="HashSetRecord.SetDate" />
    /// </summary>
    /// <param name="jObject">Json объект</param>
    /// <returns></returns>
    private static DateTime GetSetDate(JObject jObject)
    {
        const string propertyName = nameof(HashSetRecord.SetDate);

        var value = GetRequiredValue(jObject, propertyName);
        CheckValueType(value, JTokenType.Date, propertyName);

        return value.Value<DateTime>();
    }

    /// <summary>
    /// Возвращает из json-а значение для свойства <see cref="HashSetRecord.ExpiryDate" />
    /// </summary>
    /// <param name="jObject">Json объект</param>
    /// <returns></returns>
    private static DateTime? GetExpiryDate(JObject jObject)
    {
        const string propertyName = nameof(HashSetRecord.ExpiryDate);
        
        if (!jObject.TryGetValue(propertyName, InvariantCultureIgnoreCase, out var value))
            return null;

        if (value.Type is JTokenType.Null)
            return null;
        
        CheckValueType(value, JTokenType.Date, propertyName);
        return value.Value<DateTime>();
    }

    /// <summary>
    /// Возвращает из json-а значение для свойства <see cref="HashSetRecord.ValueLifeTime" />
    /// </summary>
    /// <param name="jObject">Json объект</param>
    /// <returns></returns>
    private static TimeSpan? GetValueLifeTime(JObject jObject)
    {
        const string propertyName = nameof(HashSetRecord.ValueLifeTime);
        
        if (!jObject.TryGetValue(propertyName, InvariantCultureIgnoreCase, out var value))
            return null;

        if (value.Type is JTokenType.Null)
            return null;

        CheckValueType(value, JTokenType.String, propertyName);
        return value.ToObject<TimeSpan>();
    }
    
    private static JToken GetRequiredValue(JObject jObject, string propertyName)
    {
        if (jObject.TryGetValue(propertyName, InvariantCultureIgnoreCase, out var value))
            return value;
        
        throw new InvalidOperationException(
            $"В json не содержится обязательное поле '{propertyName}' для заполнения свойства " +
            $"'{WRAPPER_NAME}.{propertyName}'");
    }
    
    private static void CheckValueType(
        JToken value,
        JTokenType expectedType,
        string propertyName)
    {
        if (value.Type != expectedType)
        {
            throw new InvalidOperationException(
                $"Значение для свойства '{WRAPPER_NAME}.{propertyName}' " +
                $"должно иметь тип '{expectedType}'. " +
                $"Фактический тип: '{value.Type}'");
        }
    }
}