using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vehicle_Selling_Site.Models;

namespace Vehicle_Selling_Site.Controllers
{
    public class HomeController : Controller
    {
       private SiteDatabaseEntities DatabaseConnection = new SiteDatabaseEntities(); // a connection to the database

        //the home page:
        public ActionResult HomePage()
        {
            if (Request.Browser.IsMobileDevice)
            {
                return View("Mobile_Home");
            }
            return View();
        }

        //the home page (for mobile):
        public ActionResult Mobile_Home()
        {
            return View();
        }


        //the registration page:
        public ActionResult Registration()
        {
            if (Request.Browser.IsMobileDevice)
            {
                return View("Mobile_Registration");
            }
            return View();
        }

        //after the user fills the registration:
        [HttpPost]
        public ActionResult Registration(string Name, string Password, string Email, string Gender ,string UserName, string DateofBirth ,HttpPostedFileBase photo)
        {
            if (ModelState.IsValid)
            {
                string path = null; 
                if (photo != null && photo.ContentLength > 0) //if the user has uploaded a photo
                {
                    path = Path.Combine(Server.MapPath("~/Images"), // this is the path the image is saved at, Images folder
                                               Path.GetFileName(photo.FileName));
                    photo.SaveAs(path);
                    path = "/Images/" + photo.FileName;             // This is the path saved in the Database
                }
                else //if the user hasn't uploaded a photo
                {
                    if (Gender == "Male") //if the is a male
                    {
                        path = "/Images/MaleNoImage.jpg"; // a male's "no profile picture" image will be attached to the profile from the images folder
                    }
                    if (Gender == "Female") //if the is a female
                    {
                        path = "/Images/FemaleNoImage.jpg"; // a female's "no profile picture" image will be attached to the profile from the images folder
                    }
                }
                DatabaseConnection.UserTables.Add(new UserTable
                {
                    //***************** set the user's parameters in the database:
                    //(the left ones are the database parameters and the right ones are parameters recieved from the View)
                    Name = Name,
                    Password = Password,
                    Email = Email,
                    Gender = Gender,
                    UserName = UserName,
                    DateofBirth = DateofBirth,
                    Photo = path,
                    Type = "User" //the user will be marked as a regular user
                });
                try
                {
                    DatabaseConnection.SaveChanges();
                }
                catch (Exception) //if the registration fails
                {
                    //check if the entered username already exists:
                    UserTable ValidateUserName = DatabaseConnection.UserTables.Where(user => user.UserName == UserName).SingleOrDefault();
                    //check if the entered email already exists:
                    UserTable ValidateEmail = DatabaseConnection.UserTables.Where(user => user.Email == Email).SingleOrDefault();
                    if (ValidateUserName != null) //if the entered username already exists
                    {
                        //an error message will be sent to the view
                        ModelState.AddModelError("", "UserName already exists");
                    }

                    if (ValidateEmail != null) //if the entered email already exists
                    {
                        //an error message will be sent to the view
                        ModelState.AddModelError("", "Email already exists");
                    }
                    return View();
                }
            }
            //if the registration is successful, redirect the user to the home page:
            if (Request.Browser.IsMobileDevice)
            {
                return RedirectToAction("Mobile_Home");
            }
            return RedirectToAction("HomePage");
        }

        // the "contact us" page:
        public ActionResult Contact()
        {
            if (Request.Browser.IsMobileDevice)
            {
                return View("Mobile_Contact");
            }
            return View();
        }

        //after a message has been sent from the "contact us page":
        [HttpPost]
        public ActionResult Contact(string Subject, string Message)
        {
            DatabaseConnection.ContactTables.Add(new ContactTable
            {
                // add a new message to the database with the subject and text that have been sent from the view:
                //(the left ones are the database parameters and the right ones are parameters recieved from the View)
                Subject=Subject,
                Message=Message
            });
            DatabaseConnection.SaveChanges();
            if (Request.Browser.IsMobileDevice)
            {
                return RedirectToAction("Mobile_Home");
            }
            return RedirectToAction("HomePage");
        }

        //the page that shows the company's branches:
        public ActionResult Branches()
        {
            return View();
        }

        //the page that shows the company's branches (for mobile):
        public ActionResult Mobile_Branches()
        {
            return View();
        }

        // the "contact us" page (for mobile):
        public ActionResult Mobile_Contact()
        {
            return View();
        }
    }
}