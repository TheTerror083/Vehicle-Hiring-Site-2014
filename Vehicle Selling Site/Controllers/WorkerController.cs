using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vehicle_Selling_Site.Models;

namespace Vehicle_Selling_Site.Controllers
{
    [Authorize(Users="Worker,Admin")] //allow access only to users of type "Admin" or "Worker"
    public class WorkerController : Controller
    {
        // GET: Worker
        public SiteDatabaseEntities DatabaseConnection = new SiteDatabaseEntities(); //a connection to the database
        public List<ContactTable> Messages = new List<ContactTable>(); // a list of messages sent from users

        //the worker's zone:
        public ActionResult WorkerPage()
        {
            return View();
        }

        //the worker's zone (for mobile):
        public ActionResult Mobile_WorkerPage()
        {
            return View();
        }

        //the page that shows a list of all messages sent from the users
        public ActionResult ReadMessages()
        {
            //load each message from the database to a list and send it to the view:
            foreach (ContactTable item in DatabaseConnection.ContactTables)
            {
                Messages.Add(item);
            }
            if (Request.Browser.IsMobileDevice)
            {
                return View("Mobile_ReadMessages", Messages);
            }
            return View(Messages);
        }

        //function used to delete a message from the list:
        [HttpPost]
        public ActionResult ReadMessages(int MessageID)
        {
            //find the selected message in the database by its ID and remove it:
            ContactTable RemoveMessage = DatabaseConnection.ContactTables.Where(msg => msg.Message_ID == MessageID).SingleOrDefault();
            DatabaseConnection.ContactTables.Remove(RemoveMessage);
            DatabaseConnection.SaveChanges();
            if (Request.Browser.IsMobileDevice)
            {
                return RedirectToAction("Mobile_Home", "Home");
            }
            return RedirectToAction("HomePage", "Home");
        }

        //allows to search for a hired vehicle by entering its number:
        public ActionResult ViewHiredVehicles()
        {
            if (Request.Browser.IsMobileDevice)
            {
                return View("Mobile_ViewHiredVehicles");
            }
            return View();
        }

        //return the hired vehicle to the garage:
        [HttpPost]
        public ActionResult ViewHiredVehicles(string VehicleName)
        {
            //find the hired vehicle by its name in the list of orders:
            HiredVehiclesTable SelectedVehicle = DatabaseConnection.HiredVehiclesTables.Where(vehicle => vehicle.HiredVehicleName == VehicleName).SingleOrDefault();
            //find the vehicle in the VehicleTable by its name:
            VehicleTable UnavailableVehicle = DatabaseConnection.VehicleTables.Where(updt => updt.Name == VehicleName).SingleOrDefault();
            DatabaseConnection.VehicleTables.Add(new VehicleTable
            {
                //add a new vehicle to the database with the same parameters as the selected vehicle's,
                //but with "Available for hire" marked as true since the vehicle has been returned to the garage and is available for hiring:
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
            //remove the vehicle from the table of hired vehicles:
            DatabaseConnection.HiredVehiclesTables.Remove(SelectedVehicle);
            // remove the unavailble vehicle from the table of vehicles:
            DatabaseConnection.VehicleTables.Remove(UnavailableVehicle);
            DatabaseConnection.SaveChanges();
            if (Request.Browser.IsMobileDevice)
            {
                return RedirectToAction("Mobile_Home", "Home");
            }
            return RedirectToAction("HomePage", "Home");
        }

        //find a hired vehicle after a number has been entered:
        [HttpPost]
        public ActionResult Search_Hired_Vehicle_Number(string Number)
        {
            //find the hired vehicle by its number in the database:
            HiredVehiclesTable HiredVehicle = DatabaseConnection.HiredVehiclesTables.Where(vehicle => vehicle.Number == Number).SingleOrDefault();
            if (Request.Browser.IsMobileDevice)
            {
                return View("Mobile_ViewHiredVehicles", HiredVehicle);
            }
            return View("ViewHiredVehicles", HiredVehicle);
        }
    }
}