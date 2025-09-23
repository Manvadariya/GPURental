using System.ComponentModel.DataAnnotations;

namespace GPURental.ViewModels
{
    public class ManageAccountViewModel
    {
        // For displaying the user's email (read-only)
        public string Email { get; set; }

        // For the Profile Information form
        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Display(Name = "Time Zone")]
        public string Timezone { get; set; }

        // For the Change Password form
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}