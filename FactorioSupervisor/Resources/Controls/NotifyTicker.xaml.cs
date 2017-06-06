using System.Collections.Generic;
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
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public List<string> Titles
        {
            get => (List<string>)GetValue(TitlesProperty);
            set => SetValue(TitlesProperty, value);
        }

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(nameof(Title), typeof(string),
            typeof(NotifyTicker), new PropertyMetadata("Title"));

        public static readonly DependencyProperty TitlesProperty = DependencyProperty.Register(nameof(Title), typeof(List<string>),
            typeof(NotifyTicker), new PropertyMetadata(default(List<string>)));
    }
}
