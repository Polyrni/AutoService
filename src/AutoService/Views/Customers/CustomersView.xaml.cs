using AutoService.ViewModels.Customers;
using System.Windows.Controls;
using System.Windows.Input;

namespace AutoService.Views.Customers
{
    /// <summary>
    /// Interaction logic for CustomersView.xaml
    /// </summary>
    public partial class CustomersView : UserControl
    {
        public CustomersView()
        {
            InitializeComponent();
        }

        private void Row_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is CustomersViewModel vm && vm.EditCommand.CanExecute(null))
            {
                vm.EditCommand.Execute(null);
            }
        }
    }
}
