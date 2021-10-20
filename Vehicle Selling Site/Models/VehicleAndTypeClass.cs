using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vehicle_Selling_Site.Models
{
    public class VehicleAndTypeClass
    {
        public List<VehicleTable> LoadVehicles = new List<VehicleTable>(); // a list of vehicles
        public List<VehicleTypeTable> LoadVehicleTypes = new List<VehicleTypeTable>(); // a list of vehicle types

    }
}