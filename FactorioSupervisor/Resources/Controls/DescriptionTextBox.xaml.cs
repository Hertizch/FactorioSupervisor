using System.Windows;
using System.Windows.Controls;

namespace FactorioSupervisor.Resources.Controls
{
    /// <summary>
    /// Interaction logic for DescriptionTextBox.xaml
    /// </summary>
    public partial class DescriptionTextBox : UserControl
    {
        public DescriptionTextBox()
        {
            InitializeComponent();
        }

        public string DescriptionText
        {
            get { return (string)GetValue(DescriptionTextProperty); }
            set { SetValue(DescriptionTextProperty, value); }
        }

        public string ErrorText
        {
            get { return (string)GetValue(ErrorTextProperty); }
            set { SetValue(ErrorTextProperty, value); }
        }

        public bool HasError
        {
            get { return (bool)GetValue(HasErrorProperty); }
            set { SetValue(HasErrorProperty, value); }
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public bool IsPasswordBox
        {
            get { return (bool)GetValue(IsPasswordBoxProperty); }
            set { SetValue(IsPasswordBoxProperty, value); }
        }

        public static readonly DependencyProperty DescriptionTextProperty = DependencyProperty.Register(nameof(DescriptionText), typeof(string),
            typeof(DescriptionTextBox), new PropertyMetadata(default(string)));

        public static readonly DependencyProperty ErrorTextProperty = DependencyProperty.Register(nameof(ErrorText), typeof(string),
            typeof(DescriptionTextBox), new PropertyMetadata(default(string)));

        public static readonly DependencyProperty HasErrorProperty = DependencyProperty.Register(nameof(HasError), typeof(bool),
            typeof(DescriptionTextBox), new PropertyMetadata(default(bool)));

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(nameof(Text), typeof(string),
            typeof(DescriptionTextBox), new PropertyMetadata(default(string)));

        public static readonly DependencyProperty IsPasswordBoxProperty = DependencyProperty.Register(nameof(IsPasswordBox), typeof(bool),
            typeof(DescriptionTextBox), new PropertyMetadata(default(bool)));
    }
}
