using System.Net;

namespace FactorioSupervisor.Helpers
{
    public static class NetcodeHelpers
    {
        public static bool HasInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                {
                    using (var stream = client.OpenRead("http://www.google.com"))
                    {
                        return true;
                    }
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
