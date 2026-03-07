using AutoService.Data.Models;
using AutoService.Views.Behaviors;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace AutoService.Views.Employees;

public partial class SaveEmployeDialog : Window, INotifyPropertyChanged, IDataErrorInfo
{
    public Employe Employe { get; }

    private bool _showErrors = false;

    public string LastName
    {
        get => Employe.LastName;
        set
        {
            Employe.LastName = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(LastNameError));
        }
    }

    public string FirstName
    {
        get => Employe.FirstName;
        set
        {
            Employe.FirstName = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(FirstNameError));
        }
    }

    public string? MiddleName
    {
        get => Employe.MiddleName;
        set
        {
            Employe.MiddleName = value;
            OnPropertyChanged();
        }
    }

    public string Phone
    {
        get => Employe.Phone;
        set
        {
            Employe.Phone = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(PhoneError));
        }
    }

    public string? Note
    {
        get => Employe.Note;
        set
        {
            Employe.Note = value;
            OnPropertyChanged();
        }
    }

    private string LastNameError => _showErrors && string.IsNullOrWhiteSpace(LastName) ? "Фамилия обязательна" : string.Empty;
    private string FirstNameError => _showErrors && string.IsNullOrWhiteSpace(FirstName) ? "Имя обязательно" : string.Empty;

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

    public SaveEmployeDialog() : this(null) { }

    public SaveEmployeDialog(Employe? existingEmploye)
    {
        InitializeComponent();

        if (existingEmploye != null)
        {
            Employe = new Employe
            {
                Id = existingEmploye.Id,
                FirstName = existingEmploye.FirstName,
                LastName = existingEmploye.LastName,
                MiddleName = existingEmploye.MiddleName,
                Phone = existingEmploye.Phone,
                Note = existingEmploye.Note
            };
            Title = "Редактировать сотрудника";
        }
        else
        {
            Employe = new Employe();
            Title = "Новый сотрудник";
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

        if (string.IsNullOrWhiteSpace(LastName) ||
            string.IsNullOrWhiteSpace(FirstName) ||
            string.IsNullOrWhiteSpace(Phone) ||
            !PhoneMaskBehavior.IsPhoneComplete(Phone))
        {
            if (string.IsNullOrWhiteSpace(LastName))
                LastNameTextBox.Focus();
            else if (string.IsNullOrWhiteSpace(FirstName))
                FirstNameTextBox.Focus();
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