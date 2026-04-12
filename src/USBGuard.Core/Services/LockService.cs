using System;
using USBGuard.Core.Interop;
using USBGuard.Core.Models;

namespace USBGuard.Core.Services;

public interface ILockService
{
    event EventHandler LockRequested;
    event EventHandler UnlockRequested;
    
    void Lock(LockMode mode);
    void Unlock();
}

public class LockService : ILockService
{
    private readonly IConfigService _configService;
    private readonly ISystemManagerService _systemManager;

    public LockService(IConfigService configService, ISystemManagerService systemManager)
    {
        _configService = configService;
        _systemManager = systemManager;
    }

    public event EventHandler? LockRequested;
    public event EventHandler? UnlockRequested;

    public void Lock(LockMode mode)
    {
        if (_configService.Current.MuteWhileLocked)
        {
            _systemManager.SetSystemMute(true);
        }

        if (mode == LockMode.SystemLock)
        {
            NativeMethods.LockWorkStation();
        }
        else if (mode == LockMode.RaptorLock)
        {
            KeyboardHook.Enable();
            LockRequested?.Invoke(this, EventArgs.Empty);
        }
    }

    public void Unlock()
    {
        if (_configService.Current.MuteWhileLocked)
        {
            _systemManager.SetSystemMute(false);
        }
        
        KeyboardHook.Disable();
        UnlockRequested?.Invoke(this, EventArgs.Empty);
    }
}
