using FactorioSupervisor.Extensions;
using FactorioSupervisor.Helpers;
using FactorioSupervisor.Models;
using ModsApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
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

            if (Mods.Count == 0)
            {
                /* Create example mod */
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
                        "base >= 0.14.21",
                        "data-raw-prototypes >= 0.2.1",
                        "?aai-vehicles-flame-tumbler >= 0.2.1",
                        "?aai-vehicles-laser-tank >= 0.2.1",
                        "?bullet-trails >= 0.2.1",
                        "aai-programmable-structures >= 0.3.1",
                        "aai-zones >= 0.2.1",
                        "detached-gun-sounds >= 0.2.1",
                        "off-grid-effects >= 0.2.1",
                        "aai-signals >= 0.2.1",
                        "aai-vehicles-miner >= 0.2.1",
                        "aai-vehicles-hauler >= 0.2.1",
                        "?aai-vehicles-chaingunner >= 0.2.1"
                    },
                    FullName = @"C:\mods\example",
                    FilenameWithoutExtenion = "example_mod",
                    ReleasedAt = TimeHelpers.GetTimeSpanDuration("2017-04-28T07:47:58.903536Z"),
                    RemoteVersion = "0.3.2"
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

                /* Create example mod END */
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

        private ObservableCollection<Mod> _mods;
        private Mod _selectedMod;
        private bool _isCheckingForUpdates;
        private bool _isUpdating;
        private bool _isUpdatesAvailable;
        private bool _showProgressBar;
        private bool _fileDownloadSucceeded;
        private bool _hideIncompatibleMods;
        private FileSystemWatcher _fileSystemWatcher;
        private RelayCommand _getLocalModsCmd;
        private RelayCommand _toggleEnableModsCmd;
        private RelayCommand _openHyperlinkCmd;
        private RelayCommand _getModRemoteDataCmd;
        private RelayCommand _downloadModCmd;
        private RelayCommand _launchFactorioCmd;
        private RelayCommand _watchModDirChangesCmd;
        private RelayCommand _deleteModCmd;

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
        /// Gets or sets a boolean value whether to hide incompatible mods
        /// </summary>
        public bool HideIncompatibleMods
        {
            get { return _hideIncompatibleMods; }
            set
            {
                if (value == _hideIncompatibleMods)
                    return;

                _hideIncompatibleMods = value;
                OnPropertyChanged(nameof(HideIncompatibleMods));

                if (HideIncompatibleMods)
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
                        {
                            mod.HideInModList = false;
                        }
                    }
                }
                else
                {
                    foreach (var mod in Mods)
                        mod.HideInModList = false;
                }

                
            }
        }

        /*
         * Commands
         */

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
            (_launchFactorioCmd = new RelayCommand(Execute_LaunchFactorioCmd, p => File.Exists(BaseVm.ConfigVm.FactorioExePath)));

        public RelayCommand WatchModDirChangesCmd => _watchModDirChangesCmd ??
            (_watchModDirChangesCmd = new RelayCommand(Execute_WatchModDirChangesCmd, p => Directory.Exists(BaseVm.ConfigVm.ModsPath)));

        public RelayCommand DeleteModCmd => _deleteModCmd ??
            (_deleteModCmd = new RelayCommand(Execute_DeleteModCmd, p => true));

        /*
         * Methods
         */

        private void Execute_GetLocalModsCmd(object obj)
        {
            // Reset collection if reloading
            if (Mods.Count > 0)
                Mods.Clear();

            // Get all filesystementries from mods directory
            var fileEntries = Directory.GetFileSystemEntries(BaseVm.ConfigVm.ModsPath, "*", SearchOption.TopDirectoryOnly).ToList();

            // Loop all entries
            foreach (var fileEntry in fileEntries)
            {
                // If the file does not end with .zip, skip it
                if (!fileEntry.EndsWith(".zip"))
                    continue;

                // Get info.json as string
                string infoJsonString = JsonHelpers.GetModInfoJsonString(fileEntry);

                // If unable to read info from archive - flag an error in file entry
                if (string.IsNullOrEmpty(infoJsonString))
                {
                    Logger.WriteLine($"Error: Could not get infoJsonString from file: {fileEntry} - String is null", true);

                    // Add to collection with has error flag
                    Mods.Add(new Mod { Title = fileEntry, Name = fileEntry, FullName = fileEntry, HasError = true });

                    // Skip rest of the execution for this entry
                    continue;
                }

                // Deserialize the json string to infojson object
                var infoJson = JsonConvert.DeserializeObject<InfoJson>(infoJsonString);

                if (infoJson != null)
                {
                    // Create mod
                    var mod = new Mod()
                    {
                        Name = infoJson.Name,
                        Title = infoJson.Title,
                        Description = infoJson.Description.Replace("\n", ""), // remove newlines
                        FactorioVersion = infoJson.FactorioVersion,
                        InstalledVersion = infoJson.Version,
                        Author = infoJson.Author,
                        Homepage = infoJson.Homepage != null && infoJson.Homepage.StartsWith("http") ? infoJson.Homepage : null, // add only valid urls
                        Dependencies = infoJson.Dependencies,
                        FullName = fileEntry,
                        Filename = Path.GetFileName(fileEntry),
                        FilenameWithoutExtenion = Path.GetFileNameWithoutExtension(fileEntry)
                    };

                    // Set dependencies
                    if (mod.Dependencies != null)
                    {
                        foreach (var dep in mod.Dependencies)
                        {
                            var depStr = dep.ToString();

                            var dependency = new Dependency();

                            // set optional deps
                            if (depStr.StartsWith("?"))
                            {
                                dependency.IsOptional = true;
                                depStr = depStr.Replace("?", "").Trim();
                            }
                            
                            // set name
                            dependency.Name = depStr;

                            // Skip base mod
                            if (dependency.Name.StartsWith("base", StringComparison.OrdinalIgnoreCase))
                                continue;

                            mod.DependenciesCollection.Add(dependency);
                        }

                        // check if any optional deps
                        if (mod.DependenciesCollection.Any(x => x.IsOptional))
                            mod.HasOptionalDependencies = true;
                    }

                    // Add to collection
                    Mods.Add(mod);
                }
            }

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
            if (Mods.Where(x => x.IsEnabled).Count() == Mods.Count)
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
            ShowProgressBar = true;
            IsCheckingForUpdates = true;

            // Create api class
            var modPortalApi = new ModPortalApi();

            Logger.WriteLine($"Attempting connection with mod portal...");

            // Check if api is reachable
            if (await modPortalApi.CanReachApi())
            {
                Logger.WriteLine($"Connection successfull!");

                // Build the request string
                var sb = new StringBuilder();
                sb.Append("?page_size=max");
                foreach (var mod in Mods.Where(x => !x.HasError))
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

                    if (updateCount > 1)
                        BaseVm.NotifyBannerRelay.SetNotifyBanner($"{updateCount} updates available!");
                    else
                        BaseVm.NotifyBannerRelay.SetNotifyBanner($"{updateCount} update available!");

                    Logger.WriteLine($"{Mods.Count(x => x.UpdateAvailable)} updates available", true);
                }
                else
                {
                    BaseVm.NotifyBannerRelay.SetNotifyBanner("No updates available");
                    Logger.WriteLine("No updates available", true);
                }
            }
            else
            {
                BaseVm.MessageBoxRelay.SetMessageBox("Host unreachable", "The updater was unable to reach the mod portal. It may be temporarily down, or experiencing heavy load. Try agin later.");
                Logger.WriteLine("Failed to reach host: https://mods.factorio.com/api/mods", true);
            }

            IsCheckingForUpdates = false;
            ShowProgressBar = false;
        }

        private async void Execute_DownloadModCmd(object obj)
        {
            ShowProgressBar = true;
            IsUpdating = true;

            var authenticationApi = new AuthenticationApi();

            // Get vars from configVm
            var settings = BaseVm.ConfigVm;
            var username = settings.ModPortalUsername;
            var password = settings.ModPortalPassword;

            if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password))
            {
                Logger.WriteLine($"Aquiring authentication with mod portal...");

                // Get authentication token
                var token = await authenticationApi.GetAuthenticationToken(username, password, true);

                if (authenticationApi.Success)
                {
                    Logger.WriteLine($"Authentication successful!");

                    // Wait 500ms
                    await Task.Delay(500);

                    // Loop all mods with UpdateAvailable flag
                    foreach (var mod in Mods.Where(x => x.UpdateAvailable))
                    {
                        mod.IsUpdating = true;

                        // Show notification
                        BaseVm.NotifyBannerRelay.SetNotifyBanner($"Downloading: {mod.Title}...");

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

                        // Show notification
                        BaseVm.NotifyBannerRelay.SetNotifyBanner($"Updated: {mod.Title}");

                        mod.IsUpdating = false;
                    }

                    // Reset IsUpdatesAvailable flag
                    if (Mods.Count(x => x.UpdateAvailable) == 0)
                        IsUpdatesAvailable = false;
                }
                else
                {
                    Logger.WriteLine($"Authentication failed - Error message: {authenticationApi.ErrorMessage}");
                    BaseVm.NotifyBannerRelay.SetNotifyBanner("Authentication failed!");
                }
            }
            else
            {
                Logger.WriteLine($"Authentication failed - Username and/or password is null", true);
                BaseVm.NotifyBannerRelay.SetNotifyBanner("Authentication failed!");
            }

            ShowProgressBar = false;
            IsUpdating = false;
        }

        private async Task ExecuteDownload(Mod mod, string userName, string token)
        {
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

            // Create and start process
            using (var process = new Process())
            {
                {
                    process.StartInfo.FileName = BaseVm.ConfigVm.FactorioExePath;
                    process.Start();
                }
            }
        }

        private void Execute_DeleteModCmd(object obj)
        {
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
            string watchDir = BaseVm.ConfigVm.ModsPath;

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
            Logger.WriteLine($"[INFO]: Mod directory change detected - event: {e.ChangeType.ToString()} file: '{e.FullPath}'", true);
        }

        private void _fileSystemWatcher_Deleted(object sender, FileSystemEventArgs e)
        {
            // on deleted
            Logger.WriteLine($"[INFO]: Mod directory change detected - event: {e.ChangeType.ToString()} file: '{e.FullPath}'", true);
        }

        private void _fileSystemWatcher_Renamed(object sender, RenamedEventArgs e)
        {
            // on renamed
            Logger.WriteLine($"[INFO]: Mod directory change detected - event: {e.ChangeType.ToString()} file: '{e.FullPath}'", true);
        }
    }
}
