using System;
using System.Collections.Generic;

namespace HoldYourHorses.Models.Entities
{
    public partial class Orderrader
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ArtikelNr { get; set; }
        public int Antal { get; set; }
        public decimal Pris { get; set; }
        public string ArtikelNamn { get; set; } = null!;

        public virtual Stick ArtikelNamnNavigation { get; set; } = null!;
        public virtual Stick ArtikelNrNavigation { get; set; } = null!;
        public virtual Ordrar Order { get; set; } = null!;
    }
}
