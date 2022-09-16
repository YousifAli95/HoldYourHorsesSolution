using System.ComponentModel.DataAnnotations;

namespace HoldYourHorses.Views.Sticks
{
    public class CheckoutVM
    {
        [Display(Name = "Förnamn")]
        [Required(ErrorMessage = "Var vänlig och mata in ditt förnamn")]
        public string FirstName { get; set; }

        [Display(Name = "Efternamn")]
        [Required(ErrorMessage = "Var vänlig och mata in ditt efternamn")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Display(Name = "Adress")]
        [Required(ErrorMessage = "Var vänlig och mata in din adress")]
        public string Address { get; set; }

        [Display(Name = "Stad")]
        [Required(ErrorMessage = "Var vänlig och mata in din stad")]
        public string City { get; set; }

        [Display(Name = "Postkod")]
        [Required(ErrorMessage = "Var vänlig och mata in ditt postnummer")]
        public int ZipCode { get; set; }

        [Display(Name = "Land")]
        [Required(ErrorMessage = "Var vänlig och mata in ditt land")]
        public string Country { get; set; }
    }
}
