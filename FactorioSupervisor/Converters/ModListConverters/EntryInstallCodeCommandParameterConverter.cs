using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace FactorioSupervisor.Converters.ModListConverters
{
    public class EntryInstallCodeCommandParameterConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return values[2] == DependencyProperty.UnsetValue ? null : $"{(string) values[0]}.{(string) values[1]}.{(string) values[2]}";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
