using System.Collections.Generic;

namespace MyFirstApi.Models
{
    public interface IDateQueryable<T>
    {
        List<T> FindByDate(DateOnly date);
    }
}