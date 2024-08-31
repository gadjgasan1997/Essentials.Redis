namespace Essentials.Redis.Sample;

public class Person
{
    public string? Name { get; set; }
}

public class Person<TName>
    where TName : class
{
    public TName? Name { get; set; }
}

public class Person<TName, TAge>
    where TName : class
    where TAge : struct
{
    public TName? Name { get; set; }
    
    public TAge Age { get; set; }
}