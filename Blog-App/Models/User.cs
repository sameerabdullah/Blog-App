using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog_App.Models
{
    public class User
    {
        //Properties
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Photo { get; set; }
        public User() {} //Default Constructor
        public User(string username, string email, string password, string photo) //Parameterized Constructor
        {
            Username = username;
            Email = email;
            Password = password;
            Photo = photo;
        }
        public User(int id, string username, string email, string password, string photo) //Parameterized Constructor
        {
            Id = id;
            Username = username;
            Email = email;
            Password = password;
            Photo = photo;
        }
    }
}