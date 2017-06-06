using FactorioSupervisor.Models;
using FactorioSupervisor.ViewModels;
using System;
using System.Globalization;
using System.Windows.Data;

namespace FactorioSupervisor.Converters.SnackbarValueConverters
{
    public class ProgressPercentageConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var isUpdating = (bool)values[0];
            var mod = (Mod)values[1];

            if (isUpdating)
            {
                if (mod != null)
                    return mod.ProgressPercentage;
            }

            return (double)0;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
