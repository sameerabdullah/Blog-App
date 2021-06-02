using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog_App.Models
{
    public class Blog
    {
        //Properties
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Date { get; set; }
        public string Username { get; set; }
        public string UserImage { get; set; }
        public int UserID { get; set; }
        public bool isSame { get; set; }
        public Blog() //Default Constructor
        {
            isSame = false;
        }
        public Blog(string title, string content, string date, int uid) //Parameterized Constructor
        {
            Title = title;
            Content = content;
            Date = date;
            UserID = uid;
        }
        public Blog(int id, string title, string content, string date) //Parameterized Constructor
        {
            Id = id;
            Title = title;
            Content = content;
            Date = date;
        }
        public Blog(string title, string content, string date, string username, string userImage, int uid, int id) //Parameterized Constructor
        {
            Id = id;
            Title = title;
            Content = content;
            Date = date;
            Username = username;
            UserImage = userImage;
            UserID = uid;
            isSame = false;
        }
    }
}
