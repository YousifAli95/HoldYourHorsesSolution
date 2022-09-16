using System;
using System.Collections.Generic;

namespace HoldYourHorses.Models.Entities
{
    public partial class Favourite
    {
        public int Id { get; set; }
        public string User { get; set; } = null!;
        public int Artikelnr { get; set; }

        public virtual Stick ArtikelnrNavigation { get; set; } = null!;
        public virtual AspNetUser UserNavigation { get; set; } = null!;
    }
}
