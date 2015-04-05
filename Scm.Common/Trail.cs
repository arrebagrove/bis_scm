using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scm.Common
{
    public class Trail
    {
        private double _costs = double.NaN;

        public Point[] Points { get; set; }

        public double Costs
        {
            get
            {
                if (double.IsNaN(_costs))
                {
                    for (var p = 0; p < Points.Length - 1; p++)
                    {
                        var dX = Points[p].X - Points[p + 1].X;
                        var dY = Points[p].Y - Points[p + 1].Y;

                        _costs = Math.Sqrt(Math.Pow(dX, 2) + Math.Pow(dY, 2));
                    }
                }

                return _costs;
            }
        }

        public void ResetCosts()
        {
            _costs = double.NaN;
        }
    }
}
