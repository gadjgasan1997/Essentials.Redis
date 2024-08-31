using System.ComponentModel.DataAnnotations;

namespace Essentials.Redis.Options;


/// <summary>
/// Опции взаимодействия с редисом
/// </summary>
public class RedisOptions
{
    /// <summary>
    /// Название секции в конфигурации
    /// </summary>
    public static string Section => "Redis";
    
    /// <summary>
    /// Строка подключения
    /// </summary>
    [Required]
    public string ConnectionString { get; init; } = null!;

    /// <summary>
    /// Опции хеш таблиц
    /// </summary>
    public Dictionary<string, HashTableOptions> HashTablesOptions { get; init; } = new();
    
    /// <summary>
    /// Поиск элемента по Id хеш таблицы
    /// </summary>
    /// <param name="hashTableId">Id хеш таблицы</param>
    public HashTableOptions this[string hashTableId] => HashTablesOptions[hashTableId];
}