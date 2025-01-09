namespace MyFirstApi.Models
{
    using Microsoft.EntityFrameworkCore;

    public class CityRepository : IWriteRepository<int, City>, IReadRepository<int, City>
    {
        private WeatherForecastDbContext context;

        public CityRepository(WeatherForecastDbContext context)
        {
            this.context = context;
        }

        public City Save(City city)
        {
            context.Cities.Add(city);
            context.SaveChanges();
            return city;
        }

        public City Update(int id, City city)
        {
            city.Id = id;
            context.Cities.Update(city);
            context.SaveChanges();
            return city;
        }

        public City RemoveById(int id)
        {
            var city = context.Cities.Find(id);
            context.Cities.Remove(city);
            context.SaveChanges();
            return city;
        }

        public List<City> FindAll()
        {
            return context.Cities
            .ToList();
        }

        public City FindById(int id)
        {
            return context.Cities.Find(id);
        }

        public int Count()
        {
            return context.Cities.Count();
        }
    }
}