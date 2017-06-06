using System;
using FactorioSupervisor.Extensions;

namespace FactorioSupervisor.Models
{
    public class Dependency : ObservableObject
    {
        private string _name;
        private bool _isOptional;
        private bool _isInstalled;
        private string _remoteFilename;
        private string _fullName;
        private string _downloadUrl;
        private bool _updateAvailable;
        private bool _executeUpdate;
        private bool _isUpdating;
        private double _progressPercentage;

        /// <summary>
        /// Gets or sets the name of the dependency
        /// </summary>
        public string Name
        {
            get => _name; set { if (value == _name) return; _name = value; OnPropertyChanged(nameof(Name)); }
        }

        /// <summary>
        /// Gets or sets a boolean value if the dependency is optional
        /// </summary>
        public bool IsOptional
        {
            get => _isOptional; set { if (value == _isOptional) return; _isOptional = value; OnPropertyChanged(nameof(IsOptional)); }
        }

        /// <summary>
        /// Gets or sets a boolean value if the dependency is installed
        /// </summary>
        public bool IsInstalled
        {
            get => _isInstalled; set { if (value == _isInstalled) return; _isInstalled = value; OnPropertyChanged(nameof(IsInstalled)); }
        }

        /// <summary>
        /// Gets or sets the filename of the latest release
        /// </summary>
        public string RemoteFilename
        {
            get => _remoteFilename; set { if (value == _remoteFilename) return; _remoteFilename = value; OnPropertyChanged(nameof(RemoteFilename)); }
        }

        /// <summary>
        /// Gets or sets the filename and path
        /// </summary>
        public string FullName
        {
            get => _fullName; set { if (value == _fullName) return; _fullName = value; OnPropertyChanged(nameof(FullName)); }
        }

        /// <summary>
        /// Gets or sets the latest version download url
        /// </summary>
        public string DownloadUrl
        {
            get => _downloadUrl; set { if (value == _downloadUrl) return; _downloadUrl = value; OnPropertyChanged(nameof(DownloadUrl)); }
        }

        /// <summary>
        /// Gets or sets a boolean value if the dependency has an available update
        /// </summary>
        public bool UpdateAvailable
        {
            get => _updateAvailable; set { if (value == _updateAvailable) return; _updateAvailable = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Gets or sets a boolean value if the dependency should update
        /// </summary>
        public bool ExecuteUpdate
        {
            get => _executeUpdate; set { if (value == _executeUpdate) return; _executeUpdate = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Gets or sets a boolean value if the dependency is currently updating
        /// </summary>
        public bool IsUpdating
        {
            get => _isUpdating; set { if (value == _isUpdating) return; _isUpdating = value; OnPropertyChanged(nameof(IsUpdating)); }
        }

        /// <summary>
        /// Gets or sets the current dependency update progress as a percentage value
        /// </summary>
        public double ProgressPercentage
        {
            get => _progressPercentage; set { if (Math.Abs(value - _progressPercentage) < 0.01) return; _progressPercentage = value; OnPropertyChanged(nameof(ProgressPercentage)); }
        }
    }
}
