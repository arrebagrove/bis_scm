using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scm.Common
{
    public class Instance
    {
        public int Vehicles { get; set; }
        public int Customers { get; set; }
        public int Depots { get; set; }

        public int MaxVehicleLoad { get; set; }

        public List<Vehicle> VehicleList { get; set; }
        public List<Customer> CustomerList { get; set; }
        public List<Depot> DepotList { get; set; }

        public Instance()
        {
            VehicleList = new List<Vehicle>();
            CustomerList = new List<Customer>();
            DepotList = new List<Depot>();
        }
    }
}
