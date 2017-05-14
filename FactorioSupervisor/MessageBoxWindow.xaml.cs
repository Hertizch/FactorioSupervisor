using System.Windows;

namespace FactorioSupervisor
{
    /// <summary>
    /// Interaction logic for MessageBoxWindow.xaml
    /// </summary>
    public partial class MessageBoxWindow : Window
    {
        public MessageBoxWindow()
        {
            InitializeComponent();

            SetControlVisibility();
        }

        private static MessageBoxWindow _messageBoxWindow;
        private static MessageBoxResult _result = MessageBoxResult.No;
        private static bool _isAuthenticationDialog;

        public static MessageBoxResult Show(string title, string value, MessageBoxButton button, bool isAuthenticationDialog = false)
        {
            if (isAuthenticationDialog)
                _isAuthenticationDialog = true;
            else
                _isAuthenticationDialog = false;

            _messageBoxWindow = new MessageBoxWindow
            {
                TitleTextBlock = { Text = title },
                ValueTextBlock = { Text = value },
                Owner = Application.Current.MainWindow
            };

            _messageBoxWindow.ShowDialog();

            return _result;
        }

        private void SetControlVisibility()
        {
            if (_isAuthenticationDialog)
            {
                AuthInputBoxes.Visibility = Visibility.Visible;
            }
            else if (!_isAuthenticationDialog)
            {
                AuthInputBoxes.Visibility = Visibility.Collapsed;
                CancelButton.Visibility = Visibility.Collapsed;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender == OkButton)
                _result = MessageBoxResult.OK;
            else if (sender == CancelButton)
                _result = MessageBoxResult.Cancel;
            else if (sender == TitleBarCloseButton)
                _result = MessageBoxResult.Cancel;
            else
                _result = MessageBoxResult.None;

            _messageBoxWindow?.Close();
        }
    }
}
