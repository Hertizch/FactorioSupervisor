using FactorioSupervisor.Models;
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
            var dependency = (Dependency)values[2];

            double output = 0;

            if (isUpdating)
            {
                if (mod != null)
                    output = mod.ProgressPercentage;

                if (dependency != null)
                    output = dependency.ProgressPercentage;
            }
            else
                output = 0;

            return output;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
