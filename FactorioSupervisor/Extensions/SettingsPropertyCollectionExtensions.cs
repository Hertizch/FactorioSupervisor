using FactorioSupervisor.Properties;
using System;
using System.Configuration;

namespace FactorioSupervisor.Extensions
{
    public static class SettingsPropertyCollectionExtensions
    {
        public static T GetDefault<T>(this SettingsPropertyCollection spc, string property)
        {
            var valString = (string)Settings.Default.Properties[property].DefaultValue;
            return (T)Convert.ChangeType(valString, typeof(T));
        }
    }
}
