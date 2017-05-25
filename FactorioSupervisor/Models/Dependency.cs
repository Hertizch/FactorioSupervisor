using FactorioSupervisor.Extensions;

namespace FactorioSupervisor.Models
{
    public class Dependency : ObservableObject
    {
        private string _name;
        private bool _isOptional;
        private bool _isInstalled;
        private string _remoteFilename;
        private string _downloadUrl;
        private bool _updateAvailable;
        private bool _isUpdating;
        private double _progressPercentage;

        /// <summary>
        /// Gets or sets the name of the dependency
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { if (value == _name) return; _name = value; OnPropertyChanged(nameof(Name)); }
        }

        /// <summary>
        /// Gets or sets a boolean value if the dependency is optional
        /// </summary>
        public bool IsOptional
        {
            get { return _isOptional; }
            set { if (value == _isOptional) return; _isOptional = value; OnPropertyChanged(nameof(IsOptional)); }
        }

        /// <summary>
        /// Gets or sets a boolean value if the dependency is installed
        /// </summary>
        public bool IsInstalled
        {
            get { return _isInstalled; }
            set { if (value == _isInstalled) return; _isInstalled = value; OnPropertyChanged(nameof(IsInstalled)); }
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
        /// Gets or sets a boolean value if the dependency has an available update
        /// </summary>
        public bool UpdateAvailable
        {
            get { return _updateAvailable; }
            set { if (value == _updateAvailable) return; _updateAvailable = value; OnPropertyChanged(nameof(UpdateAvailable)); }
        }

        /// <summary>
        /// Gets or sets a boolean value if the dependency is currently updating
        /// </summary>
        public bool IsUpdating
        {
            get { return _isUpdating; }
            set { if (value == _isUpdating) return; _isUpdating = value; OnPropertyChanged(nameof(IsUpdating)); }
        }

        /// <summary>
        /// Gets or sets the current dependency update progress as a percentage value
        /// </summary>
        public double ProgressPercentage
        {
            get { return _progressPercentage; }
            set { if (value == _progressPercentage) return; _progressPercentage = value; OnPropertyChanged(nameof(ProgressPercentage)); }
        }
    }
}
