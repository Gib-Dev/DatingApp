namespace API.Interfaces;

public interface IPasswordService
{
    /// <summary>
    /// Hashes a password using PBKDF2 with a randomly generated salt
    /// </summary>
    /// <param name="password">The plain text password to hash</param>
    /// <param name="passwordHash">The resulting hash bytes</param>
    /// <param name="passwordSalt">The salt used for hashing</param>
    void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt);

    /// <summary>
    /// Verifies a password against a stored hash and salt
    /// </summary>
    /// <param name="password">The plain text password to verify</param>
    /// <param name="storedHash">The stored hash to compare against</param>
    /// <param name="storedSalt">The salt used for the stored hash</param>
    /// <returns>True if password matches, false otherwise</returns>
    bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt);
}
