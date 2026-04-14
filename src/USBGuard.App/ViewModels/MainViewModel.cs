using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using USBGuard.Core.Models;
using USBGuard.Core.Services;

namespace USBGuard.App.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly IConfigService _configService;
    private readonly IUsbMonitorService _usbMonitor;
    private readonly IKeyFileService _keyFileService;
    private readonly ILockService _lockService;
    private readonly ISystemManagerService _systemManager;

    [ObservableProperty]
    private bool _isArmed;

    [ObservableProperty]
    private string _statusText = "Desarmado";

    [ObservableProperty]
    private ObservableCollection<UsbDevice> _connectedDrives = new();

    [ObservableProperty]
    private UsbDevice? _selectedDrive;

    [ObservableProperty]
    private string _encryptionKeyInput = string.Empty;

    public AppConfig Config => _configService.Current;

    public int ThemeModeIndex
    {
        get => (int)Config.ThemeMode;
        set
        {
            if (Config.ThemeMode != (AppThemeMode)value)
            {
                Config.ThemeMode = (AppThemeMode)value;
                ApplyTheme();
                SaveSettings();
                OnPropertyChanged();
            }
        }
    }

    public int LockModeIndex
    {
        get => (int)Config.LockMode;
        set
        {
            if (Config.LockMode != (LockMode)value)
            {
                Config.LockMode = (LockMode)value;
                SaveSettings();
                OnPropertyChanged();
            }
        }
    }

    public bool StartArmed
    {
        get => Config.StartArmed;
        set
        {
            if (Config.StartArmed != value)
            {
                if (value && (Config.ConfiguredUsbHistory == null || Config.ConfiguredUsbHistory.Count == 0))
                {
                    System.Windows.MessageBox.Show("No puedes activar el Armado Automático porque no tienes ninguna Llave USB autorizada en el historial.\n\nSin esto, tu computadora se bloquearía permanentemente apenas inicie.", "Seguridad Incompleta", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                    OnPropertyChanged();
                    return;
                }
                Config.StartArmed = value;
                SaveSettings();
                OnPropertyChanged();
            }
        }
    }

    public bool StartWithWindows
    {
        get => Config.StartWithWindows;
        set
        {
            if (Config.StartWithWindows != value)
            {
                Config.StartWithWindows = value;
                _systemManager.SetRunAtStartup(value);
                SaveSettings();
                OnPropertyChanged();
            }
        }
    }

    public bool MuteWhileLocked
    {
        get => Config.MuteWhileLocked;
        set
        {
            if (Config.MuteWhileLocked != value)
            {
                Config.MuteWhileLocked = value;
                SaveSettings();
                OnPropertyChanged();
            }
        }
    }

    public string MasterKeyText
    {
        get => Config.MasterKey;
        set
        {
            if (Config.MasterKey != value)
            {
                Config.MasterKey = value;
                OnPropertyChanged(nameof(MasterKeyText));
                SaveSettings();
            }
        }
    }

    public MainViewModel(
        IConfigService configService,
        IUsbMonitorService usbMonitor,
        IKeyFileService keyFileService,
        ILockService lockService,
        ISystemManagerService systemManager)
    {
        _configService = configService;
        _usbMonitor = usbMonitor;
        _keyFileService = keyFileService;
        _lockService = lockService;
        _systemManager = systemManager;

        _usbMonitor.DeviceArrived += OnDeviceArrived;
        _usbMonitor.DeviceRemoved += OnDeviceRemoved;

        RefreshDrivesCommand.Execute(null);

        if (Config.StartArmed)
        {
            if (Config.ConfiguredUsbHistory == null || Config.ConfiguredUsbHistory.Count == 0)
            {
                Config.StartArmed = false;
                IsArmed = false;
                SaveSettings();
            }
            else
            {
                IsArmed = true;
            }
        }
        else
        {
            IsArmed = false;
        }

        UpdateStatusText();
        ApplyTheme();
    }

    private void ApplyTheme()
    {
        switch (Config.ThemeMode)
        {
            case AppThemeMode.Light:
                Wpf.Ui.Appearance.ApplicationThemeManager.Apply(Wpf.Ui.Appearance.ApplicationTheme.Light);
                break;
            case AppThemeMode.Dark:
                Wpf.Ui.Appearance.ApplicationThemeManager.Apply(Wpf.Ui.Appearance.ApplicationTheme.Dark);
                break;
            default:
                Wpf.Ui.Appearance.ApplicationThemeManager.ApplySystemTheme();
                break;
        }
    }

    [RelayCommand]
    private void RefreshDrives()
    {
        ConnectedDrives.Clear();
        foreach (var drive in _usbMonitor.GetConnectedRemovableDrives())
        {
            ConnectedDrives.Add(drive);
        }
        SelectedDrive = ConnectedDrives.FirstOrDefault();
    }

    [RelayCommand]
    private void ToggleArm()
    {
        if (!IsArmed)
        {
            if (Config.ConfiguredUsbHistory == null || Config.ConfiguredUsbHistory.Count == 0)
            {
                System.Windows.MessageBox.Show("No puedes armar el sistema porque no tienes ninguna Llave USB autorizada en el historial.\n\nPor favor, selecciona una USB conectada, escribe una contraseña y presiona 'Encriptar' para autorizarla antes de activar la seguridad.", "Seguridad Incompleta", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                return;
            }

            bool isKeyPresent = ConnectedDrives.Any(d => ValidateKey(d));
            if (!isKeyPresent)
            {
                System.Windows.MessageBox.Show("Debes tener tu Llave USB maestra encriptada e insertada ahora mismo en la computadora antes de armar el sistema, o te quedarás bloqueado inmediatamente.", "Ausencia de Llave", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                return;
            }
        }

        IsArmed = !IsArmed;
        UpdateStatusText();
        CheckCurrentDrivesForUnlockKey();
    }

    [RelayCommand]
    private void CreateKey()
    {
        if (SelectedDrive == null) return;
        if (string.IsNullOrWhiteSpace(EncryptionKeyInput)) return;

        if (Config.ConfiguredUsbHistory == null)
            Config.ConfiguredUsbHistory = new System.Collections.Generic.List<string>();

        string serialTarget = $"Serial: {(string.IsNullOrWhiteSpace(SelectedDrive.SerialNumber) ? "No expuesto por hardware" : SelectedDrive.SerialNumber)}";
        string? existingEntry = Config.ConfiguredUsbHistory.FirstOrDefault(x => x.Contains(serialTarget, StringComparison.OrdinalIgnoreCase));
        
        if (existingEntry != null)
        {
            System.Windows.MessageBox.Show($"Esta memoria USB ya está configurada en el sistema como una llave autorizada.\n\nRegistro: {existingEntry}\n\nSi deseas reescribir esta llave desde cero, primero elimínala de la lista del Historial debajo.", "USB Autorizado Previamente", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            return;
        }

        bool success = _keyFileService.CreateKeyFile(SelectedDrive.DriveLetter, EncryptionKeyInput);
        if (success)
        {
            string newUsbName = $"GUARD-{System.Security.Cryptography.RandomNumberGenerator.GetInt32(1000, 10000)}";
            try
            {
                var driveInfo = new System.IO.DriveInfo(SelectedDrive.DriveLetter);
                driveInfo.VolumeLabel = newUsbName;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"La llave se escribió pero el USB no pudo ser renombrado automáticamente: {ex.Message}", "Aviso de Renombrado", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            }

            string serialVal = string.IsNullOrWhiteSpace(SelectedDrive.SerialNumber) ? "No expuesto por hardware" : SelectedDrive.SerialNumber;
            string historyEntry = $"{newUsbName} (Serial: {serialVal}) - Creado el {DateTime.Now:dd/MM/yyyy HH:mm}";
            var history = new System.Collections.Generic.List<string>(Config.ConfiguredUsbHistory) { historyEntry };
            Config.ConfiguredUsbHistory = history;

            Config.EncryptionKey = EncryptionKeyInput;
            SaveSettings();
            OnPropertyChanged(nameof(Config));

            System.Windows.MessageBox.Show($"Llave creada con éxito.\nEl USB fue renombrado a '{newUsbName}'.\nYa puedes armar el sistema.", "USB Guard", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
            
            RefreshDrivesCommand.Execute(null);
        }
        else
        {
            System.Windows.MessageBox.Show("Error al crear archivo de llave.", "USB Guard", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
        }
    }

    private DateTime _deleteKeyLockoutEnd = DateTime.MinValue;
    private int _deleteKeyFailedAttempts = 0;

    [RelayCommand]
    private void DeleteFromHistory(string historyItem)
    {
        if (string.IsNullOrEmpty(historyItem)) return;
        if (Config.ConfiguredUsbHistory == null) return;
        
        if (DateTime.Now < _deleteKeyLockoutEnd)
        {
            int remaining = (int)(_deleteKeyLockoutEnd - DateTime.Now).TotalSeconds;
            System.Windows.MessageBox.Show($"Defensa de Fuerza Bruta Activa.\n\nDemasiados intentos fallidos. Modificación inhabilitada por {remaining} segundos.", "Panel Protegido", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            return;
        }

        bool isAuthorized = false;
        
        App.Current.Dispatcher.Invoke(() =>
        {
            var dialog = new USBGuard.App.Views.PasswordDialog(App.Current.MainWindow);
            if (dialog.ShowDialog() == true)
            {
                if (dialog.Password == Config.EncryptionKey)
                    isAuthorized = true;
            }
        });

        if (!isAuthorized)
        {
            _deleteKeyFailedAttempts++;
            if (_deleteKeyFailedAttempts >= 4)
            {
                int penaltyDuration = (int)Math.Pow(2, _deleteKeyFailedAttempts - 4) * 5;
                if (penaltyDuration > 300) penaltyDuration = 300;
                
                _deleteKeyLockoutEnd = DateTime.Now.AddSeconds(penaltyDuration);
                System.Windows.MessageBox.Show($"Contraseña incorrecta.\n\nTu caja fuerte ha entrado en modo de Bloqueo Exponencial. Modificación deshabilitada por {penaltyDuration} segundos.", "Fuerza Bruta Detectada", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
            else
            {
                System.Windows.MessageBox.Show($"Contraseña incorrecta. Llevas {_deleteKeyFailedAttempts} fallos. Al llegar a 4 serás bloqueado exponencialmente.", "Seguridad", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            }
            return;
        }

        _deleteKeyFailedAttempts = 0;

        var result = System.Windows.MessageBox.Show("¿Seguro que deseas olvidar esta llave del historial?\n\nAl olvidarla podrás reescribir el USB con una nueva llave.", "Borrar Llave", System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Question);
        if (result == System.Windows.MessageBoxResult.Yes)
        {
            var history = new System.Collections.Generic.List<string>(Config.ConfiguredUsbHistory);
            history.Remove(historyItem);
            Config.ConfiguredUsbHistory = history;
            
            if (Config.ConfiguredUsbHistory.Count == 0)
            {
                bool showDoubleAlerts = false;

                if (IsArmed)
                {
                    IsArmed = false;
                    UpdateStatusText();
                    System.Windows.MessageBox.Show("\nHas eliminado tu última Llave Autorizada mientras la computadora estaba protegida.\n\nPor seguridad preventiva, el Sistema se ha DESARMADO automáticamente. Si hubieras retirado la USB ahora, habrías perdido el acceso al equipo.", "Escudo Revocado", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                    showDoubleAlerts = true;
                }

                if (StartArmed)
                {
                    StartArmed = false;
                    if (!showDoubleAlerts)
                    {
                        System.Windows.MessageBox.Show("\nHas eliminado la última Llave Autorizada.\n\nEl interruptor de 'Armado Automático' en Ajustes del Sistema ha sido Apagado automáticamente para evitar un bloqueo letal en tu próximo reinicio de computadora.", "Seguridad Preventiva", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                    }
                }
            }

            SaveSettings();
            OnPropertyChanged(nameof(Config));
        }
    }

    [RelayCommand]
    private void GenerateMasterKey()
    {
        string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        string key = System.Security.Cryptography.RandomNumberGenerator.GetString(chars, 12);
        
        string newKey = $"MSTR-{key.Substring(0, 4)}-{key.Substring(4, 4)}-{key.Substring(8, 4)}";
        MasterKeyText = newKey;
    }

    [RelayCommand]
    private void CopyMasterKey()
    {
        if (!string.IsNullOrEmpty(MasterKeyText))
        {
            System.Windows.Clipboard.SetText(MasterKeyText);
            System.Windows.MessageBox.Show("Llave Maestra copiada al portapapeles.", "Copiado", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        }
    }

    [RelayCommand]
    private void ExportMasterKey()
    {
        if (string.IsNullOrEmpty(MasterKeyText))
        {
            System.Windows.MessageBox.Show("Aún no tienes una Llave Maestra. Genera una primero.", "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            return;
        }

        var saveDialog = new Microsoft.Win32.SaveFileDialog
        {
            Filter = "Archivo de texto (*.txt)|*.txt",
            Title = "Guardar Llave Maestra",
            FileName = "USBGuard_MasterKey.txt"
        };

        if (saveDialog.ShowDialog() == true)
        {
            System.IO.File.WriteAllText(saveDialog.FileName, $"USB GUARD - LLAVE MAESTRA DE RECUPERACION\n\nGuarda este archivo en un lugar seguro temporalmente.\nSi pierdes tus memorias USB, usa este código para acceder a tu máquina.\n\nLlave Maestra: {MasterKeyText}\n\nFecha de generación: {DateTime.Now}\n");
            System.Windows.MessageBox.Show("La Llave Maestra se ha guardado exitosamente.", "Guardado", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        }
    }

    private async void OnDeviceArrived(object? sender, UsbDevice device)
    {
        await System.Threading.Tasks.Task.Delay(500).ConfigureAwait(false);

        App.Current.Dispatcher.Invoke(() =>
        {
            foreach (var drive in _usbMonitor.GetConnectedRemovableDrives())
            {
                if (!ConnectedDrives.Any(d => d.DriveLetter == drive.DriveLetter))
                {
                    ConnectedDrives.Add(drive);
                }
            }
            if (SelectedDrive == null) SelectedDrive = ConnectedDrives.FirstOrDefault();
            
            if (IsArmed)
            {
                bool hasKey = ConnectedDrives.Any(d => ValidateKey(d));
                if (hasKey)
                {
                    _lockService.Unlock();
                }
            }
        });
    }

    private void OnDeviceRemoved(object? sender, UsbDevice device)
    {
        App.Current.Dispatcher.Invoke(() =>
        {
            var drive = ConnectedDrives.FirstOrDefault(d => d.DriveLetter == device.DriveLetter);
            if (drive != null) ConnectedDrives.Remove(drive);

            if (IsArmed)
            {
                CheckCurrentDrivesForUnlockKey();
            }
        });
    }

    private void CheckCurrentDrivesForUnlockKey()
    {
        if (!IsArmed) return;

        bool hasValidKey = false;
        foreach (var drive in ConnectedDrives)
        {
            if (ValidateKey(drive))
            {
                hasValidKey = true;
                break;
            }
        }

        if (!hasValidKey)
        {
            _lockService.Lock(Config.LockMode);
        }
        else
        {
            _lockService.Unlock();
        }
    }

    private bool ValidateKey(UsbDevice device)
    {
        if (string.IsNullOrEmpty(Config.EncryptionKey)) return false;
        
        if (Config.ConfiguredUsbHistory == null || Config.ConfiguredUsbHistory.Count == 0) return false;

        string serialTarget = $"Serial: {(string.IsNullOrWhiteSpace(device.SerialNumber) ? "No expuesto por hardware" : device.SerialNumber)}";
        bool isAuthorizedInHistory = Config.ConfiguredUsbHistory.Any(h => h.Contains(serialTarget, StringComparison.OrdinalIgnoreCase));
        
        if (!isAuthorizedInHistory) return false;

        return _keyFileService.ValidateKeyFile(device.DriveLetter, Config.EncryptionKey);
    }

    private void UpdateStatusText()
    {
        StatusText = IsArmed ? "Armado" : "Desarmado";
    }

    public void SaveSettings()
    {
        _configService.Save();
        _systemManager.SetRunAtStartup(Config.StartWithWindows);
    }
}
