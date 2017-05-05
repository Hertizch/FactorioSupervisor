using FactorioSupervisor.Models;
using FactorioSupervisor.ViewModels;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace FactorioSupervisor.Converters
{
    public class IsUpdatingToNotifyValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string output = null;
            Mod mod = null;
            bool isUpdating = ((bool)value);

            Debug.WriteLine($"isUpdating: {isUpdating}");

            if (isUpdating && BaseVm.ModsVm.Mods.Any(x => x.IsUpdating))
            {
                Debug.WriteLine($"isUpdating && BaseVm.ModsVm.Mods.Any(x => x.IsUpdating): {isUpdating}");

                mod = BaseVm.ModsVm.Mods.First(x => x.IsUpdating);
                output = $"Updating: {mod.Title} - {mod.ProgressPercentage}%";
            }
               
            return output;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
