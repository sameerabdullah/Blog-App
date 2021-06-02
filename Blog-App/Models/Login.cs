using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Blog_App.Models
{
    public class Login
    {
        //Properties
        [Required(ErrorMessage = "Username is missing")] //Check is Username exists
        [StringLength(150, ErrorMessage = "Username must be lower or equal to 150 character")] //Length Check
        [RegularExpression(@"^[A-Za-z0-9@\.\+\-_]+$", ErrorMessage = "Username must cotain Letters, digits and @/./+/-/_ only")] //Character Check
        public string Username { get; set; }
        [Required(ErrorMessage = "Password is missing")] //Check is Password exists
        [MinLengthAttribute(8, ErrorMessage = "Password must have atleast 8 characters")] //Min Length Check
        [DataType(DataType.Password)] //Datatype Check
        public string Password { get; set; }
        public string Error { get; set; }
    }
}
