using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scm.Common
{
    public class InstanceSolution
    {
        public List<Tour> Tour { get; set; }
        public double TotalCosts { get; set; }

        public InstanceSolution()
        {
            Tour = new List<Tour>();
        }
    }
}
