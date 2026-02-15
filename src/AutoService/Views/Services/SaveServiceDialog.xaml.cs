using AutoService.Data.Models;
using System.Windows;

namespace AutoService.Views.Services;

public partial class SaveServiceDialog : Window
{
    public Service Service { get; }

    public SaveServiceDialog() : this(null) { }

    public SaveServiceDialog(Service? existingService)
    {
        InitializeComponent();

        if (existingService != null)
        {
            Service = new Service
            {
                Id = existingService.Id,
                Name = existingService.Name,
            };
            Title = "Редактировать услуги";
        }
        else
        {
            Service = new Service();
            Title = "Новая услуга";
        }

        DataContext = Service;
    }

    private void Ok_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
    }
}