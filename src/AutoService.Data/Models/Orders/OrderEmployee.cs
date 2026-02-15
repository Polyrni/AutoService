namespace AutoService.Data.Models.Orders
{    public class OrderEmployee
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; }
        public int EmployeeId { get; set; }
        public Employe Employee { get; set; }
    }
}
