namespace MyFirstApi.Models
{
    public interface IUserRepository : IWriteRepository<int, User> 
    {
        User FindByUsername(string username);
        void LogUserName(string username)
        {
            Console.WriteLine("Log " + username);
        }
    }
}