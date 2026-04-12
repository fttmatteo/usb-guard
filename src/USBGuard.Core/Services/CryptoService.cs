using System;
using System.Security.Cryptography;
using System.Text;

namespace USBGuard.Core.Services;

public interface ICryptoService
{
    string EncryptString(string plainText, string password);
    string DecryptString(string cipherText, string password);
    string HashPassword(string password, out string salt);
    bool VerifyPassword(string password, string hash, string salt);
}

public class CryptoService : ICryptoService
{
    private const int SaltSize = 32; // 256 bit
    private const int NonceSize = 12; // 96 bit
    private const int TagSize = 16;  // 128 bit
    private const int KeySize = 32;  // 256 bit
    private const int Iterations = 600000;

    public string EncryptString(string plainText, string password)
    {
        byte[] salt = new byte[SaltSize];
        byte[] nonce = new byte[NonceSize];
        
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
            rng.GetBytes(nonce);
        }

        byte[] key = GetKeyFromPassword(password, salt);
        byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
        byte[] cipherBytes = new byte[plainBytes.Length];
        byte[] tag = new byte[TagSize];

        using (var aesGcm = new AesGcm(key, TagSize))
        {
            aesGcm.Encrypt(nonce, plainBytes, cipherBytes, tag);
        }

        byte[] result = new byte[SaltSize + NonceSize + TagSize + cipherBytes.Length];
        Buffer.BlockCopy(salt, 0, result, 0, SaltSize);
        Buffer.BlockCopy(nonce, 0, result, SaltSize, NonceSize);
        Buffer.BlockCopy(tag, 0, result, SaltSize + NonceSize, TagSize);
        Buffer.BlockCopy(cipherBytes, 0, result, SaltSize + NonceSize + TagSize, cipherBytes.Length);

        return Convert.ToBase64String(result);
    }

    public string DecryptString(string cipherText, string password)
    {
        byte[] data = Convert.FromBase64String(cipherText);

        byte[] salt = new byte[SaltSize];
        byte[] nonce = new byte[NonceSize];
        byte[] tag = new byte[TagSize];
        byte[] cipherBytes = new byte[data.Length - SaltSize - NonceSize - TagSize];

        Buffer.BlockCopy(data, 0, salt, 0, SaltSize);
        Buffer.BlockCopy(data, SaltSize, nonce, 0, NonceSize);
        Buffer.BlockCopy(data, SaltSize + NonceSize, tag, 0, TagSize);
        Buffer.BlockCopy(data, SaltSize + NonceSize + TagSize, cipherBytes, 0, cipherBytes.Length);

        byte[] key = GetKeyFromPassword(password, salt);
        byte[] plainBytes = new byte[cipherBytes.Length];

        using (var aesGcm = new AesGcm(key, TagSize))
        {
            aesGcm.Decrypt(nonce, cipherBytes, tag, plainBytes);
        }

        return Encoding.UTF8.GetString(plainBytes);
    }

    private byte[] GetKeyFromPassword(string password, byte[] salt)
    {
        return Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA512, KeySize);
    }

    public string HashPassword(string password, out string saltString)
    {
        byte[] salt = new byte[SaltSize];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }
        saltString = Convert.ToBase64String(salt);
        byte[] key = GetKeyFromPassword(password, salt);
        return Convert.ToBase64String(key);
    }

    public bool VerifyPassword(string password, string hash, string saltString)
    {
        byte[] salt = Convert.FromBase64String(saltString);
        byte[] key = GetKeyFromPassword(password, salt);
        return Convert.ToBase64String(key) == hash;
    }
}
