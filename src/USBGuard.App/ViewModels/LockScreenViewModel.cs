using System;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using USBGuard.Core.Services;

namespace USBGuard.App.ViewModels;

public partial class LockScreenViewModel : ObservableObject, IDisposable
{
    private readonly ILockService _lockService;
    private readonly IConfigService _configService;
    private readonly DispatcherTimer _timer;
    private bool _disposed;

    [ObservableProperty]
    private string _currentTime = string.Empty;

    [ObservableProperty]
    private string _currentDate = string.Empty;

    [ObservableProperty]
    private string _emergencyPassword = string.Empty;

    [ObservableProperty]
    private bool _isInputEnabled = true;

    [ObservableProperty]
    private string _cooldownMessage = string.Empty;

    private int _failedAttempts;

    [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
    private static extern int SystemParametersInfo(int uAction, int uParam, System.Text.StringBuilder lpvParam, int fuWinIni);
    private const int SPI_GETDESKWALLPAPER = 0x0073;

    public string CurrentWallpaper
    {
        get
        {
            var wallpaper = new System.Text.StringBuilder(260);
            SystemParametersInfo(SPI_GETDESKWALLPAPER, wallpaper.Capacity, wallpaper, 0);
            return wallpaper.ToString();
        }
    }

    public LockScreenViewModel(ILockService lockService, IConfigService configService)
    {
        _lockService = lockService;
        _configService = configService;

        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1)
        };
        _timer.Tick += (s, e) => UpdateTime();
        _timer.Start();

        UpdateTime();
    }

    private void UpdateTime()
    {
        var now = DateTime.Now;
        CurrentTime = now.ToString("HH:mm:ss");
        CurrentDate = now.ToString("dddd, dd MMMM yyyy");
    }

    [RelayCommand]
    private async System.Threading.Tasks.Task TryUnlockAsync()
    {
        if (!IsInputEnabled) return;

        var masterKey = _configService.Current.MasterKey;
        
        if (!string.IsNullOrEmpty(masterKey) && EmergencyPassword == masterKey)
        {
            _failedAttempts = 0;
            _lockService.Unlock();
        }
        else
        {
            _failedAttempts++;
            EmergencyPassword = string.Empty;

            if (_failedAttempts >= 4)
            {
                IsInputEnabled = false;
                
                int penaltyDuration = (int)Math.Pow(2, _failedAttempts - 4) * 5;
                if (penaltyDuration > 300) penaltyDuration = 300;

                for (int i = penaltyDuration; i > 0; i--)
                {
                    CooldownMessage = $"Demasiados intentos. Intenta de nuevo en {i} seg.";
                    await System.Threading.Tasks.Task.Delay(1000).ConfigureAwait(true);
                }

                CooldownMessage = string.Empty;
                IsInputEnabled = true;
            }
        }
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _timer.Stop();
            _disposed = true;
        }
    }
}
