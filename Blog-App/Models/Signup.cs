using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Blog_App.Models
{
    public class Signup
    {
        //Properties
        [Required(ErrorMessage = "Username is missing")] //Check is Username exists
        [StringLength(150, ErrorMessage = "Username must be lower or equal to 150 character")] //Length Check
        [RegularExpression(@"^[A-Za-z0-9@\.\+\-_]+$", ErrorMessage = "Username must cotain Letters, digits and @/./+/-/_ only")] //Characters Check
        public string Username { get; set; }
        [Required(ErrorMessage = "Email is missing")] //Check is Email exists
        [DataType(DataType.EmailAddress)] //Datatype Check
        [EmailAddress(ErrorMessage = "Email Format is invalid")] //Check for Email Address
        public string Email { get; set; }
        [Required(ErrorMessage = "Password is missing")] //Check is Password exists
        [MinLengthAttribute(8, ErrorMessage = "Password must have atleast 8 characters")] //Min-length Check
        [DataType(DataType.Password)] //Datatype Check
        public string Password { get; set; }
        [Required(ErrorMessage = "Photo is missing")] //Check is File exists
        public IFormFile Photo { get; set; }
        public string Error { get; set; }
        public string ExtErr { get; set; }
    }
}
