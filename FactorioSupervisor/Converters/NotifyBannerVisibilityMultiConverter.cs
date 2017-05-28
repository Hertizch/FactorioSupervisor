using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace FactorioSupervisor.Converters
{
    public class NotifyBannerVisibilityMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            bool showNotifyBanner = (bool)values[0];
            bool snackbarIsOpen = (bool)values[1];

            if (showNotifyBanner && !snackbarIsOpen)
                return Visibility.Visible;

            if (showNotifyBanner && snackbarIsOpen)
                return Visibility.Collapsed;

            // If none of the above, collapse it
            return Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
