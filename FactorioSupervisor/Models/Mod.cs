using System;
using FactorioSupervisor.Extensions;
using FactorioSupervisor.ObservableImmutable;
using Newtonsoft.Json.Linq;

namespace FactorioSupervisor.Models
{
    public class Mod : ObservableObject
    {
        public Mod()
        {
            DependenciesCollection = new ObservableImmutableList<Dependency>();
        }

        private string _filename;
        private string _filenameWithoutExtenion;
        private string _fullName;
        private string _name;
        private string _title;
        private string _description;
        private JToken _author;
        private string _homepage;
        private string _installedVersion;
        private string _remoteVersion;
        private string _releasedAt;
        private string _remoteFilename;
        private string _downloadUrl;
        private bool _isEnabled;
        private string _factorioVersion;
        private string _remoteFactorioVersion;
        private JToken _dependencies;
        private bool _updateAvailable;
        private bool _isUpdating;
        private bool _executeUpdate;
        private double _progressPercentage;
        private bool _hasError;
        private bool _hideInModList;
        private ObservableImmutableList<Dependency> _dependenciesCollection;
        private bool _hasOptionalDependencies;
        private Dependency _selectedDependency;

        /// <summary>
        /// Gets or sets the filename with extension
        /// </summary>
        public string Filename
        {
            get => _filename; set { if (value == _filename) return; _filename = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Gets or sets the filename without extension
        /// </summary>
        public string FilenameWithoutExtenion
        {
            get => _filenameWithoutExtenion; set { if (value == _filenameWithoutExtenion) return; _filenameWithoutExtenion = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Gets or sets the filename and path
        /// </summary>
        public string FullName
        {
            get => _fullName; set { if (value == _fullName) return; _fullName = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Gets or sets the name as defined in info.json
        /// </summary>
        public string Name
        {
            get => _name; set { if (value == _name) return; _name = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Gets or sets the title as defined in info.json
        /// </summary>
        public string Title
        {
            get => _title; set { if (value == _title) return; _title = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Gets or sets the description as defined in info.json
        /// </summary>
        public string Description
        {
            get => _description; set { if (value == _description) return; _description = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Gets or sets the author(s) as defined in info.json
        /// </summary>
        public JToken Author
        {
            get => _author; set { if (value == _author) return; _author = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Gets or sets the homepage as defined in info.json
        /// </summary>
        public string Homepage
        {
            get => _homepage; set { if (value == _homepage) return; _homepage = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Gets or sets the currently installed version number as defined in info.json
        /// </summary>
        public string InstalledVersion
        {
            get => _installedVersion; set { if (value == _installedVersion) return; _installedVersion = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Gets or sets the latest online version
        /// </summary>
        public string RemoteVersion
        {
            get => _remoteVersion; set { if (value == _remoteVersion) return; _remoteVersion = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Gets or sets the latest online version release date
        /// </summary>
        public string ReleasedAt
        {
            get => _releasedAt; set { if (value == _releasedAt) return; _releasedAt = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Gets or sets the filename of the latest release
        /// </summary>
        public string RemoteFilename
        {
            get => _remoteFilename; set { if (value == _remoteFilename) return; _remoteFilename = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Gets or sets the latest version download url
        /// </summary>
        public string DownloadUrl
        {
            get => _downloadUrl; set { if (value == _downloadUrl) return; _downloadUrl = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Gets or sets a boolean value if the mod should be launched
        /// </summary>
        public bool IsEnabled
        {
            get => _isEnabled; set { if (value == _isEnabled) return; _isEnabled = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Gets or sets the factorio version as defined in info.json
        /// </summary>
        public string FactorioVersion
        {
            get => _factorioVersion; set { if (value == _factorioVersion) return; _factorioVersion = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Gets or sets the latest online factorio version
        /// </summary>
        public string RemoteFactorioVersion
        {
            get => _remoteFactorioVersion; set { if (value == _remoteFactorioVersion) return; _remoteFactorioVersion = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Gets or sets the dependency(ies) as defined in info.json
        /// </summary>
        public JToken Dependencies
        {
            get => _dependencies; set { if (value == _dependencies) return; _dependencies = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Gets or sets a boolean value if the mod has an available update
        /// </summary>
        public bool UpdateAvailable
        {
            get => _updateAvailable; set { if (value == _updateAvailable) return; _updateAvailable = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Gets or sets a boolean value if the mod is currently updating
        /// </summary>
        public bool IsUpdating
        {
            get => _isUpdating; set { if (value == _isUpdating) return; _isUpdating = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Gets or sets a boolean value if the update should execute
        /// </summary>
        public bool ExecuteUpdate
        {
            get => _executeUpdate; set { if (value == _executeUpdate) return; _executeUpdate = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Gets or sets the current mod update progress as a percentage value
        /// </summary>
        public double ProgressPercentage
        {
            get => _progressPercentage; set { if (Math.Abs(value - _progressPercentage) < 0.01) return; _progressPercentage = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Gets or sets a boolean value if the mod entountered any error while being read
        /// </summary>
        public bool HasError
        {
            get => _hasError; set { if (value == _hasError) return; _hasError = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Gets or sets a boolean value if the mod should be dispalyed in the mod list
        /// </summary>
        public bool HideInModList
        {
            get => _hideInModList; set { if (value == _hideInModList) return; _hideInModList = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Gets or sets the dependencies collection
        /// </summary>
        public ObservableImmutableList<Dependency> DependenciesCollection
        {
            get => _dependenciesCollection; set { if (value == _dependenciesCollection) return; _dependenciesCollection = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Gets or sets a boolean value if the dependencies collection has optional items
        /// </summary>
        public bool HasOptionalDependencies
        {
            get => _hasOptionalDependencies; set { if (value == _hasOptionalDependencies) return; _hasOptionalDependencies = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Gets or sets the currently selected dependency object
        /// </summary>
        public Dependency SelectedDependency
        {
            get => _selectedDependency; set { if (value == _selectedDependency) return; _selectedDependency = value; OnPropertyChanged(); }
        }
    }
}
