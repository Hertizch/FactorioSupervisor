using System.Windows;
using System.Windows.Controls;

namespace FactorioSupervisor.Resources.Controls
{
    /// <summary>
    /// Interaction logic for Snackbar.xaml
    /// </summary>
    public partial class Snackbar : UserControl
    {
        public Snackbar()
        {
            InitializeComponent();
        }

        public bool IsOpen
        {
            get => (bool)GetValue(IsOpenProperty);
            set => SetValue(IsOpenProperty, value);
        }

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public double ItemProgressPercentage
        {
            get => (double)GetValue(ItemProgressPercentageProperty);
            set => SetValue(ItemProgressPercentageProperty, value);
        }

        public double TotalProgressPercentage
        {
            get => (double)GetValue(TotalProgressPercentageProperty);
            set => SetValue(TotalProgressPercentageProperty, value);
        }

        public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register(nameof(IsOpen), typeof(bool),
            typeof(Snackbar), new PropertyMetadata(default(bool)));

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(nameof(Title), typeof(string),
            typeof(Snackbar), new PropertyMetadata(default(string)));

        public static readonly DependencyProperty ItemProgressPercentageProperty = DependencyProperty.Register(nameof(ItemProgressPercentage), typeof(double),
            typeof(Snackbar), new PropertyMetadata((double)0));

        public static readonly DependencyProperty TotalProgressPercentageProperty = DependencyProperty.Register(nameof(TotalProgressPercentage), typeof(double),
            typeof(Snackbar), new PropertyMetadata((double)0));
    }
}
