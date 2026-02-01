using AutoService.Data.Models;
using System.Windows;

namespace AutoService.Views.Customers;

public partial class AddCustomerDialog : Window
{
    public Customer Customer { get; }

    public AddCustomerDialog()
    {
        InitializeComponent();
        Customer = new Customer();
        DataContext = Customer;
    }

    private void Ok_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
    }
}