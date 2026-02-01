using System.Collections.ObjectModel;
using System.Windows.Input;
using AutoService.Data;
using AutoService.Data.Models;
using AutoService.Infrastructure;
using AutoService.Views.Customers;
using Microsoft.EntityFrameworkCore;

namespace AutoService.ViewModels.Customers;

public class CustomersViewModel
{
    private readonly AppDbContext _db = Db.CreateContext();

    public ObservableCollection<Customer> Customers { get; } = new();

    public ICommand AddCommand { get; }
    public ICommand RefreshCommand { get; }

    public CustomersViewModel()
    {
        AddCommand = new RelayCommand(Add);
        RefreshCommand = new RelayCommand(Refresh);
        Refresh();
    }

    private void Refresh()
    {
        Customers.Clear();
        foreach (var c in _db.Customers.AsNoTracking().OrderBy(x => x.FullName))
        {
            Customers.Add(c);
        }
    }

    private void Add()
    {
        var dlg = new AddCustomerDialog();
        if (dlg.ShowDialog() != true)
        {
            return;
        }

        _db.Customers.Add(dlg.Customer);
        _db.SaveChanges();

        Refresh();
    }
}