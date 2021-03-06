﻿using System;
using System.Windows;

namespace FactorioSupervisor.Helpers
{
    public static class ResourceHelper
    {
        public static string GetValue(string value)
        {
            var resourceDictionary = new ResourceDictionary()
            {
                Source = new Uri("/FactorioSupervisor;component/Resources/Strings.xaml", UriKind.RelativeOrAbsolute)
            };

            return resourceDictionary[value].ToString();
        }
    }
}
