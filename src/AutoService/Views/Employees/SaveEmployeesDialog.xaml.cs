using AutoService.Data.Models;
using System.Windows;

namespace AutoService.Views.Employees;

public partial class SaveEmployeDialog : Window
{
    public Employe Employe { get; }

    public SaveEmployeDialog() : this(null) { }

    public SaveEmployeDialog(Employe? existingEmploye)
    {
        InitializeComponent();

        if (existingEmploye != null)
        {
            Employe = new Employe
            {
                Id = existingEmploye.Id,
                FullName = existingEmploye.FullName,
                Phone = existingEmploye.Phone,
                Email = existingEmploye.Email,
                Note = existingEmploye.Note
            };
            Title = "Редактировать сотрудника";
        }
        else
        {
            Employe = new Employe();
            Title = "Новый сотрудник";
        }

        DataContext = Employe;
    }

    private void Ok_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
    }
}