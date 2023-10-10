namespace HoldYourHorses.Models.Entities
{
    public partial class Category
    {
        public Category()
        {
            Articles = new HashSet<Article>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;

        public virtual ICollection<Article> Articles { get; set; }
    }
}
