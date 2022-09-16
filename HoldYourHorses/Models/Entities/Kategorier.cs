using System;
using System.Collections.Generic;

namespace HoldYourHorses.Models.Entities
{
    public partial class Kategorier
    {
        public Kategorier()
        {
            Sticks = new HashSet<Stick>();
        }

        public int Id { get; set; }
        public string Namn { get; set; } = null!;

        public virtual ICollection<Stick> Sticks { get; set; }
    }
}
