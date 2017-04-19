using System.Windows;
using System.Windows.Media;

namespace FactorioSupervisor.Resources.Controls
{
    /// <summary>
    /// Interaction logic for IconButton.xaml
    /// </summary>
    public partial class IconButton
    {
        public IconButton()
        {
            InitializeComponent();
        }

        public PathGeometry Icon
        {
            get { return (PathGeometry)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        public double ViewboxWidth
        {
            get { return (double)GetValue(ViewboxWidthProperty); }
            set { SetValue(ViewboxWidthProperty, value); }
        }

        public static readonly DependencyProperty IconProperty = DependencyProperty.Register(nameof(Icon), typeof(PathGeometry),
            typeof(IconButton), new PropertyMetadata(default(PathGeometry)));

        public static readonly DependencyProperty ViewboxWidthProperty = DependencyProperty.Register(nameof(ViewboxWidth), typeof(double),
            typeof(IconButton), new PropertyMetadata(default(double)));
    }
}
