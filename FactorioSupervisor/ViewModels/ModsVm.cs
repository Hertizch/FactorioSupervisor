using FactorioSupervisor.Extensions;
using FactorioSupervisor.Helpers;
using FactorioSupervisor.Models;
using FactorioSupervisor.Properties;
using ModsApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FactorioSupervisor.ViewModels
{
    public class ModsVm : ObservableObject
    {
        public ModsVm()
        {
            Logger.WriteLine($"Class created: {nameof(ModsVm)}");

            Mods = new ObservableCollection<Mod>();

            if (GetLocalModsCmd.CanExecute(null))
                GetLocalModsCmd.Execute(null);

            if (Mods.Count <= 0)
            {
                Mods.Add(new Mod
                {
                    Title = "Mod title",
                    Description = "Mod description",
                    InstalledVersion = "0.0.0",
                    RemoteVersion = "0.0.0",
                    FactorioVersion = "0.14",
                    Homepage = "http://www.mod.com",
                    Author = "Mod author"
                });

                SelectedMod = Mods[0];
            }

            // Auto check for mod updates - if user specified
            if (BaseVm.ConfigVm.AutoCheckModUpdate)
            {
                if (GetModRemoteDataCmd.CanExecute(null))
                    GetModRemoteDataCmd.Execute(null);
            }
        }

        /*
         * Private fields
         */

        private readonly List<string> _unwantedFileExtensions = new List<string> { ".db", ".json", ".txt", ".rar" };
        private ObservableCollection<Mod> _mods;
        private Mod _selectedMod;
        private bool _isCheckingForUpdates;
        private bool _isUpdating;
        private bool _isUpdatesAvailable;
        private bool _showProgressBar;
        private bool _showNotifyBanner;
        private string _notifyBannerValue;
        private bool _fileDownloadSucceeded;
        private Stopwatch _stopwatch = null;
        private RelayCommand _getLocalModsCmd;
        private RelayCommand _toggleEnableModsCmd;
        private RelayCommand _openHyperlinkCmd;
        private RelayCommand _getModRemoteDataCmd;
        private RelayCommand _downloadModCmd;
        private RelayCommand _closeNotifyBannerCmd;
        private RelayCommand _launchFactorioCmd;

        /*
         * Properties
         */

        /// <summary>
        /// Gets or sets the collection of mods
        /// </summary>
        public ObservableCollection<Mod> Mods
        {
            get { return _mods; }
            set { if (value == _mods) return; _mods = value; OnPropertyChanged(nameof(Mods)); }
        }

        /// <summary>
        /// Gets or sets the currently selected mod from the mods list
        /// </summary>
        public Mod SelectedMod
        {
            get { return _selectedMod; }
            set { if (value == _selectedMod) return; _selectedMod = value; OnPropertyChanged(nameof(SelectedMod)); }
        }

        /// <summary>
        /// Gets or sets a boolean value whether the application is checking for mod updates
        /// </summary>
        public bool IsCheckingForUpdates
        {
            get { return _isCheckingForUpdates; }
            set { if (value == _isCheckingForUpdates) return; _isCheckingForUpdates = value; OnPropertyChanged(nameof(IsCheckingForUpdates)); }
        }

        /// <summary>
        /// Gets or sets a boolean value whether any mod update is in progress
        /// </summary>
        public bool IsUpdating
        {
            get { return _isUpdating; }
            set { if (value == _isUpdating) return; _isUpdating = value; OnPropertyChanged(nameof(IsUpdating)); }
        }

        /// <summary>
        /// Gets or sets a boolean value whether any mod updates is available
        /// </summary>
        public bool IsUpdatesAvailable
        {
            get { return _isUpdatesAvailable; }
            set { if (value == _isUpdatesAvailable) return; _isUpdatesAvailable = value; OnPropertyChanged(nameof(IsUpdatesAvailable)); }
        }

        /// <summary>
        /// Gets or sets a boolean value whether the progress bar should appear
        /// </summary>
        public bool ShowProgressBar
        {
            get { return _showProgressBar; }
            set { if (value == _showProgressBar) return;_showProgressBar = value; OnPropertyChanged(nameof(ShowProgressBar)); }
        }

        /// <summary>
        /// Gets or sets a boolean value whether the notification banner should be shown
        /// </summary>
        public bool ShowNotifyBanner
        {
            get { return _showNotifyBanner; }
            set { if (value == _showNotifyBanner) return; _showNotifyBanner = value; OnPropertyChanged(nameof(ShowNotifyBanner)); }
        }

        /// <summary>
        /// Gets or sets a value the notification banner should display
        /// </summary>
        public string NotifyBannerValue
        {
            get { return _notifyBannerValue; }
            set { if (value == _notifyBannerValue) return; _notifyBannerValue = value; OnPropertyChanged(nameof(NotifyBannerValue)); }
        }

        /*
         * Commands
         */

        public RelayCommand GetLocalModsCmd => _getLocalModsCmd ??
            (_getLocalModsCmd = new RelayCommand(Execute_GetLocalModsCmd, p => Directory.Exists(BaseVm.ConfigVm.ModsPath)));

        public RelayCommand ToggleEnableModsCmd => _toggleEnableModsCmd ??
            (_toggleEnableModsCmd = new RelayCommand(Execute_ToggleEnableModsCmd, p => true));

        public RelayCommand OpenHyperlinkCmd => _openHyperlinkCmd ??
            (_openHyperlinkCmd = new RelayCommand(Execute_OpenHyperlinkCmd, p => true));

        public RelayCommand GetModRemoteDataCmd => _getModRemoteDataCmd ??
            (_getModRemoteDataCmd = new RelayCommand(Execute_GetModRemoteDataCmd, p => true));

        public RelayCommand DownloadModCmd => _downloadModCmd ??
            (_downloadModCmd = new RelayCommand(Execute_DownloadModCmd, p => IsUpdatesAvailable));

        public RelayCommand CloseNotifyBannerCmd => _closeNotifyBannerCmd ??
            (_closeNotifyBannerCmd = new RelayCommand(Execute_CloseNotifyBannerCmd, p => true));

        public RelayCommand LaunchFactorioCmd => _launchFactorioCmd ??
            (_launchFactorioCmd = new RelayCommand(Execute_LaunchFactorioCmd, p => File.Exists(BaseVm.ConfigVm.FactorioExePath)));

        /*
         * Methods
         */

        private void Execute_GetLocalModsCmd(object obj)
        {
            MethodExecutionHandler(nameof(Execute_GetLocalModsCmd), true);

            // Reset collection if reloading
            if (Mods.Count > 0)
                Mods.Clear();

            if (BaseVm.ConfigVm.ModsPath == null)
                return;

            // Get all filesystementries from mods directory
            var fileEntries = Directory.GetFileSystemEntries(BaseVm.ConfigVm.ModsPath, "*", SearchOption.TopDirectoryOnly).ToList();

            // Loop all entries
            foreach (var fileEntry in fileEntries)
            {
                // Skip unwanted files
                if (_unwantedFileExtensions.Any(unwantedFileExtension => fileEntry.EndsWith(unwantedFileExtension)))
                {
                    Logger.WriteLine($"Skipped entry: {fileEntry}", true);
                    continue;
                }

                // Get info.json as string
                string infoJsonString = JsonHelpers.GetModInfoJsonString(fileEntry);

                // If unable to read info from archive - flag an error in file entry
                if (string.IsNullOrEmpty(infoJsonString))
                {
                    Logger.WriteLine($"Error: Could not get infoJsonString from file: {fileEntry} - Flagged as error", true);

                    // Add to collection with has error flag
                    Mods.Add(new Mod { Title = fileEntry, FullName = fileEntry, HasError = true });

                    // Show notification
                    SetNotifyBanner($"There's an error with one or more of your mods!");

                    // Skip rest of the execution for this entry
                    continue;
                }

                // Deserialize the json string to infojson object
                var infoJson = JsonConvert.DeserializeObject<InfoJson>(infoJsonString);

                if (infoJson != null)
                {
                    // Add to mods collection
                    Mods.Add(new Mod
                    {
                        Name = infoJson.Name,
                        Title = infoJson.Title,
                        Description = infoJson.Description.Replace("\n", ""),
                        FactorioVersion = infoJson.FactorioVersion,
                        InstalledVersion = infoJson.Version,
                        Author = infoJson.Author,
                        Homepage = infoJson.Homepage,
                        Dependencies = infoJson.Dependencies,
                        FullName = fileEntry,
                        Filename = Path.GetFileName(fileEntry),
                        FilenameWithoutExtenion = Path.GetFileNameWithoutExtension(fileEntry)
                    });
                }
            }

            if (Mods.Count <= 0)
                Logger.WriteLine($"No mods found in path: {BaseVm.ConfigVm.ModsPath}", true);
            else
                SelectedMod = Mods[0];

            MethodExecutionHandler(nameof(Execute_GetLocalModsCmd), false);
        }

        private void Execute_ToggleEnableModsCmd(object obj)
        {
            // If all mods are enabled, set to false for all
            if (Mods.Where(x => x.IsEnabled).Count() == Mods.Count)
            {
                foreach (var mod in Mods)
                    mod.IsEnabled = false;
            }
            else
            {
                // Set enabled for all
                foreach (var mod in Mods)
                    mod.IsEnabled = true;
            }
        }

        private void Execute_OpenHyperlinkCmd(object obj)
        {
            if (SelectedMod.Homepage != null)
                Process.Start(SelectedMod.Homepage);
        }

        private void Execute_CloseNotifyBannerCmd(object obj)
        {
            ShowNotifyBanner = false;
        }

        private async void Execute_GetModRemoteDataCmd(object obj)
        {
            MethodExecutionHandler(nameof(Execute_GetModRemoteDataCmd), true);

            ShowProgressBar = true;
            IsCheckingForUpdates = true;

            var modPortalApi = new ModPortalApi();

            // Build the request string
            var sb = new StringBuilder();
            sb.Append("?page_size=max");
            foreach (var mod in Mods)
                sb.Append($"&namelist={mod.Name}");

            // Build the Api Data
            await modPortalApi.BuildApiData(sb.ToString());

            // Loop the api data results and get it's properties
            foreach (var result in modPortalApi.ApiData.Results)
            {
                // Select the first entry in the loop
                var mod = Mods.First(x => result.Name == x.Name);

                mod.RemoteVersion = result.Releases.First().Version;
                mod.DownloadUrl = result.Releases.First().DownloadUrl;
                mod.RemoteFilename = result.Releases.First().FileName;

                // Check if remote version is greater than installed version
                if (Version.Parse(mod.RemoteVersion) > Version.Parse(mod.InstalledVersion))
                    mod.UpdateAvailable = true;
            }

            // Set IsUpdatesAvailable flag
            if (Mods.Any(x => x.UpdateAvailable))
            {
                IsUpdatesAvailable = true;
                SetNotifyBanner($"{Mods.Count(x => x.UpdateAvailable)} updates available");
                Logger.WriteLine($"{Mods.Count(x => x.UpdateAvailable)} updates available", true);
            }
            else
            {
                SetNotifyBanner("No updates available");
                Logger.WriteLine("No updates available", true);
            }

            IsCheckingForUpdates = false;
            ShowProgressBar = false;

            MethodExecutionHandler(nameof(Execute_GetModRemoteDataCmd), false);
        }

        private async void Execute_DownloadModCmd(object obj)
        {
            MethodExecutionHandler(nameof(Execute_DownloadModCmd), true);

            ShowProgressBar = true;

            var authenticationApi = new AuthenticationApi();

            // Get vars from configVm
            var settings = BaseVm.ConfigVm;
            var username = settings.ModPortalUsername;
            var password = settings.ModPortalPassword;

            if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password))
            {
                Logger.WriteLine($"Aquiring authentication with mod portal...", true);

                // Get authentication token
                var token = await authenticationApi.GetAuthenticationToken(username, password, true);

                if (authenticationApi.Success)
                {
                    Logger.WriteLine($"Authentication successful!", true);

                    // Wait 500 ms
                    await Task.Delay(500);

                    // Loop all mods with UpdateAvailable flag
                    foreach (var mod in Mods.Where(x => x.UpdateAvailable))
                    {
                        mod.IsUpdating = true;

                        // Execute the download
                        await ExecuteDownload(mod, username, token);

                        // If successfull
                        if (_fileDownloadSucceeded)
                        {
                            Exception exception = null;

                            // Delete old mod file
                            try
                            {
                                File.Delete(mod.FullName);
                            }
                            catch (Exception ex)
                            {
                                exception = ex;
                                Logger.WriteLine($"Failed to delete file: {mod.FullName}", true, ex);
                            }
                            finally
                            {
                                if (exception == null)
                                    Logger.WriteLine($"Deleted old file: {mod.FullName}", true);
                            }

                            // Wait 2 secs
                            await Task.Delay(2000);

                            // Set new/reset properties
                            mod.UpdateAvailable = false;
                            mod.InstalledVersion = mod.RemoteVersion;
                            mod.FullName = Path.Combine(settings.ModsPath, mod.RemoteFilename);
                            mod.Filename = mod.RemoteFilename;
                            mod.FilenameWithoutExtenion = Path.GetFileNameWithoutExtension(mod.RemoteFilename);
                            mod.ProgressPercentage = 0;
                        }

                        mod.IsUpdating = false;
                    }
                }
                else
                {
                    Logger.WriteLine($"Authentication failed - Error message: {authenticationApi.ErrorMessage}");
                    SetNotifyBanner("Authentication failed!");
                }
            }
            else
            {
                Logger.WriteLine($"Authentication failed - Username and/or password is null", true);
                SetNotifyBanner("Authentication failed!");
            }

            ShowProgressBar = false;

            MethodExecutionHandler(nameof(Execute_DownloadModCmd), false);
        }

        private async Task ExecuteDownload(Mod mod, string userName, string token)
        {
            MethodExecutionHandler(nameof(ExecuteDownload), true);

            _fileDownloadSucceeded = false;
            Exception exception = null;

            // Create the webclient
            using (var webClient = new WebClient { Proxy = null })
            {
                Logger.WriteLine($"Started download of: {mod.DownloadUrl}", true);

                // Report progress to mod object
                webClient.DownloadProgressChanged += (sender, args) => mod.ProgressPercentage = args.ProgressPercentage;

                // Execute download
                try
                {
                    await webClient.DownloadFileTaskAsync($"https://mods.factorio.com{mod.DownloadUrl}?username={userName}&token={token}", Path.Combine(BaseVm.ConfigVm.ModsPath, mod.RemoteFilename));
                }
                catch (Exception ex)
                {
                    exception = ex;
                    Logger.WriteLine($"Error in method {nameof(ExecuteDownload)}", true, ex);
                    Logger.WriteLine($"File download failed of: {mod.DownloadUrl}", true, ex);
                }
                finally
                {
                    if (exception == null)
                    {
                        _fileDownloadSucceeded = true;
                        Logger.WriteLine($"Successfully downloaded file: {mod.DownloadUrl}", true);
                    }
                }
            }

            MethodExecutionHandler(nameof(ExecuteDownload), false);
        }

        private async void Execute_LaunchFactorioCmd(object obj)
        {
            MethodExecutionHandler(nameof(Execute_LaunchFactorioCmd), true);

            if (Directory.Exists(BaseVm.ConfigVm.ModsPath))
            {
                // Select all mods from collection
                var modListItems = Mods.Select(mod => new ModListItem
                {
                    Name = mod.Name,
                    Enabled = mod.IsEnabled
                }).ToList();

                // Serialize selection to json
                string json = JsonConvert.SerializeObject((dynamic)(new
                {
                    mods = modListItems
                }), Formatting.Indented).ToString();

                Exception exception = null;

                // Write json to mod-list.json
                try
                {
                    File.WriteAllText(Path.Combine(BaseVm.ConfigVm.ModsPath, "mod-list.json"), json);
                }
                catch (Exception ex)
                {
                    exception = ex;
                    Logger.WriteLine($"Failed to write to file: {Path.Combine(BaseVm.ConfigVm.ModsPath, "mod-list.json")}", true, ex);
                }
                finally
                {
                    if (exception == null)
                        Logger.WriteLine($"Successfully written json to file: {Path.Combine(BaseVm.ConfigVm.ModsPath, "mod-list.json")}", true);
                }
            }
            else
            {
                Logger.WriteLine($"Directory does not exist: {BaseVm.ConfigVm.ModsPath} - Launching without mods", true);
            }

            // Wait 1 sec
            await Task.Delay(1000);

            // Create and start process
            using (var process = new Process())
            {
                {
                    process.StartInfo.FileName = BaseVm.ConfigVm.FactorioExePath;
                    process.Start();
                }
            }

            MethodExecutionHandler(nameof(Execute_LaunchFactorioCmd), false);
        }

        private void SetNotifyBanner(string value)
        {
            ShowNotifyBanner = true;
            NotifyBannerValue = value;
        }

        private void MethodExecutionHandler(string methodName, bool create)
        {
            if (create)
            {
                Logger.WriteLine($"Executing method '{methodName}'", true);
                _stopwatch = Stopwatch.StartNew();
            }
            else
            {
                _stopwatch?.Stop();
                Logger.WriteLine($"Execution of method '{methodName}' completed in {_stopwatch?.ElapsedMilliseconds} ms. ({_stopwatch?.Elapsed})", true);
            }
        }
    }
}
