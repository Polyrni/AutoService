using AutoService.Data;
using AutoService.Data.Models;
using AutoService.Infrastructure;
using AutoService.Views.Employees;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace AutoService.ViewModels.Employees;

public class EmployeesViewModel : INotifyPropertyChanged
{
    private readonly AppDbContext _db = Db.CreateContext();

    public ObservableCollection<Employe> Employees { get; } = new();

    private Employe? _selectedEmploye;

    public Employe? SelectedEmploye
    {
        get => _selectedEmploye;
        set
        {
            _selectedEmploye = value;
            OnPropertyChanged();
            ((RelayCommand)EditCommand).RaiseCanExecuteChanged();
            ((RelayCommand)DeleteCommand).RaiseCanExecuteChanged();
        }
    }

    public ICommand AddCommand { get; }
    public ICommand EditCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand RefreshCommand { get; }

    public EmployeesViewModel()
    {
        AddCommand = new RelayCommand(Add);
        EditCommand = new RelayCommand(Edit, CanEdit);
        DeleteCommand = new RelayCommand(Delete, CanDelete);
        RefreshCommand = new RelayCommand(Refresh);
        Refresh();
    }

    private void Refresh()
    {
        Employees.Clear();
        foreach (var c in _db.Employees.AsNoTracking().OrderBy(x => x.FullName))
        {
            Employees.Add(c);
        }
    }

    private void Add()
    {
        var dlg = new SaveEmployeDialog
        {
            Owner = App.Current.MainWindow
        };
        if (dlg.ShowDialog() != true) return;

        _db.Employees.Add(dlg.Employe);
        _db.SaveChanges();
        Refresh();
    }

    private bool CanEdit() => SelectedEmploye != null;

    private void Edit()
    {
        if (SelectedEmploye == null)
        {
            return;
        }

        var dlg = new SaveEmployeDialog(SelectedEmploye)
        {
            Owner = App.Current.MainWindow
        };

        if (dlg.ShowDialog() != true)
        {
            return;
        }

        var existing = _db.Employees.Find(dlg.Employe.Id);
        if (existing != null)
        {
            existing.FullName = dlg.Employe.FullName;
            existing.Phone = dlg.Employe.Phone;
            existing.Email = dlg.Employe.Email;
            existing.Note = dlg.Employe.Note;
            _db.SaveChanges();
        }

        Refresh();
    }

    private bool CanDelete() => SelectedEmploye != null;

    private void Delete()
    {
        if (SelectedEmploye == null) return;

        var result = MessageBox.Show(
            $"Удалить клиента \"{SelectedEmploye.FullName}\"?",
            "Подтверждение",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (result != MessageBoxResult.Yes)
        {
            return;
        }

        var existing = _db.Employees.Find(SelectedEmploye.Id);
        if (existing != null)
        {
            _db.Employees.Remove(existing);
            _db.SaveChanges();
        }

        Refresh();
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? n = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));
}
