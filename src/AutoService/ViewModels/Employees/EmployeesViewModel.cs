using AutoService.Data.Models;
using AutoService.Views.Employees;
using Microsoft.EntityFrameworkCore;

namespace AutoService.ViewModels.Employees;

public class EmployeesViewModel : BaseCRUDViewModel<Employe>
{
    protected override IOrderedQueryable<Employe> GetEntities()
    {
        return _db.Employees.AsNoTracking().OrderBy(x => x.LastName).ThenBy(x => x.FirstName);
    }

    protected override void AddEntity()
    {
        var dlg = new SaveEmployeDialog
        {
            Owner = App.Current.MainWindow
        };
        if (dlg.ShowDialog() != true) return;

        _db.Employees.Add(dlg.Employe);
        _db.SaveChanges();
    }

    protected override void EditEntity()
    {
        var dlg = new SaveEmployeDialog(SelectedEntity)
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
            existing.FirstName = dlg.Employe.FirstName;
            existing.LastName = dlg.Employe.LastName;
            existing.MiddleName = dlg.Employe.MiddleName;
            existing.Phone = dlg.Employe.Phone;
            existing.Note = dlg.Employe.Note;
            _db.SaveChanges();
        }
    }

    protected override void DeleteEntity()
    {
        var existing = _db.Employees.Find(SelectedEntity.Id);
        if (existing != null)
        {
            _db.Employees.Remove(existing);
            _db.SaveChanges();
        }
    }

    protected override string ConfirmationDeletionMessage()
    {
        return $"Удалить сотрудника \"{SelectedEntity.FullName}\"?";
    }
}