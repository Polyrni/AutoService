using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using AutoService.Data;
using AutoService.Data.Models;
using AutoService.Infrastructure;
using AutoService.Views.Services;
using Microsoft.EntityFrameworkCore;

namespace AutoService.ViewModels.Services;

public class ServicesViewModel : INotifyPropertyChanged
{
    private readonly AppDbContext _db = Db.CreateContext();

    public ObservableCollection<Service> Services { get; } = new();

    private Service? _selectedService;

    public Service? SelectedService
    {
        get => _selectedService;
        set
        {
            _selectedService = value;
            OnPropertyChanged();
            ((RelayCommand)EditCommand).RaiseCanExecuteChanged();
            ((RelayCommand)DeleteCommand).RaiseCanExecuteChanged();
        }
    }

    public ICommand AddCommand { get; }
    public ICommand EditCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand RefreshCommand { get; }

    public ServicesViewModel()
    {
        AddCommand = new RelayCommand(Add);
        EditCommand = new RelayCommand(Edit, CanEdit);
        DeleteCommand = new RelayCommand(Delete, CanDelete);
        RefreshCommand = new RelayCommand(Refresh);
        Refresh();
    }

    private void Refresh()
    {
        Services.Clear();
        foreach (var c in _db.Services.AsNoTracking().OrderBy(x => x.Id))
        {
            Services.Add(c);
        }
    }

    private void Add()
    {
        var dlg = new SaveServiceDialog
        {
            Owner = App.Current.MainWindow
        };
        if (dlg.ShowDialog() != true) return;

        _db.Services.Add(dlg.Service);
        _db.SaveChanges();
        Refresh();
    }

    private bool CanEdit() => SelectedService != null;

    private void Edit()
    {
        if (SelectedService == null)
        {
            return;
        }

        var dlg = new SaveServiceDialog(SelectedService)
        {
            Owner = App.Current.MainWindow
        };

        if (dlg.ShowDialog() != true)
        {
            return;
        }

        var existing = _db.Services.Find(dlg.Service.Id);
        if (existing != null)
        {
            existing.Name = dlg.Service.Name;
            _db.SaveChanges();
        }

        Refresh();
    }

    private bool CanDelete() => SelectedService != null;

    private void Delete()
    {
        if (SelectedService == null) return;

        var result = MessageBox.Show(
            $"Удалить услугу \"{SelectedService.Name}\"?",
            "Подтверждение",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (result != MessageBoxResult.Yes)
        {
            return;
        }

        var existing = _db.Services.Find(SelectedService.Id);
        if (existing != null)
        {
            _db.Services.Remove(existing);
            _db.SaveChanges();
        }

        Refresh();
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? n = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));
}
