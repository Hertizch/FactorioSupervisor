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

            FactorioExePath = settings.FactorioExePath;
            ModsPath = settings.ModsPath;
            ModPortalUsername = settings.ModPortalUsername;
            ModPortalPassword = settings.ModPortalPassword;

            Logger.WriteLine("Loaded user settings", true);
        }

        private void Execute_SaveUserSettingsCmd(object obj)
        {
            var settings = Settings.Default;

            settings.FactorioExePath = FactorioExePath;
            settings.ModsPath = ModsPath;
            settings.ModPortalUsername = ModPortalUsername;
            settings.ModPortalPassword = ModPortalPassword;

            settings.Save();

            Logger.WriteLine("Saved user settings", true);
        }
    }
}
