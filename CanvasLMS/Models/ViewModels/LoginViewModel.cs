using CanvasLMS.Attributes;
using System.ComponentModel.DataAnnotations;

namespace CanvasLMS.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [EmailDomainValidation(ErrorMessage = "Email must end with @ulacit.ed.cr")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
