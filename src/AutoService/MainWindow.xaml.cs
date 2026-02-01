using AutoService.ViewModels.Customers;
using System.Windows;

namespace AutoService
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public CustomersViewModel CustomersVm { get; } = new();

        public MainWindow()
        {
            InitializeComponent(); 
            DataContext = this;
        }
    }
}