namespace HoldYourHorses.Models.Entities
{
    public partial class OrderLine
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ArticleNr { get; set; }
        public int Amount { get; set; }
        public decimal Price { get; set; }
        public string ArticleName { get; set; } = null!;

        public virtual Article ArticleNameNavigation { get; set; } = null!;
        public virtual Article ArticleNrNavigation { get; set; } = null!;
        public virtual Order Order { get; set; } = null!;
    }
}
