using LanguageExt;
using Essentials.Redis.Extensions;
using Essentials.Redis.Sample.Extensions;

namespace Essentials.Redis.Sample;

public class Service : IHostedService
{
    private readonly IRedisCacheService _redisCacheService;
    
    private const string TEST_TABLE = "test_table";

    public Service(IRedisCacheService redisCacheService)
    {
        _redisCacheService = redisCacheService;
    }
    
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        // Разинактивить пример, чтобы увидеть результат
        //await SetDataAsync();
        //await GetDataAsync();
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    #region Sample
    
    private async Task SetDataAsync()
    {
        var nonGenericPerson = new Person
        {
            Name = "person_test"
        };
        
        var nonGenericPersons = new List<Person>
        {
            nonGenericPerson
        };
        
        var genericPerson = new Person<string>
        {
            Name = "person_test"
        };

        var genericPersons = new List<Person<string>>
        {
            genericPerson
        };
        
        var genericWithAgePerson = new Person<string, int>
        {
            Name = "person_test",
            Age = 20
        };

        var genericWithAgePersons = new List<Person<string, int>>
        {
            genericWithAgePerson
        };
        
        _ = await _redisCacheService
            .HashSetAsync(
                TEST_TABLE,
                HashSetRecord.Create("string_key", "data", TimeSpan.FromMinutes(10)))
            .Try();
        
        _ = await _redisCacheService
            .HashSetAsync(
                TEST_TABLE,
                HashSetRecord.Create("non_generic_person_key", nonGenericPerson, TimeSpan.FromMinutes(10)))
            .Try();
        
        _ = await _redisCacheService
            .HashSetAsync(
                TEST_TABLE,
                HashSetRecord.Create("non_generic_persons_key", nonGenericPersons, TimeSpan.FromMinutes(10)))
            .Try();
        
        _ = await _redisCacheService
            .HashSetAsync(
                TEST_TABLE,
                HashSetRecord.Create("generic_person_key", genericPerson, TimeSpan.FromMinutes(10)))
            .Try();
        
        _ = await _redisCacheService
            .HashSetAsync(
                TEST_TABLE,
                HashSetRecord.Create("generic_persons_key", genericPersons, TimeSpan.FromMinutes(10)))
            .Try();
        
        _ = await _redisCacheService
            .HashSetAsync(
                TEST_TABLE,
                HashSetRecord.Create("generic_with_age_person_key", genericWithAgePerson, TimeSpan.FromMinutes(10)))
            .Try();

        _ = await _redisCacheService
            .HashSetAsync(
                TEST_TABLE,
                HashSetRecord.Create("generic_with_age_persons_key", genericWithAgePersons, TimeSpan.FromMinutes(10)))
            .Try();
    }

    private async Task GetDataAsync()
    {
        var test = await _redisCacheService
            .HashGetAsync(TEST_TABLE, "string_key")
            .Match(
                s => Unit.Default,
                () => Unit.Default,
                errors => Unit.Default);
        
        var data = await _redisCacheService
            .HashPopAsync(TEST_TABLE, "string_key")
            .Unwrap(value => (string) value)
            .MatchAsync(
                recordKey: "string_key",
                onSome: value => Console.WriteLine($"string_key_value: {value}"));

        var nonGenericPerson = await _redisCacheService
            .HashPopAsync(TEST_TABLE, "non_generic_person_key")
            .Unwrap(value => (Person) value)
            .MatchAsync(
                recordKey: "non_generic_person_key",
                onSome: person => Console.WriteLine($"non_generic_person_key_value: {person.Name}"));
        
        var nonGenericPersons = await _redisCacheService
            .HashPopAsync(TEST_TABLE, "non_generic_persons_key")
            .Unwrap(value => (List<Person>) value)
            .MatchAsync(
                recordKey: "non_generic_persons_key",
                onSome: persons => Console.WriteLine($"non_generic_persons_key_count: {persons.Count}"));
        
        var genericPerson = await _redisCacheService
            .HashPopAsync(TEST_TABLE, "generic_person_key")
            .Unwrap(value => value as Person<string>)
            .MatchAsync(
                recordKey: "generic_person_key",
                onSome: person => Console.WriteLine($"generic_person_key_value: {person.Name}"));
        
        var genericPersons = await _redisCacheService
            .HashPopAsync(TEST_TABLE, "generic_persons_key")
            .Unwrap(value => value as List<Person<string>>)
            .MatchAsync(
                recordKey: "generic_persons_key",
                onSome: persons => Console.WriteLine($"generic_persons_key_count: {persons.Count}"));
        
        var genericWithAgePerson = await _redisCacheService
            .HashPopAsync(TEST_TABLE, "generic_with_age_person_key")
            .Unwrap(value => value as Person<string, int>)
            .MatchAsync(
                recordKey: "generic_with_age_person_key",
                onSome: person => Console.WriteLine($"generic_with_age_person_key_value: {person.Name}"));

        var genericWithAgePersons = await _redisCacheService
            .HashPopAsync(TEST_TABLE, "generic_with_age_persons_key")
            .Unwrap(value => value as List<Person<string, int>>)
            .MatchAsync(
                recordKey: "generic_with_age_persons_key",
                onSome: persons => Console.WriteLine($"generic_with_age_persons_key_count: {persons.Count}"));
    }

    #endregion
}