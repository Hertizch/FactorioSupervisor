using System.Windows;
using System.Windows.Media;

namespace FactorioSupervisor.Resources.Controls
{
    /// <summary>
    /// Interaction logic for IconToggleButton.xaml
    /// </summary>
    public partial class IconToggleButton
    {
        public IconToggleButton()
        {
            InitializeComponent();
        }

        public PathGeometry Icon
        {
            get { return (PathGeometry)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        public double MarginCustom
        {
            get { return (double)GetValue(MarginCustomProperty); }
            set { SetValue(MarginCustomProperty, value); }
        }

        public static readonly DependencyProperty IconProperty = DependencyProperty.Register(nameof(Icon), typeof(PathGeometry),
            typeof(IconToggleButton), new PropertyMetadata(default(PathGeometry)));

        public static readonly DependencyProperty MarginCustomProperty = DependencyProperty.Register(nameof(MarginCustom), typeof(double),
            typeof(IconToggleButton), new PropertyMetadata((double)8));
    }
}
