using AutoService.ViewModels.Customers;
using AutoService.ViewModels.Employees;
using System.Windows;

namespace AutoService
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public CustomersViewModel CustomersVm { get; } = new();

        public EmployeesViewModel EmployeesVm { get; } = new();

        public MainWindow()
        {
            InitializeComponent(); 
            DataContext = this;
        }
    }
}