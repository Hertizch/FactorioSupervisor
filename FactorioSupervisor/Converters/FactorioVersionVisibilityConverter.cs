using FactorioSupervisor.ViewModels;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace FactorioSupervisor.Converters
{
    public class FactorioVersionVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            if (value.ToString().Contains("0.14") && BaseVm.ModsVm.HideIncompatibleMods)
                return Visibility.Collapsed;

            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
