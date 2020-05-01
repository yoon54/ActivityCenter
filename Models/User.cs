using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
namespace ActivityCenter.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        [Required]
        [MinLength(2)]
        public string FirstName { get; set; }
        [Required]
        [MinLength(2)]
        public string LastName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [MinLength(8)]
        [PasswordValidation]
        [DataType(DataType.Password)]
        public string Password { get;set; }
        
        [Required]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string Confirm { get;set; }
        public List<Activitys> Plans { get;set; }
        public List<Association> Attending { get;set; }
        public DateTime Date { get; set; } = DateTime.Now;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }

    public class PasswordValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {   
            var input = Convert.ToString(value);
            var hasNumber = new Regex(@"[0-9]+");
            var hasUpperCase = new Regex(@"[A-Z]+");
            var hasSpecial = new Regex(@"[!@#$%^&*()_+=\[{\]};:<>|./?,-]");

        
            if (!hasNumber.IsMatch(input))
            {
                return new ValidationResult("Password must contain at least one Number");
            }
            else if (!hasUpperCase.IsMatch(input))
            {
                return new ValidationResult("Password must contain at least one Upper Case");
            }
            else if (!hasSpecial.IsMatch(input))
            {
                return new ValidationResult("Password must contain at least one Special Character");
            }
            else
            {
                return ValidationResult.Success;
            }
           
        }
    }
}