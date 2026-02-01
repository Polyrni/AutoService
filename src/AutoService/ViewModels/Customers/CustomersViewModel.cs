using AutoService.Data;
using AutoService.Data.Models;
using AutoService.Infrastructure;
using AutoService.Views.Customers;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

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
            ((RelayCommand)DeleteCommand).RaiseCanExecuteChanged();
        }
    }

    public ICommand AddCommand { get; }
    public ICommand EditCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand RefreshCommand { get; }

    public CustomersViewModel()
    {
        AddCommand = new RelayCommand(Add);
        EditCommand = new RelayCommand(Edit, CanEdit);
        DeleteCommand = new RelayCommand(Delete, CanDelete);
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

    private bool CanDelete() => SelectedCustomer != null;

    private void Delete()
    {
        if (SelectedCustomer == null) return;

        var result = MessageBox.Show(
            $"Удалить клиента \"{SelectedCustomer.FullName}\"?",
            "Подтверждение",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (result != MessageBoxResult.Yes)
        {
            return;
        }

        var existing = _db.Customers.Find(SelectedCustomer.Id);
        if (existing != null)
        {
            _db.Customers.Remove(existing);
            _db.SaveChanges();
        }

        Refresh();
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? n = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));
}
