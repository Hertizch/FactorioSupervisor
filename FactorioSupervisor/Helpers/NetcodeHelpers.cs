using System.Net;
using System.Threading.Tasks;

namespace FactorioSupervisor.Helpers
{
    public static class NetcodeHelpers
    {
        public static async Task<bool> VerifyInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                {
                    using (await client.OpenReadTaskAsync("http://www.google.com")) return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
