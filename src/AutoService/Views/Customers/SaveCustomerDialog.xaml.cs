using AutoService.Data.Models;
using System.Windows;

namespace AutoService.Views.Customers;

public partial class SaveCustomerDialog : Window
{
    public Customer Customer { get; }

    public SaveCustomerDialog() : this(null) { }

    public SaveCustomerDialog(Customer? existingCustomer)
    {
        InitializeComponent();

        if (existingCustomer != null)
        {
            Customer = new Customer
            {
                Id = existingCustomer.Id,
                FullName = existingCustomer.FullName,
                Phone = existingCustomer.Phone,
                Email = existingCustomer.Email,
                Note = existingCustomer.Note
            };
            Title = "Редактировать клиента";
        }
        else
        {
            Customer = new Customer();
            Title = "Новый клиент";
        }

        DataContext = Customer;
    }

    private void Ok_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
    }
}