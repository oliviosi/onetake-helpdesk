using System.Security.Cryptography;
using System.Text;
using Konscious.Security.Cryptography;

namespace HelpDesk.API.Services;

public class PasswordHasherService
{
    public string HashPassword(string password)
    {
        // Generate a random salt
        byte[] salt = new byte[16];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        // Hash the password with Argon2id
        var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
        {
            Salt = salt,
            DegreeOfParallelism = 8, // Number of threads to use
            MemorySize = 65536, // 64 MB
            Iterations = 4 // Number of iterations
        };

        byte[] hash = argon2.GetBytes(32); // Get a 32-byte hash

        // Combine the salt and hash for storage
        byte[] hashBytes = new byte[48]; // 16 bytes for salt + 32 bytes for hash
        Array.Copy(salt, 0, hashBytes, 0, 16);
        Array.Copy(hash, 0, hashBytes, 16, 32);

        return Convert.ToBase64String(hashBytes);
    }

    public bool VerifyPassword(string password, string hashedPassword)
    {
        byte[] hashBytes = Convert.FromBase64String(hashedPassword);

        // Extract the salt from the stored hash
        byte[] salt = new byte[16];
        Array.Copy(hashBytes, 0, salt, 0, 16);

        // Hash the input password with the extracted salt
        var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
        {
            Salt = salt,
            DegreeOfParallelism = 8,
            MemorySize = 65536,
            Iterations = 4
        };

        byte[] hash = argon2.GetBytes(32);

        // Compare the computed hash with the stored hash
        for (int i = 0; i < 32; i++)
        {
            if (hash[i] != hashBytes[i + 16])
                return false;
        }

        return true;
    }
} 