// AutoService.ViewModels.Orders/OrderServiceItemViewModel.cs
using AutoService.Data.Models;
using AutoService.Data.Models.Orders;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AutoService.ViewModels.Orders
{
    public class OrderServiceItemViewModel : INotifyPropertyChanged
    {
        private readonly OrderService _orderService;

        public OrderServiceItemViewModel(OrderService orderService)
        {
            _orderService = orderService;
        }

        public int Id => _orderService.Id;
        public int ServiceId => _orderService.ServiceId;
        public Service Service => _orderService.Service;

        public decimal Price
        {
            get => _orderService.Price;
            set
            {
                if (_orderService.Price != value)
                {
                    _orderService.Price = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? n = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));
    }
}