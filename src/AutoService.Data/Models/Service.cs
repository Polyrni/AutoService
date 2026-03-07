namespace AutoService.Data.Models
{
    public class Service
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Specialization { get; set; }

        public decimal Cost { get; set; }

        public override string ToString() => $"{Specialization} ({Name})";
    }
}
