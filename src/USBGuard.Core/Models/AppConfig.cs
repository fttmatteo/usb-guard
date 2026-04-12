namespace USBGuard.Core.Models;

public class AppConfig
{
    public bool IsEnabled { get; set; } = false;
    public LockMode LockMode { get; set; } = LockMode.RaptorLock;
    public bool StartWithWindows { get; set; } = false;
    public bool StartArmed { get; set; } = false;
    public bool MuteWhileLocked { get; set; } = false;
    public int LockDelaySeconds { get; set; } = 0;
    
    public AppThemeMode ThemeMode { get; set; } = AppThemeMode.System;

    public System.Collections.Generic.List<string> ConfiguredUsbHistory { get; set; } = new();

    public bool IsAppPasswordProtected { get; set; } = false;
    public string AppPasswordHash { get; set; } = string.Empty;
    public string PasswordSalt { get; set; } = string.Empty;
    public string MasterKey { get; set; } = string.Empty;
    
    public bool LockOnUsbSerialNumber { get; set; } = false;
    public string AllowedSerialNumber { get; set; } = string.Empty;
    
    public string EncryptionKey { get; set; } = string.Empty;
}
