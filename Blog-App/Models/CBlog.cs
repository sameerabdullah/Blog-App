using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Blog_App.Models
{
    public class CBlog
    {
        //Properties
        public int Id { get; set; }
        [Required(ErrorMessage = "Title is missing")] //Check is title exist
        [StringLength(150, ErrorMessage = "Title must be lower or equal to 50 character")] //Length Check
        public string Title { get; set; }
        [Required(ErrorMessage = "Content is missing")] //Check is content exist
        [StringLength(2000, ErrorMessage = "Content must be lower or equal to 2000 character")] //Length Check
        public string Content { get; set; }
        public string Error { get; set; }
        public CBlog() { } //Default Constructor
        public CBlog(string title, string content) //Parameterized Constructor
        {
            Title = title;
            Content = content;
        }
        public CBlog(int id, string title, string content) //Parameterized Constructor
        {
            Id = id;
            Title = title;
            Content = content;
        }
    }
}
