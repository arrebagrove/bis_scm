using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scm.Common
{
    public class Tour
    {
        public string DepotName { get; set; }
        public string VehicleNumber { get; set; }
        public double Costs { get; set; }
        public int VehicleLoad { get; set; }
        public List<string> Route { get; set; }
    }
}
