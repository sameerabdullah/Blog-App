using Blog_App.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Blog_App.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public ViewResult Login()
        {
            if (HttpContext.Request.Cookies.ContainsKey("admin_id")) //Checking is admin logged in
            {
                HttpContext.Response.Cookies.Delete("admin_id"); //Deleting Cookie containing admin id
                return View("AdminAuth"); //Rendering Admin Login Page
            }
            if (HttpContext.Request.Cookies.ContainsKey("login_id")) //Checking is user logged in
                HttpContext.Response.Cookies.Delete("login_id"); //Deleting Cookie containing user id
            return View(); //Rendering Login Page
        }
        [HttpPost]
        public async Task<ViewResult> LoginAsync(Signup signup)
        {
            if (!ModelState.IsValid) //Checking Model State
                return View("Signup"); //Rendering Signup Page
            string imgName = signup.Photo.FileName.ToLower(); //Convering File Name to lower case
            if ((imgName.Length > 4 && imgName.IndexOf(".png") != imgName.Length - 4 && imgName.IndexOf(".jpg") != imgName.Length - 4 && imgName.IndexOf(".jpeg") != imgName.Length - 5)) //Checking extension of File is jpg/png/jpeg
            {
                signup.ExtErr = "Invalid Image Format"; //Assigning Extension Error
                return View("Signup", signup); //Rendering Signup Page
            }
            string url = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/", signup.Username + signup.Photo.FileName); //Getting Absolute Path to store Image
            Stream fileStream = new FileStream(url, FileMode.Create); //Creating FileStream Object
            await signup.Photo.CopyToAsync(fileStream); //Copying Image in Asyns way
            try //Exception handling
            {
                CRUD crud = new CRUD(); //Opening Database
                if (crud.signUp(new User(signup.Username, signup.Email, signup.Password, $"/images/{signup.Username + signup.Photo.FileName}"))) //Signing Up User and verifing is one successfully signed up
                {
                    crud.closeDB(); //Closing Database
                    if(HttpContext.Request.Cookies.ContainsKey("admin_id")) //Checking is admin logged in
                    {
                        try
                        {
                            crud = new CRUD(); //Opening Database
                            List<User> users = crud.getUsers(); //Getting Users from database
                            crud.closeDB(); //Closing database
                            return View("AdminDashboard", users); //Rendering Admin Dashboard Page
                        }
                        catch (Exception)
                        {
                            return View("Error", "Something went wrong"); //Rendering Error Page
                        }
                    }
                    return View("Login"); //Rendering Login Page
                }
                crud.closeDB(); //Closing Database
                signup.Error = "User already exist with same Username or Email"; //Assigning Error
                return View("Signup", signup); //Rendering Signup Page
            }
            catch (Exception)
            {
                signup.Error = "Something went wrong"; //Assigning Error
                return View("Signup", signup); //Rendering Signup Page
            }
        }
        public ViewResult Signup()
        {
            return View(); //Rendering Signup Page
        }
        [HttpGet]
        public ViewResult Home()
        {   
            if (HttpContext.Request.Cookies.ContainsKey("login_id") || HttpContext.Request.Cookies.ContainsKey("admin_id")) //Checking is User or Admin Logged In
            {
                try //Exception Handling
                {
                    CRUD crud = new CRUD(); //Opening Database
                    List<Blog> blogs = crud.getBlogs(); //Getting all Blogs from database
                    if (blogs != null) //Checking are blogs successfully received
                    {
                        crud.closeDB(); //Closing Database
                        return View("Home", blogs); //Rendering to Home
                    }
                    crud.closeDB(); //Closing Database
                    return View("Error", "No blog exists"); //Rendering to Error Page
                }
                catch (Exception)
                {
                    return View("Error", "Something went wrong"); //Rendering to Error Page
                }
            }
            return View("Login"); //Rendering to Login Page
        }
        [HttpPost]
        public ViewResult Home(Login login)
        {
            if (HttpContext.Request.Cookies.ContainsKey("admin_id")) //Checking is Admin logged in
                HttpContext.Response.Cookies.Delete("admin_id"); //Deleting Cookie containing Admin id
            if (!ModelState.IsValid) //Checking Model State
                return View("Login"); //Rendering to Login Page
            try //Exception Handling
            {
                CRUD crud = new CRUD(); //Opening Database
                int id = crud.logIn(login.Username, login.Password); //Logging in User
                crud.closeDB(); //Closing Database
                if (id != -1) //Checking is user successfully logged in
                {
                    crud = new CRUD(); //Opening database
                    List<Blog> blogs = crud.getBlogs(); //Getting Blogs from database
                    crud.closeDB(); //Closing database
                    if (!HttpContext.Request.Cookies.ContainsKey("login_id")) //Checking is any Cookie of User id exist
                        HttpContext.Response.Cookies.Append("login_id", id.ToString()); //Creating new Cookie containing User id
                    return View("Home", blogs); //Rendering Home Page
                }
                login.Error = "Invalid Username or Password"; //Assigning Error
                return View("Login", login); //Rendering Login Page
            }
            catch (Exception)
            {
                login.Error = "Something went wrong"; //Assigning Error
                return View("Login", login); //Rendering Login Page
            }
        }
        [HttpGet]
        public ViewResult CreateBlog()
        {
            if (HttpContext.Request.Cookies.ContainsKey("login_id") && !HttpContext.Request.Cookies.ContainsKey("admin_id")) //Checking is User logged in and Admin logged out
                return View(); //Rendering Create Blog Page
            return View("Login"); //Rendering Login Page
        }
        [HttpPost]
        public ViewResult CreateBlog(CBlog cblog)
        {
            if (!ModelState.IsValid) //Checking Model State
                return View();//Rendering Create Blog Page
            if (!HttpContext.Request.Cookies.ContainsKey("login_id")) //Checking logged in
                return View("Login"); //Rendering Login Page
            try //Exception Handling
            {
                CRUD crud = new CRUD(); //Opening Database
                if (crud.createBlog(new Blog(cblog.Title, cblog.Content, DateTime.Now.ToString("MMMM dd, yyyy"), Convert.ToInt32(HttpContext.Request.Cookies["login_id"])))) //Creating Blog and checking is it created
                {
                    crud.closeDB(); //Closing Database
                    crud = new CRUD(); //Openning Database
                    List<Blog> blogs = crud.getBlogs(); //Getting blogs from database
                    crud.closeDB(); //Closing Database
                    return View("Home", blogs); //Rendering Home Page
                }
                crud.closeDB(); //Closing Database
                cblog.Error = "Something went wrong"; //Assigning Error
                return View("CreateBlog", cblog); //Rendering Create Blog Page
            }
            catch (Exception)
            {
                cblog.Error = "Something went wrong"; //Assigning Error
                return View("CreateBlog", cblog); //Rendering Create Blog
            }
        }
        [HttpGet]
        public ViewResult ViewBlog(int id)
        {
            if (!HttpContext.Request.Cookies.ContainsKey("login_id") && !HttpContext.Request.Cookies.ContainsKey("admin_id")) //Checking is admin and user both logged in
                return View("Login"); //Rendering to Login Page
            try //Execption Handing
            {
                CRUD crud = new CRUD(); //Opening Database
                Blog blog = crud.getBlog(id); //Getting Blog from database whose id is in URL
                crud.closeDB(); //Closing Database
                if (blog != null) // Checking is blog successfully received
                {
                    if (!HttpContext.Request.Cookies.ContainsKey("admin_id")?Convert.ToInt32(HttpContext.Request.Cookies["login_id"]) == blog.UserID :false) //Checking is blog from same user
                        blog.isSame = true; //Marking Blog
                    return View("ViewBlog", blog); //Rendering View Blog
                }
                return View("Error", "No blog exists"); //Rendering Error Page
            }
            catch (Exception){
                return View("Error", "Something went wrong"); //Rendering Error Page
            }
        }
        [HttpGet]
        public ViewResult UpdateBlog(int id)
        {
            if (!HttpContext.Request.Cookies.ContainsKey("login_id")) //Checking is user logged in
                return View("Login"); //Rendering Login Page
            try //Exception Handling
            {
                CRUD crud = new CRUD(); //Opening Database
                Blog blog = crud.getBlog(id); //Getting blog from database whose id is in URL
                crud.closeDB(); //Closing Database
                if (blog != null) //Checking is blog successfully recieved
                {
                    return View("UpdateBlog", new CBlog(id, blog.Title.Trim(), blog.Content.Trim())); //Rendering to Updatelog Page
                }
                return View("Error", "No blog exists"); //Rendering to Error Page
            }
            catch (Exception)
            {
                return View("Error", "Something went wrong"); //Rendering Error Page
            }
        }
        [HttpPost]
        public ViewResult UpdateBlog(CBlog cblog)
        {
            if (!ModelState.IsValid) //Checking Model State
                return View(); //Rednering Update Blog Page
            try //Exception Handing
            {
                CRUD crud = new CRUD(); //Opening Datase
                if (crud.updateBlog(new Blog(cblog.Id, cblog.Title, cblog.Content, DateTime.Now.ToString("MMMM dd, yyyy")))) //Updating Blog and checking is it updated successfully
                {
                    crud.closeDB(); //Closing Database
                    crud = new CRUD(); //Opening Database
                    List<Blog> blogs = crud.getBlogs(); //Getting Blogs from database
                    crud.closeDB(); //Closing Database
                    return View("Home", blogs); //Rendering to Home Page
                }
                crud.closeDB(); //
                cblog.Error = "Something went wrong"; //Assigning Error
                return View("UpdateBlog", cblog); //Rendering Update Blog Page
            }
            catch (Exception)
            {
                cblog.Error = "Something went wrong"; //Assigning Error
                return View("UpdateBlog", cblog); //Rendering Update Blog Page
            }
        }
        public ViewResult DeleteBlog(int id)
        {
            if (!ModelState.IsValid) //Checking Model Page
                return View(); //Rendering Delete Blog Page
            if (!HttpContext.Request.Cookies.ContainsKey("login_id")) //Checking is logged in
                return View("Login"); //Rendering Login 
            try //Exception Handling
            {
                CRUD crud = new CRUD(); //Opening Database
                if (crud.deleteBlog(id)) //Deleting Blog and checking is it deleted
                {
                    crud.closeDB(); //Closing Database
                    crud = new CRUD(); //Opening Database
                    List<Blog> blogs = crud.getBlogs(); //Getting Blogs from database
                    crud.closeDB(); //Closing Database
                    return View("Home", blogs); //Rendering Home Page
                }
                crud.closeDB(); //Closing Database
                return View("Error", "No blog exists"); //Rendering Error Page
            }
            catch (Exception)
            {
                return View("Error", "Something went wrong"); //Rendering Error Page
            }
        }
        [HttpGet]
        public ViewResult Profile()
        {
            if (!HttpContext.Request.Cookies.ContainsKey("login_id")) //Checking is User logged in
                return View("Login"); //Rendering Login Page
            try //Exception Handling
            {
                CRUD crud = new CRUD(); //Opening Database
                Profile prof = crud.getUserProfile(Convert.ToInt32(HttpContext.Request.Cookies["login_id"])); //Getting User Profile Info
                if (prof !=null) //Checking is User Profile Info successfully received
                {
                    crud.closeDB(); //Closing Database
                    return View("Profile", prof); //Rendering Profile Page
                }
                crud.closeDB(); //Closing Database
                return View("Error", "No user exists"); //Rendering Error Page
            }
            catch (Exception)
            {
                return View("Error", "Something went wrong"); //Rendering Error Page
            }
        }
        [HttpPost]
        public async Task<ViewResult> ProfileAsync(Profile prof)
        {
            if (!HttpContext.Request.Cookies.ContainsKey("login_id")) //Checking is User logged in
                return View("Login"); //Rendering Login Page
            if (!ModelState.IsValid) //Checking Model State
            {
                return View("Profile", prof); //Rendering Profile Page
            }
            try //Exception Handing
            {
                CRUD crud = new CRUD(); //Opening Database
                if (!crud.validatePass(Convert.ToInt32(HttpContext.Request.Cookies["login_id"]), prof.Password)) //Validating Old Password
                {
                    crud.closeDB(); //Closing Database
                    prof.PassErr = "Old Password does not matched"; //Assigning Error
                    return View("Profile", prof); //Rendering Profile
                }
                crud.closeDB(); //Closing Database
            }
            catch (Exception)
            {
                prof.Error = "Something went wrong"; //Assigning Error Page
                return View("Profile", prof); //Rendering Profile Page
            }
            string pass = prof.NewPassword; //Assigning new password
            if(pass != null) //Checking is User entered new password
            {
                if (pass.Length < 8) //Cecking is New Password's length less than 8 characters or not
                {
                    prof.NPassErr = "Password must contain at least 8 characters"; //Assigning New Password Error
                    return View("Profile", prof); //Rendering Profile Page
                }
            }
            else
                pass = prof.Password; //Assigning Old Password
            string photo = prof.UserImage; //Assigning Image Url
            if(prof.Photo != null) //Checking is url exists
            {
                string imgName = prof.Photo.FileName.ToLower(); //Changing File Name in lower case
                if ((imgName.Length > 4 && imgName.IndexOf(".png") != imgName.Length - 4 && imgName.IndexOf(".jpg") != imgName.Length - 4 && imgName.IndexOf(".jpeg") != imgName.Length - 5)) //Checking is file type is jpg/png/jpeg
                {
                    prof.ExtErr = "Invalid Image Format"; //Assigning Extension Error
                    return View("Profile", prof); //Rendering Profile Page
                }
                string url = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/", prof.Username + prof.Photo.FileName); //Getting Absolute Path to store Image
                Stream fileStream = new FileStream(url, FileMode.Create); //Creating FileStream Object
                await prof.Photo.CopyToAsync(fileStream); //Copying image in Async way
                photo = $"/images/{prof.Username + prof.Photo.FileName}"; //Assigning img src path
            }
            try //Exception Handling
            {
                CRUD crud = new CRUD(); //Opening Database
                if (crud.updateProfile(new User(Convert.ToInt32(HttpContext.Request.Cookies["login_id"]), prof.Username, prof.Email, pass, photo))) //Updating User and checking is it successfully updated
                {
                    crud.closeDB(); //Closing Database
                    crud = new CRUD(); //Opening Database
                    List<Blog> blogs = crud.getBlogs(); //Getting blogs from database
                    if (blogs != null) //Checking are blogs successfully received
                    {
                        crud.closeDB(); //Closing Database
                        return View("Home", blogs); //Rendering Home Page
                    }
                    crud.closeDB(); //Closing Database
                }
                else crud.closeDB(); //Closing database
                prof.Error = "User already exist with same Username or Email"; //Assigning Error
                return View("Profile", prof); //Rendering Profile Page
            }
            catch (Exception)
            {
                prof.Error = "Something went wrong"; //Assigning Error
                return View("Profile", prof); //Rendering Profile
            }
        }
        [HttpGet]
        public ViewResult AdminAuth()
        {
            return View("AdminAuth"); //Rendering Admin Login Page
        }
        [HttpGet]
        public ViewResult AdminDashboard()
        {
            if (!HttpContext.Request.Cookies.ContainsKey("admin_id")) //Checking is admin logged in
                return View("AdminAuth"); //Rendering Admin Page
            try //Exception Handling
            {
                CRUD crud = new CRUD(); //Opening Database
                List<User> users = crud.getUsers(); //Getting Users from database
                crud.closeDB(); //Closing database
                return View("AdminDashboard", users); //Rendering Admin Dashboard Page
            }
            catch (Exception)
            {
                return View("Error", "Something went wrong"); //Rendering Error Page
            }
        }
        [HttpPost]
        public ViewResult AdminDashboard(Login login)
        {
            if (HttpContext.Request.Cookies.ContainsKey("login_id")) //Checking is User logged in
                HttpContext.Response.Cookies.Delete("login_id"); //Deleing Cookie containing user id
            if (!ModelState.IsValid) //Checking Model State
                return View("AdminAuth"); //Rendering Admin Login Page
            try //Exception Handling
            {
                CRUD crud = new CRUD(); //Opening Database
                int id = crud.adminAuth(login.Username, login.Password); //Validating Admin
                crud.closeDB(); //Closing database
                if (id != -1) //Checking is admin credentials valid
                {
                    crud = new CRUD(); //Opening Database
                    List<User> users = crud.getUsers(); //Getting User list from database
                    crud.closeDB(); //Closing Database
                    if (!HttpContext.Request.Cookies.ContainsKey("admin_id")) //Checking is admin logged in
                        HttpContext.Response.Cookies.Append("admin_id", id.ToString()); //Creating Cookie containing admin id
                    return View("AdminDashBoard", users); //Rendering Admin Dashboard
                }
                login.Error = "Invalid Username or Password"; //Assigning Error
                return View("Login", login); //Rendering Login Page
            }
            catch (Exception)
            {
                login.Error = "Something went wrong"; //Assigning Error
                return View("Login", login); //Rendering Login Page
            }
        }
        [HttpGet]
        public ViewResult EditUser(int id)
        {
            if (!HttpContext.Request.Cookies.ContainsKey("admin_id")) //Checking is admin logged in
                return View("AdminAuth"); //Rendering Admin Login Page
            try //Exception Handling
            {
                CRUD crud = new CRUD(); //Opening Database
                EUser user = crud.getUser(id); //Getting user from database whose is in url
                if (user != null) //Checking is user received successfully
                {
                    crud.closeDB(); //Closing Database
                    return View("EditUser", user); //Rendering Edit User Page
                }
                crud.closeDB(); //Closing Database
                return View("Error", "No user exists"); //Rendering Error Page
            }
            catch (Exception)
            {
                return View("Error", "Something went wrong"); //Rendering Error Page
            }
        }
        [HttpPost]
        public async Task<ViewResult> EditUserAsync(EUser user)
        {
            if (!HttpContext.Request.Cookies.ContainsKey("admin_id")) //Checking is Admin Login
                return View("AdminAuth"); //Rendering Admin Login Page
            if (!ModelState.IsValid) //Checking Model State
            {
                return View("EditUser", user); //Rendering Edit User Page
            }
            string pass = user.Password; //Assigning password
            if (pass != null) //Checking is User entered new password
            {
                if (pass.Length < 8) //Cecking is Password's length less than 8 characters or not
                {
                    user.PassErr = "Password must contain at least 8 characters"; //Assigning Password Error
                    return View("EditUser", user); //Rendering Edit User Page
                }
            }
            string photo = user.UserImage; //Assigning Image Url
            if (user.Photo != null) //Checking is url exists
            {
                string imgName = user.Photo.FileName.ToLower(); //Changing File Name in lower case
                if ((imgName.Length > 4 && imgName.IndexOf(".png") != imgName.Length - 4 && imgName.IndexOf(".jpg") != imgName.Length - 4 && imgName.IndexOf(".jpeg") != imgName.Length - 5)) //Checking is file type is jpg/png/jpeg
                {
                    user.ExtErr = "Invalid Image Format"; //Assigning Extension Error
                    return View("EditUser", user); //Rendering Edit User Page
                }
                string url = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/", user.Username + user.Photo.FileName); //Getting Absolute Path to store Image
                Stream fileStream = new FileStream(url, FileMode.Create); //Creating FileStream Object
                await user.Photo.CopyToAsync(fileStream); //Copying image in Async way
                photo = $"/images/{user.Username + user.Photo.FileName}"; //Assigning img src path
            }
            try //Exception Handling
            {
                CRUD crud = new CRUD(); //Opening Database
                if (crud.updateProfile(new User(user.Id, user.Username, user.Email, user.Password, photo))) //Updating User and checking is user updated
                {
                    crud.closeDB(); //Closing Database
                    crud = new CRUD(); //Opening Database
                    List<User> users = crud.getUsers(); //Getting User from database
                    crud.closeDB(); //Closing Database
                    return View("AdminDashBoard", users); //Rendering Admin Dashboard Page
                }
                else crud.closeDB(); //Closing Database
                user.Error = "User already exist with same Username or Email"; //Assigning Error
                return View("EditUser", user); //Rendering Edit User Page
            }
            catch (Exception)
            {
                user.Error = "Something went wrong"; //Assigning Error
                return View("EditUser", user); //Rendering Edit User Page
            }
        }
        public ViewResult DeleteUser(int id)
        {
            if (!HttpContext.Request.Cookies.ContainsKey("admin_id")) //Checking is Admin logged in
                return View("AdminAuth"); //Rendering Admin Login Page
            try //Exception Handling
            {
                CRUD crud = new CRUD(); //Opening Database
                if (crud.deleteUser(id)) //Deleting User and checking is it successfully deleted
                {
                    crud.closeDB(); //Closing Database
                    crud = new CRUD(); //Opening Database
                    List<User> users = crud.getUsers(); //Getting Users from database
                    crud.closeDB(); //Closing Database
                    return View("AdminDashBoard", users); //Rendering Admin Dashboard Page
                }
                crud.closeDB(); //Closing Database
                return View("Error", "No user exists"); //Rendering Error Page
            }
            catch (Exception)
            {
                return View("Error", "Something went wrong"); //Rendering Erro Page 
            }
        }
        public ViewResult About()
        {
            if (!HttpContext.Request.Cookies.ContainsKey("login_id") && !HttpContext.Request.Cookies.ContainsKey("admin_id")) //Checking is User or admin logged in
                return View("Login"); //Rendering Login Page
            return View(); //Rendering About Page
        }
    }
}