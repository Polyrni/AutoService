namespace AutoService.Data.Models
{
    public abstract class Person
    {
        public int Id { get; set; }

        public string LastName { get; set; }

        public string FirstName { get; set; }

        public string? MiddleName { get; set; }

        public string Phone { get; set; }

        public string? Note { get; set; }

        public string FullName => $"{LastName} {FirstName} {MiddleName}".Trim();

        public override string ToString() => FullName;
    }
}
