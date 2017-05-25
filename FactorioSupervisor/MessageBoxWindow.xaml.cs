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
        private static MessageBoxButton _messageBoxButton;

        public static MessageBoxResult Show(string title, string value, MessageBoxButton button, bool isAuthenticationDialog = false)
        {
            _messageBoxButton = button;
            _isAuthenticationDialog = isAuthenticationDialog;

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
            // Set all to collapsed
            OkButton.Visibility = Visibility.Collapsed;
            CancelButton.Visibility = Visibility.Collapsed;
            YesButton.Visibility = Visibility.Collapsed;
            NoButton.Visibility = Visibility.Collapsed;
            AuthInputBoxes.Visibility = Visibility.Collapsed;

            if (_isAuthenticationDialog)
                AuthInputBoxes.Visibility = Visibility.Visible;

            if (_messageBoxButton == MessageBoxButton.OKCancel)
            {
                OkButton.Visibility = Visibility.Visible;
                CancelButton.Visibility = Visibility.Visible;
            }
            else if (_messageBoxButton == MessageBoxButton.YesNo)
            {
                YesButton.Visibility = Visibility.Visible;
                NoButton.Visibility = Visibility.Visible;
            }
            else if (_messageBoxButton == MessageBoxButton.OK)
                OkButton.Visibility = Visibility.Visible;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender == OkButton)
                _result = MessageBoxResult.OK;
            else if (sender == CancelButton)
                _result = MessageBoxResult.Cancel;
            else if (sender == TitleBarCloseButton)
                _result = MessageBoxResult.Cancel;
            else if (sender == YesButton)
                _result = MessageBoxResult.OK;
            else if (sender == NoButton)
                _result = MessageBoxResult.Cancel;
            else
                _result = MessageBoxResult.None;

            _messageBoxWindow?.Close();
        }
    }
}
