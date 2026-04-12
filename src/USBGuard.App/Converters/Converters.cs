using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace USBGuard.App.Converters;

public class BoolToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool isArmed)
        {
            return isArmed ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.Red);
        }
        return new SolidColorBrush(Colors.Gray);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class BoolToArmButtonTextConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool isArmed)
        {
            return isArmed ? "Desarmar" : "Armar";
        }
        return "Desarmar";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class EnumToIndexConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (int)value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return Enum.ToObject(targetType, value);
    }
}

public class ArmButtonAppearanceConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool isArmed)
        {
            return isArmed ? Wpf.Ui.Controls.ControlAppearance.Danger : Wpf.Ui.Controls.ControlAppearance.Primary;
        }
        return Wpf.Ui.Controls.ControlAppearance.Primary;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
