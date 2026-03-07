using AutoService.Data.Models;
using AutoService.Views.Services;
using Microsoft.EntityFrameworkCore;

namespace AutoService.ViewModels.Services;

public class ServicesViewModel : BaseCRUDViewModel<Service>
{
    protected override IOrderedQueryable<Service> GetEntities()
    {
        return _db.Services.AsNoTracking().OrderBy(x => x.Name);
    }

    protected override void AddEntity()
    {
        var dlg = new SaveServiceDialog
        {
            Owner = App.Current.MainWindow
        };
        if (dlg.ShowDialog() != true) return;

        _db.Services.Add(dlg.Service);
        _db.SaveChanges();
    }

    protected override void EditEntity()
    {
        var dlg = new SaveServiceDialog(SelectedEntity)
        {
            Owner = App.Current.MainWindow
        };

        if (dlg.ShowDialog() != true) return;

        var existing = _db.Services.Find(dlg.Service.Id);
        if (existing != null)
        {
            existing.Name = dlg.Service.Name;
            existing.Specialization = dlg.Service.Specialization;
            existing.Cost = dlg.Service.Cost;
            _db.SaveChanges();
        }
    }

    protected override void DeleteEntity()
    {
        var existing = _db.Services.Find(SelectedEntity.Id);
        if (existing != null)
        {
            _db.Services.Remove(existing);
            _db.SaveChanges();
        }
    }

    protected override string ConfirmationDeletionMessage()
    {
        return $"Удалить услугу \"{SelectedEntity.Name}\"?";
    }
}