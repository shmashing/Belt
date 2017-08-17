using System;
using System.ComponentModel.DataAnnotations;

namespace BeltExam.Models {
    public class RegisterViewModel : BaseEntity {
        [Required(ErrorMessage = "Please enter a first name")]
        [MinLength(3, ErrorMessage = "Names must be {1} characters or longer")]
        [RegularExpression(@"[a-zA-Z]+$", ErrorMessage="Name can only contain letters")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Please enter a last name")]
        [MinLength(3, ErrorMessage = "Names must be {1} characters or longer")]
        [RegularExpression(@"[a-zA-Z]+$", ErrorMessage="Name can only contain letters")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Please enter a username")]
        [MinLength(5, ErrorMessage = "Username must be {1} characters or longer")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Please enter an email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter a password")]
        [MinLength(8, ErrorMessage = "Passwords must be {1} characters or longer")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage="Passwords must match")]
        [DataType(DataType.Password)]
        public string PasswordConf { get; set; }
    }
}