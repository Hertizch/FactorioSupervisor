using FactorioSupervisor.Extensions;
using FactorioSupervisor.Helpers;
using FactorioSupervisor.Models;
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

            if (!Directory.Exists(_profilesPath))
                Directory.CreateDirectory(_profilesPath);

            if (LoadProfilesCmd.CanExecute(null))
                LoadProfilesCmd.Execute(null);

            // Create a default profile if collection is empty
            if (Profiles.Count <= 0)
                Profiles.Add(new Profile { Name = "Default", Filename = "Default.json" });

            SelectedProfile = Profiles.First();

            if (SwitchProfileCmd.CanExecute(null))
                SwitchProfileCmd.Execute(null);
        }

        /*
         * Fields
         */

        private string _profilesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "profiles");
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
            get { return _profiles; }
            set { if (value == _profiles) return; _profiles = value; OnPropertyChanged(nameof(Profiles)); }
        }

        /// <summary>
        /// Gets or sets the currently selected profile
        /// </summary>
        public Profile SelectedProfile
        {
            get { return _selectedProfile; }
            set { if (value == _selectedProfile) return; _selectedProfile = value; OnPropertyChanged(nameof(SelectedProfile)); }
        }

        /// <summary>
        /// Gets or sets the previously selected profile
        /// </summary>
        public Profile PreviousProfile
        {
            get { return _previousProfile; }
            set { if (value == _previousProfile) return; _previousProfile = value; OnPropertyChanged(nameof(PreviousProfile)); }
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
            (_deleteProfileCmd = new RelayCommand(p => Execute_DeleteProfileCmd(p as Profile), p => true));

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

            Exception exception = null;

            // Write to file
            try
            {
                File.WriteAllText(profileFilename, json);
            }
            catch (Exception ex)
            {
                exception = ex;
                Logger.WriteLine($"Failed to write file '{profileFilename}'", true, ex);
            }
            finally
            {
                if (exception == null)
                    Logger.WriteLine($"Saved PreviousProfile '{SelectedProfile.Name}' to '{profileFilename}'", true);
            }
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

            // Get filename
            var previousProfileFilename = Path.Combine(_profilesPath, StringExtensions.MakeValidFileName(PreviousProfile.Name) + ".json");

            // Serialize to json string
            var json = JsonConvert.SerializeObject(PreviousProfile, Formatting.Indented);

            Exception exception = null;

            // Write to file
            try
            {
                File.WriteAllText(previousProfileFilename, json);
            }
            catch (Exception ex)
            {
                exception = ex;
                Logger.WriteLine($"Failed to write file '{previousProfileFilename}'", true, ex);
            }
            finally
            {
                if (exception == null)
                    Logger.WriteLine($"Saved PreviousProfile '{PreviousProfile.Name}' to '{previousProfileFilename}'", true);
            }

            /*
             * Load and set SelectedProfile from newProfile
             */

            SelectedProfile = newProfile;

            // Set enabled mods
            SetEnabledMods();

            Logger.WriteLine($"Loaded SelectedProfile '{SelectedProfile.Name}'", true);
        }

        private void Execute_CreateProfileCmd(object obj)
        {
            string newProfileName = "New profile";

            if (Profiles.Any(x => x.Name.StartsWith("New profile")))
                newProfileName = "New profile_" + (Profiles.Count(x => x.Name.StartsWith("New profile")) + 1);

            Profiles.Add(new Profile { Name = newProfileName, Filename = newProfileName + ".json" });
        }

        private void Execute_DeleteProfileCmd(Profile selectedProfile)
        {
            Logger.WriteLine($"Executing method '{nameof(Execute_DeleteProfileCmd)}'");

            if (selectedProfile == null)
            {
                Logger.WriteLine($"This is a bug, select the profile you wish to delete first.", true);
                return;
            }

            var filename = Path.Combine(_profilesPath, selectedProfile.Filename);

            // Delete profile file
            if (File.Exists(filename))
            {
                File.Delete(filename);
                Logger.WriteLine($"Deleted file '{selectedProfile.Filename}'");
            }

            // Remove from collection
            Profiles.RemoveAt(Profiles.IndexOf(selectedProfile));

            // Create a default profile if collection is empty
            if (Profiles.Count == 0)
                Profiles.Add(new Profile { Name = "Default", Filename = "Default.json" });

            SelectedProfile = Profiles.First();
        }

        private static IEnumerable<string> GetEnabledModNames()
        {
            return BaseVm.ModsVm.Mods.Count > 0 ? new List<string>(BaseVm.ModsVm.Mods.Where(x => x.IsEnabled).Select(x => x.Name).ToList()) : null;
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
