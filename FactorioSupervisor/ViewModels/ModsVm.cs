using FactorioSupervisor.Extensions;
using FactorioSupervisor.Helpers;
using FactorioSupervisor.Models;
using FactorioSupervisor.ObservableImmutable;
using ModsApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace FactorioSupervisor.ViewModels
{
    public class ModsVm : ObservableObject
    {
        public ModsVm()
        {
            Logger.WriteLine($"Class created: {nameof(ModsVm)}");

            Mods = new ObservableImmutableList<Mod>();

            if (GetLocalModsCmd.CanExecute(null))
                GetLocalModsCmd.Execute(null);

            /* Create example mod */
            if (Mods.Count == 0)
            {
                var mod = new Mod()
                {
                    Name = "aai-programmable-vehicles",
                    Title = "AAI Programmable Vehicles",
                    Description = "Program and control autonomous vehicles using a remote control handset, or circuit conditions and zones. Can be used for base enemy base assault, patrols, friendly base navigation, vehicle-based mining, and more advanced applications. Works with vanilla and modded vehicles.",
                    FactorioVersion = "0.15",
                    InstalledVersion = "0.3.2",
                    Author = "Earendel",
                    Homepage = "https://forums.factorio.com/viewtopic.php?f=93&t=38475",
                    Dependencies = new JArray
                    {
                        "?aai-vehicles-flame-tumbler >= 0.2.1",
                        "?aai-vehicles-laser-tank >= 0.2.1",
                        "?bullet-trails >= 0.2.1",
                        "aai-programmable-structures >= 0.3.1",
                        "aai-vehicles-hauler >= 0.2.1",
                        "?aai-vehicles-chaingunner >= 0.2.1"
                    },
                    FullName = @"C:\mods\example",
                    FilenameWithoutExtenion = "example_mod",
                    ReleasedAt = TimeHelpers.GetTimeSpanDuration("2017-04-28T07:47:58.903536Z"),
                    RemoteVersion = "0.3.3",
                    RemoteFactorioVersion = "0.15",
                    UpdateAvailable = true,
                    //IsUpdating = true,
                    //ProgressPercentage = 33
                };

                // Set dependencies
                if (mod.Dependencies != null)
                {
                    foreach (var dep in mod.Dependencies)
                    {
                        var depStr = dep.ToString();

                        var dependency = new Dependency();

                        if (depStr.StartsWith("?"))
                        {
                            dependency.IsOptional = true;
                            depStr = depStr.Replace("?", "").Trim();
                        }

                        dependency.Name = depStr;

                        mod.DependenciesCollection.Add(dependency);
                    }

                    if (mod.DependenciesCollection.Any(x => x.IsOptional))
                        mod.HasOptionalDependencies = true;
                }

                Mods.Add(mod);

                SelectedMod = Mods[0];

                //IsUpdating = true;
            }
            /* Create example mod END */

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

        private ObservableImmutableList<Mod> _mods;
        private Mod _selectedMod;
        private bool _isCheckingForUpdates;
        private bool _isUpdating;
        private double _updateTotalProgress;
        private bool _isUpdatesAvailable;
        private int _totalUpdatesAvailable;
        private Mod _currentUpdatingMod;
        private Dependency _currentUpdatingDependency;
        private bool _showProgressBar;
        private bool _fileDownloadSucceeded;
        private bool _hideIncompatibleMods;
        private bool _isAuthenticated;
        private FileSystemWatcher _fileSystemWatcher;
        private RelayCommand _getLocalModsCmd;
        private RelayCommand _toggleEnableModsCmd;
        private RelayCommand _openHyperlinkCmd;
        private RelayCommand _getModRemoteDataCmd;
        private RelayCommand _downloadModCmd;
        private RelayCommand _launchFactorioCmd;
        private RelayCommand _watchModDirChangesCmd;
        private RelayCommand _deleteModCmd;
        private RelayCommand _installDependencyModCmd;
        private RelayCommand _verifyAuthenticationCmd;
        private RelayCommand _installEntryCmd;

        /*
         * Properties
         */

        #region Properties

        /// <summary>
        /// Gets or sets the collection of mods
        /// </summary>
        public ObservableImmutableList<Mod> Mods
        {
            get => _mods; set { if (value == _mods) return; _mods = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Gets or sets the currently selected mod from the mods list
        /// </summary>
        public Mod SelectedMod
        {
            get => _selectedMod; set { if (value == _selectedMod) return; _selectedMod = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Gets or sets a boolean value whether the application is checking for mod updates
        /// </summary>
        public bool IsCheckingForUpdates
        {
            get => _isCheckingForUpdates; set
            {
                if (value == _isCheckingForUpdates) return; _isCheckingForUpdates = value; OnPropertyChanged();
                ShowProgressBar = IsCheckingForUpdates;
            }
        }

        /// <summary>
        /// Gets or sets a boolean value whether any mod update is in progress
        /// </summary>
        public bool IsUpdating
        {
            get => _isUpdating; set
            {
                if (value == _isUpdating) return; _isUpdating = value; OnPropertyChanged();
                ShowProgressBar = IsUpdating;
            }
        }

        /// <summary>
        /// Gets or sets the update total progress value
        /// </summary>
        public double UpdateTotalProgress
        {
            get => _updateTotalProgress; set { if (Math.Abs(value - _updateTotalProgress) < 0.01) return; _updateTotalProgress = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Gets or sets a boolean value whether any mod updates is available
        /// </summary>
        public bool IsUpdatesAvailable
        {
            get => _isUpdatesAvailable; set { if (value == _isUpdatesAvailable) return; _isUpdatesAvailable = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Gets or sets the number of total mod updates available
        /// </summary>
        public int TotalUpdatesAvailable
        {
            get => _totalUpdatesAvailable; set { if (value == _totalUpdatesAvailable) return; _totalUpdatesAvailable = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Gets or sets the currently updating mod object
        /// </summary>
        public Mod CurrentUpdatingMod
        {
            get => _currentUpdatingMod; set { if (value == _currentUpdatingMod) return; _currentUpdatingMod = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Gets or sets the currently updating dependency object
        /// </summary>
        public Dependency CurrentUpdatingDependency
        {
            get => _currentUpdatingDependency; set { if (value == _currentUpdatingDependency) return; _currentUpdatingDependency = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Gets or sets a boolean value whether the progress bar should appear
        /// </summary>
        public bool ShowProgressBar
        {
            get => _showProgressBar; set { if (value == _showProgressBar) return; _showProgressBar = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Gets or sets a boolean value whether to hide incompatible mods
        /// </summary>
        public bool HideIncompatibleMods
        {
            get => _hideIncompatibleMods; set
            {
                if (value == _hideIncompatibleMods) return; _hideIncompatibleMods = value; OnPropertyChanged();

                // determine visibility
                if (HideIncompatibleMods && BaseVm.ConfigVm.CurrentFactorioBranch != null)
                {
                    foreach (var mod in Mods)
                    {
                        var modFactorioVersion = Version.Parse(mod.FactorioVersion);
                        var currentFactorioVersion = Version.Parse(BaseVm.ConfigVm.CurrentFactorioBranch);

                        if (modFactorioVersion != currentFactorioVersion)
                        {
                            mod.HideInModList = true;

                            if (mod.IsEnabled)
                                mod.IsEnabled = false;
                        }
                        else
                            mod.HideInModList = false;
                    }
                }
                else if (!HideIncompatibleMods && BaseVm.ConfigVm.CurrentFactorioBranch != null)
                {
                    foreach (var mod in Mods)
                        mod.HideInModList = false;
                }
            }
        }

        /// <summary>
        /// Gets or sets a boolean value whether the authentication process succeeded
        /// </summary>
        public bool IsAuthenticated
        {
            get => _isAuthenticated; set { if (value == _isAuthenticated) return; _isAuthenticated = value; OnPropertyChanged(); }
        }

        #endregion

        /*
         * Commands
         */

        #region

        public RelayCommand GetLocalModsCmd => _getLocalModsCmd ??
            (_getLocalModsCmd = new RelayCommand(Execute_GetLocalModsCmd, p => Directory.Exists(BaseVm.ConfigVm.ModsPath)));

        public RelayCommand ToggleEnableModsCmd => _toggleEnableModsCmd ??
            (_toggleEnableModsCmd = new RelayCommand(Execute_ToggleEnableModsCmd, p => true));

        public RelayCommand OpenHyperlinkCmd => _openHyperlinkCmd ??
            (_openHyperlinkCmd = new RelayCommand(p => Execute_OpenHyperlinkCmd(p as string), p => true));

        public RelayCommand GetModRemoteDataCmd => _getModRemoteDataCmd ??
            (_getModRemoteDataCmd = new RelayCommand(Execute_GetModRemoteDataCmd, p => true));

        public RelayCommand DownloadModCmd => _downloadModCmd ??
            (_downloadModCmd = new RelayCommand(Execute_DownloadModCmd, p => true));

        public RelayCommand LaunchFactorioCmd => _launchFactorioCmd ??
            (_launchFactorioCmd = new RelayCommand(Execute_LaunchFactorioCmd, p => true));

        public RelayCommand WatchModDirChangesCmd => _watchModDirChangesCmd ??
            (_watchModDirChangesCmd = new RelayCommand(Execute_WatchModDirChangesCmd, p => Directory.Exists(BaseVm.ConfigVm.ModsPath)));

        public RelayCommand DeleteModCmd => _deleteModCmd ??
            (_deleteModCmd = new RelayCommand(Execute_DeleteModCmd, p => true));

        public RelayCommand InstallDependencyModCmd => _installDependencyModCmd ??
            (_installDependencyModCmd = new RelayCommand(Execute_InstallDependencyModCmd, p => true));

        public RelayCommand VerifyAuthenticationCmd => _verifyAuthenticationCmd ??
            (_verifyAuthenticationCmd = new RelayCommand(Execute_VerifyAuthenticationCmd, p => true));

        public RelayCommand InstallEntryCmd => _installEntryCmd ??
            (_installEntryCmd = new RelayCommand(p => Execute_InstallEntryCmd(p as string), p => true));

        #endregion

        /*
         * Methods
         */

        private async void Execute_InstallDependencyModCmd(object obj)
        {
            if (!await NetcodeHelpers.VerifyInternetConnection())
            {
                // Open message box to user
                MessageBoxWindow.Show(ResourceHelper.GetValue("MessageBox_NoInternetConnection_Title"), ResourceHelper.GetValue("MessageBox_NoInternetConnection"), MessageBoxButton.OK);
                return;
            }

            if (IsUpdating)
                return;

            // Verify authentication
            if (!IsAuthenticated)
                return;

            IsUpdating = true;
            CurrentUpdatingDependency = SelectedMod.SelectedDependency;
            CurrentUpdatingDependency.IsUpdating = true;

            // Create api class
            var modPortalApi = new ModPortalApi();

            // Build the request string
            var sb = new StringBuilder();
            sb.Append("?page_size=max");
            sb.Append($"&namelist={CurrentUpdatingDependency.Name}");

            Logger.WriteLine($"Created Mod Portal Api request: {sb}", true);

            // Build the Api Data
            await modPortalApi.BuildApiData(sb.ToString());

            if (modPortalApi.ApiData != null)
            {
                Logger.WriteLine("modPortalApi.ApiData != null");

                if (modPortalApi.ApiData.Results != null)
                {
                    Logger.WriteLine("modPortalApi.ApiData.Results != null");

                    // Get remote properties
                    CurrentUpdatingDependency.DownloadUrl = modPortalApi.ApiData.Results.First().Releases.First().DownloadUrl;
                    CurrentUpdatingDependency.RemoteFilename = modPortalApi.ApiData.Results.First().Releases.First().FileName;

                    // Get vars from configVm
                    var settings = BaseVm.ConfigVm;
                    var username = settings.ModPortalUsername;
                    var token = settings.ModPortalAuthToken;

                    // Execute the download
                    await ExecuteDownload(null, username, token, true, CurrentUpdatingDependency);

                    // If successfull
                    if (_fileDownloadSucceeded)
                    {
                        Logger.WriteLine($"Downloaded file: '{CurrentUpdatingDependency.RemoteFilename}'");

                        // Add to collection
                        ItemEntryAdd(Path.Combine(BaseVm.ConfigVm.ModsPath, CurrentUpdatingDependency.RemoteFilename));

                        // Set new dependency as installed in collections
                        foreach (var mod in Mods)
                        {
                            foreach (var dep in mod.DependenciesCollection)
                            {
                                if (dep.Name == CurrentUpdatingDependency.Name)
                                    dep.IsInstalled = true;
                            }
                        }
                    }
                }
                else
                {
                    Logger.WriteLine("ERROR: modPortalApi.ApiData.Results is null", true);

                    // Open message box to user
                    MessageBoxWindow.Show(ResourceHelper.GetValue("MessageBox_ModPortalError_Title"), ResourceHelper.GetValue("MessageBox_ModPortalError"), MessageBoxButton.OK);
                }
            }
            else
            {
                Logger.WriteLine("ERROR: modPortalApi.ApiData is null", true);

                // Open message box to user
                MessageBoxWindow.Show(ResourceHelper.GetValue("MessageBox_ModPortalError_Title"), ResourceHelper.GetValue("MessageBox_ModPortalError"), MessageBoxButton.OK);
            }

            CurrentUpdatingDependency = null;
            IsUpdating = false;
        }

        private void Execute_GetLocalModsCmd(object obj)
        {
            // Reset collection if reloading
            if (Mods.Count > 0)
                Mods.Clear();

            // Get all filesystementries from mods directory
            var fileEntries = Directory.GetFileSystemEntries(BaseVm.ConfigVm.ModsPath, "*", SearchOption.TopDirectoryOnly).ToList();

            // Loop all entries and add to collection
            foreach (var fileEntry in fileEntries)
                ItemEntryAdd(fileEntry);

            // Check for installed dependencies
            foreach (var mod in Mods)
            {
                if (mod.DependenciesCollection.Count == 0)
                    continue;

                foreach (var dep in mod.DependenciesCollection)
                {
                    foreach (var item in Mods)
                    {
                        if (dep.Name.Contains(item.Name))
                            dep.IsInstalled = true;
                    }
                }
            }

            // check for errors
            if (Mods.Any(x => x.HasError))
            {
                var errorCount = Mods.Count(x => x.HasError);

                // Show notification
                BaseVm.NotifyBannerRelay.SetNotifyBanner($"There's an error with {errorCount} of your mods!");
            }

            // Set selected mod
            if (Mods.Count == 0)
                Logger.WriteLine($"No mods found in path: {BaseVm.ConfigVm.ModsPath}", true);
            else
                SelectedMod = Mods[0];

            // Start filesystemwatcher
            if (WatchModDirChangesCmd.CanExecute(Directory.Exists(BaseVm.ConfigVm.ModsPath)))
                WatchModDirChangesCmd.Execute(Directory.Exists(BaseVm.ConfigVm.ModsPath));

            //todo load profiles when reloading
        }

        private void Execute_ToggleEnableModsCmd(object obj)
        {
            // If all mods are enabled, set to false for all
            if (Mods.Count(x => x.IsEnabled) == Mods.Count)
            {
                foreach (var mod in Mods.Where(x => !x.HasError))
                    mod.IsEnabled = false;
            }
            else
            {
                // Set enabled for all
                foreach (var mod in Mods.Where(x => !x.HasError))
                    mod.IsEnabled = true;
            }
        }

        private void Execute_OpenHyperlinkCmd(string param)
        {
            if (param == "homepage" && SelectedMod.Homepage != null)
                Process.Start(SelectedMod.Homepage);

            if (param == "portalpage" && SelectedMod.Name != null)
                Process.Start($"https://mods.factorio.com/?q={SelectedMod.Name}");
        }

        private async void Execute_GetModRemoteDataCmd(object obj)
        {
            if (!await NetcodeHelpers.VerifyInternetConnection())
            {
                // Open message box to user
                MessageBoxWindow.Show(ResourceHelper.GetValue("MessageBox_NoInternetConnection_Title"), ResourceHelper.GetValue("MessageBox_NoInternetConnection"), MessageBoxButton.OK);
                return;
            }

            IsCheckingForUpdates = true;

            // Create api class
            var modPortalApi = new ModPortalApi();

                // Build the request string
                var sb = new StringBuilder();
                sb.Append("?page_size=max");
                foreach (var mod in Mods.Where(x => !x.HasError))
                    sb.Append($"&namelist={mod.Name}");

                Logger.WriteLine($"[INFO] Created Mod Portal Api request: {sb}", true);

                // Build the Api Data
                await modPortalApi.BuildApiData(sb.ToString());

            if (modPortalApi.ApiData.Results != null)
            {
                // Loop the api data results and get it's properties
                foreach (var result in modPortalApi.ApiData.Results)
                {
                    // Select the first entry in the loop
                    var mod = Mods.First(x => result.Name == x.Name);

                    mod.RemoteVersion = result.Releases.First().Version;
                    mod.DownloadUrl = result.Releases.First().DownloadUrl;
                    mod.RemoteFilename = result.Releases.First().FileName;
                    mod.ReleasedAt = TimeHelpers.GetTimeSpanDuration(result.Releases.First().ReleasedAt);
                    mod.RemoteFactorioVersion = result.Releases.First().FactorioVersion;

                    // Check if remote version is greater than installed version
                    if (Version.Parse(mod.RemoteVersion) > Version.Parse(mod.InstalledVersion))
                        mod.UpdateAvailable = true;
                }

                // Set IsUpdatesAvailable flag
                if (Mods.Any(x => x.UpdateAvailable))
                {
                    IsUpdatesAvailable = true;
                    var updateCount = Mods.Count(x => x.UpdateAvailable);
                    TotalUpdatesAvailable = updateCount;

                    /*BaseVm.NotifyBannerRelay.SetNotifyBanner(updateCount > 1
                        ? $"{updateCount} updates are available!"
                        : $"{updateCount} update are available!");*/

                    Logger.WriteLine($"[INFO] {Mods.Count(x => x.UpdateAvailable)} updates are available", true);
                }
                else
                {
                    //BaseVm.NotifyBannerRelay.SetNotifyBanner("There are currently no updates available");
                    Logger.WriteLine("[INFO] No updates available", true);
                }
            }
            else
            {
                Logger.WriteLine($"[ERROR] Method {nameof(Execute_GetModRemoteDataCmd)} failed - modPortalApi.ApiData.Results is null", true);

                // Open message box to user
                MessageBoxWindow.Show(ResourceHelper.GetValue("MessageBox_ModPortalError_Title"), ResourceHelper.GetValue("MessageBox_ModPortalError"), MessageBoxButton.OK);
            }

            IsCheckingForUpdates = false;
        }

        private async void Execute_DownloadModCmd(object obj)
        {
            if (IsUpdating)
                return;

            // Verify authentication
            if (!IsAuthenticated)
                return;

            IsUpdating = true;
            CurrentUpdatingMod = null;
            var totalUpdateCount = Mods.Count(x => x.UpdateAvailable);
            double updateCount = 0;
            UpdateTotalProgress = 0;

            // Get vars from configVm
            var settings = BaseVm.ConfigVm;
            var username = settings.ModPortalUsername;
            var token = settings.ModPortalAuthToken;

            // Loop all mods with UpdateAvailable flag
            foreach (var mod in Mods.Where(x => x.UpdateAvailable))
            {
                CurrentUpdatingMod = mod;
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

                    // Wait 500ms
                    await Task.Delay(500);

                    // Set new/reset properties
                    mod.UpdateAvailable = false;
                    mod.InstalledVersion = mod.RemoteVersion;
                    mod.FactorioVersion = mod.RemoteFactorioVersion;
                    mod.FullName = Path.Combine(settings.ModsPath, mod.RemoteFilename);
                    mod.Filename = mod.RemoteFilename;
                    mod.FilenameWithoutExtenion = Path.GetFileNameWithoutExtension(mod.RemoteFilename);
                    mod.ProgressPercentage = 0;
                }

                mod.IsUpdating = false;

                // Calculate UpdateTotalProgress
                updateCount++;
                UpdateTotalProgress = (updateCount / totalUpdateCount) * 100;
            }

            TotalUpdatesAvailable = Mods.Count(x => x.UpdateAvailable);

            // Reset IsUpdatesAvailable flag
            if (Mods.Count(x => x.UpdateAvailable) == 0)
                IsUpdatesAvailable = false;

            CurrentUpdatingMod = null;
            IsUpdating = false;
        }

        private async Task ExecuteDownload(Mod mod, string userName, string token, bool updateDependency = false, Dependency dependency = null)
        {
            _fileDownloadSucceeded = false;
            Exception exception = null;

            if (dependency == null) return;

            // Create the webclient
            using (var webClient = new WebClient { Proxy = null })
            {
                Logger.WriteLine(
                    updateDependency
                        ? $"Started download of: {dependency.DownloadUrl}"
                        : $"Started download of: {mod.DownloadUrl}", true);

                // Report progress to mod object
                webClient.DownloadProgressChanged += (sender, args) =>
                {
                    if (updateDependency)
                    {
                        CurrentUpdatingDependency.ProgressPercentage = args.ProgressPercentage;
                        dependency.ProgressPercentage = args.ProgressPercentage;
                    }
                    else
                    {
                        CurrentUpdatingMod.ProgressPercentage = args.ProgressPercentage;
                        mod.ProgressPercentage = args.ProgressPercentage;
                    }
                };

                // Execute download
                try
                {
                    if (updateDependency)
                        await webClient.DownloadFileTaskAsync($"https://mods.factorio.com{dependency.DownloadUrl}?username={userName}&token={token}", Path.Combine(BaseVm.ConfigVm.ModsPath, dependency.RemoteFilename));
                    else
                        await webClient.DownloadFileTaskAsync($"https://mods.factorio.com{mod.DownloadUrl}?username={userName}&token={token}", Path.Combine(BaseVm.ConfigVm.ModsPath, mod.RemoteFilename));
                }
                catch (Exception ex)
                {
                    exception = ex;
                    Logger.WriteLine($"Error in method {nameof(ExecuteDownload)}", true, ex);

                    Logger.WriteLine(
                        updateDependency
                            ? $"File download failed of: {dependency.DownloadUrl}"
                            : $"File download failed of: {mod.DownloadUrl}", true, ex);
                }
                finally
                {
                    if (exception == null)
                    {
                        _fileDownloadSucceeded = true;

                        Logger.WriteLine(
                            updateDependency
                                ? $"Successfully downloaded file: {dependency.DownloadUrl}"
                                : $"Successfully downloaded file: {mod.DownloadUrl}", true);
                    }
                    else
                    {
                        // Delete partial file if error downloading
                        if (updateDependency)
                        {
                            if (File.Exists(Path.Combine(BaseVm.ConfigVm.ModsPath, dependency.RemoteFilename)))
                                File.Delete(Path.Combine(BaseVm.ConfigVm.ModsPath, dependency.RemoteFilename));
                        }
                        else
                        {
                            if (File.Exists(Path.Combine(BaseVm.ConfigVm.ModsPath, mod.RemoteFilename)))
                                File.Delete(Path.Combine(BaseVm.ConfigVm.ModsPath, mod.RemoteFilename));
                        }    
                    }
                }
            }
        }

        private async void Execute_LaunchFactorioCmd(object obj)
        {
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

            // Wait 500ms
            await Task.Delay(500);

            var factorioExe = Path.Combine(BaseVm.ConfigVm.FactorioPath, @"bin\x64\factorio.exe");

            if (!File.Exists(factorioExe))
            {
                BaseVm.NotifyBannerRelay.SetNotifyBanner("Cannot launch Factorio. File not found!");
                Logger.WriteLine($"Executable file does not exist: {factorioExe}", true);
                return;
            }

            // Create and start process
            using (var process = new Process())
            {
                {
                    process.StartInfo.FileName = factorioExe;
                    process.Start();
                }
            }
        }

        private void Execute_DeleteModCmd(object obj)
        {
            // Open message box to user
            var messageBoxResult = MessageBoxWindow.Show(ResourceHelper.GetValue("MessageBox_DeleteMod_Title"), ResourceHelper.GetValue("MessageBox_DeleteMod"), MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Cancel)
                return;

            Exception exception = null;

            try
            {
                File.Delete(SelectedMod.FullName);
            }
            catch (Exception ex)
            {
                exception = ex;
                Logger.WriteLine($"Failed to delete file: {SelectedMod.FullName}", true, ex);
            }
            finally
            {
                if (exception == null)
                {
                    Logger.WriteLine($"Deleted file: {SelectedMod.FullName}", true);
                    Mods.RemoveAt(Mods.IndexOf(SelectedMod));
                }
            }
        }

        private void Execute_WatchModDirChangesCmd(object obj)
        {
            var watchDir = BaseVm.ConfigVm.ModsPath;

            // create and start watcher
            _fileSystemWatcher = new FileSystemWatcher(watchDir, "*.zip") { EnableRaisingEvents = true };

            // create event handlers
            _fileSystemWatcher.Renamed += _fileSystemWatcher_Renamed;
            _fileSystemWatcher.Deleted += _fileSystemWatcher_Deleted;
            _fileSystemWatcher.Created += _fileSystemWatcher_Created;
        }

        private void _fileSystemWatcher_Created(object sender, FileSystemEventArgs e)
        {
            // on created
            Logger.WriteLine($"[INFO]: Mod directory change detected - event: {e.ChangeType} file: '{e.FullPath}'", true);

            if (IsUpdating)
                return;

            ItemEntryAdd(e.FullPath);
        }

        private static void _fileSystemWatcher_Deleted(object sender, FileSystemEventArgs e)
        {
            // on deleted
            Logger.WriteLine($"[INFO]: Mod directory change detected - event: {e.ChangeType} file: '{e.FullPath}'", true);
        }

        private static void _fileSystemWatcher_Renamed(object sender, RenamedEventArgs e)
        {
            // on renamed
            Logger.WriteLine($"[INFO]: Mod directory change detected - event: {e.ChangeType} file: '{e.FullPath}'", true);
        }

        private void ModEntryDelete(Mod mod)
        {
            Exception exception = null;

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
                {
                    Logger.WriteLine($"Deleted file: {mod.FullName}", true);
                    Mods.RemoveAt(Mods.IndexOf(mod));
                }
            }
        }

        private static async Task<bool> VerifyAuthentication()
        {
            var result = false;

            if (await NetcodeHelpers.VerifyInternetConnection())
            {
                // Is auth token from configVm is null
                if (string.IsNullOrWhiteSpace(BaseVm.ConfigVm.ModPortalAuthToken))
                {
                    // Open message box to user
                    MessageBoxWindow.Show(ResourceHelper.GetValue("MessageBox_AuthRequired_Title"), ResourceHelper.GetValue("MessageBox_AuthRequired"), MessageBoxButton.OKCancel);
                }

                Exception exception = null;

                // Check if we can download anything
                using (var webClient = new WebClient { Proxy = null })
                {
                    webClient.OpenReadCompleted += (sender, args) =>
                    {
                    // if download successfull return true
                    if (args.Error == null)
                        {
                            Logger.WriteLine($"[AUTH]: Successfully authenticated with mod portal with username: '{BaseVm.ConfigVm.ModPortalUsername}' and password: [SECRET]", true);
                            result = true;
                        }
                    };

                    try
                    {
                        await webClient.OpenReadTaskAsync($"https://mods.factorio.com/api/downloads/data/mods/34/rso-mod_3.2.1.zip?username={BaseVm.ConfigVm.ModPortalUsername}&token={BaseVm.ConfigVm.ModPortalAuthToken}");
                    }
                    catch (Exception ex)
                    {
                        exception = ex;
                        Logger.WriteLine($"[AUTH]: Failed authentication with mod portal with username: '{BaseVm.ConfigVm.ModPortalUsername}' and password: [SECRET]", true, ex);
                    }
                }

                // If exception get new token
                if (exception != null)
                {
                    // Attempt authentication
                    var authenticationApi = new AuthenticationApi();

                    // Get/set vars
                    var username = BaseVm.ConfigVm.ModPortalUsername;
                    var password = BaseVm.ConfigVm.ModPortalPassword;
                    string authToken = null;

                    // Get token
                    try
                    {
                        authToken = await authenticationApi.GetAuthenticationToken(username, password, true);
                    }
                    catch (Exception)
                    {
                        // ignored
                    }

                    // Is success or fail
                    if (authenticationApi.Success)
                    {
                        Logger.WriteLine($"[AUTH]: Successfully authenticated with mod portal with username: '{BaseVm.ConfigVm.ModPortalUsername}' and password: [SECRET]", true);

                        // Set auth token to configVm
                        BaseVm.ConfigVm.ModPortalAuthToken = authToken;

                        result = true;
                    }
                    else
                    {
                        Logger.WriteLine($"[AUTH]: Failed authentication with mod portal with username: '{BaseVm.ConfigVm.ModPortalUsername}' and password: [SECRET]", true);

                        // Open message box to user
                        var messageBoxResult = MessageBoxWindow.Show(ResourceHelper.GetValue("MessageBox_AuthRequired_Title"), ResourceHelper.GetValue("MessageBox_AuthRequired"), MessageBoxButton.OKCancel);
                        if (messageBoxResult == MessageBoxResult.Cancel)
                            result = false;
                    }
                }
            }
            else
            {
                Logger.WriteLine($"[AUTH]: Failed authentication with mod portal with username: '{BaseVm.ConfigVm.ModPortalUsername}' and password: [SECRET] - No internet connection", true);
            }

            return result;
        }

        private void AddModToCollection(InfoJson infoJson, string filename)
        {
            // Create mod object
            var mod = new Mod
            {
                Name = infoJson.Name,
                Title = infoJson.Title,
                Description = infoJson.Description.Replace("\n", string.Empty), // remove newlines
                FactorioVersion = infoJson.FactorioVersion,
                InstalledVersion = infoJson.Version,
                Author = infoJson.Author,
                Homepage = infoJson.Homepage != null && infoJson.Homepage.StartsWith("http") ? infoJson.Homepage : null, // only valid urls
                Dependencies = infoJson.Dependencies,
                FullName = filename,
                Filename = Path.GetFileName(filename),
                FilenameWithoutExtenion = Path.GetFileNameWithoutExtension(filename)
            };

            // Set dependencies
            if (mod.Dependencies != null)
            {
                foreach (var dep in mod.Dependencies)
                {
                    // Convert to string
                    var depString = dep.ToString();

                    // Remove unwanted content
                    if (depString.Contains(">="))
                        depString = Regex.Replace(depString, ">=(?:.*)", "").Trim();

                    if (depString.Contains("<"))
                        depString = Regex.Replace(depString, "<(?:.*)", "").Trim();

                    // Create dependency object
                    var dependency = new Dependency();

                    // set optional dependencies
                    if (depString.StartsWith("?"))
                    {
                        dependency.IsOptional = true;
                        mod.HasOptionalDependencies = true;
                        depString = depString.Replace("?", "").Trim(); // remove "?" and trim start/end whitespaces
                    }

                    // set name
                    dependency.Name = depString;

                    // Skip "base" dependency
                    if (dependency.Name.StartsWith("base", StringComparison.OrdinalIgnoreCase))
                        continue;

                    // Add dependency to dependencies collection
                    mod.DependenciesCollection.Add(dependency);
                }
            }

            // Add to mods collection
            Mods.Add(mod);
        }

        private void ItemEntryAdd(string filename)
        {
            // If the file does not end with .zip, skip it
            if (!filename.EndsWith(".zip"))
                return;

            // Get info.json as string
            var infoJsonString = JsonHelpers.GetModInfoJsonString(filename);

            // If unable to read info from archive - flag an error in file entry
            if (string.IsNullOrEmpty(infoJsonString))
            {
                Logger.WriteLine($"Error: Could not get infoJsonString from file: {filename} - String is null", true);

                // Add to collection with has error flag
                Mods.Add(new Mod { Title = filename, Name = filename, FullName = filename, HasError = true });

                // Skip rest of the execution
                return;
            }

            // Deserialize the json string to infojson object
            var infoJson = JsonConvert.DeserializeObject<InfoJson>(infoJsonString);

            if (infoJson != null)
            {
                // Check for duplicates
                if (Mods.Any(x => x.Name == infoJson.Name))
                    Logger.WriteLine($"Duplicated mod found: {infoJson.Name}");

                // Create mod
                AddModToCollection(infoJson, filename);
            }
        }

        private async void Execute_VerifyAuthenticationCmd(object obj)
        {
            if (await VerifyAuthentication())
                IsAuthenticated = true;
        }





        private async void Execute_InstallEntryCmd(string entryInstallCode)
        {
            if (!await NetcodeHelpers.VerifyInternetConnection())
            {
                // Open message box to user
                MessageBoxWindow.Show(ResourceHelper.GetValue("MessageBox_NoInternetConnection_Title"), ResourceHelper.GetValue("MessageBox_NoInternetConnection"), MessageBoxButton.OK);
                return;
            }

            if (IsUpdating || !IsAuthenticated)
                return;

            // Parse code to an array
            var code = entryInstallCode.Split('.');

            // If the array is empty, return
            if (code.Length <= 0)
                return;

            switch (code[0])
            {
                case "installNewMod":
                {
                    if (code[1] == "single")
                        await InstallEntry(installNewMod:true, modName:code[2]);
                    break;
                }

                case "installNewDependency":
                {
                    if (code[1] == "single")
                        await InstallEntry(installNewDependency:true, dependencyName:code[2]);
                    break;
                }

                case "updateExistingMod":
                {
                    switch (code[1])
                    {
                        case "all":
                            await InstallEntry(true, updateExistingMod:true, modNames:new List<string>(Mods.Where(x => x.UpdateAvailable).Select(x => x.Name)));
                            break;
                        case "single":
                            await InstallEntry(updateExistingMod:true, modName:code[2]);
                            break;
                    }
                    break;
                }
            }
        }

        private async Task InstallEntry([Optional] bool multiple, [Optional] bool installNewMod, [Optional] bool updateExistingMod, [Optional] bool installNewDependency, [Optional] IEnumerable<string> modNames, [Optional] string modName, [Optional] IEnumerable<string> dependencyNames, [Optional] string dependencyName)
        {
            IsUpdating = true;

            /*
             * Install New Mod
             */

            if (installNewMod)
            {
                
            }


            /*
             * Update Existing Mod
             */

            if (updateExistingMod)
            {
                // Set ExecuteUpdate boolean
                if (multiple && modNames != null)
                {
                    foreach (var name in modNames)
                    foreach (var mod in Mods.Where(x => x.UpdateAvailable))
                        if (name == mod.Name)
                            mod.ExecuteUpdate = true;
                }
                else if (!multiple && modName != null)
                {
                    foreach (var mod in Mods.Where(x => x.UpdateAvailable && x.Name == modName))
                        mod.ExecuteUpdate = true;
                }

                // Loop all entries in mods collection with execute update flag
                foreach (var mod in Mods.Where(x => x.ExecuteUpdate))
                {
                    Logger.WriteLine($"[INFO] Begin update of: {mod.Title}");

                    CurrentUpdatingMod = mod;
                    mod.IsUpdating = true;

                    // Execute the download
                    var modDownloader = new ModDownloader(mod, CurrentUpdatingMod);
                    await modDownloader.ExecuteDownload();

                    // If success
                    if (modDownloader.DownloadSuccessful)
                    {
                        // Delete old file
                        modDownloader.DeleteOldFile();

                        // Reset update properties
                        mod.UpdateAvailable = false;

                        // Set new filename
                        mod.Filename = mod.RemoteFilename;
                        mod.FilenameWithoutExtenion = Path.GetFileNameWithoutExtension(mod.Filename);
                        mod.FullName = Path.Combine(BaseVm.ConfigVm.ModsPath, mod.Filename);

                        // Set new version info
                        GetUpdatedVersionInfo(mod.FullName, out string version, out string factorioVersion);
                        mod.InstalledVersion = version;
                        mod.FactorioVersion = factorioVersion;
                    }

                    // Reset update properties
                    mod.IsUpdating = false;
                    mod.ExecuteUpdate = false;

                    // Log
                    Logger.WriteLine(modDownloader.DownloadSuccessful ? $"[INFO] Update of mod succeeded: {mod.Title}" : $"[ERROR] Update of mod failed: {mod.Title}");
                }
            }


            /*
             * Install New Dependency
             */

            if (installNewDependency)
            {
                // Create ModPortalAPI request
                var modPortalApi = new ModPortalApi();
                var sb = new StringBuilder();
                sb.Append("?page_size=max");
                sb.Append($"&namelist={dependencyName}");

                Logger.WriteLine($"[INFO] Called ModPortalAPI with request parameter: {sb}", true);

                // Build the Api Data
                await modPortalApi.BuildApiData(sb.ToString());

                if (modPortalApi.ApiData.Results != null)
                {
                    // Create temporary dependency object
                    var dependency = new Dependency
                    {
                        Name = dependencyName,
                        DownloadUrl = modPortalApi.ApiData.Results.First().Releases.First().DownloadUrl,
                        RemoteFilename = modPortalApi.ApiData.Results.First().Releases.First().FileName
                    };

                    // Execute the download
                    var modDownloader = new ModDownloader(dependency:dependency);
                    await modDownloader.ExecuteDownload();

                    if (modDownloader.DownloadSuccessful)
                    {
                        // Add to collection
                        ItemEntryAdd(Path.Combine(BaseVm.ConfigVm.ModsPath, dependency.RemoteFilename));

                        // Set new dependency as installed in collections
                        foreach (var mod in Mods)
                        {
                            foreach (var dep in mod.DependenciesCollection)
                            {
                                if (dep.Name == dependency.Name)
                                    dep.IsInstalled = true;
                            }
                        }
                    }
                }
            }


            // Set new TotalUpdatesAvailable and IsUpdatesAvailable
            TotalUpdatesAvailable = Mods.Count(x => x.UpdateAvailable);
            if (TotalUpdatesAvailable == 0)
                IsUpdatesAvailable = false;

            //IsUpdating = false;
        }

        private static void GetUpdatedVersionInfo(string infoJsonFilename, out string version, out string factorioVersion)
        {
            version = null;
            factorioVersion = null;

            // Get info.json as string
            var infoJsonString = JsonHelpers.GetModInfoJsonString(infoJsonFilename);
            if (infoJsonString == null) return;

            // Deserialize the json string to infojson object
            var infoJson = JsonConvert.DeserializeObject<InfoJson>(infoJsonString);
            if (infoJson == null) return;

            version = infoJson.Version;
            factorioVersion = infoJson.FactorioVersion;
        }
    }
}
