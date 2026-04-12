using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using USBGuard.Core.Services;
using USBGuard.App.ViewModels;
using USBGuard.App.Views;

namespace USBGuard.App;

public partial class App : Application
{
    public IServiceProvider Services { get; }

    public new static App Current => (App)Application.Current;

    public App()
    {
        Services = ConfigureServices();
    }

    private static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        services.AddSingleton<ICryptoService, CryptoService>();
        services.AddSingleton<IConfigService, ConfigService>();
        services.AddSingleton<IUsbMonitorService, UsbMonitorService>();
        services.AddSingleton<IKeyFileService, KeyFileService>();
        services.AddSingleton<ILockService, LockService>();
        services.AddSingleton<ISystemManagerService, SystemManagerService>();

        services.AddTransient<MainViewModel>();
        services.AddTransient<LockScreenViewModel>();

        services.AddTransient<MainWindow>();
        services.AddTransient<LockScreenWindow>();

        return services.BuildServiceProvider();
    }

    private void OnStartup(object sender, StartupEventArgs e)
    {
        var configService = Services.GetRequiredService<IConfigService>();
        configService.Load();

        var mainWindow = Services.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }

    private void OnExit(object sender, ExitEventArgs e)
    {
        var configService = Services.GetRequiredService<IConfigService>();
        configService.Save();
        
        var usbMonitor = Services.GetRequiredService<IUsbMonitorService>();
        usbMonitor.StopMonitoring();
    }
}
