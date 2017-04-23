using FactorioSupervisor.Extensions;

namespace FactorioSupervisor.Relays
{
    public class MessageBoxRelay : ObservableObject
    {
        public MessageBoxRelay()
        {
            //
        }

        private bool _showMessageBox;
        private string _messageBoxTitle;
        private string _messageBoxValue;
        private RelayCommand _closeMessageBoxCmd;

        /// <summary>
        /// Gets or sets a value if the message box should display
        /// </summary>
        public bool ShowMessageBox
        {
            get { return _showMessageBox; }
            set { if (value == _showMessageBox) return; _showMessageBox = value; OnPropertyChanged(nameof(ShowMessageBox)); }
        }

        /// <summary>
        /// Gets or sets the message box title
        /// </summary>
        public string MessageBoxTitle
        {
            get { return _messageBoxTitle; }
            set { if (value == _messageBoxTitle) return; _messageBoxTitle = value; OnPropertyChanged(nameof(MessageBoxTitle)); }
        }

        /// <summary>
        /// Gets or sets the message box value
        /// </summary>
        public string MessageBoxValue
        {
            get { return _messageBoxValue; }
            set { if (value == _messageBoxValue) return; _messageBoxValue = value; OnPropertyChanged(nameof(MessageBoxValue)); }
        }

        public RelayCommand CloseMessageBoxCmd => _closeMessageBoxCmd ??
            (_closeMessageBoxCmd = new RelayCommand(Execute_CloseMessageBoxCmd, p => true));

        public void SetMessageBox(string title, string value)
        {
            ShowMessageBox = true;
            MessageBoxTitle = title;
            MessageBoxValue = value;
        }

        private void Execute_CloseMessageBoxCmd(object obj)
        {
            ShowMessageBox = false;
        }
    }
}
