using Microsoft.Win32;

namespace FactorioSupervisor.Helpers
{
    public static class EnvironmentHelpers
    {
        public static string HKLM_GetString(string path, string key)
        {
            try
            {
                var rk = Registry.LocalMachine.OpenSubKey(path);
                if (rk == null) return "";
                return (string)rk.GetValue(key);
            }
            catch { return ""; }
        }

        public static string GetOsFriendlyName()
        {
            var productName = HKLM_GetString(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ProductName");
            var csdVersion = HKLM_GetString(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "CSDVersion");
            if (productName != "")
            {
                return (productName.StartsWith("Microsoft") ? "" : "Microsoft ") + productName +
                            (csdVersion != "" ? " " + csdVersion : "").Trim();
            }
            return "";
        }
    }
}
