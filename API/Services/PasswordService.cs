using System.Security.Cryptography;
using System.Text;
using API.Interfaces;

namespace API.Services;

public class PasswordService : IPasswordService
{
    // PBKDF2 configuration (following OWASP recommendations)
    private const int SaltSize = 128 / 8; // 128 bits
    private const int KeySize = 256 / 8; // 256 bits
    private const int Iterations = 600000; // OWASP 2023 recommendation for PBKDF2-HMAC-SHA256
    private static readonly HashAlgorithmName HashAlgorithm = HashAlgorithmName.SHA256;

    public void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password cannot be empty or whitespace", nameof(password));

        // Generate a cryptographically secure random salt
        passwordSalt = RandomNumberGenerator.GetBytes(SaltSize);

        // Derive the hash using PBKDF2
        passwordHash = Rfc2898DeriveBytes.Pbkdf2(
            password: Encoding.UTF8.GetBytes(password),
            salt: passwordSalt,
            iterations: Iterations,
            hashAlgorithm: HashAlgorithm,
            outputLength: KeySize
        );
    }

    public bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password cannot be empty or whitespace", nameof(password));

        if (storedHash.Length != KeySize)
            return false;

        if (storedSalt.Length != SaltSize)
            return false;

        // Derive a hash from the provided password using the stored salt
        var computedHash = Rfc2898DeriveBytes.Pbkdf2(
            password: Encoding.UTF8.GetBytes(password),
            salt: storedSalt,
            iterations: Iterations,
            hashAlgorithm: HashAlgorithm,
            outputLength: KeySize
        );

        // Use constant-time comparison to prevent timing attacks
        return CryptographicOperations.FixedTimeEquals(computedHash, storedHash);
    }
}
