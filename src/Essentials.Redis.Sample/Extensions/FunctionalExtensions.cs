using LanguageExt;

namespace Essentials.Redis.Sample.Extensions;

/// <summary>
/// Методы расширения для монад
/// </summary>
public static class FunctionalExtensions
{
    public static async Task<Unit> MatchAsync<T>(
        this TryOptionAsync<T> tryOptionAsync,
        string recordKey,
        Action<T> onSome)
    {
        return await tryOptionAsync
            .Match(
                onSome,
                None: () => Console.WriteLine($"Запись с ключом '{recordKey}' не найдена"),
                Fail: ex => Console.WriteLine($"Ошибка получения значения для записи '{recordKey}'. {ex.Message}"));
    }
}