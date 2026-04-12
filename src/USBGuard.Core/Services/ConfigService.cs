using System;
using System.IO;
using System.Security.Cryptography;
using System.Text.Json;
using USBGuard.Core.Models;

namespace USBGuard.Core.Services;

public interface IConfigService
{
    AppConfig Current { get; }
    void Save();
    void Load();
}

public class ConfigService : IConfigService
{
    private readonly string _configFilePath;
    private readonly string _configDirectory;
    private AppConfig _current = new AppConfig();

    public AppConfig Current => _current;

    public ConfigService()
    {
        _configDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "USBGuard");
        _configFilePath = Path.Combine(_configDirectory, "config.dat");
    }

    public void Save()
    {
        if (!Directory.Exists(_configDirectory))
        {
            Directory.CreateDirectory(_configDirectory);
        }

        string json = JsonSerializer.Serialize(_current);
        byte[] plainBytes = System.Text.Encoding.UTF8.GetBytes(json);
        
        byte[] encryptedBytes = ProtectedData.Protect(plainBytes, null, DataProtectionScope.CurrentUser);
        
        File.WriteAllBytes(_configFilePath, encryptedBytes);
    }

    public void Load()
    {
        if (!File.Exists(_configFilePath))
        {
            _current = new AppConfig();
            return;
        }

        try
        {
            byte[] encryptedBytes = File.ReadAllBytes(_configFilePath);
            byte[] plainBytes = ProtectedData.Unprotect(encryptedBytes, null, DataProtectionScope.CurrentUser);
            
            string json = System.Text.Encoding.UTF8.GetString(plainBytes);
            _current = JsonSerializer.Deserialize<AppConfig>(json) ?? new AppConfig();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading config: {ex.Message}");
            _current = new AppConfig();
        }
    }
}
