using System;
using System.Globalization;
using System.Windows.Data;

namespace FactorioSupervisor.Converters
{
    public class IsNewFactorioVersionToStringConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null)
                return null;

            if (values.Length <= 2)
                return null;

            var localVersion = Version.Parse((string)values[0]);
            var remoteVersion = Version.Parse((string)values[1]);

            string output = null;

            if (remoteVersion > localVersion)
                output = $" - Now compatible with Factorio {remoteVersion.ToString()}!";

            return output;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
