using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace USBGuard.Core.Services;

public interface ISystemManagerService
{
    void SetRunAtStartup(bool enable);
    void SetSystemMute(bool mute);
}

public class SystemManagerService : ISystemManagerService
{
    private const string AppName = "USBGuard";
    private const string RegistryRunPath = @"Software\Microsoft\Windows\CurrentVersion\Run";
    
    [ComImport]
    [Guid("A95664D2-9614-4F35-A746-DE8DB63617E6")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    private interface IMMDeviceEnumerator
    {
        int EnumAudioEndpoints(int dataFlow, int stateMask, out IntPtr ppDevices);
        [PreserveSig]
        int GetDefaultAudioEndpoint(int dataFlow, int role, out IMMDevice ppEndpoint);
    }

    [ComImport]
    [Guid("D666063F-1587-4E43-81F1-B948E807363F")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    private interface IMMDevice
    {
        [PreserveSig]
        int Activate(ref Guid iid, int dwClsCtx, IntPtr pActivationParams, out IAudioEndpointVolume ppInterface);
    }

    [ComImport]
    [Guid("5CDF2C82-841E-4546-9722-0CF74078229A")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    private interface IAudioEndpointVolume
    {
        int NotImpl1();
        int NotImpl2();
        int NotImpl3();
        int NotImpl4();
        int NotImpl5();
        int NotImpl6();
        int NotImpl7();
        int NotImpl8();
        int NotImpl9();
        int NotImpl10();
        int NotImpl11();
        int NotImpl12();
        int NotImpl13();
        [PreserveSig]
        int SetMute([MarshalAs(UnmanagedType.Bool)] bool bMute, IntPtr pguidEventContext);
        [PreserveSig]
        int GetMute(out bool pbMute);
    }

    private static readonly Guid IID_IAudioEndpointVolume = typeof(IAudioEndpointVolume).GUID;

    public void SetRunAtStartup(bool enable)
    {
        try
        {
            using RegistryKey? key = Registry.CurrentUser.OpenSubKey(RegistryRunPath, true);
            if (key == null) return;

            if (enable)
            {
                string appPath = Process.GetCurrentProcess().MainModule?.FileName ?? "";
                key.SetValue(AppName, $"\"{appPath}\" --hidden");
            }
            else
            {
                if (key.GetValue(AppName) != null)
                {
                    key.DeleteValue(AppName);
                }
            }
        }
        catch (Exception)
        {
            // Logging
        }
    }

    public void SetSystemMute(bool mute)
    {
        try
        {
            Type? deviceEnumeratorType = Type.GetTypeFromCLSID(new Guid("BCDE0395-E52F-467C-8E3D-C4579291692E"));
            if (deviceEnumeratorType == null) return;

            IMMDeviceEnumerator? deviceEnumerator = (IMMDeviceEnumerator?)Activator.CreateInstance(deviceEnumeratorType);
            if (deviceEnumerator == null) return;
            
            deviceEnumerator.GetDefaultAudioEndpoint(0, 1, out IMMDevice device);
            if (device == null) return;

            var iid = IID_IAudioEndpointVolume;
            device.Activate(ref iid, 1, IntPtr.Zero, out IAudioEndpointVolume endpointVolume);
            
            if (endpointVolume != null)
            {
                endpointVolume.SetMute(mute, IntPtr.Zero);
                Marshal.ReleaseComObject(endpointVolume);
            }

            Marshal.ReleaseComObject(device);
            Marshal.ReleaseComObject(deviceEnumerator);
        }
        catch (Exception)
        {
            // Fallback fail silently
        }
    }
}
