using System;
using System.Windows;
using System.Windows.Interop;
using USBGuard.App.ViewModels;
using USBGuard.Core.Services;

namespace USBGuard.App.Views;

public partial class MainWindow
{
    private readonly MainViewModel _viewModel;
    private readonly IUsbMonitorService _usbMonitor;
    private readonly ILockService _lockService;
    private System.Collections.Generic.List<LockScreenWindow> _lockScreens = new();
    private bool _isExplicitClose = false;

    public MainWindow(MainViewModel viewModel, IUsbMonitorService usbMonitor, ILockService lockService)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = _viewModel;
        _usbMonitor = usbMonitor;
        _lockService = lockService;

        _lockService.LockRequested += OnLockRequested;
        _lockService.UnlockRequested += OnUnlockRequested;
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        Wpf.Ui.Appearance.SystemThemeWatcher.Watch(this);
        
        var helper = new WindowInteropHelper(this);
        var hwnd = helper.Handle;
        
        _usbMonitor.StartMonitoring(hwnd);

        HwndSource source = HwndSource.FromHwnd(hwnd);
        source?.AddHook(HwndHook);
    }

    private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        _usbMonitor.HandleDeviceChange(msg, wParam, lParam);
        return IntPtr.Zero;
    }

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        if (!_isExplicitClose)
        {
            e.Cancel = true;
            this.Hide();
        }
    }

    private void Window_Closed(object sender, EventArgs e)
    {
        _viewModel.SaveSettings();
        _usbMonitor.StopMonitoring();
        TrayIcon.Dispose();
    }

    private void TrayIcon_TrayMouseDoubleClick(object sender, RoutedEventArgs e)
    {
        this.Show();
        this.WindowState = WindowState.Normal;
        this.Activate();
    }

    private void ShowConfig_Click(object sender, RoutedEventArgs e)
    {
        this.Show();
        this.WindowState = WindowState.Normal;
        this.Activate();
    }

    private void ExitApp_Click(object sender, RoutedEventArgs e)
    {
        _isExplicitClose = true;
        Application.Current.Shutdown();
    }

    private void OnLockRequested(object? sender, EventArgs e)
    {
        Dispatcher.Invoke(() =>
        {
            if (_lockScreens.Count > 0) return;

            foreach (var screen in System.Windows.Forms.Screen.AllScreens)
            {
                if (App.Current.Services.GetService(typeof(LockScreenWindow)) is LockScreenWindow lockScreen)
                {
                    lockScreen.Left = screen.Bounds.Left;
                    lockScreen.Top = screen.Bounds.Top;
                    lockScreen.Width = screen.Bounds.Width;
                    lockScreen.Height = screen.Bounds.Height;
                    lockScreen.WindowStartupLocation = WindowStartupLocation.Manual;
                    
                    lockScreen.Show();
                    _lockScreens.Add(lockScreen);
                }
            }
        });
    }

    private void OnUnlockRequested(object? sender, EventArgs e)
    {
        Dispatcher.Invoke(() =>
        {
            foreach (var lockScreen in _lockScreens)
            {
                lockScreen.Close();
            }
            _lockScreens.Clear();
        });
    }

    private void EncryptionKeyBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is ViewModels.MainViewModel vm && sender is Wpf.Ui.Controls.PasswordBox pb)
        {
            vm.EncryptionKeyInput = pb.Password;
        }
    }

    private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
    {
        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
        {
            FileName = e.Uri.AbsoluteUri,
            UseShellExecute = true
        });
        e.Handled = true;
    }
}
