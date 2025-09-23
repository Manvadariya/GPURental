using System.ComponentModel.DataAnnotations;

namespace GPURental.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        // --- ADD THIS PROPERTY ---
        [Display(Name = "Time Zone")]
        public string Timezone { get; set; }
        // -------------------------

        [Display(Name = "Register as a Provider?")]
        public bool IsProvider { get; set; }
    }
}