In today's lesson we'll look at generic type constraints. A constraint on a C# generic restricts the types that can be used as arguments for a generic type parameter, ensuring they meet specific requirements such as implementing an interface. Constraints are defined using the `where` keyword, like where T : interface.

``` cs
using System.Numerics;

namespace MyFirstApi.Models
{
    public interface IWriteRepository<TId, T> where TId : INumber<TId>
    {
        T Save(T entity);
        T Update(TId id, T entity);
        T RemoveById(TId id);
        int Count();
    }
}
```

`dotnet run`

In the coding exercise you will use a generic type constraint.

## Main Points
- A constraint on a C# generic restricts the types that can be used as arguments for a generic type parameter, ensuring they meet specific requirements such as implementing an interface. 
- Generic type constraints are defined using the `where` keyword, like where T : interface.

## Suggested Coding Exercise
- Have students use a generic type constraint

## Building toward CSTA Standards:
None

## Resources
- https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/generics/constraints-on-type-parameters