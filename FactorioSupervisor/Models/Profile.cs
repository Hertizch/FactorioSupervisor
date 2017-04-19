using FactorioSupervisor.Extensions;
using System.Collections.Generic;

namespace FactorioSupervisor.Models
{
    public class Profile : ObservableObject
    {
        private string _name;
        private string _filename;
        private IEnumerable<string> _enabledModNames;

        /// <summary>
        /// Gets or sets the profile name
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { if (value == _name) return; _name = value; OnPropertyChanged(nameof(Name)); }
        }

        /// <summary>
        /// Gets or sets the profile filename
        /// </summary>
        public string Filename
        {
            get { return _filename; }
            set { if (value == _filename) return; _filename = value; OnPropertyChanged(nameof(Filename)); }
        }

        /// <summary>
        /// Gets or sets the mod names associated to this profile
        /// </summary>
        public IEnumerable<string> EnabledModNames
        {
            get { return _enabledModNames; }
            set { if (value == _enabledModNames) return; _enabledModNames = value; OnPropertyChanged(nameof(EnabledModNames)); }
        }
    }
}
