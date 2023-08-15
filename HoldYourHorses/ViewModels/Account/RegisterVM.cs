using System.ComponentModel.DataAnnotations;

namespace HoldYourHorses.ViewModels.Account
{
    public class RegisterVM
    {
        [Required(ErrorMessage = "Användarnamn är obligatoriskt.")]
        [Display(Name = "Användarnamn")]
        public string Username { get; set; }

        [EmailAddress(ErrorMessage = "Ange en giltig e-postadress.")]
        [Required(ErrorMessage = "E-postadress är obligatorisk.")]
        [Display(Name = "E-postadress")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Lösenord är obligatoriskt.")]
        [DataType(DataType.Password)]
        [Display(Name = "Lösenord")]
        public string Password { get; set; }

        [Required(ErrorMessage = "'Repetera Lösenord' är obligatoriskt.")]
        [DataType(DataType.Password)]
        [Display(Name = "Repetera lösenord")]
        [Compare(nameof(Password), ErrorMessage = "Lösenorden matchar inte.")]
        public string PasswordRepeat { get; set; }
    }
}
