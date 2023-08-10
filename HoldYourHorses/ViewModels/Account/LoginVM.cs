using System.ComponentModel.DataAnnotations;

namespace HoldYourHorses.ViewModels.Account
{
    public class LoginVM
    {
        [Display(Name = "Användarnamn")]
        [Required]
        public string Username { get; set; }

        [Display(Name = "Lösenord")]
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
