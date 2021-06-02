using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Blog_App.Models
{
    public class Profile
    {
        //Properties
        [Required(ErrorMessage = "Username is missing")] //Check is Username exists
        [StringLength(150, ErrorMessage = "Username must be lower or equal to 150 character")] //Length Check
        [RegularExpression(@"^[A-Za-z0-9@\.\+\-_]+$", ErrorMessage = "Username must cotain Letters, digits and @/./+/-/_ only")] //Character Check
        public string Username { get; set; }
        [Required(ErrorMessage = "Email is missing")] //Check is Email exist
        [DataType(DataType.EmailAddress)] //Datatype Check
        [EmailAddress(ErrorMessage = "Email Format is invalid")] //Check for Email Address
        public string Email { get; set; }
        [Required(ErrorMessage = "Old Password is missing")] //Check is Password exists
        [MinLengthAttribute(8, ErrorMessage = "Password must have atleast 8 characters")] //Min-Length Check
        [DataType(DataType.Password)] //Datatyep Check
        public string Password { get; set; }
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
        public string UserImage { get; set; }
        public string Image { get; set; }
        public IFormFile Photo { get; set; }
        public string Error { get; set; }
        public string ExtErr { get; set; }
        public string PassErr { get; set; }
        public string NPassErr { get; set; }
        public Profile() { } //Default Constructor
        public Profile(string username, string email, string userImage, string image) //Paramterized Constructor
        {
            Username = username;
            Email = email;
            UserImage = userImage;
            Image = image;
        }
    }
}
