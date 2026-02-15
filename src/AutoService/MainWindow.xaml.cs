using AutoService.ViewModels.Customers;
using AutoService.ViewModels.Employees;
using System.Windows;
using AutoService.ViewModels.Orders;
using AutoService.ViewModels.Services;

namespace AutoService
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public CustomersViewModel CustomersVm { get; } = new();

        public EmployeesViewModel EmployeesVm { get; } = new();

        public ServicesViewModel ServicesVm { get; } = new();

        public OrdersViewModel OrdersVm { get; } = new();

        public MainWindow()
        {
            InitializeComponent(); 
            DataContext = this;
        }
    }
}