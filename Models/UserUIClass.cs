using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Royal_Project_3.Models
{
    public class UserUIClass
    {
        public string ID { get; set; }

        [Display(Name = "Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Name required...!!!")]
        public string cname { get; set; }

        [Display(Name = "Mobile")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Phone number is required...!!!")]
        [MinLength(10,ErrorMessage ="Minimum 10 digits required...!!!")]
        [MaxLength(10,ErrorMessage ="Maximum 10 digits required...!!!")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$",
                   ErrorMessage = "Entered phone format is not valid.")]
        public string cmobile { get; set; }

        [Display(Name = "Email")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Email ID required...!!!")]
        [DataType(DataType.EmailAddress)]
        public string cemail { get; set; }

        [Display(Name = "Query")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Query required...!!!")]
        public string cquery { get; set; }
    }
}