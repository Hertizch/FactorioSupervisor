using FactorioSupervisor.Helpers;
using System.Reflection;

namespace FactorioSupervisor.ViewModels
{
    public class BaseVm
    {
        public BaseVm()
        {
            Logger.WriteLine($"Class created: {nameof(BaseVm)}");
            Logger.WriteLine("\n\n", true);
            Logger.WriteLine($"Factorio Supervisor startup version {Assembly.GetExecutingAssembly().GetName().Version.ToString()}", true);
            Logger.WriteLine($"Windows OS version {EnvironmentHelpers.GetOsFriendlyName()}", true);

            ConfigVm = new ConfigVm();
            ModsVm = new ModsVm();
            ProfilesVm = new ProfilesVm();
        }

        public static ConfigVm ConfigVm { get; private set; }

        public static ModsVm ModsVm { get; private set; }

        public static ProfilesVm ProfilesVm { get; private set; }
    }
}
