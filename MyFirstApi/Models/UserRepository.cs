namespace MyFirstApi.Models
{
    using System;

    public class UserRepository : IUserRepository
    {
        private WeatherForecastDbContext context;

        public UserRepository(WeatherForecastDbContext context)
        {
            this.context = context;
        }

        public User Save(User user)
        {
            context.Users.Add(user);
            
            context.SaveChanges();
            return user;
        }

        public User Update(int id, User user)
        {
            user.Id = id;
            context.Users.Update(user);
            context.SaveChanges();
            return user;
        }


        public User FindByUsername(string username)
        {
            return context.Users.FirstOrDefault(u => u.Username == username);
        }

        public User RemoveById(int id)
        {
            var user = context.Users.Find(id);
            context.Users.Remove(user);
            context.SaveChanges();
            return user;
        }

        public int Count()
        {
            return context.Users.Count();
        }
    }
}