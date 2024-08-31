using LanguageExt;
using static LanguageExt.Prelude;

namespace Essentials.Redis.Extensions;

/// <summary>
/// Методы расширения для монад
/// </summary>
public static class FunctionalExtensions
{
    /// <summary>
    /// Снимает обертку со значения, хранящегося в хеш таблице в редисе, возвращая объект из нее
    /// </summary>
    /// <param name="tryOptionAsync">Делегат с оберткой</param>
    /// <returns>Делегат с объектом</returns>
    public static TryOptionAsync<object> Unwrap(this TryOptionAsync<HashSetRecord> tryOptionAsync)
    {
        return tryOptionAsync
            .Bind(value =>
                TryOptionAsync(value.Value is null
                    ? Option<object>.None
                    : Option<object>.Some(value.Value)));
    }
    
    /// <summary>
    /// Снимает обертку со значения, хранящегося в хеш таблице в редисе, преобразуя объект в тип <typeparamref name="T" />
    /// </summary>
    /// <param name="tryOptionAsync">Делегат с оберткой</param>
    /// <param name="converter">Делегат преобразования полученного ответа в тип <typeparamref name="T"/></param>
    /// <typeparam name="T">Тип результата</typeparam>
    /// <returns>Делегат с результатом</returns>
    public static TryOptionAsync<T> Unwrap<T>(
        this TryOptionAsync<HashSetRecord> tryOptionAsync,
        Func<object, T?> converter)
    {
        return tryOptionAsync
            .Unwrap()
            .Bind(value => TryOptionAsync(() => converter(value).AsTask()))!;
    }
}