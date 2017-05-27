using System.Windows;
using System.Windows.Controls;

namespace FactorioSupervisor.Resources.Controls
{
    /// <summary>
    /// Interaction logic for NotifyTicker.xaml
    /// </summary>
    public partial class NotifyTicker : UserControl
    {
        public NotifyTicker()
        {
            InitializeComponent();
        }

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(nameof(Title), typeof(string),
            typeof(NotifyTicker), new PropertyMetadata("Title"));
    }
}
