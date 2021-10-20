using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Vehicle_Selling_Site.Models
{
    public class LoginViewModel //the login page's model
    {
        //*********** the Email field:
        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        //*********** the Password field:
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
    }
}
