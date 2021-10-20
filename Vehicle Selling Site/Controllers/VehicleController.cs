using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vehicle_Selling_Site.Models;

namespace Vehicle_Selling_Site.Controllers
{
    public class VehicleController : Controller
    {
        SiteDatabaseEntities DatabaseConnection = new SiteDatabaseEntities(); // the connection to the database
        //an object of a model that contains a list of vehicles and a list of vehicle types:
        VehicleAndTypeClass Vehicles = new VehicleAndTypeClass();

        // GET: Vehicle
        //the page that displays vehicles available for hiring:
        public ActionResult HireVehicle()
        {
            // load each vehicle from the database to a list and send it to the view:
            foreach (VehicleTable item in DatabaseConnection.VehicleTables)
            {
                if (item.Available_for_Hire==true)
                {
                    Vehicles.LoadVehicles.Add(item);
                }
            }
            // load each vehicle type from the database to a list and send it to the view:
            foreach (VehicleTypeTable item in DatabaseConnection.VehicleTypeTables)
            {
                Vehicles.LoadVehicleTypes.Add(item);
            }
            return View(Vehicles);
        }

        //the page that displays vehicles available for hiring (for mobile):
        public ActionResult Mobile_HireVehicle()
        {
            // load each vehicle from the database to a list and send it to the view:
            foreach (VehicleTable item in DatabaseConnection.VehicleTables)
            {
                if (item.Available_for_Hire == true)
                {
                    Vehicles.LoadVehicles.Add(item);
                }
            }
            // load each vehicle type from the database to a list and send it to the view:
            foreach (VehicleTypeTable item in DatabaseConnection.VehicleTypeTables)
            {
                Vehicles.LoadVehicleTypes.Add(item);
            }
            return View(Vehicles);
        }

        // the function that displays search results on the "HireVehicle" page:
        [HttpPost]
        public ActionResult HireVehicleSearch(string SearchText,string VehicleType, string SearchYear,string TransmissionType, string StartDate,
            string EndDate)
        {
            foreach (VehicleTable item in DatabaseConnection.VehicleTables) //load each vehicle from the database
            {
                if (item.Available_for_Hire == true) // if the current vehicle is available for hire
                {
                    //if the vehicle's year is the year the user has entered and the vehicle's name contains the free search text:
                    if (item.Year == SearchYear && item.Name.Contains(SearchText)) 
                    {
                        //if the vehicle type is of the selected type or if the user has chosen "any" type of vehicle:
                        if (item.Vehicle_Type == VehicleType || VehicleType == "Any")
                        {
                            //if the vehicle's transmission is of the selected type or if the user has chosen "any" vehicle transmission: 
                            if (item.Transmission == TransmissionType || TransmissionType == "Any")
                            {
                                //add the vehicle to the list of vehicles (the loop will continue to the next vehicle)
                                Vehicles.LoadVehicles.Add(item);
                            }
                        }
                    }
                    //if the entered startdate is the start of the vehicle's availability period and the vehicle's name contains the free search text:
                    else if (item.Hiring_Period_Start == StartDate && item.Name.Contains(SearchText))
                    {
                        //if the vehicle type is of the selected type or if the user has chosen "any" type of vehicle:
                        if (item.Vehicle_Type == VehicleType || VehicleType == "Any")
                        {
                            //if the vehicle's transmission is of the selected type or if the user has chosen "any" vehicle transmission: 
                            if (item.Transmission == TransmissionType || TransmissionType == "Any")
                            {
                                //add the vehicle to the list of vehicles (the loop will continue to the next vehicle)
                                Vehicles.LoadVehicles.Add(item);
                            }
                        }
                    }
                    //if the entered enddate is the end of the vehicle's availability period and the vehicle's name contains the free search text:
                    else if (item.Hiring_Period_End == EndDate && item.Name.Contains(SearchText))
                    {
                        //if the vehicle type is of the selected type or if the user has chosen "any" type of vehicle:
                        if (item.Vehicle_Type == VehicleType || VehicleType == "Any")
                        {
                            //if the vehicle's transmission is of the selected type or if the user has chosen "any" vehicle transmission: 
                            if (item.Transmission == TransmissionType || TransmissionType == "Any")
                            {
                                //add the vehicle to the list of vehicles (the loop will continue to the next vehicle)
                                Vehicles.LoadVehicles.Add(item);
                            }
                        }
                    }

                    else if (StartDate.Length > 1 && EndDate.Length > 1) //if the user has entered dates in the start and end date boxes
                    {
                        //convert the date entered at the startdate box to a datetime object: (the null means the date will use the local culture info)
                        string DateFormat = "";
                        //there are 2 available date formats one is html ("MM/dd/yyyy") and one is mobile ("yyyy-MM-dd"):
                        if (StartDate.Contains("/"))// check if the entered date is of format "MM/dd/yyyy" (html format)
                        {
                           DateFormat = "MM/dd/yyyy"; // set the date format to match the entered date 
                        }

                        if (StartDate.Contains("-"))// check if the entered date is of format "yyyy-MM-dd" (mobile format)
                        {
                            DateFormat = "yyyy-MM-dd"; // set the date format to match the entered date 
                        }
                        DateTime MinDate = DateTime.ParseExact(StartDate, DateFormat, null);
                        //convert the date entered at the enddate box to a datetime object:
                        DateTime MaxDate = DateTime.ParseExact(EndDate, DateFormat, null);

                        //read the start date of the vehicle's availability period and convert it to a datetime object:
                        if (item.Hiring_Period_Start.Contains("/")) // if the date is of the Html format ("MM/dd/yyyy")
                        {
                            DateFormat = "MM/dd/yyyy"; // set the date format to match the date
                        }
                        if (item.Hiring_Period_Start.Contains("-")) // if the date is of the Mobile format ("yyyy-MM-dd")
                        {
                            DateFormat = "yyyy-MM-dd"; // set the date format to match the date
                        }

                        //get the start date of the vehicle's availability period:
                        DateTime VehicleHireDate = DateTime.ParseExact(item.Hiring_Period_Start, DateFormat, null);

                        //read the end date of the vehicle's availability period and convert it to a datetime object:
                        if (item.Hiring_Period_End.Contains("/")) // if the date is of the Html format ("MM/dd/yyyy")
                        {
                            DateFormat = "MM/dd/yyyy"; // set the date format to match the date
                        }
                        if (item.Hiring_Period_End.Contains("-")) // if the date is of the Mobile format ("yyyy-MM-dd")
                        {
                            DateFormat = "yyyy-MM-dd"; // set the date format to match the date
                        }

                        //get the end date of the vehicle's availability period:
                        DateTime VehicleHireEndDate = DateTime.ParseExact(item.Hiring_Period_End, DateFormat, null);

                        while (MinDate < MaxDate) //while the enddate is bigger than the startdate
                        {
                            if (MinDate >= VehicleHireDate && MinDate <= VehicleHireEndDate) //if the current date (startdate) is between the vehicle's availability period
                            {
                                //if the vehicle type is of the selected type or if the user has chosen "any" type of vehicle:
                                if (item.Vehicle_Type == VehicleType || VehicleType == "Any")
                                {
                                    //if the vehicle's transmission is of the selected type or if the user has chosen "any" vehicle transmission: 
                                    if (item.Transmission == TransmissionType || TransmissionType == "Any")
                                    {
                                        //the vehicle will be added to the list of vehicles and this loop will break
                                        Vehicles.LoadVehicles.Add(item);
                                        break;
                                    }
                                }
                            }
                            //if one of the above comparsions fail, add 1 day to the startdate and the loop will continue
                            MinDate = MinDate.AddDays(1);
                        }
                    }
                        //if the vehicle's name contatins the free text search and the user hasn't entered any
                    else if (item.Name.Contains(SearchText) && StartDate.Length < 1 && EndDate.Length < 1 )
                    {
                        //if the vehicle type is of the selected type or if the user has chosen "any" type of vehicle:
                        if (item.Vehicle_Type == VehicleType || VehicleType == "Any")
                        {
                            //if the vehicle's transmission is of the selected type or if the user has chosen "any" vehicle transmission: 
                            if (item.Transmission == TransmissionType || TransmissionType == "Any")
                            {
                                //add the vehicle to the list of vehicles (the loop will continue to the next vehicle)
                                Vehicles.LoadVehicles.Add(item);
                            }
                        }
                    }
                }
            }
            // load each vehicle type from the database to a list and send it to the view:
            foreach (VehicleTypeTable item in DatabaseConnection.VehicleTypeTables)
            {
                Vehicles.LoadVehicleTypes.Add(item);
            }

            if (Vehicles.LoadVehicles.Count == 0) //if no vehicles were found
            {
                ModelState.AddModelError("", "Sorry, no results were found"); // send a message to the view as an error
            }

            if (Request.Browser.IsMobileDevice)
            {
                return View("Mobile_HireVehicle", Vehicles);
            }
            return View("HireVehicle", Vehicles);
        }

        //redirects the user to the "ConfirmHiring" page after choosing a vehicle:
        [HttpPost]
        public ActionResult HireVehicle(string carName)
        {
            //find the selected vehicle by its name in the database and sendit to the "ConfirmHiring" page:
            VehicleTable SelectedVehicle = DatabaseConnection.VehicleTables.Where(model => model.Name == carName).SingleOrDefault();
            return View("ConfirmHiring", SelectedVehicle); //send the parameters to the page that displays the hiring summary
        }

        //redirects the user to the "ConfirmHiring" page after choosing a vehicle (for mobile):
        [HttpPost]
        public ActionResult Mobile_HireVehicle(string carName)
        {
            //find the selected vehicle by its name in the database and sendit to the "ConfirmHiring" page:
            VehicleTable SelectedVehicle = DatabaseConnection.VehicleTables.Where(model => model.Name == carName).SingleOrDefault();
            return View("Mobile_ConfirmHiring", SelectedVehicle); //send the parameters to the page that displays the hiring summary
        }

        //the page that displays the chosen vehicle for hiring:
        [Authorize] // only logged in users can hire vehicles!
        [HttpPost]
        public ActionResult ConfirmHiring(string carName, int TotalCostValue, string HiringStart, string HiringEndDate, string HiringUser)
        {
            // find the selected vehicle in the database by its name:
            VehicleTable SelectedVehicle = DatabaseConnection.VehicleTables.Where(vehicle => vehicle.Name == carName).SingleOrDefault();
            DatabaseConnection.VehicleTables.Add(new VehicleTable
            {
                //add a new vehicle to the database with the same parameters as the selected vehicle's,
                //but with "Available for hire" marked as false since the vehicle has been hired:
                //(the left ones are the database parameters and the right ones are parameters recieved from the View)
                Available_for_Hire = false,
                Branch = SelectedVehicle.Branch,
                Daily_Cost = SelectedVehicle.Daily_Cost,
                Fixed_for_Hire = SelectedVehicle.Fixed_for_Hire,
                Hiring_Period_End = SelectedVehicle.Hiring_Period_End,
                Hiring_Period_Start = SelectedVehicle.Hiring_Period_Start,
                Image = SelectedVehicle.Image,
                Late_Cost = SelectedVehicle.Late_Cost,
                Manufacturer = SelectedVehicle.Manufacturer,
                Mileage = SelectedVehicle.Mileage,
                Model = SelectedVehicle.Model,
                Name = SelectedVehicle.Name,
                Number = SelectedVehicle.Number,
                Transmission = SelectedVehicle.Transmission,
                Vehicle_Type = SelectedVehicle.Vehicle_Type,
                Year = SelectedVehicle.Year
            });
            DatabaseConnection.HiredVehiclesTables.Add(new HiredVehiclesTable
            {
                //add the selected vehicle's parameters combined with the parameters sent from the view to the list of orders:
                //(the left ones are the database parameters and the right ones are parameters recieved from the View)
                HiredVehicleName = SelectedVehicle.Name,
                Total_Cost = TotalCostValue,
                End_of_Hiring = HiringEndDate,
                Start_of_Hiring = HiringStart,
                Image = SelectedVehicle.Image,
                Late_Cost = SelectedVehicle.Late_Cost,
                Daily_Cost = SelectedVehicle.Daily_Cost,
                Number = SelectedVehicle.Number,
                Hired_By_User = HiringUser
            });
            DatabaseConnection.Hired_Vehicles_History.Add(new Hired_Vehicles_History
            {
                //add the order's parameters to the history of hired vehicles table:
                //(the left ones are the database parameters and the right ones are parameters recieved from the View)
                Hired_By_User = HiringUser,
                Name = SelectedVehicle.Name,
                Daily_Cost = SelectedVehicle.Daily_Cost,
                Number = SelectedVehicle.Number,
                Image = SelectedVehicle.Image,
                Total_Cost = TotalCostValue,
                Start_of_Hiring = HiringStart,
                End_of_Hiring = HiringEndDate
            });
            DatabaseConnection.VehicleTables.Remove(SelectedVehicle);
            DatabaseConnection.SaveChanges();
            if (Request.Browser.IsMobileDevice)
            {
                return RedirectToAction("Mobile_ThanksForHiring");     
            }
            return RedirectToAction("ThanksForHiring");
        }

        //this function sends the vehicle names recieved from the layout page to the "PreviouslyViewedVehicles" partial view to display the vehiclesthe user has previously viewed:
        public PartialViewResult PreviouslyViewedVehicles(string carName)
        {
            try
            {
                //find the vehicle by its name in the database:
                VehicleTable PreviousVehicle = DatabaseConnection.VehicleTables.Where(vehicle => vehicle.Name == carName).SingleOrDefault();
                return PartialView(PreviousVehicle); //send it to the partial view
            }
            catch (Exception) //in case the vehicle is not found in the database an empty partial view will be returned
            {
                return PartialView();
            }
        }

        //this function redirects the user to the "ConfirmHiring" page after clicking on a vehicle from the list of previously viewed vehicles:
        [HttpPost]
        public ActionResult PreviouslyViewedVehicles_Redirect(string Redirect_To_Vehicle)
        {
            //find the vehicle by name in the database:
            VehicleTable PreviousVehicle = DatabaseConnection.VehicleTables.Where(vehicle => vehicle.Name == Redirect_To_Vehicle).SingleOrDefault();
            if (Request.Browser.IsMobileDevice)
            {
                return View("Mobile_ConfirmHiring", PreviousVehicle);
            }
            return View("ConfirmHiring", PreviousVehicle);
        }

        // this page appears to the user after he\she has hired a vehicle:
        public ActionResult ThanksForHiring()
        {
            return View();
        }
        // this page appears to the user after he\she has hired a vehicle (for mobile):
        public ActionResult Mobile_ThanksForHiring()
        {
            return View();
        }
    }
}