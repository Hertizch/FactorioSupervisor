using FactorioSupervisor.Extensions;
using Newtonsoft.Json.Linq;

namespace FactorioSupervisor.Models
{
    public class Mod : ObservableObject
    {
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
        private JToken _dependencies;
        private bool _updateAvailable;
        private bool _isUpdating;
        private int _progressPercentage;
        private bool _hasError;

        /// <summary>
        /// Gets or sets the filename with extension
        /// </summary>
        public string Filename
        {
            get { return _filename; }
            set { if (value == _filename) return; _filename = value; OnPropertyChanged(nameof(Filename)); }
        }

        /// <summary>
        /// Gets or sets the filename without extension
        /// </summary>
        public string FilenameWithoutExtenion
        {
            get { return _filenameWithoutExtenion; }
            set { if (value == _filenameWithoutExtenion) return; _filenameWithoutExtenion = value; OnPropertyChanged(nameof(FilenameWithoutExtenion)); }
        }

        /// <summary>
        /// Gets or sets the filename and path
        /// </summary>
        public string FullName
        {
            get { return _fullName; }
            set { if (value == _fullName) return; _fullName = value; OnPropertyChanged(nameof(FullName)); }
        }

        /// <summary>
        /// Gets or sets the name as defined in info.json
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { if (value == _name) return; _name = value; OnPropertyChanged(nameof(Name)); }
        }

        /// <summary>
        /// Gets or sets the title as defined in info.json
        /// </summary>
        public string Title
        {
            get { return _title; }
            set { if (value == _title) return; _title = value; OnPropertyChanged(nameof(Title)); }
        }

        /// <summary>
        /// Gets or sets the description as defined in info.json
        /// </summary>
        public string Description
        {
            get { return _description; }
            set { if (value == _description) return; _description = value; OnPropertyChanged(nameof(Description)); }
        }

        /// <summary>
        /// Gets or sets the author(s) as defined in info.json
        /// </summary>
        public JToken Author
        {
            get { return _author; }
            set { if (value == _author) return; _author = value; OnPropertyChanged(nameof(Author)); }
        }

        /// <summary>
        /// Gets or sets the homepage as defined in info.json
        /// </summary>
        public string Homepage
        {
            get { return _homepage; }
            set { if (value == _homepage) return; _homepage = value; OnPropertyChanged(nameof(Homepage)); }
        }

        /// <summary>
        /// Gets or sets the currently installed version number as defined in info.json
        /// </summary>
        public string InstalledVersion
        {
            get { return _installedVersion; }
            set { if (value == _installedVersion) return; _installedVersion = value; OnPropertyChanged(nameof(InstalledVersion)); }
        }

        /// <summary>
        /// Gets or sets the latest online version
        /// </summary>
        public string RemoteVersion
        {
            get { return _remoteVersion; }
            set { if (value == _remoteVersion) return; _remoteVersion = value; OnPropertyChanged(nameof(RemoteVersion)); }
        }

        /// <summary>
        /// Gets or sets the latest online version release date
        /// </summary>
        public string ReleasedAt
        {
            get { return _releasedAt; }
            set { if (value == _releasedAt) return; _releasedAt = value; OnPropertyChanged(nameof(ReleasedAt)); }
        }

        /// <summary>
        /// Gets or sets the filename of the latest release
        /// </summary>
        public string RemoteFilename
        {
            get { return _remoteFilename; }
            set { if (value == _remoteFilename) return; _remoteFilename = value; OnPropertyChanged(nameof(RemoteFilename)); }
        }

        /// <summary>
        /// Gets or sets the latest version download url
        /// </summary>
        public string DownloadUrl
        {
            get { return _downloadUrl; }
            set { if (value == _downloadUrl) return; _downloadUrl = value; OnPropertyChanged(nameof(DownloadUrl)); }
        }

        /// <summary>
        /// Gets or sets a boolean value if the mod should be launched
        /// </summary>
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set { if (value == _isEnabled) return; _isEnabled = value; OnPropertyChanged(nameof(IsEnabled)); }
        }

        /// <summary>
        /// Gets or sets the factorio version as defined in info.json
        /// </summary>
        public string FactorioVersion
        {
            get { return _factorioVersion; }
            set { if (value == _factorioVersion) return; _factorioVersion = value; OnPropertyChanged(nameof(FactorioVersion)); }
        }

        /// <summary>
        /// Gets or sets the dependency(ies) as defined in info.json
        /// </summary>
        public JToken Dependencies
        {
            get { return _dependencies; }
            set { if (value == _dependencies) return; _dependencies = value; OnPropertyChanged(nameof(Dependencies)); }
        }

        /// <summary>
        /// Gets or sets a boolean value if the mod has an available update
        /// </summary>
        public bool UpdateAvailable
        {
            get { return _updateAvailable; }
            set { if (value == _updateAvailable) return; _updateAvailable = value; OnPropertyChanged(nameof(UpdateAvailable)); }
        }

        /// <summary>
        /// Gets or sets a boolean value if the mod is currently updating
        /// </summary>
        public bool IsUpdating
        {
            get { return _isUpdating; }
            set { if (value == _isUpdating) return; _isUpdating = value; OnPropertyChanged(nameof(IsUpdating)); }
        }

        /// <summary>
        /// Gets or sets the current mod update progress as a percentage value
        /// </summary>
        public int ProgressPercentage
        {
            get { return _progressPercentage; }
            set { if (value == _progressPercentage) return; _progressPercentage = value; OnPropertyChanged(nameof(ProgressPercentage)); }
        }

        /// <summary>
        /// Gets or sets a boolean value if the mod entountered any error while being read
        /// </summary>
        public bool HasError
        {
            get { return _hasError; }
            set { if (value == _hasError) return; _hasError = value; OnPropertyChanged(nameof(HasError)); }
        }
    }
}
