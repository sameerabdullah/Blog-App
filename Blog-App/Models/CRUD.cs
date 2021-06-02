using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog_App.Models
{
    public class CRUD
    {
        SqlConnection con; //Connection instance
        public CRUD() //Default Constructor
        {
            try //Exception Hnadling
            {
                string connString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=BlogDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False"; //Connection String
                con = new SqlConnection(connString); //Creating connection bbject
                con.Open(); //Opening Connection
            }
            catch (Exception ex)
            {
                throw ex; //Throwing exception
            }
        }
        public bool signUp(User user)
        {
            if (con == null) //Checking is Database is opened
                return false;
            try //Exception Handling
            {
                string query = $"insert into Users(Username, Email, Password, Photo) values('{user.Username}', '{user.Email}', '{user.Password}', '{user.Photo}')"; //SQL Query to add User
                SqlCommand cmd = new SqlCommand(query, con); //Creating object of SQL Command
                if (cmd.ExecuteNonQuery() > 0) //Executing Query and checking is any row in Users table effected
                    return true;
            }
            catch (Exception)
            {
                return false;
            }
            return false;
        }
        public bool createBlog(Blog blog)
        {
            if (con == null) //Checking is database opened
                return false;
            try //Handling Exception
            {
                string query = $"insert into Blogs(Title, Content, Date, UserID) values('{blog.Title}', '{blog.Content}', '{blog.Date}', {blog.UserID})"; //SQL Query to Add Blog
                SqlCommand cmd = new SqlCommand(query, con); //Creating SQL Command Object
                if (cmd.ExecuteNonQuery() > 0) //Executing Query and checking is any row effected
                    return true;
            }
            catch (Exception)
            {
                return false;
            }
            return false;
        }
        public bool updateBlog(Blog blog)
        {
            if (con == null) //Checking is database opened
                return false;
            try //Exception Handling
            {
                string query = $"Update Blogs set Title = '{blog.Title}', Content = '{blog.Content}', Date = '{blog.Date}' where Id = {blog.Id}"; //SQL Query to Update Blogs
                SqlCommand cmd = new SqlCommand(query, con); //Creating SQL Command Object
                if (cmd.ExecuteNonQuery() > 0) //Executing Query and checking is any line effected
                    return true;
            }
            catch (Exception)
            {
                return false;
            }
            return false;
        }
        public bool deleteBlog(int id)
        {
            if (con == null) //Checking is database opened
                return false;
            try //Exception Hanlding
            {
                string query = $"Delete from Blogs where Id = {id}"; //
                SqlCommand cmd = new SqlCommand(query, con); //Creating SQL Command object
                if (cmd.ExecuteNonQuery() > 0) //Executing Query and checking is any line effected in table
                    return true;
            }
            catch (Exception)
            {
                return false;
            }
            return false;
        }
        public int logIn(string username, string password)
        {
            if (con == null) //Checking is database opened
                return -1;
            string query = $"select * from Users where Username = @user and password = @pass"; //SQL Query to validate user
            SqlParameter user = new SqlParameter("user", username); //Creating query paramter
            SqlParameter pass = new SqlParameter("pass", password); //Creating query paramter
            SqlCommand cmd = new SqlCommand(query, con); //Creating SQL Command Object
            cmd.Parameters.Add(user); //Passing parameter to query
            cmd.Parameters.Add(pass); //Passing parameter to query 
            SqlDataReader dr = cmd.ExecuteReader(); //Executing Query
            if (dr.HasRows) //Checking is Data Reader contain any row 
            {
                dr.Read(); //Iterating row
                return Convert.ToInt32(dr[0]); //Returning user id
            }
            return -1;
        }
        public int adminAuth(string username, string password)
        {
            if (con == null) //Checking is database opened
                return -1;
            string query = $"select * from Admins where Username = @user and password = @pass"; //SQL Query to validate admin
            SqlParameter user = new SqlParameter("user", username); //Creating Query paramter
            SqlParameter pass = new SqlParameter("pass", password); //Creating Query paramter
            SqlCommand cmd = new SqlCommand(query, con); //Creating SQL Command Object
            cmd.Parameters.Add(user); //Adding paramter to query
            cmd.Parameters.Add(pass); //Adding paramter to query
            SqlDataReader dr = cmd.ExecuteReader(); //Executing Query
            if (dr.HasRows) //Checking is data reader has rows
            {
                dr.Read(); //Iterating rows
                return Convert.ToInt32(dr[0]); //Returing id
            }
            return -1;
        }
        public bool validatePass(int id, string password)
        {
            if (con == null) //Checking is database opened
                return false;
            string query = $"select * from Users where Id = {id} and password = @pass"; //SQL Quey to validate pass
            SqlParameter pass = new SqlParameter("pass", password); //Creating quer paramter
            SqlCommand cmd = new SqlCommand(query, con); //Creating SQL Command Object
            cmd.Parameters.Add(pass); //Adding paramter to query
            SqlDataReader dr = cmd.ExecuteReader(); //Executing Query
            if (dr.HasRows) //Checking is data reader contain any row
                return true;
            return false;
        }
        public Profile getUserProfile(int id)
        {
            if (con == null) //Checking is database opened
                return null;
            string query = $"select * from Users where Id = {id}"; //SQL Query to get user
            SqlCommand cmd = new SqlCommand(query, con); //Creating SQL Command Object
            SqlDataReader dr = cmd.ExecuteReader(); //Executing Query
            if (dr.HasRows) //Checking is data reader contain any row
            {
                dr.Read(); //Iterating row
                return new Profile(dr[1].ToString().Trim(), dr[2].ToString().Trim(), dr[4].ToString().Trim(), dr[4].ToString().Substring(8, dr[4].ToString().Length - 8).Trim());
            }
            return null;
        }
        public EUser getUser(int id)
        {
            if (con == null) //Checking is database opened
                return null;
            string query = $"select * from Users where Id = {id}"; //SQL Query to get user
            SqlCommand cmd = new SqlCommand(query, con); //Creating SQL Command Object
            SqlDataReader dr = cmd.ExecuteReader(); //Executing Query
            if (dr.HasRows) //Checking is data reader has any row
            {
                dr.Read(); //Iterating row
                return new EUser(Convert.ToInt32(dr[0]), dr[1].ToString().Trim(), dr[2].ToString().Trim(), dr[4].ToString().Trim(), dr[4].ToString().Substring(8, dr[4].ToString().Length - 8).Trim());
            }
            return null;
        }
        public List<Blog> getBlogs()
        {
            if (con == null) return null;
            List<Blog> blogs = new List<Blog>();
            string query = $"select Title, Content, Date, Username, Photo, UserID, b.Id from Blogs b, Users u where u.Id = b.UserID order by b.id DESC";
            SqlCommand cmd = new SqlCommand(query, con);
            SqlDataReader dr = cmd.ExecuteReader();
            if(!dr.HasRows) return null;
            while (dr.Read())
                blogs.Add(new Blog(dr[0].ToString(), dr[1].ToString(), dr[2].ToString(), dr[3].ToString(), dr[4].ToString(), Convert.ToInt32(dr[5]), Convert.ToInt32(dr[6])));
            return blogs;
        }
        public Blog getBlog(int id)
        {
            if (con == null) //Checking is database opened
                return null;
            string query = $"select Title, Content, Date, Username, Photo, UserID, b.Id from Blogs b, Users u where u.Id = b.UserID and b.id = {id}"; //SQL Query to get blogs
            SqlCommand cmd = new SqlCommand(query, con); //Creating SQL Command Object
            SqlDataReader dr = cmd.ExecuteReader(); //Executing Query
            if (!dr.HasRows) //Checking is data reader has any row
                return null;
            while (dr.Read())
                return new Blog(dr[0].ToString(), dr[1].ToString(), dr[2].ToString(), dr[3].ToString(), dr[4].ToString(), Convert.ToInt32(dr[5]), Convert.ToInt32(dr[6]));
            return null;
        }
        public bool updateProfile(User user)
        {
            if (con == null) //checkinh is database opened
                return false;
            string pr = ""; //Initalizing string
            if (user.Password != null) //Checking is password contain any value
                pr = $", Password = '{user.Password}'"; //Settng extra part for query
            try //Exception Handling
            {
                string query = $"Update Users set Username = '{user.Username}', Email = '{user.Email}'{pr}, Photo = '{user.Photo}' where Id = {user.Id}"; //SQL Query to Update User
                SqlCommand cmd = new SqlCommand(query, con); //Creating SQL Command Object
                if (cmd.ExecuteNonQuery() > 0) //Executing query and checking is any row effected in table
                    return true;
            }
            catch (Exception)
            {
                return false;
            }
            return false;
        }
        public List<User> getUsers()
        {
            if (con == null) //Checking is database opened
                return null;
            List<User> users = new List<User>(); //Creating User List
            string query = $"select * from Users"; //SQL Query to get all users
            SqlCommand cmd = new SqlCommand(query, con); //Creating SQL Command Object
            SqlDataReader dr = cmd.ExecuteReader(); //Executing Query
            if (!dr.HasRows) //Checking is data reader contain any row
                return null;
            while (dr.Read()) //Iterating row and checking is next row exist
                users.Add(new User(Convert.ToInt32(dr[0]), dr[1].ToString(), dr[2].ToString(), dr[3].ToString(), dr[4].ToString())); //Adding user in User list
            return users;
        }
        public bool deleteUser(int id)
        {
            if (con == null) //Checking is database opened
                return false;
            try //Execption Handling
            {
                string query = $"Delete from Users where Id = {id}"; //SQL Query for Deleting User
                SqlCommand cmd = new SqlCommand(query, con); //Creating SQL Command Object
                if (cmd.ExecuteNonQuery() > 0) //Executing Query and checking is any row is effected in table
                    return true;
            }
            catch (Exception)
            {
                return false;
            }
            return false;
        }
        public void closeDB()
        {
            con.Close(); //Closing database
        }
    }
}
