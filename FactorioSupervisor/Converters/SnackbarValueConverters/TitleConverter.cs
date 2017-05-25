using FactorioSupervisor.Models;
using System;
using System.Globalization;
using System.Windows.Data;

namespace FactorioSupervisor.Converters.SnackbarValueConverters
{
    public class TitleConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var isUpdating = (bool)values[0];
            var mod = (Mod)values[1];
            var dependency = (Dependency)values[2];

            string output = null;

            if (isUpdating)
            {
                if (mod != null)
                    output = $"Updating: {mod.Title} - {mod.ProgressPercentage}%";

                if (dependency != null)
                    output = $"Installing: {dependency.Name} - {dependency.ProgressPercentage}%";
            }
            else
                output = "Warming up...";

            return output;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
