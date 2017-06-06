using FactorioSupervisor.Extensions;
using FactorioSupervisor.Helpers;
using FactorioSupervisor.Models;
using FactorioSupervisor.Properties;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace FactorioSupervisor.ViewModels
{
    public class ProfilesVm : ObservableObject
    {
        public ProfilesVm()
        {
            Logger.WriteLine($"Class created: {nameof(ProfilesVm)}");

            Profiles = new ObservableCollection<Profile>();

            // Create profiles folder
            if (!Directory.Exists(_profilesPath))
            {
                Logger.WriteLine($"Directory: '{_profilesPath}' does not exist", true);

                Exception exception = null;

                try
                {
                    Directory.CreateDirectory(_profilesPath);
                }
                catch (Exception ex)
                {
                    exception = ex;
                    Logger.WriteLine($"ERROR: Failed to create directory: '{_profilesPath}'", true, ex);
                }
                finally
                {
                    if (exception == null)
                        Logger.WriteLine($"Successfully created directory: '{_profilesPath}'", true);
                }
            }

            // Load profiles
            if (LoadProfilesCmd.CanExecute(null))
                LoadProfilesCmd.Execute(null);

            // Create a default profile if needed
            CreateDefaultProfile();

            // Select profile from settings
            if (!string.IsNullOrEmpty(Settings.Default.SelectedProfile))
            {
                var profileExists = Profiles.Any(x => x.Name == Settings.Default.SelectedProfile);

                if (profileExists)
                {
                    SelectedProfile = Profiles.First(x => x.Name == Settings.Default.SelectedProfile);
                    Logger.WriteLine($"Property: '{nameof(Settings.Default.SelectedProfile)}' has value: '{Settings.Default.SelectedProfile}' and exists in profiles collection: '{profileExists}' - Setting property: '{nameof(SelectedProfile)}' to value: '{Settings.Default.SelectedProfile}'");
                }
                else
                {
                    SelectedProfile = Profiles.First();
                    Logger.WriteLine($"Property: '{nameof(Settings.Default.SelectedProfile)}' has value: '{Settings.Default.SelectedProfile}' and exists in profiles collection: '{profileExists}' - Setting property: '{nameof(SelectedProfile)}' to value: '{Profiles.First().Name}'");
                }
            }
            else
            {
                SelectedProfile = Profiles.First();
                Logger.WriteLine($"Property: '{nameof(Settings.Default.SelectedProfile)}' is null - Setting property: '{nameof(SelectedProfile)}' to value: '{Profiles.First().Name}'");
            }

            if (SwitchProfileCmd.CanExecute(null))
                SwitchProfileCmd.Execute(null);
        }

        /*
         * Fields
         */

        private readonly string _profilesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "profiles");
        private ObservableCollection<Profile> _profiles;
        private Profile _selectedProfile;
        private Profile _previousProfile;
        private RelayCommand _saveSelectedProfileCmd;
        private RelayCommand _loadProfilesCmd;
        private RelayCommand _switchProfileCmd;
        private RelayCommand _createProfileCmd;
        private RelayCommand _deleteProfileCmd;

        /*
         * Properties
         */

        /// <summary>
        /// Gets or sets the collection of profiles
        /// </summary>
        public ObservableCollection<Profile> Profiles
        {
            get => _profiles; set { if (value == _profiles) return; _profiles = value; OnPropertyChanged(nameof(Profiles)); }
        }

        /// <summary>
        /// Gets or sets the currently selected profile
        /// </summary>
        public Profile SelectedProfile
        {
            get => _selectedProfile; set { if (value == _selectedProfile) return; _selectedProfile = value; OnPropertyChanged(nameof(SelectedProfile)); }
        }

        /// <summary>
        /// Gets or sets the previously selected profile
        /// </summary>
        public Profile PreviousProfile
        {
            get => _previousProfile; set { if (value == _previousProfile) return; _previousProfile = value; OnPropertyChanged(nameof(PreviousProfile)); }
        }

        /*
         * Commands
         */

        public RelayCommand SaveSelectedProfileCmd => _saveSelectedProfileCmd ??
            (_saveSelectedProfileCmd = new RelayCommand(Execute_SaveSelectedProfileCmd, p => true));

        public RelayCommand LoadProfilesCmd => _loadProfilesCmd ??
            (_loadProfilesCmd = new RelayCommand(Execute_LoadProfilesCmd, p => true));

        public RelayCommand SwitchProfileCmd => _switchProfileCmd ??
            (_switchProfileCmd = new RelayCommand(p => Execute_SwitchProfileCmd(p as Profile), p => true));

        public RelayCommand CreateProfileCmd => _createProfileCmd ??
            (_createProfileCmd = new RelayCommand(Execute_CreateProfileCmd, p => true));

        public RelayCommand DeleteProfileCmd => _deleteProfileCmd ??
            (_deleteProfileCmd = new RelayCommand(Execute_DeleteProfileCmd, p => true));

        /*
         * Methods
         */

        private void Execute_LoadProfilesCmd(object obj)
        {
            Logger.WriteLine($"Executing method '{nameof(Execute_LoadProfilesCmd)}'");

            var fileEntries = Directory.GetFileSystemEntries(_profilesPath, "*.json", SearchOption.TopDirectoryOnly).ToList();

            foreach (var fileEntry in fileEntries)
            {
                var json = File.ReadAllText(fileEntry);
                var profile = JsonConvert.DeserializeObject<Profile>(json);

                Profiles.Add(profile);
            }
        }

        private void Execute_SaveSelectedProfileCmd(object obj)
        {
            Logger.WriteLine($"Executing method '{nameof(Execute_SaveSelectedProfileCmd)}'");

            // Get filename
            var profileFilename = Path.Combine(_profilesPath, StringExtensions.MakeValidFileName(SelectedProfile.Name) + ".json");
            SelectedProfile.Filename = Path.GetFileName(profileFilename);

            // Get enabled mod names
            SelectedProfile.EnabledModNames = GetEnabledModNames();

            // Serialize to json string
            var json = JsonConvert.SerializeObject(SelectedProfile, Formatting.Indented);

            // Write to file
            WriteProfileJsonToFile(SelectedProfile, json);
        }

        private void Execute_SwitchProfileCmd(Profile newProfile)
        {
            Logger.WriteLine($"Executing method '{nameof(Execute_SwitchProfileCmd)}'");

            // If newProfile is null, we assume the app is in startup - so set enabled mods for the currently selected profile
            if (newProfile == null)
            {
                SetEnabledMods();
                return;
            }

            /*
             * Set and save PreviousProfile from SelectedProfile
             */

            PreviousProfile = SelectedProfile;

            // Get enabled mod names
            PreviousProfile.EnabledModNames = GetEnabledModNames();

            // Serialize to json string
            var json = JsonConvert.SerializeObject(PreviousProfile, Formatting.Indented);

            // Write to file
            WriteProfileJsonToFile(PreviousProfile, json);

            /*
             * Load and set SelectedProfile from newProfile
             */

            SelectedProfile = newProfile;

            // Set enabled mods
            SetEnabledMods();

            Logger.WriteLine($"Loaded SelectedProfile '{SelectedProfile.Name}'", true);
        }

        private void WriteProfileJsonToFile(Profile profile, string json)
        {
            // Set filename
            var filename = Path.Combine(_profilesPath, StringExtensions.MakeValidFileName(profile.Name) + ".json");

            Exception exception = null;

            try
            {
                File.WriteAllText(filename, json);
            }
            catch (Exception ex)
            {
                exception = ex;
                Logger.WriteLine($"ERROR: Failed to create/overwrite file: '{filename}'", true, ex);
            }
            finally
            {
                if (exception == null)
                    Logger.WriteLine($"Successfully created/overwritten file: '{filename}' for profile: '{profile.Name}'", true);
            }
        }

        private void Execute_CreateProfileCmd(object obj)
        {
            string newProfileName = "New profile";

            if (Profiles.Any(x => x.Name.StartsWith("New profile")))
                newProfileName = "New profile_" + (Profiles.Count(x => x.Name.StartsWith("New profile")) + 1);

            Profiles.Add(new Profile { Name = newProfileName, Filename = newProfileName + ".json" });
        }

        private void Execute_DeleteProfileCmd(object obj)
        {
            Logger.WriteLine($"Executing method '{nameof(Execute_DeleteProfileCmd)}'");

            var filename = Path.Combine(_profilesPath, SelectedProfile.Filename);

            Exception exception = null;

            // Delete profile file
            try
            {
                File.Delete(filename);
            }
            catch (Exception ex)
            {
                exception = ex;
                Logger.WriteLine($"ERROR: Failed to delete file: '{SelectedProfile.Filename}' for profile: '{SelectedProfile.Name}'", true, ex);
            }
            finally
            {
                if (exception == null)
                {
                    Logger.WriteLine($"Successfully deleted file: '{SelectedProfile.Filename}' for profile: '{SelectedProfile.Name}'", true);

                    // Remove from collection
                    Profiles.RemoveAt(Profiles.IndexOf(SelectedProfile));
                }
            }

            // Create a default profile if needed
            CreateDefaultProfile();

            SelectedProfile = Profiles.First();
        }

        private static IEnumerable<string> GetEnabledModNames()
        {
            return BaseVm.ModsVm.Mods.Count > 0 ? new List<string>(BaseVm.ModsVm.Mods.Where(x => x.IsEnabled).Select(x => x.Name).ToList()) : null;
        }

        private void CreateDefaultProfile()
        {
            if (Profiles.Count == 0)
            {
                var profile = new Profile()
                {
                    Name = "Default",
                    Filename = "Default.json"
                };

                Profiles.Add(profile);

                Logger.WriteLine($"Profiles collection count is 0 - Created default profile: '{profile.Name}'.");
            }
        }

        private void SetEnabledMods()
        {
            // Clear all is enabled
            foreach (var mod in BaseVm.ModsVm.Mods)
                mod.IsEnabled = false;

            // If null it's most likely a new profile
            if (SelectedProfile.EnabledModNames == null)
                return;

            foreach (var mod in from enabledModName in SelectedProfile.EnabledModNames
                                where BaseVm.ModsVm.Mods.Count > 0
                                from mod in BaseVm.ModsVm.Mods.Where(mod => mod.Name == enabledModName)
                                select mod)
                mod.IsEnabled = true;
        }
    }
}
