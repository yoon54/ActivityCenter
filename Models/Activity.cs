using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
namespace ActivityCenter.Models
{
    public class Activitys
    {
        [Key]
        public int ActivityId { get;set; }
        [Required]
        public string Title { get;set; }
        [Required]
        [FutureDate]
        [DataType(DataType.Date) ]
        public DateTime Date { get;set; }
        [Required]
        [DataType(DataType.Time)]
        public DateTime Time { get;set; }
        [Required]
        [Range(1,100)]
        public int Duration { get;set; }
        [Required]
        public string TimeScale { get;set; }
        public DateTime ActivityEnd { get;set;}

        [Required]
        public string Description { get;set; }
        public User Creator { get;set; }
        public int UserId { get;set; }
        public List<Association> Guests { get;set; }

        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;

    }
    public class FutureDateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if(value == null)
            {
                return new ValidationResult("Please Select the Date");
            }
            DateTime compare;
            
            // string nowtime = DateTime.Now.ToString("MM/dd/yyyy 00:00");
            // DateTime today = DateTime.ParseExact(nowtime, "MM/dd/yyyy HH:mm", CultureInfo.InvariantCulture);
            if(value is DateTime)
            {
                compare = (DateTime)value;
                if(compare < DateTime.Today)
                {
                    return new ValidationResult("Please Select the Date in the Future");
                }
                else
                {
                    return ValidationResult.Success;
                }
            }
            else
            {
                return new ValidationResult("Not a Valid Date");
            }
        }
    }

}