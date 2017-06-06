using FactorioSupervisor.Extensions;
using Newtonsoft.Json;
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
        [JsonProperty(PropertyName = "name")]
        public string Name
        {
            get => _name; set { if (value == _name) return; _name = value; OnPropertyChanged(nameof(Name)); }
        }

        /// <summary>
        /// Gets or sets the profile filename
        /// </summary>
        [JsonProperty(PropertyName = "filename")]
        public string Filename
        {
            get => _filename; set { if (value == _filename) return; _filename = value; OnPropertyChanged(nameof(Filename)); }
        }

        /// <summary>
        /// Gets or sets the mod names associated to this profile
        /// </summary>
        [JsonProperty(PropertyName = "enabled_mod_names")]
        public IEnumerable<string> EnabledModNames
        {
            get => _enabledModNames; set { if (value == _enabledModNames) return; _enabledModNames = value; OnPropertyChanged(nameof(EnabledModNames)); }
        }
    }
}
