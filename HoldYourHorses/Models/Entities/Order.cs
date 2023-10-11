namespace HoldYourHorses.Models.Entities
{
    public partial class Order
    {
        public Order()
        {
            OrderLines = new HashSet<OrderLine>();
        }

        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string City { get; set; } = null!;
        public int ZipCode { get; set; }
        public string Address { get; set; } = null!;
        public string Country { get; set; } = null!;
        public string? User { get; set; }
        public DateTime OrderDate { get; set; }

        public virtual AspNetUser? UserNavigation { get; set; }
        public virtual ICollection<OrderLine> OrderLines { get; set; }
    }
}
