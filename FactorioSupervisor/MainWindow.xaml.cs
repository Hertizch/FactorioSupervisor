using FactorioSupervisor.Extensions;
using FactorioSupervisor.Helpers;
using FactorioSupervisor.Models;
using FactorioSupervisor.ViewModels;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace FactorioSupervisor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INotifyPropertyChanged
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private string _searchFilter;

        /// <summary>
        /// 
        /// </summary>
        public string SearchFilter
        {
            get { return _searchFilter; }
            set
            {
                if (value == _searchFilter) return;

                _searchFilter = value;
                OnPropertyChanged();

                if (!string.IsNullOrEmpty(_searchFilter))
                    AddFilter();

                ((CollectionViewSource)Resources["ModsVs"])?.View.Refresh();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ModList_Filter(object sender, FilterEventArgs e)
        {
            var mod = e.Item as Mod;

            if (mod == null)
                e.Accepted = false;
            else if (!mod.Title.Contains(_searchFilter, StringComparison.OrdinalIgnoreCase) && !mod.Name.Contains(_searchFilter, StringComparison.OrdinalIgnoreCase) && !mod.HideInModList)
                e.Accepted = false;
        }

        /// <summary>
        /// 
        /// </summary>
        private void AddFilter()
        {
            ((CollectionViewSource)Resources["ModsVs"]).Filter -= ModList_Filter;
            ((CollectionViewSource)Resources["ModsVs"]).Filter += ModList_Filter;
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
            if (ItemsControl.ContainerFromElement(ModsListBox, e.OriginalSource as DependencyObject) is ListBoxItem item)
            {
                var mod = item.DataContext as Mod;

                if (mod.HasError)
                    return;

                if (mod.IsEnabled)
                    mod.IsEnabled = false;
                else
                    mod.IsEnabled = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ModsListBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (ItemsControl.ContainerFromElement(ModsListBox, e.OriginalSource as DependencyObject) is ListBoxItem item && e.Key == Key.Return)
            {
                var mod = item.DataContext as Mod;

                if (mod.HasError)
                    return;

                if (mod.IsEnabled)
                    mod.IsEnabled = false;
                else
                    mod.IsEnabled = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WindowRoot_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) | Keyboard.IsKeyDown(Key.RightCtrl) && e.Key == Key.F)
            {
                e.Handled = true;

                SearchTextBox.SelectAll();
                SearchTextBox.Focus();
            }
        }

        private async void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            UpdaterSnackbar.Title = "Test mod";

            for (int i = 0; i < 101; i++)
            {
                await Task.Delay(50);
                UpdaterSnackbar.ItemProgressPercentage = i;
            }

            for (int i = 0; i < 101; i++)
            {
                await Task.Delay(50);
                UpdaterSnackbar.TotalProgressPercentage = i;
            }
        }
    }
}
