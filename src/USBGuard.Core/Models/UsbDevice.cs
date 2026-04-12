namespace USBGuard.Core.Models;

public class UsbDevice
{
    public string DriveLetter { get; set; } = string.Empty;
    public string VolumeName { get; set; } = string.Empty;
    public string SerialNumber { get; set; } = string.Empty;
    public string DeviceId { get; set; } = string.Empty;
}
