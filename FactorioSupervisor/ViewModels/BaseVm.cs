using System.ComponentModel;
using FactorioSupervisor.Helpers;
using FactorioSupervisor.Relays;
using System.Reflection;
using System.Windows;

namespace FactorioSupervisor.ViewModels
{
    public class BaseVm
    {
        public BaseVm()
        {
            Logger.WriteLine($"Class created: {nameof(BaseVm)}");
            Logger.WriteLine($"Factorio Supervisor startup version: '{Assembly.GetExecutingAssembly().GetName().Version}'", true);
            Logger.WriteLine($"Windows OS version: '{EnvironmentHelpers.GetOsFriendlyName()}'", true);

            // Create relays
            NotifyBannerRelay = new NotifyBannerRelay();

            // Create view models
            ConfigVm = new ConfigVm();
            ModsVm = new ModsVm();
            ProfilesVm = new ProfilesVm();
        }

        public static bool DesignMode { get; set; } = DesignerProperties.GetIsInDesignMode(new DependencyObject());

        public static NotifyBannerRelay NotifyBannerRelay { get; private set; }

        public static ConfigVm ConfigVm { get; private set; }

        public static ModsVm ModsVm { get; private set; }

        public static ProfilesVm ProfilesVm { get; private set; }
    }
}
