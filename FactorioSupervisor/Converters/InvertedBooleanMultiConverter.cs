using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace FactorioSupervisor.Converters
{
    public class InvertedBooleanMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return values.All(value => (!(value is bool)) || !(bool) value);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
