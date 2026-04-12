using System;
using System.Windows;
using System.Windows.Input;
using USBGuard.App.ViewModels;

namespace USBGuard.App.Views;

public partial class LockScreenWindow : Window
{
    private readonly LockScreenViewModel _viewModel;

    public LockScreenWindow(LockScreenViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = _viewModel;
    }

    protected override void OnPreviewKeyDown(KeyEventArgs e)
    {
        // Block Alt+F4 and Alt+Tab
        if (e != null && e.SystemKey == Key.F4 && Keyboard.Modifiers == ModifierKeys.Alt)
        {
            e.Handled = true;
        }

        base.OnPreviewKeyDown(e);
    }
}
