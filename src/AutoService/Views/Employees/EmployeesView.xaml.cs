using AutoService.ViewModels.Customers;
using AutoService.ViewModels.Employees;
using System.Windows.Controls;
using System.Windows.Input;

namespace AutoService.Views.Employees
{
    /// <summary>
    /// Interaction logic for EmployeesView.xaml
    /// </summary>
    public partial class EmployeesView : UserControl
    {
        public EmployeesView()
        {
            InitializeComponent();
        }

        private void Row_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is EmployeesViewModel vm && vm.EditCommand.CanExecute(null))
            {
                vm.EditCommand.Execute(null);
            }
        }
    }
}
