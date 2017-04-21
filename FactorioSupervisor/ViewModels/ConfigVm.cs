using FactorioSupervisor.Extensions;
using FactorioSupervisor.Helpers;
using FactorioSupervisor.Properties;
using System.Diagnostics;

namespace FactorioSupervisor.ViewModels
{
    public class ConfigVm : ObservableObject
    {
        public ConfigVm()
        {
            Logger.WriteLine($"Class created: {nameof(ConfigVm)}");

            if (LoadUserSettingsCmd.CanExecute(null))
                LoadUserSettingsCmd.Execute(null);
        }

        /*
         * Private fields
         */

        private string _factorioExePath;
        private string _modsPath;
        private string _modPortalUsername;
        private string _modPortalPassword;
        private bool _autoCheckModUpdate;
        private bool _autoDownloadModUpdate;
        private bool _autoHideNotifyBanner;
        private RelayCommand _loadUserSettingsCmd;
        private RelayCommand _saveUserSettingsCmd;

        /*
         * Properties
         */

        /// <summary>
        /// Gets or sets the factorio exe full path
        /// </summary>
        public string FactorioExePath
        {
            get { return _factorioExePath; }
            set { if (value == _factorioExePath) return; _factorioExePath = value; OnPropertyChanged(nameof(FactorioExePath)); }
        }

        /// <summary>
        /// Gets or sets the factorio mods full path
        /// </summary>
        public string ModsPath
        {
            get { return _modsPath; }
            set { if (value == _modsPath) return; _modsPath = value; OnPropertyChanged(nameof(ModsPath)); }
        }

        /// <summary>
        /// Gets or sets the factorio mod portal authentication username
        /// </summary>
        public string ModPortalUsername
        {
            get { return _modPortalUsername; }
            set { if (value == _modPortalUsername) return; _modPortalUsername = value; OnPropertyChanged(nameof(ModPortalUsername)); }
        }

        /// <summary>
        /// Gets or sets the factorio mod portal authentication password
        /// </summary>
        public string ModPortalPassword
        {
            get { return _modPortalPassword; }
            set { if (value == _modPortalPassword) return; _modPortalPassword = value; OnPropertyChanged(nameof(ModPortalPassword)); }
        }

        /// <summary>
        /// Gets or sets if the application should automatically check for mod updates at startup
        /// </summary>
        public bool AutoCheckModUpdate
        {
            get { return _autoCheckModUpdate; }
            set { if (value == _autoCheckModUpdate) return; _autoCheckModUpdate = value; OnPropertyChanged(nameof(AutoCheckModUpdate)); }
        }

        /// <summary>
        /// Gets or sets if the application should automatically download mod updates when available
        /// </summary>
        public bool AutoDownloadModUpdate
        {
            get { return _autoDownloadModUpdate; }
            set { if (value == _autoDownloadModUpdate) return; _autoDownloadModUpdate = value; OnPropertyChanged(nameof(AutoDownloadModUpdate)); }
        }

        /// <summary>
        /// Gets or sets if the application should automatically hide the notification banner
        /// </summary>
        public bool AutoHideNotifyBanner
        {
            get { return _autoHideNotifyBanner; }
            set { if (value == _autoHideNotifyBanner) return; _autoHideNotifyBanner = value; OnPropertyChanged(nameof(AutoHideNotifyBanner)); }
        }

        /*
         * Commands
         */

        public RelayCommand LoadUserSettingsCmd => _loadUserSettingsCmd ??
            (_loadUserSettingsCmd = new RelayCommand(Execute_LoadUserSettingsCmd, p => true));

        public RelayCommand SaveUserSettingsCmd => _saveUserSettingsCmd ??
            (_saveUserSettingsCmd = new RelayCommand(Execute_SaveUserSettingsCmd, p => true));

        /*
         * Methods
         */

        private void Execute_LoadUserSettingsCmd(object obj)
        {
            var settings = Settings.Default;

            // Persist user settings with version increments
            if (settings.UpdateSettings)
            {
                settings.Upgrade();
                settings.UpdateSettings = false;
                settings.Save();
            }

            FactorioExePath = settings.FactorioExePath;
            ModsPath = settings.ModsPath;
            ModPortalUsername = settings.ModPortalUsername;
            ModPortalPassword = settings.ModPortalPassword;
            AutoCheckModUpdate = settings.AutoCheckModUpdate;
            AutoDownloadModUpdate = settings.AutoDownloadModUpdate;
            AutoHideNotifyBanner = settings.AutoHideNotifyBanner;

            Logger.WriteLine("Loaded user settings", true);
        }

        private void Execute_SaveUserSettingsCmd(object obj)
        {
            var settings = Settings.Default;

            settings.FactorioExePath = FactorioExePath;
            settings.ModsPath = ModsPath;
            settings.ModPortalUsername = ModPortalUsername;
            settings.ModPortalPassword = ModPortalPassword;
            settings.SelectedProfile = BaseVm.ProfilesVm.SelectedProfile.Name;
            settings.AutoCheckModUpdate = AutoCheckModUpdate;
            settings.AutoDownloadModUpdate = AutoDownloadModUpdate;
            settings.AutoHideNotifyBanner = AutoHideNotifyBanner;

            settings.Save();

            Logger.WriteLine("Saved user settings", true);
        }
    }
}
