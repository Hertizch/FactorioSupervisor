using FactorioSupervisor.Extensions;
using System.Timers;

namespace FactorioSupervisor.Relays
{
    public class NotifyBannerRelay : ObservableObject
    {
        private bool _showNotifyBanner;
        private string _notifyBannerValue;
        private Timer _timer;
        private RelayCommand _closeNotifyBannerCmd;

        /// <summary>
        /// Gets or sets a boolean value whether the notification banner should be shown
        /// </summary>
        public bool ShowNotifyBanner
        {
            get => _showNotifyBanner; set { if (value == _showNotifyBanner) return; _showNotifyBanner = value; OnPropertyChanged(nameof(ShowNotifyBanner)); }
        }

        /// <summary>
        /// Gets or sets a value if the notification banner should display
        /// </summary>
        public string NotifyBannerValue
        {
            get => _notifyBannerValue; set { if (value == _notifyBannerValue) return; _notifyBannerValue = value; OnPropertyChanged(nameof(NotifyBannerValue)); }
        }

        public RelayCommand CloseNotifyBannerCmd => _closeNotifyBannerCmd ??
            (_closeNotifyBannerCmd = new RelayCommand(Execute_CloseNotifyBannerCmd, p => true));

        public void SetNotifyBanner(string value)
        {
            ShowNotifyBanner = true;
            NotifyBannerValue = value;

            // Close after 8 secs
            _timer = new Timer(8000);
            _timer.Elapsed += Timer_Elapsed;
            _timer.Enabled = true;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _timer.Dispose();

            if (ShowNotifyBanner)
                ShowNotifyBanner = false;
        }

        private void Execute_CloseNotifyBannerCmd(object obj)
        {
            ShowNotifyBanner = false;
        }
    }
}
