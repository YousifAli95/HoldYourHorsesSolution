namespace HoldYourHorses.Models.Entities
{
    public partial class Article
    {
        public Article()
        {
            Favourites = new HashSet<Favourite>();
            OrderLineArticleNameNavigations = new HashSet<OrderLine>();
            OrderLineArticleNrNavigations = new HashSet<OrderLine>();
        }

        public int ArticleNr { get; set; }
        public decimal Price { get; set; }
        public int HorsePowers { get; set; }
        public int TreeDensity { get; set; }
        public string ArticleName { get; set; } = null!;
        public int MaterialId { get; set; }
        public int CategoryId { get; set; }
        public string Description { get; set; } = null!;
        public int OriginCountryId { get; set; }
        public bool AbsBrake { get; set; }

        public virtual Category Category { get; set; } = null!;
        public virtual Material Material { get; set; } = null!;
        public virtual OriginCountry OriginCountry { get; set; } = null!;
        public virtual ICollection<Favourite> Favourites { get; set; }
        public virtual ICollection<OrderLine> OrderLineArticleNameNavigations { get; set; }
        public virtual ICollection<OrderLine> OrderLineArticleNrNavigations { get; set; }
    }
}
