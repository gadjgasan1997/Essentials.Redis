using Newtonsoft.Json;
using Essentials.Utils.Extensions;
using Essentials.Utils.Reflection.Models;
using EssentialsTypeName = Essentials.Utils.Reflection.Models.TypeName;

namespace Essentials.Redis;

/// <summary>
/// Запись в хеш таблице
/// </summary>
public record HashSetRecord
{
    [JsonConstructor]
    internal HashSetRecord(
        string key,
        object? value,
        string typeName,
        DateTime setDate,
        DateTime? expiryDate,
        TimeSpan? valueLifeTime = null)
    {
        Key = key;
        Value = value;
        TypeName = typeName;
        SetDate = setDate;
        ExpiryDate = expiryDate;
        ValueLifeTime = valueLifeTime;
    }
    
    /// <summary>
    /// Ключ
    /// </summary>
    public string Key { get; }
    
    /// <summary>
    /// Значение
    /// </summary>
    public object? Value { get; }
    
    /// <summary>
    /// Полное название типа элемента
    /// </summary>
    public string TypeName { get; }
    
    /// <summary>
    /// Дата проставления
    /// </summary>
    public DateTime SetDate { get; }
    
    /// <summary>
    /// Дата истечения времени жизни элемента
    /// </summary>
    public DateTime? ExpiryDate { get; }
    
    /// <summary>
    /// Признак, что время жизни элемента истекло
    /// </summary>
    [JsonIgnore]
    public bool IsExpired => ExpiryDate is not null && ExpiryDate < DateTime.Now;
    
    /// <summary>
    /// Время жизни записи в хеш таблице
    /// </summary>
    public TimeSpan? ValueLifeTime { get; }

    /// <summary>
    /// Создает объект записи в хеш таблице
    /// </summary>
    /// <param name="key">Ключ</param>
    /// <param name="value">Значение</param>
    /// <param name="valueLifeTime">Время жизни записи в хеш таблице</param>
    /// <typeparam name="TKey">Тип ключа</typeparam>
    /// <typeparam name="TValue">Тип значения</typeparam>
    /// <returns>Объект записи</returns>
    public static HashSetRecord Create<TKey, TValue>(
        TKey key,
        TValue? value,
        TimeSpan? valueLifeTime = null)
    {
        key.CheckNotNull("Ключ для сохранения элемента в хеш таблице не может быть пустым");

        var assemblyName = EssentialsAssemblyName.CreateShortName(typeof(TValue).Assembly);
        var typeName = EssentialsTypeName.CreateFullName<TValue>(assemblyName);
        
        return new HashSetRecord(
            key.ToString()!,
            value,
            typeName.Value,
            DateTime.Now,
            valueLifeTime.HasValue ? DateTime.Now.Add(valueLifeTime.Value) : null,
            valueLifeTime);
    }
}