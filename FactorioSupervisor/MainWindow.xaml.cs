using FactorioSupervisor.Helpers;
using FactorioSupervisor.Models;
using FactorioSupervisor.ViewModels;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;

namespace FactorioSupervisor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INotifyPropertyChanged
    {
        public MainWindow()
        {
            // Determine rendering mode of ui elements
            if (PresentationSource.FromVisual(this) is HwndSource hwndSource)
                hwndSource.CompositionTarget.RenderMode = BaseVm.ConfigVm.UiDisableHardwareAcc ? RenderMode.SoftwareOnly : RenderMode.Default;

            InitializeComponent();

            // Prompt to input paths - open settings
            if (string.IsNullOrWhiteSpace(BaseVm.ConfigVm.FactorioPath) || string.IsNullOrWhiteSpace(BaseVm.ConfigVm.ModsPath))
                SettingsButton.IsChecked = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MinimizeButton_OnClick(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseButton_OnClick(object sender, RoutedEventArgs e)
        {
            // Save user settings
            if (BaseVm.ConfigVm.SaveUserSettingsCmd.CanExecute(null))
                BaseVm.ConfigVm.SaveUserSettingsCmd.Execute(null);

            // Save selected profile
            if (BaseVm.ProfilesVm.SaveSelectedProfileCmd.CanExecute(null))
                BaseVm.ProfilesVm.SaveSelectedProfileCmd.Execute(null);

            Logger.WriteLine("Goodbye!\n", true);

            Close();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListBox_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!(ItemsControl.ContainerFromElement(ModsListBox, e.OriginalSource as DependencyObject) is ListBoxItem item)) return;

            var mod = item.DataContext as Mod;
            if (mod == null) return;

            if (mod.HasError)
                return;

            mod.IsEnabled = !mod.IsEnabled;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ModsListBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!(ItemsControl.ContainerFromElement(ModsListBox, e.OriginalSource as DependencyObject) is ListBoxItem item) || e.Key != Key.Return) return;

            var mod = item.DataContext as Mod;
            if (mod == null) return;

            if (mod.HasError)
                return;

            mod.IsEnabled = !mod.IsEnabled;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WindowRoot_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!(Keyboard.IsKeyDown(Key.LeftCtrl) | Keyboard.IsKeyDown(Key.RightCtrl)) || e.Key != Key.F) return;

            e.Handled = true;

            SearchTextBox.SelectAll();
            SearchTextBox.Focus();
        }
    }
}
