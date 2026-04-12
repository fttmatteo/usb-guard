using System;
using System.Collections.Generic;
using System.IO;
using System.Management;
using System.Runtime.InteropServices;
using USBGuard.Core.Interop;
using USBGuard.Core.Models;

namespace USBGuard.Core.Services;

public interface IUsbMonitorService
{
    event EventHandler<UsbDevice> DeviceArrived;
    event EventHandler<UsbDevice> DeviceRemoved;
    void StartMonitoring(IntPtr windowHandle);
    void StopMonitoring();
    void HandleDeviceChange(int msg, IntPtr wParam, IntPtr lParam);
    IEnumerable<UsbDevice> GetConnectedRemovableDrives();
}

public class UsbMonitorService : IUsbMonitorService
{
    private IntPtr _notificationHandle;

    public event EventHandler<UsbDevice>? DeviceArrived;
    public event EventHandler<UsbDevice>? DeviceRemoved;

    public void StartMonitoring(IntPtr windowHandle)
    {
        NativeMethods.DEV_BROADCAST_HDR dbi = new NativeMethods.DEV_BROADCAST_HDR
        {
            dbch_devicetype = NativeMethods.DBT_DEVTYP_VOLUME,
            dbch_reserved = 0
        };
        dbi.dbch_size = (uint)Marshal.SizeOf(dbi);

        IntPtr buffer = Marshal.AllocHGlobal((int)dbi.dbch_size);
        Marshal.StructureToPtr(dbi, buffer, true);

        _notificationHandle = NativeMethods.RegisterDeviceNotificationW(windowHandle, buffer, NativeMethods.DEVICE_NOTIFY_WINDOW_HANDLE);
        Marshal.FreeHGlobal(buffer);
    }

    public void StopMonitoring()
    {
        if (_notificationHandle != IntPtr.Zero)
        {
            NativeMethods.UnregisterDeviceNotification(_notificationHandle);
            _notificationHandle = IntPtr.Zero;
        }
    }

    public void HandleDeviceChange(int msg, IntPtr wParam, IntPtr lParam)
    {
        if (msg != NativeMethods.WM_DEVICECHANGE) return;

        int eventType = wParam.ToInt32();
        if (eventType == NativeMethods.DBT_DEVICEARRIVAL || eventType == NativeMethods.DBT_DEVICEREMOVECOMPLETE)
        {
            var hdr = Marshal.PtrToStructure<NativeMethods.DEV_BROADCAST_HDR>(lParam);
            if (hdr.dbch_devicetype == NativeMethods.DBT_DEVTYP_VOLUME)
            {
                var vol = Marshal.PtrToStructure<NativeMethods.DEV_BROADCAST_VOLUME>(lParam);
                string driveLetter = GetDriveLetterFromMask(vol.dbcv_unitmask);

                var device = new UsbDevice { DriveLetter = driveLetter };

                if (eventType == NativeMethods.DBT_DEVICEARRIVAL)
                {
                    device.SerialNumber = GetSerialNumber(driveLetter);
                    try
                    {
                        var driveInfo = new DriveInfo(driveLetter);
                        if (driveInfo.IsReady) device.VolumeName = driveInfo.VolumeLabel;
                    }
                    catch { } // Silent fallback
                    
                    DeviceArrived?.Invoke(this, device);
                }
                else
                {
                    DeviceRemoved?.Invoke(this, device);
                }
            }
        }
    }

    private string GetDriveLetterFromMask(uint mask)
    {
        for (int i = 0; i < 26; i++)
        {
            if ((mask & 1) == 1)
            {
                return (char)('A' + i) + ":\\";
            }
            mask >>= 1;
        }
        return string.Empty;
    }

    public IEnumerable<UsbDevice> GetConnectedRemovableDrives()
    {
        foreach (var drive in DriveInfo.GetDrives())
        {
            if (drive.DriveType == DriveType.Removable && drive.IsReady)
            {
                yield return new UsbDevice
                {
                    DriveLetter = drive.Name,
                    VolumeName = drive.VolumeLabel,
                    SerialNumber = GetSerialNumber(drive.Name)
                };
            }
        }
    }

    private string GetSerialNumber(string driveLetter)
    {
        try
        {
            string driveRoot = driveLetter.TrimEnd('\\');
            var searcher = new ManagementObjectSearcher($"SELECT VolumeSerialNumber FROM Win32_LogicalDisk WHERE Name='{driveRoot}'");
            foreach (var obj in searcher.Get())
            {
                var serial = obj["VolumeSerialNumber"]?.ToString();
                if (serial != null) return serial;
            }
        }
        catch
        {
            // Silent error
        }
        return string.Empty;
    }
}
