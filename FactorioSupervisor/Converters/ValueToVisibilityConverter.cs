using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace FactorioSupervisor.Converters
{
    public class ValueToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter != null)
                if (value != null && ((string)parameter == "inverted" && (bool)value))
                    return Visibility.Collapsed;

            // If object is null
            if (value == null)
                return Visibility.Collapsed;

            // If object is string
            if (value is string)
            {
                if (string.IsNullOrWhiteSpace((string)value))
                    return Visibility.Collapsed;
            }

            // If object is boolean
            if (value as bool? == false)
                return Visibility.Collapsed;

            // If value is integer
            if (value as int? == 0)
                return Visibility.Collapsed;

            // If object is JArray
            if (value is JArray)
            {
                var jArray = (JArray) value;
                jArray.ToObject<List<string>>();

                if (jArray.Count <= 0)
                    return Visibility.Collapsed;
            }

            // If object is JObject
            if (value is JObject)
            {
                var jObject = (JObject) value;
                jObject.ToObject<string>();
            }

            // If none of the above, make it visible
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
