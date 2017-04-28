using FactorioSupervisor.Extensions;
using FactorioSupervisor.Helpers;
using FactorioSupervisor.Models;
using FactorioSupervisor.Properties;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;

namespace FactorioSupervisor.ViewModels
{
    public class ConfigVm : ObservableObject
    {
        public ConfigVm()
        {
            Logger.WriteLine($"Class created: {nameof(ConfigVm)}");

            if (LoadUserSettingsCmd.CanExecute(null))
                LoadUserSettingsCmd.Execute(null);

            if (GetCurrentFactorioBranchCmd.CanExecute(null))
                GetCurrentFactorioBranchCmd.Execute(null);
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
        private string _currentFactorioBranch;
        private double _uiPosLeft;
        private double _uiPosTop;
        private double _uiDimWidth;
        private double _uiDimHeight;
        private GridLength _uiDimModListWidth;
        private GridLength _uiDimModDetailsWidth;
        private RelayCommand _loadUserSettingsCmd;
        private RelayCommand _saveUserSettingsCmd;
        private RelayCommand _getCurrentFactorioBranchCmd;
        private RelayCommand _resetWindowPosDimCmd;

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
        /// Gets or sets the currently installed branch version of factorio
        /// </summary>
        public string CurrentFactorioBranch
        {
            get { return _currentFactorioBranch; }
            set { if (value == _currentFactorioBranch) return; _currentFactorioBranch = value; OnPropertyChanged(nameof(CurrentFactorioBranch)); }
        }

        /// <summary>
        /// Gets or sets the UI window position left
        /// </summary>
        public double UiPosLeft
        {
            get { return _uiPosLeft; }
            set { if (value == _uiPosLeft) return; _uiPosLeft = value; OnPropertyChanged(nameof(UiPosLeft)); }
        }

        /// <summary>
        /// Gets or sets the UI window position top
        /// </summary>
        public double UiPosTop
        {
            get { return _uiPosTop; }
            set { if (value == _uiPosTop) return; _uiPosTop = value; OnPropertyChanged(nameof(UiPosTop)); }
        }

        /// <summary>
        /// Gets or sets the UI window width
        /// </summary>
        public double UiDimWidth
        {
            get { return _uiDimWidth; }
            set { if (value == _uiDimWidth) return; _uiDimWidth = value; OnPropertyChanged(nameof(UiDimWidth)); }
        }

        /// <summary>
        /// Gets or sets the UI window height
        /// </summary>
        public double UiDimHeight
        {
            get { return _uiDimHeight; }
            set { if (value == _uiDimHeight) return; _uiDimHeight = value; OnPropertyChanged(nameof(UiDimHeight)); }
        }

        /// <summary>
        /// Gets or sets the UI mod list width
        /// </summary>
        public GridLength UiDimModListWidth
        {
            get { return _uiDimModListWidth; }
            set { if (value == _uiDimModListWidth) return; _uiDimModListWidth = value; OnPropertyChanged(nameof(UiDimModListWidth)); }
        }

        /// <summary>
        /// Gets or sets the UI mod details width
        /// </summary>
        public GridLength UiDimModDetailsWidth
        {
            get { return _uiDimModDetailsWidth; }
            set { if (value == _uiDimModDetailsWidth) return; _uiDimModDetailsWidth = value; OnPropertyChanged(nameof(UiDimModDetailsWidth)); }
        }

        /*
         * Commands
         */

        public RelayCommand LoadUserSettingsCmd => _loadUserSettingsCmd ??
            (_loadUserSettingsCmd = new RelayCommand(Execute_LoadUserSettingsCmd, p => true));

        public RelayCommand SaveUserSettingsCmd => _saveUserSettingsCmd ??
            (_saveUserSettingsCmd = new RelayCommand(Execute_SaveUserSettingsCmd, p => true));

        public RelayCommand GetCurrentFactorioBranchCmd => _getCurrentFactorioBranchCmd ??
            (_getCurrentFactorioBranchCmd = new RelayCommand(Execute_GetCurrentFactorioBranchCmd, p => true));

        public RelayCommand ResetWindowPosDimCmd => _resetWindowPosDimCmd ??
            (_resetWindowPosDimCmd = new RelayCommand(Execute_ResetWindowPosDimCmd, p => true));

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

            // UI
            UiPosLeft = settings.UiPosLeft;
            UiPosTop = settings.UiPosTop;
            UiDimWidth = settings.UiDimWidth;
            UiDimHeight = settings.UiDimHeight;
            UiDimModListWidth = settings.UiDimModListWidth;
            UiDimModDetailsWidth = settings.UiDimModDetailsWidth;

            FactorioExePath = settings.FactorioExePath;
            ModsPath = settings.ModsPath;
            ModPortalUsername = settings.ModPortalUsername;
            ModPortalPassword = settings.ModPortalPassword;
            AutoCheckModUpdate = settings.AutoCheckModUpdate;
            AutoDownloadModUpdate = settings.AutoDownloadModUpdate;
            CurrentFactorioBranch = settings.CurrentFactorioBranch;

            Logger.WriteLine("Loaded user settings", true);
        }

        private void Execute_SaveUserSettingsCmd(object obj)
        {
            var settings = Settings.Default;

            // UI
            settings.UiPosLeft = UiPosLeft;
            settings.UiPosTop = UiPosTop;
            settings.UiDimWidth = UiDimWidth;
            settings.UiDimHeight = UiDimHeight;
            settings.UiDimModListWidth = UiDimModListWidth;
            settings.UiDimModDetailsWidth = UiDimModDetailsWidth;

            settings.FactorioExePath = FactorioExePath;
            settings.ModsPath = ModsPath;
            settings.ModPortalUsername = ModPortalUsername;
            settings.ModPortalPassword = ModPortalPassword;
            settings.SelectedProfile = BaseVm.ProfilesVm.SelectedProfile.Name;
            settings.AutoCheckModUpdate = AutoCheckModUpdate;
            settings.AutoDownloadModUpdate = AutoDownloadModUpdate;
            settings.CurrentFactorioBranch = CurrentFactorioBranch;

            settings.Save();

            Logger.WriteLine("Saved user settings", true);
        }

        private void Execute_GetCurrentFactorioBranchCmd(object obj)
        {
            var factorioBaseFilename = Path.Combine(RegistryHelper.GetSteamPath(), @"steamapps\common\Factorio\data\base\info.json");

            if (File.Exists(factorioBaseFilename))
            {
                string infoJsonStr = null;
                Exception exception = null;
                InfoJson infoJson = null;

                try
                {
                    infoJsonStr = File.ReadAllText(factorioBaseFilename).Trim();
                }
                catch (Exception ex)
                {
                    exception = ex;
                    Logger.WriteLine($"Failed to read from file: {factorioBaseFilename}", true, ex);
                }
                finally
                {
                    Logger.WriteLine(infoJsonStr);

                    if (exception == null)
                        infoJson = JsonConvert.DeserializeObject<InfoJson>(infoJsonStr);
                }

                if (infoJson != null)
                {
                    // Only get major and minor version number (x.xx)
                    var match = Regex.Match(infoJson.Version, "(\\d\\.\\d\\d)\\.\\d*");

                    if (match.Success)
                    {
                        CurrentFactorioBranch = match.Groups[1].Value;
                        Logger.WriteLine($"Retreived current Factorio branch version: {CurrentFactorioBranch}", true);
                    }
                    else
                        Logger.WriteLine($"Unable to retreive current Factorio branch version", true);
                }

            }
            else
            {
                Logger.WriteLine($"Unable to find Factorio base mod info.json file at path: {factorioBaseFilename}", true);
            }
        }

        private void Execute_ResetWindowPosDimCmd(object obj)
        {
            var settings = Settings.Default;

            UiPosLeft = settings.Properties.GetDefault<double>("UiPosLeft");
            UiPosTop = settings.Properties.GetDefault<double>("UiPosTop");
            UiDimWidth = settings.Properties.GetDefault<double>("UiDimWidth");
            UiDimHeight = settings.Properties.GetDefault<double>("UiDimHeight");
            UiDimModListWidth = new GridLength(1.2, GridUnitType.Star);
            UiDimModDetailsWidth = new GridLength(2.5, GridUnitType.Star);
        }
    }
}
