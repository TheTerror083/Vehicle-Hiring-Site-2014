using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Web.Security;
using Vehicle_Selling_Site.Models;
using System.Collections.Generic;

namespace Vehicle_Selling_Site.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        public SiteDatabaseEntities DatabaseConnection = new SiteDatabaseEntities(); // a connection to the database
        List<Hired_Vehicles_History> HiredVehicles = new List<Hired_Vehicles_History>(); // a list of all vehicles hired by the user


        //the login page:
        [AllowAnonymous]
        public ActionResult Login()
        {
            if (Request.Browser.IsMobileDevice)
            {
                return View("Mobile_Login");
            }
            return View();
        }

        //when the user tries to log in:
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string Email, string Password)
        {
            //find the user by the email recieved from the view in the database:
            UserTable User = DatabaseConnection.UserTables.Where(user => user.Email == Email).SingleOrDefault();
            if (User != null && User.Password == Password) // if the user has been found and the entered password matches the user's password
            {
                //create a non persistent authentication cookie in the user's device with the user's type attached to it to match the user's authentication level
                FormsAuthentication.SetAuthCookie(User.Type , false);
                if (Request.Browser.IsMobileDevice)
                {
                    return View("Mobile_RedirectAfterLogin", User);
                }
                return View("RedirectAfterLogin", User);
            }
            //if the user fails to log in, an error will be sent ot the view:
            ModelState.AddModelError("", "Login Failed");
            if (Request.Browser.IsMobileDevice)
            {
                return View("Mobile_Login");
            }
            return View();
        }

        //the user's personal zone:
        [HttpPost]
        public ActionResult UserZone(string ConnectedUserName)
        {
            //find the connected user by username in the database and send the user's parameters to the view:
            UserTable ConnectedUser = DatabaseConnection.UserTables.Where(user => user.UserName == ConnectedUserName).SingleOrDefault();
            if (Request.Browser.IsMobileDevice)
            {
                return View("Mobile_UserZone", ConnectedUser);
            }
            return View(ConnectedUser);
        }

        //this page appears to the user after logging in:
        public ActionResult RedirectAfterLogin()
        {
            return View();
        }

        //this page displays all the vehicles that have been hired by the user:
        [HttpPost]
        public ActionResult VehiclesHiredByUser(string ConnectedUser) 
        {
            //load each vehicle hired by the connected user (found by username) into a list and send it to the view:
            foreach (Hired_Vehicles_History Vehicle in DatabaseConnection.Hired_Vehicles_History)
            {
                if (Vehicle.Hired_By_User == ConnectedUser)
                {
                    HiredVehicles.Add(Vehicle);
                }
            }
            if (Request.Browser.IsMobileDevice)
            {
                return View("Mobile_VehiclesHiredByUser", HiredVehicles);
            }
            return View(HiredVehicles);
        }

        //when the user clicks on the log out button on the layout:
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut(); //log out and remove the authentication cookie from the user's device
            //****************** redirect the user to the Home Page:
            if (Request.Browser.IsMobileDevice)
            {
                return RedirectToAction("Mobile_Home", "Home");
            }
            return RedirectToAction("HomePage", "Home");
        }
    }
}