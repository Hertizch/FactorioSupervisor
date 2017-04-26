using Microsoft.Win32;

namespace FactorioSupervisor.Helpers
{
    public static class RegistryHelper
    {
        /// <summary>
        /// Gets the Steam path from the registry
        /// </summary>
        /// <returns>Absolute path</returns>
        public static string GetSteamPath()
        {
            string path = null;

            using (var registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Valve\\Steam"))
            {
                var value = registryKey?.GetValue("SteamPath");
                if (value != null)
                    path = value.ToString().Replace(@"/", @"\");
            }

            return path;
        }
    }
}
