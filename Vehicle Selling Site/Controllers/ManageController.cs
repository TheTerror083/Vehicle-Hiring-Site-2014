using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Vehicle_Selling_Site.Models;
using System.IO;
using System.Collections.Generic;

namespace Vehicle_Selling_Site.Controllers
{
    [Authorize(Users = "Admin")] //allow access only to users of type "Admin"
    public class ManageController : Controller
    {
        public List<VehicleTypeTable> VehicleTypeList = new List<VehicleTypeTable>();   //the list of vehicle types
        public AddVehicleClass AddVehicle = new AddVehicleClass();                      //contains all vehicle parameters and a list of vehicle types
        public List<VehicleTable> Vehicles = new List<VehicleTable>();                  //the list of vehicles
        public List<UserTable> Users = new List<UserTable>();                           //the list of users
        public List<HiredVehiclesTable> HiredVehicles = new List<HiredVehiclesTable>(); //the list of orders (currently hired vehicles)

        SiteDatabaseEntities DatabaseConnection = new SiteDatabaseEntities(); // add a connection to the site's database
        //this is the page that allows to add new vehicles:
        public ActionResult AddNewVehicle()
        {
            //load each vehicle type from the database and send it to the view:
            foreach (VehicleTypeTable item in DatabaseConnection.VehicleTypeTables)
            {
                AddVehicle.VehicleTypes.Add(item);
            }
            if (Request.Browser.IsMobileDevice)
            {
                return View("Mobile_AddNewVehicle", AddVehicle);
            }
            return View(AddVehicle);
        }

        //this is the page that allows to add new vehicles:
        [HttpPost]
        public ActionResult AddNewVehicle(HttpPostedFileBase photo, string Name, string Transmission, int Daily_Cost, int Late_Cost, string Branch, int Mileage,
            string Manufacturer,bool Available_for_Hire,bool Fixed_for_Hire, string Hiring_Period_Start, string Hiring_Period_End,string Model,string Number,
            string Year, string VehicleType)
        {
            if (ModelState.IsValid)
            {
                string path = Path.Combine(Server.MapPath("~/Vehicles"), // create a path to save the photo uploaded by the manager
                                           Path.GetFileName(photo.FileName)); // and cobine it with the photo's name 
                photo.SaveAs(path); // save the photo in the generated path
                DatabaseConnection.VehicleTables.Add(new VehicleTable
                {
                    //*************************** add a new vehicle to the database with the parameters recieved from the view:
                    //(the left ones are the database parameters and the right ones are parameters recieved from the View)
                    Transmission = Transmission,
                    Daily_Cost = Daily_Cost,
                    Late_Cost = Late_Cost,
                    Branch = Branch,
                    Mileage = Mileage,
                    Manufacturer = Manufacturer,
                    Available_for_Hire = Available_for_Hire,
                    Fixed_for_Hire = Fixed_for_Hire,
                    Hiring_Period_Start = Hiring_Period_Start,
                    Hiring_Period_End = Hiring_Period_End,
                    Model = Model,
                    Number = Number,
                    Year = Year,
                    Name = Name,
                    Vehicle_Type=VehicleType,
                    Image = "/Vehicles/"+photo.FileName //set the image's path in the database to allow using it in other views
                    //(giving the full physical path would NOT allow to display the image in the view)
                });
                try
                {
                DatabaseConnection.SaveChanges();
                }
                catch (Exception)
                {
                    // check if the entered vehicle name already exists:
                    VehicleTable CheckVehicleName = DatabaseConnection.VehicleTables.Where(vehicle => vehicle.Name == Name).SingleOrDefault();
                    // check if the entered vehicle number already exists:
                    VehicleTable CheckVehicleNumber = DatabaseConnection.VehicleTables.Where(vehicle => vehicle.Number == Number).SingleOrDefault();
                    if (CheckVehicleName != null)
                    {
                        //if the entered vehicle name exists, an error message will be sent to the view to notify the manager
                        ModelState.AddModelError("", "Vehicle Name already exists");
                    }
                    if (CheckVehicleNumber != null)
                    {
                        //if the entered vehicle number exists, an error message will be sent to the view to notify the manager
                        ModelState.AddModelError("", "Vehicle Number already exists");
                    }
                    foreach (VehicleTypeTable item in DatabaseConnection.VehicleTypeTables)
                    {
                        //load each vehicle type from the database and send it to the view:
                        //(done again due to the manager being redirected to the page that adds vehicles again if an error has occured)
                        AddVehicle.VehicleTypes.Add(item);
                    }
                    // display the page that allows to add new vehicles
                    if (Request.Browser.IsMobileDevice)
                    {
                        return View("Mobile_AddNewVehicle", AddVehicle);
                    }
                    return View(AddVehicle);
                }
            }
            // if the vehicle has been successfully created, the manager will be redirected to the manager's zone:
            if (Request.Browser.IsMobileDevice)
            {
                return RedirectToAction("Mobile_ManagerPage");
            }
            return RedirectToAction("ManagerPage");
        }
        // this is the page that displays the list of vehicles to choose one to update:
        public ActionResult UpdateVehicleList()
        {
            //load each vehicle from the database to a list and send it to thew view:
            foreach (VehicleTable item in DatabaseConnection.VehicleTables)
            {
                Vehicles.Add(item);
            }
            if (Request.Browser.IsMobileDevice)
            {
                return View("Mobile_UpdateVehicleList" ,Vehicles);
            }
            return View(Vehicles);
        }

        // this is the page that displays the list of vehicles to choose one to update:
        [HttpPost]
        public ActionResult UpdateVehicleList(string SelectedVehicle)
        {
            // Find the selected Vehicle in the Database by its name:
            VehicleTable UpdatedVehicle = DatabaseConnection.VehicleTables.Where(vehicle => vehicle.Name == SelectedVehicle).SingleOrDefault();
            //*************************************** Send the selected Vehicle's attributes to the html page:
            //(the left ones are the "AddVehicleClass" Model parameters and the right ones are the selected vehicle's parameters)
            AddVehicle.Name = UpdatedVehicle.Name;
            AddVehicle.Available_for_Hire = UpdatedVehicle.Available_for_Hire;
            AddVehicle.Branch = UpdatedVehicle.Branch;
            AddVehicle.Daily_Cost = UpdatedVehicle.Daily_Cost;
            AddVehicle.Fixed_for_Hire = UpdatedVehicle.Fixed_for_Hire;
            AddVehicle.Hiring_Period_End = UpdatedVehicle.Hiring_Period_End;
            AddVehicle.Hiring_Period_Start = UpdatedVehicle.Hiring_Period_Start;
            AddVehicle.Image = UpdatedVehicle.Image;
            AddVehicle.Late_Cost = UpdatedVehicle.Late_Cost;
            AddVehicle.Manufacturer = UpdatedVehicle.Manufacturer;
            AddVehicle.Mileage = UpdatedVehicle.Mileage;
            AddVehicle.Model = UpdatedVehicle.Model;
            AddVehicle.Number = UpdatedVehicle.Number;
            AddVehicle.Transmission = UpdatedVehicle.Transmission;
            AddVehicle.Vehicle_Type = UpdatedVehicle.Vehicle_Type;
            AddVehicle.Year = UpdatedVehicle.Year;
            //****************************************** load the available Vehicle Types from the Database to a list:
            foreach (VehicleTypeTable item in DatabaseConnection.VehicleTypeTables)
            {
                AddVehicle.VehicleTypes.Add(item);
            }
            // send the list of vehicles types and the selected vehicle's parameters to the page that allows editing vehicles:
            if (Request.Browser.IsMobileDevice)
            {
               return View("Mobile_UpdateVehicle", AddVehicle);
            }
            return View("UpdateVehicle", AddVehicle);
        }

        // this is the page that allows editing vehicles:
        [HttpPost]
        public ActionResult UpdateVehicle(HttpPostedFileBase photo,string VehicleName, string Name, string Transmission, int Daily_Cost, int Late_Cost, string Branch, int Mileage,
        string Manufacturer, bool Available_for_Hire, bool Fixed_for_Hire, string Hiring_Period_Start, string Hiring_Period_End, string Model, string Number, string Year,
        string VehicleType)
        {
            if (ModelState.IsValid)
            {
                string path = Path.Combine(Server.MapPath("~/Vehicles"), // create a path to save the photo uploaded by the manager
                                           Path.GetFileName(photo.FileName)); // and cobine it with the photo's name 
                photo.SaveAs(path); // save the photo in the generated path

                //find the selected vehicle by name in the database:
                VehicleTable UpdatedVehicle = DatabaseConnection.VehicleTables.Where(vehicle => vehicle.Name == VehicleName).SingleOrDefault();
                //check if the vehicle is currently hired by a user:
                HiredVehiclesTable CheckIfHired = DatabaseConnection.HiredVehiclesTables.Where(hiredv => hiredv.HiredVehicleName == VehicleName).SingleOrDefault();
                if (CheckIfHired != null) //if the vehicle is currently hired
                {
                    if (Available_for_Hire == true) // if the manager attempts to mark the vehicle as Available while it is hired
                    {
                        // deny updating the vehicle and send an error to the view:
                        ModelState.AddModelError("", "The vehicle cannot be marked as available while it is still hired");
                        #region Re-acquire the vehicle's parameters and all vehicle types and send them to the view
                        //*************************************** Send the selected Vehicle's attributes to the view:
                        //(the left ones are the "AddVehicleClass" Model parameters and the right ones are the selected vehicle's parameters)
                        AddVehicle.Name = UpdatedVehicle.Name;
                        AddVehicle.Available_for_Hire = UpdatedVehicle.Available_for_Hire;
                        AddVehicle.Branch = UpdatedVehicle.Branch;
                        AddVehicle.Daily_Cost = UpdatedVehicle.Daily_Cost;
                        AddVehicle.Fixed_for_Hire = UpdatedVehicle.Fixed_for_Hire;
                        AddVehicle.Hiring_Period_End = UpdatedVehicle.Hiring_Period_End;
                        AddVehicle.Hiring_Period_Start = UpdatedVehicle.Hiring_Period_Start;
                        AddVehicle.Image = UpdatedVehicle.Image;
                        AddVehicle.Late_Cost = UpdatedVehicle.Late_Cost;
                        AddVehicle.Manufacturer = UpdatedVehicle.Manufacturer;
                        AddVehicle.Mileage = UpdatedVehicle.Mileage;
                        AddVehicle.Model = UpdatedVehicle.Model;
                        AddVehicle.Number = UpdatedVehicle.Number;
                        AddVehicle.Transmission = UpdatedVehicle.Transmission;
                        AddVehicle.Vehicle_Type = UpdatedVehicle.Vehicle_Type;
                        AddVehicle.Year = UpdatedVehicle.Year;
                        //****************************************** load the available Vehicle Types from the Database to a list:
                        foreach (VehicleTypeTable item in DatabaseConnection.VehicleTypeTables)
                        {
                            AddVehicle.VehicleTypes.Add(item);
                        }
                        // send the list of vehicles types and the selected vehicle's parameters to the page that allows editing vehicles: 
                        #endregion
                        if (Request.Browser.IsMobileDevice)
                        {
                            return View("Mobile_UpdateVehicle", AddVehicle);
                        }
                        return View(AddVehicle);
                    }
                    // remove the original order from the database:
                    DatabaseConnection.HiredVehiclesTables.Remove(CheckIfHired);
                    // replace the original order with a new order that contains the updated parameters from the view:
                    // other parameters like the order's daily cost and selected hiring period will be retrieved from the original order
                    DatabaseConnection.HiredVehiclesTables.Add(new HiredVehiclesTable
                    {
                        //(the left ones are the database parameters and the right ones are parameters recieved from the View and the original order)
                        //************ parameters recieved from the view:
                        HiredVehicleName = Name,
                        Image = "/Vehicles/" + photo.FileName,
                        Number = Number,
                        //************ parameters retrieved from the original order:
                        Hired_By_User = CheckIfHired.Hired_By_User,
                        Start_of_Hiring = CheckIfHired.Start_of_Hiring,
                        End_of_Hiring = CheckIfHired.End_of_Hiring,
                        Daily_Cost = CheckIfHired.Daily_Cost,
                        Late_Cost = CheckIfHired.Late_Cost,
                        Total_Cost = CheckIfHired.Total_Cost
                    });
                }
                // remove the selected vehicle from the database:
                DatabaseConnection.VehicleTables.Remove(UpdatedVehicle);
                // and add a new vehicle to replace the original one using the parameters recieved from the view:
                DatabaseConnection.VehicleTables.Add (new VehicleTable
                {
                    //(the left ones are the database parameters and the right ones are parameters recieved from the View)
                    Transmission = Transmission,
                    Daily_Cost = Daily_Cost,
                    Late_Cost = Late_Cost,
                    Branch = Branch,
                    Mileage = Mileage,
                    Manufacturer = Manufacturer,
                    Available_for_Hire = Available_for_Hire,
                    Fixed_for_Hire = Fixed_for_Hire,
                    Hiring_Period_Start = Hiring_Period_Start,
                    Hiring_Period_End = Hiring_Period_End,
                    Model = Model,
                    Number = Number,
                    Year = Year,
                    Name = Name,
                    Vehicle_Type = VehicleType,
                    Image = "/Vehicles/" + photo.FileName
                });
                try
                {
                    DatabaseConnection.SaveChanges();
                }
                catch (Exception)
                {
                    // check if the entered vehicle name already exists in the database:
                    VehicleTable CheckVehicleName = DatabaseConnection.VehicleTables.Where(vehicle => vehicle.Name == Name
                        && vehicle.Name != UpdatedVehicle.Name).SingleOrDefault(); //ignore the selected vehicle's original name to allow updating it
                    // check if the entered vehicle number already exists in the database:
                    VehicleTable CheckVehicleNumber = DatabaseConnection.VehicleTables.Where(vehicle => vehicle.Number == Number
                        && vehicle.Number != UpdatedVehicle.Number).SingleOrDefault(); //ignore the selected vehicle's original number to allow updating it
                    if (CheckVehicleName != null) // if the entered name already exists
                    {
                        //an error message will be sent to the view to notify the manager
                        ModelState.AddModelError("", "Vehicle Name already exists");
                    }
                    if (CheckVehicleNumber != null) // if the entered number already exists
                    {
                        //an error message will be sent to the view to notify the manager
                        ModelState.AddModelError("", "Vehicle Number already exists");
                    }
                    //*************************************** Send the selected Vehicle's attributes to the view again:
                    //(the left ones are the "AddVehicleClass" Model parameters and the right ones are the selected vehicle's parameters)
                    AddVehicle.Name = UpdatedVehicle.Name;
                    AddVehicle.Available_for_Hire = UpdatedVehicle.Available_for_Hire;
                    AddVehicle.Branch = UpdatedVehicle.Branch;
                    AddVehicle.Daily_Cost = UpdatedVehicle.Daily_Cost;
                    AddVehicle.Fixed_for_Hire = UpdatedVehicle.Fixed_for_Hire;
                    AddVehicle.Hiring_Period_End = UpdatedVehicle.Hiring_Period_End;
                    AddVehicle.Hiring_Period_Start = UpdatedVehicle.Hiring_Period_Start;
                    AddVehicle.Image = UpdatedVehicle.Image;
                    AddVehicle.Late_Cost = UpdatedVehicle.Late_Cost;
                    AddVehicle.Manufacturer = UpdatedVehicle.Manufacturer;
                    AddVehicle.Mileage = UpdatedVehicle.Mileage;
                    AddVehicle.Model = UpdatedVehicle.Model;
                    AddVehicle.Number = UpdatedVehicle.Number;
                    AddVehicle.Transmission = UpdatedVehicle.Transmission;
                    AddVehicle.Vehicle_Type = UpdatedVehicle.Vehicle_Type;
                    AddVehicle.Year = UpdatedVehicle.Year;
                    //****************************************** load the available Vehicle Types from the Database to a list:
                    foreach (VehicleTypeTable item in DatabaseConnection.VehicleTypeTables)
                    {
                        AddVehicle.VehicleTypes.Add(item);
                    }
                    // send the list of vehicles types and the selected vehicle's parameters to the page that allows editing vehicles:
                    if (Request.Browser.IsMobileDevice)
                    {
                        return View("Mobile_UpdateVehicle", AddVehicle);
                    }
                    return View(AddVehicle);
                }
            }
            if (Request.Browser.IsMobileDevice)
            {
                return RedirectToAction("Mobile_ManagerPage");
            }
            return RedirectToAction("ManagerPage"); // if the vehicle has been successfully updated redirect the manager to the manager's zone
        }

        // this is the page that allows removing vehicles:
        public ActionResult RemoveVehicle()
        {
            //load each vehicle from the database to a list and send it to the view:
            foreach (VehicleTable item in DatabaseConnection.VehicleTables)
            {
                Vehicles.Add(item);
            }
            if (Request.Browser.IsMobileDevice)
            {
                 return View("Mobile_RemoveVehicle", Vehicles);
            }
            return View(Vehicles);
        }

        // this is the page that allows removing vehicles:
        [HttpPost]
        public ActionResult RemoveVehicle(string carName)
        {
            if (ModelState.IsValid)
            {
                // find the selected vehicle by name in the database:
                VehicleTable SelectedVehicle = DatabaseConnection.VehicleTables.Where(vehicle => vehicle.Name == carName).SingleOrDefault(); // find the vehicle's name in the database
                if (SelectedVehicle.Available_for_Hire == true) // if the vehicle is marked as available for hire
                {
                    // look for the vehicle's name in the list of orders:
                    HiredVehiclesTable Check_if_Hired = DatabaseConnection.HiredVehiclesTables.Where(vehicle => vehicle.HiredVehicleName == carName).SingleOrDefault();
                    if (Check_if_Hired == null) // if the vehicle is currently not hired
                    {
                        DatabaseConnection.VehicleTables.Remove(SelectedVehicle); // remove the vehicle from the database
                        DatabaseConnection.SaveChanges();
                    }
                }
            }
            if (Request.Browser.IsMobileDevice)
            {
                return RedirectToAction("Mobile_ManagerPage");
            }
            return RedirectToAction("ManagerPage");
        }
        // this is the page that allows the manager to register users:
        public ActionResult ManagerRegistration()
        {
            if (Request.Browser.IsMobileDevice)
            {
                return View("Mobile_ManagerRegistration");
            }
            return View();
        }
        // this is the page that allows the manager to register users:
        [HttpPost]
        public ActionResult ManagerRegistration(string Name, string Password, string Email, string Gender, string UserName, string DateofBirth,
            HttpPostedFileBase photo, string UserType)
        {
            if (ModelState.IsValid)
            {
                string path = null;
                if (photo != null && photo.ContentLength > 0) //if the user has uploaded a photo
                {
                    path = Path.Combine(Server.MapPath("~/Images"),
                                               Path.GetFileName(photo.FileName)); //set the photo's path to the Images folder
                    photo.SaveAs(path);                                  // save the photo to the Images folder
                    path = "/Images/" + photo.FileName;             // This is the path saved in the Database to allow loading the image and display it on the web
                }
                else // if the user hasn't uploaded a photo
                {
                    if (Gender == "Male") //if the user is a male
                    {
                        path = "/Images/MaleNoImage.jpg"; // a male's "no profile picture" image will be attached to the profile from the images folder
                    }
                    if (Gender == "Female") //if the user is a female
                    {
                        path = "/Images/FemaleNoImage.jpg";  // a female's "no profile picture" image will be attached to the profile from the images folder
                    }
                }
                //Note: the database accepts a null entrance for the photo,
                //but if the user doesn't upload a photo a "NoImage" photo will always be attached to his\her profile to improve the interface
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
                    Type = UserType
                });
                try
                {
                DatabaseConnection.SaveChanges(); // try to update the database
                }
                catch (Exception) // if updating the database fails
                {
                    // check if the entered UserName already exists:
                    UserTable ValidateUserName = DatabaseConnection.UserTables.Where(user => user.UserName == UserName).SingleOrDefault();
                    // check if the entered Email already exists:
                    UserTable ValidateEmail = DatabaseConnection.UserTables.Where(user => user.Email == Email).SingleOrDefault();
                    if (ValidateUserName != null) //if the username already exists
                    {
                        ModelState.AddModelError("", "UserName already exists"); // send error to the view to notify the manager
                    }

                    if (ValidateEmail != null) //if the email already exists
                    {
                        ModelState.AddModelError("", "Email already exists"); // send error to the view to notify the manager
                    }

                    if (Request.Browser.IsMobileDevice)
                    {
                        return View("Mobile_ManagerRegistration");
                    }
                    return View();
                }
            }
            //if the registration is successful, redirect the manager to the manager zone
            if (Request.Browser.IsMobileDevice)
            {
                return RedirectToAction("Mobile_ManagerPage");
            }
            return RedirectToAction("ManagerPage");
        }
        //this is the page that allows to remove users:
        public ActionResult RemoveUser()
        {
            // add each user from the database to a list and send it to the view:
            foreach (UserTable item in DatabaseConnection.UserTables) 
            {
                Users.Add(item);
            }
            if (Request.Browser.IsMobileDevice)
            {
                return View("Mobile_RemoveUser", Users);
            }
            return View(Users);
        }

        //this is the page that allows to remove users:
        [HttpPost]
        public ActionResult RemoveUser(string Username, string AdminName)
        {
            if (ModelState.IsValid)
            {
                if (AdminName == Username) // if the Admin tries to remove the user he's currently logged in to
                {
                    // deny the request and send an error to the view:
                    ModelState.AddModelError("", "You cannot remove the account you are currently logged in with");
                    // add each user from the database to a list and send it to the view:
                    foreach (UserTable item in DatabaseConnection.UserTables)
                    {
                        Users.Add(item);
                    }
                    if (Request.Browser.IsMobileDevice)
                    {
                        return View("Mobile_RemoveUser", Users);
                    }
                    return View(Users);
                }
                UserTable SelectedUser = DatabaseConnection.UserTables.Where(user => user.UserName == Username).SingleOrDefault(); // find the selected user by UserName
                DatabaseConnection.UserTables.Remove(SelectedUser); //remove the user
                DatabaseConnection.SaveChanges();
            }
            if (Request.Browser.IsMobileDevice)
            {
                return RedirectToAction("Mobile_ManagerPage");
            }
            return RedirectToAction("ManagerPage");
        }

        //this is the page that shows all users to choose one to update:
        public ActionResult UpdateUserList()
        {
            // add each user from the database to a list and send it to the view:
            foreach (UserTable item in DatabaseConnection.UserTables)
            {
                Users.Add(item);
            }
            if (Request.Browser.IsMobileDevice)
            {
                return View("Mobile_UpdateUserList", Users);
            }
            return View(Users);
        }

        //this is the page that shows all users to choose one to update:
        [HttpPost]
        public ActionResult UpdateUserList(string SelectedName, string AdminName)
        {
            if (AdminName == SelectedName) // if the Admin tries to update the user he's currently logged in to
            {
                // deny the request and send an error to the view:
                ModelState.AddModelError("", "You cannot edit the account you are currently logged in with");
                // add each user from the database to a list and send it to the view:
                foreach (UserTable item in DatabaseConnection.UserTables)
                {
                    Users.Add(item);
                }
                if (Request.Browser.IsMobileDevice)
                {
                    return View("Mobile_UpdateUserList", Users);
                }
                return View(Users);
            }
            // find the selected user by UserName:
            UserTable UpdatedUser = DatabaseConnection.UserTables.Where(User => User.UserName == SelectedName).SingleOrDefault();
            // send the user's parameters to the update user page and display it to the manager:
            if (Request.Browser.IsMobileDevice)
            {
                return View("Mobile_UpdateUser",UpdatedUser);   
            }
            return View("UpdateUser",UpdatedUser); 
        }

        //this is the page that allows to update users:
        [HttpPost]
        public ActionResult UpdateUser(string ChosenUser, string Name, string Password, string Email, string Gender, string UserName, string DateofBirth,
            HttpPostedFileBase photo, string UserType)
        {
            if (ModelState.IsValid)
            {
                string path = null;
                if (photo != null && photo.ContentLength > 0) //if the user has uploaded a photo
                {
                    path = Path.Combine(Server.MapPath("~/Images"),
                                               Path.GetFileName(photo.FileName)); //set the photo's path to the Images folder
                    photo.SaveAs(path);                                  // save the photo to the Images folder
                    path = "/Images/" + photo.FileName;             // This is the path saved in the Database to allow loading the image and display it on the web
                }
                else // if the user hasn't uploaded a photo
                {
                    if (Gender == "Male") //if the user is a male
                    {
                        path = "/Images/MaleNoImage.jpg"; // a male's "no profile picture" image will be attached to the profile from the images folder
                    }
                    if (Gender == "Female") //if the user is a female
                    {
                        path = "/Images/FemaleNoImage.jpg";  // a female's "no profile picture" image will be attached to the profile from the images folder
                    }
                }
                //Note: the database accepts a null entrance for the photo,
                //but if the user doesn't upload a photo a "NoImage" photo will always be attached to his\her profile to improve the interface
                UserTable SelectedUser = DatabaseConnection.UserTables.Where(user => user.UserName == ChosenUser).SingleOrDefault(); // find the selected user by UserName
                DatabaseConnection.UserTables.Remove(SelectedUser); // remove the selected user from the database
                DatabaseConnection.UserTables.Add(new UserTable    // add a new user instead of the old one:
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
                    Type = UserType
                });
                // check if any vehicle is currently hired by the user:
                foreach (HiredVehiclesTable vehicle in DatabaseConnection.HiredVehiclesTables)
                {
                    if (vehicle.Hired_By_User == ChosenUser) // if the updated user has hired the current vehicle
                    {
                        //replace the original order with a new order with the same parameters while the only difference is the hiring user's username:
                        DatabaseConnection.HiredVehiclesTables.Add(new HiredVehiclesTable
                        {
                            //(the left ones are the database parameters and the right ones are parameters recieved from the View)
                            Daily_Cost = vehicle.Daily_Cost,
                            End_of_Hiring = vehicle.End_of_Hiring,
                            Hired_By_User = UserName, // replace the original username with the updated one
                            HiredVehicleName = vehicle.HiredVehicleName,
                            Image = vehicle.Image,
                            Late_Cost = vehicle.Late_Cost,
                            Number = vehicle.Number,
                            Start_of_Hiring = vehicle.Start_of_Hiring,
                            Total_Cost = vehicle.Total_Cost
                        });
                        DatabaseConnection.HiredVehiclesTables.Remove(vehicle); // remove the original order
                    }
                }
                //check if the user has hired any vehicle in the past: 
                foreach (Hired_Vehicles_History vehicle in DatabaseConnection.Hired_Vehicles_History)
                {
                    if (vehicle.Hired_By_User == ChosenUser) //if the vehicle has been hired by the user
                    {
                        DatabaseConnection.Hired_Vehicles_History.Add(new Hired_Vehicles_History
                        {
                            //(the left ones are the database parameters and the right ones are parameters recieved from the View)
                            Daily_Cost = vehicle.Daily_Cost,
                            End_of_Hiring = vehicle.End_of_Hiring,
                            Hired_By_User = UserName, // replace the original username with the updated one
                            Name = vehicle.Name,
                            Image = vehicle.Image,
                            Number = vehicle.Number,
                            Start_of_Hiring = vehicle.Start_of_Hiring,
                            Total_Cost = vehicle.Total_Cost
                        });
                        DatabaseConnection.Hired_Vehicles_History.Remove(vehicle); //remove the original order from the histroy table
                    }
                }
                try
                {
                DatabaseConnection.SaveChanges();// try to update the database
                }
                catch (Exception) // if updating the database fails
                {
                    // check if the entered UserName already exists:
                    UserTable ValidateUserName = DatabaseConnection.UserTables.Where(user => user.UserName == UserName).SingleOrDefault();
                    // check if the entered Email already exists:
                    UserTable ValidateEmail = DatabaseConnection.UserTables.Where(user => user.Email == Email).SingleOrDefault();
                    if (ValidateUserName != null) //if the username already exists
                    {
                        ModelState.AddModelError("", "UserName already exists"); // send error to the view to notify the manager
                    }

                    if (ValidateEmail != null) //if the email already exists
                    {
                        ModelState.AddModelError("", "Email already exists"); // send error to the view to notify the manager
                    }
                    // send the selected user's paramaters to the view and display the view:
                    if (Request.Browser.IsMobileDevice)
                    {
                      return View("Mobile_UpdateUser", SelectedUser);
                    }
                    return View(SelectedUser);
                }
            }
            //if the update is successful, redirect the manager to the manager zone:
            if (Request.Browser.IsMobileDevice)
            {
                return RedirectToAction("Mobile_ManagerPage");
            }
            return RedirectToAction("ManagerPage");
        }

        //this is the page that allows to add vehicle types:
        public ActionResult AddVehicleType()
        {
            if (Request.Browser.IsMobileDevice)
            {
                return View("Mobile_AddVehicleType");
            }
            return View();
        }

        //this is the page that allows to add vehicle types:
        [HttpPost]
        public ActionResult AddVehicleType(string VehicleType)
        {
            if (ModelState.IsValid)
            {
                // add the vehicle type to the database:
                DatabaseConnection.VehicleTypeTables.Add(new VehicleTypeTable
                {
                    VehicleType=VehicleType
                });
                try
                {
                DatabaseConnection.SaveChanges();
                }
                catch (Exception) // if adding the vehicle type fails
                {
                    ModelState.AddModelError("", "Vehicle Type already exists");  // send an error to the view to notify the manager
                    if (Request.Browser.IsMobileDevice)
                    {
                        return View("Mobile_AddVehicleType");
                    }
                    return View();
                }
            }
            if (Request.Browser.IsMobileDevice)
            {
                return RedirectToAction("Mobile_ManagerPage");
            }
            return RedirectToAction("ManagerPage");
        }

        //this is the page that allows to remove vehicle types:
        public ActionResult RemoveVehicleType()
        {
            // add each vehicle type from the database to a list and send it to the view:
            foreach (VehicleTypeTable item in DatabaseConnection.VehicleTypeTables)
            {
                VehicleTypeList.Add(item);
            }
            if (Request.Browser.IsMobileDevice)
            {
               return View("Mobile_RemoveVehicleType", VehicleTypeList);
            }
            return View(VehicleTypeList);
        }

        //this is the page that allows to remove vehicle types:
        [HttpPost]
        public ActionResult RemoveVehicleType(string SelectedType)
        {
            // find the selected vehicle type in the database:
            VehicleTypeTable RemoveType = DatabaseConnection.VehicleTypeTables.Where(type => type.VehicleType == SelectedType).SingleOrDefault();
            //check if there are any vehicles of the selected type that are currently hired:
            VehicleTable CheckHiredVehicles = DatabaseConnection.VehicleTables.Where(vehicle => vehicle.Available_for_Hire == false &&
                vehicle.Vehicle_Type == SelectedType).SingleOrDefault();
            if (CheckHiredVehicles != null) //if one or more vehicle of the selected type is currently hired
            {
                //deny the request and send an error to the view:
                ModelState.AddModelError("", "one or more vehicles of this type are currently hired, to remove this vehicle type you must first return these vehicles to the garage");
                // add each vehicle type from the database to a list and send it to the view:
                foreach (VehicleTypeTable item in DatabaseConnection.VehicleTypeTables)
                {
                    VehicleTypeList.Add(item);
                }
                if (Request.Browser.IsMobileDevice)
                {
                    return View("Mobile_RemoveVehicleType", VehicleTypeList);
                }
                return View(VehicleTypeList);
            }
            DatabaseConnection.VehicleTypeTables.Remove(RemoveType); // remove the selected vehicle type from the database

            //*********************** look for vehicles of the removed type in the database:
            foreach (VehicleTable vehicle in DatabaseConnection.VehicleTables)
            {
                if (vehicle.Vehicle_Type == SelectedType) // if a vehicle is of the same type that has been removed
                {
                    DatabaseConnection.VehicleTables.Remove(vehicle); // remove the vehicle from the database
                }
            }
            DatabaseConnection.SaveChanges();
            if (Request.Browser.IsMobileDevice)
            {
                return RedirectToAction("Mobile_ManagerPage");
            }
            return RedirectToAction("ManagerPage");
        }

        //this is the page that displays all vehicle types and allows to choose one to update:
        public ActionResult EditVehicleTypeList()
        {
            // add each vehicle type from the database to a list and send it to the view:
            foreach (VehicleTypeTable item in DatabaseConnection.VehicleTypeTables)
            {
                VehicleTypeList.Add(item);
            }
            if (Request.Browser.IsMobileDevice)
            {
                return View("Mobile_EditVehicleTypeList", VehicleTypeList); 
            }
            return View(VehicleTypeList);
        }

        //this is the page that displays all vehicle types and allows to choose one to update:
        [HttpPost]
        public ActionResult EditVehicleTypeList(string SelectedType)
        {
            // find the selected vehicle type in the database:
            VehicleTypeTable UpdatedType = DatabaseConnection.VehicleTypeTables.Where(type => type.VehicleType == SelectedType).SingleOrDefault();
            // send the vehicle type to  to the page that allows to edit vehicle types and display the page:
            if (Request.Browser.IsMobileDevice)
            {
               return View("Mobile_EditVehicleType", UpdatedType); 
            }
            return View("EditVehicleType", UpdatedType); 
        }

        //this is the page that allows to edit vehicle types:
        [HttpPost]
        public ActionResult EditVehicleType(string ChosenType, string VehicleType)
        {
            // find the selected vehicle type in the database:
            VehicleTypeTable SelectedType = DatabaseConnection.VehicleTypeTables.Where(type => type.VehicleType == ChosenType).SingleOrDefault();
            DatabaseConnection.VehicleTypeTables.Remove(SelectedType); // remove the vehicle type from the database
            // add a new vehicle type to replace the removed one:
            DatabaseConnection.VehicleTypeTables.Add(new VehicleTypeTable { VehicleType = VehicleType });
            //look for vehicles of the selected type in the database:
            foreach (VehicleTable vehicle in DatabaseConnection.VehicleTables)
            {
                if (vehicle.Vehicle_Type == ChosenType) // if the vehicle is of the same type as the selected type
                {
                    #region Update each vehicle of the same type
                    DatabaseConnection.VehicleTables.Add(new VehicleTable
            {
                //********************* add a new vehicle to the database with the same parameters with the only difference being the vehicle's type:
                //(the left ones are the database parameters and the right ones are the vehicle's parameters)
                Available_for_Hire = vehicle.Available_for_Hire,
                Branch = vehicle.Branch,
                Daily_Cost = vehicle.Daily_Cost,
                Fixed_for_Hire = vehicle.Fixed_for_Hire,
                Hiring_Period_End = vehicle.Hiring_Period_End,
                Hiring_Period_Start = vehicle.Hiring_Period_Start,
                Image = vehicle.Image,
                Late_Cost = vehicle.Late_Cost,
                Manufacturer = vehicle.Manufacturer,
                Mileage = vehicle.Mileage,
                Model = vehicle.Model,
                Name = vehicle.Name,
                Number = vehicle.Number,
                Transmission = vehicle.Transmission,
                Year = vehicle.Year,
                Vehicle_Type = VehicleType //the vehicle's type will be updated to the new type
            });
                    //**************** remove the original vehicle from the database:
                    DatabaseConnection.VehicleTables.Remove(vehicle);
                    #endregion
                } 
            }
            try
            {
                DatabaseConnection.SaveChanges();
            }
            catch (Exception) // if editing the vehicle type fails
            {
                //send an error to the view to notify the manager:
                ModelState.AddModelError("", "Vehicle Type already exists");
                // send the seleceted vehicle type to the view and display the view:
                if (Request.Browser.IsMobileDevice)
                {
                    return View("Mobile_EditVehicleType", SelectedType);
                }
                return View(SelectedType);
            }
            //if the vehicle type has been successfully updated, redirect the manager to the manager zone:
            if (Request.Browser.IsMobileDevice)
            {
                return RedirectToAction("Mobile_ManagerPage");
            }
            return RedirectToAction("ManagerPage");
        }

        //this is the page that allows to remove orders:
        public ActionResult RemoveInvitation()
        {
            // add each order from the database to a list and send it to the view:
            foreach (HiredVehiclesTable item in DatabaseConnection.HiredVehiclesTables)
            {
                HiredVehicles.Add(item);
            }
            if (Request.Browser.IsMobileDevice)
            {
                return View("Mobile_RemoveInvitation" ,HiredVehicles);
            }
            return View(HiredVehicles);
        }

        //this is the page that allows to remove orders:
        [HttpPost]
        public ActionResult RemoveInvitation(string VehicleName)
        {
            // find the hired vehicle by name in the database in the table of hired vehicles:
            HiredVehiclesTable SelectedVehicle = DatabaseConnection.HiredVehiclesTables.Where(vehicle => vehicle.HiredVehicleName == VehicleName).SingleOrDefault();
            // find the vehicle by name in the (non hired) vehicles table in the database (the vehicle will be marked as unavailble):
            VehicleTable UnavailableVehicle = DatabaseConnection.VehicleTables.Where(updt => updt.Name == VehicleName).SingleOrDefault();
            DatabaseConnection.VehicleTables.Add(new VehicleTable
            {
                //***************** add a new vehicle in the non hired vehicles table in the database:
                // the new vehicle's parameters will be the same as the old one's with the only difference being the Available for Hire = true,
                // since the original vehicle's Available for hire was marked as false because the vehicle has been hired
                //(the left ones are the database parameters and the right ones are parameters recieved from the View)
                Available_for_Hire = true,
                Branch = UnavailableVehicle.Branch,
                Daily_Cost = UnavailableVehicle.Daily_Cost,
                Fixed_for_Hire = UnavailableVehicle.Fixed_for_Hire,
                Hiring_Period_End = UnavailableVehicle.Hiring_Period_End,
                Hiring_Period_Start = UnavailableVehicle.Hiring_Period_Start,
                Image = UnavailableVehicle.Image,
                Late_Cost = UnavailableVehicle.Late_Cost,
                Manufacturer = UnavailableVehicle.Manufacturer,
                Mileage = UnavailableVehicle.Mileage,
                Model = UnavailableVehicle.Model,
                Name = UnavailableVehicle.Name,
                Number = UnavailableVehicle.Number,
                Transmission = UnavailableVehicle.Transmission,
                Vehicle_Type = UnavailableVehicle.Vehicle_Type,
                Year = UnavailableVehicle.Year
            });
            DatabaseConnection.HiredVehiclesTables.Remove(SelectedVehicle); // remove the selected hired vehicle from the table of orders database
            DatabaseConnection.VehicleTables.Remove(UnavailableVehicle); // remove the previous (marked as unavailable) vehicle in the vehicle table,
            // while the new, marked as available, vehicle added above will replace it
            DatabaseConnection.SaveChanges();
            if (Request.Browser.IsMobileDevice)
            {
                return RedirectToAction("Mobile_Home", "Home"); 
            }
            return RedirectToAction("HomePage", "Home");
        }

        //this is the page that shows all orders and allows to choose one to update:
        public ActionResult UpdateInvitationList()
        {
            // add each order from the database to a list and send it to the view:
            foreach (HiredVehiclesTable item in DatabaseConnection.HiredVehiclesTables)
            {
                HiredVehicles.Add(item);
            }
            if (Request.Browser.IsMobileDevice)
            {
                return View("Mobile_UpdateInvitationList", HiredVehicles);
            }
            return View(HiredVehicles);
        }

        //this is the page that shows all orders and allows to choose one to update:
        [HttpPost]
        public ActionResult UpdateInvitationList(string VehicleName)
        {
            //find the selected order by the vehicle's name and send it to the page that allows editing orders:
            HiredVehiclesTable SelectedVehicle = DatabaseConnection.HiredVehiclesTables.Where(vehicle => vehicle.HiredVehicleName == VehicleName).SingleOrDefault();
            if (Request.Browser.IsMobileDevice)
            {
                return View("Mobile_UpdateInvitation", SelectedVehicle);
            }
            return View("UpdateInvitation", SelectedVehicle);
        }

        //this is the page that allows to edit orders
        [HttpPost]
        public ActionResult UpdateInvitation(string VehicleName, string Start_of_Hiring, string End_of_Hiring, int Daily_Cost, int Late_Cost, int TotalCostValue)
        {
            // find the selected order in the database by the ordered vehicle's name:
            HiredVehiclesTable SelectedVehicle = DatabaseConnection.HiredVehiclesTables.Where(vehicle => vehicle.HiredVehicleName == VehicleName).SingleOrDefault();
            // add a new order (to replace the selected order) with the parameters recieved from the view:
            DatabaseConnection.HiredVehiclesTables.Add(new HiredVehiclesTable
            {
                //(the left ones are the database parameters and the right ones are parameters recieved from the View)
                HiredVehicleName = SelectedVehicle.HiredVehicleName,
                Hired_By_User = SelectedVehicle.Hired_By_User,
                Image = SelectedVehicle.Image,
                Number = SelectedVehicle.Number,
                Daily_Cost = Daily_Cost,
                Late_Cost = Late_Cost,
                Start_of_Hiring = Start_of_Hiring,
                End_of_Hiring = End_of_Hiring,
                Total_Cost = TotalCostValue
            });
            // remove the selected order from the database:
            DatabaseConnection.HiredVehiclesTables.Remove(SelectedVehicle);
            DatabaseConnection.SaveChanges();
            if (Request.Browser.IsMobileDevice)
            {
                return RedirectToAction("Mobile_Home", "Home");
            }
            return RedirectToAction("HomePage", "Home");
        }
        // this page is the manager's zone:
        public ActionResult ManagerPage()
        {
            return View();
        }
        // the manager's zone for the mobile version:
        public ActionResult Mobile_ManagerPage()
        {
            return View();
        }
    }
}