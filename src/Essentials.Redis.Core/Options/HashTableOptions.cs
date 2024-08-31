using System.ComponentModel.DataAnnotations;

namespace Essentials.Redis.Options;

/// <summary>
/// Опции хеш таблицы
/// </summary>
public class HashTableOptions
{
    /// <summary>
    /// Название хеш таблицы
    /// </summary>
    [Required]
    public string TableName { get; init; } = null!;
    
    /// <summary>
    /// Время жизни ключа
    /// </summary>
    public TimeSpan? KeyLifeTime { get; init; }
    
    /// <summary>
    /// Признак необходимости удалять истекшие записи в таблице
    /// </summary>
    public bool NeedDeleteExpiredRecords { get; init; }
}