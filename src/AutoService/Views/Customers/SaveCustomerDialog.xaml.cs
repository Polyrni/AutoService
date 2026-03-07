using AutoService.Data.Models;
using AutoService.Views.Behaviors;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace AutoService.Views.Customers;

public partial class SaveCustomerDialog : Window, INotifyPropertyChanged, IDataErrorInfo
{
    public Customer Customer { get; }

    private bool _showErrors = false;

    public string LastName
    {
        get => Customer.LastName;
        set
        {
            Customer.LastName = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(LastNameError));
        }
    }

    public string FirstName
    {
        get => Customer.FirstName;
        set
        {
            Customer.FirstName = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(FirstNameError));
        }
    }

    public string? MiddleName
    {
        get => Customer.MiddleName;
        set
        {
            Customer.MiddleName = value;
            OnPropertyChanged();
        }
    }

    public string Phone
    {
        get => Customer.Phone;
        set
        {
            Customer.Phone = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(PhoneError));
        }
    }

    public string? CarBrand
    {
        get => Customer.CarBrand;
        set
        {
            Customer.CarBrand = value;
            OnPropertyChanged();
        }
    }

    public string? LicensePlate
    {
        get => Customer.LicensePlate;
        set
        {
            Customer.LicensePlate = value;
            OnPropertyChanged();
        }
    }

    public string? Note
    {
        get => Customer.Note;
        set
        {
            Customer.Note = value;
            OnPropertyChanged();
        }
    }

    private string LastNameError => _showErrors && string.IsNullOrWhiteSpace(LastName) ? "Фамилия обязательна" : string.Empty;
    private string FirstNameError => _showErrors && string.IsNullOrWhiteSpace(FirstName) ? "Имя обязательно" : string.Empty;
    private string CarBrandError => _showErrors && string.IsNullOrWhiteSpace(FirstName) ? "Модель машины обязательна" : string.Empty;
    private string LicensePlateError => _showErrors && string.IsNullOrWhiteSpace(FirstName) ? "Гос. номер обязателен" : string.Empty;

    // Валидация телефона: заполнен и полностью введён (9 цифр)
    private string PhoneError
    {
        get
        {
            if (!_showErrors) return string.Empty;
            if (string.IsNullOrWhiteSpace(Phone)) return "Телефон обязателен";
            if (!PhoneMaskBehavior.IsPhoneComplete(Phone)) return "Введите полный номер (9 цифр)";
            return string.Empty;
        }
    }

    public SaveCustomerDialog() : this(null) { }

    public SaveCustomerDialog(Customer? existingCustomer)
    {
        InitializeComponent();

        if (existingCustomer != null)
        {
            Customer = new Customer
            {
                Id = existingCustomer.Id,
                FirstName = existingCustomer.FirstName,
                LastName = existingCustomer.LastName,
                MiddleName = existingCustomer.MiddleName,
                Phone = existingCustomer.Phone,
                CarBrand = existingCustomer.CarBrand,
                LicensePlate = existingCustomer.LicensePlate,
                Note = existingCustomer.Note
            };
            Title = "Редактировать клиента";
        }
        else
        {
            Customer = new Customer();
            Title = "Новый клиент";
        }

        DataContext = this;
    }

    public string Error => string.Empty;

    public string this[string columnName]
    {
        get
        {
            if (!_showErrors) return string.Empty;

            return columnName switch
            {
                nameof(LastName) => LastNameError,
                nameof(FirstName) => FirstNameError,
                nameof(Phone) => PhoneError,
                nameof(LicensePlate) => LicensePlateError,
                nameof(CarBrand) => CarBrandError,
                _ => string.Empty
            };
        }
    }

    private void Ok_Click(object sender, RoutedEventArgs e)
    {
        _showErrors = true;

        OnPropertyChanged(nameof(LastName));
        OnPropertyChanged(nameof(FirstName));
        OnPropertyChanged(nameof(Phone));
        OnPropertyChanged(nameof(LicensePlate));
        OnPropertyChanged(nameof(CarBrand));

        if (string.IsNullOrWhiteSpace(LastName) ||
            string.IsNullOrWhiteSpace(CarBrand) ||
            string.IsNullOrWhiteSpace(LicensePlate) ||
            string.IsNullOrWhiteSpace(FirstName) ||
            string.IsNullOrWhiteSpace(Phone) ||
            !PhoneMaskBehavior.IsPhoneComplete(Phone))
        {
            if (string.IsNullOrWhiteSpace(LastName))
                LastNameTextBox.Focus();
            else if (string.IsNullOrWhiteSpace(FirstName))
                FirstNameTextBox.Focus();
            else if (string.IsNullOrWhiteSpace(LicensePlate))
                LicensePlateTextBox.Focus();
            else if (string.IsNullOrWhiteSpace(CarBrand))
                CarBrandTextBox.Focus();
            else
                PhoneTextBox.Focus();

            return;
        }

        DialogResult = true;
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}