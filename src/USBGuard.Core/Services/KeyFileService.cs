using System;
using System.IO;

namespace USBGuard.Core.Services;

public interface IKeyFileService
{
    bool CreateKeyFile(string driveLetter, string encryptionKey);
    bool ValidateKeyFile(string driveLetter, string encryptionKey);
}

public class KeyFileService : IKeyFileService
{
    private readonly ICryptoService _cryptoService;
    private const string KeyFileName = "unlock.key";
    private const string ExpectedDecryptedContent = "USBGuard_ValidKey";

    public KeyFileService(ICryptoService cryptoService)
    {
        _cryptoService = cryptoService;
    }

    public bool CreateKeyFile(string driveLetter, string encryptionKey)
    {
        try
        {
            string filePath = Path.Combine(driveLetter, KeyFileName);
            string encryptedContent = _cryptoService.EncryptString(ExpectedDecryptedContent, encryptionKey);
            File.WriteAllText(filePath, encryptedContent);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public bool ValidateKeyFile(string driveLetter, string encryptionKey)
    {
        try
        {
            string filePath = Path.Combine(driveLetter, KeyFileName);
            if (!File.Exists(filePath))
                return false;

            string encryptedContent = File.ReadAllText(filePath);
            string decryptedContent = _cryptoService.DecryptString(encryptedContent, encryptionKey);

            return decryptedContent == ExpectedDecryptedContent;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
