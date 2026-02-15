using AutoService.Data;
using AutoService.Data.Models.Orders;
using AutoService.Managers;
using AutoService.ViewModels.Orders;
using System.Windows;

namespace AutoService.Views.Orders
{
    /// <summary>
    /// Interaction logic for SaveOrderDialog.xaml
    /// </summary>
    public partial class SaveOrderDialog : Window
    {
        public SaveOrderDialog(AppDbContext db, OrdersManager ordersManager, Order? order = null)
        {
            InitializeComponent();
            DataContext = new SaveOrderViewModel(db, ordersManager, order);
        }
    }
}
