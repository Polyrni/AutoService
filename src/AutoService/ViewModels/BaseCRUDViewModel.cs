using AutoService.Data;
using AutoService.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace AutoService.ViewModels
{
    public abstract class BaseCRUDViewModel<Entity> : INotifyPropertyChanged
        where Entity: class
    {
        protected readonly AppDbContext _db = Db.CreateContext();

        public ObservableCollection<Entity> Entities { get; } = new();

        private Entity? _selectedEntity;

        public Entity? SelectedEntity
        {
            get => _selectedEntity;
            set
            {
                _selectedEntity = value;
                OnPropertyChanged();
                ((RelayCommand)EditCommand).RaiseCanExecuteChanged();
                ((RelayCommand)DeleteCommand).RaiseCanExecuteChanged();
            }
        }

        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand RefreshCommand { get; }

        public BaseCRUDViewModel()
        {
            AddCommand = new RelayCommand(Add);
            EditCommand = new RelayCommand(Edit, CanEdit);
            DeleteCommand = new RelayCommand(Delete, CanDelete);
            RefreshCommand = new RelayCommand(Refresh);
            Refresh();
        }

        protected abstract IOrderedQueryable<Entity> GetEntities();
        protected abstract void AddEntity();
        protected abstract void EditEntity();
        protected abstract void DeleteEntity();
        protected abstract string ConfirmationDeletionMessage();

        private void Refresh()
        {
            Entities.Clear();
            foreach (var c in GetEntities())
            {
                Entities.Add(c);
            }
        }

        private void Add()
        {
            AddEntity();
            Refresh();
        }

        private bool CanEdit() => SelectedEntity != null;

        private void Edit()
        {
            if (SelectedEntity == null)
            {
                return;
            }

            EditEntity();

            Refresh();
        }

        private bool CanDelete() => SelectedEntity != null;

        private void Delete()
        {
            if (SelectedEntity == null)
            {
                return;
            }

            var result = MessageBox.Show(
                ConfirmationDeletionMessage(),
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
            {
                return;
            }

            try
            {
                DeleteEntity();

                Refresh();
            }
            catch (DbUpdateException)
            {
                MessageBox.Show(
                    "Нельзя удалить запись, так как он указан в существующих заказах.",
                    "Удаление запрещено",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? n = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));

    }
}
