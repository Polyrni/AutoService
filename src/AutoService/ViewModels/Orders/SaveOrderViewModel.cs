using AutoService.Data;
using AutoService.Data.Models;
using AutoService.Data.Models.Orders;
using AutoService.Infrastructure;
using AutoService.Managers;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace AutoService.ViewModels.Orders;

public class SaveOrderViewModel : INotifyPropertyChanged
{
    private readonly AppDbContext _db;
    private readonly OrdersManager _ordersManager;
    private Order? _editingOrder;

    public SaveOrderViewModel(AppDbContext db, OrdersManager ordersManager, Order? editingOrder = null)
    {
        _db = db;
        _ordersManager = ordersManager;
        _editingOrder = editingOrder;

        AddServiceCommand = new RelayCommand(AddService, CanAddService);
        RemoveServiceCommand = new RelayCommand(RemoveService, CanRemoveService);
        AddEmployeeCommand = new RelayCommand(AddEmployee, CanAddEmployee);
        RemoveEmployeeCommand = new RelayCommand(RemoveEmployee, CanRemoveEmployee);
        SaveCommand = new RelayCommand(Save, CanSave);
        CancelCommand = new RelayCommand(Cancel);

        SelectedServices.CollectionChanged += OnSelectedServicesChanged;

        LoadData();

        if (editingOrder != null)
        {
            LoadOrderForEdit(editingOrder);
        }
        else
        {
            OrderNumber = _ordersManager.GenerateOrderNumber();
            CreatedAt = DateTime.Now;
        }
    }

    // Свойства
    private string _orderNumber = string.Empty;
    public string OrderNumber
    {
        get => _orderNumber;
        set { _orderNumber = value; OnPropertyChanged(); }
    }

    private Customer? _selectedCustomer;
    public Customer? SelectedCustomer
    {
        get => _selectedCustomer;
        set
        {
            _selectedCustomer = value;
            OnPropertyChanged();
            ((RelayCommand)SaveCommand).RaiseCanExecuteChanged();

            // После выбора показываем ФИО в поле фильтра
            if (value != null)
            {
                _customerFilter = $"{value.LastName} {value.FirstName} {value.MiddleName}".Trim();
                OnPropertyChanged(nameof(CustomerFilter));
                // НЕ вызываем CustomersView.Refresh() — чтобы не перефильтровывать
            }
        }
    }

    private DateTime _createdAt;
    public DateTime CreatedAt
    {
        get => _createdAt;
        set { _createdAt = value; OnPropertyChanged(); }
    }

    private string? _note;
    public string? Note
    {
        get => _note;
        set { _note = value; OnPropertyChanged(); }
    }

    private decimal _totalAmount;
    public decimal TotalAmount
    {
        get => _totalAmount;
        set { _totalAmount = value; OnPropertyChanged(); }
    }

    // Коллекции
    public ObservableCollection<Customer> Customers { get; } = new();
    public ObservableCollection<Service> AvailableServices { get; } = new();
    public ObservableCollection<Employe> AvailableEmployees { get; } = new();
    public ObservableCollection<OrderServiceItemViewModel> SelectedServices { get; } = new();
    public ObservableCollection<Employe> SelectedEmployees { get; } = new();

    // Команды
    public ICommand AddServiceCommand { get; }
    public ICommand RemoveServiceCommand { get; }
    public ICommand AddEmployeeCommand { get; }
    public ICommand RemoveEmployeeCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }

    public Action<bool>? CloseDialog { get; set; }

    #region Filters definition  

    // Фильтр клиентов
    private string _customerFilter = string.Empty;
    public string CustomerFilter
    {
        get => _customerFilter;
        set
        {
            if (_customerFilter == value) return;
            _customerFilter = value;
            OnPropertyChanged();
            CustomersView.Refresh(); // вызывается только при ручном вводе
        }
    }

    public ICollectionView CustomersView { get; private set; } = null!;

    // Фильтр сотрудников
    private string _employeeFilter = string.Empty;
    public string EmployeeFilter
    {
        get => _employeeFilter;
        set
        {
            if (_employeeFilter == value) return;
            _employeeFilter = value;
            OnPropertyChanged();
            EmployeesView.Refresh();
        }
    }
    public ICollectionView EmployeesView { get; private set; } = null!;

    // Фильтр услуг
    private string _serviceFilter = string.Empty;
    public string ServiceFilter
    {
        get => _serviceFilter;
        set
        {
            if (_serviceFilter == value) return;
            _serviceFilter = value;
            OnPropertyChanged();
            ServicesView.Refresh();
        }
    }
    public ICollectionView ServicesView { get; private set; } = null!;

    #endregion

    private void OnSelectedServicesChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        ((RelayCommand)SaveCommand).RaiseCanExecuteChanged();

        if (e.NewItems != null)
        {
            foreach (OrderServiceItemViewModel item in e.NewItems)
            {
                item.PropertyChanged += OnOrderServiceItemPropertyChanged;
            }
        }

        if (e.OldItems != null)
        {
            foreach (OrderServiceItemViewModel item in e.OldItems)
            {
                item.PropertyChanged -= OnOrderServiceItemPropertyChanged;
            }
        }

        RecalculateTotal();
    }

    private void OnOrderServiceItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(OrderServiceItemViewModel.Price))
        {
            RecalculateTotal();
        }
    }

    private void LoadData()
    {
        // Клиенты
        Customers.Clear();
        foreach (var c in _db.Customers
                     .OrderBy(x => x.LastName)
                     .ThenBy(x => x.FirstName)
                     .ToList())
        {
            Customers.Add(c);
        }
        CustomersView = CollectionViewSource.GetDefaultView(Customers);
        CustomersView.Filter = o =>
        {
            if (o is not Customer c) return false;
            if (string.IsNullOrWhiteSpace(CustomerFilter)) return true;
            var text = CustomerFilter.Trim();
            return (c.LastName?.Contains(text, StringComparison.OrdinalIgnoreCase) ?? false)
                || (c.FirstName?.Contains(text, StringComparison.OrdinalIgnoreCase) ?? false)
                || (c.MiddleName?.Contains(text, StringComparison.OrdinalIgnoreCase) ?? false);
        };
        OnPropertyChanged(nameof(CustomersView));

        // Услуги — сначала заполняем, потом View
        AvailableServices.Clear();
        foreach (var s in _db.Services
                     .OrderBy(x => x.Specialization)
                     .ThenBy(x => x.Name)
                     .ToList())
        {
            AvailableServices.Add(s);
        }
        ServicesView = CollectionViewSource.GetDefaultView(AvailableServices);
        ServicesView.Filter = o =>
        {
            if (o is not Service s) return false;
            if (string.IsNullOrWhiteSpace(ServiceFilter)) return true;
            var text = ServiceFilter.Trim();
            return (s.Name?.Contains(text, StringComparison.OrdinalIgnoreCase) ?? false)
                || (s.Specialization?.Contains(text, StringComparison.OrdinalIgnoreCase) ?? false);
        };
        OnPropertyChanged(nameof(ServicesView));

        // Сотрудники — сначала заполняем, потом View
        AvailableEmployees.Clear();
        foreach (var e in _db.Employees
                     .OrderBy(x => x.LastName)
                     .ThenBy(x => x.FirstName)
                     .ToList())
        {
            AvailableEmployees.Add(e);
        }
        EmployeesView = CollectionViewSource.GetDefaultView(AvailableEmployees);
        EmployeesView.Filter = o =>
        {
            if (o is not Employe e) return false;
            if (string.IsNullOrWhiteSpace(EmployeeFilter)) return true;
            var text = EmployeeFilter.Trim();
            return (e.LastName?.Contains(text, StringComparison.OrdinalIgnoreCase) ?? false)
                || (e.FirstName?.Contains(text, StringComparison.OrdinalIgnoreCase) ?? false)
                || (e.MiddleName?.Contains(text, StringComparison.OrdinalIgnoreCase) ?? false);
        };
        OnPropertyChanged(nameof(EmployeesView));
    }

    private void LoadOrderForEdit(Order order)
    {
        OrderNumber = order.OrderNumber;
        //SelectedCustomer = order.Customer;
        SelectedCustomer = Customers.FirstOrDefault(c => c.Id == order.CustomerId);
        CreatedAt = order.CreatedAt;
        Note = order.Note;
        TotalAmount = order.TotalAmount;

        // Услуги
        var orderServices = _db.OrderServices
            .Include(os => os.Service)
            .Where(os => os.OrderId == order.Id)
            .ToList();

        foreach (var os in orderServices)
        {
            SelectedServices.Add(new OrderServiceItemViewModel(os));
        }

        // Сотрудники
        var orderEmployees = _db.OrderEmployees
            .Include(oe => oe.Employee)
            .Where(oe => oe.OrderId == order.Id)
            .Select(oe => oe.Employee)
            .ToList();

        foreach (var e in orderEmployees)
        {
            SelectedEmployees.Add(e);
        }
    }

    private void AddService()
    {
        if (_selectedService != null)
        {
            if (SelectedServices.Any(s => s.ServiceId == _selectedService.Id))
            {
                MessageBox.Show("Эта услуга уже добавлена");
                return;
            }

            var price = _ordersManager.GetServicePrice(_selectedService.Id);

            var orderService = new OrderService
            {
                Service = _selectedService,
                ServiceId = _selectedService.Id,
                Price = price
            };

            SelectedServices.Add(new OrderServiceItemViewModel(orderService));

            _selectedService = null;
            OnPropertyChanged(nameof(SelectedService));
            ((RelayCommand)AddServiceCommand).RaiseCanExecuteChanged();
        }
    }

    private bool CanAddService() => _selectedService != null;

    private void RemoveService()
    {
        if (_selectedOrderService != null)
        {
            SelectedServices.Remove(_selectedOrderService);
            _selectedOrderService = null;
            OnPropertyChanged(nameof(SelectedOrderService));
            ((RelayCommand)RemoveServiceCommand).RaiseCanExecuteChanged();
        }
    }

    private bool CanRemoveService() => _selectedOrderService != null;

    private void AddEmployee()
    {
        if (_selectedEmployee != null)
        {
            if (SelectedEmployees.Any(e => e.Id == _selectedEmployee.Id))
            {
                MessageBox.Show("Этот сотрудник уже добавлен");
                return;
            }

            SelectedEmployees.Add(_selectedEmployee);

            _selectedEmployee = null;
            OnPropertyChanged(nameof(SelectedEmployee));
            ((RelayCommand)AddEmployeeCommand).RaiseCanExecuteChanged();
        }
    }

    private bool CanAddEmployee() => _selectedEmployee != null;

    private void RemoveEmployee()
    {
        if (_selectedEmployeeForRemove != null)
        {
            SelectedEmployees.Remove(_selectedEmployeeForRemove);
            _selectedEmployeeForRemove = null;
            OnPropertyChanged(nameof(SelectedEmployeeForRemove));
            ((RelayCommand)RemoveEmployeeCommand).RaiseCanExecuteChanged();
        }
    }

    private bool CanRemoveEmployee() => _selectedEmployeeForRemove != null;

    private bool CanSave() => SelectedCustomer != null && SelectedServices.Any();

    private void Save()
    {
        if (!CanSave()) return;

        try
        {
            var order = new Order
            {
                Id = _editingOrder?.Id ?? 0,
                OrderNumber = OrderNumber,
                CustomerId = SelectedCustomer!.Id,
                CreatedAt = CreatedAt,
                UpdatedAt = DateTime.Now,
                TotalAmount = TotalAmount,
                Note = Note
            };

            var services = SelectedServices.Select(s => new OrderService
            {
                Id = s.Id,
                ServiceId = s.ServiceId,
                Price = s.Price
            }).ToList();

            var employeeIds = SelectedEmployees.Select(e => e.Id).ToList();

            if (_editingOrder == null)
            {
                _ordersManager.CreateOrder(order, services, employeeIds);
            }
            else
            {
                _ordersManager.UpdateOrder(order, services, employeeIds);
            }

            CloseDialog?.Invoke(true);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void Cancel() => CloseDialog?.Invoke(false);

    private void RecalculateTotal() => TotalAmount = SelectedServices.Sum(s => s.Price);

    // Свойства для выбранных элементов
    private Service? _selectedService;
    public Service? SelectedService
    {
        get => _selectedService;
        set
        {
            _selectedService = value;
            OnPropertyChanged();
            ((RelayCommand)AddServiceCommand).RaiseCanExecuteChanged();

            if (value != null)
            {
                _serviceFilter = value.ToString()!;
                OnPropertyChanged(nameof(ServiceFilter));
            }
        }
    }

    private OrderServiceItemViewModel? _selectedOrderService;
    public OrderServiceItemViewModel? SelectedOrderService
    {
        get => _selectedOrderService;
        set
        {
            _selectedOrderService = value;
            OnPropertyChanged();
            ((RelayCommand)RemoveServiceCommand).RaiseCanExecuteChanged();
        }
    }

    private Employe? _selectedEmployee;
    public Employe? SelectedEmployee
    {
        get => _selectedEmployee;
        set
        {
            _selectedEmployee = value;
            OnPropertyChanged();
            ((RelayCommand)AddEmployeeCommand).RaiseCanExecuteChanged();

            if (value != null)
            {
                _employeeFilter = value.ToString()!;
                OnPropertyChanged(nameof(EmployeeFilter));
            }
        }
    }

    private Employe? _selectedEmployeeForRemove;
    public Employe? SelectedEmployeeForRemove
    {
        get => _selectedEmployeeForRemove;
        set
        {
            _selectedEmployeeForRemove = value;
            OnPropertyChanged();
            ((RelayCommand)RemoveEmployeeCommand).RaiseCanExecuteChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? n = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));
}
