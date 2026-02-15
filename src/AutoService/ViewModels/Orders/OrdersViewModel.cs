// AutoService.ViewModels.Orders/OrdersViewModel.cs
using AutoService.Data;
using AutoService.Data.Models.Orders;
using AutoService.Infrastructure;
using AutoService.Managers;
using AutoService.Views.Orders;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace AutoService.ViewModels.Orders;

public class OrdersViewModel : INotifyPropertyChanged
{
    private readonly AppDbContext _db;
    private readonly OrdersManager _ordersManager;

    public ObservableCollection<Order> Orders { get; } = new();

    private Order? _selectedOrder;
    public Order? SelectedOrder
    {
        get => _selectedOrder;
        set
        {
            _selectedOrder = value;
            OnPropertyChanged();
            ((RelayCommand)EditCommand).RaiseCanExecuteChanged();
            ((RelayCommand)DeleteCommand).RaiseCanExecuteChanged();
        }
    }

    public ICommand AddCommand { get; }
    public ICommand EditCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand RefreshCommand { get; }

    public OrdersViewModel()
    {
        _db = Db.CreateContext();
        _ordersManager = new OrdersManager(_db);

        AddCommand = new RelayCommand(Add);
        EditCommand = new RelayCommand(Edit, CanEdit);
        DeleteCommand = new RelayCommand(Delete, CanDelete);
        RefreshCommand = new RelayCommand(Refresh);

        Refresh();
    }

    private void Refresh()
    {
        try
        {
            var orders = _ordersManager.GetAllOrdersWithDetails();

            Orders.Clear();
            foreach (var o in orders)
            {
                Orders.Add(o);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при загрузке заказов: {ex.Message}", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void Add()
    {
        var dlg = new SaveOrderDialog(_db, _ordersManager)
        {
            Owner = App.Current.MainWindow
        };

        if (dlg.DataContext is SaveOrderViewModel vm)
        {
            vm.CloseDialog = (result) =>
            {
                if (result) Refresh();
                dlg.Close();
            };
        }

        dlg.ShowDialog();
    }

    private bool CanEdit() => SelectedOrder != null;

    private void Edit()
    {
        if (SelectedOrder == null) return;

        var dlg = new SaveOrderDialog(_db, _ordersManager, SelectedOrder)
        {
            Owner = App.Current.MainWindow
        };

        if (dlg.DataContext is SaveOrderViewModel vm)
        {
            vm.CloseDialog = (result) =>
            {
                if (result) Refresh();
                dlg.Close();
            };
        }

        dlg.ShowDialog();
    }

    private bool CanDelete() => SelectedOrder != null;

    private void Delete()
    {
        if (SelectedOrder == null) return;

        var result = MessageBox.Show(
            $"Удалить заказ №{SelectedOrder.OrderNumber}?",
            "Подтверждение",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (result != MessageBoxResult.Yes) return;

        try
        {
            var success = _ordersManager.DeleteOrder(SelectedOrder.Id);
            if (success)
            {
                Refresh();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при удалении заказа: {ex.Message}", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? n = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));
}