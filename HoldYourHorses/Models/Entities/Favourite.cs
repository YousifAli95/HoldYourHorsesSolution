namespace HoldYourHorses.Models.Entities
{
    public partial class Favourite
    {
        public int Id { get; set; }
        public string User { get; set; } = null!;
        public int ArticleNr { get; set; }

        public virtual Article ArticleNrNavigation { get; set; } = null!;
        public virtual AspNetUser UserNavigation { get; set; } = null!;
    }
}
