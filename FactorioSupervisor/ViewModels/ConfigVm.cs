using FactorioSupervisor.Extensions;
using FactorioSupervisor.Helpers;
using FactorioSupervisor.Models;
using FactorioSupervisor.Properties;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Forms;

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

        private string _factorioPath;
        private string _modsPath;
        private string _modPortalUsername;
        private string _modPortalPassword;
        private string _modPortalAuthToken;
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
        private RelayCommand _resetAllUserSettingsCmd;

        /*
         * Properties
         */

        #region Properties

        /// <summary>
        /// Gets or sets the factorio exe full path
        /// </summary>
        public string FactorioPath
        {
            get => _factorioPath; set { if (value == _factorioPath) return; _factorioPath = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Gets or sets the factorio mods full path
        /// </summary>
        public string ModsPath
        {
            get => _modsPath; set { if (value == _modsPath) return; _modsPath = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Gets or sets the factorio mod portal authentication username
        /// </summary>
        public string ModPortalUsername
        {
            get => _modPortalUsername; set { if (value == _modPortalUsername) return; _modPortalUsername = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Gets or sets the factorio mod portal authentication password
        /// </summary>
        public string ModPortalPassword
        {
            get => _modPortalPassword; set { if (value == _modPortalPassword) return; _modPortalPassword = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Gets or sets the factorio mod portal authentication token
        /// </summary>
        public string ModPortalAuthToken
        {
            get => _modPortalAuthToken; set { if (value == _modPortalAuthToken) return; _modPortalAuthToken = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Gets or sets if the application should automatically check for mod updates at startup
        /// </summary>
        public bool AutoCheckModUpdate
        {
            get => _autoCheckModUpdate; set { if (value == _autoCheckModUpdate) return; _autoCheckModUpdate = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Gets or sets if the application should automatically download mod updates when available
        /// </summary>
        public bool AutoDownloadModUpdate
        {
            get => _autoDownloadModUpdate; set { if (value == _autoDownloadModUpdate) return; _autoDownloadModUpdate = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Gets or sets the currently installed branch version of factorio
        /// </summary>
        public string CurrentFactorioBranch
        {
            get => _currentFactorioBranch; set { if (value == _currentFactorioBranch) return; _currentFactorioBranch = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Gets or sets the UI window position left
        /// </summary>
        public double UiPosLeft
        {
            get => _uiPosLeft; set { if (Math.Abs(value - _uiPosLeft) < 0.01) return; _uiPosLeft = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Gets or sets the UI window position top
        /// </summary>
        public double UiPosTop
        {
            get => _uiPosTop; set { if (Math.Abs(value - _uiPosTop) < 0.01) return; _uiPosTop = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Gets or sets the UI window width
        /// </summary>
        public double UiDimWidth
        {
            get => _uiDimWidth; set { if (Math.Abs(value - _uiDimWidth) < 0.01) return; _uiDimWidth = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Gets or sets the UI window height
        /// </summary>
        public double UiDimHeight
        {
            get => _uiDimHeight; set { if (Math.Abs(value - _uiDimHeight) < 0.01) return; _uiDimHeight = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Gets or sets the UI mod list width
        /// </summary>
        public GridLength UiDimModListWidth
        {
            get => _uiDimModListWidth; set { if (value == _uiDimModListWidth) return; _uiDimModListWidth = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Gets or sets the UI mod details width
        /// </summary>
        public GridLength UiDimModDetailsWidth
        {
            get => _uiDimModDetailsWidth; set { if (value == _uiDimModDetailsWidth) return; _uiDimModDetailsWidth = value; OnPropertyChanged(); }
        }

        #endregion

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

        public RelayCommand ResetAllUserSettingsCmd => _resetAllUserSettingsCmd ??
            (_resetAllUserSettingsCmd = new RelayCommand(Execute_ResetAllUserSettingsCmd, p => true));

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

            // If the window position is larger then the available resolution, or at the default setting (99999) - reset to center primary screen
            if (UiPosLeft > ScreenHelpers.GetScreensWorkingAreaWidth())
                UiPosLeft = ScreenHelpers.GetWindowPosLeft(Screen.PrimaryScreen, settings.Properties.GetDefault<double>("UiDimWidth"));

            if (UiPosTop > ScreenHelpers.GetScreensWorkingAreaHeight())
                UiPosTop = ScreenHelpers.GetWindowPosTop(Screen.PrimaryScreen, settings.Properties.GetDefault<double>("UiDimHeight"));

            UiDimWidth = settings.UiDimWidth;
            UiDimHeight = settings.UiDimHeight;
            UiDimModListWidth = settings.UiDimModListWidth;
            UiDimModDetailsWidth = settings.UiDimModDetailsWidth;

            FactorioPath = settings.FactorioPath;
            ModsPath = settings.ModsPath;
            ModPortalUsername = settings.ModPortalUsername;
            ModPortalAuthToken = settings.ModPortalAuthToken;
            AutoCheckModUpdate = settings.AutoCheckModUpdate;
            AutoDownloadModUpdate = settings.AutoDownloadModUpdate;

            Logger.WriteLine("Loaded user settings");
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

            settings.FactorioPath = FactorioPath;
            settings.ModsPath = ModsPath;
            settings.ModPortalUsername = ModPortalUsername;
            settings.ModPortalAuthToken = ModPortalAuthToken;
            settings.SelectedProfile = BaseVm.ProfilesVm.SelectedProfile.Name;
            settings.AutoCheckModUpdate = AutoCheckModUpdate;
            settings.AutoDownloadModUpdate = AutoDownloadModUpdate;
            settings.HideGroups = BaseVm.ModsVm.HideGroups;
            settings.ModsSortDirection = BaseVm.ModsVm.ModsSortDirection;

            settings.Save();

            Logger.WriteLine("Saved user settings");
        }

        private void Execute_GetCurrentFactorioBranchCmd(object obj)
        {
            var factorioBaseFilename = Path.Combine(FactorioPath, @"data\base\info.json");

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
                    Logger.WriteLine($"[ERROR] Failed to read from file: {factorioBaseFilename}", true, ex);
                }
                finally
                {
                    if (exception == null)
                        Logger.WriteLine($"[INFO] Read file: {factorioBaseFilename}", true);
                }

                // Deserialize json string
                try
                {
                    infoJson = JsonConvert.DeserializeObject<InfoJson>(infoJsonStr);
                }
                catch (Exception ex)
                {
                    Logger.WriteLine($"Failed to deserialize json string from file: {factorioBaseFilename}", true, ex);
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
                        Logger.WriteLine("Unable to retreive current Factorio branch version", true);
                }
                else
                {
                    Logger.WriteLine("Unable to retreive current Factorio branch version", true);
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

            UiPosLeft = ScreenHelpers.GetWindowPosLeft(Screen.PrimaryScreen, settings.Properties.GetDefault<double>("UiDimWidth"));
            UiPosTop = ScreenHelpers.GetWindowPosTop(Screen.PrimaryScreen, settings.Properties.GetDefault<double>("UiDimHeight"));
            UiDimWidth = settings.Properties.GetDefault<double>("UiDimWidth");
            UiDimHeight = settings.Properties.GetDefault<double>("UiDimHeight");
            UiDimModListWidth = new GridLength(1.2, GridUnitType.Star);
            UiDimModDetailsWidth = new GridLength(2.5, GridUnitType.Star);

            Logger.WriteLine("UI user settings reset to default");
        }

        private static void Execute_ResetAllUserSettingsCmd(object obj)
        {
            var p = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "FactorioSupervisor");
            var ss = Directory.GetDirectories(p, "FactorioSupervisor.*");
            foreach (var s in ss)
            {
                Exception exception = null;

                try
                {
                    FileHelpers.DeleteDirectory(s);
                }
                catch (Exception ex)
                {
                    exception = ex;
                    Logger.WriteLine($"[ERROR] Unable to delete folder: '{s}'", true, ex);
                }
                finally
                {
                    if (exception == null)
                        Logger.WriteLine($"[INFO] Deleted folder: '{s}'", true);
                }
            }

            var settings = Settings.Default;
            settings.Reset();

            Logger.WriteLine("All user settings reset to default");
        }
    }
}
