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

        public double MarginCustom
        {
            get { return (double)GetValue(MarginCustomProperty); }
            set { SetValue(MarginCustomProperty, value); }
        }

        public static readonly DependencyProperty IconProperty = DependencyProperty.Register(nameof(Icon), typeof(PathGeometry),
            typeof(IconButton), new PropertyMetadata(default(PathGeometry)));

        public static readonly DependencyProperty MarginCustomProperty = DependencyProperty.Register(nameof(MarginCustom), typeof(double),
            typeof(IconButton), new PropertyMetadata((double)8));
    }
}
