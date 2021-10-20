using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vehicle_Selling_Site.Models
{
    public class AddVehicleClass
    {
        // *************************************** Vehicle's database parameters:
        public string Name { get; set; }
        public string Year { get; set; }
        public int Mileage { get; set; }
        public string Transmission { get; set; }
        public bool Available_for_Hire { get; set; }
        public bool Fixed_for_Hire { get; set; }
        public string Number { get; set; }
        public string Branch { get; set; }
        public string Hiring_Period_Start { get; set; }
        public string Hiring_Period_End { get; set; }
        public string Model { get; set; }
        public string Manufacturer { get; set; }
        public int Daily_Cost { get; set; }
        public int Late_Cost { get; set; }
        public string Image { get; set; }
        public string Vehicle_Type { get; set; }
        //******************************************************************

        public List<VehicleTypeTable> VehicleTypes = new List<VehicleTypeTable>(); // a list of vehicle types
    }
}