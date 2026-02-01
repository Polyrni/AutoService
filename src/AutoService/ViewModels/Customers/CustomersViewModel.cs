using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using AutoService.Data;
using AutoService.Data.Models;
using AutoService.Infrastructure;
using AutoService.Views.Customers;
using Microsoft.EntityFrameworkCore;

namespace AutoService.ViewModels.Customers;

public class CustomersViewModel : INotifyPropertyChanged
{
    private readonly AppDbContext _db = Db.CreateContext();

    public ObservableCollection<Customer> Customers { get; } = new();

    private Customer? _selectedCustomer;

    public Customer? SelectedCustomer
    {
        get => _selectedCustomer;
        set
        {
            _selectedCustomer = value;
            OnPropertyChanged();
            ((RelayCommand)EditCommand).RaiseCanExecuteChanged();
        }
    }

    public ICommand AddCommand { get; }
    public ICommand EditCommand { get; }
    public ICommand RefreshCommand { get; }

    public CustomersViewModel()
    {
        AddCommand = new RelayCommand(Add);
        EditCommand = new RelayCommand(Edit, CanEdit);
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
        var dlg = new SaveCustomerDialog
        {
            Owner = App.Current.MainWindow
        };
        if (dlg.ShowDialog() != true) return;

        _db.Customers.Add(dlg.Customer);
        _db.SaveChanges();
        Refresh();
    }

    private bool CanEdit() => SelectedCustomer != null;

    private void Edit()
    {
        if (SelectedCustomer == null)
        {
            return;
        }

        var dlg = new SaveCustomerDialog(SelectedCustomer)
        {
            Owner = App.Current.MainWindow
        };

        if (dlg.ShowDialog() != true)
        {
            return;
        }

        var existing = _db.Customers.Find(dlg.Customer.Id);
        if (existing != null)
        {
            existing.FullName = dlg.Customer.FullName;
            existing.Phone = dlg.Customer.Phone;
            existing.Email = dlg.Customer.Email;
            existing.Note = dlg.Customer.Note;
            _db.SaveChanges();
        }

        Refresh();
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? n = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));
}
