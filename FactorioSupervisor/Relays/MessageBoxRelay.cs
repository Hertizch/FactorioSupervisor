using FactorioSupervisor.Extensions;
using System.Linq;
using System.Windows;

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
        private bool _isAuthenticationDialog;
        private MessageBoxWindow _messageBoxWindow;
        private RelayCommand _showMessageBoxCmd;
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

        /// <summary>
        /// Gets or sets a value if the message box should display the authentication dialog
        /// </summary>
        public bool IsAuthenticationDialog
        {
            get { return _isAuthenticationDialog; }
            set { if (value == _isAuthenticationDialog) return; _isAuthenticationDialog = value; OnPropertyChanged(nameof(IsAuthenticationDialog)); }
        }

        public RelayCommand ShowMessageBoxCmd => _showMessageBoxCmd ??
            (_showMessageBoxCmd = new RelayCommand(Execute_ShowMessageBoxCmd, p => true));

        public RelayCommand CloseMessageBoxCmd => _closeMessageBoxCmd ??
            (_closeMessageBoxCmd = new RelayCommand(Execute_CloseMessageBoxCmd, p => true));

        public void SetMessageBox(string title, string value, bool isAuthenticationDialog = false)
        {
            ShowMessageBox = true;
            MessageBoxTitle = title;
            MessageBoxValue = value;
            IsAuthenticationDialog = isAuthenticationDialog;

            _messageBoxWindow = new MessageBoxWindow { Owner = Application.Current.MainWindow };
            _messageBoxWindow.ShowDialog();
        }

        private void Execute_ShowMessageBoxCmd(object obj)
        {
            _messageBoxWindow = new MessageBoxWindow { Owner = Application.Current.MainWindow };
            _messageBoxWindow.ShowDialog();
        }

        private void Execute_CloseMessageBoxCmd(object obj)
        {
            ShowMessageBox = false;
            _messageBoxWindow?.Close();
        }
    }
}
