using System.Windows;
using System.Windows.Media;

namespace FactorioSupervisor.Extensions
{
    public class ExtendedButton
    {
        public static readonly DependencyProperty IconProperty =
        DependencyProperty.RegisterAttached("Icon", typeof(PathGeometry), typeof(ExtendedButton), new PropertyMetadata(default(PathGeometry)));

        public static void SetIcon(UIElement element, PathGeometry value)
        {
            element.SetValue(IconProperty, value);
        }

        public static PathGeometry GetIcon(UIElement element)
        {
            return (PathGeometry)element.GetValue(IconProperty);
        }
    }
}
