using FactorioSupervisor.Extensions;
using FactorioSupervisor.Models;
using FactorioSupervisor.ViewModels;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
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

        void ModList_Filter(object sender, FilterEventArgs e)
        {
            var mod = e.Item as Mod;

            if (mod == null)
                e.Accepted = false;
            else if (!mod.Title.Contains(_searchFilter, StringComparison.OrdinalIgnoreCase) && !mod.Name.Contains(_searchFilter, StringComparison.OrdinalIgnoreCase) && !mod.HideInModList)
                e.Accepted = false;
        }

        private void AddFilter()
        {
            ((CollectionViewSource)Resources["ModsVs"]).Filter -= ModList_Filter;
            ((CollectionViewSource)Resources["ModsVs"]).Filter += ModList_Filter;
        }

        private void CloseCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            // Save user settings
            if (BaseVm.ConfigVm.SaveUserSettingsCmd.CanExecute(null))
                BaseVm.ConfigVm.SaveUserSettingsCmd.Execute(null);

            // Save selected profile
            if (BaseVm.ProfilesVm.SaveSelectedProfileCmd.CanExecute(null))
                BaseVm.ProfilesVm.SaveSelectedProfileCmd.Execute(null);

            Close();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

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
    }
}
