using AutoService.Data.Models;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AutoService.Views.Services;

public partial class SaveServiceDialog : Window, INotifyPropertyChanged, IDataErrorInfo
{
    public Service Service { get; }

    private bool _showErrors = false;

    public string ServiceName
    {
        get => Service.Name;
        set
        {
            Service.Name = value ?? string.Empty;
            OnPropertyChanged();
            OnPropertyChanged(nameof(NameError));
        }
    }

    public string Specialization
    {
        get => Service.Specialization;
        set
        {
            Service.Specialization = value ?? string.Empty;
            OnPropertyChanged();
            OnPropertyChanged(nameof(SpecializationError));
        }
    }

    private string _costRaw = string.Empty;

    public string CostText
    {
        get => _costRaw;
        set
        {
            _costRaw = value ?? string.Empty;
            OnPropertyChanged();
            OnPropertyChanged(nameof(CostError));
        }
    }

    private string CostError
    {
        get
        {
            if (!_showErrors) return string.Empty;
            if (string.IsNullOrWhiteSpace(_costRaw))
                return "Стоимость должна быть больше 0";

            var normalized = _costRaw.Replace(',', '.');

            if (!decimal.TryParse(normalized, NumberStyles.Number, CultureInfo.InvariantCulture, out var result))
                return "Некорректный формат стоимости";

            var abs = Math.Abs(result);
            var scaled = abs * 100;
            if (scaled != Math.Truncate(scaled))
                return "Не более двух знаков после запятой";

            if (result <= 0)
                return "Стоимость должна быть больше 0";

            return string.Empty;
        }
    }

    private static readonly Regex _costRegex = new(@"^\d*(?:[.,]\d{0,2})?$");

    private void CostTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        var tb = (TextBox)sender;

        // Учёт выделения: какой текст получится после ввода символа
        var proposed = tb.Text.Remove(tb.SelectionStart, tb.SelectionLength)
            .Insert(tb.SelectionStart, e.Text);

        e.Handled = !_costRegex.IsMatch(proposed);
    }

    private string NameError => _showErrors && string.IsNullOrWhiteSpace(ServiceName) ? "Вид работы обязателен" : string.Empty;
    private string SpecializationError => _showErrors && string.IsNullOrWhiteSpace(Specialization) ? "Специализация обязательна" : string.Empty;

    public SaveServiceDialog() : this(null) { }

    public SaveServiceDialog(Service? existingService)
    {
        InitializeComponent();

        if (existingService != null)
        {
            Service = new Service
            {
                Id = existingService.Id,
                Name = existingService.Name ?? string.Empty,
                Specialization = existingService.Specialization ?? string.Empty,
                Cost = existingService.Cost
            };
            Title = "Редактировать услугу";
        }
        else
        {
            Service = new Service
            {
                Name = string.Empty,
                Specialization = string.Empty,
                Cost = 0
            };
            Title = "Новая услуга";
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
                nameof(ServiceName) => NameError,
                nameof(Specialization) => SpecializationError,
                nameof(CostText) => CostError,
                _ => string.Empty
            };
        }
    }
    
    private void Ok_Click(object sender, RoutedEventArgs e)
    {
        _showErrors = true;

        OnPropertyChanged(nameof(Specialization));
        OnPropertyChanged(nameof(ServiceName));
        OnPropertyChanged(nameof(CostText));

        bool hasSpecializationError = string.IsNullOrWhiteSpace(Specialization);
        bool hasNameError = string.IsNullOrWhiteSpace(ServiceName);
        bool hasCostError = !string.IsNullOrEmpty(CostError);

        if (hasSpecializationError || hasNameError || hasCostError)
        {
            if (hasSpecializationError)
                SpecializationTextBox.Focus();
            else if (hasNameError)
                NameTextBox.Focus();
            else if (hasCostError)
                CostTextBox.Focus();
            return;
        }

        // Здесь уже точно всё валидно — можно безопасно парсить и записать в Service.Cost
        var normalized = _costRaw.Replace(',', '.');
        if (decimal.TryParse(normalized, NumberStyles.Number, CultureInfo.InvariantCulture, out var result))
        {
            Service.Cost = result;
        }

        DialogResult = true;
    }


    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}