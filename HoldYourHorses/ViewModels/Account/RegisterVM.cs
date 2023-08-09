using System.ComponentModel.DataAnnotations;

namespace HoldYourHorses.ViewModels.Account
{
    public class RegisterVM
    {
        [Display(Name = "Användarnamn")]
        [Required]
        public string Username { get; set; }

        [Display(Name = "Lösenord")]
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Repetera lösenord")]
        [Compare(nameof(Password))]
        public string PasswordRepeat { get; set; }
    }
}
