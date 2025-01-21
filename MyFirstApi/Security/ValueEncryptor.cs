using System.IO;
using System.Security.Cryptography;
using System.Text;

public class ValueEncryptor
{
    private static string key;
    private static string iv;

    public ValueEncryptor(IConfiguration configuration)
    {
        key = configuration["AES:Key"];
        iv = configuration["AES:InitializationVector"];
    }

    public string Encrypt(string plainText)
    {
        if (key.Length != 32 || iv.Length != 16)
        {
            throw new ArgumentException("Key must be 32 bytes and IV must be 16 bytes long.");
        }

        using var aesAlg = Aes.Create();
        aesAlg.Key = Encoding.UTF8.GetBytes(key);
        aesAlg.IV = Encoding.UTF8.GetBytes(iv);

        var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

        byte[] encrypted;
        using (MemoryStream msEncrypt = new MemoryStream())
        {
            using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
            {
                using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                {
                    swEncrypt.Write(plainText);
                }
            }

            encrypted = msEncrypt.ToArray();
        }

        return Convert.ToBase64String(encrypted);
    }

    public string Decrypt(string cipherText)
    {
        if (key.Length != 32 || iv.Length != 16)
        {
            throw new ArgumentException("Key must be 32 bytes and IV must be 16 bytes long.");
        }

        using var aesAlg = Aes.Create();
        aesAlg.Key = Encoding.UTF8.GetBytes(key);
        aesAlg.IV = Encoding.UTF8.GetBytes(iv);

        var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

        string plaintext;
        byte[] cipherBytes = Convert.FromBase64String(cipherText);

        using (MemoryStream msDecrypt = new MemoryStream(cipherBytes))
        {
            using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
            {
                using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                {
                    plaintext = srDecrypt.ReadToEnd();
                }
            }
        }

        return plaintext;
    }
}