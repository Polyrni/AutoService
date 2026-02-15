using AutoService.ViewModels.Services;
using System.Windows.Controls;
using System.Windows.Input;

namespace AutoService.Views.Services
{
    /// <summary>
    /// Interaction logic for ServicesView.xaml
    /// </summary>
    public partial class ServicesView : UserControl
    {
        public ServicesView()
        {
            InitializeComponent();
        }

        private void Row_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is ServicesViewModel vm && vm.EditCommand.CanExecute(null))
            {
                vm.EditCommand.Execute(null);
            }
        }

        private void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }
    }
}
