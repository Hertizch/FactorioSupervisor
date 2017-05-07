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
    }
}
