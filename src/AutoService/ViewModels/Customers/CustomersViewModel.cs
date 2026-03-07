using AutoService.Data.Models;
using AutoService.Views.Customers;
using Microsoft.EntityFrameworkCore;

namespace AutoService.ViewModels.Customers;

public class CustomersViewModel : BaseCRUDViewModel<Customer>
{
    protected override void DeleteEntity()
    {
        var existing = _db.Customers.Find(SelectedEntity.Id);
        if (existing != null)
        {
            _db.Customers.Remove(existing);
            _db.SaveChanges();
        }
    }

    protected override string ConfirmationDeletionMessage()
    {
        return $"Удалить клиента \"{SelectedEntity.FullName}\"?";
    }

    protected override IOrderedQueryable<Customer> GetEntities()
    {
        return _db.Customers.AsNoTracking().OrderBy(x => x.LastName).ThenBy(x => x.FirstName);
    }

    protected override void AddEntity()
    {
        var dlg = new SaveCustomerDialog
        {
            Owner = App.Current.MainWindow
        };
        if (dlg.ShowDialog() != true) return;

        _db.Customers.Add(dlg.Customer);
        _db.SaveChanges();
    }

    protected override void EditEntity()
    {
        var dlg = new SaveCustomerDialog(SelectedEntity)
        {
            Owner = App.Current.MainWindow
        };

        if (dlg.ShowDialog() != true)
        {
            return;
        }

        var existing = _db.Customers.Find(dlg.Customer.Id);
        if (existing != null)
        {
            existing.FirstName = dlg.Customer.FirstName;
            existing.LastName = dlg.Customer.LastName;
            existing.MiddleName = dlg.Customer.MiddleName;
            existing.Phone = dlg.Customer.Phone;
            existing.CarBrand = dlg.Customer.CarBrand;
            existing.LicensePlate = dlg.Customer.LicensePlate;
            existing.Note = dlg.Customer.Note;
            _db.SaveChanges();
        }
    }
}