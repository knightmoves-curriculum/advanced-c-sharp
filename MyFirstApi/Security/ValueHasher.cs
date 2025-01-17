using System.Security.Cryptography;
using System.Text;

public class ValueHasher
{
    public string HashPassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashBytes);
        }
    }

    public bool VerifyPassword(string hashedPassword, string password)
    {
        var hashedInputPassword = HashPassword(password);
        return hashedPassword == hashedInputPassword;
    }
}